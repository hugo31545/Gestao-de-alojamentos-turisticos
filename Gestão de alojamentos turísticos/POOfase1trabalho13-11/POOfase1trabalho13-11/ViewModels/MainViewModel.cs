using Microsoft.Extensions.DependencyInjection;
using POOfase1trabalho13_11.ViewModels.Commands;
using POOfase1trabalho13_11.Views.Enums;
using POOfase1trabalho13_11.Views.Interfaces;
using System;
using System.Windows;
using System.Windows.Input;

namespace POOfase1trabalho13_11.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Fields
        private readonly IServiceProvider _serviceProvider;
        private object? _currentView; 
        private string _usernameLogado = string.Empty;
        private readonly IViewFactory _viewFactory;
        public Action? CloseWindowAction { get; set; }
        #endregion

        #region Properties
        public object? CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public string UsernameLogado
        {
            get => _usernameLogado;
            set => SetProperty(ref _usernameLogado, value);
        }
        #endregion

        #region Commands
        public ICommand ShowAlojamentosCommand { get; }
        public ICommand ShowClientesCommand { get; }
        public ICommand ShowReservasCommand { get; }
        public ICommand ShowAuditoriaCommand { get; }
        public ICommand ShowRegistryCommand { get; }
        public ICommand LogoutCommand { get; }
        #endregion

        #region Constructor
        public MainViewModel(IServiceProvider serviceProvider, IViewFactory viewFactory)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _viewFactory = viewFactory;
            
            UsernameLogado = "Utilizador";

            // Inicialização dos comandos
            ShowAlojamentosCommand = new ViewModelCommand(p => Navegar<AlojamentoViewModel>());
            ShowClientesCommand = new ViewModelCommand(p => Navegar<ClienteViewModel>());
            ShowReservasCommand = new ViewModelCommand(p => Navegar<ReservaViewModel>());
            ShowAuditoriaCommand = new ViewModelCommand(p => Navegar<AuditoriaViewModel>());
           
            ShowRegistryCommand = new ViewModelCommand(p => Navegar<RegistryViewModel>());

            LogoutCommand = new ViewModelCommand(p => ExecutarLogout());

            
            Navegar<AlojamentoViewModel>();
        }
        #endregion

        #region Helper Methods
       
        private void Navegar<T>() where T : BaseViewModel
        {
            try
            {
                
                var viewModel = _serviceProvider.GetRequiredService<T>();
                CurrentView = viewModel;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar a página: {ex.Message}", "Erro de Navegação",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                System.Diagnostics.Debug.WriteLine($"Erro crítico DI: {ex}");
            }
        }

        private void ExecutarLogout()
        {
            var result = MessageBox.Show("Deseja realmente sair do sistema?", "Confirmar Logout",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
               
                var loginWindow = _viewFactory.CreateView(ViewType.Login);
                loginWindow.Show();

              
                CloseWindowAction?.Invoke();
            }
        }
        #endregion
    }
}