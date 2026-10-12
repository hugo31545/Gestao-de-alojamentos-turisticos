using NUnit.Framework;
using POOfase1trabalho13_11.Models.Entities;
using POOfase1trabalho13_11.Models.Enums;
using POOfase1trabalho13_11.Models.Repositories.Implementations;
using POOfase1trabalho13_11.Models.Services.Implementations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace POOfase1trabalho13_11.Tests.Integration
{
    /// <summary>
    /// Testes com os repositórios reais, a gravar em ficheiros JSON numa pasta temporária.
    /// Apanham erros que os testes com mocks não veem (ex.: Remove proibido nos logs).
    /// </summary>
    [TestFixture]
    [NonParallelizable] // ConfiguracaoDados.Diretorio é estático
    public class JsonRepositoryTests
    {
        private string _pasta = string.Empty;

        [SetUp]
        public void Setup()
        {
            _pasta = Path.Combine(Path.GetTempPath(), "GestaoAlojamentosTests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_pasta);
            ConfiguracaoDados.Diretorio = _pasta;
        }

        [TearDown]
        public void TearDown()
        {
            try { Directory.Delete(_pasta, recursive: true); } catch (IOException) { }
        }

        [Test]
        public void Add_GeraIdsSequenciaisEPersisteNoFicheiro()
        {
            var repo = new ClienteRepository();
            var a = new Cliente { Nome = "Ana", Nif = "123456789", Email = "ana@ipca.pt" };
            var b = new Cliente { Nome = "Rui", Nif = "987654321", Email = "rui@ipca.pt" };

            repo.Add(a);
            repo.Add(b);

            Assert.That(a.Id, Is.EqualTo(1));
            Assert.That(b.Id, Is.EqualTo(2));

            // Uma nova instância lê o mesmo ficheiro
            var recarregado = new ClienteRepository();
            Assert.That(recarregado.GetAll().Select(c => c.Nome), Is.EquivalentTo(new[] { "Ana", "Rui" }));
        }

        [Test]
        public void Add_IdRepetido_LancaInvalidOperationException()
        {
            var repo = new ClienteRepository();
            repo.Add(new Cliente { Id = 5, Nome = "Ana", Nif = "123456789", Email = "ana@ipca.pt" });

            Assert.Throws<InvalidOperationException>(() =>
                repo.Add(new Cliente { Id = 5, Nome = "Rui", Nif = "987654321", Email = "rui@ipca.pt" }));
            Assert.That(repo.GetAll().Count(), Is.EqualTo(1));
        }

        [Test]
        public void FicheiroCorrompido_GuardaCopiaEEmiteAviso()
        {
            // Arrange: ficheiro com JSON inválido
            string ficheiro = Path.Combine(_pasta, "clientes.json");
            File.WriteAllText(ficheiro, "[ { isto não é json ");
            var avisos = new List<string>();
            Action<string> handler = avisos.Add;
            AvisosDeDados.AvisoEmitido += handler;

            try
            {
                // Act
                var repo = new ClienteRepository();

                // Assert: repositório vazio, mas o original fica preservado numa cópia
                Assert.That(repo.GetAll(), Is.Empty);
                var copias = Directory.GetFiles(_pasta, "clientes.corrupt-*.json");
                Assert.That(copias, Has.Length.EqualTo(1));
                Assert.That(File.ReadAllText(copias[0]), Does.Contain("isto não é json"));
                Assert.That(avisos, Has.Count.EqualTo(1));
                Assert.That(avisos[0], Does.Contain("clientes.json"));
            }
            finally
            {
                AvisosDeDados.AvisoEmitido -= handler;
            }
        }

        [Test]
        public void RegistoService_LimparLogsAntigos_FuncionaComRepositorioReal()
        {
            // Antes da correção, Remove lançava NotSupportedException e a limpeza falhava sempre
            var repo = new RegistoRepository();
            repo.Add(new Registo(0, "A", "antigo", "U", "C", 1, new DateTime(2020, 1, 1)));
            repo.Add(new Registo(0, "A", "recente", "U", "C", 2, DateTime.Now));
            var service = new RegistoService(repo);

            bool ok = service.LimparLogsAntigos(new DateTime(2024, 1, 1), out string mensagem);

            Assert.IsTrue(ok, mensagem);
            Assert.That(new RegistoRepository().GetAll().Select(r => r.Detalhes), Is.EquivalentTo(new[] { "recente" }));
        }

        [Test]
        public void Autenticacao_RegistoELogin_FicheiroNaoContemPassword()
        {
            var auth = new AuthenticationService(new UserRepository());

            Assert.IsTrue(auth.CreateUser("ana@ipca.pt", "Segredo123", "Segredo123", out string mensagem), mensagem);

            // Login com uma nova instância (lê do ficheiro)
            var auth2 = new AuthenticationService(new UserRepository());
            Assert.IsTrue(auth2.UserExists("ana@ipca.pt", "Segredo123"));
            Assert.IsFalse(auth2.UserExists("ana@ipca.pt", "errada123"));
            Assert.That(File.ReadAllText(Path.Combine(_pasta, "users.json")), Does.Not.Contain("Segredo123"));
        }

        [Test]
        public void FluxoCompleto_ReservaCheckInCheckOut_FaturacaoEOcupacao()
        {
            // Arrange
            var alojamentos = new AlojamentoRepository();
            var clientes = new ClienteRepository();
            var reservas = new ReservaRepository();
            var registos = new RegistoRepository();
            var reservaService = new ReservaService(reservas, alojamentos, clientes, registos);
            var checkInService = new CheckInService(reservas, alojamentos, registos);
            var consultaService = new ConsultaService(alojamentos, reservas, clientes, new ConsultaRepository());

            var alojamento = new Alojamento { Nome = "T1 Centro", Capacidade = 2, PrecoPorNoite = 60 };
            alojamentos.Add(alojamento);
            var cliente = new Cliente { Nome = "Ana", Nif = "123456789", Email = "ana@ipca.pt" };
            clientes.Add(cliente);

            // Act: reserva para hoje (2 noites), confirmação e check-in
            Assert.IsTrue(reservaService.RealizarReserva(cliente.Id, alojamento.Id, DateTime.Today, DateTime.Today.AddDays(2), 2, out string msg), msg);
            var reserva = reservas.GetAll().Single();
            Assert.IsTrue(reservaService.ConfirmarReserva(reserva.Id, out msg), msg);
            Assert.IsTrue(checkInService.RealizarCheckIn(reserva.Id, "ana", out msg), msg);

            // Assert: alojamento ocupado
            Assert.That(alojamentos.GetById(alojamento.Id)!.Estado, Is.EqualTo(EstadoAlojamento.Ocupado));
            Assert.That(consultaService.CalcularTaxaOcupacao(), Is.EqualTo(100));

            // Act: check-out
            Assert.IsTrue(checkInService.RealizarCheckOut(reserva.Id, "ana", out msg), msg);

            // Assert: reserva finalizada conta para a faturação
            Assert.That(reservas.GetById(reserva.Id)!.Estado, Is.EqualTo(EstadoReserva.Finalizada));
            Assert.That(consultaService.CalcularFaturacaoTotal(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(5)), Is.EqualTo(120m));
            Assert.That(registos.GetAll().Select(r => r.Operacao), Is.SupersetOf(new[] { "CREATE", "CONFIRM", "CHECKIN", "CHECKOUT" }));
        }
    }
}
