//-----------------------------------------------------------------
//    <copyright file="EmployeeService.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq; 
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Repositories.Interfaces;
using POOfase1trabalho13_11.Models.Services.Interfaces;
using POOfase1trabalho13_11.Models.Validation;

namespace POOfase1trabalho13_11.Models.Services.Implementations
{

    /// <summary>
    /// Implementa a lógica de negócio para a gestão de funcionários (Employees).
    /// Centraliza validações de NIF, gestão de cargos e auditoria de alterações.
    /// </summary>
    public class EmployeeService : IEmployeeService
    {


        #region Fields
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IRegistoRepository _registoRepository;

        #endregion

        #region Constructors

        /// <summary>
        /// Inicializa o serviço injetando as dependências de persistência e auditoria.
        /// </summary>
        public EmployeeService(IEmployeeRepository employeeRepository, IRegistoRepository registoRepository)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _registoRepository = registoRepository ?? throw new ArgumentNullException(nameof(registoRepository));
        }

        #endregion

        #region Read Operations


        /// <summary>
        /// Verifica se um NIF já se encontra atribuído a algum funcionário no sistema.
        /// </summary>
        public bool VerificarNifDuplicado(string nif)
        {
            return _employeeRepository.GetAll().Any(e => e.NIF == nif);
        }


        /// <summary>
        /// Obtém a listagem completa de funcionários.
        /// </summary>
        public IEnumerable<Employee> ObterTodos() => _employeeRepository.GetAll();


        /// <summary>
        /// Procura um funcionário específico pelo seu identificador.
        /// </summary>
        public Employee? ObterPorId(int id) => _employeeRepository.GetById(id);

        /// <summary>
        /// Valida os dados de um novo funcionário sem gravar nada.
        /// </summary>
        public bool ValidarNovoFuncionario(string nome, string email, string nif, out string mensagem)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                mensagem = "O nome é obrigatório.";
                return false;
            }

            if (!Validador.EmailValido(email))
            {
                mensagem = "O email não tem um formato válido (ex.: nome@dominio.pt).";
                return false;
            }

            if (!Validador.NifValido(nif))
            {
                mensagem = "O NIF deve ter exatamente 9 dígitos.";
                return false;
            }

            if (VerificarNifDuplicado(nif))
            {
                mensagem = "Este NIF já está registado em outro funcionário.";
                return false;
            }

            if (_employeeRepository.GetByEmail(email) != null)
            {
                mensagem = "Este email já está registado em outro funcionário.";
                return false;
            }

            mensagem = string.Empty;
            return true;
        }

        #endregion

        #region Write Operations

        /// <summary>
        /// Atalho para criação rápida de um funcionário a partir de dados básicos.
        /// Dados inválidos (ex.: NIF com formato errado) devolvem false em vez de lançar exceção.
        /// </summary>
        public bool AdicionarFuncionario(string nome, string email, string nif, string cargo)
        {
            Employee novoFuncionario;
            try
            {
                novoFuncionario = new Employee
                {
                    Nome = nome,
                    Email = email,
                    NIF = nif,
                    Cargo = cargo,
                    Id = 0 // Garante que o repositório trate como novo
                };
            }
            catch (ArgumentException)
            {
                return false;
            }

            return GuardarFuncionario(novoFuncionario, out _);
        }

        /// <summary>
        /// Regista ou atualiza um funcionário, garantindo que o NIF é exclusivo.
        /// </summary>
        /// <param name="funcionario">Objeto funcionário a persistir.</param>
        /// <param name="mensagem">Mensagem de feedback sobre a operação.</param>
        /// <returns>True se a operação foi bem-sucedida.</returns>
        public bool GuardarFuncionario(Employee funcionario, out string mensagem)
        {
            try
            {
                if (funcionario == null)
                {
                    mensagem = "Dados do funcionário são inválidos.";
                    return false;
                }

                // Validação de NIF único (Ignora o próprio registo em caso de UPDATE)
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
        /// Remove um funcionário do sistema após validação de existência.
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
        /// Atualiza os dados de um funcionário que já existe na base de dados.
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