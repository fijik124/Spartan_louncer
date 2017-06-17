namespace _11thLauncher.Services.Contracts
{
    internal interface ISecurityService
    {
        string EncryptPassword(string text);
        string DecryptPassword(string text);
    }
}
