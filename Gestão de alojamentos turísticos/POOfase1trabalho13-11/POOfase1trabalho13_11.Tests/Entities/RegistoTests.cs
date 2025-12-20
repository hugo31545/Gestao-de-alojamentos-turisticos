using NUnit.Framework;
using POOfase1trabalho13_11.Models.Entities;
using System;

namespace POOfase1trabalho13_11.Tests.Entities
{
    [TestFixture]
    public class RegistoTests
    {
        [Test]
        public void ConstrutorComParametros_DeveAtribuirPropriedadesCorretamente()
        {
            // Arrange
            int id = 1;
            string tipo = "Reserva";
            string detalhes = "Criação de nova reserva";
            string user = "Admin";
            string op = "CREATE";
            int entId = 101;
            DateTime data = new DateTime(2023, 11, 13, 10, 0, 0);

            // Act
            var registo = new Registo(id, tipo, detalhes, user, op, entId, data);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(registo.Id, Is.EqualTo(id));
                Assert.That(registo.TipoEntidade, Is.EqualTo(tipo));
                Assert.That(registo.Detalhes, Is.EqualTo(detalhes));
                Assert.That(registo.Utilizador, Is.EqualTo(user));
                Assert.That(registo.Operacao, Is.EqualTo(op));
                Assert.That(registo.EntidadeId, Is.EqualTo(entId));
                Assert.That(registo.DataRegisto, Is.EqualTo(data));
            });
        }

        [Test]
        public void SetId_ComValorNegativo_DeveLancarArgumentException()
        {
            // Arrange
            var registo = new Registo();
            int idInvalido = -5;

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => registo.Id = idInvalido);
            Assert.That(ex.Message, Is.EqualTo("ID inválido."));
        }

        [Test]
        public void SetTipoEntidade_SeNuloOuVazio_DeveDefinirComoDesconhecido()
        {
            // Arrange
            var registo = new Registo();

            // Act
            registo.TipoEntidade = "";
            string resultadoVazio = registo.TipoEntidade;

            registo.TipoEntidade = null!;
            string resultadoNulo = registo.TipoEntidade;

            // Assert
            Assert.That(resultadoVazio, Is.EqualTo("Desconhecido"));
            Assert.That(resultadoNulo, Is.EqualTo("Desconhecido"));
        }

        [Test]
        public void SetUtilizador_SeNuloOuEspacos_DeveDefinirComoSistema()
        {
            // Arrange
            var registo = new Registo();

            // Act
            registo.Utilizador = "   "; 

            // Assert
            Assert.That(registo.Utilizador, Is.EqualTo("Sistema"));
        }

        [Test]
        public void SetDetalhes_SeNulo_DeveDefinirComoStringVazia()
        {
            // Arrange
            var registo = new Registo();

            // Act
            registo.Detalhes = null!;

            // Assert
            Assert.That(registo.Detalhes, Is.EqualTo(string.Empty));
        }

        [Test]
        public void ConstrutorPadrao_DeveDefinirDataRegistoAtual()
        {
            // Arrange
            DateTime antes = DateTime.Now.AddSeconds(-1);

            // Act
            var registo = new Registo();

            // Assert
            Assert.That(registo.DataRegisto, Is.GreaterThanOrEqualTo(antes));
            Assert.That(registo.DataRegisto, Is.LessThanOrEqualTo(DateTime.Now));
        }

        [Test]
        public void SetOperacao_SeNulo_DeveDefinirComoLOG()
        {
            // Arrange
            var registo = new Registo();

            // Act
            registo.Operacao = null!;

            // Assert
            Assert.That(registo.Operacao, Is.EqualTo("LOG"), "Se a operação for nula, o sistema deve assumir o padrão 'LOG'.");
        }

        [Test]
        public void Construtor_SeDataRegistoForDefault_DeveAtribuirDataAtual()
        {
            // Arrange
            
            DateTime dataDefault = default;

            // Act
            var registo = new Registo(1, "Teste", "Detalhe", "User", "OP", 10, dataDefault);

            // Assert
            Assert.That(registo.DataRegisto, Is.GreaterThan(new DateTime(2020, 1, 1)),
                "Se a data for nula/default na desserialização, deve ser gerada a data atual.");
        }

        [Test]
        public void TipoEntidade_SeApenasEspacos_DeveRetornarDesconhecido()
        {
            // Arrange
            var registo = new Registo();

            // Act
            registo.TipoEntidade = "    ";

            // Assert
            Assert.That(registo.TipoEntidade, Is.EqualTo("Desconhecido"), "Espaços em branco devem ser tratados como 'Desconhecido'.");
        }

        [Test]
        public void EntidadeId_DevePermitirQualquerValorInteiro()
        {
            // Arrange
            var registo = new Registo();
            int idEsperado = 999;

            // Act
            registo.EntidadeId = idEsperado;

            // Assert
            Assert.That(registo.EntidadeId, Is.EqualTo(idEsperado));
        }
    }
}