//-----------------------------------------------------------------
//    <copyright file="DialogService.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>1</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.ViewModels.Services;
using System.Windows;

namespace POOfase1trabalho13_11.Views.Services
{
    /// <summary>
    /// Implementação WPF de <see cref="IDialogService"/> com <see cref="MessageBox"/>.
    /// </summary>
    public class DialogService : IDialogService
    {
        #region Methods
        public void Informar(string mensagem, string titulo = "Informação") =>
            MessageBox.Show(mensagem, titulo, MessageBoxButton.OK, MessageBoxImage.Information);

        public void Avisar(string mensagem, string titulo = "Atenção") =>
            MessageBox.Show(mensagem, titulo, MessageBoxButton.OK, MessageBoxImage.Warning);

        public void Erro(string mensagem, string titulo = "Erro") =>
            MessageBox.Show(mensagem, titulo, MessageBoxButton.OK, MessageBoxImage.Error);

        public bool Confirmar(string mensagem, string titulo = "Confirmar") =>
            MessageBox.Show(mensagem, titulo, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        #endregion
    }
}
