using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Repositories.Interfaces;

namespace POOfase1trabalho13_11.Models.Repositories.Implementations
{
    public class RegistoRepository : JsonRepository<Registo>, IRegistoRepository
    {
        public RegistoRepository() : base("auditoria_logs.json") { }

        public IEnumerable<Registo> GetByEntidade(string tipoEntidade, int entidadeId) =>
            Items.Where(r => r.TipoEntidade.Equals(tipoEntidade, StringComparison.OrdinalIgnoreCase) && r.EntidadeId == entidadeId);

        public IEnumerable<Registo> GetByPeriodo(DateTime inicio, DateTime fim) =>
            Items.Where(r => r.DataRegisto >= inicio && r.DataRegisto <= fim);

        public IEnumerable<Registo> GetByUtilizador(string utilizador) =>
            Items.Where(r => r.Utilizador.Equals(utilizador, StringComparison.OrdinalIgnoreCase));

        public override bool Update(Registo entity) =>
            throw new NotSupportedException("Logs de auditoria não podem ser alterados.");

        public override bool Remove(int id) =>
            throw new NotSupportedException("Logs de auditoria não podem ser removidos.");

        public override void Add(Registo log)
        {
            // Auto-incremento de segurança para logs
            if (log.Id <= 0)
                log.Id = Items.Any() ? Items.Max(r => r.Id) + 1 : 1;

            base.Add(log);
        }

    }
}