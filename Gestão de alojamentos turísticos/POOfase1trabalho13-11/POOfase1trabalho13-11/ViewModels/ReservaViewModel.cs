//-----------------------------------------------------------------
//    <copyright file="ReservaViewModel.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.ViewModels.Services;
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Enums;
using POOfase1trabalho13_11.Models.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Input;
using POOfase1trabalho13_11.ViewModels.Commands;

namespace POOfase1trabalho13_11.ViewModels
{

    /// <summary>
    /// ViewModel responsável pela gestão de reservas, incluindo a lógica de disponibilidade,
    /// registo de estadias (Check-In/Out) e cancelamentos.
    /// </summary>
    public class ReservaViewModel : BaseViewModel
    {
        private readonly IDialogService _dialog;

        #region Services
        private readonly IReservaService _reservaService;
        private readonly IClienteService _clienteService;
        private readonly IAlojamentoService _alojamentoService;
        private readonly ICheckInService _checkInService;
        private readonly ISessionService _sessao;

        #endregion

        #region Private Properties
        private Cliente? _clienteSelecionado;
        private Alojamento? _alojamentoSelecionado;
        private DateTime _dataInicio = DateTime.Today;
        private DateTime _dataFim = DateTime.Today.AddDays(1);
        private int _numHospedes = 1;
        private string _motivoCancelamento = string.Empty;
        private Reserva? _reservaSelecionada;

        // Os botões de cada linha passam a própria reserva como parâmetro
        private Reserva? ReservaAlvo(object? parameter) => parameter as Reserva ?? ReservaSelecionada;
        private bool PodeConfirmar(object? p) => ReservaAlvo(p)?.Estado == EstadoReserva.Pendente;
        private bool PodeFazerCheckIn(object? p) => ReservaAlvo(p)?.Estado == EstadoReserva.Confirmada;
        private bool PodeFazerCheckOut(object? p) => ReservaAlvo(p)?.Estado == EstadoReserva.CheckInRealizado;
        private bool PodeCancelar(object? p) => ReservaAlvo(p)?.PodeSerCancelada == true;
        #endregion

        #region Public Properties
        // Listas observáveis que alimentam as ComboBoxes e DataGrids na View
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

                // Reavalia comandos de estado sempre que uma reserva é selecionada na lista
                (CancelarReservaCommand as ViewModelCommand)?.RaiseCanExecuteChanged();
                (ConfirmarReservaCommand as ViewModelCommand)?.RaiseCanExecuteChanged();
                (CheckInCommand as ViewModelCommand)?.RaiseCanExecuteChanged();
                (CheckOutCommand as ViewModelCommand)?.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Nome registado na auditoria como responsável pelo check-in/check-out.
        /// </summary>
        private string UtilizadorAtual => _sessao.NomeParaAuditoria;
        #endregion



        #region Commands
        public ICommand CriarReservaCommand { get; }
        public ICommand ConfirmarReservaCommand { get; }
        public ICommand CancelarReservaCommand { get; }
        public ICommand CarregarDadosCommand { get; }
        public ICommand CheckInCommand { get; }
        public ICommand CheckOutCommand { get; }
        #endregion

        #region Constructor



        public ReservaViewModel(IReservaService reservaService, IClienteService clienteService, IAlojamentoService alojamentoService, ICheckInService checkInService, ISessionService sessao, IDialogService dialogService)
        {
            _dialog = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _reservaService = reservaService;
            _clienteService = clienteService;
            _alojamentoService = alojamentoService;
            _checkInService = checkInService;
            _sessao = sessao;


            CriarReservaCommand = new ViewModelCommand(p => ExecutarCriarReserva(), p => PodeReservar());
            ConfirmarReservaCommand = new ViewModelCommand(ExecutarConfirmar, PodeConfirmar);
            CancelarReservaCommand = new ViewModelCommand(ExecutarCancelar, PodeCancelar);
            CarregarDadosCommand = new ViewModelCommand(p => CarregarDados());
            CheckInCommand = new ViewModelCommand(ExecutarCheckIn, PodeFazerCheckIn);
            CheckOutCommand = new ViewModelCommand(ExecutarCheckOut, PodeFazerCheckOut);

            CarregarDados();
        }
        #endregion

        #region Helper Methods

        /// <summary>
        /// Filtra alojamentos que não possuem conflitos de datas para o período selecionado.
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



        private void ExecutarConfirmar(object? parameter)
        {
            var reserva = ReservaAlvo(parameter);
            if (reserva == null) return;

            if (_reservaService.ConfirmarReserva(reserva.Id, out string msg))
            {
                _dialog.Informar(msg, "Confirmação");
                CarregarDados();
            }
            else
            {
                _dialog.Avisar(msg, "Atenção");
            }
        }

        private void ExecutarCheckIn(object? parameter)
        {
            var reserva = ReservaAlvo(parameter);
            if (reserva == null) return;

            // O CheckInService também marca o alojamento como Ocupado e regista na auditoria
            if (_checkInService.RealizarCheckIn(reserva.Id, UtilizadorAtual, out string msg))
            {
                _dialog.Informar(msg, "Check-In");
                CarregarDados();
            }
            else
            {
                _dialog.Avisar(msg, "Atenção");
            }
        }

        private void ExecutarCheckOut(object? parameter)
        {
            var reserva = ReservaAlvo(parameter);
            if (reserva == null) return;

            // Finaliza a reserva (conta para a faturação) e envia o alojamento para manutenção/limpeza
            if (_checkInService.RealizarCheckOut(reserva.Id, UtilizadorAtual, out string msg))
            {
                _dialog.Informar(msg, "Check-Out");
                CarregarDados();
            }
            else
            {
                _dialog.Erro(msg, "Erro");
            }
        }

        private void ExecutarCancelar(object? parameter)
        {
            var reserva = ReservaAlvo(parameter);
            if (reserva == null) return;

            if (string.IsNullOrWhiteSpace(MotivoCancelamento))
            {
                _dialog.Avisar("Por favor, indique um motivo no campo em baixo.", "Aviso");
                return;
            }

            if (_reservaService.CancelarReserva(reserva.Id, MotivoCancelamento, out string msg))
            {
                _dialog.Informar(msg);
                MotivoCancelamento = string.Empty;
                CarregarDados();
            }
            else
            {
                _dialog.Avisar(msg, "Atenção");
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

            if (sucesso) _dialog.Informar(mensagem, "Sucesso");
            else _dialog.Avisar(mensagem);

            if (sucesso)
            {
                
                CarregarDados();
                AlojamentoSelecionado = null;
            }
        }
        
        #endregion
    }
}