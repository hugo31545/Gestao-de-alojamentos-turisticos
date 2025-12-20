using POOfase1trabalho13_11.Models.Entities;

namespace POOfase1trabalho13_11.Models.Repositories.Interfaces
{
    public interface IConsultaRepository : IRepository<Consulta>
    {
        IEnumerable<Consulta> GetByUtilizador(string utilizador);
        IEnumerable<Consulta> GetByTipo(string tipo);
    }
}