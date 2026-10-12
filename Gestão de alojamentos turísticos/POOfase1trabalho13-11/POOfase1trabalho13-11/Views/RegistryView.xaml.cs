using System.Windows.Controls;

namespace POOfase1trabalho13_11.Views
{
    /// <summary>
    /// Formulário de registo de funcionário. Só trata do foco inicial: a lógica está na RegistryViewModel.
    /// </summary>
    public partial class RegistryView : UserControl
    {
        public RegistryView()
        {
            InitializeComponent();

            // O cursor começa no primeiro campo
            Loaded += (_, _) => CampoNome.FocusInput();
        }
    }
}
