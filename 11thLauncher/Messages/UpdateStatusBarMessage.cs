using _11thLauncher.Models;

namespace _11thLauncher.Messages
{
    public class UpdateStatusBarMessage
    {
        public readonly AsyncAction Action;
        public readonly bool IsRunning;

        public UpdateStatusBarMessage(AsyncAction action, bool isRunning)
        {
            Action = action;
            IsRunning = isRunning;
        }
    }
}
