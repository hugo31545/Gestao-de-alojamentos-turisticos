
using POOfase1trabalho13_11.Models.Services.Interfaces;
using POOfase1trabalho13_11.Views.Enums;
using System.Windows.Input;
using POOfase1trabalho13_11.ViewModels.Commands;
using POOfase1trabalho13_11.Views.Interfaces;

namespace POOfase1trabalho13_11.ViewModels
{
    /// <summary>
    /// ViewModel responsável pela lógica de autenticação e navegação inicial.
    /// </summary>
    public class LoginViewModel : BaseViewModel
    {
        #region Fields

        private readonly IAuthenticationService _authenticationService;
        private readonly IViewFactory _viewFactory;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;

        #endregion

        #region Properties

        
        public Action? CloseWindowAction { get; set; }

        public string Username
        {
            get => _username;
            set
            {
                if (SetField(ref _username, value)) 
                {
                    OnPropertyChanged(nameof(Username));
                    ErrorMessage = string.Empty;
                  
                    (LoginCommand as ViewModelCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (SetField(ref _password, value))
                {
                    OnPropertyChanged(nameof(Password));
                    ErrorMessage = string.Empty;
                    (LoginCommand as ViewModelCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Permite mostrar mensagens de erro diretamente na UI sem MessageBox.
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(nameof(ErrorMessage)); }
        }

        public ICommand LoginCommand { get; }
        public ICommand RegistryCommand { get; }

        #endregion

        #region Constructors

        public LoginViewModel(IAuthenticationService authenticationService, IViewFactory viewFactory)
        {
            _viewFactory = viewFactory ?? throw new ArgumentNullException(nameof(viewFactory));
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));

            LoginCommand = new ViewModelCommand(ExecuteLogin, CanExecuteLogin);
            RegistryCommand = new ViewModelCommand(ExecuteRegistry);
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Validação em tempo real: O botão de login só ativa se houver dados.
        /// </summary>
        private bool CanExecuteLogin(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
        }

        private void ExecuteLogin(object? parameter)
        {
            try
            {
           
                if (_authenticationService.UserExists(Username, Password))
                {
                  
                    var mainWindow = _viewFactory.CreateView(ViewType.Main);

                   
                    mainWindow.Show();

                   
                    CloseWindowAction?.Invoke();
                }
                else
                {
                    ErrorMessage = "Utilizador ou palavra-passe incorretos.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Erro técnico ao aceder ao sistema.";
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        
        private void ExecuteRegistry(object? parameter)
        {
            var registryWindow = _viewFactory.CreateView(ViewType.Registry);
            registryWindow.Show();
            CloseWindowAction?.Invoke();
        }

        protected bool SetField<T>(ref T field, T value)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            return true;
        }

        #endregion
    }
}