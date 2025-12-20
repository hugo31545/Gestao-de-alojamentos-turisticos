using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace POOfase1trabalho13_11.Models.Repositories.Implementations
{
    public class ConsultaRepository : JsonRepository<Consulta>, IConsultaRepository
    {
        public ConsultaRepository() : base("historico_consultas.json") { }

        public IEnumerable<Consulta> GetByUtilizador(string utilizador) =>
            Items.Where(c => c.Utilizador.Equals(utilizador, StringComparison.OrdinalIgnoreCase));

        public IEnumerable<Consulta> GetByTipo(string tipo) =>
            Items.Where(c => c.Tipo.Equals(tipo, StringComparison.OrdinalIgnoreCase));

        public override void Add(Consulta consulta)
        {
            // Lógica de auto-incremento
            if (consulta.Id <= 0)
                consulta.Id = Items.Any() ? Items.Max(c => c.Id) + 1 : 1;

            base.Add(consulta);
        }
    }
}