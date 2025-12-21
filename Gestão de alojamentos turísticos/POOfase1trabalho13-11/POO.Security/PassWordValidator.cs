//-----------------------------------------------------------------
//    <copyright file="PasswordValidator.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>1.0</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
namespace POO.Utils.Security
{
    /// <summary>
    /// Fornece métodos auxiliares para validação de critérios de segurança de credenciais.
    /// Esta classe é estática para permitir a sua utilização global sem necessidade de instanciação.
    /// </summary>
    public static class PasswordValidator
    {

        #region Methods
        /// <summary>
        /// Valida se uma palavra-passe cumpre os requisitos mínimos de segurança do sistema.
        /// </summary>
        /// <param name="password">A string a ser validada.</param>
        /// <param name="mensagemErro">Retorna a descrição do motivo da falha de validação, caso ocorra.</param>
        /// <returns>Verdadeiro se a password for segura; caso contrário, falso.</returns>
        public static bool Validar(string password, out string mensagemErro)
        {
            mensagemErro = string.Empty;

            // 1. Validação de nulidade ou espaços em branco
            if (string.IsNullOrWhiteSpace(password))
            {
                
                return false;
            }

            // 2. Validação de comprimento mínimo
            // Requisito: Segurança base para evitar ataques de força bruta simples.
            if (password.Length < 6)
            {
                
                return false;
            }

            // verificar se contém pelo menos um número
            if (!System.Linq.Enumerable.Any(password, char.IsDigit))
            {
                
                return false;
            }

            return true;
        }
        #endregion
    }
}