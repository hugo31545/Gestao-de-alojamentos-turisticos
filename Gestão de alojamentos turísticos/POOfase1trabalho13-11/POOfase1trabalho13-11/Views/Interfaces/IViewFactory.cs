

using POOfase1trabalho13_11.Views.Enums;
using System.Windows;

namespace POOfase1trabalho13_11.Views.Interfaces
{
    /// <summary>
    /// Define o contrato para a fábrica de vistas, abstraindo a criação de janelas
    /// e a associação das suas respectivas ViewModels através de Injeção de Dependência.
    /// </summary>
    public interface IViewFactory
    {
        #region Methods

        
        Window CreateView(ViewType type, object? parameter = null);

        #endregion
    }
}