using System;
using System.Runtime.Serialization;
using Caliburn.Micro;

namespace _11thLauncher.Models
{
    [DataContract]
    public class UserProfile : PropertyChangedBase
    {
        private string _name;
        private bool _isDefault;

        public UserProfile(string name, bool isDefault = false)
        {
            Id = Guid.NewGuid();
            Name = name;
            IsDefault = isDefault;
        }

        public UserProfile(Guid guid, string name, bool isDefault = false)
        {
            Id = guid;
            Name = name;
            IsDefault = isDefault;
        }

        #region Properties

        [DataMember]
        public Guid Id { get; }

        [DataMember]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => DisplayName);
            }
        }

        public string DisplayName => IsDefault ? "★ " + Name : Name;

        public bool IsDefault
        {
            get => _isDefault;
            set
            {
                _isDefault = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => DisplayName);
            }
        }

        #endregion

        public override bool Equals(object obj)
        {
            var item = obj as UserProfile;

            return item != null && Id.Equals(item.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        //public void Write()
        //{
            //if (!Directory.Exists(ApplicationConfig.ProfilesPath))
            //{
                //Directory.CreateDirectory(ApplicationConfig.ProfilesPath);
            //}

            //File.WriteAllText(Path.Combine(ApplicationConfig.ProfilesPath, Id + ".json"), JsonConvert.SerializeObject(this));
        //}

        //[Obsolete]
        //public void Writes()
        //{
            //if (!Directory.Exists(ApplicationConfig.ProfilesPath))
            //{
                //Directory.CreateDirectory(ApplicationConfig.ProfilesPath);
            //}

            //XmlWriterSettings settings = new XmlWriterSettings
            //{
                //Indent = true,
                //IndentChars = "\t"
            //};

            //using (XmlWriter writer = XmlWriter.Create(ApplicationConfig.ProfilesPath + "\\" + Name + ".xml", settings))
            //{
                //writer.WriteStartDocument();
                //writer.WriteStartElement("Profile");

                //// Parameters
                ////writer.WriteStartElement("Parameters");
                ////if (ProfileParameters != null)
                ////{
                    ////foreach (KeyValuePair<string, string> entry in ProfileParameters)
                    ////{
                        ////writer.WriteStartElement("Parameter");
                        ////writer.WriteAttributeString("name", entry.Key);
                        ////writer.WriteString(entry.Value);
                        ////writer.WriteEndElement();
                    ////}
                ////}
                ////writer.WriteEndElement();

                //// Addons
                //writer.WriteStartElement("ArmA3Addons");
                //if (ProfileAddons != null)
                //{
                    //foreach (var addon in ProfileAddons)
                    //{
                        //writer.WriteStartElement("A3Addon");
                        //writer.WriteAttributeString("name", addon.Name);
                        //writer.WriteString(addon.IsEnabled.ToString());
                        //writer.WriteEndElement();
                    //}
                //}
                //writer.WriteEndElement();

                //// ServerConfig
                ////writer.WriteStartElement("ArmA3Server");
                ////if (ProfileServerConfig != null)
                ////{
                    ////foreach (KeyValuePair<string, string> entry in ProfileServerConfig)
                    ////{
                        ////writer.WriteStartElement("A3ServerInfo");
                        ////writer.WriteAttributeString("name", entry.Key);
                        ////writer.WriteString(entry.Value);
                        ////writer.WriteEndElement();
                    ////}
                ////}
                ////writer.WriteEndElement();

                //writer.WriteEndElement();
                //writer.WriteEndDocument();
            //}
        //}

        //public void Read(BindableCollection<Addon.Addon> addons, BindableCollection<LaunchParameter> parameters,
            //ServerConfig serverConfig)
        //{
            //ProfileAddons = addons;
            //var addonIndex = 0;
            //ProfileParameters = parameters;
            //ProfileServerConfig = serverConfig;

            //JsonConvert.PopulateObject(File.ReadAllText(Path.Combine(ApplicationConfig.ProfilesPath, Id + ".json")), this);
            ////TODO
        //}

        //[Obsolete]
        //public void Reads(BindableCollection<Addon.Addon> addons, BindableCollection<LaunchParameter> parameters, ServerConfig serverConfig)
        //{
            //ProfileAddons = addons;
            //var addonIndex = 0;
            //ProfileParameters = parameters;
            //ProfileServerConfig = serverConfig;

            ////Read profile
            //using (XmlReader reader = XmlReader.Create(ApplicationConfig.ProfilesPath + "\\" + Name + ".xml"))
            //{
                //while (reader.Read())
                //{
                    //if (!reader.IsStartElement()) continue;

                    //string parameter;
                    //string value;
                    //switch (reader.Name)
                    //{
                        //case "Parameter":
                            //parameter = reader["name"];
                            //reader.Read();
                            //value = reader.Value.Trim();
                            ////if (parameter != null) ProfileParameters[parameter] = value; TODO PARAM READ
                            //break;
                        //case "A3Addon":
                            //parameter = reader["name"];
                            //reader.Read();
                            //value = reader.Value.Trim();
                            //if (parameter != null)
                            //{
                                //var addon = ProfileAddons.FirstOrDefault(a => a.Name == parameter);
                                //if (addon != null)
                                //{
                                    //addon.SetStatus(bool.Parse(value));//TODO tryparse?
                                    //addons.Move(addons.IndexOf(addon), addonIndex);
                                    //addonIndex++;
                                //}
                            //}
                            //break;
                        //case "A3ServerInfo":
                            //parameter = reader["name"];
                            //reader.Read();
                            //value = reader.Value.Trim();
                            ////if (parameter != null) ProfileServerConfig[parameter] = value; TODO server config read
                            //break;
                    //}
                //}
            //}
        //}
    }
}
