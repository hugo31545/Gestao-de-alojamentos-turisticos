//-----------------------------------------------------------------
//    <copyright file="Reserva.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.Models.Entities.Interfaces;
using POOfase1trabalho13_11.Models.Enums;
using System.Text.Json.Serialization;

namespace POOfase1trabalho13_11.Models.Entities
{

    /// <summary>
    /// Representa uma reserva de alojamento efetuada por um cliente.
    /// Gere o ciclo de vida da estadia, desde o pedido pendente até à confirmação ou cancelamento.
    /// Implementa a interface <see cref="IEntity"/>.
    /// </summary>
    public class Reserva : IEntity
    {
        #region Private Fields
        private int _id;
        private int _numeroHospedes;
        private EstadoReserva _estado;
        #endregion

        #region Public Properties
        /// <summary>
        /// Obtém ou define o identificador único da reserva.
        /// </summary>
        public int Id
        {
            get => _id;
            set => _id = value; 
        }

        /// <summary>
        /// Obtém ou define o identificador do cliente que realizou a reserva.
        /// </summary>
        public int ClienteId { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do alojamento reservado.
        /// </summary>
        public int AlojamentoId { get; set; }

        /// <summary>
        /// Obtém ou define a data em que a reserva foi registada no sistema.
        /// </summary>
        public DateTime DataReserva { get; set; } = DateTime.Now;

        /// <summary>
        /// Obtém ou define a data prevista para a entrada (Check-In).
        /// </summary>
        public DateTime DataCheckIn { get; set; }

        /// <summary>
        /// Obtém ou define a data prevista para a saída (Check-Out).
        /// </summary>
        public DateTime DataCheckOut { get; set; }

        /// <summary>
        /// Obtém ou define o número de pessoas que irão usufruir do alojamento nesta reserva.
        /// </summary>
        public int NumeroHospedes
        {
            get => _numeroHospedes;
            set => _numeroHospedes = value; 
        }

        /// <summary>
        /// Obtém ou define observações ou pedidos especiais relativos à reserva.
        /// </summary>
        public string Observacoes { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define o valor total da estadia, calculado no momento da reserva.
        /// Guardar o valor evita que alterações futuras ao preço do alojamento mudem a faturação.
        /// Reservas antigas têm 0 (o valor é então calculado com o preço atual).
        /// </summary>
        public decimal PrecoTotal { get; set; }

        /// <summary>
        /// Obtém ou define o estado atual da reserva (ex: Pendente, Confirmada, Cancelada).
        /// </summary>
        public EstadoReserva Estado
        {
            get => _estado;
            set => _estado = value;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Reserva"/> com parâmetros.
        /// Utilizado para a desserialização de dados a partir do ficheiro JSON.
        /// </summary>
        /// <param name="id">ID da reserva.</param>
        /// <param name="clienteId">ID do cliente.</param>
        /// <param name="alojamentoId">ID do alojamento.</param>
        /// <param name="dataCheckIn">Data de entrada.</param>
        /// <param name="dataCheckOut">Data de saída.</param>
        /// <param name="numeroHospedes">Quantidade de pessoas.</param>
        /// <param name="observacoes">Notas adicionais.</param>
        /// <param name="estado">Estado da reserva.</param>
        /// <param name="dataReserva">Data de criação do registo.</param>
        [JsonConstructor]
        public Reserva(int id, int clienteId, int alojamentoId, DateTime dataCheckIn, DateTime dataCheckOut, int numeroHospedes, string observacoes, EstadoReserva estado, DateTime dataReserva)
        {
            
            this.Id = id;
            this.ClienteId = clienteId;
            this.AlojamentoId = alojamentoId;
            this.DataCheckIn = dataCheckIn;
            this.DataCheckOut = dataCheckOut;
            this.NumeroHospedes = numeroHospedes;
            this.Observacoes = observacoes ?? string.Empty;
            this.Estado = estado;
            this.DataReserva = dataReserva == default ? DateTime.Now : dataReserva;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Reserva"/> com valores padrão.
        /// Define o estado inicial como <see cref="EstadoReserva.Pendente"/>.
        /// </summary>
        public Reserva()
        {
            DataReserva = DateTime.Now;
            Estado = EstadoReserva.Pendente;
        }
        #endregion

        #region Logic Methods
        /// <summary>
        /// Indica se a reserva ainda ocupa o alojamento (pendente, confirmada ou com check-in feito).
        /// </summary>
        [JsonIgnore]
        public bool EstaAtiva =>
            Estado == EstadoReserva.Pendente ||
            Estado == EstadoReserva.Confirmada ||
            Estado == EstadoReserva.CheckInRealizado;

        /// <summary>
        /// Indica se a estadia já terminou. Inclui <see cref="EstadoReserva.CheckOutRealizado"/>,
        /// usado por reservas antigas antes de o fluxo passar a terminar em <see cref="EstadoReserva.Finalizada"/>.
        /// </summary>
        [JsonIgnore]
        public bool EstaConcluida =>
            Estado == EstadoReserva.Finalizada ||
            Estado == EstadoReserva.CheckOutRealizado;

        /// <summary>
        /// Indica se a reserva ainda pode ser cancelada (antes do check-in).
        /// </summary>
        [JsonIgnore]
        public bool PodeSerCancelada =>
            Estado == EstadoReserva.Pendente ||
            Estado == EstadoReserva.Confirmada;

        /// <summary>
        /// Altera o estado da reserva de Pendente para Confirmada.
        /// </summary>
        /// <exception cref="InvalidOperationException">Se a reserva não estiver pendente.</exception>
        public void Confirmar()
        {
            if (Estado != EstadoReserva.Pendente)
                throw new InvalidOperationException($"Só é possível confirmar reservas pendentes (estado atual: {Estado}).");
            Estado = EstadoReserva.Confirmada;
        }

        /// <summary>
        /// Altera o estado da reserva para Cancelada.
        /// </summary>
        /// <exception cref="InvalidOperationException">Se o check-in já foi feito ou a reserva já terminou.</exception>
        public void Cancelar()
        {
            if (!PodeSerCancelada)
                throw new InvalidOperationException($"Não é possível cancelar uma reserva no estado {Estado}.");
            Estado = EstadoReserva.Cancelada;
        }

        /// <summary>
        /// Regista o check-in: Confirmada → CheckInRealizado.
        /// </summary>
        /// <exception cref="InvalidOperationException">Se a reserva não estiver confirmada.</exception>
        public void RegistarCheckIn()
        {
            if (Estado != EstadoReserva.Confirmada)
                throw new InvalidOperationException($"Só é possível fazer check-in de reservas confirmadas (estado atual: {Estado}).");
            Estado = EstadoReserva.CheckInRealizado;
        }

        /// <summary>
        /// Regista o check-out: CheckInRealizado → Finalizada.
        /// </summary>
        /// <exception cref="InvalidOperationException">Se o check-in ainda não foi feito.</exception>
        public void RegistarCheckOut()
        {
            if (Estado != EstadoReserva.CheckInRealizado)
                throw new InvalidOperationException($"Só é possível fazer check-out de reservas com check-in feito (estado atual: {Estado}).");
            Estado = EstadoReserva.Finalizada;
        }

        /// <summary>
        /// Devolve uma representação textual simplificada da reserva.
        /// </summary>
        /// <returns>String com ID, Cliente, Alojamento e Estado.</returns>
        public override string ToString() => $"Reserva {Id} - Cliente: {ClienteId}, Alojamento: {AlojamentoId}, Estado: {Estado}";
        #endregion
    }
}