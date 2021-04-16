namespace GraphicsEditor.UIElements
{
    using System;
    using System.Collections.Generic;
    using System.Windows;

    public interface IUIElementCollection : IEnumerable<UIElement>
    {
        event EventHandler<IEnumerable<UIElement>> Added;

        event EventHandler<IEnumerable<UIElement>> Removed;

        void Add(UIElement child);

        void AddRange(IEnumerable<UIElement> children);

        void Remove(UIElement child);

        void RemoveRange(IEnumerable<UIElement> children);

        void Clear();
    }
}
