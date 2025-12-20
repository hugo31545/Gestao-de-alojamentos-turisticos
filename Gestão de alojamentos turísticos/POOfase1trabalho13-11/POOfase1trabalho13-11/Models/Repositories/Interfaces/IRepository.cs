using POOfase1trabalho13_11.Models.Entities.Interfaces;

namespace POOfase1trabalho13_11.Models.Repositories.Interfaces
{
    /// <summary>
    /// Interface genérica para repositórios, definindo operações CRUD básicas.
    /// </summary>
    /// <typeparam name="T">Tipo da entidade</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Adiciona uma nova entidade ao repositório.
        /// </summary>
        void Add(T entity);

        /// <summary>
        /// Atualiza uma entidade existente no repositório.
        /// </summary>
        bool Update(T entity);

        /// <summary>
        /// Remove uma entidade do repositório.
        /// </summary>
        bool Remove(int id);

       

        /// <summary>
        /// Obtém todas as entidades do repositório.
        /// </summary>
        IEnumerable<T> GetAll();

        T? GetById(int id); 

        bool Save(T entity);
    }
}
