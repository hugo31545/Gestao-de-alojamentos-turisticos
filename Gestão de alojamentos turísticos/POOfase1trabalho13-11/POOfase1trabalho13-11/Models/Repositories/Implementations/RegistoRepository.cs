//-----------------------------------------------------------------
//    <copyright file="RegistoRepository.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Repositories.Interfaces;

namespace POOfase1trabalho13_11.Models.Repositories.Implementations
{
    /// <summary>
    /// Repositório especializado na gestão de logs de auditoria (<see cref="Registo"/>).
    /// Implementa um padrão de escrita única (Append-only) para garantir a integridade do histórico do sistema.
    /// </summary>
    public class RegistoRepository : JsonRepository<Registo>, IRegistoRepository
    {

        /// <summary>
        /// Inicializa uma nova instância de <see cref="RegistoRepository"/>,
        /// configurando o ficheiro "auditoria_logs.json" para armazenamento.
        /// </summary>
        public RegistoRepository() : base("auditoria_logs.json") { }


        /// <summary>
        /// Obtém todos os registos de auditoria associados a uma entidade específica e ao seu ID.
        /// </summary>
        /// <param name="tipoEntidade">O nome da classe da entidade (ex: "Reserva").</param>
        /// <param name="entidadeId">O ID da instância da entidade.</param>
        /// <returns>Coleção de registos correspondentes.</returns>
        public IEnumerable<Registo> GetByEntidade(string tipoEntidade, int entidadeId) =>
            Items.Where(r => r.TipoEntidade.Equals(tipoEntidade, StringComparison.OrdinalIgnoreCase) && r.EntidadeId == entidadeId);


        /// <summary>
        /// Filtra os logs de auditoria por um intervalo de datas.
        /// </summary>
        /// <param name="inicio">Data de início do período.</param>
        /// <param name="fim">Data de fim do período.</param>
        /// <returns>Coleção de registos dentro do intervalo temporal.</returns>
        public IEnumerable<Registo> GetByPeriodo(DateTime inicio, DateTime fim) =>
            Items.Where(r => r.DataRegisto >= inicio && r.DataRegisto <= fim);



        /// <summary>
        /// Obtém todas as ações registadas por um determinado utilizador.
        /// </summary>
        /// <param name="utilizador">O nome ou email do utilizador.</param>
        /// <returns>Coleção de registos efetuados pelo utilizador indicado.</returns>
        public IEnumerable<Registo> GetByUtilizador(string utilizador) =>
            Items.Where(r => r.Utilizador.Equals(utilizador, StringComparison.OrdinalIgnoreCase));


        /// <summary>
        /// Operação não suportada. Os logs de auditoria não permitem modificações após a criação.
        /// </summary>
        /// <exception cref="NotSupportedException">Lançada sempre que se tenta atualizar um log.</exception>
        public override bool Update(Registo entity) =>
            throw new NotSupportedException("Logs de auditoria não podem ser alterados.");


        /// <summary>
        /// Operação não suportada. Os logs de auditoria não podem ser removidos para garantir a rastreabilidade.
        /// </summary>
        /// <exception cref="NotSupportedException">Lançada sempre que se tenta remover um log.</exception>
        public override bool Remove(int id) =>
            throw new NotSupportedException("Logs de auditoria não podem ser removidos.");


        /// <summary>
        /// Adiciona um novo log de auditoria ao ficheiro, gerando automaticamente um identificador único.
        /// </summary>
        /// <param name="log">O objeto de log a registar.</param>
        public override void Add(Registo log)
        {
            // Auto-incremento de segurança para garantir a ordem cronológica dos logs
            if (log.Id <= 0)
                log.Id = Items.Any() ? Items.Max(r => r.Id) + 1 : 1;

            base.Add(log);
        }

    }
}