using System.Collections.Generic;
using POOfase1trabalho13_11.Models.Entities;

namespace POOfase1trabalho13_11.Models.Services.Interfaces
{
    /// <summary>
    /// Define as regras de negócio para a gestão de clientes.
    /// </summary>
    public interface IClienteService
    {
        /// <summary>
        /// Obtém a lista completa de clientes registados.
        /// </summary>
        IEnumerable<Cliente> ObterTodos();

        /// <summary>
        /// Procura um cliente pelo seu identificador único.
        /// </summary>
        Cliente? ObterPorId(int id);

        /// <summary>
        /// Procura um cliente pelo Número de Identificação Fiscal.
        /// </summary>
        Cliente? ObterPorNif(string nif);

        /// <summary>
        /// Regista um novo cliente no sistema após validações de NIF e E-mail.
        /// </summary>
        /// <param name="novoCliente">Dados do cliente a registar.</param>
        /// <param name="mensagem">Mensagem de feedback sobre o sucesso ou motivo da falha.</param>
        /// <returns>True se o registo foi concluído.</returns>
        bool RegistarCliente(Cliente novoCliente, out string mensagem);

        /// <summary>
        /// Atualiza os dados de um cliente existente.
        /// </summary>
        /// <param name="cliente">Dados atualizados.</param>
        /// <param name="mensagem">Mensagem de feedback.</param>
        /// <returns>True se a atualização foi persistida.</returns>
        bool AtualizarDados(Cliente cliente, out string mensagem);

        /// <summary>
        /// Remove um cliente do sistema.
        /// </summary>
        /// <param name="id">ID do cliente a remover.</param>
        /// <param name="mensagem">Mensagem de feedback.</param>
        /// <returns>True se a remoção foi concluída.</returns>
        bool RemoverCliente(int id, out string mensagem);
    }
}