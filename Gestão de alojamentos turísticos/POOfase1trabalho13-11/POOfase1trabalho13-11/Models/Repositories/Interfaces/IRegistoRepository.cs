using POOfase1trabalho13_11.Models.Entities;

namespace POOfase1trabalho13_11.Models.Repositories.Interfaces
{
    public interface IRegistoRepository : IRepository<Registo>
    {
        IEnumerable<Registo> GetByEntidade(string tipoEntidade, int entidadeId);
        IEnumerable<Registo> GetByPeriodo(DateTime inicio, DateTime fim);
        IEnumerable<Registo> GetByUtilizador(string utilizador);
    }
}