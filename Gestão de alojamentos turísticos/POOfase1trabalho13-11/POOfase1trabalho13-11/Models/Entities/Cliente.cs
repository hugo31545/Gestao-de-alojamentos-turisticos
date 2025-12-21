//-----------------------------------------------------------------
//    <copyright file="Cliente.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.Models.Entities.Interfaces;
using System;
using System.Text.Json.Serialization;

namespace POOfase1trabalho13_11.Models.Entities
{
    /// <summary>
    /// Representa um cliente no sistema com informações pessoais, contactos e dados fiscais.
    /// Implementa a interface <see cref="IEntity"/>.
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

        /// <summary>
        /// Obtém ou define o identificador único do cliente.
        /// </summary>
        /// <exception cref="ArgumentException">Lançada se o ID for negativo.</exception>
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

        /// <summary>
        /// Obtém ou define o nome completo do cliente.
        /// </summary>
        /// <exception cref="ArgumentException">Lançada se o nome for nulo ou apenas espaços em branco.</exception>
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

        /// <summary>
        /// Obtém ou define o endereço de correio eletrónico do cliente.
        /// </summary>
        public string Email
        {
            get => _email;
            set
            {
                
                _email = value ?? string.Empty;
            }
        }

        /// <summary>
        /// Obtém ou define o número de telefone de contacto do cliente.
        /// </summary>
        public string Telefone
        {
            get => _telefone;
            set => _telefone = value ?? string.Empty;
        }

        /// <summary>
        /// Obtém ou define o Número de Identificação Fiscal (NIF) do cliente.
        /// </summary>
        public string Nif
        {
            get => _nif;
            set
            {
                
                _nif = value ?? string.Empty;
            }
        }


        /// <summary>
        /// Obtém ou define a data em que o cliente foi registado no sistema.
        /// </summary>
        public DateTime DataRegisto { get; set; }

        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Cliente"/> com parâmetros.
        /// Utilizado para reconstruir o objeto a partir de dados persistidos (JSON).
        /// </summary>
        /// <param name="id">ID do cliente.</param>
        /// <param name="nome">Nome completo.</param>
        /// <param name="email">Email de contacto.</param>
        /// <param name="telefone">Telefone de contacto.</param>
        /// <param name="nif">Número de contribuinte.</param>
        /// <param name="dataRegisto">Data original de registo.</param>
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

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Cliente"/> com valores por defeito.
        /// Necessário para a correta operação da serialização JSON.
        /// </summary>
        public Cliente() { }

        #endregion

        
    }
}