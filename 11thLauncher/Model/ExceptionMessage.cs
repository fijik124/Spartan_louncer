using System;

namespace _11thLauncher.Model
{
    public class ExceptionMessage
    {
        public Exception Exception;
        public string Message;

        public ExceptionMessage(Exception e)
        {
            Exception = e;
        }
    }
}
