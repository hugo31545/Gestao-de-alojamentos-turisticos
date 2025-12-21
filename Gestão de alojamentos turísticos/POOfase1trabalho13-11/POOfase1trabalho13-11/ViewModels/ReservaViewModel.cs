using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Enums;
using POOfase1trabalho13_11.Models.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using POOfase1trabalho13_11.ViewModels.Commands;

namespace POOfase1trabalho13_11.ViewModels
{
    public class ReservaViewModel : BaseViewModel
    {
        #region Services
        private readonly IReservaService _reservaService;
        private readonly IClienteService _clienteService;
        private readonly IAlojamentoService _alojamentoService;

        #endregion

        #region Private Properties
        private Cliente? _clienteSelecionado;
        private Alojamento? _alojamentoSelecionado;
        private DateTime _dataInicio = DateTime.Today;
        private DateTime _dataFim = DateTime.Today.AddDays(1);
        private int _numHospedes = 1;
        private string _motivoCancelamento = string.Empty;
        private Reserva? _reservaSelecionada;

        private bool PodeFazerCheckIn() => ReservaSelecionada?.Estado == EstadoReserva.Confirmada;
        private bool PodeFazerCheckOut() => ReservaSelecionada?.Estado == EstadoReserva.CheckInRealizado;
        #endregion

        #region Public Properties

        public ObservableCollection<Reserva> ListaReservas { get; set; } = new();
        public ObservableCollection<Cliente> ListaClientes { get; set; } = new();
        public ObservableCollection<Alojamento> ListaAlojamentosDisponiveis { get; set; } = new();

        public Cliente? ClienteSelecionado
        {
            get => _clienteSelecionado;
            set
            {
                _clienteSelecionado = value;
                OnPropertyChanged(nameof(ClienteSelecionado));
                NotificarComandos();
            }
        }

        public Alojamento? AlojamentoSelecionado
        {
            get => _alojamentoSelecionado;
            set
            {
                _alojamentoSelecionado = value;
                OnPropertyChanged(nameof(AlojamentoSelecionado));
                NotificarComandos();
            }
        }

        public int NumHospedes
        {
            get => _numHospedes;
            set
            {
                _numHospedes = value;
                OnPropertyChanged(nameof(NumHospedes));
                NotificarComandos();
            }
        }

        public DateTime DataInicio
        {
            get => _dataInicio;
            set
            {
                _dataInicio = value;
                OnPropertyChanged(nameof(DataInicio));
                AtualizarAlojamentosDisponiveis();
                NotificarComandos();
            }
        }

        public DateTime DataFim
        {
            get => _dataFim;
            set
            {
                _dataFim = value;
                OnPropertyChanged(nameof(DataFim));
                AtualizarAlojamentosDisponiveis();
                NotificarComandos();
            }
        }

        public string MotivoCancelamento
        {
            get => _motivoCancelamento;
            set
            {
                _motivoCancelamento = value;
                OnPropertyChanged(nameof(MotivoCancelamento));
            }
        }

        
        

        public Reserva? ReservaSelecionada
        {
            get => _reservaSelecionada;
            set
            {
                _reservaSelecionada = value;
                OnPropertyChanged(nameof(ReservaSelecionada));

                // Notificar todos os comandos que dependem da seleção
                (CancelarReservaCommand as ViewModelCommand)?.RaiseCanExecuteChanged();
                (CheckInCommand as ViewModelCommand)?.RaiseCanExecuteChanged();
                (CheckOutCommand as ViewModelCommand)?.RaiseCanExecuteChanged();
            }
        }
        #endregion



        #region Commands
        public ICommand CriarReservaCommand { get; }
        public ICommand CancelarReservaCommand { get; }
        public ICommand CarregarDadosCommand { get; }
        public ICommand CheckInCommand { get; }
        public ICommand CheckOutCommand { get; }
        #endregion

        #region Constructor


        
        public ReservaViewModel(IReservaService reservaService, IClienteService clienteService, IAlojamentoService alojamentoService)
        {
            _reservaService = reservaService;
            _clienteService = clienteService;
            _alojamentoService = alojamentoService;

            
            CriarReservaCommand = new ViewModelCommand(p => ExecutarCriarReserva(), p => PodeReservar());
            CancelarReservaCommand = new ViewModelCommand(p => ExecutarCancelar(p), p => true);
            CarregarDadosCommand = new ViewModelCommand(p => CarregarDados());
            CheckInCommand = new ViewModelCommand(p => ExecutarCheckIn(p), p => true);
            CheckOutCommand = new ViewModelCommand(p => ExecutarCheckOut(p), p => true);

            CarregarDados();
        }
        #endregion

        #region Helper Methods

        /// <summary>
        /// Atualiza a lista de alojamentos filtrando apenas os que estão livres no período selecionado.
        /// </summary>
        private void AtualizarAlojamentosDisponiveis()
        {
            ListaAlojamentosDisponiveis.Clear();

            // Só pesquisa se as datas forem válidas para evitar erros de lógica
            if (DataFim > DataInicio)
            {
                var disponiveis = _alojamentoService.BuscarDisponiveisPorPeriodo(DataInicio, DataFim);
                foreach (var a in disponiveis)
                {
                    ListaAlojamentosDisponiveis.Add(a);
                }
            }

            // Se o alojamento selecionado anteriormente já não estiver na lista (ficou ocupado), limpa a seleção
            if (AlojamentoSelecionado != null && !ListaAlojamentosDisponiveis.Contains(AlojamentoSelecionado))
            {
                AlojamentoSelecionado = null;
            }
        }



        private void ExecutarCheckIn(object? parameter)
        {
            
            var reserva = parameter as Reserva ?? ReservaSelecionada;
            if (reserva == null) return;

            // Lógica extra: Se estiver Pendente, o sistema confirma-a automaticamente antes do Check-In
            if (reserva.Estado == EstadoReserva.Pendente)
            {
                reserva.Estado = EstadoReserva.Confirmada;
               
            }

            if (_reservaService.EfetuarCheckIn(reserva.Id, out string msg))
            {
                MessageBox.Show(msg, "Check-In", MessageBoxButton.OK, MessageBoxImage.Information);
                CarregarDados();
            }
            else
            {
                MessageBox.Show(msg, "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ExecutarCheckOut(object? parameter)
        {
            var reserva = parameter as Reserva ?? ReservaSelecionada;
            if (reserva == null) return;

            if (_reservaService.EfetuarCheckOut(reserva.Id, out string msg))
            {
                MessageBox.Show(msg, "Check-Out", MessageBoxButton.OK, MessageBoxImage.Information);
                CarregarDados();
            }
            else
            {
                MessageBox.Show(msg, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecutarCancelar(object? parameter)
        {
            var reserva = parameter as Reserva ?? ReservaSelecionada;
            if (reserva == null) return;

            if (string.IsNullOrWhiteSpace(MotivoCancelamento))
            {
                MessageBox.Show("Por favor, indique um motivo no campo em baixo.", "Aviso");
                return;
            }

            if (_reservaService.CancelarReserva(reserva.Id, MotivoCancelamento, out string msg))
            {
                MessageBox.Show(msg);
                MotivoCancelamento = string.Empty;
                CarregarDados();
            }
        }
        private void CarregarDados()
        {
            // 1. Carregar Clientes
            ListaClientes.Clear();
            var clientes = _clienteService.ObterTodos();
            foreach (var c in clientes) ListaClientes.Add(c);

            // 2. Atualizar Alojamentos Livres
            AtualizarAlojamentosDisponiveis();

            // 3. CARREGAR AS RESERVAS 
            ListaReservas.Clear();
            
            var reservas = _reservaService.ObterTodasReservas();

            foreach (var r in reservas)
            {
                ListaReservas.Add(r);
            }
        
        }


        /// <summary>
        /// Regras que definem se o botão "Reservar" está ativo ou cinzento.
        /// </summary>
        private bool PodeReservar()
        {
            return ClienteSelecionado != null &&
                   AlojamentoSelecionado != null &&
                   NumHospedes > 0 &&
                   NumHospedes <= AlojamentoSelecionado.Capacidade &&
                   DataFim > DataInicio;
        }

        

        /// <summary>
        /// Força a interface a reavaliar se os botões podem ser clicados.
        /// </summary>
        private void NotificarComandos()
        {
            (CriarReservaCommand as ViewModelCommand)?.RaiseCanExecuteChanged();
        }

        #endregion

        #region Command Execution Methods

        private void ExecutarCriarReserva()
        {
            bool sucesso = _reservaService.RealizarReserva(
                ClienteSelecionado!.Id,
                AlojamentoSelecionado!.Id,
                DataInicio,
                DataFim,
                NumHospedes,
                out string mensagem
            );

            MessageBox.Show(mensagem, sucesso ? "Sucesso" : "Atenção", MessageBoxButton.OK,
                            sucesso ? MessageBoxImage.Information : MessageBoxImage.Warning);

            if (sucesso)
            {
                
                CarregarDados();
                AlojamentoSelecionado = null;
            }
        }
        
        #endregion
    }
}