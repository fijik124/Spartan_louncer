using _11thLauncher.Util;

namespace _11thLauncher.Services
{
    public class AbstractService
    {
        protected readonly ILogger Logger;

        protected AbstractService(ILogger logger)
        {
            Logger = logger;
        }
    }
}
