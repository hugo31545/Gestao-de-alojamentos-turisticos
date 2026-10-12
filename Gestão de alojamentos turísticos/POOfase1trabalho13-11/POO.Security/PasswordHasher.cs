//-----------------------------------------------------------------
//    <copyright file="PasswordHasher.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>1.0</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------

using System.Security.Cryptography;

namespace POO.Utils.Security
{
    /// <summary>
    /// Gera e verifica hashes de palavras-passe com PBKDF2 (SHA-256) e salt aleatório.
    /// O resultado é guardado num único texto com o formato
    /// <c>PBKDF2$iterações$salt$hash</c> (salt e hash em Base64).
    /// </summary>
    public static class PasswordHasher
    {
        #region Constants
        private const string Prefixo = "PBKDF2";
        private const int TamanhoSalt = 16;
        private const int TamanhoHash = 32;
        private const int Iteracoes = 100_000;
        #endregion

        #region Methods
        /// <summary>
        /// Gera o hash de uma palavra-passe, com um salt novo em cada chamada.
        /// </summary>
        /// <param name="password">A palavra-passe em texto simples.</param>
        /// <returns>O texto a guardar no repositório.</returns>
        public static string Hash(string password)
        {
            ArgumentNullException.ThrowIfNull(password);

            byte[] salt = RandomNumberGenerator.GetBytes(TamanhoSalt);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iteracoes, HashAlgorithmName.SHA256, TamanhoHash);

            return $"{Prefixo}${Iteracoes}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }

        /// <summary>
        /// Verifica se a palavra-passe corresponde ao hash guardado.
        /// A comparação é feita em tempo constante.
        /// </summary>
        /// <param name="password">A palavra-passe introduzida.</param>
        /// <param name="hashGuardado">O valor produzido anteriormente por <see cref="Hash"/>.</param>
        /// <returns><c>true</c> se corresponder; caso contrário, <c>false</c>.</returns>
        public static bool Verificar(string password, string? hashGuardado)
        {
            if (password == null || string.IsNullOrEmpty(hashGuardado)) return false;

            string[] partes = hashGuardado.Split('$');
            if (partes.Length != 4 || partes[0] != Prefixo || !int.TryParse(partes[1], out int iteracoes) || iteracoes <= 0)
                return false;

            byte[] salt;
            byte[] esperado;
            try
            {
                salt = Convert.FromBase64String(partes[2]);
                esperado = Convert.FromBase64String(partes[3]);
            }
            catch (FormatException)
            {
                return false;
            }

            byte[] calculado = Rfc2898DeriveBytes.Pbkdf2(password, salt, iteracoes, HashAlgorithmName.SHA256, esperado.Length);
            return CryptographicOperations.FixedTimeEquals(calculado, esperado);
        }
        #endregion
    }
}
