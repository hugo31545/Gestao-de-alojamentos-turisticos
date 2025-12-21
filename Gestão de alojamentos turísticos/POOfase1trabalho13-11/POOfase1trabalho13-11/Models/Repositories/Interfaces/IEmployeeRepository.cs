//-----------------------------------------------------------------
//    <copyright file="IEmployeeRepository.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.Models.Entities;
namespace POOfase1trabalho13_11.Models.Repositories.Interfaces
{
    /// <summary>
    /// Define o contrato para as operações de acesso a dados da entidade <see cref="Employee"/>.
    /// Estende a interface genérica <see cref="IRepository{T}"/> para incluir lógica de persistência específica.
    /// </summary>
    public interface IEmployeeRepository : IRepository<Employee>
    {

        #region Methods
        /// <summary>
        /// Guarda um funcionário no sistema, decidindo internamente 
        /// se deve criar um novo registo ou atualizar um existente com base no ID.
        /// </summary>
        /// <param name="employee">O objeto funcionário (<see cref="Employee"/>) a persistir.</param>
        /// <returns>True se a operação foi bem-sucedida; caso contrário, false.</returns>
        bool Save(Employee employee);

        #endregion
    }
}