using System.IO;
using UnityEditor;
using UnityEngine;

namespace LogViewer
{
    public class LogViewerWindow : EditorWindow
    {
        [MenuItem("Window/Analysis/Log Viewer")]
        public static void Open()
        {
            GetWindow<LogViewerWindow>("Log Viewer");
        }

        [SerializeField] public string lastOpenedLogPath;
      
        [SerializeField]
        public int selectedLogIndex;

        private LogViewerOnGUI viewer;

        private void OnEnable()
        {
            Styles.Initialize();

            viewer = new LogViewerOnGUI();

            if (lastOpenedLogPath != null)
            {
                if (File.Exists(lastOpenedLogPath))
                {
                    Load(lastOpenedLogPath);
                }
            }
        }

        private void Load(string path)
        {
            viewer.SetLog(LogFile.LoadFromFile(path), selectedLogIndex);
            titleContent = new GUIContent(Path.GetFileName(path));
            lastOpenedLogPath = path;
        }

        private void OnDisable()
        {
            selectedLogIndex = viewer.selectedLogIndex;
            Styles.Deinitialize();
        }

        private void OnGUI()
        {
            Event currentEvent = Event.current;

            if (currentEvent.type == EventType.DragPerform || currentEvent.type == EventType.DragUpdated)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (currentEvent.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    foreach (string path in DragAndDrop.paths)
                    {
                        if (File.Exists(path))
                        {
                            selectedLogIndex = -1;
                            Load(path);
                            break;
                        }
                    }
                }
            }

            viewer.OnGUI();
        }
    }
}
