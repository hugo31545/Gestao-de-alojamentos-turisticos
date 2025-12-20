using Moq;
using NUnit.Framework;
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Repositories.Interfaces;
using POOfase1trabalho13_11.Models.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace POOfase1trabalho13_11.Tests.Services
{
    [TestFixture]
    public class RegistoServiceTests
    {
        private Mock<IRegistoRepository> _mockRegistoRepo;
        private RegistoService _registoService;

        [SetUp]
        public void Setup()
        {
            _mockRegistoRepo = new Mock<IRegistoRepository>();
            _registoService = new RegistoService(_mockRegistoRepo.Object);
        }

        #region Testes de Consulta e Ordenação

        [Test]
        public void ObterTodosLogs_DeveRetornarLogsOrdenadosPorDataDescendente()
        {
            // Arrange
            var dataAntiga = new DateTime(2023, 1, 1);
            var dataRecente = new DateTime(2023, 12, 31);

            var listaLogs = new List<Registo>
            {
                new Registo(1, "Reserva", "Log Antigo", "Admin", "CREATE", 1, dataAntiga),
                new Registo(2, "Reserva", "Log Recente", "Admin", "CREATE", 2, dataRecente)
            };

            _mockRegistoRepo.Setup(r => r.GetAll()).Returns(listaLogs);

            // Act
            var resultado = _registoService.ObterTodosLogs().ToList();

            // Assert
            Assert.That(resultado.First().Id, Is.EqualTo(2), "O log mais recente deve vir primeiro.");
            Assert.That(resultado.Last().Id, Is.EqualTo(1));
        }

        [Test]
        public void ObterLogsPorOperacao_DeveFiltrarIgnorandoMaiusculas()
        {
            // Arrange
            var listaLogs = new List<Registo>
            {
                new Registo(1, "User", "Msg", "Admin", "CREATE", 1, DateTime.Now),
                new Registo(2, "User", "Msg", "Admin", "UPDATE", 2, DateTime.Now)
            };

            _mockRegistoRepo.Setup(r => r.GetAll()).Returns(listaLogs);

            // Act
            var resultado = _registoService.ObterLogsPorOperacao("create").ToList();

            // Assert
            Assert.That(resultado.Count, Is.EqualTo(1));
            Assert.That(resultado.First().Operacao, Is.EqualTo("CREATE"));
        }

        [Test]
        public void ObterLogsRecentes_DeveRetornarApenasQuantidadeSolicitada()
        {
            // Arrange
            var lista = new List<Registo>();
            for (int i = 1; i <= 10; i++)
                lista.Add(new Registo(i, "Ent", "M", "A", "C", i, DateTime.Now.AddDays(i)));

            _mockRegistoRepo.Setup(r => r.GetAll()).Returns(lista);

            // Act
            var resultado = _registoService.ObterLogsRecentes(3).ToList();

            // Assert
            Assert.That(resultado.Count, Is.EqualTo(3));
        }

        #endregion

        #region Testes de Manutenção (Limpeza)

        [Test]
        public void LimparLogsAntigos_DeveRemoverApenasRegistosAnterioresADataCorte()
        {
            // Arrange
            var dataCorte = new DateTime(2024, 1, 1);

            var logParaApagar = new Registo(1, "A", "M", "U", "C", 1, new DateTime(2023, 12, 31));
            var logParaManter = new Registo(2, "A", "M", "U", "C", 2, new DateTime(2024, 1, 2));

            var lista = new List<Registo> { logParaApagar, logParaManter };
            _mockRegistoRepo.Setup(r => r.GetAll()).Returns(lista);

            // Simula sucesso ao remover o ID 1
            _mockRegistoRepo.Setup(r => r.Remove(1)).Returns(true);

            // Act
            bool resultado = _registoService.LimparLogsAntigos(dataCorte, out string mensagem);

            // Assert
            Assert.IsTrue(resultado);
            Assert.That(mensagem, Does.Contain("1 registos antigos foram removidos."));

          
            _mockRegistoRepo.Verify(r => r.Remove(1), Times.Once);
            _mockRegistoRepo.Verify(r => r.Remove(2), Times.Never);
        }

        [Test]
        public void LimparLogsAntigos_QuandoOcorreExcecao_RetornaFalse()
        {
            // Arrange
            _mockRegistoRepo.Setup(r => r.GetAll()).Throws(new Exception("Erro de disco"));

            // Act
            bool resultado = _registoService.LimparLogsAntigos(DateTime.Now, out string mensagem);

            // Assert
            Assert.IsFalse(resultado);
            Assert.That(mensagem, Does.Contain("Erro na manutenção"));
        }

        #endregion
    }
}