using NUnit.Framework;
using POO.Utils.Security;

namespace POOfase1trabalho13_11.Tests.Security
{
    [TestFixture]
    public class PasswordHasherTests
    {
        [Test]
        public void Hash_MesmaPassword_GeraHashesDiferentes()
        {
            string h1 = PasswordHasher.Hash("Segredo123");
            string h2 = PasswordHasher.Hash("Segredo123");

            Assert.That(h1, Is.Not.EqualTo(h2), "Cada hash deve usar um salt novo.");
            Assert.That(h1, Does.StartWith("PBKDF2$"));
            Assert.That(h1, Does.Not.Contain("Segredo123"));
        }

        [Test]
        public void Verificar_PasswordCorreta_RetornaTrue()
        {
            string hash = PasswordHasher.Hash("Segredo123");

            Assert.IsTrue(PasswordHasher.Verificar("Segredo123", hash));
        }

        [Test]
        public void Verificar_PasswordErrada_RetornaFalse()
        {
            string hash = PasswordHasher.Hash("Segredo123");

            Assert.IsFalse(PasswordHasher.Verificar("segredo123", hash));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("texto-simples")]
        [TestCase("PBKDF2$abc$xx$yy")]
        [TestCase("PBKDF2$1000$não-é-base64$yy")]
        public void Verificar_HashInvalido_RetornaFalse(string? hash)
        {
            Assert.IsFalse(PasswordHasher.Verificar("Segredo123", hash));
        }

        [TestCase("", "obrigatória")]
        [TestCase("abc1", "6 caracteres")]
        [TestCase("abcdefg", "número")]
        public void PasswordValidator_PasswordInvalida_IndicaMotivo(string password, string motivo)
        {
            bool valida = PasswordValidator.Validar(password, out string mensagem);

            Assert.IsFalse(valida);
            Assert.That(mensagem, Does.Contain(motivo));
        }
    }
}
