//-----------------------------------------------------------------
//    <copyright file="User.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------

using POOfase1trabalho13_11.Models.Entities.Interfaces;

/// <summary>
/// Representa as credenciais de acesso de um utilizador ao sistema.
/// Esta classe armazena os dados necessários para o processo de autenticação.
/// Implementa a interface <see cref="IEntity"/>.
/// </summary>
public class User : IEntity
{

    /// <summary>
    /// Obtém ou define o identificador único do utilizador.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Obtém ou define o nome de utilizador.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define a palavra-passe do utilizador.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define a data e hora em que a conta de utilizador foi criada.
    /// </summary>
    public DateTime DataRegisto { get; set; }
}