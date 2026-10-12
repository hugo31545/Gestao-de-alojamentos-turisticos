//-----------------------------------------------------------------
//    <copyright file="ClienteService.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Repositories.Interfaces;
using POOfase1trabalho13_11.Models.Services.Interfaces;
using POOfase1trabalho13_11.Models.Validation;

namespace POOfase1trabalho13_11.Models.Services.Implementations
{

    /// <summary>
    /// Implementa a lógica de negócio para a gestão de clientes.
    /// Controla regras de unicidade de NIF e Email, auditoria de alterações e persistência.
    /// </summary>
    public class ClienteService : IClienteService
    {

        #region Fields
        private readonly IClienteRepository _clienteRepository;
        private readonly IRegistoRepository _registoRepository;
        private readonly IReservaRepository _reservaRepository;
        private readonly ISessionService? _sessao;

        /// <summary>
        /// Autor registado na auditoria: o utilizador com sessão iniciada ou "Sistema".
        /// </summary>
        private string Autor => _sessao?.NomeParaAuditoria ?? "Sistema";
        #endregion


        #region Constructors

        /// <summary>
        /// Inicializa uma nova instância de <see cref="ClienteService"/> injetando os repositórios necessários.
        /// </summary>
        public ClienteService(IClienteRepository clienteRepository, IRegistoRepository registoRepository, IReservaRepository reservaRepository, ISessionService? sessao = null)
        {
            _sessao = sessao;
            _clienteRepository = clienteRepository;
            _registoRepository = registoRepository;
            _reservaRepository = reservaRepository;
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Obtém a listagem total de clientes registados.
        /// </summary>
        public IEnumerable<Cliente> ObterTodos() => _clienteRepository.GetAll();


        /// <summary>
        /// Procura um cliente pelo seu identificador único.
        /// </summary>
        public Cliente? ObterPorId(int id) => _clienteRepository.GetById(id);


        /// <summary>
        /// Procura um cliente através do NIF.
        /// </summary>
        public Cliente? ObterPorNif(string nif) => _clienteRepository.GetByNif(nif);


        /// <summary>
        /// Regista um novo cliente no sistema após validar a unicidade do NIF e Email.
        /// </summary>
        /// <param name="novoCliente">Objeto com os dados do cliente.</param>
        /// <param name="mensagem">Mensagem de feedback para o utilizador.</param>
        /// <returns>True se o registo for concluído com sucesso.</returns>
        public bool RegistarCliente(Cliente novoCliente, out string mensagem)
        {
            // 0. Validação de formato (NIF com 9 dígitos, email válido)
            if (!ValidarFormato(novoCliente, out mensagem)) return false;

            // 1. Validação de Unicidade de NIF
            if (_clienteRepository.GetByNif(novoCliente.Nif) != null)
            {
                mensagem = "Já existe um cliente registado com este NIF.";
                return false;
            }

            // 2. Validação de Unicidade de Email
            if (_clienteRepository.GetByEmail(novoCliente.Email) != null)
            {
                mensagem = "Este endereço de e-mail já está em uso.";
                return false;
            }

            try
            {
                // Garantir que a data é definida no momento do registo
                novoCliente.DataRegisto = DateTime.Now;

                _clienteRepository.Add(novoCliente);

                // Auditoria automática
                _registoRepository.Add(new Registo(0, "Cliente",
                    $"Novo cliente: {novoCliente.Nome} (NIF: {novoCliente.Nif})",
                    Autor, "CREATE", novoCliente.Id, DateTime.Now));

                mensagem = "Cliente registado com sucesso.";
                return true;
            }
            catch (Exception ex)
            {
                mensagem = $"Erro técnico: {ex.Message}";
                return false;
            }
        }


        /// <summary>
        /// Atualiza os dados de um cliente existente, prevenindo conflitos de NIF com outros registos.
        /// </summary>
        public bool AtualizarDados(Cliente cliente, out string mensagem)
        {
            if (!ValidarFormato(cliente, out mensagem)) return false;

            // Validação: O NIF pode existir, mas tem de pertencer ao próprio cliente que está a ser editado
            var nifExistente = _clienteRepository.GetByNif(cliente.Nif);
            if (nifExistente != null && nifExistente.Id != cliente.Id)
            {
                mensagem = "O NIF indicado já pertence a outro cliente.";
                return false;
            }

            // O mesmo para o email
            var emailExistente = _clienteRepository.GetByEmail(cliente.Email);
            if (emailExistente != null && emailExistente.Id != cliente.Id)
            {
                mensagem = "Este endereço de e-mail já pertence a outro cliente.";
                return false;
            }

            // Usa o método Update do repositório base
            if (_clienteRepository.Update(cliente))
            {
                _registoRepository.Add(new Registo(0, "Cliente",
                    $"Atualização cliente ID: {cliente.Id}",
                    Autor, "UPDATE", cliente.Id, DateTime.Now));

                mensagem = "Dados atualizados com sucesso.";
                return true;
            }

            mensagem = "Erro ao atualizar: Cliente não encontrado.";
            return false;
        }


        /// <summary>
        /// Remove um cliente do sistema e regista a ação na auditoria.
        /// </summary>
        /// <summary>
        /// Valida o formato dos dados do cliente (a unicidade é verificada à parte).
        /// </summary>
        private static bool ValidarFormato(Cliente? cliente, out string mensagem)
        {
            if (cliente == null || string.IsNullOrWhiteSpace(cliente.Nome))
            {
                mensagem = "O nome do cliente é obrigatório.";
                return false;
            }
            if (!Validador.NifValido(cliente.Nif))
            {
                mensagem = "O NIF deve ter exatamente 9 dígitos.";
                return false;
            }
            if (!Validador.EmailValido(cliente.Email))
            {
                mensagem = "O email não tem um formato válido (ex.: nome@dominio.pt).";
                return false;
            }

            mensagem = string.Empty;
            return true;
        }

        public bool RemoverCliente(int id, out string mensagem)
        {
            // Não remover clientes com reservas ativas (ficariam reservas sem cliente)
            if (_reservaRepository.FiltrarPorCliente(id).Any(r => r.EstaAtiva))
            {
                mensagem = "Não é possível remover: o cliente tem reservas ativas (pendentes, confirmadas ou com check-in).";
                return false;
            }

            bool sucesso = _clienteRepository.Remove(id);
            if (sucesso)
            {
                _registoRepository.Add(new Registo(0, "Cliente", $"Remoção do cliente ID: {id}", Autor, "DELETE", id, DateTime.Now));
                mensagem = "Cliente removido com sucesso.";
            }
            else
            {
                mensagem = "Cliente não encontrado ou não pode ser removido.";
            }

            return sucesso;
        }
        #endregion
    }
}