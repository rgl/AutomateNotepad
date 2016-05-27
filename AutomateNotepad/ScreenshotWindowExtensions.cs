using System;
using System.Drawing;
using System.Runtime.InteropServices;
using TestStack.White.UIItems.WindowItems;

namespace AutomateNotepad
{
    // this came from https://github.com/TestStack/White/blob/3c12f781a8988b7e1d0bed08b3df8975805105eb/src/TestStack.White/WindowsAPI/DisplayedItem.cs
    // but was changed to use DwmGetWindowAttribute instead of GetWindowRect. The former does not include the DWM window shadow.
    public static class ScreenshotWindowExtensions
    {
        private const int srccopy = 0x00CC0020;

        [DllImport("GDI32.dll")]
        private static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hObjectSource, int nXSrc, int nYSrc, int dwRop);

        [DllImport("GDI32.dll")]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

        [DllImport("GDI32.dll")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("GDI32.dll")]
        private static extern bool DeleteDC(IntPtr hDC);

        [DllImport("GDI32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        [DllImport("GDI32.dll")]
        private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public readonly int left;
            public readonly int top;
            public int right;
            public int bottom;
        }

        [DllImport("User32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("User32.dll")]
        private static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("User32.dll")]
        private static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        private enum DWMWINDOWATTRIBUTE : uint
        {
            NCRenderingEnabled = 1,
            NCRenderingPolicy,
            TransitionsForceDisabled,
            AllowNCPaint,
            CaptionButtonBounds,
            NonClientRtlLayout,
            ForceIconicRepresentation,
            Flip3DPolicy,
            ExtendedFrameBounds,
            HasIconicBitmap,
            DisallowPeek,
            ExceludedFromPeek,
            Cloak,
            Cloaked,
            FreezeRepresentation
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmGetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE dwAttribute, out Rect pvAttribute, int cbAttribute);

        private static Bitmap Screenshot(IntPtr windowHandle)
        {
            var compatibleDeviceContext = IntPtr.Zero;
            var deviceContext = IntPtr.Zero;
            var bitmap = IntPtr.Zero;
            System.Drawing.Image img;
            try
            {
                deviceContext = GetWindowDC(windowHandle);
                var rect = new Rect();
                //GetWindowRect(windowHandle, ref rect);
                DwmGetWindowAttribute(windowHandle, DWMWINDOWATTRIBUTE.ExtendedFrameBounds, out rect, Marshal.SizeOf(rect));
                int width = rect.right - rect.left;
                int height = rect.bottom - rect.top;
                compatibleDeviceContext = CreateCompatibleDC(deviceContext);
                bitmap = CreateCompatibleBitmap(deviceContext, width, height);
                IntPtr @object = SelectObject(compatibleDeviceContext, bitmap);
                BitBlt(compatibleDeviceContext, 0, 0, width, height, deviceContext, rect.left, rect.top, srccopy);
                SelectObject(compatibleDeviceContext, @object);
            }
            finally
            {
                DeleteDC(compatibleDeviceContext);
                ReleaseDC(windowHandle, deviceContext);
                img = System.Drawing.Image.FromHbitmap(bitmap);
                DeleteObject(bitmap);
            }
            using (img) return new Bitmap(img);
        }

        public static Bitmap Screenshot(this Window window)
        {
            return Screenshot(new IntPtr(window.AutomationElement.Current.NativeWindowHandle));
        }
    }
}
