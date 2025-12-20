using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Enums;
using POOfase1trabalho13_11.Models.Repositories.Interfaces;

namespace POOfase1trabalho13_11.Models.Repositories.Implementations 
{
    // AlojamentoRepository herda do nosso JsonRepository genérico
    public class AlojamentoRepository : JsonRepository<Alojamento>, IAlojamentoRepository
    {


        
        public AlojamentoRepository() : base("alojamentos.json")
        {
        }
        #region Consultas Específicas

        

        /// <summary>
        /// Retorna todos os alojamentos disponíveis.
        /// </summary>
        public IEnumerable<Alojamento> BuscarDisponiveis()
        {
            return Items.Where(a => a.Estado == EstadoAlojamento.Disponivel);
        }


        public override bool Save(Alojamento alojamento)
        {
            if (alojamento == null) return false;

            // Se é novo (ID <= 0) ou se não existe na lista atual
            if (alojamento.Id <= 0 || GetById(alojamento.Id) == null)
            {
                Add(alojamento); 
                return true;
            }

            // Se já existe, força o Update e consequentemente o SaveToFile
            return Update(alojamento);
        }
        /// <summary>
        /// Busca um alojamento pelo nome.
        /// </summary>
        public Alojamento? BuscarPorNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome)) return null;

            return Items.FirstOrDefault(a => a.Nome.Equals(nome, System.StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Busca todos os alojamentos de um determinado tipo.
        /// </summary>
        public IEnumerable<Alojamento> FiltrarPorTipo(TipoAlojamento tipo)
        {
            return Items.Where(a => a.Tipo == tipo);
        }

        public IEnumerable<Alojamento> FiltrarPorCapacidadeMinima(int capacidadeMinima)
        {
            return Items.Where(a => a.Capacidade >= capacidadeMinima);
        }
        #endregion 

        #region Overrides (Customização da Base)
        // 2. CREATE (Adicionar com Atribuição de ID) - Sobrescreve o método base
        // No AlojamentoRepository.cs
        public override void Add(Alojamento alojamento)
        {
            // 1. Verificação de segurança para IDs manuais
            if (alojamento.Id > 0 && Items.Any(a => a.Id == alojamento.Id))
                throw new InvalidOperationException($"O ID {alojamento.Id} já está em uso.");

            // 2. Auto-incremento robusto
            if (alojamento.Id <= 0)
            {
                alojamento.Id = Items.Any() ? Items.Max(a => a.Id) + 1 : 1;
            }

            base.Add(alojamento); // Chama o Add da base que já faz o SaveToFile com Streaming
        }
        #endregion
    }
}