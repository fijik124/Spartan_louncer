using Caliburn.Micro;
using _11thLauncher.Models;

namespace _11thLauncher.Messages
{
    /// <summary>
    /// A message sent when the repositories are loaded. To be handled by an event aggregator.
    /// </summary>
    public class RepositoriesLoadedMessage
    {
        /// <summary>
        /// Collection of repositories loaded.
        /// </summary>
        public BindableCollection<Repository> Repositories;

        /// <summary>
        /// Creates a new instance of the <see cref="RepositoriesLoadedMessage"/> class, with the loaded repositories
        /// </summary>
        public RepositoriesLoadedMessage(BindableCollection<Repository> repositories)
        {
            Repositories = repositories;
        }
    }
}