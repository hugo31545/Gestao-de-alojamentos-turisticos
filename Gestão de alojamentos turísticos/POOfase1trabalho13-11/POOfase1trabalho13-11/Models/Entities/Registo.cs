//-----------------------------------------------------------------
//    <copyright file="Registo.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.Models.Entities.Interfaces;
using System.Text.Json.Serialization;

namespace POOfase1trabalho13_11.Models.Entities
{
    /// <summary>
    /// Representa um log de auditoria do sistema para rastrear operações realizadas.
    /// Esta classe é essencial para garantir a integridade e o histórico de alterações no sistema.
    /// Implementa a interface <see cref="IEntity"/>.
    /// </summary>
    public class Registo : IEntity
    {
        #region Private Fields
        private int _id;
        private string _tipoEntidade = string.Empty;
        private string _detalhes = string.Empty;
        private string _utilizador = string.Empty;
        private string _operacao = string.Empty;
        #endregion

        #region Public Properties

        /// <summary>
        /// Obtém ou define o identificador único do registo de auditoria.
        /// </summary>
        /// <exception cref="ArgumentException">Lançada se o ID for negativo.</exception>
        public int Id
        {
            get => _id;
            set
            {
                if (value < 0) throw new ArgumentException("ID inválido.");
                _id = value;
            }
        }

        /// <summary>
        /// Obtém ou define o tipo de entidade afetada pela operação (ex: Alojamento, Reserva).
        /// </summary>
        public string TipoEntidade
        {
            get => _tipoEntidade;
            set => _tipoEntidade = string.IsNullOrWhiteSpace(value) ? "Desconhecido" : value;
        }

        /// <summary>
        /// Obtém ou define a descrição detalhada da operação realizada.
        /// </summary>
        public string Detalhes
        {
            get => _detalhes;
            set => _detalhes = value ?? string.Empty;
        }

        /// <summary>
        /// Obtém ou define o nome do utilizador que realizou a ação. 
        /// Assume "Sistema" caso não seja especificado.
        /// </summary>
        public string Utilizador
        {
            get => _utilizador;
            set => _utilizador = string.IsNullOrWhiteSpace(value) ? "Sistema" : value;
        }

        /// <summary>
        /// Obtém ou define o tipo de operação executada (ex: CREATE, UPDATE, DELETE).
        /// </summary>
        public string Operacao
        {
            get => _operacao;
            set => _operacao = value ?? "LOG";
        }

        /// <summary>
        /// Obtém ou define o identificador da entidade específica que foi alvo da operação.
        /// </summary>
        public int EntidadeId { get; set; }

        /// <summary>
        /// Obtém ou define a data e hora em que a operação foi registada.
        /// </summary>
        public DateTime DataRegisto { get; set; }

        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Registo"/> com parâmetros.
        /// Utilizado principalmente para reconstruir o histórico a partir de persistência JSON.
        /// </summary>
        /// <param name="id">ID do log.</param>
        /// <param name="tipoEntidade">Classe do objeto afetado.</param>
        /// <param name="detalhes">Descrição do evento.</param>
        /// <param name="utilizador">Autor da ação.</param>
        /// <param name="operacao">Tipo de comando executado.</param>
        /// <param name="entidadeId">ID do objeto afetado.</param>
        /// <param name="dataRegisto">Data do evento.</param>
        [JsonConstructor]
        public Registo(int id, string tipoEntidade, string detalhes, string utilizador, string operacao, int entidadeId, DateTime dataRegisto)
        {
            Id = id;
            TipoEntidade = tipoEntidade;
            Detalhes = detalhes;
            Utilizador = utilizador;
            Operacao = operacao;
            EntidadeId = entidadeId;
            DataRegisto = dataRegisto == default ? DateTime.Now : dataRegisto;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Registo"/> com a data atual.
        /// </summary>
        public Registo()
        {
            DataRegisto = DateTime.Now;
        }
        #endregion

    }
}