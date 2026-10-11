//-----------------------------------------------------------------
//    <copyright file="IRepository.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.Models.Entities.Interfaces;

namespace POOfase1trabalho13_11.Models.Repositories.Interfaces
{
    /// <summary>
    /// Interface genérica que define o contrato base para operações CRUD (Create, Read, Update, Delete).
    /// Esta abstração permite a gestão padronizada de qualquer entidade do sistema.
    /// </summary>
    /// <typeparam name="T">O tipo da entidade que deve ser uma classe.</typeparam>
    public interface IRepository<T> where T : class
    {

        #region Methods
        /// <summary>
        /// Adiciona uma nova instância da entidade ao suporte de dados.
        /// </summary>
        /// <param name="entity">O objeto a ser adicionado.</param>
        void Add(T entity);

        /// <summary>
        /// Atualiza os dados de uma entidade já existente no sistema.
        /// </summary>
        /// <param name="entity">O objeto com os dados atualizados.</param>
        /// <returns><c>true</c> se a atualização for bem-sucedida; caso contrário, <c>false</c>.</returns>
        bool Update(T entity);

        /// <summary>
        /// Remove permanentemente uma entidade do sistema através do seu identificador único.
        /// </summary>
        /// <param name="id">O ID da entidade a remover.</param>
        /// <returns><c>true</c> se a remoção for concluída; caso contrário, <c>false</c>.</returns>
        bool Remove(int id);



        /// <summary>
        /// Obtém a listagem completa de todas as entidades armazenadas.
        /// </summary>
        /// <returns>Uma coleção enumerável contendo todos os registos do tipo <typeparamref name="T"/>.</returns>
        IEnumerable<T> GetAll();


        /// <summary>
        /// Procura e devolve uma entidade específica através do seu identificador único.
        /// </summary>
        /// <param name="id">O ID da entidade pretendida.</param>
        /// <returns>A entidade encontrada ou <c>null</c> caso não exista.</returns>
        T? GetById(int id);


        /// <summary>
        /// Persiste a entidade no sistema, decidindo automaticamente se deve
        /// realizar uma inserção (Add) ou uma atualização (Update).
        /// </summary>
        /// <param name="entity">O objeto a guardar.</param>
        /// <returns><c>true</c> se a operação de persistência for bem-sucedida.</returns>
        bool Save(T entity);
        #endregion
    }
}
