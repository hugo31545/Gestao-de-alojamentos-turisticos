//-----------------------------------------------------------------
//    <copyright file="IRegistoService.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>1</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using System;
using System.Collections.Generic;
using POOfase1trabalho13_11.Models.Entities;

namespace POOfase1trabalho13_11.Models.Services.Interfaces
{

    /// <summary>
    /// Define o contrato para serviços de auditoria e logs do sistema.
    /// Permite a rastreabilidade de todas as operações de escrita (Create, Update, Delete) 
    /// realizadas nas diversas entidades do domínio.
    /// </summary>
    public interface IRegistoService
    {

        #region Consultas de Auditoria
        /// <summary>
        /// Obtém o histórico completo de todos os registos de auditoria.
        /// </summary>
        /// <returns>Uma coleção de objetos <see cref="Registo"/>.</returns>
        IEnumerable<Registo> ObterTodosLogs();

        /// <summary>
        /// Filtra o histórico de auditoria para uma entidade específica (ex: um Alojamento ou Cliente em particular).
        /// </summary>
        /// <param name="tipoEntidade">O nome da classe da entidade (ex: "Alojamento").</param>
        /// <param name="entidadeId">O ID único da instância dessa entidade.</param>
        /// <returns>Coleção de registos associados ao objeto especificado.</returns>
        IEnumerable<Registo> ObterLogsPorEntidade(string tipoEntidade, int entidadeId);


        /// <summary>
        /// Filtra os logs pelo tipo de ação realizada.
        /// </summary>
        /// <param name="operacao">A operação a filtrar (ex: "CREATE", "UPDATE", "DELETE").</param>
        /// <returns>Coleção de registos que correspondem à operação indicada.</returns>
        IEnumerable<Registo> ObterLogsPorOperacao(string operacao);

        /// <summary>
        /// Obtém uma lista reduzida dos registos mais recentes inseridos no sistema.
        /// </summary>
        /// <param name="quantidade">O número de registos pretendidos (ex: os últimos 50).</param>
        /// <returns>Coleção dos logs mais recentes, tipicamente ordenada por data descendente.</returns>
        IEnumerable<Registo> ObterLogsRecentes(int quantidade);
        #endregion

        #region Manutenção do Sistema

        /// <summary>
        /// Realiza a limpeza do histórico de auditoria para otimização de armazenamento,
        /// removendo registos anteriores à data especificada.
        /// </summary>
        /// <param name="dataCorte">Registos com data inferior a esta serão eliminados.</param>
        /// <param name="mensagem">Feedback sobre o resultado da operação (ex: "X registos removidos").</param>
        /// <returns>True se a operação de manutenção foi concluída com sucesso.</returns>
        bool LimparLogsAntigos(DateTime dataCorte, out string mensagem);

        #endregion
    }
}