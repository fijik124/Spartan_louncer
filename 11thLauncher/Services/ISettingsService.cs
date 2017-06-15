using System;
using Caliburn.Micro;
using _11thLauncher.Model.Profile;
using _11thLauncher.Model.Server;
using _11thLauncher.Model.Settings;

namespace _11thLauncher.Services
{
    public interface ISettingsService
    {
        ApplicationSettings ApplicationSettings { get; set; }

        Guid DefaultProfileId { get; set; }

        BindableCollection<UserProfile> UserProfiles { get; set; }

        BindableCollection<Server> Servers { get; set; }


        bool SettingsExist();

        /// <summary>
        /// Get the version number of the local game executable
        /// </summary>
        /// <returns>Game version</returns>
        string GetGameVersion();

        /// <summary>
        /// Try to read the game path from the windows registry
        /// </summary>
        void ReadPath();

        void Read(bool settingsExist);

        void Write();
    }
}
