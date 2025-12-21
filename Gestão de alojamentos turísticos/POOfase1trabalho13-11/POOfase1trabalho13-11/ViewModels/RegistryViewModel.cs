using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Services.Interfaces;
using POOfase1trabalho13_11.ViewModels.Commands;
using POOfase1trabalho13_11.Views.Enums;
using POOfase1trabalho13_11.Views.Factories;
using POOfase1trabalho13_11.Views.Interfaces;
using System;
using System.Windows;
using System.Windows.Input;

namespace POOfase1trabalho13_11.ViewModels
{
    public class RegistryViewModel : BaseViewModel
    {
        #region Fields
        private readonly IEmployeeService _employeeService;
        private readonly IRegistoService _registoService;
        private readonly IViewFactory _viewFactory;
        private readonly IAuthenticationService _authService;
        private string _password = string.Empty;
        private string _passwordConfirmacao = string.Empty;
        private string _nome = string.Empty;
        private string _email = string.Empty;
        private string _nif = string.Empty;
        private string _cargo = "Funcionário";
        private string _mensagemFeedback = string.Empty;
        #endregion

        #region Properties
        public string Nome
        {
            get => _nome;
            set { _nome = value; OnPropertyChanged(nameof(Nome)); NotificarComandos(); }
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(nameof(Email)); NotificarComandos(); }
        }

        public string NIF
        {
            get => _nif;
            set { _nif = value; OnPropertyChanged(nameof(NIF)); NotificarComandos(); }
        }

        public string Cargo
        {
            get => _cargo;
            set { _cargo = value; OnPropertyChanged(nameof(Cargo)); NotificarComandos(); }
        }

        public string MensagemFeedback
        {
            get => _mensagemFeedback;
            set { _mensagemFeedback = value; OnPropertyChanged(nameof(MensagemFeedback)); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(nameof(Password)); NotificarComandos(); }
        }

        public string PasswordConfirmacao
        {
            get => _passwordConfirmacao;
            set { _passwordConfirmacao = value; OnPropertyChanged(nameof(PasswordConfirmacao)); NotificarComandos(); }
        }
        #endregion

        #region Commands
        public ICommand RegistarCommand { get; }
        public ICommand LimparCamposCommand { get; }
        #endregion
        public Action? CloseWindowAction { get; set; }


        #region Constructor
        public RegistryViewModel(IEmployeeService employeeService, IRegistoService registoService,
                         IViewFactory viewFactory, IAuthenticationService authService)
        {
            _employeeService = employeeService;
            _registoService = registoService;
            _viewFactory = viewFactory;
            _authService = authService; // Inicializar

            RegistarCommand = new ViewModelCommand(p => ExecutarRegisto(), p => PodeRegistar());
            LimparCamposCommand = new ViewModelCommand(p => LimparCampos());
        }
        #endregion

        #region Methods
        private bool PodeRegistar()
        {
            return !string.IsNullOrWhiteSpace(Nome) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(NIF) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   Password == PasswordConfirmacao && // Passwords devem coincidir
                   Password.Length >= 4;
        }

        private void NotificarComandos()
        {
            (RegistarCommand as ViewModelCommand)?.RaiseCanExecuteChanged();
        }
        private void ExecutarRegisto()
        {
            try
            {
                // 1. VALIDAÇÕES PRÉVIAS (Não grava nada ainda)
                // Verificar se as passwords coincidem (camada de UI/VM)
                if (Password != PasswordConfirmacao)
                {
                    MensagemFeedback = "As palavras-passe não coincidem.";
                    return;
                }

                // Verificar NIF duplicado (Regra de Negócio do Employee)
                if (_employeeService.VerificarNifDuplicado(NIF))
                {
                    MensagemFeedback = "Erro: Este NIF já está registado em outro funcionário.";
                    return;
                }

                // 2. EXECUÇÃO DA CRIAÇÃO (Ordem lógica)

                // Primeiro tentamos criar o User (Login)
               
                bool userCriado = _authService.CreateUser(Email, Password, PasswordConfirmacao);

                if (!userCriado)
                {
                    MensagemFeedback = "Erro: O Email (Username) já está a ser utilizado.";
                    return;
                }

                // Se o utilizador foi criado, criamos o perfil de funcionário
                bool sucessoFunc = _employeeService.AdicionarFuncionario(Nome, Email, NIF, Cargo);

                if (sucessoFunc)
                {
                    MessageBox.Show("Registo completo! Utilizador e Funcionário criados.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Navegação
                    var loginWin = _viewFactory.CreateView(ViewType.Login);
                    loginWin.Show();
                    CloseWindowAction?.Invoke();
                }
                else
                {
                    // Caso extremo: User criado mas Employee falhou
                    MensagemFeedback = "O utilizador foi criado, mas houve um erro ao guardar os detalhes do funcionário.";
                }
            }
            catch (Exception ex)
            {
                MensagemFeedback = $"Erro crítico: {ex.Message}";
            }
        }

        private void LimparCampos()
        {
            _nome = string.Empty;
            _email = string.Empty;
            _nif = string.Empty;
            _cargo = "Funcionário";
            OnPropertyChanged(nameof(Nome));
            OnPropertyChanged(nameof(Email));
            OnPropertyChanged(nameof(NIF));
            OnPropertyChanged(nameof(Cargo));
        }
        #endregion
    }
}