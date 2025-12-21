//-----------------------------------------------------------------
//    <copyright file="ViewType.cs" company="IPCA">
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

namespace POOfase1trabalho13_11.Views.Enums
{
    
        
    
        /// <summary>
        /// Especifica os tipos de vistas (Views) disponíveis na aplicação para navegação.
        /// </summary>
        public enum ViewType
        {
           

            /// <summary>
            /// Janela LOGIN
            /// </summary>
            Login,
            

            
            /// <summary>
            /// Janela principal
            /// </summary>
            Main,
           

            
            /// <summary>
            /// Gestão de alojamentos.
            /// </summary>
            Alojamentos,

            /// <summary>
            /// Gestão de clientes/hóspedes.
            /// </summary>
            Clientes,

            /// <summary>
            /// Gestão de reservas e estadias.
            /// </summary>
            Reservas,

            /// <summary>
            /// Visualização do histórico de auditoria (Logs).
            /// </summary>
            Auditoria,
            
            /// <summary>
            /// Registo de novos funcionários.
            /// </summary>
            Registry

            
        }
}