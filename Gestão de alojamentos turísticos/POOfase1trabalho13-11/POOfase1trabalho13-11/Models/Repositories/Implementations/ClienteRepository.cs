//-----------------------------------------------------------------
//    <copyright file="ClienteRepository.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace POOfase1trabalho13_11.Models.Repositories.Implementations
{

    /// <summary>
    /// Repositório especializado na gestão de dados de <see cref="Cliente"/>.
    /// Implementa operações de persistência em JSON e consultas por critérios específicos (NIF, Email).
    /// </summary>
    public class ClienteRepository : JsonRepository<Cliente>, IClienteRepository
    {

        /// <summary>
        /// Inicializa uma nova instância de <see cref="ClienteRepository"/>,
        /// carregando ou criando o ficheiro "clientes.json".
        /// </summary>
        public ClienteRepository() : base("clientes.json")
        {
        }


        /// <summary>
        /// Obtém um cliente através do seu endereço de email.
        /// </summary>
        /// <param name="email">O email a pesquisar.</param>
        /// <returns>O <see cref="Cliente"/> encontrado ou null caso não exista.</returns>
        public Cliente? GetByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;
            return Items.FirstOrDefault(c => c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Obtém um cliente através do seu Número de Identificação Fiscal (NIF).
        /// </summary>
        /// <param name="nif">O NIF a pesquisar.</param>
        /// <returns>O <see cref="Cliente"/> correspondente ou null.</returns>
        public Cliente? GetByNif(string nif)
        {
         
            return Items.FirstOrDefault(c => c.Nif == nif);
        }


        /// <summary>
        /// Filtra clientes cujo nome contenha o termo de pesquisa indicado.
        /// </summary>
        /// <param name="nomeParcial">Parte do nome a procurar.</param>
        /// <returns>Uma coleção de clientes que satisfazem o critério.</returns>
        public IEnumerable<Cliente> FiltrarPorNome(string nomeParcial)
        {
            if (string.IsNullOrWhiteSpace(nomeParcial)) return Enumerable.Empty<Cliente>();
            // Filtra na lista em memória
            return Items.Where(c => c.Nome.Contains(nomeParcial, StringComparison.OrdinalIgnoreCase));
        }

        #region Save Logic
        /// <summary>
        /// Determina se o cliente deve ser adicionado como novo ou atualizado.
        /// Interface simplificada para utilização nas ViewModels.
        /// </summary>
        /// <param name="cliente">O cliente a gravar.</param>
        /// <returns>Verdadeiro se a operação for concluída com sucesso.</returns>
        public override bool Save(Cliente cliente)
        {
            if (cliente == null) return false;

            // Se o ID é 0 ou não existe, usamos o Add (que já gera o ID automático no nosso override abaixo)
            if (cliente.Id <= 0 || GetById(cliente.Id) == null)
            {
                Add(cliente);
                return true;
            }

            return Update(cliente);
        }

        /// <summary>
        /// Adiciona um novo cliente e garante a atribuição de um ID único incremental.
        /// </summary>
        /// <param name="cliente">O cliente a adicionar.</param>
        public override void Add(Cliente cliente)
        {
            // Garantir ID único se ainda não tiver um
            if (cliente.Id <= 0)
            {
                cliente.Id = Items.Any() ? Items.Max(c => c.Id) + 1 : 1;
            }
            base.Add(cliente);
        }
        #endregion
    }
}