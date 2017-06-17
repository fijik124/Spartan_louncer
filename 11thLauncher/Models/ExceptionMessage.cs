using System;

namespace _11thLauncher.ViewModels
{
    public class ExceptionMessage
    {
        public Exception Exception;
        public string ClassName;

        public ExceptionMessage(Exception e, string className)
        {
            Exception = e;
            ClassName = className;
        }
    }
}
