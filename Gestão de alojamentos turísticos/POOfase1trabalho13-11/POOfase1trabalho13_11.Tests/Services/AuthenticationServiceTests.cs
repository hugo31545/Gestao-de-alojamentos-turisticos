using Moq;
using NUnit.Framework;
using POO.Utils.Security;
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Repositories.Interfaces;
using POOfase1trabalho13_11.Models.Services.Implementations;
using System;

namespace POOfase1trabalho13_11.Tests.Services
{
    [TestFixture]
    public class AuthenticationServiceTests
    {
        private Mock<IUserRepository> _mockUserRepo;
        private AuthenticationService _authService;

        [SetUp]
        public void Setup()
        {
            _mockUserRepo = new Mock<IUserRepository>();
            _authService = new AuthenticationService(_mockUserRepo.Object);
        }


        #region Testes de Criação de Utilizador

        [Test]
        public void CreateUser_DadosValidos_GuardaApenasOHash()
        {
            // Arrange
            string? hashGuardado = null;
            _mockUserRepo.Setup(r => r.CreateUser("utilizadorValido", It.IsAny<string>()))
                         .Callback<string, string>((_, hash) => hashGuardado = hash)
                         .Returns(true);

            // Act
            bool resultado = _authService.CreateUser("utilizadorValido", "Password123!", "Password123!", out string mensagem);

            // Assert
            Assert.IsTrue(resultado, mensagem);
            Assert.That(hashGuardado, Is.Not.Null.And.Not.EqualTo("Password123!"));
            Assert.IsTrue(PasswordHasher.Verificar("Password123!", hashGuardado));
        }

        [Test]
        public void CreateUser_PasswordFraca_RetornaFalseComMotivo()
        {
            bool resultado = _authService.CreateUser("utilizadorValido", "abc", "abc", out string mensagem);

            Assert.IsFalse(resultado);
            Assert.That(mensagem, Does.Contain("6 caracteres"));
            _mockUserRepo.Verify(r => r.CreateUser(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void CreateUser_PasswordsDiferentes_RetornaFalse()
        {
            bool resultado = _authService.CreateUser("utilizadorValido", "Password123", "Password124", out string mensagem);

            Assert.IsFalse(resultado);
            Assert.That(mensagem, Does.Contain("não coincidem"));
        }

        [Test]
        public void CreateUser_UsernameJaExiste_RetornaFalseComMotivo()
        {
            _mockUserRepo.Setup(r => r.CreateUser(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            bool resultado = _authService.CreateUser("existente", "Password123", "Password123", out string mensagem);

            Assert.IsFalse(resultado);
            Assert.That(mensagem, Does.Contain("já está a ser utilizado"));
        }

        #endregion

        #region Testes de Login

        [Test]
        public void UserExists_PasswordCorreta_RetornaTrue()
        {
            var user = new User { Id = 1, Username = "ana", PasswordHash = PasswordHasher.Hash("segredo1") };
            _mockUserRepo.Setup(r => r.GetUserByUsername("ana")).Returns(user);

            Assert.IsTrue(_authService.UserExists("ana", "segredo1"));
            Assert.IsFalse(_authService.UserExists("ana", "errada1"));
        }

        [Test]
        public void UserExists_ContaAntigaEmTextoSimples_MigraParaHash()
        {
            // Arrange: conta criada antes da introdução do hash
            var user = new User { Id = 1, Username = "antigo", Password = "antiga123" };
            _mockUserRepo.Setup(r => r.GetUserByUsername("antigo")).Returns(user);
            _mockUserRepo.Setup(r => r.Update(user)).Returns(true);

            // Act
            bool resultado = _authService.UserExists("antigo", "antiga123");

            // Assert
            Assert.IsTrue(resultado);
            Assert.IsNull(user.Password);
            Assert.IsTrue(PasswordHasher.Verificar("antiga123", user.PasswordHash));
            _mockUserRepo.Verify(r => r.Update(user), Times.Once);
        }

        [Test]
        public void UserExists_UtilizadorInexistente_RetornaFalse()
        {
            _mockUserRepo.Setup(r => r.GetUserByUsername(It.IsAny<string>())).Returns((User?)null);

            Assert.IsFalse(_authService.UserExists("ninguem", "qualquer1"));
        }

        #endregion
    }
}
