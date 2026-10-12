//-----------------------------------------------------------------
//    <copyright file="LoginViewModel.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.Models.Services.Interfaces;
using POOfase1trabalho13_11.Views.Enums;
using System.Windows.Input;
using POOfase1trabalho13_11.ViewModels.Commands;
using POOfase1trabalho13_11.Views.Interfaces;

namespace POOfase1trabalho13_11.ViewModels
{
    /// <summary>
    /// ViewModel responsável pela lógica de autenticação, validação de credenciais
    /// e navegação inicial entre o Login, Registo e Janela Principal.
    /// </summary>
    public class LoginViewModel : BaseViewModel
    {
        #region Fields

        private readonly IAuthenticationService _authenticationService;
        private readonly IViewFactory _viewFactory;
        private readonly ISessionService _sessao;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        /// Ação delegada para fechar a janela atual. 
        /// É injetada pela View no Code-behind (Login.xaml.cs).
        /// </summary>
        public Action? CloseWindowAction { get; set; }


        /// <summary>
        /// Nome de utilizador introduzido na View.
        /// Limpa mensagens de erro e reavalia a execução do comando de login ao mudar.
        /// </summary>
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


        /// <summary>
        /// Palavra-passe introduzida na View.
        /// Reavalia a execução do comando de login em cada alteração.
        /// </summary>
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
        /// Mensagem informativa de erro exibida na interface (ex: "Credenciais Inválidas").
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

        /// <summary>
        /// Inicializa a ViewModel injetando os serviços de autenticação e a fábrica de janelas.
        /// </summary>
        public LoginViewModel(IAuthenticationService authenticationService, IViewFactory viewFactory, ISessionService sessao)
        {
            _viewFactory = viewFactory ?? throw new ArgumentNullException(nameof(viewFactory));
            _sessao = sessao ?? throw new ArgumentNullException(nameof(sessao));
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));

            LoginCommand = new ViewModelCommand(ExecuteLogin, CanExecuteLogin);
            RegistryCommand = new ViewModelCommand(ExecuteRegistry);
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Define a lógica de habilitação do botão de Login.
        /// O comando só pode ser executado se o utilizador e a password não estiverem vazios.
        /// </summary>
        private bool CanExecuteLogin(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
        }

        /// <summary>
        /// Executa a tentativa de autenticação e navega para a MainView em caso de sucesso.
        /// </summary>
        private void ExecuteLogin(object? parameter)
        {
            try
            {
           
                if (_authenticationService.UserExists(Username, Password))
                {
                    // Guarda o utilizador para a janela principal e para a auditoria
                    _sessao.Iniciar(Username);

                    // Utiliza a fábrica para criar a janela principal sem acoplar a ViewModel a uma classe concreta
                    var mainWindow = _viewFactory.CreateView(ViewType.Main);

                   
                    mainWindow.Show();

                    // Fecha a janela de login
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

        /// <summary>
        /// Navega para a janela de registo de novos utilizadores.
        /// </summary>
        private void ExecuteRegistry(object? parameter)
        {
            var registryWindow = _viewFactory.CreateView(ViewType.Registry);
            registryWindow.Show();
            CloseWindowAction?.Invoke();
        }


        /// <summary>
        /// Helper para atualização de campos privados e notificação de propriedades.
        /// </summary>
        protected bool SetField<T>(ref T field, T value)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            return true;
        }

        #endregion
    }
}