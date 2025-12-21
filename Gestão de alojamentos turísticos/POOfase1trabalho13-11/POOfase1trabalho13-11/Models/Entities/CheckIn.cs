//-----------------------------------------------------------------
//    <copyright file="CheckIn.cs" company="IPCA">
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
    /// Representa o registo de um check-in de um cliente para uma reserva específica.
    /// Armazena detalhes sobre a entrada no alojamento e o funcionário responsável.
    /// </summary>
    public class CheckIn : IEntity
    {
        #region Private Fields
        private int _id;
        private int _numeroHospedes;
        private string _funcionarioResponsavel = string.Empty;
        #endregion

        #region Public Properties

        /// <summary>
        /// Obtém ou define o identificador único do Check-In.
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
        /// Obtém ou define o ID da Reserva associada a este Check-In.
        /// </summary>
        public int ReservaId { get; set; }

        /// <summary>
        /// Obtém ou define a data e hora em que o Check-In foi realizado.
        /// </summary>
        public DateTime DataHora { get; set; }

        /// <summary>
        /// Obtém ou define o número de hóspedes presentes.
        /// </summary>
        /// <exception cref="ArgumentException">Lançada se o valor for menor ou igual a zero.</exception>
        public int NumeroHospedes
        {
            get => _numeroHospedes;
            set
            {
                if (value <= 0) throw new ArgumentException("Número de hóspedes deve ser maior que zero.");
                _numeroHospedes = value;
            }
        }

        /// <summary>
        /// Obtém ou define notas ou observações adicionais sobre a entrada.
        /// </summary>
        public string Observacoes { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define se os documentos de identificação foram verificados.
        /// </summary>
        public bool DocumentosVerificados { get; set; }

        /// <summary>
        /// Obtém ou define o nome do funcionário que realizou o procedimento.
        /// </summary>
        /// <exception cref="ArgumentException">Lançada se o nome estiver vazio ou nulo.</exception>
        public string FuncionarioResponsavel
        {
            get => _funcionarioResponsavel;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Funcionário responsável é obrigatório.");
                _funcionarioResponsavel = value;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="CheckIn"/> com todos os parâmetros.
        /// </summary>
        /// <param name="id">ID do Check-In.</param>
        /// <param name="reservaId">ID da Reserva.</param>
        /// <param name="dataHora">Data do evento.</param>
        /// <param name="funcionarioResponsavel">Nome do funcionário.</param>
        /// <param name="numeroHospedes">Total de pessoas.</param>
        /// <param name="observacoes">Notas extras.</param>
        /// <param name="documentosVerificados">Estado da verificação documental.</param>
        [JsonConstructor]
        public CheckIn(int id, int reservaId, DateTime dataHora, string funcionarioResponsavel, int numeroHospedes, string observacoes, bool documentosVerificados)
        {
            Id = id;
            ReservaId = reservaId;
            DataHora = dataHora == default ? DateTime.Now : dataHora;
            FuncionarioResponsavel = funcionarioResponsavel;
            NumeroHospedes = numeroHospedes;
            Observacoes = observacoes ?? string.Empty;
            DocumentosVerificados = documentosVerificados;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="CheckIn"/> com valores padrão.
        /// </summary>
        public CheckIn() { }

        #endregion

        #region Methods
        /// <summary>
        /// Gera uma chave de acesso única baseada nos dados da reserva e data.
        /// </summary>
        /// <returns>Uma string representando a chave de acesso ao alojamento.</returns>
        public string GerarChaveAcesso()
        {
            return $"KEY-{ReservaId}-{Id}-{DataHora:yyyyMMdd}";
        }

        
        #endregion
    }
}