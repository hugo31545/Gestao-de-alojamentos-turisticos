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
    public class CheckInServiceTests
    {
        private Mock<IReservaRepository> _mockReservaRepo;
        private Mock<IAlojamentoRepository> _mockAlojamentoRepo;
        private Mock<IRegistoRepository> _mockRegistoRepo;
        private CheckInService _checkInService;

        [SetUp]
        public void Setup()
        {
            _mockReservaRepo = new Mock<IReservaRepository>();
            _mockAlojamentoRepo = new Mock<IAlojamentoRepository>();
            _mockRegistoRepo = new Mock<IRegistoRepository>();

            _checkInService = new CheckInService(
                _mockReservaRepo.Object,
                _mockAlojamentoRepo.Object,
                _mockRegistoRepo.Object
            );
        }

        #region Testes de RealizarCheckIn

        [Test]
        public void RealizarCheckIn_ReservaInexistente_RetornaFalse()
        {
            // Arrange
            _mockReservaRepo.Setup(r => r.GetById(It.IsAny<int>())).Returns((Reserva)null);

            // Act
            bool resultado = _checkInService.RealizarCheckIn(999, "Admin", out string mensagem);

            // Assert
            Assert.IsFalse(resultado);
            Assert.That(mensagem, Does.Contain("não encontrada"));
        }

        [Test]
        public void RealizarCheckIn_DataAntecipada_RetornaFalse()
        {
            // Arrange
            int id = 1;
            var dataFutura = DateTime.Now.AddDays(2);
            var reserva = new Reserva { Id = id, DataCheckIn = dataFutura, Estado = EstadoReserva.Confirmada };

            _mockReservaRepo.Setup(r => r.GetById(id)).Returns(reserva);

            // Act
            bool resultado = _checkInService.RealizarCheckIn(id, "Admin", out string mensagem);

            // Assert
            Assert.IsFalse(resultado);
            Assert.That(mensagem, Does.Contain("só é permitido a partir do dia"));
        }

        [Test]
        public void RealizarCheckIn_Sucesso_AtualizaReservaEAlojamento()
        {
            // Arrange
            int resId = 1, alojId = 10;
            var reserva = new Reserva { Id = resId, AlojamentoId = alojId, DataCheckIn = DateTime.Now.AddDays(-1), Estado = EstadoReserva.Confirmada };
            var alojamento = new Alojamento { Id = alojId, Estado = EstadoAlojamento.Disponivel };

            _mockReservaRepo.Setup(r => r.GetById(resId)).Returns(reserva);
            _mockAlojamentoRepo.Setup(r => r.GetById(alojId)).Returns(alojamento);

            // Act
            bool resultado = _checkInService.RealizarCheckIn(resId, "Funcionario1", out string mensagem);

            // Assert
            Assert.IsTrue(resultado);
            Assert.That(reserva.Estado, Is.EqualTo(EstadoReserva.CheckInRealizado));
            Assert.That(alojamento.Estado, Is.EqualTo(EstadoAlojamento.Ocupado));

            
            _mockReservaRepo.Verify(r => r.Update(reserva), Times.Once);
            _mockAlojamentoRepo.Verify(r => r.Update(alojamento), Times.Once);
            _mockRegistoRepo.Verify(r => r.Add(It.Is<Registo>(log => log.Operacao == "CHECKIN")), Times.Once);
        }

        #endregion

        #region Testes de RealizarCheckOut

        [Test]
        public void RealizarCheckOut_Sucesso_ColocaAlojamentoEmManutencao()
        {
            // Arrange
            int resId = 1, alojId = 20;
            var reserva = new Reserva { Id = resId, AlojamentoId = alojId, Estado = EstadoReserva.CheckInRealizado };
            var alojamento = new Alojamento { Id = alojId, Estado = EstadoAlojamento.Ocupado };

            _mockReservaRepo.Setup(r => r.GetById(resId)).Returns(reserva);
            _mockAlojamentoRepo.Setup(r => r.GetById(alojId)).Returns(alojamento);

            // Act
            bool resultado = _checkInService.RealizarCheckOut(resId, "Admin", out string mensagem);

            // Assert
            Assert.IsTrue(resultado);
            Assert.That(reserva.Estado, Is.EqualTo(EstadoReserva.Finalizada));
            Assert.That(alojamento.Estado, Is.EqualTo(EstadoAlojamento.Manutencao)); // Regra de limpeza
            _mockRegistoRepo.Verify(r => r.Add(It.Is<Registo>(log => log.Operacao == "CHECKOUT")), Times.Once);
        }

        [Test]
        public void RealizarCheckOut_ReservaNaoAtiva_RetornaFalse()
        {
            // Arrange
            var reservaCancelada = new Reserva { Id = 1, Estado = EstadoReserva.Cancelada };
            _mockReservaRepo.Setup(r => r.GetById(1)).Returns(reservaCancelada);

            // Act
            bool resultado = _checkInService.RealizarCheckOut(1, "Admin", out string mensagem);

            // Assert
            Assert.IsFalse(resultado);
            Assert.That(mensagem, Does.Contain("estadia ativa"));
        }

        #endregion

        #region Consultas

        [Test]
        public void ObterEstadiasAtuais_DeveFiltrarCorretamente()
        {
            // Arrange
            var lista = new List<Reserva>
            {
                new Reserva { Id = 1, Estado = EstadoReserva.CheckInRealizado },
                new Reserva { Id = 2, Estado = EstadoReserva.Confirmada },
                new Reserva { Id = 3, Estado = EstadoReserva.CheckInRealizado }
            };
            _mockReservaRepo.Setup(r => r.GetAll()).Returns(lista);

            // Act
            var resultado = _checkInService.ObterEstadiasAtuais().ToList();

            // Assert
            Assert.That(resultado.Count, Is.EqualTo(2));
            Assert.IsTrue(resultado.All(r => r.Estado == EstadoReserva.CheckInRealizado));
        }

        #endregion
    }
}