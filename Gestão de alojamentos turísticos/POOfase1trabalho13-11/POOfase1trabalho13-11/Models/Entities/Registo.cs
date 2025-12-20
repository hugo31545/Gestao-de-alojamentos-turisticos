using POOfase1trabalho13_11.Models.Entities.Interfaces;
using System.Text.Json.Serialization;

namespace POOfase1trabalho13_11.Models.Entities
{
    /// <summary>
    /// Representa um log de auditoria do sistema para rastrear operações realizadas.
    /// Esta classe é imutável após a criação para garantir a integridade do histórico.
    /// </summary>
    public class Registo : IEntity
    {
        #region Private Fields
        private int _id;
        private string _tipoEntidade = string.Empty;
        private string _detalhes = string.Empty;
        private string _utilizador = string.Empty;
        private string _operacao = string.Empty;
        #endregion

        #region Public Properties

        public int Id
        {
            get => _id;
            set
            {
                if (value < 0) throw new ArgumentException("ID inválido.");
                _id = value;
            }
        }

        public string TipoEntidade
        {
            get => _tipoEntidade;
            set => _tipoEntidade = string.IsNullOrWhiteSpace(value) ? "Desconhecido" : value;
        }

        public string Detalhes
        {
            get => _detalhes;
            set => _detalhes = value ?? string.Empty;
        }

        public string Utilizador
        {
            get => _utilizador;
            set => _utilizador = string.IsNullOrWhiteSpace(value) ? "Sistema" : value;
        }

        public string Operacao
        {
            get => _operacao;
            set => _operacao = value ?? "LOG";
        }

        public int EntidadeId { get; set; }

        public DateTime DataRegisto { get; set; }

        #endregion

        #region Constructors

        [JsonConstructor]
        public Registo(int id, string tipoEntidade, string detalhes, string utilizador, string operacao, int entidadeId, DateTime dataRegisto)
        {
            Id = id;
            TipoEntidade = tipoEntidade;
            Detalhes = detalhes;
            Utilizador = utilizador;
            Operacao = operacao;
            EntidadeId = entidadeId;
            DataRegisto = dataRegisto == default ? DateTime.Now : dataRegisto;
        }
        public Registo()
        {
            DataRegisto = DateTime.Now;
        }
        #endregion

    }
}