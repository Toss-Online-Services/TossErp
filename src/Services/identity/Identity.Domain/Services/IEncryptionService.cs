namespace Identity.Domain.Services;

public interface IEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
    string GenerateSecureKey(int length = 32);
    string EncryptSensitiveData(string data);
    string DecryptSensitiveData(string encryptedData);
}
