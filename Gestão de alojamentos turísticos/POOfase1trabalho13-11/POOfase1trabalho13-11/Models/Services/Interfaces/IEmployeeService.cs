//-----------------------------------------------------------------
//    <copyright file="IEmployeeService.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>1</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.Models.Entities;
using System.Collections.Generic;

namespace POOfase1trabalho13_11.Models.Services.Interfaces
{

    /// <summary>
    /// Define o contrato para os serviços de gestão de funcionários (Employees).
    /// Esta interface centraliza a lógica de admissão, atualização de cargos e 
    /// verificações de integridade de dados do pessoal.
    /// </summary>
    public interface IEmployeeService
    {

        #region Read Operations
        /// <summary>
        /// Recupera a listagem completa de todos os funcionários ativos e registados.
        /// </summary>
        /// <returns>Uma coleção de <see cref="Employee"/>.</returns>
        IEnumerable<Employee> ObterTodos();

        /// <summary>
        /// Procura um funcionário específico através do seu identificador único.
        /// </summary>
        /// <param name="id">O ID do funcionário.</param>
        /// <returns>A instância de <see cref="Employee"/> se encontrada; caso contrário, null.</returns>
        Employee? ObterPorId(int id);

        /// <summary>
        /// Valida se um determinado NIF já está associado a outro funcionário, 
        /// prevenindo duplicados legais.
        /// </summary>
        /// <param name="nif">O número de identificação fiscal a validar.</param>
        /// <returns>True se o NIF já existir no sistema.</returns>
        bool VerificarNifDuplicado(string nif);

        /// <summary>
        /// Valida os dados de um novo funcionário antes de qualquer gravação:
        /// campos obrigatórios, formato do NIF e do email, e unicidade de NIF e email.
        /// </summary>
        /// <param name="nome">Nome do funcionário.</param>
        /// <param name="email">Email (também usado como nome de utilizador).</param>
        /// <param name="nif">NIF com 9 dígitos.</param>
        /// <param name="mensagem">Motivo da falha, se existir.</param>
        /// <returns>True se os dados forem válidos.</returns>
        bool ValidarNovoFuncionario(string nome, string email, string nif, out string mensagem);
        #endregion

        #region Write Operations

        /// <summary>
        /// Persiste ou atualiza um objeto funcionário completo, validando regras de integridade.
        /// </summary>
        /// <param name="funcionario">A entidade <see cref="Employee"/> a guardar.</param>
        /// <param name="mensagem">Feedback detalhado sobre o resultado da operação.</param>
        /// <returns>True se a operação foi concluída com sucesso.</returns>
        bool GuardarFuncionario(Employee funcionario, out string mensagem);


        /// <summary>
        /// Remove o registo de um funcionário do sistema.
        /// </summary>
        /// <param name="id">ID do funcionário a remover.</param>
        /// <param name="mensagem">Mensagem indicando o sucesso ou motivo do impedimento.</param>
        /// <returns>True se a remoção foi efetuada.</returns>
        bool RemoverFuncionario(int id, out string mensagem);


        /// <summary>
        /// Atualiza os dados biográficos ou profissionais de um funcionário existente.
        /// </summary>
        /// <param name="employee">Objeto com os dados atualizados.</param>
        /// <param name="mensagem">Mensagem de feedback para o utilizador.</param>
        /// <returns>True se a atualização foi persistida.</returns>
        bool AtualizarDados(Employee employee, out string mensagem);

        /// <summary>
        /// Adiciona um novo funcionário ao sistema através de dados simplificados.
        /// </summary>
        /// <param name="nome">Nome completo.</param>
        /// <param name="email">Endereço de correio eletrónico profissional.</param>
        /// <param name="nif">Número de Identificação Fiscal.</param>
        /// <param name="cargo">Cargo ou função desempenhada.</param>
        /// <returns>True se a criação foi bem-sucedida.</returns>
        bool AdicionarFuncionario(string nome, string email, string nif, string cargo);
        #endregion
    }
}