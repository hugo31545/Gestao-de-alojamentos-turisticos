using System.Collections.Generic;
using POOfase1trabalho13_11.Models.Entities;

namespace POOfase1trabalho13_11.Models.Services.Interfaces
{
    public interface ICheckInService
    {
        // Tenta realizar o check-in de uma reserva existente
        bool RealizarCheckIn(int reservaId, string funcionarioResponsavel, out string mensagem);

        // Finaliza a estadia (Check-out)
        bool RealizarCheckOut(int reservaId, string funcionarioResponsavel, out string mensagem);

        // Obtém todos os check-ins ativos (estadias atuais)
        IEnumerable<Reserva> ObterEstadiasAtuais();
    }
}