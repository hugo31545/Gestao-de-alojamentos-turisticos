using NUnit.Framework;
using POOfase1trabalho13_11.Models.Entities;
using System;

namespace POOfase1trabalho13_11.Tests.Entities
{
    [TestFixture]
    public class CheckInTests
    {
        #region Testes de Construtor e Inicialização

        [Test]
        public void ConstrutorComParametros_DeveAtribuirPropriedadesCorretamente()
        {
            // Arrange
            int id = 1;
            int reservaId = 100;
            DateTime data = new DateTime(2023, 11, 15, 14, 30, 0);
            string func = "Maria Rececionista";
            int hospedes = 2;
            string obs = "Check-in antecipado";
            bool docs = true;

            // Act
            var checkIn = new CheckIn(id, reservaId, data, func, hospedes, obs, docs);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(checkIn.Id, Is.EqualTo(id));
                Assert.That(checkIn.ReservaId, Is.EqualTo(reservaId));
                Assert.That(checkIn.DataHora, Is.EqualTo(data));
                Assert.That(checkIn.FuncionarioResponsavel, Is.EqualTo(func));
                Assert.That(checkIn.NumeroHospedes, Is.EqualTo(hospedes));
                Assert.That(checkIn.Observacoes, Is.EqualTo(obs));
                Assert.That(checkIn.DocumentosVerificados, Is.True);
            });
        }

        [Test]
        public void Construtor_DeveUsarDataAtual_QuandoDataDefaultForPassada()
        {
            // Arrange
            DateTime antes = DateTime.Now.AddSeconds(-1);

            // Act
            var checkIn = new CheckIn(1, 10, default, "Func", 1, "", true);

            // Assert
            Assert.That(checkIn.DataHora, Is.GreaterThanOrEqualTo(antes));
        }

        #endregion

        #region Testes de Validação (Exceções)

        [Test]
        public void SetId_Negativo_DeveLancarArgumentException()
        {
            // Arrange
            var checkIn = new CheckIn();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => checkIn.Id = -1);
        }

        [Test]
        public void SetNumeroHospedes_Invalido_DeveLancarArgumentException()
        {
            // Arrange
            var checkIn = new CheckIn();

            // Act & Assert
            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentException>(() => checkIn.NumeroHospedes = 0);
                Assert.Throws<ArgumentException>(() => checkIn.NumeroHospedes = -5);
            });
        }

        [Test]
        public void SetFuncionarioResponsavel_VazioOuNulo_DeveLancarArgumentException()
        {
            // Arrange
            var checkIn = new CheckIn();

            // Act & Assert
            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentException>(() => checkIn.FuncionarioResponsavel = "");
                Assert.Throws<ArgumentException>(() => checkIn.FuncionarioResponsavel = "   ");
                Assert.Throws<ArgumentException>(() => checkIn.FuncionarioResponsavel = null!);
            });
        }

        #endregion

        #region Testes de Lógica e Métodos

        [Test]
        public void GerarChaveAcesso_DeveRetornarFormatoCorreto()
        {
            // Arrange
            int rId = 50;
            int cId = 5;
            DateTime data = new DateTime(2023, 12, 25);
            var checkIn = new CheckIn(cId, rId, data, "Admin", 2, "", true);
            string esperado = $"KEY-50-5-20231225";

            // Act
            string chave = checkIn.GerarChaveAcesso();

            // Assert
            Assert.That(chave, Is.EqualTo(esperado));
        }

        

        #endregion
    }
}