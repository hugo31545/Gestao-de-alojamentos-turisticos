using POOfase1trabalho13_11.Models.Entities.Interfaces;
using POOfase1trabalho13_11.Models.Enums;
using System.Text.Json.Serialization;

namespace POOfase1trabalho13_11.Models.Entities
{
    /// <summary>
    /// Representa um alojamento turístico contendo detalhes como localização, tipo, preço e comodidades.
    /// </summary>
   
    public class Alojamento : IEntity
    {
        #region Private Fields
        private int _id;
        private string _nome = string.Empty;
        private string _localizacao = string.Empty;
        private decimal _precoPorNoite;
        private int _capacidade;
        private string _descricao = string.Empty;
        private List<string> _comodidades = new List<string>();
        private EstadoAlojamento _estado;
        private TipoAlojamento _tipo;
        #endregion

        #region Public Properties

        public int Id
        {
            get => _id;
            set 
            {
                if (value < 0)
                    throw new ArgumentException("ID nao pode ser negativo.", nameof(Id));
                _id = value;
            }
        }

        public string Nome
        {
            get => _nome;
            set
            {
                _nome = value ?? string.Empty;
            }
        }



        public string Localizacao
        {
            get => _localizacao;
            set
            {
                
                // Se o valor for nulo, garantimos uma string vazia para evitar erros de referência.
                _localizacao = value ?? string.Empty;
            }
        }



        public decimal PrecoPorNoite
        {
            get => _precoPorNoite;
            set => _precoPorNoite = value; 
        }

        public int Capacidade
        {
            get => _capacidade;
            set
            {
                
                _capacidade = value;
            }
        }
        public string Descricao
        {
            get => _descricao;
            set => _descricao = value ?? string.Empty;
        }

        /// <summary>
        /// Obtém a lista de comodidades. É exposto como IReadOnlyList para 
        /// prevenir modificações externas diretas.
        /// </summary>
        public IReadOnlyList<string> Comodidades
        {
            get => _comodidades;
            set => _comodidades = value != null ? new List<string>(value) : new List<string>();
        }

        public EstadoAlojamento Estado
        {
            get => _estado;
            set => _estado = value;
        }

        public TipoAlojamento Tipo
        {
            get => _tipo;
            set => _tipo = value;
        }

        #endregion

        #region Constructors
        // O atributo faz com que o JSON use este construtor para recriar o objeto do ficheiro
        [JsonConstructor]
        public Alojamento(
        int id,
        string nome,
        string localizacao,
        decimal precoPorNoite,
        int capacidade,
        string descricao,
        TipoAlojamento tipo,
        IReadOnlyList<string> comodidades, 
        EstadoAlojamento estado)
        {
            Id = id;
            Nome = nome;
            Localizacao = localizacao;
            PrecoPorNoite = precoPorNoite;
            Capacidade = capacidade;
            Descricao = descricao;
            Tipo = tipo;

            Comodidades = comodidades;
            Estado = estado;
        }


        public Alojamento()
        {
            _comodidades = new List<string>();
            _nome = string.Empty;
            _localizacao = string.Empty;
            _descricao = string.Empty;
            Estado = EstadoAlojamento.Disponivel;
        }
        #endregion

    }
}