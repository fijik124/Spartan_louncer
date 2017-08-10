using System;
using System.IO;
using System.Linq;
using System.Xml;
using Caliburn.Micro;
using Newtonsoft.Json;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;
using _11thLauncher.Util;
using Formatting = Newtonsoft.Json.Formatting;

namespace _11thLauncher.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IParameterService _parameterService;

        public ProfileService(IParameterService parameterService)
        {
            _parameterService = parameterService;
        }

        public void Write(UserProfile profile, BindableCollection<Addon> addons, 
            BindableCollection<LaunchParameter> parameters, LaunchSettings launchSettings)
        {
            ProfileFile profileFile = new ProfileFile
            {
                Profile = profile,
                Addons = addons ?? new BindableCollection<Addon>(),
                Parameters = parameters ?? new BindableCollection<LaunchParameter>(),
                LaunchSettings = launchSettings ?? new LaunchSettings()
            };

            if (!Directory.Exists(Path.Combine(Constants.ConfigPath, Constants.ProfilesFolder)))
            {
                Directory.CreateDirectory(Path.Combine(Constants.ConfigPath, Constants.ProfilesFolder));
            }

            File.WriteAllText(Path.Combine(Constants.ConfigPath, Constants.ProfilesFolder, string.Format(Constants.ProfileNameFormat, profile.Id)), 
                JsonConvert.SerializeObject(profileFile, Formatting.Indented));
        }

        public void Read(UserProfile profile, out BindableCollection<Addon> addons, out BindableCollection<LaunchParameter> parameters, out LaunchSettings launchSettings)
        {
            ProfileFile profileFile = new ProfileFile { Profile = profile };

            var settings = new JsonSerializerSettings { Converters = { new JsonLaunchParameterConverter() } };
            JsonConvert.PopulateObject(File.ReadAllText(Path.Combine(Constants.ConfigPath, Constants.ProfilesFolder,
                string.Format(Constants.ProfileNameFormat, profile.Id))), profileFile, settings); //TODO TRYCTACH

            addons = profileFile.Addons;
            parameters = profileFile.Parameters;
            launchSettings = profileFile.LaunchSettings;
        }

        public void DeleteProfile(UserProfile profile)
        {
            var profileFile = Path.Combine(Constants.ConfigPath, Constants.ProfilesFolder, string.Format(Constants.ProfileNameFormat, profile.Id));
            if (File.Exists(profileFile))
            {
                File.Delete(profileFile);
            }
        }

        public void PortLegacyProfiles(BindableCollection<UserProfile> profiles)
        {
            foreach (var profile in profiles)
            {
                try
                {
                    var profileFile = Path.Combine(Constants.ConfigPath, Constants.ProfilesFolder, string.Format(Constants.LegacyProfileNameFormat, profile.Name));

                    BindableCollection<Addon> addons = new BindableCollection<Addon>();
                    BindableCollection<LaunchParameter> parameters = new BindableCollection<LaunchParameter>();
                    LaunchSettings launchSettings = new LaunchSettings();

                    using (XmlReader reader = XmlReader.Create(profileFile))
                    {
                        while (reader.Read())
                        {
                            if (reader.IsStartElement())
                            {
                                string parameter;
                                string value;
                                switch (reader.Name)
                                {
                                    case "Parameter":
                                        parameter = reader["name"];
                                        reader.Read();
                                        value = reader.Value.Trim();
                                        if (parameter != null)
                                        {
                                            var matchingParameter = _parameterService.Parameters.FirstOrDefault(p => p.LegacyName.Equals(parameter));
                                            if (matchingParameter != null)
                                            {
                                                switch (matchingParameter.Type)
                                                {
                                                    case ParameterType.Boolean:
                                                        bool parsedValue;
                                                        var parsed = bool.TryParse(value, out parsedValue);
                                                        matchingParameter.IsEnabled = parsed && parsedValue;
                                                        break;
                                                    case ParameterType.Selection:
                                                        //TODO
                                                        break;
                                                    case ParameterType.Text:
                                                        //TODO
                                                        break;
                                                    default:
                                                        throw new ArgumentOutOfRangeException();
                                                }
                                                parameters.Add(matchingParameter);
                                            }
                                        }
                                        break;
                                    case "A3Addon":
                                        parameter = reader["name"];
                                        reader.Read();
                                        value = reader.Value.Trim();
                                        //If addon no longer exists, discard it 
                                        //if (Addons.LocalAddons.Contains(parameter))
                                        //{
                                        //    if (parameter != null) ProfileAddons[parameter] = value;
                                        //}
                                        break;
                                    case "A3ServerInfo":
                                        parameter = reader["name"];
                                        reader.Read();
                                        value = reader.Value.Trim();
                                        //if (parameter != null) ProfileServerInfo[parameter] = value;
                                        break;
                                }
                            }
                        }
                    }

                    Write(profile, addons, parameters, launchSettings);
                    File.Delete(profileFile);
                }
                catch (Exception e)
                {
                    throw;
                }
            }
        }
    }
}
