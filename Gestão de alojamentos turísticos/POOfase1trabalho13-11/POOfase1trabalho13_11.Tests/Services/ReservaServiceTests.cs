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
    public class ReservaServiceTests
    {
        // 1. Declarar os Mocks dos Repositórios
        private Mock<IReservaRepository> _mockReservaRepo;
        private Mock<IAlojamentoRepository> _mockAlojamentoRepo;
        private Mock<IClienteRepository> _mockClienteRepo;
        private Mock<IRegistoRepository> _mockRegistoRepo;

        // O serviço que vamos testar
        private ReservaService _reservaService;

        [SetUp]
        public void Setup()
        {
            // 2. Inicializar os Mocks
            _mockReservaRepo = new Mock<IReservaRepository>();
            _mockAlojamentoRepo = new Mock<IAlojamentoRepository>();
            _mockClienteRepo = new Mock<IClienteRepository>();
            _mockRegistoRepo = new Mock<IRegistoRepository>();

            // 3. Injetar os Mocks no Serviço
            _reservaService = new ReservaService(
                _mockReservaRepo.Object,
                _mockAlojamentoRepo.Object,
                _mockClienteRepo.Object,
                _mockRegistoRepo.Object
            );
        }

        #region Testes de RealizarReserva

        [Test]
        public void RealizarReserva_DadosValidos_RetornaTrueEMensagemSucesso()
        {
            // Arrange
            int clienteId = 1;
            int alojamentoId = 10;

           
            _mockClienteRepo.Setup(r => r.GetById(clienteId)).Returns(new Cliente { Id = clienteId, Nome = "Teste" });

            
            _mockAlojamentoRepo.Setup(r => r.GetById(alojamentoId)).Returns(new Alojamento
            {
                Id = alojamentoId,
                Capacidade = 5,
                Estado = EstadoAlojamento.Disponivel
            });

            
            _mockReservaRepo.Setup(r => r.GetAll()).Returns(new List<Reserva>());

            // Act
            bool resultado = _reservaService.RealizarReserva(clienteId, alojamentoId,
                DateTime.Now.AddDays(1), DateTime.Now.AddDays(3), 2, out string mensagem);

            // Assert
            Assert.IsTrue(resultado);
            Assert.That(mensagem, Does.Contain("sucesso"));

            
            _mockReservaRepo.Verify(r => r.Add(It.IsAny<Reserva>()), Times.Once);
        }

        [Test]
        public void RealizarReserva_CapacidadeExcedida_RetornaFalse()
        {
            // Arrange
            int clienteId = 1;
            int alojamentoId = 10;

            _mockClienteRepo.Setup(r => r.GetById(clienteId)).Returns(new Cliente());
            _mockAlojamentoRepo.Setup(r => r.GetById(alojamentoId)).Returns(new Alojamento
            {
                Capacidade = 2, // Só cabem 2
                Estado = EstadoAlojamento.Disponivel
            });

            // Act
            bool resultado = _reservaService.RealizarReserva(clienteId, alojamentoId,
                DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), 5, out string mensagem); // Tentamos meter 5

            // Assert
            Assert.IsFalse(resultado);
            Assert.That(mensagem, Is.EqualTo("Número de hóspedes excede a capacidade."));
        }

        [Test]
        public void RealizarReserva_ConflitoDeDatas_RetornaFalse()
        {
            // Arrange
            int alojamentoId = 10;
            DateTime dataExistenteIn = new DateTime(2025, 12, 20);
            DateTime dataExistenteOut = new DateTime(2025, 12, 25);

            _mockClienteRepo.Setup(r => r.GetById(It.IsAny<int>())).Returns(new Cliente());
            _mockAlojamentoRepo.Setup(r => r.GetById(alojamentoId)).Returns(new Alojamento
            { Id = alojamentoId, Capacidade = 10, Estado = EstadoAlojamento.Disponivel });

           
            var reservasExistentes = new List<Reserva>
            {
                new Reserva(1, 1, alojamentoId, dataExistenteIn, dataExistenteOut, 2, "", EstadoReserva.Confirmada, DateTime.Now)
            };
            _mockReservaRepo.Setup(r => r.GetAll()).Returns(reservasExistentes);

           
            bool resultado = _reservaService.RealizarReserva(1, alojamentoId,
                new DateTime(2025, 12, 21), new DateTime(2025, 12, 23), 2, out string mensagem);

            // Assert
            Assert.IsFalse(resultado);
            Assert.That(mensagem, Is.EqualTo("O alojamento já está ocupado no período selecionado."));
        }

        #endregion

        #region Testes de Check-In e Check-Out (Continuação)

        [Test]
        public void EfetuarCheckIn_ReservaConfirmada_RetornaTrueEAtualizaEstado()
        {
            // Arrange
            int resId = 1;
            var reservaConfirmada = new Reserva { Id = resId, Estado = EstadoReserva.Confirmada };
            _mockReservaRepo.Setup(r => r.GetById(resId)).Returns(reservaConfirmada);

            // Act
            bool resultado = _reservaService.EfetuarCheckIn(resId, out string mensagem);

            // Assert
            Assert.IsTrue(resultado);
            Assert.That(reservaConfirmada.Estado, Is.EqualTo(EstadoReserva.CheckInRealizado));
            _mockReservaRepo.Verify(r => r.Update(reservaConfirmada), Times.Once);
        }

        [Test]
        public void EfetuarCheckOut_SemCheckInPrevio_RetornaFalse()
        {
            // Arrange
            int resId = 1;
            var reservaConfirmada = new Reserva { Id = resId, Estado = EstadoReserva.Confirmada };
            _mockReservaRepo.Setup(r => r.GetById(resId)).Returns(reservaConfirmada);

            // Act
            bool resultado = _reservaService.EfetuarCheckOut(resId, out string mensagem);

            // Assert
            Assert.IsFalse(resultado);
            Assert.That(mensagem, Does.Contain("Só pode fazer Check-out de reservas que já fizeram Check-in"));
        }

        [Test]
        public void EfetuarCheckOut_DadosValidos_RetornaTrueEAtualizaEstado()
        {
            // Arrange
            int resId = 1;
            var reservaComCheckIn = new Reserva { Id = resId, Estado = EstadoReserva.CheckInRealizado };
            _mockReservaRepo.Setup(r => r.GetById(resId)).Returns(reservaComCheckIn);

            // Act
            bool resultado = _reservaService.EfetuarCheckOut(resId, out string mensagem);

            // Assert
            Assert.IsTrue(resultado);
            Assert.That(reservaComCheckIn.Estado, Is.EqualTo(EstadoReserva.CheckOutRealizado));
            _mockReservaRepo.Verify(r => r.Update(reservaComCheckIn), Times.Once);
        }

        #endregion

        #region Testes de Cancelamento

        [Test]
        public void CancelarReserva_ReservaExistente_RetornaTrueERegistaNoLog()
        {
            // Arrange
            int resId = 50;
            var reserva = new Reserva { Id = resId, Estado = EstadoReserva.Confirmada, Observacoes = "" };
            _mockReservaRepo.Setup(r => r.GetById(resId)).Returns(reserva);

            // Act
            bool resultado = _reservaService.CancelarReserva(resId, "Cliente desistiu", out string mensagem);

            // Assert
            Assert.IsTrue(resultado);
            Assert.That(reserva.Estado, Is.EqualTo(EstadoReserva.Cancelada));
            Assert.That(reserva.Observacoes, Does.Contain("Cliente desistiu"));

           
            _mockRegistoRepo.Verify(r => r.Add(It.Is<Registo>(log => log.Operacao == "CANCEL")), Times.Once);
        }

        [Test]
        public void CancelarReserva_Inexistente_RetornaFalse()
        {
            // Arrange
            _mockReservaRepo.Setup(r => r.GetById(It.IsAny<int>())).Returns((Reserva?)null);

            // Act
            bool resultado = _reservaService.CancelarReserva(999, "Erro", out string mensagem);

            // Assert
            Assert.IsFalse(resultado);
            Assert.That(mensagem, Is.EqualTo("Reserva não encontrada."));
        }

        #endregion
    }
}