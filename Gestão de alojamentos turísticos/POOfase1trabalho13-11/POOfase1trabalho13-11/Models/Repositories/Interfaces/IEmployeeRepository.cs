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
        // O método Save é herdado de IRepository<Employee>.
    }
}