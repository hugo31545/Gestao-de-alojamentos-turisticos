//-----------------------------------------------------------------
//    <copyright file="AlojamentoViewModel.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.Exceptions;
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Enums;
using POOfase1trabalho13_11.Models.Services.Interfaces;
using POOfase1trabalho13_11.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;



namespace POOfase1trabalho13_11.ViewModels
{

    /// <summary>
    /// ViewModel responsável pela lógica de interação da vista de Gestão de Alojamentos.
    /// Coordena a listagem, criação, edição e remoção de unidades de alojamento.
    /// </summary>
    public class AlojamentoViewModel : BaseViewModel
    {
        #region Fields
        private readonly IAlojamentoService _alojamentoService;
        private ObservableCollection<Alojamento> _listaAlojamentos;
        private Alojamento _alojamentoEmEdicao;
        private Alojamento? _alojamentoSelecionado;
        private string _mensagemStatus = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        /// Coleção de alojamentos ligada à DataGrid ou ListView da View.
        /// </summary>
        public ObservableCollection<Alojamento> ListaAlojamentos //observableCollection para notificar a UI sobre alterações na lista
        {
            get => _listaAlojamentos;
            set { _listaAlojamentos = value; OnPropertyChanged(nameof(ListaAlojamentos)); } // Notifica a UI sobre alterações na lista
        }

        /// <summary>
        /// Obtém ou define o alojamento selecionado na lista. 
        /// Ao ser alterado, prepara uma cópia para edição.
        /// </summary>
        public Alojamento? AlojamentoSelecionado // Pode ser nulo se nada estiver selecionado
        {
            get => _alojamentoSelecionado;
            set
            {
                if (SetProperty(ref _alojamentoSelecionado, value)) // Usa SetProperty para notificar a UI
                {
                    PrepararEdicao();
                    (RemoverAlojamentoCommand as ViewModelCommand)?.RaiseCanExecuteChanged(); // Atualiza o estado do comando de remoção
                }
            }
        }

        /// <summary>
        /// Objeto temporário que contém os dados atuais do formulário de entrada.
        /// </summary>
        public Alojamento AlojamentoEmEdicao
        {
            get => _alojamentoEmEdicao;
            set
            {
                _alojamentoEmEdicao = value; // Define o alojamento em edição
                OnPropertyChanged(nameof(AlojamentoEmEdicao)); // Notifica a UI sobre alterações no alojamento em edição
                (AdicionarAlojamentoCommand as ViewModelCommand)?.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Texto informativo exibido na barra de estado da aplicação.
        /// </summary>
        public string MensagemStatus
        {
            get => _mensagemStatus;
            set { _mensagemStatus = value; OnPropertyChanged(nameof(MensagemStatus)); }
        }
        #endregion

        #region Commands
        public ICommand AdicionarAlojamentoCommand { get; }
        public ICommand NovoCommand { get; }
        public ICommand RemoverAlojamentoCommand { get; }
        public ICommand MudarEstadoCommand { get; }
        public ICommand CarregarDadosCommand { get; }
        #endregion


        #region Constructor

        /// <summary>
        /// Inicializa a ViewModel e injeta as dependências necessárias.
        /// </summary>
        public AlojamentoViewModel(IAlojamentoService alojamentoService)
        {
            _alojamentoService = alojamentoService ?? throw new ArgumentNullException(nameof(alojamentoService));
            _listaAlojamentos = new ObservableCollection<Alojamento>();
            _alojamentoEmEdicao = new Alojamento();

            // Inicialização dos comandos com as respetivas ações e validações
            AdicionarAlojamentoCommand = new ViewModelCommand(p => ExecutarGravar(), p => PodeGravar());
            NovoCommand = new ViewModelCommand(p => ExecutarNovo());
            RemoverAlojamentoCommand = new ViewModelCommand(p => ExecutarRemover(), p => AlojamentoSelecionado != null);
            MudarEstadoCommand = new RelayCommand<EstadoAlojamento>(ExecutarMudarEstado);
            CarregarDadosCommand = new ViewModelCommand(p => ExecutarCarregarDados());

            ExecutarCarregarDados();
        }
        #endregion


        #region Methods


        /// <summary>
        /// Carrega ou atualiza a coleção de alojamentos a partir do serviço.
        /// </summary>
        private void ExecutarCarregarDados()
        {
            try
            {
                var dados = _alojamentoService.ObterTodosAlojamentos();

              
                if (ListaAlojamentos == null)
                {
                    ListaAlojamentos = new ObservableCollection<Alojamento>(dados);
                }
                else
                {
                    ListaAlojamentos.Clear();
                    foreach (var item in dados)
                    {
                        ListaAlojamentos.Add(item);
                    }
                }

               

                MensagemStatus = $"Lista atualizada: {DateTime.Now:HH:mm:ss}";
            }
            catch (Exception ex)
            {
                MensagemStatus = "Erro ao carregar dados.";
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        /// <summary>
        /// Cria uma cópia profunda (clone) do alojamento selecionado para edição, 
        /// garantindo que alterações canceladas não afetem a lista original.
        /// </summary>
        private void PrepararEdicao()
        {
            if (AlojamentoSelecionado != null)
            {
              
                AlojamentoEmEdicao = new Alojamento
                {
                    Id = AlojamentoSelecionado.Id,
                    Nome = AlojamentoSelecionado.Nome,
                    Localizacao = AlojamentoSelecionado.Localizacao,
                    Capacidade = AlojamentoSelecionado.Capacidade,
                    PrecoPorNoite = AlojamentoSelecionado.PrecoPorNoite,
                    Descricao = AlojamentoSelecionado.Descricao,
                    Tipo = AlojamentoSelecionado.Tipo,
                    Estado = AlojamentoSelecionado.Estado,
                    Comodidades = AlojamentoSelecionado.Comodidades
                };
            }
            else
            {
                AlojamentoEmEdicao = new Alojamento();
            }
        }

        /// <summary>
        /// Valida se os campos obrigatórios do formulário estão preenchidos.
        /// </summary>
        private bool PodeGravar()
        {
            return !string.IsNullOrWhiteSpace(AlojamentoEmEdicao?.Nome) &&
                   !string.IsNullOrWhiteSpace(AlojamentoEmEdicao?.Localizacao) &&
                   AlojamentoEmEdicao.Capacidade > 0 &&
                   AlojamentoEmEdicao.PrecoPorNoite > 0;
        }

        /// <summary>
        /// Limpa o formulário e a seleção para permitir um novo registo.
        /// </summary>
        private void ExecutarNovo()
        {
            
            _alojamentoSelecionado = null;
            OnPropertyChanged(nameof(AlojamentoSelecionado));
            AlojamentoEmEdicao = new Alojamento();
            MensagemStatus = "Pronto para novo registo.";
            (RemoverAlojamentoCommand as ViewModelCommand)?.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Invoca o serviço para persistir as alterações do alojamento em edição.
        /// </summary>
        private void ExecutarGravar()
        {
            if (AlojamentoEmEdicao == null || string.IsNullOrEmpty(AlojamentoEmEdicao.Nome))
            {
                MensagemStatus = "Dados inválidos para gravação.";
                return;
            }

            try
            {
                _alojamentoService.GuardarAlojamento(AlojamentoEmEdicao);

                ExecutarCarregarDados();
                ExecutarNovo();

                MessageBox.Show("Alojamento persistido com sucesso!", "Sucesso",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                
            }
            catch (BusinessException ex)
            {
                MensagemStatus = $"Aviso: {ex.Message}";
                MessageBox.Show(ex.Message, "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MensagemStatus = "Erro crítico ao gravar.";
                MessageBox.Show($"Erro inesperado: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Remove o alojamento selecionado após confirmação do utilizador.
        /// </summary>
        private void ExecutarRemover()
        {
            if (AlojamentoSelecionado == null) return;

            var confirmacao = MessageBox.Show($"Deseja remover '{AlojamentoSelecionado.Nome}'?\nEsta ação não pode ser desfeita.",
                "Confirmar Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (confirmacao == MessageBoxResult.Yes)
            {
                try
                {
                    _alojamentoService.RemoverAlojamento(AlojamentoSelecionado.Id);

                    ExecutarCarregarDados();
                    ExecutarNovo();
                    MensagemStatus = "Alojamento removido.";
                }
                catch (BusinessException ex)
                {
                    MensagemStatus = ex.Message;
                    MessageBox.Show(ex.Message, "Bloqueio", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
        }

        /// <summary>
        /// Altera o estado operacional do alojamento (Ex: Manutenção, Disponível).
        /// </summary>
        private void ExecutarMudarEstado(EstadoAlojamento novoEstado)
        {
            if (AlojamentoSelecionado == null) return;

            try
            {
                if (_alojamentoService.MudarEstado(AlojamentoSelecionado.Id, novoEstado))
                {
                    ExecutarCarregarDados();
                    MensagemStatus = $"Estado alterado para {novoEstado}.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao mudar estado: {ex.Message}");
            }
        }
       
        #endregion
    }
}