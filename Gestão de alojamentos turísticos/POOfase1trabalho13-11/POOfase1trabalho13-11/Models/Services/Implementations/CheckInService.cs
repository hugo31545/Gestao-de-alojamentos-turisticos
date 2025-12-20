using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Enums;
using POOfase1trabalho13_11.Models.Repositories.Interfaces;
using POOfase1trabalho13_11.Models.Services.Interfaces;

namespace POOfase1trabalho13_11.Models.Services.Implementations
{
    public class CheckInService : ICheckInService
    {
        private readonly IReservaRepository _reservaRepo;
        private readonly IAlojamentoRepository _alojamentoRepo;
        private readonly IRegistoRepository _registoRepo;

        public CheckInService(
            IReservaRepository reservaRepo,
            IAlojamentoRepository alojamentoRepo,
            IRegistoRepository registoRepo)
        {
            _reservaRepo = reservaRepo;
            _alojamentoRepo = alojamentoRepo;
            _registoRepo = registoRepo;
        }

        public bool RealizarCheckIn(int reservaId, string funcionarioResponsavel, out string mensagem)
        {
            // 1. Validar existência da reserva
            var reserva = _reservaRepo.GetById(reservaId);
            if (reserva == null) { mensagem = "Erro: Reserva não encontrada."; return false; }

            // 2. Validar estado atual (Só se faz check-in de reservas confirmadas ou pendentes)
            if (reserva.Estado == EstadoReserva.CheckInRealizado) { mensagem = "Aviso: O Check-In já foi realizado anteriormente."; return false; }
            if (reserva.Estado == EstadoReserva.Cancelada) { mensagem = "Erro: Não é possível fazer check-in de uma reserva cancelada."; return false; }

            // 3. Validação de Data (Regra de Negócio: não permitir check-in muito antes da data)
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

                // 6. Auditoria
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

                // 2. Libertar Alojamento (Mudar para Manutenção ou Disponível conforme regra)
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

        public IEnumerable<Reserva> ObterEstadiasAtuais()
        {
            return _reservaRepo.GetAll().Where(r => r.Estado == EstadoReserva.CheckInRealizado);
        }
    }
}