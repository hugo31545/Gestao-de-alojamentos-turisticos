using POOfase1trabalho13_11.Models.Entities;

namespace POOfase1trabalho13_11.Models.Services.Interfaces
{
    public interface IReservaService
    {
        bool RealizarReserva(int clienteId, int alojamentoId, DateTime checkIn, DateTime checkOut, int numHospedes, out string mensagem);
        bool CancelarReserva(int reservaId, string motivo, out string mensagem);
        bool EfetuarCheckIn(int reservaId, out string mensagem);
        bool EfetuarCheckOut(int reservaId, out string mensagem);
        IEnumerable<Reserva> ObterTodasReservas();
    }
}