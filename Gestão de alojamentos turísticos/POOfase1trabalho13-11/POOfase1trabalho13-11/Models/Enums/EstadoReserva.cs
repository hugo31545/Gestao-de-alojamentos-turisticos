//-----------------------------------------------------------------
//    <copyright file="EstadoReserva.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace POOfase1trabalho13_11.Models.Enums
{
    /// <summary>
    /// Define os diferentes estados de uma reserva ao longo do seu ciclo de vida.
    /// </summary>
    public enum EstadoReserva
    {
        /// <summary>
        /// A reserva foi criada pelo sistema ou cliente, mas ainda aguarda validação ou pagamento.
        /// </summary>
        Pendente,

        /// <summary>
        /// A reserva foi validada pelo gestor e o alojamento está garantido para o cliente.
        /// </summary>
        Confirmada,

        /// <summary>
        /// O cliente já se apresentou no alojamento e o processo de entrada foi concluído.
        /// </summary>
        CheckInRealizado,

        /// <summary>
        /// O cliente já libertou o alojamento e o processo de saída foi concluído.
        /// </summary>
        CheckOutRealizado,

        /// <summary>
        /// A reserva foi anulada por iniciativa do cliente ou do sistema.
        /// </summary>
        Cancelada,

        /// <summary>
        /// A reserva foi totalmente concluída, incluindo processos administrativos e faturação.
        /// </summary>
        Finalizada
    }
}
