//-----------------------------------------------------------------
//    <copyright file="BaseViewModel.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace POOfase1trabalho13_11.ViewModels
{
    /// <summary>
    /// Classe base abstrata para todas as ViewModels do sistema.
    /// Implementa o mecanismo de notificação necessário para o Data Binding do WPF.
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        #region Events

        /// <summary>
        /// Evento disparado sempre que uma propriedade da ViewModel é alterada.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region Methods
        /// <summary>
        /// Atualiza o valor de uma propriedade e dispara a notificação para a View,
        /// mas apenas se o novo valor for diferente do atual.
        /// </summary>
        /// <typeparam name="T">O tipo da propriedade.</typeparam>
        /// <param name="storage">Referência para o campo privado (field).</param>
        /// <param name="value">O novo valor a ser atribuído.</param>
        /// <param name="propertyName">Nome da propriedade (preenchido automaticamente pelo compilador).</param>
        /// <returns>True se o valor foi alterado; False se o valor era idêntico.</returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            // Se o valor for o mesmo, não fazemos nada (otimização de performance)
            if (Equals(storage, value)) return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }


        /// <summary>
        /// Dispara manualmente o evento <see cref="PropertyChanged"/>.
        /// </summary>
        /// <param name="propertyName">Nome da propriedade que mudou.</param>
        protected void OnPropertyChanged(string? propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
