using NUnit.Framework;
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Enums;
using System;
using System.Collections.Generic;

namespace POOfase1trabalho13_11.Tests.Entities
{
    [TestFixture]
    public class AlojamentoTests
    {
        #region Testes de Construtor e Inicialização (AAA)

        [Test]
        public void ConstrutorComParametros_DeveAtribuirPropriedadesCorretamente()
        {
            // Arrange
            int id = 1;
            string nome = "Villa Mar";
            string local = "Algarve";
            decimal preco = 120.50m;
            int cap = 4;
            string desc = "Vista mar";
            var tipo = TipoAlojamento.Apartamento;
            var comodidades = new List<string> { "Wi-Fi", "Piscina" };
            var estado = EstadoAlojamento.Disponivel;

            // Act
            var alojamento = new Alojamento(id, nome, local, preco, cap, desc, tipo, comodidades, estado);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(alojamento.Id, Is.EqualTo(id));
                Assert.That(alojamento.Nome, Is.EqualTo(nome));
                Assert.That(alojamento.PrecoPorNoite, Is.EqualTo(preco));
                Assert.That(alojamento.Comodidades.Count, Is.EqualTo(2));
                Assert.That(alojamento.Tipo, Is.EqualTo(tipo));
            });
        }

        [Test]
        public void ConstrutorPadrao_DeveInicializarComEstadoDisponivelEColecoesVazias()
        {
            // Act
            var alojamento = new Alojamento();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(alojamento.Estado, Is.EqualTo(EstadoAlojamento.Disponivel));
                Assert.That(alojamento.Comodidades, Is.Not.Null);
                Assert.That(alojamento.Comodidades.Count, Is.EqualTo(0));
            });
        }

        #endregion

        #region Testes de Proteção contra Nulos e Sanitize

        [Test]
        public void SetComodidades_SeNulo_DeveInicializarListaVazia()
        {
            // Arrange
            var alojamento = new Alojamento();

            // Act
            alojamento.Comodidades = null!;

            // Assert
            Assert.That(alojamento.Comodidades, Is.Not.Null);
            Assert.That(alojamento.Comodidades.Count, Is.EqualTo(0));
        }

        [Test]
        public void SetNomeELocalizacao_SeNulos_DeveConverterParaStringVazia()
        {
            // Arrange
            var alojamento = new Alojamento();

            // Act
            alojamento.Nome = null!;
            alojamento.Localizacao = null!;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(alojamento.Nome, Is.EqualTo(string.Empty));
                Assert.That(alojamento.Localizacao, Is.EqualTo(string.Empty));
            });
        }

        #endregion

        #region Testes de Validação (Exceções)

        [Test]
        public void SetId_Negativo_DeveLancarArgumentException()
        {
            // Arrange
            var alojamento = new Alojamento();

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => alojamento.Id = -1);
            Assert.That(ex.Message, Does.Contain("ID nao pode ser negativo."));
        }

        #endregion

        #region Testes de Comportamento de Coleção

        [Test]
        public void Comodidades_DeveSerUmaNovaInstancia_ParaEvitarModificacaoExterna()
        {
            // Arrange
            var listaOriginal = new List<string> { "TV" };
            var alojamento = new Alojamento();
            alojamento.Comodidades = listaOriginal;

            // Act
            listaOriginal.Add("AC"); 

            // Assert
            //  'set' faz 'new List<string>(value)', a lista interna deve manter apenas 1 item
            Assert.That(alojamento.Comodidades.Count, Is.EqualTo(1));
        }

        #region Testes de Robustez e Valores Limite

        [Test]
        public void PrecoPorNoite_DeveAceitarValoresExtremos()
        {
            // Arrange
            var alojamento = new Alojamento();
            decimal precoCaro = 999999.99m;
            decimal precoGratis = 0m;

            // Act
            alojamento.PrecoPorNoite = precoCaro;
            decimal resultadoCaro = alojamento.PrecoPorNoite;

            alojamento.PrecoPorNoite = precoGratis;
            decimal resultadoGratis = alojamento.PrecoPorNoite;

            // Assert
            Assert.That(resultadoCaro, Is.EqualTo(precoCaro));
            Assert.That(resultadoGratis, Is.EqualTo(0m));
        }

        [Test]
        public void Capacidade_DeveAceitarValoresGrandes()
        {
            // Arrange
            var alojamento = new Alojamento();
            int capacidadeGrande = 500; 

            // Act
            alojamento.Capacidade = capacidadeGrande;

            // Assert
            Assert.That(alojamento.Capacidade, Is.EqualTo(capacidadeGrande));
        }

        [Test]
        public void Descricao_DeveSuportarTextoMuitoLongo()
        {
            // Arrange
            var alojamento = new Alojamento();
            string textoLongo = new string('X', 5000); 

            // Act
            alojamento.Descricao = textoLongo;

            // Assert
            Assert.That(alojamento.Descricao.Length, Is.EqualTo(5000));
        }

        [Test]
        public void Comodidades_NaoDevePermitirAdicionarDiretamente_PoisEIReadOnlyList()
        {
            // Arrange
            var alojamento = new Alojamento();
            alojamento.Comodidades = new List<string> { "Wi-Fi" };

            // Act & Assert
            

            Assert.That(alojamento.Comodidades, Is.InstanceOf<IReadOnlyList<string>>());
        }

        [Test]
        public void MudancaDeEstado_DeveSerRefletidaCorretamente()
        {
            // Arrange
            var alojamento = new Alojamento();
            alojamento.Estado = EstadoAlojamento.Disponivel;

            // Act
            alojamento.Estado = EstadoAlojamento.Manutencao;

            // Assert
            Assert.That(alojamento.Estado, Is.EqualTo(EstadoAlojamento.Manutencao));
        }

        #endregion
        #endregion
    }
}