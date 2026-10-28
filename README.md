# RaceDay RESTful API — PROG6212 Part 2

ASP.NET Core 8 REST API using EF Core Code First and SQL Server. Implements the seven entities and all 23 routes from the Part 1 endpoint plan, including Azure Blob image uploads.

## Roles

- **Organiser**: registers/logs in, edits own profile, creates/updates/deletes owned events, manages categories and enrolment statuses, captures/corrects results, uploads event banners.
- **Participant**: registers/logs in, edits own profile, uploads profile picture, browses events/categories, enrols once per event, views own enrolments/results.
- **Public**: upcoming events, event details/categories, published leaderboards.

## Prerequisites

Visual Studio 2022 with ASP.NET workload, .NET 8 SDK, SQL Server Express or LocalDB, optionally SSMS and an Azure Storage account for uploads.

## Setup and run

1. Open `RaceDay.sln` in Visual Studio. Restore NuGet packages.
2. Update `RaceDay.Api/appsettings.json` connection string if your SQL Server is not `(localdb)\MSSQLLocalDB`.
3. Open **Tools → NuGet Package Manager → Package Manager Console**. Set Default project to `RaceDay.Api`.
4. Install EF tooling if needed: `dotnet tool install --global dotnet-ef --version 8.0.11` (or use Visual Studio PMC).
5. **Generate the first Code-First migration**: `Add-Migration InitialCreate -Project RaceDay.Api -StartupProject RaceDay.Api` in Package Manager Console, then `Update-Database -Project RaceDay.Api -StartupProject RaceDay.Api`. Alternatively from solution root: `dotnet ef migrations add InitialCreate --project RaceDay.Api --startup-project RaceDay.Api` then `dotnet ef database update --project RaceDay.Api --startup-project RaceDay.Api`.
6. This creates `RaceDayDB` and all seven tables. **Do not run Part 1's DROP DATABASE script against an existing database.** Its placeholder password hashes are not valid logins. Register new users via API instead.
7. Set `RaceDay.Api` as Startup Project. Press F5; open `/swagger` (typically `https://localhost:7055/swagger`).
8. POST `/api/auth/register` twice with different emails and roles, then POST `/api/auth/login`. Swagger in the same browser retains the cookie for subsequent requests. Log out before switching roles.
9. To use image uploads, set `AzureStorage__ConnectionString` via user secrets or environment variables. **Do not commit credentials.** The Azure container is private; returned blob URLs are not publicly readable unless you later implement signed access.

## Test

Run `dotnet test RaceDay.sln` from the solution root or **Test → Run All Tests** in Visual Studio. The test suite uses EF Core InMemory; SQL Server is not required for CI tests. The tests cover registration, duplicate registration, invalid login, session logout, unauthenticated access, organiser event management, role restrictions, ownership, participant enrolment and duplicate prevention.

## GitHub Actions

`.github/workflows/raceday-api.yml` restores, builds and runs tests on pushes and pull requests affecting Part 2. No Azure or SQL Server secrets are required for CI. After your first successful run, capture the **green build screenshot** and save it as `docs/green-build.png`.

![GitHub Actions successful build — add your real screenshot after CI passes](docs/green-build.png)

## YouTube demonstration

**Unlisted demonstration URL:** ADD YOUR VIDEO LINK HERE.

Show Swagger register/login, both roles, forbidden responses, CRUD, enrolments, results, SQL Server tables, Visual Studio tests, GitHub Actions green run and explain the source structure.

## Code layout

- `RaceDay.Api/Models` — seven EF Core entities
- `RaceDay.Api/Data` — DbContext, relationships, SQL constraints
- `RaceDay.Api/DTOs` — request models
- `RaceDay.Api/Controllers` — authentication, profile, events, categories, enrolments, results
- `RaceDay.Api/Services` — image uploads and validation
- `RaceDay.Tests` — xUnit integration-style API tests
- `.github/workflows` — automated build/test

## Important implementation notes

- Authentication uses encrypted ASP.NET Core cookies with an eight-hour sliding expiry, not JWT tokens. API calls return 401/403 rather than HTML redirects.
- Passwords are hashed using ASP.NET Core `PasswordHasher<User>`; plaintext is never saved.
- Deleting an event/category with enrolments returns 409 to preserve recorded race data.
- Race results require a confirmed enrolment and a past event date.
- Event age categories are checked against date of birth on the event date.
- Public leaderboard entries become visible after the event date, when results have been entered.
- The API is intended for same-origin Swagger and a future same-site MVC frontend. For cross-site production use, configure CORS, secure cookies, CSRF protection and Azure Blob access appropriately.
- Part 1's SQL sample rows use placeholder hashes and are **not** automatically seeded. Generate the migration from the entity model.
- Tests and production builds must be run in your own .NET environment before claiming a green build.

## Commit history

Make at least **20 genuine incremental commits** under your own account as you build, test, document and refine this project. Do not manufacture a misleading commit history. Suggested checkpoints: solution, packages, each entity, DbContext, migration, each controller, auth, Swagger, tests, workflow, README, demo.
