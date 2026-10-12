//-----------------------------------------------------------------
//    <copyright file="IDialogService.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>1</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
namespace POOfase1trabalho13_11.ViewModels.Services
{
    /// <summary>
    /// Abstrai as caixas de diálogo para que as ViewModels não dependam de <c>MessageBox</c>
    /// e possam ser testadas sem interface gráfica.
    /// </summary>
    public interface IDialogService
    {
        #region Methods
        /// <summary>Mostra uma mensagem informativa.</summary>
        void Informar(string mensagem, string titulo = "Informação");

        /// <summary>Mostra um aviso (ex.: validação falhada).</summary>
        void Avisar(string mensagem, string titulo = "Atenção");

        /// <summary>Mostra uma mensagem de erro.</summary>
        void Erro(string mensagem, string titulo = "Erro");

        /// <summary>Pede confirmação (Sim/Não).</summary>
        /// <returns><c>true</c> se o utilizador escolher Sim.</returns>
        bool Confirmar(string mensagem, string titulo = "Confirmar");
        #endregion
    }
}
