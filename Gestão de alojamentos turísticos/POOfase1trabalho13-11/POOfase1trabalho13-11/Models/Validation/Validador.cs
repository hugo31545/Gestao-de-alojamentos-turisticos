//-----------------------------------------------------------------
//    <copyright file="Validador.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>1</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using System.Net.Mail;

namespace POOfase1trabalho13_11.Models.Validation
{
    /// <summary>
    /// Regras de validação de formato partilhadas pelas entidades e pelos serviços.
    /// </summary>
    public static class Validador
    {
        #region Methods
        /// <summary>
        /// Verifica se o NIF tem exatamente 9 dígitos.
        /// </summary>
        /// <param name="nif">O NIF a validar.</param>
        /// <returns><c>true</c> se o formato for válido.</returns>
        public static bool NifValido(string? nif)
        {
            return nif != null && nif.Length == 9 && nif.All(char.IsDigit);
        }

        /// <summary>
        /// Verifica se o email tem um formato válido (utilizador@dominio.tld).
        /// </summary>
        /// <param name="email">O email a validar.</param>
        /// <returns><c>true</c> se o formato for válido.</returns>
        public static bool EmailValido(string? email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            return MailAddress.TryCreate(email, out var endereco)
                   && endereco.Address == email
                   && endereco.Host.Contains('.')
                   && !endereco.Host.EndsWith('.');
        }
        #endregion
    }
}
