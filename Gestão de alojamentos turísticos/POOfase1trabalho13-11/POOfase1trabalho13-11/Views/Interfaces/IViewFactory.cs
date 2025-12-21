//-----------------------------------------------------------------
//    <copyright file="IViewFactory.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------

using POOfase1trabalho13_11.Views.Enums;
using System.Windows;

namespace POOfase1trabalho13_11.Views.Interfaces
{
    /// <summary>
    /// Define o contrato para a fábrica de vistas (Views) da aplicação.
    /// Esta interface abstrai a lógica de criação de janelas WPF, garantindo o 
    /// desacoplamento entre a navegação e a implementação física das janelas, 
    /// facilitando a Injeção de Dependências e o padrão MVVM.
    /// </summary>
    public interface IViewFactory
    {
        #region Methods

        /// <summary>
        /// Cria e configura uma instância de uma janela (<see cref="Window"/>) baseada no tipo especificado.
        /// </summary>
        /// <param name="type">O tipo de vista solicitado através do enumerador <see cref="ViewType"/>.</param>
        /// <param name="parameter">
        /// Parâmetro opcional para passar dados contextuais à ViewModel da nova janela 
        /// (ex: IDs, nomes de utilizador ou entidades para edição).
        /// </param>
        /// <returns>Uma instância de <see cref="Window"/> devidamente inicializada com o seu DataContext.</returns>
        Window CreateView(ViewType type, object? parameter = null);

        #endregion
    }
}