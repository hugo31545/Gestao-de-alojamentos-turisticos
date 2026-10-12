//-----------------------------------------------------------------
//    <copyright file="ConsultaRepository.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace POOfase1trabalho13_11.Models.Repositories.Implementations
{

    /// <summary>
    /// Repositório especializado na gestão do histórico de objetos do tipo <see cref="Consulta"/>.
    /// Armazena as pesquisas e consultas efetuadas no sistema para fins de histórico e auditoria.
    /// </summary>
    public class ConsultaRepository : JsonRepository<Consulta>, IConsultaRepository
    {

        /// <summary>
        /// Inicializa uma nova instância de <see cref="ConsultaRepository"/>,
        /// configurando o ficheiro "historico_consultas.json" para armazenamento persistente.
        /// </summary>
        public ConsultaRepository() : base("historico_consultas.json") { }


        /// <summary>
        /// Obtém todas as consultas realizadas por um determinado utilizador.
        /// </summary>
        /// <param name="utilizador">O nome do utilizador a pesquisar.</param>
        /// <returns>Uma coleção de consultas associadas ao utilizador indicado.</returns>
        public IEnumerable<Consulta> GetByUtilizador(string utilizador) =>
            Items.Where(c => c.Utilizador.Equals(utilizador, StringComparison.OrdinalIgnoreCase));


        /// <summary>
        /// Filtra o histórico de consultas por uma categoria ou tipo específico.
        /// </summary>
        /// <param name="tipo">O tipo de consulta (ex: "Alojamento", "Reserva").</param>
        /// <returns>Uma coleção de consultas do tipo especificado.</returns>
        public IEnumerable<Consulta> GetByTipo(string tipo) =>
            Items.Where(c => c.Tipo.Equals(tipo, StringComparison.OrdinalIgnoreCase));

    }
}