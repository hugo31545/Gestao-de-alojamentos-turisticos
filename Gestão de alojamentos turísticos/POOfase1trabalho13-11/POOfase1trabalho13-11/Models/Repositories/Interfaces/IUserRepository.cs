

namespace POOfase1trabalho13_11.Models.Repositories.Interfaces
{
    /// <summary>
    /// Define as operações específicas para a gestão de persistência de utilizadores.
    /// Herda as operações básicas de IRepository (Add, Remove, GetAll, etc).
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        #region Methods

        /// <summary>
        /// Procura um utilizador com base no seu nome de utilizador (Username).
        /// </summary>
        /// <param name="username">O nome de utilizador a pesquisar.</param>
        /// <returns>O <see cref="User"/> correspondente ou null se não for encontrado.</returns>
        User? GetUserByUsername(string username);

        /// <summary>
        /// Realiza a criação rápida de um utilizador no suporte de armazenamento.
        /// </summary>
        /// <param name="username">Nome de utilizador único.</param>
        /// <param name="password">Palavra-passe.</param>
        /// <returns>True se a criação foi bem sucedida.</returns>
        bool CreateUser(string username, string password);

        #endregion
    }
}