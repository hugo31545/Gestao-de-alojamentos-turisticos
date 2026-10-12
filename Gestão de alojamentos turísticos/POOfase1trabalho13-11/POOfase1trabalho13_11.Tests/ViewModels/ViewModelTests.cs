using Moq;
using NUnit.Framework;
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Enums;
using POOfase1trabalho13_11.Models.Services.Implementations;
using POOfase1trabalho13_11.Models.Services.Interfaces;
using POOfase1trabalho13_11.ViewModels;
using POOfase1trabalho13_11.ViewModels.Services;
using POOfase1trabalho13_11.Views.Interfaces;
using System;
using System.Collections.Generic;

namespace POOfase1trabalho13_11.Tests.ViewModels
{
    /// <summary>
    /// Diálogo falso: regista as mensagens em vez de abrir janelas.
    /// </summary>
    public class DialogoFalso : IDialogService
    {
        public List<string> Mensagens { get; } = new();
        public bool RespostaConfirmar { get; set; } = true;

        public void Informar(string mensagem, string titulo = "Informação") => Mensagens.Add($"INFO: {mensagem}");
        public void Avisar(string mensagem, string titulo = "Atenção") => Mensagens.Add($"AVISO: {mensagem}");
        public void Erro(string mensagem, string titulo = "Erro") => Mensagens.Add($"ERRO: {mensagem}");
        public bool Confirmar(string mensagem, string titulo = "Confirmar") => RespostaConfirmar;
    }

    [TestFixture]
    public class ReservaViewModelTests
    {
        private Mock<IReservaService> _reservaService;
        private Mock<ICheckInService> _checkInService;
        private SessionService _sessao;
        private DialogoFalso _dialogo;
        private ReservaViewModel _vm;

        [SetUp]
        public void Setup()
        {
            _reservaService = new Mock<IReservaService>();
            _checkInService = new Mock<ICheckInService>();
            _sessao = new SessionService();
            _sessao.Iniciar("ana@ipca.pt");
            _dialogo = new DialogoFalso();

            _vm = new ReservaViewModel(_reservaService.Object, new Mock<IClienteService>().Object,
                new Mock<IAlojamentoService>().Object, _checkInService.Object, _sessao, _dialogo);
        }

        [TestCase(EstadoReserva.Pendente, true, false, false, true)]
        [TestCase(EstadoReserva.Confirmada, false, true, false, true)]
        [TestCase(EstadoReserva.CheckInRealizado, false, false, true, false)]
        [TestCase(EstadoReserva.Finalizada, false, false, false, false)]
        [TestCase(EstadoReserva.Cancelada, false, false, false, false)]
        public void Comandos_SoAtivosNoEstadoCerto(EstadoReserva estado, bool confirmar, bool checkIn, bool checkOut, bool cancelar)
        {
            var reserva = new Reserva { Id = 1, Estado = estado };

            Assert.That(_vm.ConfirmarReservaCommand.CanExecute(reserva), Is.EqualTo(confirmar), "Confirmar");
            Assert.That(_vm.CheckInCommand.CanExecute(reserva), Is.EqualTo(checkIn), "Check-In");
            Assert.That(_vm.CheckOutCommand.CanExecute(reserva), Is.EqualTo(checkOut), "Check-Out");
            Assert.That(_vm.CancelarReservaCommand.CanExecute(reserva), Is.EqualTo(cancelar), "Cancelar");
        }

        [Test]
        public void CheckIn_UsaCheckInServiceComUtilizadorDaSessao()
        {
            var reserva = new Reserva { Id = 3, Estado = EstadoReserva.Confirmada };
            string msg = "ok";
            _checkInService.Setup(s => s.RealizarCheckIn(3, "ana@ipca.pt", out msg)).Returns(true);

            _vm.CheckInCommand.Execute(reserva);

            _checkInService.Verify(s => s.RealizarCheckIn(3, "ana@ipca.pt", out msg), Times.Once);
            Assert.That(reserva.Estado, Is.EqualTo(EstadoReserva.Confirmada), "A VM não deve alterar o estado diretamente.");
        }

        [Test]
        public void Cancelar_FalhaNoServico_MostraAviso()
        {
            var reserva = new Reserva { Id = 4, Estado = EstadoReserva.Confirmada };
            string msg = "Não é possível cancelar.";
            _reservaService.Setup(s => s.CancelarReserva(4, It.IsAny<string>(), out msg)).Returns(false);
            _vm.MotivoCancelamento = "Desistência";

            _vm.CancelarReservaCommand.Execute(reserva);

            Assert.That(_dialogo.Mensagens, Has.Some.Contains("Não é possível cancelar."));
        }
    }

    [TestFixture]
    public class RegistryViewModelTests
    {
        private Mock<IEmployeeService> _employeeService;
        private Mock<IAuthenticationService> _authService;
        private DialogoFalso _dialogo;
        private RegistryViewModel _vm;

        [SetUp]
        public void Setup()
        {
            _employeeService = new Mock<IEmployeeService>();
            _authService = new Mock<IAuthenticationService>();
            _dialogo = new DialogoFalso();
            _vm = new RegistryViewModel(_employeeService.Object, new Mock<IRegistoService>().Object,
                new Mock<IViewFactory>().Object, _authService.Object, _dialogo)
            {
                Nome = "Ana",
                Email = "ana@ipca.pt",
                NIF = "123",
                Password = "Segredo123",
                PasswordConfirmacao = "Segredo123"
            };
        }

        [Test]
        public void Registo_DadosInvalidos_NaoCriaUtilizador()
        {
            string msg = "O NIF deve ter exatamente 9 dígitos.";
            _employeeService.Setup(s => s.ValidarNovoFuncionario("Ana", "ana@ipca.pt", "123", out msg)).Returns(false);

            _vm.RegistarCommand.Execute(null);

            Assert.That(_vm.MensagemFeedback, Does.Contain("NIF"));
            string ignorado;
            _authService.Verify(a => a.CreateUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), out ignorado), Times.Never);
        }

        [Test]
        public void Registo_FuncionarioFalha_RemoveUtilizadorCriado()
        {
            string vazio = string.Empty;
            _employeeService.Setup(s => s.ValidarNovoFuncionario(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), out vazio)).Returns(true);
            _authService.Setup(a => a.CreateUser("ana@ipca.pt", "Segredo123", "Segredo123", out vazio)).Returns(true);
            _employeeService.Setup(s => s.AdicionarFuncionario(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            _vm.RegistarCommand.Execute(null);

            _authService.Verify(a => a.RemoveUser("ana@ipca.pt"), Times.Once);
            Assert.That(_vm.MensagemFeedback, Does.Contain("Nenhum dado foi gravado"));
        }
    }
}
