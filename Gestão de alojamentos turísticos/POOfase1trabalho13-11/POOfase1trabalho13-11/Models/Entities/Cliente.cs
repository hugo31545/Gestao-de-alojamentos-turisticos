
using POOfase1trabalho13_11.Models.Entities.Interfaces;
using System;
using System.Text.Json.Serialization;

namespace POOfase1trabalho13_11.Models.Entities
{
    /// <summary>
    /// Representa um cliente com informações pessoais, contacto e documento de identificação.
    /// </summary>
    public class Cliente : IEntity
    {
        #region Private Fields
        private int _id;
        private string _nome = string.Empty;
        private string _email = string.Empty;
        private string _telefone = string.Empty;
        private string _nif = string.Empty;
        #endregion

        #region Public Properties

        public int Id
        {
            get => _id;
            set
            {
                if (value < 0)
                    throw new ArgumentException("ID não pode ser negativo.", nameof(Id));
                _id = value;
            }
        }

        public string Nome
        {
            get => _nome;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("O nome é obrigatório.", nameof(Nome));
                _nome = value;
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                
                _email = value ?? string.Empty;
            }
        }

        public string Telefone
        {
            get => _telefone;
            set => _telefone = value ?? string.Empty;
        }

        /// <summary>
        /// Número de Identificação Fiscal (NIF).
        /// </summary>
        public string Nif
        {
            get => _nif;
            set
            {
                
                _nif = value ?? string.Empty;
            }
        }
        public DateTime DataRegisto { get; set; }

        #endregion

        #region Constructors

        [JsonConstructor]
        public Cliente(int id, string nome, string email, string telefone, string nif, DateTime dataRegisto)
        {
            Id = id;
            Nome = nome;
            Email = email;
            Telefone = telefone;
            Nif = nif;
            // Se for um novo cliente (DataRegisto padrão), usamos Now, caso contrário usamos o que vem do JSON
            DataRegisto = dataRegisto == default ? DateTime.Now : dataRegisto;
        }

        // Construtor necessário para o funcionamento do motor JSON
        public Cliente() { }

        #endregion

        
    }
}