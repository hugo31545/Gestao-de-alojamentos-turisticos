//-----------------------------------------------------------------
//    <copyright file="CheckInService.cs" company="IPCA">
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
    /// Serviço responsável pela gestão do ciclo de vida presencial do hóspede.
    /// Controla as operações de entrada (Check-In) e saída (Check-Out), garantindo a 
    /// atualização sincronizada entre reservas e estados de alojamento.
    /// </summary>
    public class CheckInService : ICheckInService
    {

        #region Fields
        private readonly IReservaRepository _reservaRepo;
        private readonly IAlojamentoRepository _alojamentoRepo;
        private readonly IRegistoRepository _registoRepo;
        #endregion


        #region Constructors

        /// <summary>
        /// Inicializa o serviço injetando os repositórios necessários para a operação.
        /// </summary>
        public CheckInService(
            IReservaRepository reservaRepo,
            IAlojamentoRepository alojamentoRepo,
            IRegistoRepository registoRepo)
        {
            _reservaRepo = reservaRepo;
            _alojamentoRepo = alojamentoRepo;
            _registoRepo = registoRepo;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Realiza a entrada do hóspede, valida datas e altera o estado do alojamento para Ocupado.
        /// </summary>
        /// <param name="reservaId">ID da reserva a processar.</param>
        /// <param name="funcionarioResponsavel">Utilizador que realiza a operação.</param>
        /// <param name="mensagem">Mensagem de feedback para a interface.</param>
        /// <returns>True se o check-in foi concluído com sucesso.</returns>
        public bool RealizarCheckIn(int reservaId, string funcionarioResponsavel, out string mensagem)
        {
            // 1. Validar existência da reserva
            var reserva = _reservaRepo.GetById(reservaId);
            if (reserva == null) { mensagem = "Erro: Reserva não encontrada."; return false; }

            // 2. Validar estado atual (Só se faz check-in de reservas confirmadas ou pendentes)
            if (reserva.Estado == EstadoReserva.CheckInRealizado) { mensagem = "Aviso: O Check-In já foi realizado anteriormente."; return false; }
            if (reserva.Estado == EstadoReserva.Cancelada) { mensagem = "Erro: Não é possível fazer check-in de uma reserva cancelada."; return false; }

            // 3. Validação de Data (Regra: Não permitir check-in antecipado ao dia previsto)
            if (DateTime.Now.Date < reserva.DataCheckIn.Date)
            {
                mensagem = $"Erro: O check-in só é permitido a partir do dia {reserva.DataCheckIn:dd/MM/yyyy}.";
                return false;
            }

            try
            {
                // 4. Atualizar a Reserva
                reserva.Estado = EstadoReserva.CheckInRealizado;
                _reservaRepo.Update(reserva);

                // 5. Atualizar o Alojamento (Mudar para Ocupado)
                var alojamento = _alojamentoRepo.GetById(reserva.AlojamentoId);
                if (alojamento != null)
                {
                    alojamento.Estado = EstadoAlojamento.Ocupado;
                    _alojamentoRepo.Update(alojamento);
                }

                // 6. Auditoria de segurança
                _registoRepo.Add(new Registo(0, "CheckIn",
                    $"Check-in realizado para Reserva {reservaId}",
                    funcionarioResponsavel, "CHECKIN", reservaId, DateTime.Now));

                mensagem = "Check-in realizado com sucesso! O alojamento está agora marcado como ocupado.";
                return true;
            }
            catch (Exception ex)
            {
                mensagem = $"Erro técnico ao processar check-in: {ex.Message}";
                return false;
            }
        }


        /// <summary>
        /// Finaliza a estadia do hóspede, liberta a reserva e envia o alojamento para Manutenção/Limpeza.
        /// </summary>
        public bool RealizarCheckOut(int reservaId, string funcionarioResponsavel, out string mensagem)
        {
            var reserva = _reservaRepo.GetById(reservaId);
            if (reserva == null || reserva.Estado != EstadoReserva.CheckInRealizado)
            {
                mensagem = "Erro: Reserva não está em estado de estadia ativa.";
                return false;
            }

            try
            {
                // 1. Finalizar Reserva
                reserva.Estado = EstadoReserva.Finalizada;
                _reservaRepo.Update(reserva);

                // 2. Libertar Alojamento (Mudar para Manutenção para garantir limpeza)
                var alojamento = _alojamentoRepo.GetById(reserva.AlojamentoId);
                if (alojamento != null)
                {
                    
                    alojamento.Estado = EstadoAlojamento.Manutencao;
                    _alojamentoRepo.Update(alojamento);
                }

                // 3. Auditoria
                _registoRepo.Add(new Registo(0, "CheckOut",
                    $"Check-out finalizado para Reserva {reservaId}. Alojamento enviado para manutenção/limpeza.",
                    funcionarioResponsavel, "CHECKOUT", reservaId, DateTime.Now));

                mensagem = "Check-out realizado com sucesso!";
                return true;
            }
            catch (Exception ex)
            {
                mensagem = $"Erro ao processar check-out: {ex.Message}";
                return false;
            }
        }


        /// <summary>
        /// Obtém a listagem de todas as reservas que estão atualmente com hóspedes no alojamento.
        /// </summary>
        public IEnumerable<Reserva> ObterEstadiasAtuais()
        {
            return _reservaRepo.GetAll().Where(r => r.Estado == EstadoReserva.CheckInRealizado);
        }
        #endregion
    }
}