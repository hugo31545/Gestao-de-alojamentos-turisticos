//-----------------------------------------------------------------
//    <copyright file="AlojamentoService.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>2</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Enums;

namespace POOfase1trabalho13_11.Models.Services.Interfaces
{
    /// <summary>
    /// Define o contrato para a lógica de negócio de excelência relacionada a alojamentos.
    /// </summary>
    public interface IAlojamentoService
    {
        #region Gestão de Alojamentos (CRUD)

        /// <summary>
        /// Obtém a lista completa de todos os alojamentos.
        /// </summary>
        IEnumerable<Alojamento> ObterTodosAlojamentos();

        /// <summary>
        /// Procura um alojamento pelo seu ID.
        /// </summary>
        Alojamento? ObterAlojamentoPorId(int id);

        /// <summary>
        /// Guarda um alojamento no sistema. 
        /// Identifica automaticamente se é uma nova inserção ou atualização de dados existentes.
        /// </summary>
        /// <param name="alojamento">O objeto <see cref="Alojamento"/> a persistir.</param>
        /// <returns><c>true</c> se foi guardado com sucesso após validações; caso contrário, <c>false</c>.</returns>
        bool GuardarAlojamento(Alojamento alojamento);

        /// <summary>
        /// Remove um alojamento, validando primeiro se existem reservas ativas ou pendentes.
        /// </summary>
        /// <param name="id">O identificador do alojamento.</param>
        /// <returns><c>true</c> se a remoção foi permitida e concluída; caso contrário, <c>false</c>.</returns>
        bool RemoverAlojamento(int id);

        #endregion

        #region Lógica de Negócio e Estados

        /// <summary>
        /// Altera o estado (Disponível, Manutenção, Ocupado) de um alojamento específico.
        /// </summary>
        bool MudarEstado(int idAlojamento, EstadoAlojamento novoEstado);

        /// <summary>
        /// Calcula o valor total com base no preço por noite do alojamento e na duração.
        /// </summary>
        decimal CalcularPrecoTotal(int alojamentoId, int numeroNoites);

        #endregion

        #region Pesquisa e Disponibilidade Avançada

        /// <summary>
        /// Lista alojamentos disponíveis no inventário geral.
        /// </summary>
        IEnumerable<Alojamento> BuscarAlojamentosDisponiveis();

        /// <summary>
        /// Filtra alojamentos disponíveis que não possuem reservas que colidam com o período indicado.
        /// </summary>
        IEnumerable<Alojamento> BuscarDisponiveisPorPeriodo(DateTime checkIn, DateTime checkOut);

        /// <summary>
        /// Filtra alojamentos com base no seu tipo (ex: Apartamento, Quarto).
        /// </summary>
        IEnumerable<Alojamento> FiltrarPorTipo(TipoAlojamento tipo);

        /// <summary>
        /// Procura o alojamento mais adequado respeitando a capacidade, o tipo preferido e as datas de estadia.
        /// </summary>
        /// <param name="capacidade">Capacidade mínima de pessoas.</param>
        /// <param name="tipo">Tipo de alojamento preferido.</param>
        /// <param name="checkIn">Data de entrada.</param>
        /// <param name="checkOut">Data de saída.</param>
        /// <returns>O <see cref="Alojamento"/> ideal ou <c>null</c> se nada for encontrado para as datas.</returns>
        Alojamento? EncontrarMelhorOpcao(int capacidade, TipoAlojamento tipo, DateTime checkIn, DateTime checkOut);

        #endregion
    }
}