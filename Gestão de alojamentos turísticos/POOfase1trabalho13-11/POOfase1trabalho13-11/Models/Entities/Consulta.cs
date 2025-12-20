using POOfase1trabalho13_11.Models.Entities.Interfaces;
using System.Text.Json.Serialization;

namespace POOfase1trabalho13_11.Models.Entities
{
    /// <summary>
    /// Representa o histórico de uma consulta/pesquisa realizada no sistema.
    /// </summary>
    public class Consulta : IEntity
    {
        #region Private Fields
        private int _id;
        private string _tipo = "Geral";
        private string _utilizador = "Sistema";
        #endregion

        #region Public Properties

        public int Id
        {
            get => _id;
            set
            {
                if (value < 0) throw new ArgumentException("ID não pode ser negativo.");
                _id = value;
            }
        }

        public string Tipo
        {
            get => _tipo;
            set => _tipo = string.IsNullOrWhiteSpace(value) ? "Geral" : value;
        }

        public DateTime DataConsulta { get; set; }

        public string Utilizador
        {
            get => _utilizador;
            set => _utilizador = string.IsNullOrWhiteSpace(value) ? "Sistema" : value;
        }

        // Usamos tipos concretos para garantir que o Serializador JSON saiba o que fazer
        public Dictionary<string, string> Parametros { get; set; } = new();
        public List<string> Resultados { get; set; } = new();

        #endregion

        #region Constructors

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

        public Consulta() { }

        #endregion

        
    }
}