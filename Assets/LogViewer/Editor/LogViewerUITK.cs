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

#if ENABLE_CACHE_STATS
        private Label uitkHitsPercent;
        private Label imguiHitsPercent;

        void UpdatePercent(Label lbl, int hit, int miss)
        {
            var total = hit + miss;

            if (hit + miss > 0)
            {
                float v = hit / (float)(hit + miss);

                lbl.text = "" + (int)(v* 100);
            }
            else
            {
                lbl.text = "---";
            }
        }
        void UpdatePercent()
        {
            UpdatePercent(uitkHitsPercent, TextHandleCacheStats.UITK_Hits, TextHandleCacheStats.UITK_Miss);
            UpdatePercent(imguiHitsPercent, TextHandleCacheStats.IMGUI_Hits, TextHandleCacheStats.IMGUI_Miss);
        }
#endif

        public void OnEnable()
        {

#if ENABLE_CACHE_STATS
            var stats = new VisualElement();
            stats.style.flexDirection = FlexDirection.Row;
            uitkHitsPercent = new Label();
            imguiHitsPercent = new Label();

            uitkHitsPercent.style.width = 100;
            imguiHitsPercent.style.width = 100;
            
            stats.Add(new Label("UITK Text cache Hits"));
            stats.Add(uitkHitsPercent);
            stats.Add(new Label("%"));
            
            stats.Add(new Label("IMGUI Text cache Hits"));
            stats.Add(imguiHitsPercent);
            stats.Add(new Label("%"));
            rootVisualElement.Add(stats);

            rootVisualElement.schedule.Execute(UpdatePercent).Every(100);
#endif
            
            var view = m_LogViewerView.Instantiate();
            view.style.flexGrow = 1;
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
                viewer.SetLog(LogFile.LoadFromFile(path), -1);
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

