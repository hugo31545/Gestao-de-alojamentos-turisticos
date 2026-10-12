//-----------------------------------------------------------------
//    <copyright file="ReservaRepository.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Enums;
using POOfase1trabalho13_11.Models.Repositories.Interfaces;

namespace POOfase1trabalho13_11.Models.Repositories.Implementations
{

    /// <summary>
    /// Repositório especializado na gestão de objetos do tipo <see cref="Reserva"/>.
    /// Permite a persistência em JSON e fornece métodos de filtragem por cliente, alojamento e estado da reserva.
    /// </summary>
    public class ReservaRepository : JsonRepository<Reserva>, IReservaRepository
    {
        #region Constructors
        /// <summary>
        /// Inicializa uma nova instância de <see cref="ReservaRepository"/>,
        /// configurando o ficheiro "reservas.json" para armazenamento.
        /// </summary>
        public ReservaRepository() : base("reservas.json") { }
        #endregion


        #region Public Methods

        /// <summary>
        /// Obtém todas as reservas associadas a um cliente específico.
        /// </summary>
        /// <param name="clienteId">O identificador único do cliente.</param>
        /// <returns>Uma coleção de reservas do cliente indicado.</returns>
        public IEnumerable<Reserva> FiltrarPorCliente(int clienteId) =>
            Items.Where(r => r.ClienteId == clienteId);


        /// <summary>
        /// Obtém todas as reservas registadas para um determinado alojamento.
        /// </summary>
        /// <param name="alojamentoId">O identificador único do alojamento.</param>
        /// <returns>Uma coleção de reservas associadas ao alojamento.</returns>
        public IEnumerable<Reserva> FiltrarPorAlojamento(int alojamentoId) =>
            Items.Where(r => r.AlojamentoId == alojamentoId);


        /// <summary>
        /// Procura todas as reservas que estão atualmente em curso (Confirmadas ou com Check-In já realizado).
        /// </summary>
        /// <returns>Coleção de reservas ativas.</returns>
        public IEnumerable<Reserva> BuscarAtivas() =>
            Items.Where(r => r.Estado == EstadoReserva.Confirmada || r.Estado == EstadoReserva.CheckInRealizado);


        /// <summary>
        /// Procura todas as reservas que aguardam validação (Estado Pendente).
        /// </summary>
        /// <returns>Coleção de reservas pendentes.</returns>
        public IEnumerable<Reserva> BuscarPendentes() =>
            Items.Where(r => r.Estado == EstadoReserva.Pendente);
        #endregion
    }
}