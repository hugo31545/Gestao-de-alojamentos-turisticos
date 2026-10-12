//-----------------------------------------------------------------
//    <copyright file="AuthenticationService.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>3</version>
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
    /// As palavras-passe nunca são guardadas em texto simples: apenas o hash (ver <see cref="PasswordHasher"/>).
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
        /// Contas antigas com a password em texto simples são convertidas para hash no primeiro login correto.
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
                var user = _userRepository.GetUserByUsername(username);
                if (user == null) return false;

                if (!string.IsNullOrEmpty(user.PasswordHash))
                    return PasswordHasher.Verificar(password, user.PasswordHash);

                // Conta antiga (Fase 2): password guardada em texto simples
                if (user.Password != null && user.Password == password)
                {
                    user.PasswordHash = PasswordHasher.Hash(password);
                    user.Password = null;
                    _userRepository.Update(user);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro na autenticação: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Valida os requisitos de segurança e cria um novo utilizador no sistema.
        /// </summary>
        /// <param name="username">Nome de utilizador pretendido.</param>
        /// <param name="password">Palavra-passe definida.</param>
        /// <param name="passwordRepeat">Confirmação da palavra-passe.</param>
        /// <param name="mensagem">Motivo da falha, ou mensagem de sucesso.</param>
        /// <returns>True se o utilizador for criado com sucesso; False se falhar na validação ou se já existir.</returns>
        public bool CreateUser(string username, string password, string passwordRepeat, out string mensagem)
        {
            // 1. Validação básica de campos
            if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
            {
                mensagem = "O nome de utilizador deve ter pelo menos 3 caracteres.";
                return false;
            }

            // 2. Validação de consistência e confirmação
            if (password != passwordRepeat)
            {
                mensagem = "As palavras-passe não coincidem.";
                return false;
            }

            // 3. Validação complexa via DLL de Segurança
            if (!PasswordValidator.Validar(password, out string mensagemErro))
            {
                mensagem = mensagemErro;
                return false;
            }

            // 4. Delegar a criação definitiva ao repositório, apenas com o hash
            if (!_userRepository.CreateUser(username, PasswordHasher.Hash(password)))
            {
                mensagem = "Este nome de utilizador já está a ser utilizado.";
                return false;
            }

            mensagem = "Utilizador criado com sucesso.";
            return true;
        }

        /// <summary>
        /// Remove a conta de um utilizador. Usado para desfazer um registo incompleto.
        /// </summary>
        /// <param name="username">O nome de utilizador a remover.</param>
        /// <returns>True se a conta existia e foi removida.</returns>
        public bool RemoveUser(string username)
        {
            var user = _userRepository.GetUserByUsername(username);
            return user != null && _userRepository.Remove(user.Id);
        }
        #endregion
    }
}
