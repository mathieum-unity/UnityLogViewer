using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace LogViewer
{
    public class LogListItem: VisualElement
    {
        private VisualElement icon;
        private Label summary;

        private static string className = "log-list-item";
        private static string iconClassName = className + "__icon";
        private static string summaryClassName = className + "__summary";
        
        private static string messageIconClassName = "icon--message";
        private static string errorIconClassName = "icon--error";

        public LogListItem()
        {
            AddToClassList(className);
            icon = new VisualElement();
            icon.AddToClassList(iconClassName);
            Add(icon);

            summary = new Label();
            summary.AddToClassList(summaryClassName);
            
            Add(summary);
        }

        public void DisplayLogSummary(LogFile.Event evt)
        {
            icon.EnableInClassList(messageIconClassName, evt.EventType == LogFile.EventTypes.Message);
            icon.EnableInClassList(errorIconClassName, evt.EventType == LogFile.EventTypes.Error);
            
            summary.text = evt.Summary;
        }
    }
    public class LogViewerController :  AbstractLogViewer
    {
        private VisualElement view;

        private Button messageButton;
        private Button errorButton;
        private TextElement messageCount;
        private TextElement errorCount;

        private ListView logList;

        private TextElement logEntryContent;
        
        // 2 base containers

        private VisualElement empty;
        private VisualElement logs;
        
        private const string visibleClassName = "visible";
        private const string hiddenClassName= "hidden";
        private const string toggleButtonDeselected = "top-bar-button--unselected";


        public LogViewerController(VisualElement viewRoot)
        {
            this.view = viewRoot;

            messageButton = view.Q<Button>("message-button");
            errorButton = view.Q<Button>("error-button");

            messageCount = view.Q<TextElement>("message-count");
            errorCount = view.Q<TextElement>("error-count");

            logList = view.Q<ListView>("log-list");
            logEntryContent = view.Q<TextElement>("log-contents");

            empty = view.Q("empty-view");
            logs = view.Q("log-view");
            
            logList.makeItem = () =>
            {
                // we have to put our element inside a wrapper as the ListView changes the inline styles
                // of our LogListItem and the padding-top gets overridden
                var dummy = new VisualElement();
                dummy.Add(new LogListItem());
                return dummy;
            };
            logList.bindItem = ShowFilteredLog;
            
            #if !UNITY_2021_2_OR_NEWER
            logList.itemHeight = 40;
            #endif
           
            #if UNITY_2022_2_OR_NEWER
            logList.selectedIndicesChanged += indices =>
            {
                int logIndex = -1;
                foreach (var i in indices)
                {
                    logIndex = i;
                    break;
                }

                OnLogItemSelected(logIndex);
            };
            #else
            logList.onSelectionChange += indices =>
            {
                int logIndex = -1;
                foreach (var i in indices)
                {
                    logIndex = (int)i;
                    break;
                }

                OnLogItemSelected(logIndex);
            };
            #endif
            
            SetEmpty(true);

            messageButton.clicked += () => ToggleLogVisibility(LogFile.EventTypes.Message);
            errorButton.clicked += () => ToggleLogVisibility(LogFile.EventTypes.Error);
        }

        // A bit of a dumb way to do it, but ¯\_(ツ)_/¯
        void SetEmpty(bool isEmpty)
        {
            empty?.EnableInClassList(visibleClassName, isEmpty);
            empty?.EnableInClassList(hiddenClassName, !isEmpty);

            logs?.EnableInClassList(visibleClassName, !isEmpty);
            logs?.EnableInClassList(hiddenClassName, isEmpty);
        }

        void ToggleLogVisibility(LogFile.EventTypes logType)
        {
            visibleEventTypes ^= logType;
            UpdateToggleButtons();
            UpdateListContents();
        }
        
        private List<int> filteredLogs = new List<int>();
        void UpdateListContents()
        {
            //this is not the most performant thing, but it's the simplest for now
            filteredLogs.Clear();

            var events = logFile.Events;
            var eventCount = events.Count;
            
            for (int i = 0; i < eventCount; i++)
            {
                var log = events[i];
                if ((log.EventType & visibleEventTypes) == 0)
                    continue;
                filteredLogs.Add(i);
            }

            logList.itemsSource = filteredLogs;
            
            if (selectedLogIndex >= 0)
            {
                if ((events[selectedLogIndex].EventType & visibleEventTypes) == 0)
                {
                    OnLogItemSelected(-1);
                }
            }
            
            #if UNITY_2021_2_OR_NEWER
            logList.RefreshItems();
            #else
            logList.Refresh();
            #endif
        }

        private void ShowFilteredLog(VisualElement element, int filteredIndex)
        {
            var itemView = element.Q<LogListItem>();

            var index = filteredLogs[filteredIndex];
            itemView?.DisplayLogSummary(logFile.Events[index]);
        }

        public void SetViewParameters(LogFile.EventTypes visibleTypes, int selectedIndex)
        {
            visibleEventTypes = visibleTypes;
            
            if (logFile != null && filteredLogs != null)
            {
                logList.selectedIndex = filteredLogs.IndexOf(selectedIndex);
            }else
            {
                selectedLogIndex = selectedIndex;
            }
        }
        private void OnLogItemSelected(int logIndex)
        {
                selectedLogIndex = logIndex;
               
                logEntryContent.text =
                    (selectedLogIndex >= 0) ? logFile.Events[selectedLogIndex].Content : string.Empty;
        }
        protected override void OnLogfileLoaded()
        {
            if (logFile != null)
            {
                SetEmpty(false);

                messageCount.text = "" + logFile.MessageCount;
                errorCount.text = "" + logFile.ErrorCount;

                UpdateToggleButtons();
                UpdateListContents();
            }
            else
            {
                SetEmpty(true);
            }
        }
        
        void UpdateToggleButtons()
        {
            messageButton.EnableInClassList(toggleButtonDeselected, !showMessages);
            errorButton.EnableInClassList(toggleButtonDeselected, !showErrors);
        }
    }
}
