//-----------------------------------------------------------------
//    <copyright file="RelayCommand.cs" company="IPCA">
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
    /// Implementação genérica do padrão Command para o WPF.
    /// Permite expor métodos da ViewModel para a View com suporte a parâmetros tipados,
    /// facilitando o desacoplamento entre a UI e a lógica de negócio.
    /// </summary>
    /// <typeparam name="T">O tipo do parâmetro esperado pela lógica de execução do comando.</typeparam>
    public class RelayCommand<T> : ICommand
    {
        #region Fields
        /// <summary>
        /// Ação (delegate) que contém a lógica a ser executada pelo comando.
        /// </summary>
        private readonly Action<T> _execute;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="RelayCommand{T}"/>.
        /// </summary>
        /// <param name="execute">O método de destino a ser executado.</param>
        /// <exception cref="ArgumentNullException">Lançada se o parâmetro execute for nulo.</exception>
        public RelayCommand(Action<T> execute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }
        #endregion

        #region ICommand Members

        /// <summary>
        /// Determina se o comando pode ser executado no estado atual.
        /// Nesta implementação simplificada, retorna sempre true.
        /// </summary>
        /// <param name="parameter">Dados usados pelo comando.</param>
        /// <returns>Sempre verdadeiro (<c>true</c>).</returns>
        public bool CanExecute(object? parameter) => true;


        /// <summary>
        /// Executa a lógica associada ao comando, realizando o casting do parâmetro
        /// para o tipo genérico <typeparamref name="T"/>.
        /// </summary>
        /// <param name="parameter">O parâmetro passado pela View (ex: CommandParameter).</param>

        public void Execute(object? parameter)
        {
            if (parameter is T value)
            {
                _execute(value);
            }
            else if (parameter == null && !typeof(T).IsValueType)
            {
                // Permite a execução se o parâmetro for nulo e T for um tipo de referência
                _execute(default!);
            }
        }

        /// <summary>
        /// Evento disparado quando ocorrem alterações que afetam a viabilidade de execução do comando.
        /// Utiliza o <see cref="CommandManager.RequerySuggested"/> para forçar a atualização da UI.
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value; // faz a subscrição do evento
            remove => CommandManager.RequerySuggested -= value; // remove a subscrição do evento
        }
        #endregion
    }
} 