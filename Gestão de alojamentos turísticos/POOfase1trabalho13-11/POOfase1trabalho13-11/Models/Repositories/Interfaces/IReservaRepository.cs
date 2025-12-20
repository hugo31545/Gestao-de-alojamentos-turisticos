using POOfase1trabalho13_11.Models.Entities;

namespace POOfase1trabalho13_11.Models.Repositories.Interfaces
{
    public interface IReservaRepository : IRepository<Reserva>
    {
        IEnumerable<Reserva> FiltrarPorCliente(int clienteId);
        IEnumerable<Reserva> FiltrarPorAlojamento(int alojamentoId);
        IEnumerable<Reserva> BuscarAtivas();
        IEnumerable<Reserva> BuscarPendentes();
    }
}