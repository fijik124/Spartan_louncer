namespace _11thLauncher.Services.Contracts
{
    public interface ISecurityService
    {
        string EncryptPassword(string text);
        string DecryptPassword(string text);
    }
}
