using UnityEngine;

namespace LogViewer
{
    public abstract class AbstractLogViewer
    {
        public void SetLog(LogFile logFile)
        {
            this.logFile = logFile;

            selectedLogIndex = -1;
            OnLogfileLoaded();
        }

        protected abstract void OnLogfileLoaded();

        protected LogFile logFile;
        public LogFile.EventTypes visibleEventTypes = (LogFile.EventTypes)~0;

        public int selectedLogIndex  
        {
            get;
            protected set;
        }

        public bool showMessages
        {
            get => (visibleEventTypes & LogFile.EventTypes.Message) != 0;
            set
            {
                if (value)
                {
                    visibleEventTypes |= LogFile.EventTypes.Message;
                }
                else
                {
                    visibleEventTypes &= ~LogFile.EventTypes.Message;
                }
            }
        }

        public bool showErrors
        {
            get => (visibleEventTypes & LogFile.EventTypes.Error) != 0;
            set
            {
                if (value)
                {
                    visibleEventTypes |= LogFile.EventTypes.Error;
                }
                else
                {
                    visibleEventTypes &= ~LogFile.EventTypes.Error;
                }
            }
        }
    }
    
    public class LogViewerOnGUI : AbstractLogViewer
    {
        private Vector2 logListScrollPosition, logScrollPosition;

        protected override void OnLogfileLoaded()
        {
            logListScrollPosition = Vector2.zero;
            logScrollPosition = Vector2.zero;
        }

        
        public void OnGUI()
        {
            GUILayout.BeginVertical(GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));

            if (logFile != null)
            {
                Formatted();
            }
            else
            {
                Pending();
            }

            GUILayout.EndVertical();
        }

        private void Pending()
        {
            GUILayout.Label("Drop a <b>Player.log</b> here", Styles.CenteredMessage, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
        }

        private void Formatted()
        {
            GUILayout.BeginHorizontal();

            int messageCount = logFile.MessageCount;
            int errorCount = logFile.ErrorCount;

            GUIStyle messageStyle = showMessages ? Styles.MiniButtonSelected : Styles.MiniButton;
            GUIStyle errorStyle = showErrors ? Styles.MiniButtonSelected : Styles.MiniButton;

            if (GUILayout.Button(new GUIContent(messageCount.ToString(), Styles.iconInfo.image), messageStyle, GUILayout.MinWidth(42), GUILayout.ExpandWidth(false)))
                visibleEventTypes ^= LogFile.EventTypes.Message;

            GUILayout.Space(2);

            if (GUILayout.Button(new GUIContent(errorCount.ToString(), Styles.iconError.image), errorStyle, GUILayout.MinWidth(42), GUILayout.ExpandWidth(false)))
                visibleEventTypes ^= LogFile.EventTypes.Error;

            GUILayout.EndHorizontal();

            logListScrollPosition = GUILayout.BeginScrollView(logListScrollPosition, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));

            string selectedLogContents = string.Empty;

            if (selectedLogIndex != -1)
            {
                selectedLogContents = logFile.Events[selectedLogIndex].Content;
            }

            for (int i = 0; i < logFile.Events.Count; i++)
            {
                LogFile.Event log = logFile.Events[i];

                if ((log.EventType & visibleEventTypes) == 0)
                    continue;

                GUIStyle style;

                if (selectedLogIndex == i)
                    style = Styles.SelectedBackground;
                else
                    style = i % 2 == 0 ? Styles.EvenBackground : Styles.OddBackground;

                GUILayout.BeginHorizontal(style, GUILayout.Height(40));

                bool clicked = false;

                if (log.EventType == LogFile.EventTypes.Message)
                    clicked = GUILayout.Button(Styles.iconInfo, Styles.IconStyle) | clicked;
                else
                    clicked = GUILayout.Button(Styles.iconError, Styles.IconStyle) | clicked;

                clicked = GUILayout.Button(log.Summary, style, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)) | clicked;

                if (clicked)
                {
                    selectedLogIndex = i;
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            logScrollPosition = GUILayout.BeginScrollView(logScrollPosition, GUILayout.Height(256));

            GUILayout.TextArea(selectedLogContents, Styles.MessageStyle);
            GUILayout.EndScrollView();
        }
    }
}
