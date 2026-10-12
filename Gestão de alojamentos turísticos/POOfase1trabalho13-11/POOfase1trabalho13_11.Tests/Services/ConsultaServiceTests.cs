using Moq;
using NUnit.Framework;
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Enums;
using POOfase1trabalho13_11.Models.Repositories.Interfaces;
using POOfase1trabalho13_11.Models.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace POOfase1trabalho13_11.Tests.Services
{
    [TestFixture]
    public class ConsultaServiceTests
    {
        private Mock<IAlojamentoRepository> _mockAlojamentoRepo;
        private Mock<IReservaRepository> _mockReservaRepo;
        private Mock<IClienteRepository> _mockClienteRepo;
        private Mock<IConsultaRepository> _mockConsultaRepo;
        private ConsultaService _consultaService;

        [SetUp]
        public void Setup()
        {
            _mockAlojamentoRepo = new Mock<IAlojamentoRepository>();
            _mockReservaRepo = new Mock<IReservaRepository>();
            _mockClienteRepo = new Mock<IClienteRepository>();
            _mockConsultaRepo = new Mock<IConsultaRepository>();

            _consultaService = new ConsultaService(
                _mockAlojamentoRepo.Object,
                _mockReservaRepo.Object,
                _mockClienteRepo.Object,
                _mockConsultaRepo.Object
            );
        }

        #region Testes de Alojamento e Ocupação

        [Test]
        public void CalcularTaxaOcupacao_DeveRetornarPercentagemCorreta()
        {
            // Arrange
            var alojamentos = new List<Alojamento>
            {
                new Alojamento { Id = 1, Estado = EstadoAlojamento.Ocupado },
                new Alojamento { Id = 2, Estado = EstadoAlojamento.Disponivel },
                new Alojamento { Id = 3, Estado = EstadoAlojamento.Disponivel },
                new Alojamento { Id = 4, Estado = EstadoAlojamento.Manutencao }
            };
            _mockAlojamentoRepo.Setup(r => r.GetAll()).Returns(alojamentos);

            // Act
            double taxa = _consultaService.CalcularTaxaOcupacao();

            // Assert
            Assert.That(taxa, Is.EqualTo(25.0));
        }

        [Test]
        public void ObterAlojamentosMaisCaros_DeveRetornarOrdenadoEPorQuantidade()
        {
            // Arrange
            var alojamentos = new List<Alojamento>
            {
                new Alojamento { Id = 1, PrecoPorNoite = 50 },
                new Alojamento { Id = 2, PrecoPorNoite = 150 },
                new Alojamento { Id = 3, PrecoPorNoite = 100 }
            };
            _mockAlojamentoRepo.Setup(r => r.GetAll()).Returns(alojamentos);

            // Act
            var resultado = _consultaService.ObterAlojamentosMaisCaros(2).ToList();

            // Assert
            Assert.That(resultado.Count, Is.EqualTo(2));
            Assert.That(resultado.First().PrecoPorNoite, Is.EqualTo(150));
        }

        #endregion

        #region Testes de Faturação

        [Test]
        public void CalcularFaturacaoTotal_DeveSomasApenasReservasFinalizadasNoPeriodo()
        {
            // Arrange
            DateTime inicio = new DateTime(2023, 01, 01);
            DateTime fim = new DateTime(2023, 12, 31);
            int alojamentoId = 1;

            var reservas = new List<Reserva>
            {
                
                new Reserva(1, 1, alojamentoId, inicio, inicio.AddDays(2), 2, "", EstadoReserva.Finalizada, inicio),
               
                new Reserva(2, 1, alojamentoId, inicio, inicio.AddDays(2), 2, "", EstadoReserva.Pendente, inicio),
                
                new Reserva(3, 1, alojamentoId, fim.AddDays(10), fim.AddDays(12), 2, "", EstadoReserva.Finalizada, fim.AddDays(10))
            };

            _mockReservaRepo.Setup(r => r.GetAll()).Returns(reservas);
            _mockAlojamentoRepo.Setup(r => r.GetById(alojamentoId)).Returns(new Alojamento { PrecoPorNoite = 100 });

            // Act
            decimal total = _consultaService.CalcularFaturacaoTotal(inicio, fim);

            // Assert
            Assert.That(total, Is.EqualTo(200m));
        }

        [Test]
        public void CalcularFaturacaoTotal_UsaPrecoGuardadoNaReserva()
        {
            // Arrange: a reserva foi feita a 80€/noite; o preço atual subiu para 150€
            DateTime inicio = new DateTime(2023, 01, 01);
            DateTime fim = new DateTime(2023, 12, 31);
            var reserva = new Reserva(1, 1, 1, inicio, inicio.AddDays(2), 2, "", EstadoReserva.Finalizada, inicio) { PrecoTotal = 160m };

            _mockReservaRepo.Setup(r => r.GetAll()).Returns(new List<Reserva> { reserva });
            _mockAlojamentoRepo.Setup(r => r.GetById(1)).Returns(new Alojamento { PrecoPorNoite = 150 });

            // Act
            decimal total = _consultaService.CalcularFaturacaoTotal(inicio, fim);

            // Assert
            Assert.That(total, Is.EqualTo(160m));
        }

        #endregion

        #region Testes de Clientes e Histórico

        [Test]
        public void ObterClientesComMaisReservas_DeveRetornarRankingCorreto()
        {
            // Arrange
            var reservas = new List<Reserva>
            {
                new Reserva { ClienteId = 1 },
                new Reserva { ClienteId = 1 },
                new Reserva { ClienteId = 2 }
            };
            _mockReservaRepo.Setup(r => r.GetAll()).Returns(reservas);
            _mockClienteRepo.Setup(r => r.GetById(1)).Returns(new Cliente { Id = 1, Nome = "Cliente Top" });
            _mockClienteRepo.Setup(r => r.GetById(2)).Returns(new Cliente { Id = 2, Nome = "Cliente 2" });

            // Act
            var ranking = _consultaService.ObterClientesComMaisReservas(1).ToList();

            // Assert
            Assert.That(ranking.Count, Is.EqualTo(1));
            Assert.That(ranking.First().Id, Is.EqualTo(1));
        }

        [Test]
        public void RegistarPesquisaNoHistorico_DeveChamarAddNoRepositorio()
        {
            // Act
            _consultaService.RegistarPesquisaNoHistorico("Admin", "Alojamentos", new Dictionary<string, string>(), 5);

            // Assert
            _mockConsultaRepo.Verify(r => r.Add(It.Is<Consulta>(c =>
                c.Utilizador == "Admin" &&
                c.Resultados.First().Contains("5"))), Times.Once);
        }

        #endregion
    }
}