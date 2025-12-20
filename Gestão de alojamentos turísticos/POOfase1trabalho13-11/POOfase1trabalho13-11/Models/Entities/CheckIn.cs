using POOfase1trabalho13_11.Models.Entities.Interfaces;
using System.Text.Json.Serialization;

namespace POOfase1trabalho13_11.Models.Entities
{
    /// <summary>
    /// Representa o registo de um check-in de um cliente para uma reserva específica.
    /// </summary>
    public class CheckIn : IEntity
    {
        #region Private Fields
        private int _id;
        private int _numeroHospedes;
        private string _funcionarioResponsavel = string.Empty;
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

        public int ReservaId { get; set; }

        public DateTime DataHora { get; set; }

        public int NumeroHospedes
        {
            get => _numeroHospedes;
            set
            {
                if (value <= 0) throw new ArgumentException("Número de hóspedes deve ser maior que zero.");
                _numeroHospedes = value;
            }
        }

        public string Observacoes { get; set; } = string.Empty;

        public bool DocumentosVerificados { get; set; }

        public string FuncionarioResponsavel
        {
            get => _funcionarioResponsavel;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Funcionário responsável é obrigatório.");
                _funcionarioResponsavel = value;
            }
        }

        #endregion

        #region Constructors

        [JsonConstructor]
        public CheckIn(int id, int reservaId, DateTime dataHora, string funcionarioResponsavel, int numeroHospedes, string observacoes, bool documentosVerificados)
        {
            Id = id;
            ReservaId = reservaId;
            DataHora = dataHora == default ? DateTime.Now : dataHora;
            FuncionarioResponsavel = funcionarioResponsavel;
            NumeroHospedes = numeroHospedes;
            Observacoes = observacoes ?? string.Empty;
            DocumentosVerificados = documentosVerificados;
        }

        public CheckIn() { }

        #endregion

        #region Methods
        public string GerarChaveAcesso()
        {
            return $"KEY-{ReservaId}-{Id}-{DataHora:yyyyMMdd}";
        }

        
        #endregion
    }
}