using Microsoft.Extensions.DependencyInjection;
using POOfase1trabalho13_11.Models.Repositories;
using POOfase1trabalho13_11.Models.Repositories.Implementations;
using POOfase1trabalho13_11.Models.Repositories.Interfaces;
using POOfase1trabalho13_11.Models.Services.Implementations;
using POOfase1trabalho13_11.Models.Services.Interfaces;
using POOfase1trabalho13_11.ViewModels;
using POOfase1trabalho13_11.Views;
using POOfase1trabalho13_11.Views.Enums;
using POOfase1trabalho13_11.Views.Factories;
using POOfase1trabalho13_11.Views.Interfaces;
using System;
using System.Windows;

namespace POOfase1trabalho13_11
{
    public partial class App : Application
    {
        /// <summary>
        /// Exposição global do provedor de serviços (necessário para a navegação interna na MainViewModel).
        /// </summary>
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            // Chamamos a base
            base.OnStartup(e);

            // 1. Configurar o contentor de injeção de dependência
            var services = new ServiceCollection();
            ConfigureServices(services);

            // 2. Construir o provedor
            ServiceProvider = services.BuildServiceProvider();

            // 3. Obter a Factory
            var viewFactory = ServiceProvider.GetRequiredService<IViewFactory>();

            // 4. FLUXO DE ENTRADA: Abrir sempre o Login primeiro
            // A MainWindow só será aberta pela LoginViewModel após sucesso
            var loginWindow = viewFactory.CreateView(ViewType.Login);
            loginWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // --- 0. INFRAESTRUTURA ---
            services.AddSingleton<IViewFactory, ViewFactory>();

            // --- 1. REPOSITÓRIOS ---
            
            services.AddSingleton<IAlojamentoRepository, AlojamentoRepository>();
            services.AddSingleton<IClienteRepository, ClienteRepository>();
            services.AddSingleton<ICheckInRepository, CheckInRepository>();
            services.AddSingleton<IConsultaRepository, ConsultaRepository>();
            services.AddSingleton<IReservaRepository, ReservaRepository>();
            services.AddSingleton<IRegistoRepository, RegistoRepository>();
            services.AddSingleton<IEmployeeRepository, EmployeeRepository>();
            services.AddSingleton<IUserRepository, UserRepository>();


            // --- 2. SERVIÇOS ---
            services.AddSingleton<IAlojamentoService, AlojamentoService>();
            services.AddSingleton<IClienteService, ClienteService>();
            services.AddSingleton<IReservaService, ReservaService>();
            services.AddSingleton<IRegistoService, RegistoService>();
            services.AddSingleton<IEmployeeService, EmployeeService>();
            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<ICheckInService, CheckInService>();
            services.AddSingleton<IConsultaService, ConsultaService>();

            // --- 3. VIEWMODELS ---
          
            services.AddSingleton<MainViewModel>();

            
            services.AddTransient<LoginViewModel>();
            services.AddTransient<AuditoriaViewModel>();
            
            
            services.AddTransient<ReservaViewModel>();
            
            services.AddTransient<RegistryViewModel>();
            services.AddTransient<RegistryWindow>();

            services.AddTransient<AlojamentoViewModel>();
            services.AddSingleton<ClienteViewModel>();
            
            services.AddTransient<LoginWindow>();
            services.AddTransient<MainWindow>();
        }
    }
}