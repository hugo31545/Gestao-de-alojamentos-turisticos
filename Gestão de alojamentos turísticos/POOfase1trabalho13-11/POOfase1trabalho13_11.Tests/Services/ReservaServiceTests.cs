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
            DateTime dataExistenteIn = DateTime.Today.AddDays(20);
            DateTime dataExistenteOut = DateTime.Today.AddDays(25);

            _mockClienteRepo.Setup(r => r.GetById(It.IsAny<int>())).Returns(new Cliente());
            _mockAlojamentoRepo.Setup(r => r.GetById(alojamentoId)).Returns(new Alojamento
            { Id = alojamentoId, Capacidade = 10, Estado = EstadoAlojamento.Disponivel });

           
            var reservasExistentes = new List<Reserva>
            {
                new Reserva(1, 1, alojamentoId, dataExistenteIn, dataExistenteOut, 2, "", EstadoReserva.Confirmada, DateTime.Now)
            };
            _mockReservaRepo.Setup(r => r.GetAll()).Returns(reservasExistentes);

           
            bool resultado = _reservaService.RealizarReserva(1, alojamentoId,
                DateTime.Today.AddDays(21), DateTime.Today.AddDays(23), 2, out string mensagem);

            // Assert
            Assert.IsFalse(resultado);
            Assert.That(mensagem, Is.EqualTo("O alojamento já está ocupado no período selecionado."));
        }

        [Test]
        public void RealizarReserva_DataNoPassado_RetornaFalse()
        {
            _mockClienteRepo.Setup(r => r.GetById(1)).Returns(new Cliente { Id = 1, Nome = "Teste" });
            _mockAlojamentoRepo.Setup(r => r.GetById(10)).Returns(new Alojamento { Id = 10, Capacidade = 4 });

            bool resultado = _reservaService.RealizarReserva(1, 10,
                DateTime.Today.AddDays(-3), DateTime.Today.AddDays(-1), 2, out string mensagem);

            Assert.IsFalse(resultado);
            Assert.That(mensagem, Does.Contain("passado"));
        }

        [Test]
        public void RealizarReserva_SemHospedes_RetornaFalse()
        {
            _mockClienteRepo.Setup(r => r.GetById(1)).Returns(new Cliente { Id = 1, Nome = "Teste" });
            _mockAlojamentoRepo.Setup(r => r.GetById(10)).Returns(new Alojamento { Id = 10, Capacidade = 4 });
            _mockReservaRepo.Setup(r => r.GetAll()).Returns(new List<Reserva>());

            bool resultado = _reservaService.RealizarReserva(1, 10,
                DateTime.Today.AddDays(1), DateTime.Today.AddDays(2), 0, out string mensagem);

            Assert.IsFalse(resultado);
            Assert.That(mensagem, Does.Contain("pelo menos um hóspede"));
        }

        [Test]
        public void RealizarReserva_AlojamentoOcupadoHoje_PermiteReservaFuturaEGuardaPreco()
        {
            // Arrange: Ocupado é o estado atual; não deve bloquear reservas para outras datas
            Reserva? criada = null;
            _mockClienteRepo.Setup(r => r.GetById(1)).Returns(new Cliente { Id = 1, Nome = "Teste" });
            _mockAlojamentoRepo.Setup(r => r.GetById(10)).Returns(new Alojamento
            { Id = 10, Capacidade = 4, PrecoPorNoite = 50, Estado = EstadoAlojamento.Ocupado });
            _mockReservaRepo.Setup(r => r.GetAll()).Returns(new List<Reserva>());
            _mockReservaRepo.Setup(r => r.Add(It.IsAny<Reserva>())).Callback<Reserva>(r => criada = r);

            // Act
            bool resultado = _reservaService.RealizarReserva(1, 10,
                DateTime.Today.AddDays(10), DateTime.Today.AddDays(13), 2, out string mensagem);

            // Assert
            Assert.IsTrue(resultado, mensagem);
            Assert.That(criada!.PrecoTotal, Is.EqualTo(150m));
        }

        [Test]
        public void RealizarReserva_AlojamentoIndisponivel_RetornaFalse()
        {
            _mockClienteRepo.Setup(r => r.GetById(1)).Returns(new Cliente { Id = 1, Nome = "Teste" });
            _mockAlojamentoRepo.Setup(r => r.GetById(10)).Returns(new Alojamento
            { Id = 10, Capacidade = 4, Estado = EstadoAlojamento.Indisponivel });
            _mockReservaRepo.Setup(r => r.GetAll()).Returns(new List<Reserva>());

            bool resultado = _reservaService.RealizarReserva(1, 10,
                DateTime.Today.AddDays(1), DateTime.Today.AddDays(2), 2, out string mensagem);

            Assert.IsFalse(resultado);
            Assert.That(mensagem, Does.Contain("não pode receber reservas"));
        }

        #endregion

        #region Testes de Confirmação

        [Test]
        public void ConfirmarReserva_Pendente_ConfirmaERegistaNoLog()
        {
            var reserva = new Reserva { Id = 1, Estado = EstadoReserva.Pendente };
            _mockReservaRepo.Setup(r => r.GetById(1)).Returns(reserva);

            bool resultado = _reservaService.ConfirmarReserva(1, out string mensagem);

            Assert.IsTrue(resultado, mensagem);
            Assert.That(reserva.Estado, Is.EqualTo(EstadoReserva.Confirmada));
            _mockReservaRepo.Verify(r => r.Update(reserva), Times.Once);
            _mockRegistoRepo.Verify(r => r.Add(It.Is<Registo>(log => log.Operacao == "CONFIRM")), Times.Once);
        }

        [Test]
        public void ConfirmarReserva_JaCancelada_RetornaFalse()
        {
            var reserva = new Reserva { Id = 1, Estado = EstadoReserva.Cancelada };
            _mockReservaRepo.Setup(r => r.GetById(1)).Returns(reserva);

            bool resultado = _reservaService.ConfirmarReserva(1, out string mensagem);

            Assert.IsFalse(resultado);
            Assert.That(reserva.Estado, Is.EqualTo(EstadoReserva.Cancelada));
            _mockReservaRepo.Verify(r => r.Update(It.IsAny<Reserva>()), Times.Never);
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

        [TestCase(EstadoReserva.CheckInRealizado)]
        [TestCase(EstadoReserva.Finalizada)]
        [TestCase(EstadoReserva.Cancelada)]
        public void CancelarReserva_EstadoNaoCancelavel_RetornaFalse(EstadoReserva estado)
        {
            var reserva = new Reserva { Id = 7, Estado = estado };
            _mockReservaRepo.Setup(r => r.GetById(7)).Returns(reserva);

            bool resultado = _reservaService.CancelarReserva(7, "Motivo", out string mensagem);

            Assert.IsFalse(resultado);
            Assert.That(reserva.Estado, Is.EqualTo(estado));
            _mockReservaRepo.Verify(r => r.Update(It.IsAny<Reserva>()), Times.Never);
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