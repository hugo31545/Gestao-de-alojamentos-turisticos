using POOfase1trabalho13_11.Models.Repositories.Interfaces;
using POOfase1trabalho13_11.Models.Services.Interfaces;
using POO.Utils.Security;

namespace POOfase1trabalho13_11.Models.Services.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;

        public AuthenticationService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public bool UserExists(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return false;

            try
            {
                // Procura o utilizador no repositório
                var user = _userRepository.GetUserByUsername(username);

               
                return user != null && user.Password == password;
            }
            catch (Exception)
            {
                
                return false;
            }
        }

        
        public bool CreateUser(string username, string password, string passwordRepeat)
        {
           
            if (string.IsNullOrWhiteSpace(username) || username.Length < 3) return false;

           
            if (!PasswordValidator.Validar(password, out string mensagemErro))
            {
                
                System.Diagnostics.Debug.WriteLine($"Falha na password: {mensagemErro}");
                return false;
            }
            if (string.IsNullOrWhiteSpace(password) || password.Length < 4) return false;
            if (password != passwordRepeat) return false;

            return _userRepository.CreateUser(username, password);
        }
    }
}


        
           