namespace GraphicsEditor.UIElements
{
    using System.Windows;
    using System.Windows.Input;

    public interface IPanel
    {
        event MouseButtonEventHandler PreviewMouseLeftButtonDown;

        event MouseButtonEventHandler PreviewMouseRightButtonDown;

        IUIElementCollection Children { get; }

        double ActualWidth { get; }

        double ActualHeight { get; }

        Visibility Visibility { get; set; }
    }
}
