using System.Windows;
using _11thLauncher.Accessors.Contracts;

namespace _11thLauncher.Accessors
{
    public class ClipboardAccessor : IClipboardAccessor
    {
        public void SetText(string text)
        {
            Clipboard.SetText(text);
        }
    }
}
