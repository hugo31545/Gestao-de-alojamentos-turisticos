//-----------------------------------------------------------------
//    <copyright file="ISessionService.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>1</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
namespace POOfase1trabalho13_11.Models.Services.Interfaces
{
    /// <summary>
    /// Guarda o utilizador com sessão iniciada, para ser mostrado na interface
    /// e registado como autor das operações na auditoria.
    /// </summary>
    public interface ISessionService
    {
        #region Properties
        /// <summary>
        /// Nome do utilizador autenticado, ou <c>null</c> se não houver sessão.
        /// </summary>
        string? UtilizadorAtual { get; }

        /// <summary>
        /// Nome a registar na auditoria: o utilizador autenticado ou "Sistema".
        /// </summary>
        string NomeParaAuditoria { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Inicia a sessão do utilizador indicado.
        /// </summary>
        void Iniciar(string username);

        /// <summary>
        /// Termina a sessão atual.
        /// </summary>
        void Terminar();
        #endregion
    }
}
