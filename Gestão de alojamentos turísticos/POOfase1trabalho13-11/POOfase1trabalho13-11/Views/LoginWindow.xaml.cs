using System.Windows;

namespace POOfase1trabalho13_11.Views
{
    /// <summary>
    /// Janela de início de sessão. Só trata do foco inicial: a lógica está na LoginViewModel.
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            // O cursor começa no campo do email, para se poder escrever logo
            Loaded += (_, _) => CampoEmail.FocusInput();
        }
    }
}
