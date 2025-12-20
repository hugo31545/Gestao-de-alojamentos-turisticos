using POOfase1trabalho13_11.Models.Entities;
using System.Collections.Generic;

namespace POOfase1trabalho13_11.Models.Repositories.Interfaces
{
    /// <summary>
    /// Define as operações específicas de persistência para Clientes.
    /// Herda as operações genéricas (GetAll, Add, Update, Remove) de IRepository.
    /// </summary>
    public interface IClienteRepository : IRepository<Cliente>
    {
       

        /// <summary>
        /// Procura um cliente pelo NIF (Necessário para a validação no Service).
        /// </summary>
        Cliente? GetByNif(string nif);

        /// <summary>
        /// Procura um cliente pelo Email (Renomeado de BuscarPorEmail para GetByEmail).
        /// </summary>
        Cliente? GetByEmail(string email);

        /// <summary>
        /// Filtra clientes que contenham parte do nome.
        /// </summary>
        IEnumerable<Cliente> FiltrarPorNome(string nomeParcial);
    }
}