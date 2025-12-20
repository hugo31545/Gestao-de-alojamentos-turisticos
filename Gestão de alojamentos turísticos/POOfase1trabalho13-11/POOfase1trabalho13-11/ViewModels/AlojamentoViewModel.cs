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
        

        public ObservableCollection<Alojamento> ListaAlojamentos
        {
            get => _listaAlojamentos;
            set { _listaAlojamentos = value; OnPropertyChanged(nameof(ListaAlojamentos)); }
        }

        
        public Alojamento? AlojamentoSelecionado
        {
            get => _alojamentoSelecionado;
            set
            {
                if (SetProperty(ref _alojamentoSelecionado, value))
                {
                    PrepararEdicao();
                    (RemoverAlojamentoCommand as ViewModelCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public Alojamento AlojamentoEmEdicao
        {
            get => _alojamentoEmEdicao;
            set
            {
                _alojamentoEmEdicao = value;
                OnPropertyChanged(nameof(AlojamentoEmEdicao));
                (AdicionarAlojamentoCommand as ViewModelCommand)?.RaiseCanExecuteChanged();
            }
        }

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
        public AlojamentoViewModel(IAlojamentoService alojamentoService)
        {
            _alojamentoService = alojamentoService ?? throw new ArgumentNullException(nameof(alojamentoService));
            _listaAlojamentos = new ObservableCollection<Alojamento>();
            _alojamentoEmEdicao = new Alojamento();

            AdicionarAlojamentoCommand = new ViewModelCommand(p => ExecutarGravar(), p => PodeGravar());
            NovoCommand = new ViewModelCommand(p => ExecutarNovo());
            RemoverAlojamentoCommand = new ViewModelCommand(p => ExecutarRemover(), p => AlojamentoSelecionado != null);
            MudarEstadoCommand = new RelayCommand<EstadoAlojamento>(ExecutarMudarEstado);
            CarregarDadosCommand = new ViewModelCommand(p => ExecutarCarregarDados());

            ExecutarCarregarDados();
        }
        #endregion


        #region Methods

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
        private bool PodeGravar()
        {
            return !string.IsNullOrWhiteSpace(AlojamentoEmEdicao?.Nome) &&
                   !string.IsNullOrWhiteSpace(AlojamentoEmEdicao?.Localizacao) &&
                   AlojamentoEmEdicao.Capacidade > 0 &&
                   AlojamentoEmEdicao.PrecoPorNoite > 0;
        }

        private void ExecutarNovo()
        {
            
            _alojamentoSelecionado = null;
            OnPropertyChanged(nameof(AlojamentoSelecionado));
            AlojamentoEmEdicao = new Alojamento();
            MensagemStatus = "Pronto para novo registo.";
            (RemoverAlojamentoCommand as ViewModelCommand)?.RaiseCanExecuteChanged();
        }

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