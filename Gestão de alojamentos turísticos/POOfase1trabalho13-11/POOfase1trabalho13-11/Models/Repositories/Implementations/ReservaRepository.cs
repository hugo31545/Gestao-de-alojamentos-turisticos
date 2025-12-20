using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Enums;
using POOfase1trabalho13_11.Models.Repositories.Interfaces;

namespace POOfase1trabalho13_11.Models.Repositories.Implementations
{
    public class ReservaRepository : JsonRepository<Reserva>, IReservaRepository
    {
        public ReservaRepository() : base("reservas.json") { }

        public IEnumerable<Reserva> FiltrarPorCliente(int clienteId) =>
            Items.Where(r => r.ClienteId == clienteId);

        public IEnumerable<Reserva> FiltrarPorAlojamento(int alojamentoId) =>
            Items.Where(r => r.AlojamentoId == alojamentoId);

        public IEnumerable<Reserva> BuscarAtivas() =>
            Items.Where(r => r.Estado == EstadoReserva.Confirmada || r.Estado == EstadoReserva.CheckInRealizado);

        public IEnumerable<Reserva> BuscarPendentes() =>
            Items.Where(r => r.Estado == EstadoReserva.Pendente);

        // Auto-incremento de ID
        public override void Add(Reserva reserva)
        {
            if (reserva.Id <= 0)
                reserva.Id = Items.Any() ? Items.Max(r => r.Id) + 1 : 1;
            base.Add(reserva);
        }
    }
}