//-----------------------------------------------------------------
//    <copyright file="AuditoriaViewModel.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.ViewModels.Services;
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Input;
using POOfase1trabalho13_11.ViewModels.Commands;

namespace POOfase1trabalho13_11.ViewModels
{
    /// <summary>
    /// ViewModel responsável pela visualização e análise do histórico de auditoria (Logs).
    /// Esta classe foca-se na leitura e filtragem de dados, garantindo que o histórico
    /// permanece imutável para assegurar a integridade do sistema.
    /// </summary>
    public class AuditoriaViewModel : BaseViewModel
    {
        private readonly IDialogService _dialog;

        #region Fields
        private readonly IRegistoService _registoService;
        private string _filtroGeral = string.Empty;
        private Registo? _registoSelecionado;

        /// <summary>
        /// Cache local para permitir filtragem rápida em memória sem acessos repetidos ao disco.
        /// </summary>
        private List<Registo> _cacheLogs = new();
        #endregion

        #region Properties

        /// <summary>
        /// Coleção de registos atualmente visíveis na interface após aplicação de filtros.
        /// </summary>
        public ObservableCollection<Registo> ListaLogs { get; } = new();

        /// <summary>
        /// Texto de pesquisa dinâmica que filtra logs por Entidade, Operação, Utilizador ou Detalhes.
        /// A atualização desta propriedade despoleta automaticamente a re-filtragem da lista.
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

        /// <summary>
        /// Registo selecionado na tabela de auditoria para visualização detalhada.
        /// </summary>
        public Registo? RegistoSelecionado
        {
            get => _registoSelecionado;
            set => SetProperty(ref _registoSelecionado, value);
        }


        #endregion

        #region Commands
        /// <summary>
        /// Comando para recarregar os dados a partir da fonte persistente (JSON).
        /// </summary>
        public ICommand AtualizarCommand { get; }
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa uma nova instância da <see cref="AuditoriaViewModel"/> e carrega o histórico inicial.
        /// </summary>
        /// <param name="registoService">Serviço de gestão de registos injetado.</param>
        public AuditoriaViewModel(IRegistoService registoService, IDialogService dialogService)
        {
            _dialog = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _registoService = registoService ?? throw new ArgumentNullException(nameof(registoService));

            // Define a ação do comando de atualização
            AtualizarCommand = new ViewModelCommand(p => CarregarDados());

            // Carregamento inicial automático
            CarregarDados();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Obtém todos os logs do serviço, ordena-os por data (mais recentes primeiro)
        /// e atualiza o cache local.
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
                _dialog.Erro($"Falha ao ler o histórico: {ex.Message}", "Erro");
            }
        }


        /// <summary>
        /// Executa a lógica de filtragem LINQ sobre o cache local com base no <see cref="FiltroGeral"/>.
        /// Atualiza a <see cref="ListaLogs"/> que está em Binding com a View.
        /// </summary>
        private void AplicarFiltroLocal()
        {
            IEnumerable<Registo> filtrados;

            if (string.IsNullOrWhiteSpace(FiltroGeral))
            {
                filtrados = _cacheLogs;
            }
            else
            {
                // Pesquisa case-insensitive em múltiplos campos da entidade Registo
                filtrados = _cacheLogs.Where(l =>
                    (l.TipoEntidade?.Contains(FiltroGeral, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (l.Operacao?.Contains(FiltroGeral, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (l.Utilizador?.Contains(FiltroGeral, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (l.Detalhes?.Contains(FiltroGeral, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            // Atualização atómica da ObservableCollection para refletir na UI
            ListaLogs.Clear();
            foreach (var log in filtrados)
            {
                ListaLogs.Add(log);
            }
        }
        

        #endregion
    }
}