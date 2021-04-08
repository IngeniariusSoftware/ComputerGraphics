namespace GraphicsEditor.UIElements
{
    using System.Windows;

    public interface IPanel
    {
        IUIElementCollection Children { get; }

        double ActualWidth { get; }

        double ActualHeight { get; }

        Visibility Visibility { get; set; }
    }
}
