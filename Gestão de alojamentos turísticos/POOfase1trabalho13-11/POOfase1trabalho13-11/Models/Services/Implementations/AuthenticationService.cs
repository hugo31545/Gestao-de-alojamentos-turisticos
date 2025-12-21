//-----------------------------------------------------------------
//    <copyright file="AuthenticationService.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.Models.Repositories.Interfaces;
using POOfase1trabalho13_11.Models.Services.Interfaces;
using POO.Utils.Security;

namespace POOfase1trabalho13_11.Models.Services.Implementations
{

    /// <summary>
    /// Fornece serviços de autenticação e gestão de contas de utilizador.
    /// Esta classe serve como ponte entre a interface gráfica e a persistência de credenciais.
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {

        #region Fields
        private readonly IUserRepository _userRepository;
        #endregion

        #region Constructors

        /// <summary>
        /// Inicializa o serviço de autenticação injetando a dependência do repositório de utilizadores.
        /// </summary>
        /// <param name="userRepository">Instância do repositório de utilizadores.</param>
        /// <exception cref="ArgumentNullException">Lançada se o repositório for nulo.</exception>
        public AuthenticationService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }
        #endregion


        #region Public Methods

        /// <summary>
        /// Verifica se as credenciais fornecidas (utilizador e password) coincidem com um registo existente.
        /// </summary>
        /// <param name="username">O nome de utilizador introduzido.</param>
        /// <param name="password">A palavra-passe introduzida.</param>
        /// <returns>True se o utilizador existir e a password estiver correta; caso contrário, False.</returns>
        public bool UserExists(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return false;

            try
            {
                // Procura o utilizador no repositório JSON
                var user = _userRepository.GetUserByUsername(username);

                // Comparação direta 
                return user != null && user.Password == password;
            }
            catch (Exception)
            {
                
                return false;
            }
        }

        /// <summary>
        /// Valida os requisitos de segurança e cria um novo utilizador no sistema.
        /// </summary>
        /// <param name="username">Nome de utilizador pretendido.</param>
        /// <param name="password">Palavra-passe definida.</param>
        /// <param name="passwordRepeat">Confirmação da palavra-passe.</param>
        /// <returns>True se o utilizador for criado com sucesso; False se falhar na validação ou se já existir.</returns>
        public bool CreateUser(string username, string password, string passwordRepeat)
        {
            // 1. Validação básica de campos
            if (string.IsNullOrWhiteSpace(username) || username.Length < 3) return false;

            // 2. Validação complexa via DLL de Segurança
            if (!PasswordValidator.Validar(password, out string mensagemErro))
            {
                // Log de debug para ajudar a perceber porque é que a password foi rejeitada
                System.Diagnostics.Debug.WriteLine($"Falha na password: {mensagemErro}");
                return false;
            }
            // 3. Validação de consistência e confirmação
            if (string.IsNullOrWhiteSpace(password) || password.Length < 4) return false;
            if (password != passwordRepeat) return false;

            // 4. Delegar a criação definitiva ao repositório
            return _userRepository.CreateUser(username, password);
        }
        #endregion
    }
}


        
           