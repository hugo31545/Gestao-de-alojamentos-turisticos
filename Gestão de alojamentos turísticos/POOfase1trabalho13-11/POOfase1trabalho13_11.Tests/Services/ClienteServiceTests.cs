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
    public class ClienteServiceTests
    {
        private Mock<IClienteRepository> _mockClienteRepo;
        private Mock<IRegistoRepository> _mockRegistoRepo;
        private Mock<IReservaRepository> _mockReservaRepo;
        private ClienteService _clienteService;

        [SetUp]
        public void Setup()
        {
            _mockClienteRepo = new Mock<IClienteRepository>();
            _mockRegistoRepo = new Mock<IRegistoRepository>();
            _mockReservaRepo = new Mock<IReservaRepository>();
            _clienteService = new ClienteService(_mockClienteRepo.Object, _mockRegistoRepo.Object, _mockReservaRepo.Object);
        }

        #region Testes de Registo

        [Test]
        public void RegistarCliente_NifDuplicado_RetornaFalse()
        {
            // Arrange
            var novoCliente = new Cliente { Nome = "Novo", Nif = "123456789", Email = "novo@test.com" };
           
            _mockClienteRepo.Setup(r => r.GetByNif("123456789")).Returns(new Cliente());

            // Act
            bool resultado = _clienteService.RegistarCliente(novoCliente, out string mensagem);

            // Assert
            Assert.IsFalse(resultado);
            Assert.That(mensagem, Does.Contain("NIF"));
            _mockClienteRepo.Verify(r => r.Add(It.IsAny<Cliente>()), Times.Never);
        }

        [Test]
        public void RegistarCliente_EmailDuplicado_RetornaFalse()
        {
            // Arrange
            var novoCliente = new Cliente { Nome = "Novo", Nif = "999999999", Email = "existente@test.com" };
            _mockClienteRepo.Setup(r => r.GetByNif(It.IsAny<string>())).Returns((Cliente?)null);
            
            _mockClienteRepo.Setup(r => r.GetByEmail("existente@test.com")).Returns(new Cliente());

            // Act
            bool resultado = _clienteService.RegistarCliente(novoCliente, out string mensagem);

            // Assert
            Assert.IsFalse(resultado);
            Assert.That(mensagem, Does.Contain("e-mail"));
        }

        [Test]
        public void RegistarCliente_Sucesso_DefineDataERegistaLog()
        {
            // Arrange
            var novoCliente = new Cliente { Id = 10, Nome = "Joaquim", Nif = "111111111", Email = "j@t.com" };
            _mockClienteRepo.Setup(r => r.GetByNif(It.IsAny<string>())).Returns((Cliente?)null);
            _mockClienteRepo.Setup(r => r.GetByEmail(It.IsAny<string>())).Returns((Cliente?)null);

            // Act
            bool resultado = _clienteService.RegistarCliente(novoCliente, out string mensagem);

            // Assert
            Assert.IsTrue(resultado);
            Assert.That(novoCliente.DataRegisto.Date, Is.EqualTo(DateTime.Now.Date));
            _mockRegistoRepo.Verify(r => r.Add(It.Is<Registo>(l => l.Operacao == "CREATE")), Times.Once);
        }

        #endregion

        #region Testes de Atualização e Remoção

        [Test]
        public void AtualizarDados_NifJaPertenceAOutroCliente_RetornaFalse()
        {
            // Arrange
            var clienteEditado = new Cliente { Id = 1, Nif = "123456789", Nome = "Editado", Email = "editado@test.com" };
            
            _mockClienteRepo.Setup(r => r.GetByNif("123456789")).Returns(new Cliente { Id = 2, Nif = "123456789" });

            // Act
            bool resultado = _clienteService.AtualizarDados(clienteEditado, out string mensagem);

            // Assert
            Assert.IsFalse(resultado);
            Assert.That(mensagem, Does.Contain("NIF indicado já pertence a outro"));
        }

        [Test]
        public void AtualizarDados_Sucesso_ChamaUpdateERegistaLog()
        {
            // Arrange
            var cliente = new Cliente { Id = 1, Nome = "Ana", Nif = "123456789", Email = "ana@test.com" };
            _mockClienteRepo.Setup(r => r.GetByNif("123")).Returns(cliente); 
            _mockClienteRepo.Setup(r => r.Update(cliente)).Returns(true);

            // Act
            bool resultado = _clienteService.AtualizarDados(cliente, out string mensagem);

            // Assert
            Assert.IsTrue(resultado);
            _mockRegistoRepo.Verify(r => r.Add(It.Is<Registo>(l => l.Operacao == "UPDATE")), Times.Once);
        }

        [Test]
        public void RemoverCliente_Inexistente_RetornaFalse()
        {
            // Arrange
            _mockClienteRepo.Setup(r => r.Remove(99)).Returns(false);

            // Act
            bool resultado = _clienteService.RemoverCliente(99, out string mensagem);

            // Assert
            Assert.IsFalse(resultado);
            Assert.That(mensagem, Does.Contain("não encontrado"));
            _mockRegistoRepo.Verify(r => r.Add(It.IsAny<Registo>()), Times.Never);
        }

        [TestCase("12345", "ana@ipca.pt", "NIF")]
        [TestCase("12345678X", "ana@ipca.pt", "NIF")]
        [TestCase("123456789", "novo@", "email")]
        public void RegistarCliente_FormatoInvalido_RetornaFalseSemGravar(string nif, string email, string campo)
        {
            var cliente = new Cliente { Nome = "Ana", Nif = nif, Email = email };

            bool resultado = _clienteService.RegistarCliente(cliente, out string mensagem);

            Assert.IsFalse(resultado);
            Assert.That(mensagem, Does.Contain(campo));
            _mockClienteRepo.Verify(r => r.Add(It.IsAny<Cliente>()), Times.Never);
        }

        [Test]
        public void AtualizarDados_EmailDeOutroCliente_RetornaFalse()
        {
            var cliente = new Cliente { Id = 1, Nome = "Ana", Nif = "123456789", Email = "rui@ipca.pt" };
            _mockClienteRepo.Setup(r => r.GetByEmail("rui@ipca.pt")).Returns(new Cliente { Id = 2, Nome = "Rui" });

            bool resultado = _clienteService.AtualizarDados(cliente, out string mensagem);

            Assert.IsFalse(resultado);
            Assert.That(mensagem, Does.Contain("e-mail"));
            _mockClienteRepo.Verify(r => r.Update(It.IsAny<Cliente>()), Times.Never);
        }

        [Test]
        public void RemoverCliente_ComReservaAtiva_RetornaFalseSemRemover()
        {
            // Arrange
            _mockReservaRepo.Setup(r => r.FiltrarPorCliente(1))
                            .Returns(new List<Reserva> { new Reserva { ClienteId = 1, Estado = POOfase1trabalho13_11.Models.Enums.EstadoReserva.Pendente } });

            // Act
            bool resultado = _clienteService.RemoverCliente(1, out string mensagem);

            // Assert
            Assert.IsFalse(resultado);
            Assert.That(mensagem, Does.Contain("reservas ativas"));
            _mockClienteRepo.Verify(r => r.Remove(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void RemoverCliente_Sucesso_RegistaDelete()
        {
            // Arrange
            _mockClienteRepo.Setup(r => r.Remove(1)).Returns(true);

            // Act
            bool resultado = _clienteService.RemoverCliente(1, out string mensagem);

            // Assert
            Assert.IsTrue(resultado);
            _mockRegistoRepo.Verify(r => r.Add(It.Is<Registo>(l => l.Operacao == "DELETE")), Times.Once);
        }

        #endregion
    }
}