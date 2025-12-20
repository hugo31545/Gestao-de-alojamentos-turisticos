using POOfase1trabalho13_11.Models.Entities;
namespace POOfase1trabalho13_11.Models.Repositories.Interfaces
{
    /// <summary>
    /// Define o contrato específico para operações de dados de funcionários.
    /// Estende a interface genérica IRepository para incluir lógica de persistência específica.
    /// </summary>
    public interface IEmployeeRepository : IRepository<Employee>
    {
        /// <summary>
        /// Guarda um funcionário no sistema, decidindo internamente 
        /// se deve criar um novo ou atualizar um existente.
        /// </summary>
        /// <param name="employee">O objeto funcionário a persistir.</param>
        /// <returns>True se a operação foi bem-sucedida.</returns>
        bool Save(Employee employee);

        
    }
}