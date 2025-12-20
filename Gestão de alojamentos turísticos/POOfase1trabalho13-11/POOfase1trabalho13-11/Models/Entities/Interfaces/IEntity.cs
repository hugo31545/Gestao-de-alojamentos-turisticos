using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POOfase1trabalho13_11.Models.Entities.Interfaces
{
    /// <summary>
    /// Contrato base para todas as entidades que possuem uma identidade única.
    /// </summary>
    public interface IEntity
    {
        int Id { get; set; }
    }
}
