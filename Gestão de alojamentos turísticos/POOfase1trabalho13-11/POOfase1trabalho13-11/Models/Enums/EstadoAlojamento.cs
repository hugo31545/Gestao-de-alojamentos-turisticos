//-----------------------------------------------------------------
//    <copyright file="EstadoAlojamento.cs" company="IPCA">
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
    /// Define os estados possíveis em que um alojamento se pode encontrar no sistema.
    /// </summary>
    public enum EstadoAlojamento

    {
        /// <summary>
        /// O alojamento está livre e pode ser selecionado para novas reservas.
        /// </summary>
        Disponivel,

        /// <summary>
        /// O alojamento já possui uma reserva em andamento ou hóspedes presentes.
        /// </summary>s
        Ocupado,

        /// <summary>
        /// O alojamento está em processo de limpeza ou reparação, não podendo ser ocupado.
        /// </summary>
        Manutencao,

        /// <summary>
        /// O alojamento está desativado ou impossibilitado de ser reservado por tempo indeterminado.
        /// </summary>
        Indisponivel
    }
}
