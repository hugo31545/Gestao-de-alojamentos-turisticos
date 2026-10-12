//-----------------------------------------------------------------
//    <copyright file="ClienteViewModel.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.ViewModels.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Services.Interfaces;
using POOfase1trabalho13_11.ViewModels.Commands;

namespace POOfase1trabalho13_11.ViewModels
{
    /// <summary>
    /// ViewModel responsável pela gestão da interface de Clientes.
    /// Coordena o fluxo de registo, alteração e remoção de clientes, garantindo a integridade dos dados na UI.
    /// </summary>
    public class ClienteViewModel : BaseViewModel
    {
        private readonly IDialogService _dialog;

        #region Fields
        private readonly IClienteService _clienteService;
        private ObservableCollection<Cliente> _listaClientes;
        private Cliente _clienteEmEdicao;
        private Cliente? _clienteSelecionado;
        #endregion

        #region Properties
        /// <summary>
        /// Coleção de clientes para exibição na View.
        /// </summary>
        public ObservableCollection<Cliente> ListaClientes
        {
            get => _listaClientes;
            set { _listaClientes = value; OnPropertyChanged(nameof(ListaClientes)); }
        }


        /// <summary>
        /// Instância do cliente que está a ser manipulada no formulário de entrada.
        /// </summary>
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


        /// <summary>
        /// Cliente selecionado na listagem. Ao ser alterado, despoleta a preparação da edição.
        /// </summary>
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

        /// <summary>
        /// Inicializa a ViewModel injetando o serviço de clientes.
        /// </summary>
        public ClienteViewModel(IClienteService clienteService, IDialogService dialogService)
        {
            _dialog = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
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

        /// <summary>
        /// Atualiza a coleção de clientes a partir da base de dados/ficheiro.
        /// </summary>
        private void CarregarDados()
        {
           
            var clientes = _clienteService.ObterTodos();
            ListaClientes = new ObservableCollection<Cliente>(clientes);
        }


        /// <summary>
        /// Prepara uma cópia de segurança para edição, evitando que edições não guardadas reflitam na lista.
        /// </summary>
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


        /// <summary>
        /// Validação básica de UI para habilitar o botão Gravar.
        /// </summary>
        private bool PodeGravar()
        {
           
            return !string.IsNullOrWhiteSpace(ClienteEmEdicao?.Nome) &&
                   !string.IsNullOrWhiteSpace(ClienteEmEdicao?.Email) &&
                   ClienteEmEdicao.Email.Contains("@") &&
                   !string.IsNullOrWhiteSpace(ClienteEmEdicao?.Nif) &&
                   ClienteEmEdicao.Nif.Length >= 9;
        }


        /// <summary>
        /// Limpa o estado de seleção e prepara o formulário para um novo registo.
        /// </summary>
        private void ExecutarNovo()
        {
            _clienteSelecionado = null; 
            OnPropertyChanged(nameof(ClienteSelecionado));
            ClienteEmEdicao = new Cliente();
            (RemoverCommand as ViewModelCommand)?.RaiseCanExecuteChanged();
        }


        /// <summary>
        /// Tenta persistir os dados do cliente, decidindo entre novo registo ou atualização.
        /// </summary>
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
                    _dialog.Informar(mensagem, "Sucesso");
                    CarregarDados();
                    ExecutarNovo();
                }
                else
                {
                    _dialog.Avisar(mensagem, "Aviso");
                }
            }
            catch (Exception ex)
            {
                _dialog.Erro($"Erro ao gravar: {ex.Message}", "Erro Crítico");
            }
        }


        /// <summary>
        /// Remove o cliente selecionado após confirmação explícita do utilizador.
        /// </summary>
        private void ExecutarRemover()
        {
            if (ClienteSelecionado == null) return;

            bool resultado = _dialog.Confirmar($"Tem a certeza que deseja remover o cliente {ClienteSelecionado.Nome}?\nEsta ação não pode ser desfeita.", "Confirmar Remoção");

            if (resultado)
            {
                if (_clienteService.RemoverCliente(ClienteSelecionado.Id, out string mensagem))
                {
                    _dialog.Informar(mensagem, "Removido");
                    CarregarDados();
                    ExecutarNovo();
                }
                else
                {
                    _dialog.Erro(mensagem, "Erro");
                }
            }
        }
        #endregion
    }
}