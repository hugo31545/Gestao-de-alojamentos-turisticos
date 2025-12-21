//-----------------------------------------------------------------
//    <copyright file="AlojamentoRepository.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------

using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Enums;
using POOfase1trabalho13_11.Models.Repositories.Interfaces;

namespace POOfase1trabalho13_11.Models.Repositories.Implementations 
{
    /// <summary>
    /// Repositório especializado para a gestão de objetos do tipo <see cref="Alojamento"/>.
    /// Estende a funcionalidade genérica de <see cref="JsonRepository{T}"/> para incluir pesquisas específicas.
    /// </summary>
    public class AlojamentoRepository : JsonRepository<Alojamento>, IAlojamentoRepository
    {


        /// <summary>
        /// Inicializa uma nova instância de <see cref="AlojamentoRepository"/> definindo o ficheiro de destino.
        /// </summary>
        public AlojamentoRepository() : base("alojamentos.json")
        {
        }
        #region Consultas Específicas



        /// <summary>
        /// Obtém todos os alojamentos que se encontram no estado <see cref="EstadoAlojamento.Disponivel"/>.
        /// </summary>
        /// <returns>Uma coleção de alojamentos disponíveis.</returns>
        public IEnumerable<Alojamento> BuscarDisponiveis()
        {
            return Items.Where(a => a.Estado == EstadoAlojamento.Disponivel);
        }

        /// <summary>
        /// Salva um alojamento, decidindo se deve adicionar um novo ou atualizar um existente.
        /// </summary>
        /// <param name="alojamento">O objeto alojamento a persistir.</param>
        /// <returns>Verdadeiro se a operação for bem-sucedida.</returns>
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
        /// Procura um alojamento através do seu nome, ignorando maiúsculas e minúsculas.
        /// </summary>
        /// <param name="nome">O nome a pesquisar.</param>
        /// <returns>O alojamento encontrado ou null caso não exista.</returns>
        public Alojamento? BuscarPorNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome)) return null;

            return Items.FirstOrDefault(a => a.Nome.Equals(nome, System.StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Filtra a lista de alojamentos com base no tipo especificado.
        /// </summary>
        /// <param name="tipo">O <see cref="TipoAlojamento"/> pretendido.</param>
        /// <returns>Coleção de alojamentos do tipo indicado.</returns>
        public IEnumerable<Alojamento> FiltrarPorTipo(TipoAlojamento tipo)
        {
            return Items.Where(a => a.Tipo == tipo);
        }


        /// <summary>
        /// Obtém alojamentos que suportem, pelo menos, o número de hóspedes indicado.
        /// </summary>
        /// <param name="capacidadeMinima">Capacidade mínima necessária.</param>
        /// <returns>Coleção de alojamentos que cumprem o requisito.</returns>
        public IEnumerable<Alojamento> FiltrarPorCapacidadeMinima(int capacidadeMinima)
        {
            return Items.Where(a => a.Capacidade >= capacidadeMinima);
        }
        #endregion 

        #region Overrides (Customização da Base)
        /// <summary>
        /// Adiciona um novo alojamento à lista e atribui automaticamente um ID único (auto-incremento).
        /// </summary>
        /// <param name="alojamento">O alojamento a adicionar.</param>
        /// <exception cref="InvalidOperationException">Lançada se o ID fornecido manualmente já estiver em uso.</exception>
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