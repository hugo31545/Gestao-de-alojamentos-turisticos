//-----------------------------------------------------------------
//    <copyright file="ICheckInService.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>1</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using System.Collections.Generic;
using POOfase1trabalho13_11.Models.Entities;

namespace POOfase1trabalho13_11.Models.Services.Interfaces
{

    /// <summary>
    /// Define as operações necessárias para a gestão de entradas (Check-in) e saídas (Check-out).
    /// Esta interface assegura a transição de estados entre reservas e a ocupação efetiva dos alojamentos.
    /// </summary>
    public interface ICheckInService
    {

        #region Methods
        /// <summary>
        /// Tenta realizar o check-in de uma reserva existente, validando o seu estado atual.
        /// </summary>
        /// <param name="reservaId">Identificador único da reserva.</param>
        /// <param name="funcionarioResponsavel">Nome ou ID do funcionário que processa a entrada.</param>
        /// <param name="mensagem">Mensagem de saída com o detalhe do sucesso ou erro da operação.</param>
        /// <returns>True se o check-in for realizado com sucesso; caso contrário, false.</returns>
        bool RealizarCheckIn(int reservaId, string funcionarioResponsavel, out string mensagem);

        /// <summary>
        /// Finaliza a estadia de um cliente, libertando o alojamento e atualizando o estado da reserva.
        /// </summary>
        /// <param name="reservaId">Identificador único da reserva.</param>
        /// <param name="funcionarioResponsavel">Nome ou ID do funcionário que processa a saída.</param>
        /// <param name="mensagem">Mensagem de saída com o detalhe do sucesso ou erro da operação.</param>
        /// <returns>True se o check-out for concluído; caso contrário, false.</returns>
        bool RealizarCheckOut(int reservaId, string funcionarioResponsavel, out string mensagem);

        /// <summary>
        /// Obtém a listagem de todas as reservas que se encontram atualmente com estadia em curso.
        /// </summary>
        /// <returns>Coleção de objetos <see cref="Reserva"/> com estado de check-in ativo.</returns>
        IEnumerable<Reserva> ObterEstadiasAtuais();
        #endregion
    }
}