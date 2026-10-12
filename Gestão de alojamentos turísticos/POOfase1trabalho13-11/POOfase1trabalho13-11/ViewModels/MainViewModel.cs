//-----------------------------------------------------------------
//    <copyright file="MainViewModel.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.Models.Services.Interfaces;
using POOfase1trabalho13_11.ViewModels.Services;
using Microsoft.Extensions.DependencyInjection;
using POOfase1trabalho13_11.ViewModels.Commands;
using POOfase1trabalho13_11.Views.Enums;
using POOfase1trabalho13_11.Views.Interfaces;
using System;
using System.Windows.Input;

namespace POOfase1trabalho13_11.ViewModels
{
    /// <summary>
    /// ViewModel principal que atua como o Content Shell da aplicação.
    /// Gere a navegação entre os diferentes módulos e o estado da sessão do utilizador.
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        private readonly IDialogService _dialog;

        #region Fields
        private readonly IServiceProvider _serviceProvider;
        private object? _currentView; 
        private string _usernameLogado = string.Empty;
        private readonly IViewFactory _viewFactory;
        private readonly ISessionService _sessao;
        
        #endregion

        #region Properties

        /// <summary>
        /// Define a ViewModel ativa que será renderizada na região principal da View (ContentControl).
        /// </summary>
        public object? CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }


        /// <summary>
        /// Nome do utilizador autenticado para exibição no Dashboard.
        /// </summary>
        public string UsernameLogado
        {
            get => _usernameLogado;
            set => SetProperty(ref _usernameLogado, value);
        }


        /// <summary>
        /// Ação para fechar a janela principal, geralmente invocada durante o Logout.
        /// </summary>
        public Action? CloseWindowAction { get; set; }
        #endregion

        #region Commands
        public ICommand ShowAlojamentosCommand { get; }
        public ICommand ShowClientesCommand { get; }
        public ICommand ShowReservasCommand { get; }
        public ICommand ShowAuditoriaCommand { get; }
        public ICommand ShowRelatoriosCommand { get; }
        public ICommand ShowRegistryCommand { get; }
        public ICommand LogoutCommand { get; }
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa a MainViewModel e define a página inicial.
        /// </summary>
        public MainViewModel(IServiceProvider serviceProvider, IViewFactory viewFactory, ISessionService sessao, IDialogService dialogService)
        {
            _dialog = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _viewFactory = viewFactory;
            _sessao = sessao ?? throw new ArgumentNullException(nameof(sessao));
            
            UsernameLogado = _sessao.UtilizadorAtual ?? "Utilizador";

            // Mapeamento de comandos para os métodos de navegação genéricos
            ShowAlojamentosCommand = new ViewModelCommand(p => Navegar<AlojamentoViewModel>());
            ShowClientesCommand = new ViewModelCommand(p => Navegar<ClienteViewModel>());
            ShowReservasCommand = new ViewModelCommand(p => Navegar<ReservaViewModel>());
            ShowAuditoriaCommand = new ViewModelCommand(p => Navegar<AuditoriaViewModel>());
            ShowRelatoriosCommand = new ViewModelCommand(p => Navegar<RelatoriosViewModel>());
           
            ShowRegistryCommand = new ViewModelCommand(p => Navegar<RegistryViewModel>());

            LogoutCommand = new ViewModelCommand(p => ExecutarLogout());

            // Define a página de Alojamentos como a vista inicial por defeito
            Navegar<AlojamentoViewModel>();
        }
        #endregion

        #region Helper Methods


        /// <summary>
        /// Resolve a ViewModel solicitada via Content IoC e atualiza a View principal.
        /// </summary>
        /// <typeparam name="T">O tipo da ViewModel (deve herdar de BaseViewModel).</typeparam>
        private void Navegar<T>() where T : BaseViewModel
        {
            try
            {
                // Obtém a instância da ViewModel já com todas as suas dependências injetadas
                var viewModel = _serviceProvider.GetRequiredService<T>();
                CurrentView = viewModel;
            }
            catch (Exception ex)
            {
                _dialog.Erro($"Erro ao carregar a página: {ex.Message}", "Erro de Navegação");
                System.Diagnostics.Debug.WriteLine($"Erro crítico DI: {ex}");
            }
        }


        /// <summary>
        /// Finaliza a sessão atual e regressa ao ecrã de Login.
        /// </summary>
        private void ExecutarLogout()
        {
            bool result = _dialog.Confirmar("Deseja realmente sair do sistema?", "Confirmar Logout");

            if (result)
            {
                _sessao.Terminar();

                // Regressa ao Login usando a Factory
                var loginWindow = _viewFactory.CreateView(ViewType.Login);
                loginWindow.Show();

                // Fecha a MainWindow
                CloseWindowAction?.Invoke();
            }
        }
        #endregion
    }
}