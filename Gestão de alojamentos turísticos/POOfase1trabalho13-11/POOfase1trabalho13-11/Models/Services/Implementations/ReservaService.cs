//-----------------------------------------------------------------
//    <copyright file="ReservaService.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Enums;
using POOfase1trabalho13_11.Models.Repositories.Interfaces;
using POOfase1trabalho13_11.Models.Services.Interfaces;

namespace POOfase1trabalho13_11.Models.Services.Implementations
{

    /// <summary>
    /// Fornece serviços para a gestão completa do ciclo de vida de reservas.
    /// Inclui validação de disponibilidade, gestão de estados e auditoria de operações.
    /// </summary>
    public class ReservaService : IReservaService
    {

        #region Fields
        private readonly IReservaRepository _reservaRepo;
        private readonly IAlojamentoRepository _alojamentoRepo;
        private readonly IClienteRepository _clienteRepo;
        private readonly IRegistoRepository _registoRepo;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa o serviço injetando as dependências necessárias para a operação de reservas.
        /// </summary>
        public ReservaService(
            IReservaRepository reservaRepo,
            IAlojamentoRepository alojamentoRepo,
            IClienteRepository clienteRepo,
            IRegistoRepository registoRepo)
        {
            _reservaRepo = reservaRepo;
            _alojamentoRepo = alojamentoRepo;
            _clienteRepo = clienteRepo;
            _registoRepo = registoRepo;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Cria uma nova reserva após validar cliente, alojamento, capacidade e disponibilidade de datas.
        /// </summary>
        public bool RealizarReserva(int clienteId, int alojamentoId, DateTime checkIn, DateTime checkOut, int numHospedes, out string mensagem)
        {
            // 1. Validações de Existência
            var cliente = _clienteRepo.GetById(clienteId);
            if (cliente == null) { mensagem = "Cliente não encontrado."; return false; }

            var alojamento = _alojamentoRepo.GetById(alojamentoId);
            if (alojamento == null) { mensagem = "Alojamento não encontrado."; return false; }

            // 2. Validação de Datas
            if (checkIn >= checkOut) { mensagem = "Data de Check-In deve ser anterior ao Check-Out."; return false; }

            // 3. Validação de Disponibilidade (Conflito de Calendário)
            if (ExisteConflitoDeDatas(alojamentoId, checkIn, checkOut))
            {
                mensagem = "O alojamento já está ocupado no período selecionado.";
                return false;
            }


            if (alojamento.Estado != EstadoAlojamento.Disponivel)
            {
                mensagem = $"O alojamento não pode receber reservas (Estado: {alojamento.Estado}).";
                return false;
            }

            if (numHospedes > alojamento.Capacidade) { mensagem = "Número de hóspedes excede a capacidade."; return false; }

            try
            {
                // Criar a Entidade
                var novaReserva = new Reserva(
                    0,
                    clienteId,
                    alojamentoId,
                    checkIn,
                    checkOut,
                    numHospedes,
                    "Reserva criada via sistema",
                    EstadoReserva.Pendente,
                    DateTime.Now
                );

                //  Persistência
                _reservaRepo.Add(novaReserva);

                // Log de Auditoria
                _registoRepo.Add(new Registo(
                    0,
                    "Reserva",
                    $"Nova reserva ID {novaReserva.Id} para o cliente {cliente.Nome}",
                    "Sistema",
                    "CREATE",
                    novaReserva.Id,
                    DateTime.Now
                ));

                mensagem = $"Reserva {novaReserva.Id} realizada com sucesso!";
                return true;
            }
            catch (Exception ex)
            {
                mensagem = $"Erro interno ao processar reserva: {ex.Message}";
                return false;
            }
        }


        /// <summary>
        /// Cancela uma reserva existente e liberta o período no calendário.
        /// </summary>
        public bool CancelarReserva(int reservaId, string motivo, out string mensagem)
        {
            var reserva = _reservaRepo.GetById(reservaId);
            if (reserva == null) { mensagem = "Reserva não encontrada."; return false; }

            reserva.Cancelar();
            reserva.Observacoes += $" | Cancelamento: {motivo}";

            _reservaRepo.Update(reserva);

            _registoRepo.Add(new Registo(0, "Reserva", motivo, "Sistema", "CANCEL", reservaId, DateTime.Now));

            mensagem = "Reserva cancelada com sucesso.";
            return true;
        }



        /// <summary>
        /// Atualiza o estado da reserva para Check-In realizado.
        /// </summary>
        public bool EfetuarCheckIn(int reservaId, out string mensagem)
        {
            var reserva = _reservaRepo.GetById(reservaId);
            if (reserva == null) { mensagem = "Reserva não encontrada."; return false; }

            if (reserva.Estado != EstadoReserva.Confirmada)
            {
                mensagem = "Só pode fazer Check-in de reservas já Confirmadas.";
                return false;
            }

            reserva.Estado = EstadoReserva.CheckInRealizado;
            _reservaRepo.Update(reserva);
            mensagem = "Check-in efetuado com sucesso!";
            return true;
        }


        /// <summary>
        /// Finaliza a reserva após o Check-Out.
        /// </summary>
        public bool EfetuarCheckOut(int reservaId, out string mensagem)
        {
            var reserva = _reservaRepo.GetById(reservaId);
            if (reserva == null) { mensagem = "Reserva não encontrada."; return false; }

            if (reserva.Estado != EstadoReserva.CheckInRealizado)
            {
                mensagem = "Só pode fazer Check-out de reservas que já fizeram Check-in.";
                return false;
            }

            reserva.Estado = EstadoReserva.CheckOutRealizado;
            _reservaRepo.Update(reserva);
            mensagem = "Check-out efetuado com sucesso!";
            return true;
        }


        /// <summary>
        /// Retorna a lista completa de reservas.
        /// </summary>
        public IEnumerable<Reserva> ObterTodasReservas()
        {
           
            return _reservaRepo.GetAll();
        }


        /// <summary>
        /// Lógica de sobreposição de períodos: (StartA < EndB) AND (EndA > StartB).
        /// </summary>
        private bool ExisteConflitoDeDatas(int alojamentoId, DateTime novoCheckIn, DateTime novoCheckOut)
        {
            
            var reservasNoAlojamento = _reservaRepo.GetAll()
                .Where(r => r.AlojamentoId == alojamentoId && r.Estado != EstadoReserva.Cancelada);

            
            return reservasNoAlojamento.Any(r =>
                novoCheckIn < r.DataCheckOut && novoCheckOut > r.DataCheckIn);
        }
        #endregion
    }
}