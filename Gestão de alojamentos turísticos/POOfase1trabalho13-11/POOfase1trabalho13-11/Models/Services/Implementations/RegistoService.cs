//-----------------------------------------------------------------
//    <copyright file="RegistoService.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Repositories.Interfaces;
using POOfase1trabalho13_11.Models.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace POOfase1trabalho13_11.Models.Services.Implementations
{

    /// <summary>
    /// Implementa a lógica de gestão e consulta do histórico de auditoria (Logs).
    /// Permite a filtragem de eventos por entidade, operação e manutenção preventiva dos dados.
    /// </summary>
    public class RegistoService : IRegistoService
    {

        #region Fields
        private readonly IRegistoRepository _registoRepo;
        #endregion

        #region Constructors

        /// <summary>
        /// Inicializa o serviço de registos injetando o repositório de persistência.
        /// </summary>
        public RegistoService(IRegistoRepository registoRepo)
        {
            _registoRepo = registoRepo;
        }
        #endregion


        #region Public Methods

        /// <summary>
        /// Obtém todos os registos de auditoria ordenados do mais recente para o mais antigo.
        /// </summary>
        public IEnumerable<Registo> ObterTodosLogs()
        {
            return _registoRepo.GetAll().OrderByDescending(r => r.DataRegisto);
        }


        /// <summary>
        /// Filtra o histórico de ações realizadas sobre uma instância específica de uma entidade.
        /// </summary>
        /// <param name="tipoEntidade">Nome da classe (ex: "Alojamento", "Cliente").</param>
        /// <param name="entidadeId">ID do registo específico.</param>
        public IEnumerable<Registo> ObterLogsPorEntidade(string tipoEntidade, int entidadeId)
        {
            return _registoRepo.GetByEntidade(tipoEntidade, entidadeId)
                               .OrderByDescending(r => r.DataRegisto);
        }


        /// <summary>
        /// Filtra o histórico por tipo de operação (CREATE, UPDATE, DELETE, etc).
        /// </summary>
        public IEnumerable<Registo> ObterLogsPorOperacao(string operacao)
        {
            return _registoRepo.GetAll()
                               .Where(r => r.Operacao.Equals(operacao, StringComparison.OrdinalIgnoreCase))
                               .OrderByDescending(r => r.DataRegisto);
        }


        /// <summary>
        /// Obtém uma lista reduzida contendo apenas os últimos eventos do sistema.
        /// </summary>
        /// <param name="quantidade">Número de registos a retornar.</param>
        public IEnumerable<Registo> ObterLogsRecentes(int quantidade)
        {
            return _registoRepo.GetAll()
                               .OrderByDescending(r => r.DataRegisto)
                               .Take(quantidade);
        }

        /// <summary>
        /// Realiza a manutenção do suporte de armazenamento, eliminando registos anteriores a uma determinada data.
        /// </summary>
        /// <param name="dataCorte">Data limite; registos mais antigos que esta data serão removidos.</param>
        /// <param name="mensagem">Feedback sobre o resultado da operação.</param>
        /// <returns>True se a limpeza foi processada; False em caso de erro técnico.</returns>
        public bool LimparLogsAntigos(DateTime dataCorte, out string mensagem)
        {
            try
            {
                // O repositório não permite remover logs individuais; usa a operação de retenção
                int contador = _registoRepo.RemoverAnterioresA(dataCorte);

                mensagem = $"Limpeza concluída. {contador} registos antigos foram removidos.";
                return true;
            }
            catch (Exception ex)
            {
                mensagem = $"Erro na manutenção de logs: {ex.Message}";
                return false;
            }
        }
        #endregion
    }
}