//-----------------------------------------------------------------
//    <copyright file="ViewModelCommand.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using System;
using System.Windows.Input;

namespace POOfase1trabalho13_11.ViewModels.Commands
{
    /// <summary>
    /// Fornece uma implementação da interface <see cref="ICommand"/> baseada em delegados.
    /// Esta classe permite que as ViewModels exponham comandos para a View sem a necessidade
    /// de criar uma nova classe para cada comando individual.
    /// </summary>
    public class ViewModelCommand : ICommand
    {
        #region Delegates
        /// <summary>
        /// Define a assinatura do método que será executado pelo comando.
        /// </summary>
        public delegate void ICommandOnExecute(object? parameter);

        /// <summary>
        /// Define a assinatura do método que validará se o comando pode ser executado.
        /// </summary>
        public delegate bool ICommandOnCanExecute(object? parameter);
        #endregion

        #region Fields
        private readonly ICommandOnExecute _execute;
        private readonly ICommandOnCanExecute? _canExecute;
        #endregion

        #region Constructors

        /// <summary>
        /// Inicializa um comando que pode ser sempre executado.
        /// </summary>
        /// <param name="onExecuteMethod">Método a ser invocado no clique/ação.</param>
        public ViewModelCommand(ICommandOnExecute onExecuteMethod)
        {
            _execute = onExecuteMethod ?? throw new ArgumentNullException(nameof(onExecuteMethod));
        }

        /// <summary>
        /// Inicializa um comando com lógica de execução e de validação de estado.
        /// </summary>
        /// <param name="onExecuteMethod">Método a ser invocado.</param>
        /// <param name="onCanExecuteMethod">Método que retorna se o botão/comando deve estar ativo.</param>
        public ViewModelCommand(ICommandOnExecute onExecuteMethod, ICommandOnCanExecute onCanExecuteMethod)
        {
            _execute = onExecuteMethod ?? throw new ArgumentNullException(nameof(onExecuteMethod));
            _canExecute = onCanExecuteMethod ?? throw new ArgumentNullException(nameof(onCanExecuteMethod));
        }

        #endregion

        #region ICommand Members

        /// <summary>
        /// Evento disparado quando as condições de execução do comando mudam.
        /// Utiliza o <see cref="CommandManager.RequerySuggested"/> para atualizar a UI automaticamente.
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }


        /// <summary>
        /// Avalia se o comando pode ser executado.
        /// </summary>
        /// <param name="parameter">Dados passados pela View.</param>
        /// <returns>True se o comando estiver habilitado.</returns>
        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }


        /// <summary>
        /// Invoca a lógica principal do comando.
        /// </summary>
        /// <param name="parameter">Dados passados pela View.</param>
        public void Execute(object? parameter)
        {
            _execute.Invoke(parameter);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Força a reavaliação imediata da lógica do <see cref="CanExecute"/>.
        /// Útil quando o estado da UI muda mas o CommandManager ainda não detetou a alteração.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
        #endregion
    }
}