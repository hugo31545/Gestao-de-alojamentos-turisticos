using Moq;
using NUnit.Framework;
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
        public void CreateUser_DadosValidos_RetornaTrue()
        {
            // Arrange
            
            _mockUserRepo.Setup(r => r.CreateUser("utilizadorValido", "Password123!"))
                         .Returns(true);

            // Act
            bool resultado = _authService.CreateUser("utilizadorValido", "Password123!", "Password123!");

            // Assert
            Assert.IsTrue(resultado);
            _mockUserRepo.Verify(r => r.CreateUser("utilizadorValido", "Password123!"), Times.Once);
        }

        #endregion

        
      
    }
}