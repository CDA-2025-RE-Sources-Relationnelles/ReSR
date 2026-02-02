using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using FluentResponse;
using FluentResponse.Interfaces;

namespace ReSR.Domain.Aggregates.Accounts.ValueObjects;
#pragma warning disable SYSLIB0060 // Type or member is obsolete

public readonly record struct Password(string Hash) {

    private const int PBKDF2_ITER_COUNT    = 1000;  // default for Rfc2898DeriveBytes
    private const int PBKDF2_SUBKEY_LENGTH = 256/8; // 256 bits
    private const int SALT_SIZE            = 128/8; // 128 bits

    /* =======================
        * HASHED PASSWORD FORMATS
        * =======================
        * 
        * Version 0:
        * PBKDF2 with HMAC-SHA1, 128-bit salt, 256-bit subkey, 1000 iterations.
        * (See also: SDL crypto guidelines v5.1, Part III)
        * Format: { 0x00, salt, subkey }
        */
    public static IResponse<Password> TryCreate(string value) {

        value = value.Trim();

        // Produce a version 0 (see comment above) text hash.
        byte[] salt;
        byte[] subkey;
        using (var deriveBytes = new Rfc2898DeriveBytes(value, SALT_SIZE, PBKDF2_ITER_COUNT, HashAlgorithmName.SHA256)) {
            salt = deriveBytes.Salt;
            subkey = deriveBytes.GetBytes(PBKDF2_SUBKEY_LENGTH);
        }

        var outputBytes = new byte[1 + SALT_SIZE + PBKDF2_SUBKEY_LENGTH];
        Buffer.BlockCopy(salt, 0, outputBytes, 1, SALT_SIZE);
        Buffer.BlockCopy(subkey, 0, outputBytes, 1 + SALT_SIZE, PBKDF2_SUBKEY_LENGTH);

        return Response.Success(new Password(Convert.ToBase64String(outputBytes)));
    }

    public static Password FromNoise() {
        var random      = new Random();
        var outputBytes = new byte[1 + SALT_SIZE + PBKDF2_SUBKEY_LENGTH];
        random.NextBytes(outputBytes);
        return new Password(Convert.ToBase64String(outputBytes));
    }


    public IResponse TryVerify(string password) {

        var hashedPasswordBytes = Convert.FromBase64String(this.Hash);

        // Verify a version 0 (see comment above) text hash.
        if (hashedPasswordBytes.Length != (1 + SALT_SIZE + PBKDF2_SUBKEY_LENGTH) || hashedPasswordBytes[0] != 0x00)
            // Wrong length or version header.
            return Response.Failure();

        var salt = new byte[SALT_SIZE];
        Buffer.BlockCopy(hashedPasswordBytes, 1, salt, 0, SALT_SIZE);
        var storedSubkey = new byte[PBKDF2_SUBKEY_LENGTH];
        Buffer.BlockCopy(hashedPasswordBytes, 1 + SALT_SIZE, storedSubkey, 0, PBKDF2_SUBKEY_LENGTH);

        byte[] generatedSubkey;
        using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, PBKDF2_ITER_COUNT, HashAlgorithmName.SHA256))
            generatedSubkey = deriveBytes.GetBytes(PBKDF2_SUBKEY_LENGTH);

        return ByteArraysEqual(storedSubkey, generatedSubkey)
            ? Response.Success()
            : Response.Failure("Mot de passe incorrect !");
    }

    // Compares two byte arrays for equality. The method is specifically written so that the loop is not optimized.
    [MethodImpl(MethodImplOptions.NoOptimization)]
    private static bool ByteArraysEqual(byte[] a, byte[] b)  {
        if (ReferenceEquals(a, b))
            return true;

        if (a == null || b == null || a.Length != b.Length)
            return false;

        var areSame = true;
        for (var i = 0; i < a.Length; i++)
            areSame &= a[i] == b[i];

        return areSame;
    }
}
#pragma warning restore SYSLIB0060 // Type or member is obsolete
