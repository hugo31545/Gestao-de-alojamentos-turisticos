using NUnit.Framework;
using POOfase1trabalho13_11.Models.Entities;
using System;
using System.Collections.Generic;

namespace POOfase1trabalho13_11.Tests.Entities
{
    [TestFixture]
    public class ConsultaTests
    {
        #region Testes de Construtor e Inicialização (AAA)

        [Test]
        public void Construtor_DeveInicializarColecoesSePassadoNulo()
        {
            // Arrange
            // Act
            var consulta = new Consulta(1, "Busca", "Admin", DateTime.Now, null!, null!);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(consulta.Parametros, Is.Not.Null, "Dicionário de Parâmetros não deve ser nulo.");
                Assert.That(consulta.Resultados, Is.Not.Null, "Lista de Resultados não deve ser nula.");
            });
        }

        [Test]
        public void Construtor_DeveAtribuirDataAtual_QuandoDataForDefault()
        {
            // Arrange
            DateTime antes = DateTime.Now.AddSeconds(-1);

            // Act
            var consulta = new Consulta(1, "Geral", "User", default, new(), new());

            // Assert
            Assert.That(consulta.DataConsulta, Is.GreaterThanOrEqualTo(antes));
        }

        #endregion

        #region Testes de Validação e Regras de Negócio (Strong Tests)

        [Test]
        public void SetId_Negativo_DeveLancarArgumentException()
        {
            // Arrange
            var consulta = new Consulta();

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => consulta.Id = -10);
            Assert.That(ex.Message, Is.EqualTo("ID não pode ser negativo."));
        }

        [Test]
        public void SetTipo_NuloOuVazio_DeveRetornarValorPadraoGeral()
        {
            // Arrange
            var consulta = new Consulta { Tipo = "Personalizado" };

            // Act
            consulta.Tipo = "";
            string resultadoVazio = consulta.Tipo;

            consulta.Tipo = null!;
            string resultadoNulo = consulta.Tipo;

            // Assert
            Assert.That(resultadoVazio, Is.EqualTo("Geral"));
            Assert.That(resultadoNulo, Is.EqualTo("Geral"));
        }

        [Test]
        public void SetUtilizador_EspacosEmBranco_DeveRetornarSistema()
        {
            // Arrange
            var consulta = new Consulta();

            // Act
            consulta.Utilizador = "     ";

            // Assert
            Assert.That(consulta.Utilizador, Is.EqualTo("Sistema"));
        }

        #endregion

        #region Testes de Manipulação de Dados (Coleções)

        [Test]
        public void Parametros_DeveArmazenarERecuperarValoresCorretamente()
        {
            // Arrange
            var consulta = new Consulta();
            var dict = new Dictionary<string, string> { { "DataInicio", "2023-01-01" }, { "PrecoMax", "500" } };

            // Act
            consulta.Parametros = dict;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(consulta.Parametros.Count, Is.EqualTo(2));
                Assert.That(consulta.Parametros["PrecoMax"], Is.EqualTo("500"));
            });
        }

        [Test]
        public void Resultados_DeveSuportarGrandeVolumeDeDados()
        {
            // Arrange
            var consulta = new Consulta();
            var listaResultados = new List<string>();
            for (int i = 0; i < 1000; i++) listaResultados.Add($"Resultado_{i}");

            // Act
            consulta.Resultados = listaResultados;

            // Assert
            Assert.That(consulta.Resultados.Count, Is.EqualTo(1000));
            Assert.That(consulta.Resultados[999], Is.EqualTo("Resultado_999"));
        }

        #endregion

        #region Testes de Stress e Edge Cases

        [Test]
        public void Consulta_DeveSerIndependente_AoModificarDicionarioExterno()
        {
            // Arrange
            var meusParametros = new Dictionary<string, string> { { "Filtro", "Ativo" } };
            var consulta = new Consulta(1, "T", "U", DateTime.Now, meusParametros, new());

            // Act
            meusParametros.Add("Hack", "Alterado");

           
            Assert.That(consulta.Parametros.Count, Is.EqualTo(2));
        }

        #endregion
    }
}