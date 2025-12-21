//-----------------------------------------------------------------
//    <copyright file="IClienteRepository.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.Models.Entities;
using System.Collections.Generic;

namespace POOfase1trabalho13_11.Models.Repositories.Interfaces
{
    /// <summary>
    /// Define o contrato para as operações de acesso a dados da entidade <see cref="Cliente"/>.
    /// Estende a interface genérica <see cref="IRepository{T}"/> com métodos de pesquisa 
    /// por identificadores fiscais e contactos.
    /// </summary>
    public interface IClienteRepository : IRepository<Cliente>
    {


        /// <summary>
        /// Procura um cliente através do seu Número de Identificação Fiscal (NIF).
        /// Útil para validações de registo e faturação.
        /// </summary>
        /// <param name="nif">O NIF do cliente a pesquisar.</param>
        /// <returns>O <see cref="Cliente"/> correspondente ou <c>null</c> caso não exista.</returns>
        Cliente? GetByNif(string nif);

        /// <summary>
        /// Procura um cliente através do seu endereço de correio eletrónico.
        /// </summary>
        /// <param name="email">O email do cliente a pesquisar.</param>
        /// <returns>O <see cref="Cliente"/> correspondente ou <c>null</c> caso não exista.</returns>
        Cliente? GetByEmail(string email);

        /// <summary>
        /// Obtém uma lista de clientes cujo nome contenha a sequência de carateres fornecida.
        /// </summary>
        /// <param name="nomeParcial">Parte do nome a procurar (ex: "Silva").</param>
        /// <returns>Uma coleção de clientes que satisfazem o critério de pesquisa.</returns>
        IEnumerable<Cliente> FiltrarPorNome(string nomeParcial);
    }
}