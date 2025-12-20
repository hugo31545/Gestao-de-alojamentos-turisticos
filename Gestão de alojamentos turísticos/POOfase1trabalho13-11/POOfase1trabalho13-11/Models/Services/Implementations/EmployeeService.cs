using System;
using System.Collections.Generic;
using System.Linq; // <-- ADICIONAR ESTE PARA O FIRSTORDEFAULT FUNCIONAR
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Repositories.Interfaces;
using POOfase1trabalho13_11.Models.Services.Interfaces;

namespace POOfase1trabalho13_11.Models.Services.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IRegistoRepository _registoRepository;

        public EmployeeService(IEmployeeRepository employeeRepository, IRegistoRepository registoRepository)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _registoRepository = registoRepository ?? throw new ArgumentNullException(nameof(registoRepository));
        }
        public bool VerificarNifDuplicado(string nif)
        {
            return _employeeRepository.GetAll().Any(e => e.NIF == nif);
        }
        #region Read Operations

        public bool AdicionarFuncionario(string nome, string email, string nif, string cargo)
        {
            var novoFuncionario = new Employee
            {
                Nome = nome,
                Email = email,
                NIF = nif,
                Cargo = cargo,
                Id = 0 // Garante que o repositório trate como novo
            };

            string msg;
            bool resultado = GuardarFuncionario(novoFuncionario, out msg);
            // Poderia registar a 'msg' em logs de sistema aqui se resultado for false
            return resultado;
        }

        public IEnumerable<Employee> ObterTodos() => _employeeRepository.GetAll();

        public Employee? ObterPorId(int id) => _employeeRepository.GetById(id);

        #endregion

        #region Write Operations
        /// <summary>
        /// Regista um novo funcionário ou atualiza um existente com validações de integridade.
        /// </summary>
        public bool GuardarFuncionario(Employee funcionario, out string mensagem)
        {
            try
            {
                if (funcionario == null)
                {
                    mensagem = "Dados do funcionário são inválidos.";
                    return false;
                }

                // Validação de NIF único (excluindo o próprio ID caso seja update)
                var existenteNif = _employeeRepository.GetAll()
                    .FirstOrDefault(e => e.NIF == funcionario.NIF && e.Id != funcionario.Id);

                if (existenteNif != null)
                {
                    mensagem = "Já existe um funcionário registado com este NIF.";
                    return false;
                }

                // Persistência
                bool eNovo = funcionario.Id == 0;
                bool sucesso = _employeeRepository.Save(funcionario);

                if (sucesso)
                {
                    // Log de Auditoria utilizando o repositório diretamente
                    _registoRepository.Add(new Registo(
                        0,
                        "Employee",
                        $"Funcionário {funcionario.Nome} ({funcionario.Cargo}) guardado.",
                        "Sistema",
                        eNovo ? "CREATE" : "UPDATE",
                        funcionario.Id,
                        DateTime.Now));

                    mensagem = "Funcionário guardado com sucesso.";
                    return true;
                }

                mensagem = "Erro ao guardar no repositório.";
                return false;
            }
            catch (Exception ex)
            {
                mensagem = $"Erro técnico: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Remove um funcionário e regista a ação na auditoria.
        /// </summary>
        public bool RemoverFuncionario(int id, out string mensagem)
        {
            var funcionario = _employeeRepository.GetById(id);
            if (funcionario == null)
            {
                mensagem = "Funcionário não encontrado.";
                return false;
            }

            bool removido = _employeeRepository.Remove(id);
            if (removido)
            {
                _registoRepository.Add(new Registo(0, "Employee",
                    $"Funcionário removido: {funcionario.Nome}",
                    "Sistema", "DELETE", id, DateTime.Now));

                mensagem = "Funcionário removido com sucesso.";
                return true;
            }

            mensagem = "Não foi possível remover o funcionário.";
            return false;
        }

        /// <summary>
        /// Atualiza os dados de um funcionário existente.
        /// </summary>
        public bool AtualizarDados(Employee employee, out string mensagem)
        {
            if (employee.Id <= 0)
            {
                mensagem = "ID de funcionário inválido para atualização.";
                return false;
            }

            // VERIFICAÇÃO EXTRA: O funcionário ainda existe no ficheiro?
            var existe = _employeeRepository.GetById(employee.Id);
            if (existe == null)
            {
                mensagem = "O funcionário que tenta editar já não existe no sistema.";
                return false;
            }

            return GuardarFuncionario(employee, out mensagem);
        }
        #endregion
    }
}