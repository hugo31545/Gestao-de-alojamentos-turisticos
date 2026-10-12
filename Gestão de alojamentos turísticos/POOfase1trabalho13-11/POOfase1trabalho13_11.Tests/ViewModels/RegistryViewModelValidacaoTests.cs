using Moq;
using NUnit.Framework;
using POOfase1trabalho13_11.Models.Services.Interfaces;
using POOfase1trabalho13_11.ViewModels;
using POOfase1trabalho13_11.Views.Interfaces;

namespace POOfase1trabalho13_11.Tests.ViewModels
{
    /// <summary>
    /// Validação em tempo real do formulário de registo (mensagens por campo e estado do botão).
    /// </summary>
    [TestFixture]
    public class RegistryViewModelValidacaoTests
    {
        private RegistryViewModel _vm;

        [SetUp]
        public void Setup()
        {
            _vm = new RegistryViewModel(new Mock<IEmployeeService>().Object, new Mock<IRegistoService>().Object,
                new Mock<IViewFactory>().Object, new Mock<IAuthenticationService>().Object, new DialogoFalso());
        }

        private void PreencherValido()
        {
            _vm.Nome = "Ana Silva";
            _vm.Email = "ana@ipca.pt";
            _vm.NIF = "123456789";
            _vm.Password = "Segredo123";
            _vm.PasswordConfirmacao = "Segredo123";
            _vm.Cargo = "Rececionista";
        }

        [Test]
        public void FormularioVazio_SemErrosVisiveis_MasBotaoInativoComAjuda()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_vm.NomeErro, Is.Empty);
                Assert.That(_vm.EmailErro, Is.Empty);
                Assert.That(_vm.NifErro, Is.Empty);
                Assert.That(_vm.PasswordErro, Is.Empty);
                Assert.That(_vm.ConfirmacaoErro, Is.Empty);
                Assert.That(_vm.AjudaFormulario, Is.Not.Empty);
                Assert.IsFalse(_vm.RegistarCommand.CanExecute(null));
            });
        }

        [Test]
        public void DadosValidos_SemErros_BotaoAtivo()
        {
            PreencherValido();

            Assert.Multiple(() =>
            {
                Assert.That(_vm.EmailErro + _vm.NifErro + _vm.PasswordErro + _vm.ConfirmacaoErro + _vm.CargoErro, Is.Empty);
                Assert.That(_vm.AjudaFormulario, Is.Empty);
                Assert.IsTrue(_vm.RegistarCommand.CanExecute(null));
            });
        }

        [TestCase("12345", "tem 5")]
        [TestCase("12345678A", "só pode conter dígitos")]
        public void NifInvalido_MostraMotivo(string nif, string motivo)
        {
            PreencherValido();
            _vm.NIF = nif;

            Assert.That(_vm.NifErro, Does.Contain(motivo));
            Assert.IsFalse(_vm.RegistarCommand.CanExecute(null));
        }

        [Test]
        public void EmailSemDominio_MostraFormatoEsperado()
        {
            _vm.Email = "ana@ipca";

            Assert.That(_vm.EmailErro, Does.Contain("nome@dominio.pt"));
        }

        [Test]
        public void PasswordFracaEConfirmacaoDiferente_MostramMotivos()
        {
            _vm.Password = "abc";
            _vm.PasswordConfirmacao = "abd";

            Assert.That(_vm.PasswordErro, Does.Contain("6 caracteres"));
            Assert.That(_vm.ConfirmacaoErro, Does.Contain("não coincidem"));
        }

        [Test]
        public void ErrosSaoNotificadosAView()
        {
            var alteradas = new List<string?>();
            _vm.PropertyChanged += (_, e) => alteradas.Add(e.PropertyName);

            _vm.NIF = "1";

            Assert.That(alteradas, Does.Contain(nameof(RegistryViewModel.NifErro)));
            Assert.That(alteradas, Does.Contain(nameof(RegistryViewModel.AjudaFormulario)));
        }

        [Test]
        public void PodeVoltar_SoQuandoEJanelaPropria()
        {
            Assert.IsFalse(_vm.PodeVoltar, "Dentro da janela principal não há 'voltar ao login'.");

            _vm.CloseWindowAction = () => { };

            Assert.IsTrue(_vm.PodeVoltar);
        }
    }
}
