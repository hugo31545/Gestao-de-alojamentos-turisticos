//-----------------------------------------------------------------
//    <copyright file="IConsultaService.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>1</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using System.Collections.Generic;
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Enums;

namespace POOfase1trabalho13_11.Models.Services.Interfaces
{

    /// <summary>
    /// Define o contrato para serviços de análise de dados, relatórios estatísticos 
    /// e gestão do histórico de consultas do sistema.
    /// </summary>
    public interface IConsultaService
    {
        #region Relatórios de Alojamentos
        /// <summary>
        /// Identifica os alojamentos com os preços por noite mais elevados.
        /// </summary>
        /// <param name="top">Número de registos a devolver.</param>
        /// <returns>Coleção de <see cref="Alojamento"/> ordenada por preço decrescente.</returns>
        IEnumerable<Alojamento> ObterAlojamentosMaisCaros(int top);

        /// <summary>
        /// Calcula a percentagem de ocupação atual com base no estado dos alojamentos.
        /// </summary>
        /// <returns>Valor double representando a taxa de ocupação (0 a 100).</returns>
        double CalcularTaxaOcupacao();
        #endregion

        #region Relatórios de Clientes
        /// <summary>
        /// Gera um ranking dos clientes que efetuaram o maior número de reservas no sistema.
        /// </summary>
        /// <param name="top">Quantidade de clientes a listar no ranking.</param>
        /// <returns>Coleção de <see cref="Cliente"/> ordenada por volume de reservas.</returns>
        IEnumerable<Cliente> ObterClientesComMaisReservas(int top);
        #endregion


        #region Relatórios de Faturação

        /// <summary>
        /// Calcula o montante total faturado num determinado intervalo temporal.
        /// </summary>
        /// <param name="inicio">Data inicial do período.</param>
        /// <param name="fim">Data final do período.</param>
        /// <returns>Valor total em <see cref="decimal"/>.</returns>
        decimal CalcularFaturacaoTotal(DateTime inicio, DateTime fim);
        #endregion

        #region Histórico de Pesquisas
        /// <summary>
        /// Regista uma operação de pesquisa no histórico para fins de auditoria e análise de tendências.
        /// </summary>
        /// <param name="utilizador">Identificação do utilizador que realizou a consulta.</param>
        /// <param name="tipo">Área consultada (ex: "Alojamentos", "Reservas").</param>
        /// <param name="filtros">Dicionário com os parâmetros de filtro aplicados.</param>
        /// <param name="resultadosEncontrados">Número de registos retornados pela pesquisa.</param>
        void RegistarPesquisaNoHistorico(string utilizador, string tipo, Dictionary<string, string> filtros, int resultadosEncontrados);


        /// <summary>
        /// Recupera as consultas realizadas mais recentemente no sistema.
        /// </summary>
        /// <param name="quantidade">Número de registos a recuperar.</param>
        /// <returns>Coleção de objetos <see cref="Consulta"/>.</returns>
        IEnumerable<Consulta> ObterHistoricoRecente(int quantidade);
        #endregion
    }
}