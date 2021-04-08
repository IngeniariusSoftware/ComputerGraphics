namespace GraphicsEditor.UIElements
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    public class ExtendedUIElementCollection : IUIElementCollection
    {
        public ExtendedUIElementCollection(UIElementCollection collection)
        {
            Collection = collection;
        }

        public event EventHandler<UIElement> Added = delegate { };

        public event EventHandler<UIElement> Removed = delegate { };

        private UIElementCollection Collection { get; }

        public int Add(UIElement child)
        {
            int result = Collection.Add(child);
            Added(this, child);
            return result;
        }

        public void Remove(UIElement child)
        {
            Collection.Remove(child);
            Removed(this, child);
        }

        public void Clear()
        {
            var children = Collection.Cast<UIElement>().ToList();
            Collection.Clear();
            children.ForEach(x => Removed(this, x));
        }

        public IEnumerator<UIElement> GetEnumerator() => Collection.Cast<UIElement>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
