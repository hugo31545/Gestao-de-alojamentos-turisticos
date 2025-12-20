using POOfase1trabalho13_11.Models.Entities;

namespace POOfase1trabalho13_11.Models.Repositories.Interfaces
{
    public interface ICheckInRepository : IRepository<CheckIn>
    {
        CheckIn? GetByReservaId(int reservaId);
    }
}