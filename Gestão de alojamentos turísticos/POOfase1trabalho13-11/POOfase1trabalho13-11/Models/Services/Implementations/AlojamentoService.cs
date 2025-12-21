using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Enums;
using POOfase1trabalho13_11.Models.Repositories.Interfaces;
using POOfase1trabalho13_11.Models.Services.Interfaces;
using POOfase1trabalho13_11.Exceptions;

namespace POOfase1trabalho13_11.Models.Services.Implementations
{

    
    public class AlojamentoService : IAlojamentoService
    {
        
        private readonly IAlojamentoRepository _alojamentoRepository;
        private readonly IRegistoRepository _registoRepository;
        private readonly IReservaRepository _reservaRepository;
        

        public AlojamentoService(
            IAlojamentoRepository alojamentoRepository,
            IRegistoRepository registoRepository,
            IReservaRepository reservaRepository)
        {
            _alojamentoRepository = alojamentoRepository;
            _registoRepository = registoRepository;
            _reservaRepository = reservaRepository;
        }

        #region Gestão de Alojamentos (CRUD)

        public IEnumerable<Alojamento> ObterTodosAlojamentos() => _alojamentoRepository.GetAll();

        public Alojamento? ObterAlojamentoPorId(int id) => _alojamentoRepository.GetById(id);

        /// <summary>
        /// Grava um alojamento (Cria novo ou atualiza existente) com validações automáticas.
        /// </summary>
        public bool GuardarAlojamento(Alojamento alojamento)
        {
            if (alojamento == null) throw new BusinessException("Os dados do alojamento são inválidos."); ;

            // 1. Validação de Unicidade de Nome (Apenas para novos ou se o nome mudou)
            var existente = _alojamentoRepository.BuscarPorNome(alojamento.Nome);
            if (existente != null && existente.Id != alojamento.Id)
            {
                // Nome já ocupado por OUTRO alojamento
                throw new BusinessException($"Já existe um alojamento com o nome '{alojamento.Nome}'.");
            }

            if (alojamento.PrecoPorNoite <= 0)
                throw new BusinessException("O preço por noite deve ser superior a zero.");

            // 2. Determinar tipo de operação para o log
            string operacao = alojamento.Id <= 0 ? "CREATE" : "UPDATE";

            // 3. Persistência Automática (Usa o Save genérico que lida com Add/Update)
            bool sucesso = _alojamentoRepository.Save(alojamento);

            if (sucesso)
            {
                GerarRegisto("Alojamento", operacao, $"Alojamento '{alojamento.Nome}' guardado com sucesso.", alojamento.Id);
            }

            return sucesso;
        }

        public bool RemoverAlojamento(int id)
        {
           
            bool temReservasAtivas = _reservaRepository.GetAll()
                .Any(r => r.AlojamentoId == id && r.Estado == EstadoReserva.Confirmada);

            if (temReservasAtivas) throw new BusinessException("Não é possível remover: Este alojamento possui reservas confirmadas."); ;

            bool sucesso = _alojamentoRepository.Remove(id);
            if (sucesso)
            {
                GerarRegisto("Alojamento", "DELETE", $"Removido ID: {id}", id);
            }

            return sucesso;
        }

        #endregion

        #region Lógica de Negócio e Estados

        public bool MudarEstado(int idAlojamento, EstadoAlojamento novoEstado)
        {
            var alojamento = _alojamentoRepository.GetById(idAlojamento);
            if (alojamento == null) return false;

            if (alojamento.Estado == novoEstado) return true;

            var estadoAntigo = alojamento.Estado;
            alojamento.Estado = novoEstado;

            if (_alojamentoRepository.Update(alojamento))
            {
                GerarRegisto("Alojamento", "STATE_CHANGE", $"Mudança de {estadoAntigo} para {novoEstado}", idAlojamento);
                return true;
            }
            return false;
        }

        public decimal CalcularPrecoTotal(int alojamentoId, int numeroNoites)
        {
            if (numeroNoites <= 0) return 0;

            var alojamento = _alojamentoRepository.GetById(alojamentoId);
            return alojamento != null ? (alojamento.PrecoPorNoite * numeroNoites) : 0;
        }


        #endregion

        #region Pesquisa e Disponibilidade (Excelência em Performance)

        public IEnumerable<Alojamento> BuscarAlojamentosDisponiveis() =>
            _alojamentoRepository.BuscarDisponiveis();

        public IEnumerable<Alojamento> BuscarDisponiveisPorPeriodo(DateTime checkIn, DateTime checkOut)
        {
            if (checkIn >= checkOut) return Enumerable.Empty<Alojamento>();

            var disponiveisGeral = _alojamentoRepository.BuscarDisponiveis();

            
            return disponiveisGeral.Where(alojamento =>
                !_reservaRepository.GetAll().Any(reserva =>
                    reserva.AlojamentoId == alojamento.Id &&
                    reserva.Estado != EstadoReserva.Cancelada &&
                    checkIn < reserva.DataCheckOut &&
                    checkOut > reserva.DataCheckIn)
            );
        }

        public Alojamento? EncontrarMelhorOpcao(int capacidade, TipoAlojamento tipo, DateTime checkIn, DateTime checkOut)
        {
            var candidatos = BuscarDisponiveisPorPeriodo(checkIn, checkOut)
                                .Where(a => a.Capacidade >= capacidade)
                                .ToList();

            
            return candidatos.FirstOrDefault(a => a.Tipo == tipo)
                   ?? candidatos.OrderBy(a => a.PrecoPorNoite).FirstOrDefault();
        }

        #endregion

        #region  Helpers
        private void GerarRegisto(string entidade, string acao, string detalhe, int refId)
        {
            _registoRepository.Add(new Registo(0, entidade, detalhe, "Sistema", acao, refId, DateTime.Now));
        }

        public IEnumerable<Alojamento> FiltrarPorTipo(TipoAlojamento tipo) => _alojamentoRepository.FiltrarPorTipo(tipo);
        #endregion
    } 
}