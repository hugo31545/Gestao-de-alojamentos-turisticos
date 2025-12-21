//-----------------------------------------------------------------
//    <copyright file="App.xaml.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
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
    /// <summary>
    /// Ponto de entrada principal da aplicação. 
    /// Responsável pela configuração do Contentor de Injeção de Dependências
    /// e pela orquestração do arranque do sistema.
    /// </summary>
    public partial class App : Application
    {
        #region Properties
        /// <summary>
        /// Provedor de serviços global. Permite o acesso ao contentor de dependências
        /// em pontos específicos onde a injeção via construtor não é possível.
        /// </summary>
        public static IServiceProvider ServiceProvider { get; private set; } = null!;
        #endregion


        #region Overrides
        /// <summary>
        /// Método invocado no arranque da aplicação. Configura o ambiente e inicia o fluxo de UI.
        /// </summary>
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
        #endregion

        #region Configuration

        /// <summary>
        /// Regista todos os componentes do sistema e define o seu tempo de vida (Scope).
        /// </summary>
        private void ConfigureServices(IServiceCollection services)
        {
            // --- 0. INFRAESTRUTURA ---
            // A Factory deve ser única para gerir todas as janelas da sessão.
            services.AddSingleton<IViewFactory, ViewFactory>();

            // --- 1. REPOSITÓRIOS (Data Access) ---
            // Definidos como Singleton para garantir que a cache em memória seja partilhada
            // entre todos os serviços durante a execução.

            services.AddSingleton<IAlojamentoRepository, AlojamentoRepository>();
            services.AddSingleton<IClienteRepository, ClienteRepository>();
            services.AddSingleton<ICheckInRepository, CheckInRepository>();
            services.AddSingleton<IConsultaRepository, ConsultaRepository>();
            services.AddSingleton<IReservaRepository, ReservaRepository>();
            services.AddSingleton<IRegistoRepository, RegistoRepository>();
            services.AddSingleton<IEmployeeRepository, EmployeeRepository>();
            services.AddSingleton<IUserRepository, UserRepository>();


            // --- 2. SERVIÇOS (Business Logic) ---
            // Contêm as regras de negócio e interagem com os repositórios.
            services.AddSingleton<IAlojamentoService, AlojamentoService>();
            services.AddSingleton<IClienteService, ClienteService>();
            services.AddSingleton<IReservaService, ReservaService>();
            services.AddSingleton<IRegistoService, RegistoService>();
            services.AddSingleton<IEmployeeService, EmployeeService>();
            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<ICheckInService, CheckInService>();
            services.AddSingleton<IConsultaService, ConsultaService>();

            // --- 3. VIEWMODELS & VIEWS ---
            // MainViewModel e ClienteViewModel como Singleton para manter o estado entre navegações.
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<ClienteViewModel>();

            // Transient: Uma nova instância é criada sempre que a janela é aberta (limpa o estado anterior).
            services.AddTransient<LoginViewModel>();
            services.AddTransient<AuditoriaViewModel>();
            services.AddTransient<ReservaViewModel>();
            services.AddTransient<RegistryViewModel>();
            

            services.AddTransient<AlojamentoViewModel>();

            // Registo das janelas físicas

            services.AddTransient<LoginWindow>();
            services.AddTransient<MainWindow>();
            services.AddTransient<RegistryWindow>();
        }
        #endregion
    }
}