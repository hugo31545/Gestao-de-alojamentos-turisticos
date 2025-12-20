using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Repositories.Interfaces;
using POOfase1trabalho13_11.Models.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace POOfase1trabalho13_11.Models.Services.Implementations
{
    public class RegistoService : IRegistoService
    {
        private readonly IRegistoRepository _registoRepo;

        public RegistoService(IRegistoRepository registoRepo)
        {
            _registoRepo = registoRepo;
        }

        public IEnumerable<Registo> ObterTodosLogs()
        {
            return _registoRepo.GetAll().OrderByDescending(r => r.DataRegisto);
        }

        public IEnumerable<Registo> ObterLogsPorEntidade(string tipoEntidade, int entidadeId)
        {
            return _registoRepo.GetByEntidade(tipoEntidade, entidadeId)
                               .OrderByDescending(r => r.DataRegisto);
        }

        public IEnumerable<Registo> ObterLogsPorOperacao(string operacao)
        {
            return _registoRepo.GetAll()
                               .Where(r => r.Operacao.Equals(operacao, StringComparison.OrdinalIgnoreCase))
                               .OrderByDescending(r => r.DataRegisto);
        }

        public IEnumerable<Registo> ObterLogsRecentes(int quantidade)
        {
            return _registoRepo.GetAll()
                               .OrderByDescending(r => r.DataRegisto)
                               .Take(quantidade);
        }

        /// <summary>
        /// Mantém o ficheiro JSON saudável removendo registos muito antigos.
        /// </summary>
        public bool LimparLogsAntigos(DateTime dataCorte, out string mensagem)
        {
            try
            {
                var logsParaRemover = _registoRepo.GetAll()
                                                .Where(r => r.DataRegisto < dataCorte)
                                                .ToList();

                int contador = 0;
                foreach (var log in logsParaRemover)
                {
                    if (_registoRepo.Remove(log.Id)) contador++;
                }

                mensagem = $"Limpeza concluída. {contador} registos antigos foram removidos.";
                return true;
            }
            catch (Exception ex)
            {
                mensagem = $"Erro na manutenção de logs: {ex.Message}";
                return false;
            }
        }
    }
}