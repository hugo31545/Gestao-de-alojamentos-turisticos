using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Repositories.Interfaces;
using System.Linq;

namespace POOfase1trabalho13_11.Models.Repositories.Implementations
{
    public class CheckInRepository : JsonRepository<CheckIn>, ICheckInRepository
    {
        public CheckInRepository() : base("checkins.json") { }

        public CheckIn? GetByReservaId(int reservaId)
        {
            return Items.FirstOrDefault(c => c.ReservaId == reservaId);
        }

        public override void Add(CheckIn checkIn)
        {
            if (checkIn.Id <= 0)
                checkIn.Id = Items.Any() ? Items.Max(c => c.Id) + 1 : 1;

            base.Add(checkIn);
        }
    }
}