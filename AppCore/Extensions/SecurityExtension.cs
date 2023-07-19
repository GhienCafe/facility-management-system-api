using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace AppCore.Extensions;

public static class SecurityExtension
{
    public static string GenerateSalt()
    {
        return Guid.NewGuid().ToString("N");
    }

    public static string HashPassword<T>(this string password, string salt) where T : class, new()
    {
        var passwordHasher = new PasswordHasher<T>();
        return passwordHasher.HashPassword(new T(), password + salt);
    }

    public static bool VerifyPassword<T>(this string password, string salt, string hash) where T : class, new()
    {
        var passwordHasher = new PasswordHasher<T>();
        return passwordHasher.VerifyHashedPassword(new T(), hash, password + salt) ==
               PasswordVerificationResult.Success;
    }

    public static string GenerateRandomString(int length)
    {
        var randomGenerator = RandomNumberGenerator.Create();
        var data = new byte[length];
        randomGenerator.GetBytes(data);
        return BitConverter.ToString(data).Replace("-", "");
    }

    public static string EncryptData(this string key, string value)
    {
        // Get bytes of plaintext string
        var encodeKey = Encoding.UTF8.GetBytes(key);
        var plainBytes = Encoding.UTF8.GetBytes(value);

        // Get parameter sizes
        var nonceSize = AesGcm.NonceByteSizes.MaxSize;
        var tagSize = AesGcm.TagByteSizes.MaxSize;
        var cipherSize = plainBytes.Length;

        // We write everything into one big array for easier encoding
        var encryptedDataLength = 4 + nonceSize + 4 + tagSize + cipherSize;
        var encryptedData = encryptedDataLength < 1024
            ? stackalloc byte[encryptedDataLength]
            : new byte[encryptedDataLength].AsSpan();

        // Copy parameters
        BinaryPrimitives.WriteInt32LittleEndian(encryptedData[..4], nonceSize);
        BinaryPrimitives.WriteInt32LittleEndian(encryptedData.Slice(4 + nonceSize, 4), tagSize);
        var nonce = encryptedData.Slice(4, nonceSize);
        var tag = encryptedData.Slice(4 + nonceSize + 4, tagSize);
        var cipherBytes = encryptedData.Slice(4 + nonceSize + 4 + tagSize, cipherSize);

        RandomNumberGenerator.Fill(nonce);

        // Encrypt
        using var aes = new AesGcm(encodeKey);
        aes.Encrypt(nonce, plainBytes.AsSpan(), cipherBytes, tag);

        // Encode for transmission
        return Convert.ToBase64String(encryptedData);
    }

    public static string DecryptData(this string key, string value)
    {
        var encodeKey = Encoding.UTF8.GetBytes(key);
        // Decode
        var encryptedData = Convert.FromBase64String(value).AsSpan();

        // Extract parameter sizes
        var nonceSize = BinaryPrimitives.ReadInt32LittleEndian(encryptedData[..4]);
        var tagSize = BinaryPrimitives.ReadInt32LittleEndian(encryptedData.Slice(4 + nonceSize, 4));
        var cipherSize = encryptedData.Length - 4 - nonceSize - 4 - tagSize;

        // Extract parameters
        var nonce = encryptedData.Slice(4, nonceSize);
        var tag = encryptedData.Slice(4 + nonceSize + 4, tagSize);
        var cipherBytes = encryptedData.Slice(4 + nonceSize + 4 + tagSize, cipherSize);

        // Decrypt
        var plainBytes = cipherSize < 1024
            ? stackalloc byte[cipherSize]
            : new byte[cipherSize];
        using var aes = new AesGcm(encodeKey);
        aes.Decrypt(nonce, cipherBytes, tag, plainBytes);

        // Convert plain bytes back into string
        return Encoding.UTF8.GetString(plainBytes);
    }
}