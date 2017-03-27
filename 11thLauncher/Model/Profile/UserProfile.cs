using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using Caliburn.Micro;
using _11thLauncher.Model.Parameter;
using _11thLauncher.Model.Server;

namespace _11thLauncher.Model.Profile
{
    public class UserProfile : INotifyPropertyChanged
    {
        public readonly string Name;
        public string DisplayName => IsDefault ? "★ " + Name : Name;

        private bool _isDefault;
        public bool IsDefault
        {
            get { return _isDefault; }
            set
            {
                _isDefault = value;
                OnPropertyChanged();
            }
        }

        public BindableCollection<Addon.Addon> ProfileAddons;
        public BindableCollection<LaunchParameter> ProfileParameters;
        public ServerConfig ProfileServerConfig;

        public UserProfile(string name, bool isDefault = false)
        {
            Name = name;
            IsDefault = isDefault;
        }

        public override bool Equals(object obj)
        {
            var item = obj as UserProfile;

            return item != null && Name.Equals(item.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public void Write()
        {
            if (!Directory.Exists(Constants.ProfilesPath))
            {
                Directory.CreateDirectory(Constants.ProfilesPath);
            }

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t"
            };

            using (XmlWriter writer = XmlWriter.Create(Constants.ProfilesPath + "\\" + Name + ".xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Profile");

                // Parameters
                //writer.WriteStartElement("Parameters");
                //if (ProfileParameters != null)
                //{
                    //foreach (KeyValuePair<string, string> entry in ProfileParameters)
                    //{
                        //writer.WriteStartElement("Parameter");
                        //writer.WriteAttributeString("name", entry.Key);
                        //writer.WriteString(entry.Value);
                        //writer.WriteEndElement();
                    //}
                //}
                //writer.WriteEndElement();

                // Addons
                writer.WriteStartElement("ArmA3Addons");
                if (ProfileAddons != null)
                {
                    foreach (var addon in ProfileAddons)
                    {
                        writer.WriteStartElement("A3Addon");
                        writer.WriteAttributeString("name", addon.Name);
                        writer.WriteString(addon.IsEnabled.ToString());
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();

                // ServerConfig
                //writer.WriteStartElement("ArmA3Server");
                //if (ProfileServerConfig != null)
                //{
                    //foreach (KeyValuePair<string, string> entry in ProfileServerConfig)
                    //{
                        //writer.WriteStartElement("A3ServerInfo");
                        //writer.WriteAttributeString("name", entry.Key);
                        //writer.WriteString(entry.Value);
                        //writer.WriteEndElement();
                    //}
                //}
                //writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }


        public void Read(BindableCollection<Addon.Addon> addons, BindableCollection<LaunchParameter> parameters, ServerConfig serverConfig)
        {
            ProfileAddons = addons;
            var addonIndex = 0;
            ProfileParameters = parameters;
            ProfileServerConfig = serverConfig;

            //Read profile
            using (XmlReader reader = XmlReader.Create(Constants.ProfilesPath + "\\" + Name + ".xml"))
            {
                while (reader.Read())
                {
                    if (!reader.IsStartElement()) continue;

                    string parameter;
                    string value;
                    switch (reader.Name)
                    {
                        case "Parameter":
                            parameter = reader["name"];
                            reader.Read();
                            value = reader.Value.Trim();
                            //if (parameter != null) ProfileParameters[parameter] = value; TODO PARAM READ
                            break;
                        case "A3Addon":
                            parameter = reader["name"];
                            reader.Read();
                            value = reader.Value.Trim();
                            if (parameter != null)
                            {
                                var addon = ProfileAddons.FirstOrDefault(a => a.Name == parameter);
                                if (addon != null)
                                {
                                    addon.SetStatus(bool.Parse(value));//TODO tryparse?
                                    addons.Move(addons.IndexOf(addon), addonIndex);
                                    addonIndex++;
                                }
                            }
                            break;
                        case "A3ServerInfo":
                            parameter = reader["name"];
                            reader.Read();
                            value = reader.Value.Trim();
                            //if (parameter != null) ProfileServerConfig[parameter] = value; TODO server config read
                            break;
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
