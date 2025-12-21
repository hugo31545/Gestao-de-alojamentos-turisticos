//-----------------------------------------------------------------
//    <copyright file="ICheckInRepository.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.Models.Entities;

namespace POOfase1trabalho13_11.Models.Repositories.Interfaces
{
    /// <summary>
    /// Define o contrato para as operações de acesso a dados da entidade <see cref="CheckIn"/>.
    /// Estende a interface genérica <see cref="IRepository{T}"/> para incluir pesquisas por reserva.
    /// </summary>
    public interface ICheckInRepository : IRepository<CheckIn>
    {
        /// <summary>
        /// Procura o registo de check-in associado a uma reserva específica.
        /// </summary>
        /// <param name="reservaId">O identificador único da reserva a pesquisar.</param>
        /// <returns>
        /// O objeto <see cref="CheckIn"/> se for encontrado um registo para o ID fornecido; 
        /// caso contrário, <c>null</c>.
        /// </returns>
        CheckIn? GetByReservaId(int reservaId);
    }
}