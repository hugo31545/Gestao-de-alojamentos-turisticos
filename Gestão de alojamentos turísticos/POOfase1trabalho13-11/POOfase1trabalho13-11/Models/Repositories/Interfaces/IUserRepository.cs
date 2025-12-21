//-----------------------------------------------------------------
//    <copyright file="IUserRepository.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
namespace POOfase1trabalho13_11.Models.Repositories.Interfaces
{
    /// <summary>
    /// Define as operações específicas para a gestão de persistência e autenticação de utilizadores.
    /// Herda as operações básicas da interface genérica <see cref="IRepository{T}"/>.
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        #region Methods

        /// <summary>
        /// Procura um utilizador no sistema com base no seu nome de utilizador exclusivo.
        /// </summary>
        /// <param name="username">O nome de utilizador a pesquisar.</param>
        /// <returns>O objeto <see cref="User"/> se encontrado; caso contrário, <c>null</c>.</returns>
        User? GetUserByUsername(string username);

        /// <summary>
        /// Realiza o registo de um novo utilizador no suporte de armazenamento,
        /// garantindo a persistência das credenciais de acesso.
        /// </summary>
        /// <param name="username">O nome de utilizador único para a nova conta.</param>
        /// <param name="password">A palavra-passe (deve chegar a este método já devidamente protegida).</param>
        /// <returns><c>true</c> se o utilizador foi criado com sucesso; <c>false</c> se o username já existir.</returns>
        bool CreateUser(string username, string password);

        #endregion
    }
}