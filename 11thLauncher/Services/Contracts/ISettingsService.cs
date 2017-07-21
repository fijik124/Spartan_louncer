using System;
using Caliburn.Micro;
using _11thLauncher.Models;

namespace _11thLauncher.Services.Contracts
{
    public interface ISettingsService
    {
        ApplicationSettings ApplicationSettings { get; set; }

        Guid DefaultProfileId { get; set; }

        BindableCollection<UserProfile> UserProfiles { get; set; }

        BindableCollection<Server> Servers { get; set; }

        /// <summary>
        /// Version of Java detected on PATH. Empty string if none detected
        /// </summary>
        string JavaVersion { get; set; }

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

        /// <summary>
        /// Try to read the stored settings from appdata or initialize default values if no settings stored
        /// </summary>
        /// <returns>True if stored settings were loaded succesfully. False if they don't exist or failed to load</returns>
        bool Read();

        void Write();

        void Delete();

        /// <summary>
        /// Sets the application theme and accent
        /// </summary>
        /// <param name="theme">Theme to use</param>
        /// <param name="accent">Accent to use</param>
        void UpdateThemeAndAccent(ThemeStyle theme, AccentColor accent);
    }
}
