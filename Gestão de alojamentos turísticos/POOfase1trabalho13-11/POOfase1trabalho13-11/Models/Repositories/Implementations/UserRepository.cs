

using POOfase1trabalho13_11.Models.Repositories.Interfaces;

namespace POOfase1trabalho13_11.Models.Repositories.Implementations
{
    /// <summary>
    /// Repositório especializado para a gestão de utilizadores (autenticação).
    /// Herda a lógica de persistência JSON da classe base genérica.
    /// </summary>
    public class UserRepository : JsonRepository<User>, IUserRepository
    {
        #region Constructors

        /// <summary>
        /// Inicializa o repositório de utilizadores apontando para o ficheiro users.json.
        /// </summary>
        public UserRepository() : base("users.json")
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Procura um utilizador pelo seu nome de utilizador único.
        /// </summary>
        /// <param name="username">O username a pesquisar.</param>
        /// <returns>A entidade <see cref="User"/> se encontrada; caso contrário, null.</returns>
        public User? GetUserByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;

            // Utilizamos a lista 'Items' que já foi carregada pela classe base
            return Items.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Cria um novo utilizador e persiste-o no armazenamento JSON.
        /// </summary>
        /// <param name="username">Nome de utilizador</param>
        /// <param name="password">Palavra-passe</param>
        /// <returns>True se o utilizador foi criado; False se já existir.</returns>
        public bool CreateUser(string username, string password)
        {
            // 1. Validação 
            if (GetUserByUsername(username) != null)
                return false;

            // 2. Gerar ID (procurar o maior ID atual e somar 1)
            int novoId = Items.Any() ? Items.Max(u => u.Id) + 1 : 1;

            // 3. Criar e Adicionar
            var novoUser = new User
            {
                Id = novoId,
                Username = username,
                Password = password,
                DataRegisto = DateTime.Now
            };

            
            Add(novoUser);

            return true;
        }

        #endregion
    }
}