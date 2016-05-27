using TestStack.White;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.WindowsAPI;

namespace AutomateNotepad
{
    // see http://white.teststack.net/docs/getting-started
    //
    // to inspect a window internal tree you should use these tools:
    //
    // * UIAVerify. It comes with the Windows SDK (https://developer.microsoft.com/en-us/windows/downloads/windows-8-1-sdk)
    //   and is normally installed at C:\Program Files (x86)\Windows Kits\8.1\bin\x64\UIAVerify\VisualUIAVerifyNative.exe.
    // * Spy++. It comes with Visual Studio (https://www.visualstudio.com/en-us/products/visual-studio-community-vs.aspx) and
    //   is normally installed at C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\Tools\spyxx.exe (and spyxx_amd64.exe).
    class Program
    {
        static void Main(string[] args)
        {
            var applicationPath = @"C:\Windows\system32\notepad.exe";

            // start notepad (or notepad2 if you have it installed to replace notepad).
            //
            // CAVEAT Dispose will Kill the application. So normally, one should orderly
            // quit the application before reaching Dispose. Like we do bellow.
            using (var application = Application.Launch(applicationPath))
            {
                // iterate over all the application windows. in this case there is only one.
                foreach (var window in application.GetWindows())
                {
                    // log the entire window structure. this is useful when you don't
                    // have UIAVerify installed.
                    //window.LogStructure();

                    // move and resize the window.
                    window.Move(0, 0, 600, 300);

                    // send some text to the editor.
                    window.Keyboard.Enter("Hello World!");

                    // send some key combinations.
                    window.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.HOME);
                    window.Keyboard.HoldKey(KeyboardInput.SpecialKeys.CONTROL);
                    window.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.RIGHT);
                    window.Keyboard.LeaveKey(KeyboardInput.SpecialKeys.CONTROL);

                    // screenshot the window.
                    // NB we use the Screenshot extension method that's inside this
                    // project. The VisibleImage property also captures the window
                    // drop shadow that newer Windows have.
                    using (var image = window.Screenshot())
                    {
                        image.Save("screenshot-main-window.png");
                    }

                    // detect notepad2 (we automate it slightly differently).
                    var isNotepad2 = window.Title.Contains("Notepad2");

                    // get a window by a Win32 Window Class Name or its type.
                    var textPane = isNotepad2
                        ? window.Get(SearchCriteria.ByClassName("Scintilla"))
                        : window.Get<TextBox>();

                    // screenshot the text pane.
                    using (var image = textPane.VisibleImage)
                    {
                        image.Save("screenshot-text-pane.png");
                    }

                    // select all text using the context menu.
                    textPane.RightClick();
                    window.Popup.Item("Select All").Click();

                    // screenshot the text pane after the text was selected.
                    using (var image = textPane.VisibleImage)
                    {
                        image.Save("screenshot-text-pane-all-text-selected.png");
                    }

                    // orderly quit the application.
                    window.Close();
                    // CAVEAT this waits for a Modal Window to appear with this exact caption.
                    var confirmationWindow = window.ModalWindow(isNotepad2 ? "Notepad2" : "Notepad");
                    confirmationWindow.Get<Button>(isNotepad2 ? "No" : "Don't Save").Click();
                }
            }
        }
    }
}