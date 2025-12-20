using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using POOfase1trabalho13_11.ViewModels.Commands;

namespace POOfase1trabalho13_11.ViewModels
{
    /// <summary>
    /// ViewModel para visualização de Auditoria. 
    /// O histórico é preservado sem opção de eliminação para garantir integridade.
    /// </summary>
    public class AuditoriaViewModel : BaseViewModel
    {
        #region Fields
        private readonly IRegistoService _registoService;
        private string _filtroGeral = string.Empty;
        private Registo? _registoSelecionado;
        private List<Registo> _cacheLogs = new();
        #endregion

        #region Properties
        
        public ObservableCollection<Registo> ListaLogs { get; } = new();

        /// <summary>
        /// Filtro de pesquisa dinâmica por Tipo de Entidade, Operação ou Utilizador.
        /// </summary>
        public string FiltroGeral
        {
            get => _filtroGeral;
            set
            {
                if (SetProperty(ref _filtroGeral, value))
                {
                    AplicarFiltroLocal();
                }
            }
        }

        public Registo? RegistoSelecionado
        {
            get => _registoSelecionado;
            set => SetProperty(ref _registoSelecionado, value);
        }

        
        #endregion

        #region Commands
        public ICommand AtualizarCommand { get; }
        #endregion

        #region Constructor
        public AuditoriaViewModel(IRegistoService registoService)
        {
            _registoService = registoService ?? throw new ArgumentNullException(nameof(registoService));

            // Inicializa o comando de atualização
            AtualizarCommand = new ViewModelCommand(p => CarregarDados());

            // Carregamento inicial de dados
            CarregarDados();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Carrega os dados do repositório JSON ordenados por data decrescente.
        /// </summary>
        private void CarregarDados()
        {
            try
            {
                
                _cacheLogs = _registoService.ObterTodosLogs()
                                           .OrderByDescending(l => l.DataRegisto)
                                           .ToList();

                AplicarFiltroLocal();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Falha ao ler o histórico: {ex.Message}", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AplicarFiltroLocal()
        {
            IEnumerable<Registo> filtrados;

            if (string.IsNullOrWhiteSpace(FiltroGeral))
            {
                filtrados = _cacheLogs;
            }
            else
            {
               
                filtrados = _cacheLogs.Where(l =>
                    (l.TipoEntidade?.Contains(FiltroGeral, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (l.Operacao?.Contains(FiltroGeral, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (l.Utilizador?.Contains(FiltroGeral, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (l.Detalhes?.Contains(FiltroGeral, StringComparison.OrdinalIgnoreCase) ?? false));
            }

           
            ListaLogs.Clear();
            foreach (var log in filtrados)
            {
                ListaLogs.Add(log);
            }
        }
        

        #endregion
    }
}