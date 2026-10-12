//-----------------------------------------------------------------
//    <copyright file="ConfiguracaoDados.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <version>1</version>
//    <author>Hugo Gomes</author>
//-----------------------------------------------------------------
using System.IO;

namespace POOfase1trabalho13_11.Models.Repositories.Implementations
{
    /// <summary>
    /// Define onde os repositórios JSON guardam os ficheiros.
    /// Por omissão usa <c>%LocalAppData%\GestaoAlojamentos\Data</c>, que não é apagada
    /// por <c>dotnet clean</c> nem ao publicar uma nova versão.
    /// Os testes podem apontar <see cref="Diretorio"/> para uma pasta temporária.
    /// </summary>
    public static class ConfiguracaoDados
    {
        #region Fields
        private static string? _diretorio;
        #endregion

        #region Properties
        /// <summary>
        /// Pasta onde os ficheiros JSON são lidos e gravados.
        /// Deve ser definida antes de se criar o primeiro repositório.
        /// </summary>
        public static string Diretorio
        {
            get => _diretorio ??= ObterDiretorioPorOmissao();
            set => _diretorio = Path.GetFullPath(value);
        }

        /// <summary>
        /// Pasta usada até à Fase 2 (junto ao executável, dentro de <c>bin/</c>).
        /// </summary>
        public static string DiretorioAntigo =>
            Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data"));
        #endregion

        #region Methods
        private static string ObterDiretorioPorOmissao()
        {
            string diretorio = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "GestaoAlojamentos", "Data");

            MigrarDadosAntigos(diretorio);
            return diretorio;
        }

        /// <summary>
        /// Na primeira execução copia os ficheiros da pasta antiga (bin/Data) para a nova,
        /// para não se perderem os dados já existentes. Os originais não são apagados.
        /// </summary>
        private static void MigrarDadosAntigos(string destino)
        {
            try
            {
                if (Directory.Exists(destino) || !Directory.Exists(DiretorioAntigo)) return;

                Directory.CreateDirectory(destino);
                foreach (string ficheiro in Directory.GetFiles(DiretorioAntigo, "*.json"))
                {
                    File.Copy(ficheiro, Path.Combine(destino, Path.GetFileName(ficheiro)), overwrite: false);
                }
            }
            catch (Exception ex)
            {
                AvisosDeDados.Emitir($"Não foi possível copiar os dados antigos de {DiretorioAntigo}: {ex.Message}");
            }
        }
        #endregion
    }

    /// <summary>
    /// Canal pelo qual os repositórios comunicam problemas de dados (ex.: ficheiro corrompido)
    /// sem dependerem da interface gráfica. A aplicação subscreve <see cref="AvisoEmitido"/>
    /// e mostra a mensagem; avisos emitidos antes da subscrição ficam em espera.
    /// </summary>
    public static class AvisosDeDados
    {
        #region Fields
        private static readonly List<string> _pendentes = new();
        private static Action<string>? _avisoEmitido;
        #endregion

        #region Events
        /// <summary>
        /// Disparado sempre que um repositório emite um aviso.
        /// Ao subscrever, recebe de imediato os avisos que estavam em espera.
        /// </summary>
        public static event Action<string>? AvisoEmitido
        {
            add
            {
                _avisoEmitido += value;
                foreach (string aviso in _pendentes) value?.Invoke(aviso);
                _pendentes.Clear();
            }
            remove => _avisoEmitido -= value;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Emite um aviso. Se ainda ninguém estiver a ouvir, fica guardado.
        /// </summary>
        public static void Emitir(string mensagem)
        {
            System.Diagnostics.Debug.WriteLine($"AVISO DE DADOS: {mensagem}");
            if (_avisoEmitido != null) _avisoEmitido(mensagem);
            else _pendentes.Add(mensagem);
        }
        #endregion
    }
}
