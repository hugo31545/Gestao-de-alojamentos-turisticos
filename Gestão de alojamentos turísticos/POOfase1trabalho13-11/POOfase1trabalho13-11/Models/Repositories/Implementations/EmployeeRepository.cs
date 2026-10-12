//-----------------------------------------------------------------
//    <copyright file="EmployeeRepository.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Repositories.Implementations;
using POOfase1trabalho13_11.Models.Repositories.Interfaces;

namespace POOfase1trabalho13_11.Models.Repositories
{
    /// <summary>
    /// Repositório especializado para a gestão de funcionários (<see cref="Employee"/>) em formato JSON.
    /// Herda a lógica base de persistência do <see cref="JsonRepository{T}"/>.
    /// </summary>
    public class EmployeeRepository : JsonRepository<Employee>, IEmployeeRepository
    {
        #region Constructors

        /// <summary>
        /// Inicializa uma nova instância de <see cref="EmployeeRepository"/>,
        /// apontando para o ficheiro de persistência "employees.json".
        /// </summary>
        public EmployeeRepository() : base("employees.json")
        {}

        #endregion


        #region Specific Logic (Exemplo)

        /// <summary>
        /// Procura um funcionário através do seu endereço de email.
        /// </summary>
        /// <param name="email">O email a pesquisar.</param>
        /// <returns>O <see cref="Employee"/> encontrado ou null caso não exista correspondência.</returns>
        public Employee? GetByEmail(string email)
        {
            return Items.FirstOrDefault(e => e.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        #endregion
    }
}