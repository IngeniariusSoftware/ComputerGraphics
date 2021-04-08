namespace GraphicsEditor.UIElements
{
    using System;
    using System.Collections.Generic;
    using System.Windows;

    public interface IUIElementCollection : IEnumerable<UIElement>
    {
        event EventHandler<UIElement> Added;

        event EventHandler<UIElement> Removed;

        int Add(UIElement child);

        void Remove(UIElement child);

        void Clear();
    }
}
