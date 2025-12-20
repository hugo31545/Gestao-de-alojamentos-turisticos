using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace POOfase1trabalho13_11.Models.Enums
{
    
    public enum EstadoAlojamento

    {
        Disponivel,    // O alojamento está livre para reserva
        Ocupado,        // Já possui reserva em andamento
        Manutencao,     // Está em manutenção
        Indisponivel    // Não pode ser reservado
    }
}
