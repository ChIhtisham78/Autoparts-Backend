namespace Autopart.Application.Helpers
{
    public interface IPasswordHelper
    {
        string CreatePasswordHash(string salt, string password);
        string CreateSalt(int size);
    }
}