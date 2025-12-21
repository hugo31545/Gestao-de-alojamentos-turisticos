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

        #region Public Methods

        /// <summary>
        /// Guarda um funcionário no sistema. Decide automaticamente entre a criação de um novo registo 
        /// (com atribuição de ID) ou a atualização de um registo existente.
        /// </summary>
        /// <param name="employee">O objeto funcionário a persistir.</param>
        /// <returns>Verdadeiro se a operação for concluída com sucesso; falso caso contrário.</returns>
        public override bool Save(Employee employee)
        {
            if (employee == null) return false;

            // Se o ID for 0 ou não existir na lista, é um novo funcionário
            if (employee.Id == 0 || GetById(employee.Id) == null)
            {
                // Garante a geração de ID se for 0
                if (employee.Id == 0)
                {
                    employee.Id = Items.Any() ? Items.Max(e => e.Id) + 1 : 1;
                }
                Add(employee); 
                return true;
            }

            return Update(employee); 
        }



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