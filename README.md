# Tourist Accommodation Management

[![CI](https://github.com/hugo31545/Gestao-de-alojamentos-turisticos/actions/workflows/main.yml/badge.svg)](https://github.com/hugo31545/Gestao-de-alojamentos-turisticos/actions/workflows/main.yml)
![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4)
![WPF](https://img.shields.io/badge/UI-WPF-0078D4)
![Tests](https://img.shields.io/badge/tests-188%20passing-brightgreen)

A **Windows desktop application (C# / .NET 8 / WPF)** for running a small hotel or holiday-rental business: guests, properties, bookings, check-in/check-out, reports and a full audit trail, in one place.

It started as the coursework project for *Object-Oriented Programming* (BSc in Informatics Engineering, IPCA, Portugal). After the first working version, I reviewed my own code, found security, data-integrity and business-logic problems, and fixed them with tests. This README documents both the product and the engineering work behind it.

> The user interface is in Portuguese. The code, comments and this document describe it in English where it matters; the full academic report (UML, requirements, design decisions) is in Portuguese: [trabalhoPOO_31545_Fase2Relatório.pdf](Gest%C3%A3o%20de%20alojamentos%20tur%C3%ADsticos/POOfase1trabalho13-11/trabalhoPOO_31545_Fase2Relat%C3%B3rio.pdf).

## Highlights

- **Clean MVVM architecture** with dependency injection, the Repository pattern and a service layer. No business logic in the views.
- **Security:** passwords stored as salted **PBKDF2-SHA256** hashes (100,000 iterations) with constant-time comparison; legacy plain-text accounts are migrated transparently on first login; password fields are masked.
- **Booking workflow as a state machine:** `Pending → Confirmed → Checked-in → Completed` (or `Cancelled`), with transitions validated by the booking entity's own methods and buttons enabled only when the action is allowed.
- **Data safety:** atomic JSON writes (temporary file + replace), automatic copy of any unreadable data file, and in-memory rollback when a write fails.
- **Audit trail:** every critical operation is recorded with the user who performed it.
- **188 automated tests** (unit, integration with real files, ViewModel and XAML-loading tests), executed on every push by **GitHub Actions**.
- **Forms that guide the user:** placeholders, field-by-field validation as you type (e.g. *"NIF must have 9 digits (has 5)"*) and an explanation when the submit button is disabled.
- **Zero compiler warnings**, nullable reference types enabled.

## Features

| Area | What it does |
|---|---|
| **Authentication** | Staff registration with a linked login account; password policy in a separate `POO.Security` library; session tracking so the signed-in user appears in the UI and in the audit log. |
| **Properties** | Create, edit, delete and change status (Available, Occupied, Maintenance, Unavailable). Deletion is blocked while active bookings exist. |
| **Guests** | Registration and editing with NIF (tax ID) and email validation and duplicate detection. A guest with active bookings cannot be deleted. |
| **Bookings** | Availability search with date-overlap detection, creation, confirmation and cancellation. No past dates, at least one guest, capacity enforced. The total price is frozen when the booking is made. |
| **Check-in / Check-out** | Check-in marks the property as Occupied; check-out completes the booking and sends the property to Maintenance (cleaning). Checks the date window and that the property is actually free. |
| **Audit log** | Searchable history of who did what and when. Individual entries cannot be edited or deleted; only a retention policy can purge old ones. |
| **Reports** | Current occupancy rate, revenue for a date range, most expensive properties, top guests, and a history of the reports each user ran. |

### Booking lifecycle

```
Pending ──Confirm──▶ Confirmed ──Check-in──▶ CheckedIn ──Check-out──▶ Completed
   │                     │
   └─────Cancel──────────┴──▶ Cancelled
```

## Architecture

```
View (XAML)  ⇄  ViewModel  ──▶  Service (business rules)  ──▶  Repository (JSON)
  data binding    commands        validation, auditing           persistence
```

| Layer | Responsibility |
|---|---|
| **Model** | Domain entities, enums, services with the business rules, repositories, validation. |
| **ViewModel** | Observable state and commands. Dialogs go through `IDialogService` and the signed-in user through `ISessionService`, so ViewModels are testable without a UI. |
| **View** | WPF/XAML bound to ViewModels; no logic in code-behind. |

**Domain model:** `Alojamento` (property), `Cliente` (guest), `Employee`, `User`, `Reserva` (booking), `CheckIn`, `Registo` (audit entry) and `Consulta` (report history entry).

**Patterns and techniques:** Repository (generic `JsonRepository<T>` plus specific repositories), Command, Factory (`ViewFactory`), Dependency Injection (`Microsoft.Extensions.DependencyInjection`), interface-based design, custom `BusinessException`, LINQ, `ObservableCollection`, and a reusable `FormField` user control (label, placeholder, password mode and inline validation message) built with dependency properties.

**OOP concepts demonstrated:** inheritance (generic repository base class with eight subclasses; `BaseViewModel`), polymorphism (interface-based services, overridable repository methods), abstraction (an interface for every service and repository) and encapsulation (validated properties and domain methods that guard state changes).

### Solution layout

```
POOfase1trabalho13-11/
├── POOfase1trabalho13-11/             WPF application
│   ├── Models/        Entities · Enums · Repositories · Services · Validation
│   ├── ViewModels/    ViewModels · Commands · IDialogService
│   ├── Views/         XAML · ViewFactory · helpers · dialog service
│   └── Exceptions/    BusinessException
├── POO.Security/                      Class library: password policy and hashing
└── POOfase1trabalho13_11.Tests/       NUnit + Moq tests
```

## Engineering work: review and hardening

After the first version worked, I reviewed the whole codebase and fixed what I found. Each item below was a real defect or risk, fixed in its own commit on a `develop` branch.

| Problem found | What I did |
|---|---|
| Passwords stored in plain text | PBKDF2 hashing with per-password salt; transparent migration of existing accounts. |
| Check-in/check-out in the UI bypassed the business rules, so occupancy and revenue always showed 0 | One workflow through `CheckInService`; added the missing *Confirm* step; entity-level state-transition checks. |
| Registering a staff member could leave a login account with no employee record | Validate everything first, and roll back the account if the second step fails. |
| A corrupted JSON file was silently replaced by an empty one on the next save | Copy the damaged file aside, warn the user, and continue. |
| The log-cleanup feature always failed against the real repository (it passed only because the test mocked it) | Added an explicit retention operation and an integration test with the real repository. |
| Login and registration screens gave no feedback: wrong credentials showed nothing, and the form never said what was wrong | Reusable `FormField` control with placeholders and live per-field validation; login error message, Enter to submit, initial focus. |
| Business rules too loose: bookings in the past, zero guests, deleting properties/guests with active bookings, bad NIF or email | Validation in the service layer, shared validator, tests for each rule. |
| `MessageBox` calls inside ViewModels and a WPF dependency inside the data layer | `IDialogService`; the repository reports problems through an event instead of opening windows. |
| Audit log always said "System" | `ISessionService` records the real user. |
| ID generation duplicated in 7 repositories | Centralised in the base repository, with duplicate-ID protection. |
| Data saved under `bin/`, lost on clean or redeploy | Stored in the user's local app-data folder, with automatic migration. |
| CI compiled the code but never ran the tests (the test project was not in the solution) | Solution fixed; CI now builds in Release and runs all tests. |

## Getting started

**Requirements:** Windows and the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (WPF is Windows-only).

```bash
git clone https://github.com/hugo31545/Gestao-de-alojamentos-turisticos.git
cd "Gestao-de-alojamentos-turisticos/Gestão de alojamentos turísticos/POOfase1trabalho13-11"
dotnet run --project POOfase1trabalho13-11
```

On first launch, click **Criar Conta** (create account) on the login screen. The email is the username.

Data is stored as JSON in `%LocalAppData%\GestaoAlojamentos\Data`, resolved per user at runtime and never hard-coded. Data from older versions found next to the executable is copied there automatically on first run.

## Testing and CI

```bash
dotnet test "Gestão de alojamentos turísticos/POOfase1trabalho13-11/POOfase1trabalho13-11.sln"
```

- **Unit tests:** services and entities, with repositories mocked using Moq.
- **Integration tests:** real repositories writing to a temporary folder, including the full flow *booking → confirm → check-in → check-out → revenue*, and recovery from a corrupted file.
- **ViewModel and UI tests:** commands enabled per booking state, live form validation, correct use of the session and dialog services, and tests that load the login, registration and reports XAML on an STA thread.

[GitHub Actions](.github/workflows/main.yml) restores, builds (Release) and tests the solution on every push and pull request to `main` and `develop`. Work is done on `develop` and merged to `main` through a pull request once CI is green.

## What I would do next

- Replace the JSON files with SQLite or SQL Server (the repository interfaces already isolate the storage).
- Add role-based permissions, so only administrators can create accounts.
- Add payments and invoicing, and a calendar view of availability.
- Add UI automation tests and screenshots.

## Author

**Hugo Gomes** — BSc in Informatics Engineering, IPCA (Instituto Politécnico do Cávado e do Ave), Portugal.
