using System;
using UnityEngine.UIElements;

namespace UnityEngine.UIElements2020
{
    internal interface IReorderable<T>
    {
        bool enableReordering { get; set; }
        Action<ItemMoveArgs<T>> onItemMoved { get; set; }
    }

    internal struct ItemMoveArgs<T>
    {
        public T item;
        public int newIndex;
        public int previousIndex;
    }
}
