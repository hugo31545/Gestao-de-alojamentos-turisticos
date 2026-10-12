//-----------------------------------------------------------------
//    <copyright file="FormField.xaml.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>1</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using System.Windows;
using System.Windows.Controls;

namespace POOfase1trabalho13_11.Views.Controls
{
    /// <summary>
    /// Campo de formulário reutilizável: etiqueta, caixa de texto (ou de password),
    /// texto de ajuda (placeholder) e mensagem de erro por baixo.
    /// Não tem lógica de negócio: apenas apresenta o que a ViewModel fornece
    /// (<see cref="Text"/> e <see cref="Error"/>).
    /// </summary>
    public partial class FormField : UserControl
    {
        #region Dependency Properties
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string), typeof(FormField), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(FormField), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty IsPasswordProperty =
            DependencyProperty.Register(nameof(IsPassword), typeof(bool), typeof(FormField), new PropertyMetadata(false));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(FormField),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTextChanged));

        public static readonly DependencyProperty ErrorProperty =
            DependencyProperty.Register(nameof(Error), typeof(string), typeof(FormField), new PropertyMetadata(string.Empty, OnErrorChanged));

        public static readonly DependencyProperty HasErrorProperty =
            DependencyProperty.Register(nameof(HasError), typeof(bool), typeof(FormField), new PropertyMetadata(false));

        public static readonly DependencyProperty IsEmptyProperty =
            DependencyProperty.Register(nameof(IsEmpty), typeof(bool), typeof(FormField), new PropertyMetadata(true));
        #endregion

        #region Properties
        /// <summary>Etiqueta apresentada acima do campo.</summary>
        public string Label { get => (string)GetValue(LabelProperty); set => SetValue(LabelProperty, value); }

        /// <summary>Texto de ajuda apresentado dentro do campo enquanto está vazio.</summary>
        public string Placeholder { get => (string)GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }

        /// <summary>Se verdadeiro, o campo esconde os caracteres (password).</summary>
        public bool IsPassword { get => (bool)GetValue(IsPasswordProperty); set => SetValue(IsPasswordProperty, value); }

        /// <summary>Valor do campo (binding bidirecional).</summary>
        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

        /// <summary>Mensagem de erro; vazia significa que o valor está correto.</summary>
        public string Error { get => (string)GetValue(ErrorProperty); set => SetValue(ErrorProperty, value); }

        /// <summary>Verdadeiro quando existe uma mensagem de erro.</summary>
        public bool HasError { get => (bool)GetValue(HasErrorProperty); private set => SetValue(HasErrorProperty, value); }

        /// <summary>Verdadeiro quando o campo não tem texto (mostra o placeholder).</summary>
        public bool IsEmpty { get => (bool)GetValue(IsEmptyProperty); private set => SetValue(IsEmptyProperty, value); }
        #endregion

        #region Fields
        private bool _atualizando;
        #endregion

        public FormField()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Coloca o cursor neste campo (usado para o foco inicial dos ecrãs).
        /// </summary>
        public void FocusInput() => (IsPassword ? (Control)EntradaPassword : EntradaTexto).Focus();

        #region Handlers
        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var campo = (FormField)d;
            string novo = e.NewValue as string ?? string.Empty;
            campo.IsEmpty = novo.Length == 0;

            // Alterações vindas da ViewModel (ex.: limpar o formulário) chegam à caixa de password
            if (!campo._atualizando && campo.EntradaPassword.Password != novo)
            {
                campo._atualizando = true;
                campo.EntradaPassword.Password = novo;
                campo._atualizando = false;
            }
        }

        private static void OnErrorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((FormField)d).HasError = !string.IsNullOrEmpty(e.NewValue as string);
        }

        private void EntradaPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_atualizando) return;

            _atualizando = true;
            Text = EntradaPassword.Password;
            _atualizando = false;
        }
        #endregion
    }
}
