# Gestão de Alojamentos Turísticos

Aplicação de secretária em **C# / WPF** para gerir alojamentos turísticos: clientes, alojamentos, reservas, check-in/check-out e auditoria das operações. O projeto segue o paradigma da Programação Orientada a Objetos e a arquitetura **MVVM**, e guarda os dados em ficheiros **JSON**.

Trabalho prático da unidade curricular **Programação Orientada a Objetos** — Licenciatura em Engenharia de Sistemas Informáticos, Escola Superior de Tecnologia, Instituto Politécnico do Cávado e do Ave (IPCA), ano letivo 2025/2026.

- **Autor:** Hugo Manuel Alves Gomes (nº 31545)
- **Docente:** Ernesto Casanova
- **Relatório:** [trabalhoPOO_31545_Fase2Relatório.pdf](Gest%C3%A3o%20de%20alojamentos%20tur%C3%ADsticos/POOfase1trabalho13-11/trabalhoPOO_31545_Fase2Relat%C3%B3rio.pdf)

---

## Funcionalidades

| Área | O que permite |
|---|---|
| **Autenticação** | Registo de funcionários com conta de acesso e login. As passwords são validadas pela DLL `POO.Security` (mínimo 6 caracteres e 1 número) e guardadas apenas como hash PBKDF2. |
| **Alojamentos** | Criar, editar, remover e mudar o estado (Disponível, Ocupado, Manutenção, Indisponível). Não é possível remover alojamentos com reservas ativas. |
| **Clientes** | Registo e edição com validação de NIF (9 dígitos) e email, sem duplicados. |
| **Reservas** | Pesquisa de alojamentos livres no período pedido (sem sobreposição de datas), criação, confirmação e cancelamento. O preço total fica guardado na reserva. |
| **Check-in / Check-out** | O check-in marca o alojamento como Ocupado; o check-out finaliza a reserva e envia o alojamento para Manutenção (limpeza). |
| **Auditoria** | Cada operação crítica gera um registo com o utilizador que a fez, consultável e filtrável na aplicação. |
| **Relatórios** | Taxa de ocupação, faturação por período, alojamentos mais caros e clientes com mais reservas. Cada consulta fica registada no histórico com o utilizador que a pediu. |

### Ciclo de vida de uma reserva

```
Pendente ──Confirmar──▶ Confirmada ──Check-in──▶ CheckInRealizado ──Check-out──▶ Finalizada
    │                        │
    └──────Cancelar──────────┴──▶ Cancelada
```

Cada botão só fica ativo quando a reserva está no estado que o permite.

---

## Arquitetura

```
View (XAML)  ⇄  ViewModel  ──▶  Service (regras de negócio)  ──▶  Repository (JSON)
   binding       comandos         validação, auditoria            persistência
```

- **Model** — entidades (`Alojamento`, `Cliente`, `Reserva`, `CheckIn`, `Employee`, `User`, `Registo`, `Consulta`), enums, serviços com as regras de negócio e repositórios.
- **ViewModel** — propriedades observáveis e comandos (`ViewModelCommand`, `RelayCommand<T>`). As caixas de diálogo passam por `IDialogService`, para as ViewModels poderem ser testadas sem interface gráfica.
- **View** — janelas e páginas WPF ligadas às ViewModels por data binding.

Padrões utilizados: **Repository** (`JsonRepository<T>` genérico + repositórios específicos), **Command**, **Factory** (`ViewFactory`) e **injeção de dependências** (`Microsoft.Extensions.DependencyInjection`, configurada em `App.xaml.cs`). As violações das regras de negócio são sinalizadas com a exceção personalizada `BusinessException`.

### Estrutura da solução

```
Gestão de alojamentos turísticos/POOfase1trabalho13-11/
├── POOfase1trabalho13-11.sln          Solução (aplicação, DLL e testes)
├── POOfase1trabalho13-11/             Aplicação WPF
│   ├── Models/
│   │   ├── Entities/                  Entidades do domínio
│   │   ├── Enums/                     Estados e tipos
│   │   ├── Repositories/              Interfaces + implementações JSON
│   │   ├── Services/                  Regras de negócio, autenticação, sessão
│   │   └── Validation/                Validação de NIF e email
│   ├── ViewModels/                    ViewModels, comandos e IDialogService
│   ├── Views/                         XAML, ViewFactory, helpers
│   └── Exceptions/                    BusinessException
├── POO.Security/                      DLL: PasswordValidator e PasswordHasher
└── POOfase1trabalho13_11.Tests/       Testes NUnit + Moq
```

---

## Como executar

**Requisitos:** Windows e [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (ou Visual Studio 2022 com a carga de trabalho *Desenvolvimento para desktop .NET*).

```bash
git clone https://github.com/hugo31545/Gestao-de-alojamentos-turisticos.git
cd "Gestao-de-alojamentos-turisticos/Gestão de alojamentos turísticos/POOfase1trabalho13-11"
dotnet run --project POOfase1trabalho13-11
```

No Visual Studio, abrir `POOfase1trabalho13-11.sln` e definir `POOfase1trabalho13-11` como projeto de arranque.

Na primeira utilização, criar uma conta em **Criar Conta** no ecrã de login (o email é o nome de utilizador).

### Onde ficam os dados

Os ficheiros JSON (`alojamentos.json`, `clientes.json`, `reservas.json`, `users.json`, …) são guardados em:

```
%LocalAppData%\GestaoAlojamentos\Data
```

Se existirem dados de versões anteriores na pasta `bin\...\Data`, são copiados automaticamente na primeira execução. Se um ficheiro estiver danificado, a aplicação guarda uma cópia (`*.corrupt-AAAAMMDD-HHMMSS.json`) antes de continuar e avisa o utilizador.

---

## Testes

```bash
dotnet test "Gestão de alojamentos turísticos/POOfase1trabalho13-11/POOfase1trabalho13-11.sln"
```

- **Testes unitários** dos serviços e entidades, com repositórios simulados (Moq).
- **Testes de integração** com os repositórios reais numa pasta temporária, incluindo o fluxo completo reserva → check-in → check-out → faturação.
- **Testes das ViewModels**, com um serviço de diálogos falso.

### Integração contínua

O workflow [`.github/workflows/main.yml`](.github/workflows/main.yml) compila a solução e corre os testes em cada *push* e *pull request* para `main` e `develop`.

**Fluxo de trabalho:** desenvolver na branch `develop` e, quando estiver estável, abrir um pull request para `main`. O merge só deve ser feito com o CI a verde.

---

## Tecnologias

C# 12 · .NET 8 · WPF/XAML · System.Text.Json · LINQ · Microsoft.Extensions.DependencyInjection · NUnit · Moq · GitHub Actions
