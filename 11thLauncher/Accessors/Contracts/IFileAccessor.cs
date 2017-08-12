using System.IO;

namespace _11thLauncher.Accessors.Contracts
{
    public interface IFileAccessor
    {
        string[] GetDirectories(string path, string searchPattern, SearchOption searchOption);

        string[] GetFiles(string path);

        string[] GetFiles(string path, string searchPattern);

        DirectoryInfo GetParent(string path);

        bool DirectoryExists(string path);

        bool FileExists(string path);

        string[] ReadAllLines(string path);

        string ReadAllText(string path);

        void WriteAllBytes(string path, byte[] bytes);

        void WriteAllText(string path, string contents);

        FileStream CreateFile(string path);

        void DeleteFile(string path);

        void CreateDirectory(string path);

        void DeleteDirectory(string path, bool recursive);
    }
}
