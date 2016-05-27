using System;
using System.Runtime.InteropServices;
using TestStack.White.UIItems.WindowItems;

namespace AutomateNotepad
{
    public static class MoveWindowExtensions
    {
        private const int SWP_NOZORDER = 0x0004;

        // see https://msdn.microsoft.com/en-us/library/windows/desktop/ms633545(v=vs.85).aspx
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hwndAfter, int x, int y, int width, int height, int flags);

        public static void Move(this Window window, int x, int y, int width, int height)
        {
            var handle = new IntPtr(window.AutomationElement.Current.NativeWindowHandle);
            SetWindowPos(handle, IntPtr.Zero, x, y, width, height, SWP_NOZORDER);
        }
    }
}
