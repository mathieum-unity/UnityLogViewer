using System.Collections.Generic;
using UnityEngine.UIElements;

namespace UnityEngine.UIElements2020
{
    internal interface IDragAndDrop
    {
        void StartDrag(StartDragArgs args);
        void AcceptDrag();
        void SetVisualMode(DragVisualMode visualMode);
        IDragAndDropData data { get; }
    }

    internal interface IDragAndDropData
    {
        object GetGenericData(string key);
        object userData { get; }
        IEnumerable<Object> unityObjectReferences { get; }
    }
}
