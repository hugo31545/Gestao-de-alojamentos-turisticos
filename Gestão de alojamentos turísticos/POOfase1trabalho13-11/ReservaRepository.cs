using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Enums;
using POOfase1trabalho13_11.Models.Repositories.Interfaces;
using POOfase1trabalho13_11.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace POOfase1trabalho13_11.Repositories.Implementations
{
    public class ReservaRepository : JsonRepository<Reserva>, IReservaRepository
    {
        public ReservaRepository(string filePath) : base(filePath) { }

        public Reserva? GetById(int id)
        {
            return ObterPorId(id);
        }

        public IEnumerable<Reserva> GetAll()
        {
            return ObterTodos();
        }

        public IEnumerable<Reserva> FiltrarPorCliente(int clienteId)
        {
            return ObterTodos().Where(r => r.Cliente.Id == clienteId);
        }

        public IEnumerable<Reserva> FiltrarPorEstado(EstadoReserva estado)
        {
            return ObterTodos().Where(r => r.Estado == estado);
        }

        public void Add(Reserva reserva)
        {
            Adicionar(reserva);
        }

        public bool Remove(Reserva reserva)
        {
            if (reserva == null) return false;
            Remover(reserva);
            return true;
        }
    }
}