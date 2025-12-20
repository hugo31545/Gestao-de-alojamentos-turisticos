using NUnit.Framework;
using POOfase1trabalho13_11.Models.Entities;
using System;

namespace POOfase1trabalho13_11.Tests.Entities
{
    [TestFixture]
    public class ClienteTests
    {
        #region Testes de Construtor e Inicialização

        [Test]
        public void ConstrutorComParametros_DeveAtribuirPropriedadesCorretamente()
        {
            // Arrange
            int id = 10;
            string nome = "João Silva";
            string email = "joao@email.com";
            string tel = "912345678";
            string nif = "123456789";
            DateTime data = new DateTime(2024, 1, 1);

            // Act
            var cliente = new Cliente(id, nome, email, tel, nif, data);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(cliente.Id, Is.EqualTo(id));
                Assert.That(cliente.Nome, Is.EqualTo(nome));
                Assert.That(cliente.Email, Is.EqualTo(email));
                Assert.That(cliente.Telefone, Is.EqualTo(tel));
                Assert.That(cliente.Nif, Is.EqualTo(nif));
                Assert.That(cliente.DataRegisto, Is.EqualTo(data));
            });
        }

        [Test]
        public void Construtor_DeveGerarDataAtual_QuandoDataForDefault()
        {
            // Arrange
            DateTime antes = DateTime.Now.AddSeconds(-1);

            // Act
            var cliente = new Cliente(1, "Teste", "e@e.com", "123", "123", default);

            // Assert
            Assert.That(cliente.DataRegisto, Is.GreaterThanOrEqualTo(antes));
        }

        #endregion

        #region Testes de Validação (Exceções e Fallbacks)

        [Test]
        public void SetId_Negativo_DeveLancarArgumentException()
        {
            // Arrange
            var cliente = new Cliente();

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => cliente.Id = -1);
            Assert.That(ex.Message, Does.Contain("ID não pode ser negativo."));
        }

        [Test]
        public void SetNome_NuloOuVazio_DeveLancarArgumentException()
        {
            // Arrange
            var cliente = new Cliente();

            // Act & Assert
            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentException>(() => cliente.Nome = null!);
                Assert.Throws<ArgumentException>(() => cliente.Nome = "");
                Assert.Throws<ArgumentException>(() => cliente.Nome = "   ");
            });
        }

        [Test]
        public void SetCamposOpcionais_SeNulos_DeveConverterParaStringVazia()
        {
            // Arrange
            var cliente = new Cliente();

            // Act
            cliente.Email = null!;
            cliente.Telefone = null!;
            cliente.Nif = null!;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(cliente.Email, Is.EqualTo(string.Empty));
                Assert.That(cliente.Telefone, Is.EqualTo(string.Empty));
                Assert.That(cliente.Nif, Is.EqualTo(string.Empty));
            });
        }

        #endregion

        #region Testes de Robustez (Fronteira)

        [Test]
        public void Cliente_DeveAceitarNifComQualquerTamanho_PoisValidacaoFoiRemovidaDoSetter()
        {
            // Arrange
            var cliente = new Cliente();
            string nifLongo = "123456789012345";

            // Act
            cliente.Nif = nifLongo;

            // Assert
            Assert.That(cliente.Nif, Is.EqualTo(nifLongo));
        }

        [Test]
        public void Id_PodeSerZero()
        {
            // Arrange
            var cliente = new Cliente();

            // Act
            cliente.Id = 0;

            // Assert
            Assert.That(cliente.Id, Is.EqualTo(0));
        }

        #endregion

        [Test]
        public void Nome_DeveSuportarNomesLongos()
        {
            // Arrange
            var cliente = new Cliente();
            
            string nomeLongo = new string('A', 200);

            // Act
            cliente.Nome = nomeLongo;

            // Assert
            Assert.That(cliente.Nome.Length, Is.EqualTo(200), "O sistema deve suportar nomes longos sem truncar.");
        }

        [Test]
        public void Email_DeveAceitarFormatosDiferentes()
        {
            // Arrange
            var cliente = new Cliente();
            string emailComplexo = "teste.sobrenome+filtro@subdominio.hotel.com";

            // Act
            cliente.Email = emailComplexo;

            // Assert
            Assert.That(cliente.Email, Is.EqualTo(emailComplexo), "O campo de email deve preservar caracteres especiais como + e .");
        }

        [Test]
        public void Telefone_DevePreservarEspacosEIndicativos()
        {
            // Arrange
            var cliente = new Cliente();
            string telefoneInternacional = "+351 912 345 678";

            // Act
            cliente.Telefone = telefoneInternacional;

            // Assert
            Assert.That(cliente.Telefone, Is.EqualTo(telefoneInternacional), "O telefone deve permitir carateres como '+' e espaços.");
        }

        [Test]
        public void Cliente_DiferentesInstancias_DevemTerDatasIndependentes()
        {
            // Act
            var cliente1 = new Cliente { DataRegisto = DateTime.Now.AddDays(-1) };
            var cliente2 = new Cliente { DataRegisto = DateTime.Now };

            // Assert
            Assert.That(cliente1.DataRegisto, Is.Not.EqualTo(cliente2.DataRegisto), "Cada cliente deve manter a sua própria data de registo única.");
        }
    }
}