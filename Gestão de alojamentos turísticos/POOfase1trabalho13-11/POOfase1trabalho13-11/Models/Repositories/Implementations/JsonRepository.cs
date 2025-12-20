using POOfase1trabalho13_11.Models.Entities.Interfaces;
using POOfase1trabalho13_11.Models.Repositories.Interfaces;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace POOfase1trabalho13_11.Models.Repositories.Implementations 
{
    // Classe base que lida com a persistência genérica de JSON
    
    public abstract class JsonRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        #region Fields

        
        private readonly string _basePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data"));
        private readonly string _filePath; // Caminho completo para o arquivo JSON

        // A lista de itens será armazenada em memória
        
        protected List<TEntity> Items { get; set; } = new List<TEntity>();

        #endregion

        #region Constructors

        /// <summary>
        /// Inicializa o repositório JSON, garantindo que o arquivo exista e carregando os dados.
        /// </summary>
        /// <param name="fileName">O nome do arquivo JSON (ex: "alojamentos.json").</param>
        public JsonRepository(string fileName)
        {
            _filePath = Path.Combine(_basePath, fileName);
            try {
                // 1. Garantir que o diretório exista
                if (!Directory.Exists(_basePath))
                    Directory.CreateDirectory(_basePath);

                // 2. Garantir que o arquivo exista e seja um array JSON vazio
                if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
            }


            // Agora carregamos usando Streaming
            LoadFromFileStreaming();
            }
            catch (Exception ex)
            {
                throw new IOException($"Falha crítica ao inicializar o repositório em {_filePath}", ex);
            }
        }



        #endregion

        #region Persistence Methods (Internal)

        private void LoadFromFileStreaming()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    Items = new List<TEntity>();
                    return;
                }

                // Se o ficheiro estiver vazio (0 bytes), o Deserialize falha. 
                // Vamos garantir que tem pelo menos o conteúdo mínimo "[]"
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

                    // LOG PARA O OUTPUT DO VISUAL STUDIO
                    System.Diagnostics.Debug.WriteLine($">>> REPOSITÓRIO: Carregados {Items.Count} itens de {typeof(TEntity).Name}");
                }
            }
            catch (JsonException jex)
            {
                // Se chegar aqui, o ficheiro JSON está corrompido ou o formato mudou
                System.Diagnostics.Debug.WriteLine($"ERRO DE FORMATO JSON: {jex.Message}");
                MessageBox.Show($"Erro de formato no ficheiro {Path.GetFileName(_filePath)}. O ficheiro pode estar corrompido.", "Erro de Dados");
                Items = new List<TEntity>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERRO CRÍTICO: {ex.Message}");
                Items = new List<TEntity>();
            }
        }

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

                // 1. Gravamos via Stream num arquivo TEMPORÁRIO
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
                // Limpeza: se falhou, tentamos apagar o lixo (arquivo temporário)
               
            }
        }


        public virtual bool Save(TEntity entity)
        {
            if (entity == null) return false;

            // Se o ID for 0 ou menor, ou se o ID não for encontrado na nossa lista atual
            if (entity.Id <= 0 || GetById(entity.Id) == null)
            {
                // Aqui ele chama o Add. 
                // Se for um Alojamento, ele vai usar o seu 'override' que gera o ID automático!
                Add(entity);
                return true;
            }

            // Se o ID já existe, ele apenas atualiza os dados na memória e grava no ficheiro
            return Update(entity);
        }
        #endregion

        #region Basic CRUD Methods

        
        public virtual IEnumerable<TEntity> GetAll()
        {
            return Items.AsReadOnly(); // Retorna ReadOnly para proteger a lista interna
        }

        // Adicionar um item e salvar
        public virtual void Add(TEntity entity)
        {
            
            Items.Add(entity);
            SaveToFile();
        }

        public virtual TEntity? GetById(int id)
        {
            return Items.FirstOrDefault(x => x.Id == id);
        }

        //  Atualizar um item
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

        public virtual bool Remove(int id)
        {
            // Remove todos os itens que tenham o ID correspondente
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