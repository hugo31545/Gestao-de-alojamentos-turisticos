//-----------------------------------------------------------------
//    <copyright file="RegistryViewModel.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.ViewModels.Services;
using POO.Utils.Security;
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Validation;
using POOfase1trabalho13_11.Models.Services.Interfaces;
using POOfase1trabalho13_11.ViewModels.Commands;
using POOfase1trabalho13_11.Views.Enums;
using POOfase1trabalho13_11.Views.Factories;
using POOfase1trabalho13_11.Views.Interfaces;
using System;
using System.Windows.Input;

namespace POOfase1trabalho13_11.ViewModels
{

    /// <summary>
    /// ViewModel responsável pelo processo de auto-registo de novos funcionários no sistema.
    /// Coordena a criação simultânea de credenciais de acesso e perfil de funcionário.
    /// </summary>
    public class RegistryViewModel : BaseViewModel
    {
        private readonly IDialogService _dialog;

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
        // Cada propriedade notifica a UI e reavalia a validade do comando de registo
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

        /// <summary>
        /// Mensagem de erro ou sucesso exibida na zona de estado da janela de registo.
        /// </summary>
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

        // Validação em tempo real. Um campo vazio não mostra erro (mostra o placeholder);
        // o erro aparece assim que o utilizador escreve algo inválido.
        public string NomeErro =>
            Nome.Length > 0 && string.IsNullOrWhiteSpace(Nome) ? "O nome é obrigatório." : string.Empty;

        public string EmailErro =>
            Email.Length == 0 || Validador.EmailValido(Email) ? string.Empty : "Email inválido. Use o formato nome@dominio.pt";

        public string NifErro =>
            NIF.Length == 0 || Validador.NifValido(NIF) ? string.Empty
            : !NIF.All(char.IsDigit) ? "O NIF só pode conter dígitos."
            : $"O NIF deve ter 9 dígitos (tem {NIF.Length}).";

        public string PasswordErro =>
            Password.Length == 0 || PasswordValidator.Validar(Password, out string motivo) ? string.Empty : motivo;

        public string ConfirmacaoErro =>
            PasswordConfirmacao.Length == 0 || PasswordConfirmacao == Password ? string.Empty : "As palavras-passe não coincidem.";

        public string CargoErro =>
            string.IsNullOrWhiteSpace(Cargo) ? "O cargo é obrigatório." : string.Empty;

        /// <summary>
        /// Explica ao utilizador porque é que o botão de registo está desativado.
        /// </summary>
        public string AjudaFormulario =>
            PodeRegistar() ? string.Empty : "Preencha todos os campos corretamente para poder finalizar o registo.";

        /// <summary>
        /// Verdadeiro quando o registo é uma janela própria (aberta a partir do Login),
        /// caso em que se pode voltar ao Login.
        /// </summary>
        public bool PodeVoltar => CloseWindowAction != null;
        #endregion

        #region Commands
        public ICommand RegistarCommand { get; }
        public ICommand LimparCamposCommand { get; }
        public ICommand VoltarCommand { get; }
        #endregion

        /// <summary>
        /// Ação para fechar a janela de registo após sucesso ou cancelamento.
        /// </summary>
        public Action? CloseWindowAction { get; set; }


        #region Constructor
        public RegistryViewModel(IEmployeeService employeeService, IRegistoService registoService,
                         IViewFactory viewFactory, IAuthenticationService authService, IDialogService dialogService)
        {
            _dialog = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _employeeService = employeeService;
            _registoService = registoService;
            _viewFactory = viewFactory;
            _authService = authService; // Inicializar

            RegistarCommand = new ViewModelCommand(p => ExecutarRegisto(), p => PodeRegistar());
            LimparCamposCommand = new ViewModelCommand(p => LimparCampos());
            VoltarCommand = new ViewModelCommand(p => VoltarAoLogin());
        }
        #endregion

        #region Methods

        /// <summary>
        /// Validação em tempo real (CanExecute). 
        /// O botão só fica ativo com todos os campos preenchidos e sem erros de validação.
        /// </summary>
        private bool PodeRegistar()
        {
            // Todos os campos preenchidos e sem nenhum erro de validação
            return Nome.Length > 0 && Email.Length > 0 && NIF.Length > 0 &&
                   Password.Length > 0 && PasswordConfirmacao.Length > 0 && Cargo.Length > 0 &&
                   string.IsNullOrEmpty(NomeErro) && string.IsNullOrEmpty(EmailErro) &&
                   string.IsNullOrEmpty(NifErro) && string.IsNullOrEmpty(PasswordErro) &&
                   string.IsNullOrEmpty(ConfirmacaoErro) && string.IsNullOrEmpty(CargoErro);
        }

        private void NotificarComandos()
        {
            (RegistarCommand as ViewModelCommand)?.RaiseCanExecuteChanged(); // reavalia CanExecute

            // Atualiza as mensagens de validação mostradas por baixo de cada campo
            OnPropertyChanged(nameof(NomeErro));
            OnPropertyChanged(nameof(EmailErro));
            OnPropertyChanged(nameof(NifErro));
            OnPropertyChanged(nameof(PasswordErro));
            OnPropertyChanged(nameof(ConfirmacaoErro));
            OnPropertyChanged(nameof(CargoErro));
            OnPropertyChanged(nameof(AjudaFormulario));
        }

        private void VoltarAoLogin()
        {
            if (CloseWindowAction == null) return;

            var loginWindow = _viewFactory.CreateView(ViewType.Login);
            loginWindow.Show();
            CloseWindowAction.Invoke();
        }


        /// <summary>
        /// Lógica principal de registo. Tenta criar o utilizador e, em seguida, o funcionário.
        /// </summary>
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

                // Validar os dados do funcionário (formato e duplicados) antes de gravar
                if (!_employeeService.ValidarNovoFuncionario(Nome, Email, NIF, out string mensagem))
                {
                    MensagemFeedback = $"Erro: {mensagem}";
                    return;
                }

                // 2. EXECUÇÃO DA CRIAÇÃO (Ordem lógica)

                // Primeiro criamos o User (Login)
                if (!_authService.CreateUser(Email, Password, PasswordConfirmacao, out mensagem))
                {
                    MensagemFeedback = $"Erro: {mensagem}";
                    return;
                }

                // Se o utilizador foi criado, criamos o perfil de funcionário
                if (!_employeeService.AdicionarFuncionario(Nome, Email, NIF, Cargo))
                {
                    // Desfaz a criação do User para não deixar uma conta sem funcionário
                    _authService.RemoveUser(Email);
                    MensagemFeedback = "Erro ao guardar os detalhes do funcionário. Nenhum dado foi gravado.";
                    return;
                }

                _dialog.Informar("Registo completo! Utilizador e Funcionário criados.", "Sucesso");

                if (CloseWindowAction != null)
                {
                    // Janela de registo aberta a partir do Login: regressa ao Login
                    var loginWin = _viewFactory.CreateView(ViewType.Login);
                    loginWin.Show();
                    CloseWindowAction.Invoke();
                }
                else
                {
                    // Página dentro da janela principal: fica na página, pronta para outro registo
                    LimparCampos();
                    MensagemFeedback = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MensagemFeedback = $"Erro crítico: {ex.Message}";
            }
        }

        /// <summary>
        /// Reseta todos os campos do formulário para o estado inicial.
        /// </summary>
        private void LimparCampos()
        {
            _nome = string.Empty;
            _email = string.Empty;
            _nif = string.Empty;
            _cargo = "Funcionário";
            _password = string.Empty;
            _passwordConfirmacao = string.Empty;
            OnPropertyChanged(nameof(Nome));
            OnPropertyChanged(nameof(Email));
            OnPropertyChanged(nameof(NIF));
            OnPropertyChanged(nameof(Cargo));
            OnPropertyChanged(nameof(Password));
            OnPropertyChanged(nameof(PasswordConfirmacao));
            NotificarComandos();
        }
        #endregion
    }
}