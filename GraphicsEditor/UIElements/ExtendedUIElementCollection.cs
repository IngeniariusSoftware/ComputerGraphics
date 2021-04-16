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

        public event EventHandler<IEnumerable<UIElement>> Added = delegate { };

        public event EventHandler<IEnumerable<UIElement>> Removed = delegate { };

        private UIElementCollection Collection { get; }

        public void Add(UIElement child)
        {
            Collection.Add(child);
            Added(this, new List<UIElement> { child });
        }

        public void AddRange(IEnumerable<UIElement> children)
        {
            var uiElements = children.ToList();
            uiElements.ForEach(x => Collection.Add(x));
            Added(this, uiElements);
        }

        public void Remove(UIElement child)
        {
            Collection.Remove(child);
            Removed(this, new List<UIElement> { child });
        }

        public void RemoveRange(IEnumerable<UIElement> children)
        {
            var uiElements = children.ToList();
            uiElements.ForEach(x => Collection.Remove(x));
            Removed(this, uiElements);
        }

        public void Clear()
        {
            var children = Collection.Cast<UIElement>().ToList();
            Collection.Clear();
            Removed(this, children);
        }

        public IEnumerator<UIElement> GetEnumerator() => Collection.Cast<UIElement>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
