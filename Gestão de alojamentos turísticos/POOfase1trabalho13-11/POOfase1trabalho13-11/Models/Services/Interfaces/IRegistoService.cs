using System;
using System.Collections.Generic;
using POOfase1trabalho13_11.Models.Entities;

namespace POOfase1trabalho13_11.Models.Services.Interfaces
{
    public interface IRegistoService
    {
        // Consultas de Auditoria
        IEnumerable<Registo> ObterTodosLogs();
        IEnumerable<Registo> ObterLogsPorEntidade(string tipoEntidade, int entidadeId);
        IEnumerable<Registo> ObterLogsPorOperacao(string operacao);
        IEnumerable<Registo> ObterLogsRecentes(int quantidade);

        
        bool LimparLogsAntigos(DateTime dataCorte, out string mensagem);
    }
}