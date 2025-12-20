using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace POOfase1trabalho13_11.Models.Repositories.Implementations
{
    public class ClienteRepository : JsonRepository<Cliente>, IClienteRepository
    {
        public ClienteRepository() : base("clientes.json")
        {
        }

        public Cliente? GetByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;
            return Items.FirstOrDefault(c => c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public Cliente? GetByNif(string nif)
        {
         
            return Items.FirstOrDefault(c => c.Nif == nif);
        }

        public IEnumerable<Cliente> FiltrarPorNome(string nomeParcial)
        {
            if (string.IsNullOrWhiteSpace(nomeParcial)) return Enumerable.Empty<Cliente>();
            // Filtra na lista em memória
            return Items.Where(c => c.Nome.Contains(nomeParcial, StringComparison.OrdinalIgnoreCase));
        }

        #region Save Logic
        /// <summary>
        /// Atalho para a ViewModel: Decide se Adiciona ou Atualiza o cliente.
        /// </summary>
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
        // Sobrescrevemos o Add para garantir IDs automáticos para novos clientes
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