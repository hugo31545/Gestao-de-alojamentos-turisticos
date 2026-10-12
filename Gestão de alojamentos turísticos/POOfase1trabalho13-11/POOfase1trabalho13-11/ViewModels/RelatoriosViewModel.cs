//-----------------------------------------------------------------
//    <copyright file="RelatoriosViewModel.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>1</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Services.Interfaces;
using POOfase1trabalho13_11.ViewModels.Commands;
using POOfase1trabalho13_11.ViewModels.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace POOfase1trabalho13_11.ViewModels
{
    /// <summary>
    /// ViewModel do painel de relatórios. Apresenta os indicadores calculados pelo
    /// <see cref="IConsultaService"/> (ocupação, faturação, rankings) e regista cada
    /// consulta pedida pelo utilizador no histórico de pesquisas.
    /// </summary>
    public class RelatoriosViewModel : BaseViewModel
    {
        #region Fields
        private readonly IConsultaService _consultaService;
        private readonly ISessionService _sessao;
        private readonly IDialogService _dialog;

        private double _taxaOcupacao;
        private decimal _faturacaoTotal;
        private DateTime _dataInicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        private DateTime _dataFim = DateTime.Today;
        private int _top = 5;
        private string _mensagemStatus = string.Empty;
        #endregion

        #region Properties
        /// <summary>
        /// Percentagem de alojamentos ocupados neste momento.
        /// </summary>
        public double TaxaOcupacao
        {
            get => _taxaOcupacao;
            private set => SetProperty(ref _taxaOcupacao, value);
        }

        /// <summary>
        /// Valor faturado (estadias com check-out) entre <see cref="DataInicio"/> e <see cref="DataFim"/>.
        /// </summary>
        public decimal FaturacaoTotal
        {
            get => _faturacaoTotal;
            private set => SetProperty(ref _faturacaoTotal, value);
        }

        /// <summary>
        /// Início do período de faturação (por omissão, o primeiro dia do mês atual).
        /// </summary>
        public DateTime DataInicio
        {
            get => _dataInicio;
            set
            {
                if (SetProperty(ref _dataInicio, value))
                    (CalcularFaturacaoCommand as ViewModelCommand)?.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Fim do período de faturação (inclusivo).
        /// </summary>
        public DateTime DataFim
        {
            get => _dataFim;
            set
            {
                if (SetProperty(ref _dataFim, value))
                    (CalcularFaturacaoCommand as ViewModelCommand)?.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Número de linhas dos rankings (alojamentos mais caros e melhores clientes).
        /// </summary>
        public int Top
        {
            get => _top;
            set => SetProperty(ref _top, Math.Clamp(value, 1, 50));
        }

        /// <summary>
        /// Texto de estado mostrado no rodapé do painel.
        /// </summary>
        public string MensagemStatus
        {
            get => _mensagemStatus;
            private set => SetProperty(ref _mensagemStatus, value);
        }

        public ObservableCollection<Alojamento> AlojamentosMaisCaros { get; } = new();
        public ObservableCollection<Cliente> MelhoresClientes { get; } = new();
        public ObservableCollection<Consulta> HistoricoRecente { get; } = new();
        #endregion

        #region Commands
        public ICommand AtualizarCommand { get; }
        public ICommand CalcularFaturacaoCommand { get; }
        #endregion

        #region Constructor
        public RelatoriosViewModel(IConsultaService consultaService, ISessionService sessao, IDialogService dialogService)
        {
            _consultaService = consultaService ?? throw new ArgumentNullException(nameof(consultaService));
            _sessao = sessao ?? throw new ArgumentNullException(nameof(sessao));
            _dialog = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            AtualizarCommand = new ViewModelCommand(p => ExecutarAtualizar());
            CalcularFaturacaoCommand = new ViewModelCommand(p => ExecutarCalcularFaturacao(), p => PodeCalcularFaturacao());

            // Carregamento inicial (sem registar no histórico: não foi um pedido do utilizador)
            CarregarIndicadores();
            CarregarHistorico();
        }
        #endregion

        #region Methods
        private bool PodeCalcularFaturacao() => DataFim.Date >= DataInicio.Date;

        /// <summary>
        /// Recalcula a ocupação e os rankings e regista a consulta no histórico.
        /// </summary>
        private void ExecutarAtualizar()
        {
            if (!CarregarIndicadores()) return;

            RegistarNoHistorico("Indicadores",
                new Dictionary<string, string> { ["Top"] = Top.ToString() },
                AlojamentosMaisCaros.Count + MelhoresClientes.Count);
        }

        /// <summary>
        /// Calcula a faturação do período escolhido e regista a consulta no histórico.
        /// </summary>
        private void ExecutarCalcularFaturacao()
        {
            if (!PodeCalcularFaturacao())
            {
                _dialog.Avisar("A data de fim tem de ser igual ou posterior à data de início.");
                return;
            }

            try
            {
                // O dia de fim conta por inteiro (até às 23:59:59)
                DateTime fimInclusivo = DataFim.Date.AddDays(1).AddTicks(-1);
                FaturacaoTotal = _consultaService.CalcularFaturacaoTotal(DataInicio.Date, fimInclusivo);
                MensagemStatus = $"Faturação de {DataInicio:dd/MM/yyyy} a {DataFim:dd/MM/yyyy}: {FaturacaoTotal:N2} €";

                RegistarNoHistorico("Faturação",
                    new Dictionary<string, string>
                    {
                        ["Inicio"] = DataInicio.ToString("dd/MM/yyyy"),
                        ["Fim"] = DataFim.ToString("dd/MM/yyyy")
                    },
                    1);
            }
            catch (Exception ex)
            {
                _dialog.Erro($"Erro ao calcular a faturação: {ex.Message}");
            }
        }

        /// <summary>
        /// Lê a taxa de ocupação e os rankings do serviço.
        /// </summary>
        /// <returns><c>true</c> se os dados foram carregados.</returns>
        private bool CarregarIndicadores()
        {
            try
            {
                TaxaOcupacao = _consultaService.CalcularTaxaOcupacao();

                AlojamentosMaisCaros.Clear();
                foreach (var a in _consultaService.ObterAlojamentosMaisCaros(Top))
                    AlojamentosMaisCaros.Add(a);

                MelhoresClientes.Clear();
                foreach (var c in _consultaService.ObterClientesComMaisReservas(Top))
                    MelhoresClientes.Add(c);

                MensagemStatus = $"Indicadores atualizados: {DateTime.Now:HH:mm:ss}";
                return true;
            }
            catch (Exception ex)
            {
                _dialog.Erro($"Erro ao carregar os relatórios: {ex.Message}");
                return false;
            }
        }

        private void CarregarHistorico()
        {
            HistoricoRecente.Clear();
            foreach (var consulta in _consultaService.ObterHistoricoRecente(10))
                HistoricoRecente.Add(consulta);
        }

        /// <summary>
        /// Guarda a consulta no histórico com o utilizador da sessão e atualiza a lista.
        /// Uma falha a gravar o histórico não impede a apresentação do relatório.
        /// </summary>
        private void RegistarNoHistorico(string tipo, Dictionary<string, string> filtros, int resultados)
        {
            try
            {
                _consultaService.RegistarPesquisaNoHistorico(_sessao.NomeParaAuditoria, tipo, filtros, resultados);
                CarregarHistorico();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Não foi possível gravar o histórico: {ex.Message}");
            }
        }
        #endregion
    }
}
