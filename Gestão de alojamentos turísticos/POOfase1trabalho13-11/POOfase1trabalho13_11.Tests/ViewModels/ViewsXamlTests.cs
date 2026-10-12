using Moq;
using NUnit.Framework;
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Services.Implementations;
using POOfase1trabalho13_11.Models.Services.Interfaces;
using POOfase1trabalho13_11.ViewModels;
using POOfase1trabalho13_11.Views;
using POOfase1trabalho13_11.Views.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Windows;

namespace POOfase1trabalho13_11.Tests.ViewModels
{
    /// <summary>
    /// Confirma que o XAML dos ecrãs carrega sem erros (bindings, estilos e recursos),
    /// algo que a compilação não verifica.
    /// </summary>
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    [NonParallelizable]
    public class ViewsXamlTests
    {
        [OneTimeSetUp]
        public void CarregarRecursosDaAplicacao()
        {
            Application.ResourceAssembly ??= typeof(App).Assembly;

            // Os ecrãs usam estilos partilhados definidos em App.xaml
            if (Application.Current == null)
            {
                var app = new App { ShutdownMode = ShutdownMode.OnExplicitShutdown };
                app.InitializeComponent();
            }
        }

        private static void Desenhar(FrameworkElement elemento, double largura, double altura)
        {
            elemento.Measure(new Size(largura, altura));
            elemento.Arrange(new Rect(0, 0, largura, altura));
            elemento.UpdateLayout();
        }

        [Test]
        public void RelatoriosView_CarregaXamlComViewModel()
        {
            var consulta = new Mock<IConsultaService>();
            consulta.Setup(c => c.ObterHistoricoRecente(It.IsAny<int>())).Returns(new List<Consulta>
            {
                new Consulta { Tipo = "Faturação", Utilizador = "ana", Parametros = new() { ["Inicio"] = "01/10/2026" }, Resultados = new() { "Encontrados 1 registos." } }
            });

            var view = new RelatoriosView
            {
                DataContext = new RelatoriosViewModel(consulta.Object, new SessionService(), new DialogoFalso())
            };
            Desenhar(view, 900, 650);

            Assert.That(view.ActualWidth, Is.GreaterThan(0));
        }

        [Test]
        public void RegistryView_CarregaXamlComViewModel()
        {
            var vm = new RegistryViewModel(new Mock<IEmployeeService>().Object, new Mock<IRegistoService>().Object,
                new Mock<IViewFactory>().Object, new Mock<IAuthenticationService>().Object, new DialogoFalso())
            {
                NIF = "123" // força uma mensagem de erro visível
            };

            var view = new RegistryView { DataContext = vm };
            Desenhar(view, 470, 780);

            Assert.That(view.ActualWidth, Is.GreaterThan(0));
        }

        [Test]
        public void LoginWindow_CarregaXamlComViewModel()
        {
            var vm = new LoginViewModel(new Mock<IAuthenticationService>().Object, new Mock<IViewFactory>().Object, new SessionService())
            {
                ErrorMessage = "Utilizador ou palavra-passe incorretos."
            };

            var janela = new LoginWindow { DataContext = vm };
            try
            {
                Desenhar((FrameworkElement)janela.Content, 420, 470);
                Assert.That(((FrameworkElement)janela.Content).ActualWidth, Is.GreaterThan(0));
            }
            finally
            {
                janela.Close();
            }
        }
    }
}
