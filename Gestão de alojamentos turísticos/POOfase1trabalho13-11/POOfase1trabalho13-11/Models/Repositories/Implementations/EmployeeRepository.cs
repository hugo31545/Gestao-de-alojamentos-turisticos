using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Repositories.Implementations;
using POOfase1trabalho13_11.Models.Repositories.Interfaces;

namespace POOfase1trabalho13_11.Models.Repositories
{
    /// <summary>
    /// Repositório especializado para a gestão de funcionários em formato JSON.
    /// Herda a lógica base de persistência do JsonRepository.
    /// </summary>
    public class EmployeeRepository : JsonRepository<Employee>, IEmployeeRepository
    {
        #region Constructors

        public EmployeeRepository() : base("employees.json")
        {}

        #endregion

        #region Public Methods

        /// <summary>
        /// Implementação do método Save. Decidimos se adicionamos um novo ou atualizamos um existente.
        /// </summary>
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

        public Employee? GetByEmail(string email)
        {
            return Items.FirstOrDefault(e => e.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        #endregion
    }
}