using NUnit.Framework;
using POOfase1trabalho13_11.Models.Entities;
using System;

namespace POOfase1trabalho13_11.Tests.Entities
{
    [TestFixture]
    public class EmployeeTests
    {
        #region Testes de Sucesso (Happy Path)

        [Test]
        public void ConstrutorComParametros_DeveAtribuirPropriedadesCorretamente()
        {
            // Arrange
            int id = 1;
            string nome = "Carlos Silva";
            string nif = "123456789";
            string cargo = "Gerente";
            string email = "carlos@hotel.com";
            DateTime data = new DateTime(2023, 5, 10);

            // Act
            var employee = new Employee(id, nome, nif, cargo, email, data);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(employee.Id, Is.EqualTo(id));
                Assert.That(employee.Nome, Is.EqualTo(nome));
                Assert.That(employee.NIF, Is.EqualTo(nif));
                Assert.That(employee.Cargo, Is.EqualTo(cargo));
                Assert.That(employee.Email, Is.EqualTo(email));
                Assert.That(employee.DataContratacao, Is.EqualTo(data));
            });
        }

        [Test]
        public void SetCargo_SeNulo_DeveDefinirComoRececionista()
        {
            // Arrange
            var employee = new Employee { Cargo = "Admin" };

            // Act
            employee.Cargo = null!;

            // Assert
            Assert.That(employee.Cargo, Is.EqualTo("Rececionista"));
        }

        #endregion

        #region Testes de Validação (Exceções)

        [Test]
        public void SetId_Negativo_DeveLancarArgumentException()
        {
            // Arrange
            var employee = new Employee();

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => employee.Id = -1);
            Assert.That(ex.Message, Is.EqualTo("ID não pode ser negativo."));
        }

        [Test]
        public void SetNome_VazioOuEspacos_DeveLancarArgumentException()
        {
            // Arrange
            var employee = new Employee();

            // Act & Assert
            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentException>(() => employee.Nome = "");
                Assert.Throws<ArgumentException>(() => employee.Nome = "   ");
            });
        }

        [Test]
        public void SetNIF_ComComprimentoIncorreto_DeveLancarArgumentException()
        {
            // Arrange
            var employee = new Employee();

            // Act & Assert
            Assert.Multiple(() =>
            {
                // NIF com 8 dígitos
                var ex8 = Assert.Throws<ArgumentException>(() => employee.NIF = "12345678");
                // NIF com 10 dígitos
                var ex10 = Assert.Throws<ArgumentException>(() => employee.NIF = "1234567890");

                Assert.That(ex8.Message, Is.EqualTo("NIF deve ter 9 dígitos."));
            });
        }

        #endregion

        #region Testes de Valores Padrão

        [Test]
        public void ConstrutorPadrao_DeveDefinirCargoPadraoEDataAtual()
        {
            // Arrange
            DateTime antes = DateTime.Now.AddSeconds(-1);

            // Act
            var employee = new Employee();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(employee.Cargo, Is.EqualTo("Rececionista"));
                Assert.That(employee.DataContratacao, Is.GreaterThanOrEqualTo(antes));
            });
        }

        #endregion

        [Test]
        public void Registo_DeveSuportarDetalhesMuitoExtensos()
        {
            // Arrange 
            var registo = new Registo();
            string logGigante = new string('A', 10000);

            // Act
            registo.Detalhes = logGigante;

            // Assert
            Assert.That(registo.Detalhes.Length, Is.EqualTo(10000), "O log deve ser capaz de armazenar detalhes extensos sem truncar.");
        }

        [Test]
        public void DataRegisto_NaoDeveMudarAposAtribuicao()
        {
            // Arrange
            var dataFixa = new DateTime(2025, 12, 25, 15, 30, 0);
            var registo = new Registo(1, "Teste", "D", "User", "OP", 1, dataFixa);

            // Act
            
            var dataLida = registo.DataRegisto;

            // Assert
            Assert.That(dataLida, Is.EqualTo(dataFixa), "A data de registo deve ser preservada exatamente como foi gravada.");
        }

        [Test]
        public void TipoEntidade_DeveTratarCaracteresEspeciais()
        {
            // Arrange
            var registo = new Registo();
            string tipoComLixo = "\n\t Reserva \r";

            // Act
            registo.TipoEntidade = tipoComLixo;

            // Assert
            
            Assert.That(registo.TipoEntidade, Is.EqualTo(tipoComLixo));
        }

        [Test]
        public void Registo_IgualdadeDeInstancia_PropriedadesDevemSerIndependentes()
        {
            // Arrange
            var registo1 = new Registo { Id = 1, Utilizador = "Admin" };
            var registo2 = new Registo { Id = 2, Utilizador = "User" };

            // Act & Assert
            Assert.That(registo1.Utilizador, Is.Not.EqualTo(registo2.Utilizador), "Instâncias diferentes de Registo não devem partilhar referências de memória.");
        }

        
    }
}