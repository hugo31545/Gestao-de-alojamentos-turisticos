//-----------------------------------------------------------------

//    <copyright file="Exceptions.cs" company="IPCA">

//     Copyright IPCA-EST. All rights reserved.

//    </copyright>

//    <version>2</version>

//    <author>Hugo Gomes</author>

//-----------------------------------------------------------------

namespace POOfase1trabalho13_11.Exceptions
{
    /// <summary>
    /// Exceção personalizada para erros de regras de negócio.
    /// </summary>
    public class BusinessException : Exception
    {
        // Construtor padrão
        public BusinessException() : base("Ocorreu um erro de negócio.") { }

        // Construtor que aceita uma mensagem especifica
        public BusinessException(string message) : base(message) { }

        // Construtor para erros 
        public BusinessException(string message, Exception inner) : base(message, inner) { }
    }
}