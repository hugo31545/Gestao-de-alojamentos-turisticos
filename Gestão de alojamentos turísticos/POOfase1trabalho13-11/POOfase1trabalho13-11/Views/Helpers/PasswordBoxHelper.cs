//-----------------------------------------------------------------
//    <copyright file="PasswordBoxHelper.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>1</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using System.Windows;
using System.Windows.Controls;

namespace POOfase1trabalho13_11.Views.Helpers
{
    /// <summary>
    /// Propriedade anexada que permite fazer binding da password de um <see cref="PasswordBox"/>
    /// (o WPF não permite binding direto a <see cref="PasswordBox.Password"/>).
    /// Uso: <c>helpers:PasswordBoxHelper.BoundPassword="{Binding Password, Mode=TwoWay}"</c>.
    /// </summary>
    public static class PasswordBoxHelper
    {
        #region Attached Properties
        public static readonly DependencyProperty BoundPasswordProperty =
            DependencyProperty.RegisterAttached(
                "BoundPassword",
                typeof(string),
                typeof(PasswordBoxHelper),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnBoundPasswordChanged));

        public static string GetBoundPassword(DependencyObject d) => (string)d.GetValue(BoundPasswordProperty);

        public static void SetBoundPassword(DependencyObject d, string value) => d.SetValue(BoundPasswordProperty, value);
        #endregion

        #region Handlers
        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not PasswordBox box) return;

            // Evita subscrever duas vezes e só atualiza a caixa quando o valor vem da ViewModel
            box.PasswordChanged -= OnPasswordChanged;
            string novo = e.NewValue as string ?? string.Empty;
            if (box.Password != novo) box.Password = novo;
            box.PasswordChanged += OnPasswordChanged;
        }

        private static void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            var box = (PasswordBox)sender;
            SetBoundPassword(box, box.Password);
        }
        #endregion
    }
}
