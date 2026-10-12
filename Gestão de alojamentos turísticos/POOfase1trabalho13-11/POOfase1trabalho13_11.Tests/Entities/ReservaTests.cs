using NUnit.Framework;
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Enums;
using System;

namespace POOfase1trabalho13_11.Tests.Entities
{
    [TestFixture]
    public class ReservaTests
    {
        [Test]
        public void ConstrutorPadrao_DeveInicializarComEstadoPendenteEDataAtual()
        {
            // Arrange
            DateTime antesDaCriacao = DateTime.Now.AddSeconds(-1);

            // Act
            var reserva = new Reserva();

            // Assert
            Assert.That(reserva.Estado, Is.EqualTo(EstadoReserva.Pendente));
            Assert.That(reserva.DataReserva, Is.GreaterThanOrEqualTo(antesDaCriacao));
            Assert.That(reserva.Observacoes, Is.EqualTo(string.Empty));
        }

        [Test]
        public void ConstrutorComParametros_DeveAtribuirPropriedadesCorretamente()
        {
            // Arrange
            int idEsperado = 1;
            int clienteIdEsperado = 10;
            int alojamentoIdEsperado = 50;
            DateTime checkIn = DateTime.Today.AddDays(1);
            DateTime checkOut = DateTime.Today.AddDays(3);
            int hospedes = 2;
            string obs = "Reserva VIP";
            EstadoReserva estado = EstadoReserva.Confirmada;
            DateTime dataCriacao = new DateTime(2023, 11, 13);

            // Act
            var reserva = new Reserva(idEsperado, clienteIdEsperado, alojamentoIdEsperado,
                                      checkIn, checkOut, hospedes, obs, estado, dataCriacao);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(reserva.Id, Is.EqualTo(idEsperado));
                Assert.That(reserva.ClienteId, Is.EqualTo(clienteIdEsperado));
                Assert.That(reserva.AlojamentoId, Is.EqualTo(alojamentoIdEsperado));
                Assert.That(reserva.DataCheckIn, Is.EqualTo(checkIn));
                Assert.That(reserva.DataCheckOut, Is.EqualTo(checkOut));
                Assert.That(reserva.NumeroHospedes, Is.EqualTo(hospedes));
                Assert.That(reserva.Observacoes, Is.EqualTo(obs));
                Assert.That(reserva.Estado, Is.EqualTo(estado));
                Assert.That(reserva.DataReserva, Is.EqualTo(dataCriacao));
            });
        }

        [Test]
        public void CicloDeVida_PendenteAteFinalizada()
        {
            var reserva = new Reserva();

            reserva.Confirmar();
            reserva.RegistarCheckIn();
            Assert.IsTrue(reserva.EstaAtiva);

            reserva.RegistarCheckOut();
            Assert.That(reserva.Estado, Is.EqualTo(EstadoReserva.Finalizada));
            Assert.IsTrue(reserva.EstaConcluida);
            Assert.IsFalse(reserva.EstaAtiva);
        }

        [Test]
        public void TransicoesInvalidas_LancamInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => new Reserva { Estado = EstadoReserva.Pendente }.RegistarCheckIn());
            Assert.Throws<InvalidOperationException>(() => new Reserva { Estado = EstadoReserva.Confirmada }.RegistarCheckOut());
            Assert.Throws<InvalidOperationException>(() => new Reserva { Estado = EstadoReserva.Cancelada }.Confirmar());
            Assert.Throws<InvalidOperationException>(() => new Reserva { Estado = EstadoReserva.CheckInRealizado }.Cancelar());
        }

        [Test]
        public void Confirmar_DeveAlterarEstadoParaConfirmada()
        {
            // Arrange
            var reserva = new Reserva();
            reserva.Estado = EstadoReserva.Pendente;

            // Act
            reserva.Confirmar();

            // Assert
            Assert.That(reserva.Estado, Is.EqualTo(EstadoReserva.Confirmada));
        }

        [Test]
        public void Cancelar_DeveAlterarEstadoParaCancelada()
        {
            // Arrange
            var reserva = new Reserva();
            reserva.Estado = EstadoReserva.Confirmada;

            // Act
            reserva.Cancelar();

            // Assert
            Assert.That(reserva.Estado, Is.EqualTo(EstadoReserva.Cancelada));
        }

        [Test]
        public void ToString_DeveRetornarFormatacaoCorreta()
        {
            // Arrange
            var reserva = new Reserva(1, 10, 20, DateTime.Now, DateTime.Now, 2, "", EstadoReserva.Pendente, DateTime.Now);
            string esperado = "Reserva 1 - Cliente: 10, Alojamento: 20, Estado: Pendente";

            // Act
            string resultado = reserva.ToString();

            // Assert
            Assert.That(resultado, Is.EqualTo(esperado));


        }

        [Test]
        public void NumeroHospedes_DeveAceitarValoresPositivos()
        {
            // Arrange
            var reserva = new Reserva();
            int valorEsperado = 5;

            // Act
            reserva.NumeroHospedes = valorEsperado;

            // Assert
            Assert.That(reserva.NumeroHospedes, Is.EqualTo(valorEsperado));
        }

        [Test]
        public void Reserva_DataCheckOut_DeveSerPosteriorAoCheckIn()
        {
            // Arrange
            var checkIn = DateTime.Now.AddDays(2);
            var checkOut = DateTime.Now.AddDays(1); // Data inválida (antes do check-in)

            // Act
            var reserva = new Reserva { DataCheckIn = checkIn, DataCheckOut = checkOut };

            // Assert
            
            Assert.That(reserva.DataCheckOut, Is.LessThan(reserva.DataCheckIn));
        }

        [Test]
        public void Construtor_SeObservacoesForNulo_DeveConverterParaStringVazia()
        {
            // Act
            var reserva = new Reserva(1, 1, 1, DateTime.Now, DateTime.Now, 1, null!, EstadoReserva.Pendente, DateTime.Now);

            // Assert
            Assert.That(reserva.Observacoes, Is.Not.Null);
            Assert.That(reserva.Observacoes, Is.EqualTo(string.Empty));
        }

        [Test]
        public void Reserva_DeveManterDadosAposAtribuicaoDeId()
        {
            // Arrange
            var reserva = new Reserva();

            // Act
            reserva.Id = 500;
            reserva.ClienteId = 10;

            // Assert
            Assert.That(reserva.Id, Is.EqualTo(500));
            Assert.That(reserva.ClienteId, Is.EqualTo(10));
        }
    }
}