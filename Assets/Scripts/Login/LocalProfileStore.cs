using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace ClousEnd.Login
{
    /// <summary>
    /// Perfil local. No es autenticación de servidor: separa partidas en este equipo.
    /// La UI no conoce PlayerPrefs; solo llama a estos métodos.
    /// </summary>
    public static class LocalProfileStore
    {
        private const string Prefix = "clousend.profile.";
        private const string LastUserKey = "clousend.lastUser";
        private const string Salt = "clous-end::aurora-9::";

        public static string LastUser => PlayerPrefs.GetString(LastUserKey, string.Empty);

        public static bool Exists(string displayName)
        {
            return PlayerPrefs.HasKey(HashKey(displayName) + ".hash");
        }

        public static string GetDisplayName(string displayName)
        {
            return PlayerPrefs.GetString(HashKey(displayName) + ".name", displayName.Trim());
        }

        public static bool Validate(string displayName, string password)
        {
            var stored = PlayerPrefs.GetString(HashKey(displayName) + ".hash", string.Empty);
            return !string.IsNullOrEmpty(stored) && FixedEquals(stored, Hash(password));
        }

        public static void Save(string displayName, string password)
        {
            var trimmed = displayName.Trim();
            var key = HashKey(trimmed);

            PlayerPrefs.SetString(key + ".name", trimmed);
            PlayerPrefs.SetString(key + ".hash", Hash(password));
            PlayerPrefs.SetString(key + ".created", DateTime.UtcNow.ToString("o"));
            PlayerPrefs.SetString(LastUserKey, trimmed);
            PlayerPrefs.Save();
        }

        private static string HashKey(string displayName)
        {
            var normalized = displayName.Trim().ToLowerInvariant();
            return Prefix + Sha256Hex(normalized).Substring(0, 20);
        }

        private static string Hash(string value)
        {
            return Sha256Hex(Salt + value);
        }

        private static string Sha256Hex(string value)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(value));
            var builder = new StringBuilder(bytes.Length * 2);

            foreach (var b in bytes)
                builder.Append(b.ToString("x2"));

            return builder.ToString();
        }

        private static bool FixedEquals(string a, string b)
        {
            if (a.Length != b.Length) return false;

            var result = 0;
            for (var i = 0; i < a.Length; i++)
                result |= a[i] ^ b[i];

            return result == 0;
        }
    }
}
