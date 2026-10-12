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
    public class EmployeeServiceTests
    {
        private Mock<IEmployeeRepository> _mockEmployeeRepo;
        private Mock<IRegistoRepository> _mockRegistoRepo;
        private EmployeeService _employeeService;

        [SetUp]
        public void Setup()
        {
            _mockEmployeeRepo = new Mock<IEmployeeRepository>();
            _mockRegistoRepo = new Mock<IRegistoRepository>();

            _employeeService = new EmployeeService(
                _mockEmployeeRepo.Object,
                _mockRegistoRepo.Object
            );
        }

        #region Testes de Validação de Novo Funcionário

        [TestCase("", "ana@ipca.pt", "123456789", "nome")]
        [TestCase("Ana", "ana@", "123456789", "email")]
        [TestCase("Ana", "ana@ipca", "123456789", "email")]
        [TestCase("Ana", "ana@ipca.pt", "12345678", "NIF")]
        [TestCase("Ana", "ana@ipca.pt", "12345678A", "NIF")]
        public void ValidarNovoFuncionario_DadosInvalidos_RetornaFalse(string nome, string email, string nif, string campo)
        {
            _mockEmployeeRepo.Setup(r => r.GetAll()).Returns(new List<Employee>());

            bool valido = _employeeService.ValidarNovoFuncionario(nome, email, nif, out string mensagem);

            Assert.IsFalse(valido);
            Assert.That(mensagem, Does.Contain(campo).IgnoreCase);
        }

        [Test]
        public void ValidarNovoFuncionario_EmailDuplicado_RetornaFalse()
        {
            _mockEmployeeRepo.Setup(r => r.GetAll()).Returns(new List<Employee>());
            _mockEmployeeRepo.Setup(r => r.GetByEmail("ana@ipca.pt"))
                             .Returns(new Employee { Id = 1, Nome = "Ana", NIF = "987654321", Email = "ana@ipca.pt" });

            bool valido = _employeeService.ValidarNovoFuncionario("Ana Nova", "ana@ipca.pt", "123456789", out string mensagem);

            Assert.IsFalse(valido);
            Assert.That(mensagem, Does.Contain("email"));
        }

        [Test]
        public void ValidarNovoFuncionario_DadosValidos_RetornaTrue()
        {
            _mockEmployeeRepo.Setup(r => r.GetAll()).Returns(new List<Employee>());

            bool valido = _employeeService.ValidarNovoFuncionario("Ana", "ana@ipca.pt", "123456789", out string mensagem);

            Assert.IsTrue(valido, mensagem);
        }

        [Test]
        public void AdicionarFuncionario_NifInvalido_RetornaFalseSemExcecao()
        {
            bool resultado = false;

            Assert.DoesNotThrow(() => resultado = _employeeService.AdicionarFuncionario("Ana", "ana@ipca.pt", "123", "Rececionista"));
            Assert.IsFalse(resultado);
            _mockEmployeeRepo.Verify(r => r.Save(It.IsAny<Employee>()), Times.Never);
        }

        #endregion

        #region Testes de Validação e Escrita

        [Test]
        public void GuardarFuncionario_NifDuplicado_RetornaFalse()
        {
            // Arrange
            string nifRepetido = "123456789";
            var funcionarioNovo = new Employee { Id = 0, NIF = nifRepetido, Nome = "Novo" };

           
            var listaExistente = new List<Employee>
            {
                new Employee { Id = 1, NIF = nifRepetido, Nome = "Antigo" }
            };
            _mockEmployeeRepo.Setup(r => r.GetAll()).Returns(listaExistente);

            // Act
            bool resultado = _employeeService.GuardarFuncionario(funcionarioNovo, out string mensagem);

            // Assert
            Assert.IsFalse(resultado);
            Assert.That(mensagem, Does.Contain("Já existe um funcionário registado com este NIF"));
            _mockEmployeeRepo.Verify(r => r.Save(It.IsAny<Employee>()), Times.Never);
        }

        [Test]
        public void AdicionarFuncionario_DadosValidos_RetornaTrueERegistaLog()
        {
            // Arrange
            _mockEmployeeRepo.Setup(r => r.GetAll()).Returns(new List<Employee>());
            _mockEmployeeRepo.Setup(r => r.Save(It.IsAny<Employee>())).Returns(true);

            // Act
            bool resultado = _employeeService.AdicionarFuncionario("Joao", "joao@email.com", "987654321", "Gerente");

            // Assert
            Assert.IsTrue(resultado);
            
            _mockRegistoRepo.Verify(r => r.Add(It.Is<Registo>(log => log.Operacao == "CREATE")), Times.Once);
        }

        #endregion

        #region Testes de Remoção

        [Test]
        public void RemoverFuncionario_Existente_RetornaTrueERegistaDelete()
        {
            // Arrange
            int idParaRemover = 5;
            var funcionario = new Employee { Id = idParaRemover, Nome = "Carlos" };

            _mockEmployeeRepo.Setup(r => r.GetById(idParaRemover)).Returns(funcionario);
            _mockEmployeeRepo.Setup(r => r.Remove(idParaRemover)).Returns(true);

            // Act
            bool resultado = _employeeService.RemoverFuncionario(idParaRemover, out string mensagem);

            // Assert
            Assert.IsTrue(resultado);
            Assert.That(mensagem, Does.Contain("sucesso"));
            _mockRegistoRepo.Verify(r => r.Add(It.Is<Registo>(log => log.Operacao == "DELETE")), Times.Once);
        }

        [Test]
        public void RemoverFuncionario_Inexistente_RetornaFalse()
        {
            // Arrange
            _mockEmployeeRepo.Setup(r => r.GetById(It.IsAny<int>())).Returns((Employee?)null);

            // Act
            bool resultado = _employeeService.RemoverFuncionario(999, out string mensagem);

            // Assert
            Assert.IsFalse(resultado);
            Assert.That(mensagem, Is.EqualTo("Funcionário não encontrado."));
        }

        #endregion

        #region Testes de Atualização

        [Test]
        public void AtualizarDados_IdInvalido_RetornaFalse()
        {
            // Arrange
            var empInvalido = new Employee { Id = 0 }; 

            // Act
            bool resultado = _employeeService.AtualizarDados(empInvalido, out string mensagem);

            // Assert
            Assert.IsFalse(resultado);
            Assert.That(mensagem, Does.Contain("ID de funcionário inválido"));
        }

        [Test]
        public void VerificarNifDuplicado_SeExiste_RetornaTrue()
        {
            // Arrange
            string nif = "222333444";
            _mockEmployeeRepo.Setup(r => r.GetAll()).Returns(new List<Employee> { new Employee { NIF = nif } });

            // Act
            bool existe = _employeeService.VerificarNifDuplicado(nif);

            // Assert
            Assert.IsTrue(existe);
        }

        #endregion
        #region Testes de Atualização e Casos Especiais

        
        

        [Test]
        public void GuardarFuncionario_UpdateComSucesso_RegistaLogUpdate()
        {
            // Arrange
            var empExistente = new Employee { Id = 1, Nome = "Antigo", NIF = "123456789" };

            _mockEmployeeRepo.Setup(r => r.GetAll()).Returns(new List<Employee>());
            _mockEmployeeRepo.Setup(r => r.Save(It.IsAny<Employee>())).Returns(true);

            // Act
            
            bool resultado = _employeeService.GuardarFuncionario(empExistente, out string mensagem);

            // Assert
            Assert.IsTrue(resultado);
            _mockRegistoRepo.Verify(r => r.Add(It.Is<Registo>(log => log.Operacao == "UPDATE")), Times.Once);
        }

        

        [Test]
        public void Construtor_DependenciaNula_DeveLancarArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new EmployeeService(null!, _mockRegistoRepo.Object));
            Assert.Throws<ArgumentNullException>(() => new EmployeeService(_mockEmployeeRepo.Object, null!));
        }

        [Test]
        public void GuardarFuncionario_SeObjetoNulo_RetornaFalse()
        {
            // Act
            bool resultado = _employeeService.GuardarFuncionario(null!, out string mensagem);

            // Assert
            Assert.IsFalse(resultado);
            Assert.That(mensagem, Does.Contain("Dados do funcionário são inválidos"));
        }

        #endregion
    }
}