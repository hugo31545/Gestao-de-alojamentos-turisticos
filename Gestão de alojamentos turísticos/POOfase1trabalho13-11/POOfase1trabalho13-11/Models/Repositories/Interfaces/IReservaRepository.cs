//-----------------------------------------------------------------
//    <copyright file="IReservaRepository.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.Models.Entities;

namespace POOfase1trabalho13_11.Models.Repositories.Interfaces
{

    /// <summary>
    /// Define o contrato para operações de acesso a dados e consultas especializadas de <see cref="Reserva"/>.
    /// Estende a interface genérica <see cref="IRepository{T}"/>.
    /// </summary>
    public interface IReservaRepository : IRepository<Reserva>
    {
        #region Query Methods
        /// <summary>
        /// Obtém todas as reservas efetuadas por um cliente específico.
        /// </summary>
        /// <param name="clienteId">O identificador único do cliente.</param>
        /// <returns>Uma coleção de reservas associadas ao cliente.</returns>
        IEnumerable<Reserva> FiltrarPorCliente(int clienteId);

        /// <summary>
        /// Obtém o histórico de reservas de um alojamento específico.
        /// </summary>
        /// <param name="alojamentoId">O identificador único do alojamento.</param>
        /// <returns>Uma coleção de reservas associadas ao alojamento.</returns>
        IEnumerable<Reserva> FiltrarPorAlojamento(int alojamentoId);

        /// <summary>
        /// Retorna as reservas que estão atualmente em vigor (Confirmadas ou com Check-in realizado).
        /// </summary>
        /// <returns>Uma coleção de reservas ativas.</returns>
        IEnumerable<Reserva> BuscarAtivas();


        /// <summary>
        /// Retorna as reservas que aguardam aprovação ou processamento inicial.
        /// </summary>
        /// <returns>Uma coleção de reservas com estado pendente.</returns>
        IEnumerable<Reserva> BuscarPendentes();
        #endregion
    }
}