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

    }
}