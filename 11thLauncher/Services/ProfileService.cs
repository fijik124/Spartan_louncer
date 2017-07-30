using System;
using System.IO;
using System.Xml;
using Caliburn.Micro;
using Newtonsoft.Json;
using _11thLauncher.Models;
using _11thLauncher.Services.Contracts;
using Formatting = Newtonsoft.Json.Formatting;

namespace _11thLauncher.Services
{
    public class ProfileService : IProfileService
    {
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
            JsonConvert.PopulateObject(File.ReadAllText(Path.Combine(Constants.ConfigPath, Constants.ProfilesFolder, 
                string.Format(Constants.ProfileNameFormat, profile.Id))), profileFile); //TODO TRYCTACH
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
            //foreach (var profile in profiles)
            //{
                //try
                //{
                    //using (XmlReader reader = XmlReader.Create(Path.Combine(Constants.ConfigPath) + "\\" + profile + ".xml"))
                    //{
                        //while (reader.Read())
                        //{
                            //if (reader.IsStartElement())
                            //{
                                //string parameter;
                                //string value;
                                //switch (reader.Name)
                                //{
                                    //case "Parameter":
                                        //parameter = reader["name"];
                                        //reader.Read();
                                        //value = reader.Value.Trim();
                                        //if (parameter != null)
                                        //{
                                            //profile.
                                            //ProfileParameters[parameter] = value;
                                        //}
                                        //break;
                                    //case "A3Addon":
                                        //parameter = reader["name"];
                                        //reader.Read();
                                        //value = reader.Value.Trim();
                                        ////If addon no longer exists, discard it 
                                        //if (Addons.LocalAddons.Contains(parameter))
                                        //{
                                            //if (parameter != null) ProfileAddons[parameter] = value;
                                        //}
                                        //break;
                                    //case "A3ServerInfo":
                                        //parameter = reader["name"];
                                        //reader.Read();
                                        //value = reader.Value.Trim();
                                        //if (parameter != null) ProfileServerInfo[parameter] = value;
                                        //break;
                                //}
                            //}
                        //}
                    //}
                //catch (Exception)
                //{
                    //continue;
                //}
            //}
        }
    }
}
