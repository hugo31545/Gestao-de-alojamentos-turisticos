//-----------------------------------------------------------------
//    <copyright file="AlojamentoService.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Enums;
using POOfase1trabalho13_11.Models.Repositories.Interfaces;
using POOfase1trabalho13_11.Models.Services.Interfaces;
using POOfase1trabalho13_11.Exceptions;

namespace POOfase1trabalho13_11.Models.Services.Implementations
{

    /// <summary>
    /// Implementa a lógica de negócio para a gestão de alojamentos.
    /// Orquestra a interação entre múltiplos repositórios e valida regras de integridade.
    /// </summary>
    public class AlojamentoService : IAlojamentoService
    {
        #region Fields
        private readonly IAlojamentoRepository _alojamentoRepository;
        private readonly IRegistoRepository _registoRepository;
        private readonly IReservaRepository _reservaRepository;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa uma nova instância do serviço através de Injeção de Dependências.
        /// </summary>
        public AlojamentoService(
            IAlojamentoRepository alojamentoRepository,
            IRegistoRepository registoRepository,
            IReservaRepository reservaRepository)
        {
            _alojamentoRepository = alojamentoRepository;
            _registoRepository = registoRepository;
            _reservaRepository = reservaRepository;
        }

        #endregion

        #region Gestão de Alojamentos (CRUD)
        /// <summary>
        /// Lista todos os alojamentos registados no sistema.
        /// </summary>
        public IEnumerable<Alojamento> ObterTodosAlojamentos() => _alojamentoRepository.GetAll();

        /// <summary>
        /// Procura um alojamento pelo seu identificador.
        /// </summary>
        public Alojamento? ObterAlojamentoPorId(int id) => _alojamentoRepository.GetById(id);

        /// <summary>
        /// Guarda ou atualiza um alojamento, validando a unicidade do nome e a validade do preço.
        /// </summary>
        /// <exception cref="BusinessException">Lançada quando o nome já existe ou o preço é inválido.</exception>
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
            // Validação de Preço
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


        /// <summary>
        /// Remove um alojamento se este não possuir reservas confirmadas pendentes.
        /// </summary>
        /// <exception cref="BusinessException">Lançada se existirem reservas ativas para o alojamento.</exception>
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


        /// <summary>
        /// Altera o estado de um alojamento (ex: de Disponível para Manutenção) e gera um log.
        /// </summary>
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


        /// <summary>
        /// Calcula o valor total com base no preço por noite do alojamento.
        /// </summary>
        public decimal CalcularPrecoTotal(int alojamentoId, int numeroNoites)
        {
            if (numeroNoites <= 0) return 0;

            var alojamento = _alojamentoRepository.GetById(alojamentoId);
            return alojamento != null ? (alojamento.PrecoPorNoite * numeroNoites) : 0;
        }


        #endregion

        #region Pesquisa e Disponibilidade 


        /// <summary>
        /// Retorna todos os alojamentos marcados como disponíveis.
        /// </summary>
        public IEnumerable<Alojamento> BuscarAlojamentosDisponiveis() =>
            _alojamentoRepository.BuscarDisponiveis();



        /// <summary>
        /// Verifica a disponibilidade real cruzando dados de alojamentos e reservas existentes num período.
        /// </summary>
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


        /// <summary>
        /// Procura o melhor alojamento disponível com base na capacidade, tipo e preço.
        /// </summary>

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

        /// <summary>
        /// Método auxiliar para gerar logs de auditoria no sistema.
        /// </summary>
        private void GerarRegisto(string entidade, string acao, string detalhe, int refId)
        {
            _registoRepository.Add(new Registo(0, entidade, detalhe, "Sistema", acao, refId, DateTime.Now));
        }


        /// <summary>
        /// Filtra alojamentos por tipo.
        /// </summary>
        public IEnumerable<Alojamento> FiltrarPorTipo(TipoAlojamento tipo) => _alojamentoRepository.FiltrarPorTipo(tipo);
        #endregion
    } 
}