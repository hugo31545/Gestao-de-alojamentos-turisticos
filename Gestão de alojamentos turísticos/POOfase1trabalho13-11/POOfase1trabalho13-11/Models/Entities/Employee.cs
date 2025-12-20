using System;
using System.Text.Json.Serialization;
using POOfase1trabalho13_11.Models.Entities.Interfaces;

namespace POOfase1trabalho13_11.Models.Entities
{
    /// <summary>
    /// Representa um funcionário (utilizador interno) do sistema de gestão.
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

        public int Id
        {
            get => _id;
            set
            {
                if (value < 0) throw new ArgumentException("ID não pode ser negativo.");
                _id = value;
            }
        }

        public string Nome
        {
            get => _nome;
            set => _nome = string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("Nome é obrigatório.") : value;
        }

        public string NIF
        {
            get => _nif;
            set => _nif = value?.Length == 9 ? value : throw new ArgumentException("NIF deve ter 9 dígitos.");
        }

        public string Cargo
        {
            get => _cargo;
            set => _cargo = value ?? "Rececionista";
        }

        // Email ou Nome de Utilizador para ligação ao User de Login
        public string Email { get; set; } = string.Empty;

        public DateTime DataContratacao { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Construtor para o desserializador JSON.
        /// </summary>
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
        /// Construtor padrão.
        /// </summary>
        public Employee()
        {
            DataContratacao = DateTime.Now;
        }

        #endregion

    }
}