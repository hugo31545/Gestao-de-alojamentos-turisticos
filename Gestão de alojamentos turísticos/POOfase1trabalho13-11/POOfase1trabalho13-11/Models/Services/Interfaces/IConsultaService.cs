using System.Collections.Generic;
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Enums;

namespace POOfase1trabalho13_11.Models.Services.Interfaces
{
    public interface IConsultaService
    {
        // Relatórios de Alojamentos
        IEnumerable<Alojamento> ObterAlojamentosMaisCaros(int top);
        double CalcularTaxaOcupacao();

        // Relatórios de Clientes
        IEnumerable<Cliente> ObterClientesComMaisReservas(int top);

        // Relatórios de Faturação
        decimal CalcularFaturacaoTotal(DateTime inicio, DateTime fim);

        // Histórico de Pesquisas
        void RegistarPesquisaNoHistorico(string utilizador, string tipo, Dictionary<string, string> filtros, int resultadosEncontrados);
        IEnumerable<Consulta> ObterHistoricoRecente(int quantidade);
    }
}