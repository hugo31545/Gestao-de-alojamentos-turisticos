//-----------------------------------------------------------------
//    <copyright file="SessionService.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>1</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.Models.Services.Interfaces;

namespace POOfase1trabalho13_11.Models.Services.Implementations
{
    /// <summary>
    /// Implementação em memória da sessão (registada como Singleton).
    /// </summary>
    public class SessionService : ISessionService
    {
        #region Constants
        private const string NomeSistema = "Sistema";
        #endregion

        #region Properties
        public string? UtilizadorAtual { get; private set; }

        public string NomeParaAuditoria => UtilizadorAtual ?? NomeSistema;
        #endregion

        #region Methods
        public void Iniciar(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("O nome de utilizador é obrigatório.", nameof(username));
            UtilizadorAtual = username;
        }

        public void Terminar() => UtilizadorAtual = null;
        #endregion
    }
}
