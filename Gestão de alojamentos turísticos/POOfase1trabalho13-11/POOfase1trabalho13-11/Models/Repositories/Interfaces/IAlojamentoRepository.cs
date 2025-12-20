
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Enums;

namespace POOfase1trabalho13_11.Models.Repositories.Interfaces
{
    /// <summary>
    /// Define o contrato para operações de acesso a dados de alojamentos.
    /// </summary>
    /// 
    
    public interface IAlojamentoRepository : IRepository<Alojamento>
    {

        #region Query Methods (Consultas Específicas)

        

        /// <summary>
        /// Busca um alojamento pelo nome (comparação insensível a maiúsculas/minúsculas).
        /// </summary>
        /// <param name="nome">O nome do alojamento a ser buscado.</param>
        /// <returns>O <see cref="Alojamento"/> correspondente; caso contrário, <c>null</c>.</returns>
        Alojamento? BuscarPorNome(string nome);


        /// <summary>
        /// Retorna todos os alojamentos disponíveis para reserva.
        /// </summary>
        /// <returns>Uma coleção de <see cref="Alojamento"/> com o estado Disponível.</returns>
        IEnumerable<Alojamento> BuscarDisponiveis();

        /// <summary>
        /// Retorna todos os alojamentos de um tipo específico.
        /// </summary>
        /// <param name="tipo">O <see cref="TipoAlojamento"/> para filtrar.</param>
        /// <returns>Uma coleção de <see cref="Alojamento"/> do tipo especificado.</returns>
        IEnumerable<Alojamento> FiltrarPorTipo(TipoAlojamento tipo);

        /// <summary>
        /// Retorna todos os alojamentos com capacidade mínima especificada.
        /// </summary>
        /// <param name="capacidadeMinima">O número mínimo de pessoas que o alojamento deve comportar.</param>
        /// <returns>Uma coleção de <see cref="Alojamento"/> que atende à capacidade mínima.</returns>
        IEnumerable<Alojamento> FiltrarPorCapacidadeMinima(int capacidadeMinima);

        #endregion
    }
}