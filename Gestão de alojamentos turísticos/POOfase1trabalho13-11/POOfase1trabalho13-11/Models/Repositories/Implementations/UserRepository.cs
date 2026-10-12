//-----------------------------------------------------------------
//    <copyright file="UserRepository.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Repositories.Interfaces;

namespace POOfase1trabalho13_11.Models.Repositories.Implementations
{
    /// <summary>
    /// Repositório especializado para a gestão de utilizadores e autenticação.
    /// Herda a lógica de persistência JSON da classe base genérica <see cref="JsonRepository{T}"/>.
    /// </summary>
    public class UserRepository : JsonRepository<User>, IUserRepository
    {
        #region Constructors

        /// <summary>
        /// Inicializa uma nova instância de <see cref="UserRepository"/> apontando para o ficheiro "users.json".
        /// </summary>
        public UserRepository() : base("users.json")
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Procura um utilizador pelo seu nome de utilizador único, ignorando maiúsculas e minúsculas.
        /// </summary>
        /// <param name="username">O nome de utilizador a pesquisar.</param>
        /// <returns>A entidade <see cref="User"/> se encontrada; caso contrário, null.</returns>
        public User? GetUserByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;

            // Utilizamos a lista 'Items' que já foi carregada pela classe base
            return Items.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Cria um novo utilizador, atribui um ID único e persiste os dados no ficheiro JSON.
        /// </summary>
        /// <param name="username">Nome de utilizador pretendido.</param>
        /// <param name="passwordHash">O hash da palavra-passe (nunca o texto simples).</param>
        /// <returns>True se o utilizador foi criado com sucesso; False se o nome de utilizador já existir.</returns>
        public bool CreateUser(string username, string passwordHash)
        {
            // 1. Validação de Unicidade: impede a criação de usernames duplicados
            if (GetUserByUsername(username) != null)
                return false;

            // 2. Gerar ID de auto-incremento
            int novoId = Items.Any() ? Items.Max(u => u.Id) + 1 : 1;

            // 3. Instanciar o novo objeto User
            var novoUser = new User
            {
                Id = novoId,
                Username = username,
                PasswordHash = passwordHash,
                DataRegisto = DateTime.Now
            };

            // 4. Adicionar à lista e disparar a gravação física no ficheiro via classe base
            Add(novoUser);

            return true;
        }

        #endregion
    }
}