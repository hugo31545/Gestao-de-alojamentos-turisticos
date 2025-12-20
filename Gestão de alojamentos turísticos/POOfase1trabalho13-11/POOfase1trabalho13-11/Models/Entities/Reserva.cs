using POOfase1trabalho13_11.Models.Entities.Interfaces;
using POOfase1trabalho13_11.Models.Enums;
using System.Text.Json.Serialization;

namespace POOfase1trabalho13_11.Models.Entities
{
    public class Reserva : IEntity
    {
        #region Private Fields
        private int _id;
        private int _numeroHospedes;
        private EstadoReserva _estado;
        #endregion

        #region Public Properties
        public int Id
        {
            get => _id;
            set => _id = value; 
        }

        
        public int ClienteId { get; set; }
        public int AlojamentoId { get; set; }

        public DateTime DataReserva { get; set; } = DateTime.Now;
        public DateTime DataCheckIn { get; set; }
        public DateTime DataCheckOut { get; set; }
        public int NumeroHospedes
        {
            get => _numeroHospedes;
            set => _numeroHospedes = value; 
        }

        public string Observacoes { get; set; } = string.Empty;

        public EstadoReserva Estado
        {
            get => _estado;
            set => _estado = value;
        }
        #endregion

        #region Constructors
        [JsonConstructor]
        public Reserva(int id, int clienteId, int alojamentoId, DateTime dataCheckIn, DateTime dataCheckOut, int numeroHospedes, string observacoes, EstadoReserva estado, DateTime dataReserva)
        {
            
            this.Id = id;
            this.ClienteId = clienteId;
            this.AlojamentoId = alojamentoId;
            this.DataCheckIn = dataCheckIn;
            this.DataCheckOut = dataCheckOut;
            this.NumeroHospedes = numeroHospedes;
            this.Observacoes = observacoes ?? string.Empty;
            this.Estado = estado;
            this.DataReserva = dataReserva == default ? DateTime.Now : dataReserva;
        }

        public Reserva()
        {
            DataReserva = DateTime.Now;
            Estado = EstadoReserva.Pendente;
        }
        #endregion

        #region Logic Methods
        public void Confirmar() => Estado = EstadoReserva.Confirmada;
        public void Cancelar() => Estado = EstadoReserva.Cancelada;
        public override string ToString() => $"Reserva {Id} - Cliente: {ClienteId}, Alojamento: {AlojamentoId}, Estado: {Estado}";
        #endregion
    }
}