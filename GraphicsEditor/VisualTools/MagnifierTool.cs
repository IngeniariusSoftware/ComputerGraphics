namespace GraphicsEditor.VisualTools
{
    using WindowsInput;
    using WindowsInput.Native;

    public class MagnifierTool : BaseTool
    {
        public MagnifierTool()
        {
            Keyboard = new InputSimulator().Keyboard;
        }

        private IKeyboardSimulator Keyboard { get; }

        public override void Open()
        {
            Keyboard.KeyDown(VirtualKeyCode.LWIN);
            Keyboard.KeyPress(VirtualKeyCode.OEM_PLUS);
            Keyboard.KeyUp(VirtualKeyCode.LWIN);
        }

        public override void Close()
        {
            Keyboard.KeyDown(VirtualKeyCode.LWIN);
            Keyboard.KeyPress(VirtualKeyCode.ESCAPE);
            Keyboard.KeyUp(VirtualKeyCode.LWIN);
        }
    }
}
