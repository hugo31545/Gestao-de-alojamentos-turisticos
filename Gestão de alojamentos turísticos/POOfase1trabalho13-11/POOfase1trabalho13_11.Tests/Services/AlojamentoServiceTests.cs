using Moq;
using NUnit.Framework;
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Enums;
using POOfase1trabalho13_11.Models.Repositories.Interfaces;
using POOfase1trabalho13_11.Models.Services.Implementations;
using POOfase1trabalho13_11.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace POOfase1trabalho13_11.Tests.Services
{
    [TestFixture]
    public class AlojamentoServiceTests
    {
        private Mock<IAlojamentoRepository> _mockAlojamentoRepo;
        private Mock<IRegistoRepository> _mockRegistoRepo;
        private Mock<IReservaRepository> _mockReservaRepo;
        private AlojamentoService _alojamentoService;

        [SetUp]
        public void Setup()
        {
            _mockAlojamentoRepo = new Mock<IAlojamentoRepository>();
            _mockRegistoRepo = new Mock<IRegistoRepository>();
            _mockReservaRepo = new Mock<IReservaRepository>();

            _alojamentoService = new AlojamentoService(
                _mockAlojamentoRepo.Object,
                _mockRegistoRepo.Object,
                _mockReservaRepo.Object
            );
        }

        #region Testes de Escrita e Validações

        [Test]
        public void GuardarAlojamento_PrecoNegativo_DeveLancarBusinessException()
        {
            // Arrange
            var alojamentoInvalido = new Alojamento { Nome = "Suite", PrecoPorNoite = -10 };

            // Act & Assert
            var ex = Assert.Throws<BusinessException>(() => _alojamentoService.GuardarAlojamento(alojamentoInvalido));
            Assert.That(ex.Message, Does.Contain("superior a zero"));
        }

        [Test]
        public void GuardarAlojamento_NomeDuplicado_DeveLancarBusinessException()
        {
            // Arrange
            var novo = new Alojamento { Id = 0, Nome = "Hotel Central" };
            var existente = new Alojamento { Id = 99, Nome = "Hotel Central" };

            _mockAlojamentoRepo.Setup(r => r.BuscarPorNome("Hotel Central")).Returns(existente);

            // Act & Assert
            Assert.Throws<BusinessException>(() => _alojamentoService.GuardarAlojamento(novo));
        }

        [Test]
        public void RemoverAlojamento_ComReservaAtiva_DeveLancarBusinessException()
        {
            // Arrange
            int alojId = 1;
            var reservas = new List<Reserva>
            {
                new Reserva { AlojamentoId = alojId, Estado = EstadoReserva.Confirmada }
            };

            _mockReservaRepo.Setup(r => r.GetAll()).Returns(reservas);

            // Act & Assert
            Assert.Throws<BusinessException>(() => _alojamentoService.RemoverAlojamento(alojId));
            _mockAlojamentoRepo.Verify(r => r.Remove(alojId), Times.Never);
        }

        [Test]
        public void GuardarAlojamento_CapacidadeZero_DeveLancarBusinessException()
        {
            var alojamento = new Alojamento { Nome = "Suite", PrecoPorNoite = 80, Capacidade = 0 };

            var ex = Assert.Throws<BusinessException>(() => _alojamentoService.GuardarAlojamento(alojamento));
            Assert.That(ex!.Message, Does.Contain("capacidade"));
            _mockAlojamentoRepo.Verify(r => r.Save(It.IsAny<Alojamento>()), Times.Never);
        }

        [Test]
        public void RemoverAlojamento_ComReservaPendente_DeveLancarBusinessException()
        {
            // Arrange: reservas pendentes também ficariam órfãs
            int alojId = 1;
            _mockReservaRepo.Setup(r => r.GetAll()).Returns(new List<Reserva>
            {
                new Reserva { AlojamentoId = alojId, Estado = EstadoReserva.Pendente }
            });

            // Act & Assert
            Assert.Throws<BusinessException>(() => _alojamentoService.RemoverAlojamento(alojId));
            _mockAlojamentoRepo.Verify(r => r.Remove(alojId), Times.Never);
        }

        #endregion

        #region Testes de Lógica de Datas e Disponibilidade

        [Test]
        public void BuscarDisponiveisPorPeriodo_AlojamentoOcupadoHoje_DisponivelNoFuturo()
        {
            // Arrange: o alojamento está Ocupado agora, mas não tem reservas para o período pedido
            var alojamento = new Alojamento { Id = 1, Nome = "Quarto 101", Estado = EstadoAlojamento.Ocupado };
            var indisponivel = new Alojamento { Id = 2, Nome = "Quarto 102", Estado = EstadoAlojamento.Indisponivel };
            _mockAlojamentoRepo.Setup(r => r.GetAll()).Returns(new List<Alojamento> { alojamento, indisponivel });
            _mockReservaRepo.Setup(r => r.GetAll()).Returns(new List<Reserva>());

            // Act
            var resultado = _alojamentoService.BuscarDisponiveisPorPeriodo(DateTime.Today.AddDays(30), DateTime.Today.AddDays(33)).ToList();

            // Assert
            Assert.That(resultado, Is.EquivalentTo(new[] { alojamento }));
        }

        [Test]
        public void BuscarDisponiveisPorPeriodo_ComSobreposicao_DeveFiltrarAlojamento()
        {
            // Arrange
            var checkIn = new DateTime(2024, 10, 10);
            var checkOut = new DateTime(2024, 10, 15);

            var alojamento = new Alojamento { Id = 1, Nome = "Quarto 101" };
            _mockAlojamentoRepo.Setup(r => r.GetAll()).Returns(new List<Alojamento> { alojamento });

            // Simular uma reserva que choca com as datas (Ex: 12 a 20 de Outubro)
            var reservaConflitante = new Reserva
            {
                AlojamentoId = 1,
                DataCheckIn = new DateTime(2024, 10, 12),
                DataCheckOut = new DateTime(2024, 10, 20),
                Estado = EstadoReserva.Confirmada
            };
            _mockReservaRepo.Setup(r => r.GetAll()).Returns(new List<Reserva> { reservaConflitante });

            // Act
            var resultado = _alojamentoService.BuscarDisponiveisPorPeriodo(checkIn, checkOut);

            // Assert
            Assert.IsEmpty(resultado, "O alojamento deveria estar ocupado devido à sobreposição de datas.");
        }

        [Test]
        public void CalcularPrecoTotal_DadosValidos_RetornaSomaCorreta()
        {
            // Arrange
            _mockAlojamentoRepo.Setup(r => r.GetById(1)).Returns(new Alojamento { PrecoPorNoite = 100 });

            // Act
            var total = _alojamentoService.CalcularPrecoTotal(1, 3);

            // Assert
            Assert.That(total, Is.EqualTo(300m));
        }

        #endregion

        #region Testes de Estados

        [Test]
        public void MudarEstado_EstadoDiferente_AtualizaERegistaLog()
        {
            // Arrange
            var aloj = new Alojamento { Id = 1, Estado = EstadoAlojamento.Disponivel };
            _mockAlojamentoRepo.Setup(r => r.GetById(1)).Returns(aloj);
            _mockAlojamentoRepo.Setup(r => r.Update(aloj)).Returns(true);

            // Act
            bool mudou = _alojamentoService.MudarEstado(1, EstadoAlojamento.Manutencao);

            // Assert
            Assert.IsTrue(mudou);
            Assert.That(aloj.Estado, Is.EqualTo(EstadoAlojamento.Manutencao));
            _mockRegistoRepo.Verify(r => r.Add(It.Is<Registo>(l => l.Operacao == "STATE_CHANGE")), Times.Once);
        }

        #endregion
    }
}