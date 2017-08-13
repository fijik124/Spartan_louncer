using Microsoft.Win32;

namespace _11thLauncher.Accessors.Contracts
{
    public interface IRegistryAccessor
    {
        object GetValue(string keyName, string valueName, object defaultValue);

        object GetKeyValue(RegistryKey key, string name, object defaultValue);

        RegistryKey OpenBaseKey(RegistryHive hkey, RegistryView view);

        RegistryKey OpenSubKey(RegistryKey key, string name);
    }
}
