//-----------------------------------------------------------------
//    <copyright file="ViewFactory.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using Microsoft.Extensions.DependencyInjection;
using POOfase1trabalho13_11.ViewModels;
using POOfase1trabalho13_11.Views.Enums;
using POOfase1trabalho13_11.Views.Interfaces;
using System.Windows;

namespace POOfase1trabalho13_11.Views.Factories
{
    /// <summary>
    /// Fábrica responsável pela criação, mapeamento e inicialização de janelas WPF.
    /// Centraliza a lógica de Injeção de Dependências, garantindo que as Views e ViewModels
    /// recebam os serviços necessários via ServiceProvider.
    /// </summary>
    public class ViewFactory : IViewFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ViewFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        #region Public Methods

        /// <summary>
        /// Cria uma janela, associa a ViewModel correspondente e configura ações de fecho.
        /// </summary>
        /// <param name="type">Tipo de view solicitada.</param>
        /// <param name="parameter">Parâmetro opcional para janelas de edição.</param>
        
        public Window CreateView(ViewType type, object? parameter = null)
        {
            // 1. View
            Window window = type switch
            {
                ViewType.Login => _serviceProvider.GetRequiredService<LoginWindow>(),
                ViewType.Main => _serviceProvider.GetRequiredService<MainWindow>(),
                ViewType.Registry => _serviceProvider.GetRequiredService<RegistryWindow>(),
                _ => throw new ArgumentException($"Tipo de vista {type} não suportado.")
            };

            // 2. ViewModel
            object viewModel = type switch
            {
                ViewType.Login => _serviceProvider.GetRequiredService<LoginViewModel>(),
                ViewType.Main => _serviceProvider.GetRequiredService<MainViewModel>(),
                ViewType.Registry => _serviceProvider.GetRequiredService<RegistryViewModel>(),
                _ => throw new ArgumentException($"ViewModel para {type} não encontrada.")
            };

            if (type == ViewType.Main && viewModel is MainViewModel mainVm && parameter is string username)
            {
                mainVm.UsernameLogado = username;
            }
            // 3. Fecho automático da janela via ViewModel
            ConfigurarAcoesDeFecho(window, viewModel);

            window.DataContext = viewModel;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            return window;
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Procura propriedades de ação de fecho na ViewModel e associa ao método Close() da Window.
        /// </summary>
        private void ConfigurarAcoesDeFecho(Window window, object viewModel)
        {
            var closeProperty = viewModel.GetType().GetProperty("CloseWindowAction");
            if (closeProperty != null)
            {
                Action closeAction = new Action(() => window.Close());
                closeProperty.SetValue(viewModel, closeAction);
            }
        }

        #endregion
    }
}