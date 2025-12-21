//-----------------------------------------------------------------
//    <copyright file="Employee.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using System;
using System.Text.Json.Serialization;
using POOfase1trabalho13_11.Models.Entities.Interfaces;

namespace POOfase1trabalho13_11.Models.Entities
{
    /// <summary>
    /// Representa um funcionário (utilizador interno) do sistema de gestão.
    /// Armazena dados de identificação, cargo e vínculo contratual.
    /// Implementa a interface <see cref="IEntity"/>.
    /// </summary>
    public class Employee : IEntity
    {
        #region Private Fields
        private int _id;
        private string _nome = string.Empty;
        private string _nif = string.Empty;
        private string _cargo = "Rececionista";
        #endregion

        #region Public Properties

        /// <summary>
        /// Obtém ou define o identificador único do funcionário.
        /// </summary>
        /// <exception cref="ArgumentException">Lançada se o ID for negativo.</exception>
        public int Id
        {
            get => _id;
            set
            {
                if (value < 0) throw new ArgumentException("ID não pode ser negativo.");
                _id = value;
            }
        }

        /// <summary>
        /// Obtém ou define o nome completo do funcionário.
        /// </summary>
        /// <exception cref="ArgumentException">Lançada se o nome for nulo ou vazio.</exception>
        public string Nome
        {
            get => _nome;
            set => _nome = string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("Nome é obrigatório.") : value;
        }

        /// <summary>
        /// Obtém ou define o Número de Identificação Fiscal (NIF) do funcionário.
        /// </summary>
        /// <exception cref="ArgumentException">Lançada se o NIF não contiver exatamente 9 dígitos.</exception>
        public string NIF
        {
            get => _nif;
            set => _nif = value?.Length == 9 ? value : throw new ArgumentException("NIF deve ter 9 dígitos.");
        }

        /// <summary>
        /// Obtém ou define o cargo ou função desempenhada.
        /// </summary>
        public string Cargo
        {
            get => _cargo;
            set => _cargo = value ?? "Rececionista";
        }

        /// <summary>
        /// Obtém ou define o email, utilizado para a ligação ao perfil de utilizador (Login).
        /// </summary>
        
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define a data em que o funcionário foi contratado.
        /// </summary>
        public DateTime DataContratacao { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Employee"/> com parâmetros detalhados.
        /// Utilizado principalmente para a desserialização de dados a partir de ficheiros JSON.
        /// </summary>
        /// <param name="id">ID do funcionário.</param>
        /// <param name="nome">Nome completo.</param>
        /// <param name="nif">Número de contribuinte.</param>
        /// <param name="cargo">Cargo ocupado.</param>
        /// <param name="email">Email de ligação ao utilizador.</param>
        /// <param name="dataContratacao">Data de início de contrato.</param>
        [JsonConstructor]
        public Employee(int id, string nome, string nif, string cargo, string email, DateTime dataContratacao)
        {
            Id = id;
            Nome = nome;
            NIF = nif;
            Cargo = cargo;
            Email = email;
            DataContratacao = dataContratacao == default ? DateTime.Now : dataContratacao;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Employee"/> com valores padrão.
        /// Define a data de contratação como o momento atual.
        /// </summary>
        public Employee()
        {
            DataContratacao = DateTime.Now;
        }

        #endregion

    }
}