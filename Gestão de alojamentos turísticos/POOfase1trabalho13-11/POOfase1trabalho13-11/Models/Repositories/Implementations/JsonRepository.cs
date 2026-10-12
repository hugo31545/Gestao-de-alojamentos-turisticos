//-----------------------------------------------------------------
//    <copyright file="JsonRepository.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.Models.Entities.Interfaces;
using POOfase1trabalho13_11.Models.Repositories.Interfaces;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace POOfase1trabalho13_11.Models.Repositories.Implementations 
{

    /// <summary>
    /// Fornece uma implementação base genérica para repositórios que utilizam ficheiros JSON como suporte de dados.
    /// Gere automaticamente o carregamento, gravação e operações CRUD em memória com sincronização em disco.
    /// </summary>
    /// <typeparam name="TEntity">O tipo da entidade, que deve implementar <see cref="IEntity"/>.</typeparam>

    public abstract class JsonRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        #region Fields

        /// <summary>
        /// Caminho base para a pasta de dados da aplicação.
        /// </summary>
        private readonly string _basePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data"));

        /// <summary>
        /// Caminho completo para o ficheiro JSON específico desta entidade.
        /// </summary>
        private readonly string _filePath; // Caminho completo para o arquivo JSON


        /// <summary>
        /// Lista interna que mantém os itens em memória para acesso rápido.
        /// </summary>        
        protected List<TEntity> Items { get; set; } = new List<TEntity>();

        #endregion

        #region Constructors

        /// <summary>
        /// Inicializa uma nova instância do repositório genérico, garante a criação do diretório e carrega os dados existentes.
        /// </summary>
        /// <param name="fileName">Nome do ficheiro (ex: "clientes.json").</param>
        public JsonRepository(string fileName)
        {
            _filePath = Path.Combine(_basePath, fileName);
            try {
                // 1. Garantir que o diretório exista
                if (!Directory.Exists(_basePath))
                    Directory.CreateDirectory(_basePath);

                // 2. Garantir que o ficheiro exista com uma estrutura JSON válida (Array vazio)
                if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
            }


                // Carrega os dados utilizando Streaming para melhor performance
                LoadFromFileStreaming();
            }
            catch (Exception ex)
            {
                throw new IOException($"Falha crítica ao inicializar o repositório em {_filePath}", ex);
            }
        }



        #endregion

        #region Persistence Methods (Internal)

        /// <summary>
        /// Lê o ficheiro JSON de forma eficiente utilizando Streams e desserializa para a lista em memória.
        /// </summary>
        private void LoadFromFileStreaming()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    Items = new List<TEntity>();
                    return;
                }

                // Verificação de integridade mínima do ficheiro
                FileInfo info = new FileInfo(_filePath);
                if (info.Length < 2)
                {
                    Items = new List<TEntity>();
                    return;
                }

                using (FileStream fs = File.OpenRead(_filePath))
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<List<TEntity>>(fs, options);

                    Items = result ?? new List<TEntity>();

                    
                    System.Diagnostics.Debug.WriteLine($">>> REPOSITÓRIO: Carregados {Items.Count} itens de {typeof(TEntity).Name}");
                }
            }
            catch (JsonException jex)
            {
                
                System.Diagnostics.Debug.WriteLine($"ERRO DE FORMATO JSON: {jex.Message}");
                string? copia = PreservarFicheiroDanificado();
                MessageBox.Show($"Erro de formato no ficheiro {Path.GetFileName(_filePath)}. O ficheiro pode estar corrompido." +
                                (copia != null ? $"\nFoi guardada uma cópia em {Path.GetFileName(copia)}." : string.Empty),
                                "Erro de Dados");
                Items = new List<TEntity>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERRO CRÍTICO: {ex.Message}");
                PreservarFicheiroDanificado();
                Items = new List<TEntity>();
            }
        }

        /// <summary>
        /// Copia um ficheiro que não foi possível ler para <c>nome.corrupt-AAAAMMDD-HHMMSS.json</c>,
        /// para que a próxima gravação (com a lista vazia) não apague os dados originais.
        /// </summary>
        /// <returns>O caminho da cópia, ou <c>null</c> se não foi possível criá-la.</returns>
        private string? PreservarFicheiroDanificado()
        {
            try
            {
                if (!File.Exists(_filePath)) return null;

                string copia = Path.Combine(
                    Path.GetDirectoryName(_filePath)!,
                    $"{Path.GetFileNameWithoutExtension(_filePath)}.corrupt-{DateTime.Now:yyyyMMdd-HHmmss}{Path.GetExtension(_filePath)}");

                File.Copy(_filePath, copia, overwrite: false);
                return copia;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Não foi possível copiar o ficheiro danificado: {ex.Message}");
                return null;
            }
        }


        /// <summary>
        /// Serializa a lista de itens e grava no disco de forma atómica utilizando um ficheiro temporário.
        /// </summary>
        /// <exception cref="IOException">Lançada se ocorrer um erro durante a escrita ou substituição do ficheiro.</exception>
        protected void SaveToFile()
        {
            string tempFile = _filePath + ".tmp";
            try
            {
                var options = new JsonSerializerOptions { 
                    WriteIndented = true,
                    PropertyNameCaseInsensitive = true // Para garantir que o motor JSON encontre as propriedades 
                                                       // mesmo que o ficheiro antigo ainda tenha nomes diferentes
                };

                // 1. Grava no ficheiro temporário para evitar corrupção em caso de quebra de crash
                using (FileStream fs = File.Create(tempFile))
                {
                    JsonSerializer.Serialize(fs, Items, options);
                }

                // 2. Se a gravação correu bem, substituímos o arquivo real pelo temporário
                // O parâmetro 'true' permite substituir se o destino já existir
                File.Move(tempFile, _filePath, true);
            }
            catch (Exception ex)
            {
                if (File.Exists(tempFile)) File.Delete(tempFile);
                throw new IOException($"Erro ao persistir dados em {_filePath}", ex);
                
               
            }
        }

        /// <summary>
        /// Guarda uma entidade, decidindo se deve adicionar uma nova ou atualizar uma existente com base no ID.
        /// </summary>
        public virtual bool Save(TEntity entity)
        {
            if (entity == null) return false;

            // Se o ID for 0 ou menor, ou se o ID não for encontrado na nossa lista atual
            if (entity.Id <= 0 || GetById(entity.Id) == null)
            {
                // Chama o Add. 
                // Se for um Alojamento, ele vai usar o seu 'override' que gera o ID automático!
                Add(entity);
                return true;
            }

            // Se o ID já existe, ele apenas atualiza os dados na memória e grava no ficheiro
            return Update(entity);
        }
        #endregion

        #region Basic CRUD Methods

        /// <summary>
        /// Devolve todos os itens do repositório como uma coleção apenas de leitura.
        /// </summary>
        public virtual IEnumerable<TEntity> GetAll()
        {
            return Items.AsReadOnly(); // Retorna ReadOnly para proteger a lista interna
        }

        /// <summary>
        /// Adiciona uma nova entidade à lista e sincroniza com o ficheiro.
        /// </summary>
        public virtual void Add(TEntity entity)
        {
            
            Items.Add(entity);
            SaveToFile();
        }

        /// <summary>
        /// Procura uma entidade pelo seu identificador único.
        /// </summary>
        public virtual TEntity? GetById(int id)
        {
            return Items.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// Atualiza os dados de uma entidade existente em memória e persiste no disco.
        /// </summary>
        public virtual bool Update(TEntity entity)
        {
            if (entity == null) return false;

            // Encontra o índice pelo ID na memória (sem aceder ao disco)
            int index = Items.FindIndex(x => x.Id == entity.Id);

            if (index != -1)
            {
                Items[index] = entity;
                SaveToFile(); // Só acede ao disco para gravar
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove uma entidade do sistema através do seu ID e atualiza o ficheiro.
        /// </summary>
        public virtual bool Remove(int id)
        {
            
            int removedCount = Items.RemoveAll(x => x.Id == id);

            if (removedCount > 0)
            {
                SaveToFile();
                return true;
            }
            return false;
        }

        
        #endregion
    }
}