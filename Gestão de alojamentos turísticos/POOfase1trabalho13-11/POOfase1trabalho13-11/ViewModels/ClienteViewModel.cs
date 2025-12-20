using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Services.Interfaces;
using POOfase1trabalho13_11.ViewModels.Commands;

namespace POOfase1trabalho13_11.ViewModels
{
    public class ClienteViewModel : BaseViewModel
    {
        #region Fields
        private readonly IClienteService _clienteService;
        private ObservableCollection<Cliente> _listaClientes;
        private Cliente _clienteEmEdicao;
        private Cliente? _clienteSelecionado;
        #endregion

        #region Properties
        public ObservableCollection<Cliente> ListaClientes
        {
            get => _listaClientes;
            set { _listaClientes = value; OnPropertyChanged(nameof(ListaClientes)); }
        }

        

        public Cliente ClienteEmEdicao
        {
            get => _clienteEmEdicao;
            set
            {
                _clienteEmEdicao = value;
                OnPropertyChanged(nameof(ClienteEmEdicao));
                
                (GravarCommand as ViewModelCommand)?.RaiseCanExecuteChanged();
            }
        }

        public Cliente? ClienteSelecionado
        {
            get => _clienteSelecionado;
            set
            {
                if (SetProperty(ref _clienteSelecionado, value))
                {
                    PrepararEdicao();
                    
                    (RemoverCommand as ViewModelCommand)?.RaiseCanExecuteChanged();
                }
            }
        }
        #endregion

        #region Commands
        public ICommand GravarCommand { get; }
        public ICommand NovoCommand { get; }
        public ICommand RemoverCommand { get; }
        public ICommand LimparCommand { get; }
        #endregion

        #region Constructor
        public ClienteViewModel(IClienteService clienteService)
        {
            _clienteService = clienteService ?? throw new ArgumentNullException(nameof(clienteService));
            _listaClientes = new ObservableCollection<Cliente>();
            _clienteEmEdicao = new Cliente();

            GravarCommand = new ViewModelCommand(p => ExecutarGravar(), p => PodeGravar());
            NovoCommand = new ViewModelCommand(p => ExecutarNovo());
            LimparCommand = new ViewModelCommand(p => ExecutarNovo());
            RemoverCommand = new ViewModelCommand(p => ExecutarRemover(), p => ClienteSelecionado != null);

            CarregarDados();
        }
        #endregion

        #region Methods
        private void CarregarDados()
        {
           
            var clientes = _clienteService.ObterTodos();
            ListaClientes = new ObservableCollection<Cliente>(clientes);
        }

        private void PrepararEdicao()
        {
            if (ClienteSelecionado != null)
            {
                
                ClienteEmEdicao = new Cliente
                {
                    Id = ClienteSelecionado.Id,
                    Nome = ClienteSelecionado.Nome,
                    Email = ClienteSelecionado.Email,
                    Telefone = ClienteSelecionado.Telefone,
                    Nif = ClienteSelecionado.Nif,
                    DataRegisto = ClienteSelecionado.DataRegisto
                };
            }
            else
            {
                ClienteEmEdicao = new Cliente();
            }
        }

        private bool PodeGravar()
        {
           
            return !string.IsNullOrWhiteSpace(ClienteEmEdicao?.Nome) &&
                   !string.IsNullOrWhiteSpace(ClienteEmEdicao?.Email) &&
                   ClienteEmEdicao.Email.Contains("@") &&
                   !string.IsNullOrWhiteSpace(ClienteEmEdicao?.Nif) &&
                   ClienteEmEdicao.Nif.Length >= 9;
        }

        private void ExecutarNovo()
        {
            _clienteSelecionado = null; 
            OnPropertyChanged(nameof(ClienteSelecionado));
            ClienteEmEdicao = new Cliente();
            (RemoverCommand as ViewModelCommand)?.RaiseCanExecuteChanged();
        }

        private void ExecutarGravar()
        {
            try
            {
                bool sucesso;
                string mensagem;

                if (ClienteEmEdicao.Id == 0)
                {
                    sucesso = _clienteService.RegistarCliente(ClienteEmEdicao, out mensagem);
                }
                else
                {
                    sucesso = _clienteService.AtualizarDados(ClienteEmEdicao, out mensagem);
                }

                if (sucesso)
                {
                    MessageBox.Show(mensagem, "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                    CarregarDados();
                    ExecutarNovo();
                }
                else
                {
                    MessageBox.Show(mensagem, "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gravar: {ex.Message}", "Erro Crítico", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecutarRemover()
        {
            if (ClienteSelecionado == null) return;

            var resultado = MessageBox.Show($"Tem a certeza que deseja remover o cliente {ClienteSelecionado.Nome}?\nEsta ação não pode ser desfeita.",
                                           "Confirmar Remoção", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (resultado == MessageBoxResult.Yes)
            {
                if (_clienteService.RemoverCliente(ClienteSelecionado.Id, out string mensagem))
                {
                    MessageBox.Show(mensagem, "Removido", MessageBoxButton.OK, MessageBoxImage.Information);
                    CarregarDados();
                    ExecutarNovo();
                }
                else
                {
                    MessageBox.Show(mensagem, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        #endregion
    }
}