//-----------------------------------------------------------------
//    <copyright file="IAlojamentoRepository.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Enums;

namespace POOfase1trabalho13_11.Models.Repositories.Interfaces
{
    /// <summary>
    /// Define o contrato para as operações de acesso a dados e lógica de pesquisa 
    /// específicas para a entidade <see cref="Alojamento"/>.
    /// Herda as operações base da interface genérica <see cref="IRepository{T}"/>.
    /// </summary>

    public interface IAlojamentoRepository : IRepository<Alojamento>
    {

        #region Query Methods (Consultas Específicas)



        /// <summary>
        /// Procura um alojamento pelo seu nome comercial.
        /// </summary>
        /// <param name="nome">O nome do alojamento a ser pesquisado.</param>
        /// <returns>O <see cref="Alojamento"/> correspondente ou <c>null</c> caso não seja encontrado.</returns>
        Alojamento? BuscarPorNome(string nome);


        /// <summary>
        /// Obtém a listagem de todos os alojamentos que se encontram prontos a ser reservados.
        /// </summary>
        /// <returns>Uma coleção de <see cref="Alojamento"/> com o estado definido como Disponível.</returns>
        IEnumerable<Alojamento> BuscarDisponiveis();

        /// <summary>
        /// Filtra os alojamentos existentes com base na sua tipologia.
        /// </summary>
        /// <param name="tipo">O <see cref="TipoAlojamento"/> (ex: Hotel, Apartamento) para filtrar.</param>
        /// <returns>Uma coleção de <see cref="Alojamento"/> pertencentes ao tipo especificado.</returns>
        IEnumerable<Alojamento> FiltrarPorTipo(TipoAlojamento tipo);

        /// <summary>
        /// Obtém alojamentos que possuam uma capacidade de hóspedes igual ou superior ao valor indicado.
        /// </summary>
        /// <param name="capacidadeMinima">O número mínimo de pessoas que o alojamento deve comportar.</param>
        /// <returns>Uma coleção de <see cref="Alojamento"/> que cumprem o requisito de capacidade.</returns>
        IEnumerable<Alojamento> FiltrarPorCapacidadeMinima(int capacidadeMinima);

        #endregion
    }
}