//-----------------------------------------------------------------
//    <copyright file="Consulta.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.Models.Entities.Interfaces;
using System.Text.Json.Serialization;

namespace POOfase1trabalho13_11.Models.Entities
{
    /// <summary>
    /// Representa o histórico de uma consulta ou pesquisa realizada no sistema.
    /// Armazena os parâmetros utilizados e os resultados obtidos para fins de auditoria.
    /// Implementa a interface <see cref="IEntity"/>.
    /// </summary>
    public class Consulta : IEntity
    {
        #region Private Fields
        private int _id;
        private string _tipo = "Geral";
        private string _utilizador = "Sistema";
        #endregion

        #region Public Properties

        /// <summary>
        /// Obtém ou define o identificador único da consulta.
        /// </summary>
        /// <exception cref="ArgumentException">Lançada se o ID for negativo.</exception>
        public int Id
        {
            get => _id;
            set
            {
                if (value < 0) throw new ArgumentException("ID não pode ser negativo.");
                _id = value;
            }
        }

        /// <summary>
        /// Obtém ou define o tipo de consulta realizada.
        /// </summary>
        public string Tipo
        {
            get => _tipo;
            set => _tipo = string.IsNullOrWhiteSpace(value) ? "Geral" : value;
        }

        /// <summary>
        /// Obtém ou define a data e hora em que a consulta foi efetuada.
        /// </summary>
        public DateTime DataConsulta { get; set; }

        /// <summary>
        /// Obtém ou define o nome do utilizador ou sistema que executou a consulta.
        /// </summary>
        public string Utilizador
        {
            get => _utilizador;
            set => _utilizador = string.IsNullOrWhiteSpace(value) ? "Sistema" : value;
        }

        /// <summary>
        /// Obtém ou define o dicionário de parâmetros aplicados na pesquisa.
        /// </summary>
        public Dictionary<string, string> Parametros { get; set; } = new();

        /// <summary>
        /// Obtém ou define a lista de resultados encontrados durante a consulta.
        /// </summary>
        public List<string> Resultados { get; set; } = new();

        /// <summary>
        /// Parâmetros em texto (ex.: "Inicio=01/10/2026, Fim=12/10/2026"), para apresentação.
        /// </summary>
        [JsonIgnore]
        public string ResumoParametros => string.Join(", ", Parametros.Select(p => $"{p.Key}={p.Value}"));

        /// <summary>
        /// Resultados em texto, para apresentação.
        /// </summary>
        [JsonIgnore]
        public string ResumoResultados => string.Join("; ", Resultados);

        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Consulta"/> com parâmetros detalhados.
        /// Utilizado principalmente para a desserialização de dados históricos.
        /// </summary>
        /// <param name="id">ID da consulta.</param>
        /// <param name="tipo">Categoria da pesquisa.</param>
        /// <param name="utilizador">Quem realizou a operação.</param>
        /// <param name="dataConsulta">Data do registo.</param>
        /// <param name="parametros">Filtros utilizados.</param>
        /// <param name="resultados">Itens encontrados.</param>
        [JsonConstructor]
        public Consulta(int id, string tipo, string utilizador, DateTime dataConsulta, Dictionary<string, string> parametros, List<string> resultados)
        {
            Id = id;
            Tipo = tipo;
            Utilizador = utilizador;
            DataConsulta = dataConsulta == default ? DateTime.Now : dataConsulta;
            Parametros = parametros ?? new Dictionary<string, string>();
            Resultados = resultados ?? new List<string>();
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Consulta"/> com valores padrão.
        /// </summary>
        public Consulta() { }

        #endregion

        
    }
}