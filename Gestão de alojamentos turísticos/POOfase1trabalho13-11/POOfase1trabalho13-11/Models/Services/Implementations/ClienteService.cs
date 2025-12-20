using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Repositories.Interfaces;
using POOfase1trabalho13_11.Models.Services.Interfaces;

namespace POOfase1trabalho13_11.Models.Services.Implementations
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IRegistoRepository _registoRepository;

        public ClienteService(IClienteRepository clienteRepository, IRegistoRepository registoRepository)
        {
            _clienteRepository = clienteRepository;
            _registoRepository = registoRepository;
        }

        public IEnumerable<Cliente> ObterTodos() => _clienteRepository.GetAll();

        public Cliente? ObterPorId(int id) => _clienteRepository.GetById(id);

        public Cliente? ObterPorNif(string nif) => _clienteRepository.GetByNif(nif);

        public bool RegistarCliente(Cliente novoCliente, out string mensagem)
        {
            // 1. Validação de Unicidade
            if (_clienteRepository.GetByNif(novoCliente.Nif) != null)
            {
                mensagem = "Já existe um cliente registado com este NIF.";
                return false;
            }

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

                _registoRepository.Add(new Registo(0, "Cliente",
                    $"Novo cliente: {novoCliente.Nome} (NIF: {novoCliente.Nif})",
                    "Sistema", "CREATE", novoCliente.Id, DateTime.Now));

                mensagem = "Cliente registado com sucesso.";
                return true;
            }
            catch (Exception ex)
            {
                mensagem = $"Erro técnico: {ex.Message}";
                return false;
            }
        }

        public bool AtualizarDados(Cliente cliente, out string mensagem)
        {
            // Validação de NIF duplicado em outros clientes
            var nifExistente = _clienteRepository.GetByNif(cliente.Nif);
            if (nifExistente != null && nifExistente.Id != cliente.Id)
            {
                mensagem = "O NIF indicado já pertence a outro cliente.";
                return false;
            }

            // Usa o método Update do repositório base
            if (_clienteRepository.Update(cliente))
            {
                _registoRepository.Add(new Registo(0, "Cliente",
                    $"Atualização cliente ID: {cliente.Id}",
                    "Sistema", "UPDATE", cliente.Id, DateTime.Now));

                mensagem = "Dados atualizados com sucesso.";
                return true;
            }

            mensagem = "Erro ao atualizar: Cliente não encontrado.";
            return false;
        }

        public bool RemoverCliente(int id, out string mensagem)
        {
            

            bool sucesso = _clienteRepository.Remove(id);
            if (sucesso)
            {
                _registoRepository.Add(new Registo(0, "Cliente", $"Remoção do cliente ID: {id}", "Sistema", "DELETE", id, DateTime.Now));
                mensagem = "Cliente removido com sucesso.";
            }
            else
            {
                mensagem = "Cliente não encontrado ou não pode ser removido.";
            }

            return sucesso;
        }
    }
}