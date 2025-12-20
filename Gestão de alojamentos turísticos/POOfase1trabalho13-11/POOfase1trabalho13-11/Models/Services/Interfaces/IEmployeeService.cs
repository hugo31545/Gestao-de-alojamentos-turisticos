using POOfase1trabalho13_11.Models.Entities;
using System.Collections.Generic;

namespace POOfase1trabalho13_11.Models.Services.Interfaces
{
    public interface IEmployeeService
    {
        IEnumerable<Employee> ObterTodos();
        Employee? ObterPorId(int id);
        
        
        bool GuardarFuncionario(Employee funcionario, out string mensagem);
        bool RemoverFuncionario(int id, out string mensagem);
        bool AtualizarDados(Employee employee, out string mensagem);
        bool VerificarNifDuplicado(string nif);
        bool AdicionarFuncionario(string nome, string email, string nif, string cargo);
    }
}