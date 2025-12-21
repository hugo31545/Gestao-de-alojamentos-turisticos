//-----------------------------------------------------------------
//    <copyright file="IConsultaRepository.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.Models.Entities;

namespace POOfase1trabalho13_11.Models.Repositories.Interfaces
{
    /// <summary>
    /// Define o contrato para as operações de armazenamento e consulta do histórico de pesquisas.
    /// Herda as operações base da interface <see cref="IRepository{T}"/>.
    /// </summary>
    public interface IConsultaRepository : IRepository<Consulta>
    {

        /// <summary>
        /// Obtém o histórico de todas as consultas efetuadas por um utilizador específico.
        /// </summary>
        /// <param name="utilizador">O nome ou identificador do utilizador.</param>
        /// <returns>Uma coleção de objetos <see cref="Consulta"/> realizados pelo utilizador.</returns>
        IEnumerable<Consulta> GetByUtilizador(string utilizador);


        /// <summary>
        /// Filtra o histórico de pesquisas com base no tipo de entidade ou categoria consultada.
        /// </summary>
        /// <param name="tipo">O tipo de consulta (ex: "Alojamento", "Reserva").</param>
        /// <returns>Uma coleção de objetos <see cref="Consulta"/> que pertencem à categoria indicada.</returns>
        IEnumerable<Consulta> GetByTipo(string tipo);
    }
}