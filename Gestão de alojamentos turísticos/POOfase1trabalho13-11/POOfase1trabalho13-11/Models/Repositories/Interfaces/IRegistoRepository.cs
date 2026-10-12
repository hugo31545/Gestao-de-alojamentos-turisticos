//-----------------------------------------------------------------
//    <copyright file="IRegistoRepository.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.Models.Entities;

namespace POOfase1trabalho13_11.Models.Repositories.Interfaces
{

    /// <summary>
    /// Define o contrato para as operações de gestão de logs de auditoria e eventos do sistema.
    /// Herda as operações base da interface genérica <see cref="IRepository{T}"/>.
    /// </summary>
    public interface IRegistoRepository : IRepository<Registo>
    {

        #region Methods

        /// <summary>
        /// Procura todos os registos de auditoria relacionados com uma entidade específica.
        /// </summary>
        /// <param name="tipoEntidade">O nome do tipo da entidade (ex: "Alojamento", "Reserva").</param>
        /// <param name="entidadeId">O identificador único da instância da entidade.</param>
        /// <returns>Uma coleção de <see cref="Registo"/> que correspondem aos critérios.</returns>
        IEnumerable<Registo> GetByEntidade(string tipoEntidade, int entidadeId);


        /// <summary>
        /// Obtém os registos de auditoria efetuados dentro de um intervalo de tempo específico.
        /// </summary>
        /// <param name="inicio">Data e hora de início da pesquisa.</param>
        /// <param name="fim">Data e hora de fim da pesquisa.</param>
        /// <returns>Uma coleção de <see cref="Registo"/> ocorridos no período indicado.</returns>
        IEnumerable<Registo> GetByPeriodo(DateTime inicio, DateTime fim);

        /// <summary>
        /// Procura todas as ações registadas por um utilizador específico no sistema.
        /// </summary>
        /// <param name="utilizador">O nome de utilizador ou identificador do autor da ação.</param>
        /// <returns>Uma coleção de <see cref="Registo"/> associados ao utilizador.</returns>
        IEnumerable<Registo> GetByUtilizador(string utilizador);

        /// <summary>
        /// Elimina todos os registos anteriores à data de corte (política de retenção).
        /// </summary>
        /// <param name="dataCorte">Registos com data inferior a esta são eliminados.</param>
        /// <returns>O número de registos eliminados.</returns>
        int RemoverAnterioresA(DateTime dataCorte);
        #endregion
    }
}