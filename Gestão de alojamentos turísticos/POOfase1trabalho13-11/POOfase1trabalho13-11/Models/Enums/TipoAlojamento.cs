//-----------------------------------------------------------------
//    <copyright file="TipoAlojamento.cs" company="IPCA">
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
    /// Define as categorias ou tipologias de alojamento disponíveis no sistema.
    /// </summary>
    public enum TipoAlojamento
    {
        /// <summary>
        /// Representa uma unidade hoteleira tradicional com serviços integrados.
        /// </summary>
        Hotel,

        /// <summary>
        /// Unidade habitacional independente e mobilada, integrada num edifício.
        /// </summary>
        Apartamento,

        /// <summary>
        /// Acomodação em quarto individual dentro de uma habitação partilhada.
        /// </summary>
        QuartoPrivado,

        /// <summary>
        /// Habitação completa destinada exclusivamente a férias.
        /// </summary>
        CasaFerias
    }
}
