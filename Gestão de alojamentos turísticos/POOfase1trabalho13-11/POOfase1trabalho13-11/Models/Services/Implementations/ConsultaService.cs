//-----------------------------------------------------------------
//    <copyright file="ConsultaService.cs" company="IPCA">
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
    /// Implementa a lógica de análise de dados e geração de relatórios.
    /// Cruza informações de diversos repositórios para obter métricas de desempenho e histórico de atividade.
    /// </summary>
    public class ConsultaService : IConsultaService
    {
        #region Fields
        private readonly IAlojamentoRepository _alojamentoRepo;
        private readonly IReservaRepository _reservaRepo;
        private readonly IClienteRepository _clienteRepo;
        private readonly IConsultaRepository _consultaRepo;
        #endregion

        #region Constructors

        /// <summary>
        /// Inicializa o serviço injetando as dependências necessárias para consultas multidimensionais.
        /// </summary>
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

        #endregion

        #region Relatórios de Alojamento


        /// <summary>
        /// Identifica os alojamentos com o preço por noite mais elevado.
        /// </summary>
        /// <param name="top">Quantidade de registos a retornar.</param>
        public IEnumerable<Alojamento> ObterAlojamentosMaisCaros(int top)
        {
            return _alojamentoRepo.GetAll()
                .OrderByDescending(a => a.PrecoPorNoite)
                .Take(top);
        }


        /// <summary>
        /// Calcula a percentagem atual de alojamentos ocupados face ao total existente.
        /// </summary>
        /// <returns>Valor percentual (0-100).</returns>
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


        /// <summary>
        /// Gera um ranking de clientes com base no volume de reservas efetuadas.
        /// </summary>
        /// <param name="top">Número de clientes para o ranking.</param>
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


        /// <summary>
        /// Calcula o total financeiro gerado por reservas concluídas num intervalo de datas.
        /// </summary>
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

        /// <summary>
        /// Regista uma entrada no histórico de pesquisas para fins de auditoria de utilizador.
        /// </summary>
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


        /// <summary>
        /// Obtém os últimos registos de pesquisa efetuados no sistema.
        /// </summary>
        public IEnumerable<Consulta> ObterHistoricoRecente(int quantidade)
        {
            return _consultaRepo.GetAll()
                .OrderByDescending(c => c.DataConsulta)
                .Take(quantidade);
        }

        #endregion
    }
}