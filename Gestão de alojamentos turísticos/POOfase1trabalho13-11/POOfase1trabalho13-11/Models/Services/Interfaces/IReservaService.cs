//-----------------------------------------------------------------
//    <copyright file="IReservaService.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>1</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.Models.Entities;

namespace POOfase1trabalho13_11.Models.Services.Interfaces
{

    /// <summary>
    /// Define o contrato para os serviços de gestão de reservas.
    /// Coordena a disponibilidade de alojamentos, validações de datas e o ciclo de vida da estadia.
    /// </summary>
    public interface IReservaService
    {
        #region Core Operations
        /// <summary>
        /// Cria uma nova reserva no sistema após validar a disponibilidade do alojamento e dados do cliente.
        /// </summary>
        /// <param name="clienteId">ID do cliente que efetua a reserva.</param>
        /// <param name="alojamentoId">ID do alojamento a reservar.</param>
        /// <param name="checkIn">Data prevista de entrada.</param>
        /// <param name="checkOut">Data prevista de saída.</param>
        /// <param name="numHospedes">Número de pessoas para a estadia.</param>
        /// <param name="mensagem">Mensagem de feedback com o resultado detalhado da operação.</param>
        /// <returns>True se a reserva foi criada com sucesso; False caso contrário.</returns>
        bool RealizarReserva(int clienteId, int alojamentoId, DateTime checkIn, DateTime checkOut, int numHospedes, out string mensagem);

        /// <summary>
        /// Cancela uma reserva existente por um motivo específico.
        /// </summary>
        /// <param name="reservaId">Identificador único da reserva.</param>
        /// <param name="motivo">Justificação para o cancelamento.</param>
        /// <param name="mensagem">Feedback sobre o sucesso do cancelamento.</param>
        /// <returns>True se o cancelamento foi processado com sucesso.</returns>
        bool CancelarReserva(int reservaId, string motivo, out string mensagem);

        #endregion
        #region Operational Flow
        /// <summary>
        /// Confirma uma reserva pendente. O check-in e o check-out são feitos pelo <see cref="ICheckInService"/>.
        /// </summary>
        /// <param name="reservaId">ID da reserva pendente.</param>
        /// <param name="mensagem">Feedback da operação.</param>
        /// <returns>True se a reserva foi confirmada.</returns>
        bool ConfirmarReserva(int reservaId, out string mensagem);

        #endregion

        #region Read Operations

        /// <summary>
        /// Lista todas as reservas registadas no histórico e sistema ativo.
        /// </summary>
        /// <returns>Coleção de objetos <see cref="Reserva"/>.</returns>
        IEnumerable<Reserva> ObterTodasReservas();
        #endregion
    }
}