//-----------------------------------------------------------------
//    <copyright file="IAuthenticationService.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
namespace POOfase1trabalho13_11.Models.Services.Interfaces

{
    /// <summary>
    /// Define os métodos necessários para serviços de autenticação e gestão de contas de utilizador.
    /// </summary>
    public interface IAuthenticationService
    {
        #region Methods

        /// <summary>
        /// Valida as credenciais de um utilizador no sistema.
        /// </summary>
        /// <param name="username">Nome de utilizador.</param>
        /// <param name="password">Palavra-passe em texto limpo.</param>
        /// <returns>True se o utilizador existe e a password coincide; caso contrário, false.</returns>
        bool UserExists(string username, string password);

        /// <summary>
        /// Cria um novo utilizador no sistema após validações básicas.
        /// </summary>
        /// <param name="username">Nome de utilizador pretendido.</param>
        /// <param name="password">Palavra-passe.</param>
        /// <param name="passwordRepeat">Confirmação da palavra-passe.</param>
        /// <param name="mensagem">Motivo da falha, ou mensagem de sucesso.</param>
        /// <returns>True se o utilizador foi criado com sucesso.</returns>
        bool CreateUser(string username, string password, string passwordRepeat, out string mensagem);

        /// <summary>
        /// Remove a conta de um utilizador (por exemplo, para desfazer um registo incompleto).
        /// </summary>
        /// <param name="username">Nome de utilizador a remover.</param>
        /// <returns>True se a conta existia e foi removida.</returns>
        bool RemoveUser(string username);

        #endregion
    }
}