using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace POOfase1trabalho13_11.Models.Enums
{
    
    public enum EstadoReserva
    {
        Pendente,           // Criada, mas não confirmada
        Confirmada,         // Confirmada pelo gestor
        CheckInRealizado,   // Check-in já efetuado
        CheckOutRealizado,  // Check-out efetuado
        Cancelada ,          // Reserva cancelada
       Finalizada
    }
}
