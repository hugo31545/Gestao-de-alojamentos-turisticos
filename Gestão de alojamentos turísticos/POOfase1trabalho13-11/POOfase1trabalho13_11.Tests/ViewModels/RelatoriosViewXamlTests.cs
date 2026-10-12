using Moq;
using NUnit.Framework;
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Services.Implementations;
using POOfase1trabalho13_11.Models.Services.Interfaces;
using POOfase1trabalho13_11.ViewModels;
using POOfase1trabalho13_11.Views;
using System.Collections.Generic;
using System.Threading;

namespace POOfase1trabalho13_11.Tests.ViewModels
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class RelatoriosViewXamlTests
    {
        [Test]
        public void RelatoriosView_CarregaXamlComViewModel()
        {
            if (System.Windows.Application.ResourceAssembly == null)
                System.Windows.Application.ResourceAssembly = typeof(RelatoriosView).Assembly;

            var consulta = new Mock<IConsultaService>();
            consulta.Setup(c => c.ObterHistoricoRecente(It.IsAny<int>())).Returns(new List<Consulta>
            {
                new Consulta { Tipo = "Faturação", Utilizador = "ana", Parametros = new() { ["Inicio"] = "01/10/2026" }, Resultados = new() { "Encontrados 1 registos." } }
            });

            var view = new RelatoriosView
            {
                DataContext = new RelatoriosViewModel(consulta.Object, new SessionService(), new DialogoFalso())
            };
            view.Measure(new System.Windows.Size(900, 650));
            view.Arrange(new System.Windows.Rect(0, 0, 900, 650));
            view.UpdateLayout();

            Assert.That(view.IsLoaded || view.ActualWidth > 0, Is.True);
        }
    }
}
