//-----------------------------------------------------------------
//    <copyright file="CheckInRepository.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Repositories.Interfaces;
using System.Linq;

namespace POOfase1trabalho13_11.Models.Repositories.Implementations
{

    /// <summary>
    /// Repositório especializado na gestão de registos de <see cref="CheckIn"/>.
    /// Herda as funcionalidades base de persistência JSON da classe <see cref="JsonRepository{T}"/>.
    /// </summary>
    public class CheckInRepository : JsonRepository<CheckIn>, ICheckInRepository
    {

        /// <summary>
        /// Inicializa uma nova instância de <see cref="CheckInRepository"/>,
        /// configurando o ficheiro "checkins.json" para armazenamento.
        /// </summary>
        public CheckInRepository() : base("checkins.json") { }


        /// <summary>
        /// Procura o registo de check-in associado a uma reserva específica.
        /// </summary>
        /// <param name="reservaId">O identificador único da reserva.</param>
        /// <returns>O objeto <see cref="CheckIn"/> encontrado ou null caso não exista registo para essa reserva.</returns>
        public CheckIn? GetByReservaId(int reservaId)
        {
            return Items.FirstOrDefault(c => c.ReservaId == reservaId);
        }

    }
}