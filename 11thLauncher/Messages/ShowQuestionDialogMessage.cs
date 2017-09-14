using System;
using MahApps.Metro.Controls.Dialogs;

namespace _11thLauncher.Messages
{
    public class ShowQuestionDialogMessage
    {
        public string Title;
        public string Content;
        public Action<MessageDialogResult> Callback;
    }
}
