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
        private readonly ISecurityService _securityService;

        public ProfileService(IParameterService parameterService, ISecurityService securityService)
        {
            _parameterService = parameterService;
            _securityService = securityService;
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
                            if (!reader.IsStartElement()) continue;

                            string parameter;
                            string value;
                            bool parsed;
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
                                                    parsed = bool.TryParse(value, out parsedValue);
                                                    matchingParameter.SetStatus(parsed && parsedValue);
                                                    break;
                                                case ParameterType.Selection:
                                                    //No legacy conversion
                                                    break;
                                                case ParameterType.Numerical:
                                                    //No legacy conversion
                                                    break;
                                                case ParameterType.Text:
                                                    ((TextParameter) matchingParameter).SetStatus(true, value);
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
                                    bool enabled;
                                    parsed = bool.TryParse(reader.Value.Trim(), out enabled);
                                    if (parameter != null && parsed)
                                    {
                                        var addon = new Addon
                                        {
                                            IsEnabled = enabled,
                                            Name = parameter
                                        };
                                        addons.Add(addon);
                                    }
                                    break;

                                case "A3ServerInfo":
                                    parameter = reader["name"];
                                    reader.Read();
                                    value = reader.Value.Trim();
                                    switch (parameter)
                                    {
                                        case "server":
                                            launchSettings.Server = value;
                                            break;

                                        case "port":
                                            launchSettings.Port = value;
                                            break;

                                        case "pass":
                                            launchSettings.Password = _securityService.EncryptPassword(value);
                                            break;

                                        default:
                                            break;
                                    }
                                    break;

                                default:
                                    break;
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
