using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Enums;
using POOfase1trabalho13_11.Models.Repositories.Interfaces;
using POOfase1trabalho13_11.Models.Services.Interfaces;

namespace POOfase1trabalho13_11.Models.Services.Implementations
{
    public class ConsultaService : IConsultaService
    {
        private readonly IAlojamentoRepository _alojamentoRepo;
        private readonly IReservaRepository _reservaRepo;
        private readonly IClienteRepository _clienteRepo;
        private readonly IConsultaRepository _consultaRepo;

        public ConsultaService(
            IAlojamentoRepository alojamentoRepo,
            IReservaRepository reservaRepo,
            IClienteRepository clienteRepo,
            IConsultaRepository consultaRepo)
        {
            _alojamentoRepo = alojamentoRepo;
            _reservaRepo = reservaRepo;
            _clienteRepo = clienteRepo;
            _consultaRepo = consultaRepo;
        }

        #region Relatórios de Alojamento

        public IEnumerable<Alojamento> ObterAlojamentosMaisCaros(int top)
        {
            return _alojamentoRepo.GetAll()
                .OrderByDescending(a => a.PrecoPorNoite)
                .Take(top);
        }

        public double CalcularTaxaOcupacao()
        {
            var totalAlojamentos = _alojamentoRepo.GetAll().Count();
            if (totalAlojamentos == 0) return 0;

            var ocupados = _alojamentoRepo.GetAll()
                .Count(a => a.Estado == EstadoAlojamento.Ocupado);

            return (double)ocupados / totalAlojamentos * 100;
        }

        #endregion

        #region Relatórios de Clientes

        public IEnumerable<Cliente> ObterClientesComMaisReservas(int top)
        {
            // Agrupa as reservas por ID de cliente, conta-as e ordena
            var rankingIds = _reservaRepo.GetAll()
                .GroupBy(r => r.ClienteId)
                .OrderByDescending(g => g.Count())
                .Take(top)
                .Select(g => g.Key);

            return rankingIds.Select(id => _clienteRepo.GetById(id)).Where(c => c != null)!;
        }

        #endregion

        #region Faturação

        public decimal CalcularFaturacaoTotal(DateTime inicio, DateTime fim)
        {
            // Soma o preço total de todas as reservas finalizadas no período
            return _reservaRepo.GetAll()
                .Where(r => r.Estado == EstadoReserva.Finalizada &&
                            r.DataReserva >= inicio &&
                            r.DataReserva <= fim)
                .Sum(r => {
                    var alojamento = _alojamentoRepo.GetById(r.AlojamentoId);
                    if (alojamento == null) return 0;
                    var noites = (r.DataCheckOut - r.DataCheckIn).Days;
                    return (noites <= 0 ? 1 : noites) * alojamento.PrecoPorNoite;
                });
        }

        #endregion

        #region Gestão do Histórico de Consultas

        public void RegistarPesquisaNoHistorico(string utilizador, string tipo, Dictionary<string, string> filtros, int resultadosEncontrados)
        {
            var novaConsulta = new Consulta
            {
                Id = 0, 
                Utilizador = utilizador,
                Tipo = tipo,
                DataConsulta = DateTime.Now,
                Parametros = filtros,
                Resultados = new List<string> { $"Encontrados {resultadosEncontrados} registos." }
            };

            _consultaRepo.Add(novaConsulta);
        }

        public IEnumerable<Consulta> ObterHistoricoRecente(int quantidade)
        {
            return _consultaRepo.GetAll()
                .OrderByDescending(c => c.DataConsulta)
                .Take(quantidade);
        }

        #endregion
    }
}