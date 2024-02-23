using System;
using System.IO;
using PlasticGui;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace LogViewer
{
    public class LogViewerUITK : EditorWindow
    {
        [SerializeField] public VisualTreeAsset m_LogViewerView;
        [SerializeField] public string lastOpenedLogPath;

        [SerializeField] public LogFile.EventTypes visibleEventTypes;

        [SerializeField]
        public int selectedLogIndex;
        
        [MenuItem("Window/Analysis/Log Viewer (UITK)")]
        public static void Open()
        {
            GetWindow<LogViewerUITK>("Log Viewer (UITK)");
        }

        private LogViewerController viewer;

        public void OnEnable()
        {
            var view = m_LogViewerView.Instantiate();
            rootVisualElement.Add(view);

            viewer = new LogViewerController(view);

            view.style.width = new Length(100, LengthUnit.Percent);
            view.style.height = new Length(100, LengthUnit.Percent);

            view.RegisterCallback<DragUpdatedEvent>(evt =>
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            });


            view.RegisterCallback<DragPerformEvent>(evt =>
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                
                foreach (string path in DragAndDrop.paths)
                {
                    if(Load(path))
                        break;
                }
            });

            if (lastOpenedLogPath != null)
            {
                Load(lastOpenedLogPath);

                viewer.SetViewParameters(visibleEventTypes, selectedLogIndex);

            }
        }

        bool Load(string path)
        {
            if (File.Exists(path))
            {
                viewer.SetLog(LogFile.LoadFromFile(path));
                lastOpenedLogPath = path;
                titleContent = new GUIContent(Path.GetFileName(path));
                return true;
            }

            return false;
        }

        private void OnDisable()
        {
            visibleEventTypes = viewer.visibleEventTypes;
            selectedLogIndex = viewer.selectedLogIndex;
        }
    }
}

