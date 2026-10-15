# RaceDay RESTful API — PROG6212 Part 2

## Project Overview

RaceDay is an event management RESTful API developed using **ASP.NET Core 8**, **Entity Framework Core Code First**, and **SQL Server**.

The system allows Organisers to manage sporting events, categories, participant enrolments and race results. Participants can register, browse events, enrol in suitable categories and view their race information.

The project includes role-based access control, Swagger API documentation, automated testing and a GitHub Actions continuous integration workflow.

## Technologies Used

- C# and ASP.NET Core Web API (.NET 8)
- Entity Framework Core (Code First)
- Microsoft SQL Server / LocalDB
- ASP.NET Core Cookie Authentication
- Swagger / OpenAPI
- xUnit testing framework
- Git and GitHub
- GitHub Actions
- Azure Blob Storage integration for image uploads

## User Roles

### Organiser
- Register and log in
- Manage their profile
- Create, update and delete owned events
- Create and manage event categories
- View participant enrolments
- Confirm or update enrolment statuses
- Record and correct race results
- Upload event banner images

### Participant
- Register and log in
- Manage their profile
- Browse available events and categories
- Enrol in eligible events
- View personal enrolments and results
- Upload a profile picture

### Public
- View upcoming events
- View event details and categories
- View published race leaderboards

## Prerequisites

Before running the project, install:

- Visual Studio 2022 with ASP.NET and web development workload
- .NET 8 SDK
- SQL Server Express or LocalDB
- SQL Server Management Studio (optional)
- Azure Storage account (optional, for image uploads)

## Installation and Setup

1. Clone or download this repository.
2. Open `RaceDay.sln` in Visual Studio 2022.
3. Restore the NuGet packages.
4. Configure the SQL Server connection string in `RaceDay.Api/appsettings.json` or through local configuration.
5. Open **Tools → NuGet Package Manager → Package Manager Console**.
6. Set the default project to `RaceDay.Api`.
7. Apply the existing database migrations:

   `Update-Database -Project RaceDay.Api -StartupProject RaceDay.Api`

   Alternatively, from the solution root:

   `dotnet ef database update --project RaceDay.Api --startup-project RaceDay.Api`

8. If no migrations exist yet, generate an initial Code-First migration before updating the database.
9. Set `RaceDay.Api` as the startup project.
10. Press **F5** to run the application.
11. Open Swagger at `/swagger`, for example `https://localhost:7055/swagger`.

**Important:** Do not commit real passwords, Azure credentials or other secrets to GitHub.

## Authentication and Authorisation

RaceDay uses ASP.NET Core cookie-based authentication.

Passwords are securely hashed using ASP.NET Core `PasswordHasher<User>`.

The API enforces role-based access:

- `401 Unauthorized` when authentication is required.
- `403 Forbidden` when an authenticated user lacks permission.
- `400 Bad Request` when a request violates business rules.

To switch between Organiser and Participant accounts in Swagger, log out before logging in with the other account.

## Main API Functionality

The API supports:

- User registration and login
- User profile management
- Event creation and management
- Event category management
- Participant enrolments
- Enrolment status updates
- Race result recording
- Race leaderboards
- Image upload integration

### Business Rules

- Only Organisers can create and manage events.
- Participants cannot enrol in an event that has already started.
- Participants cannot enrol more than once in the same event.
- Race results require confirmed enrolments.
- Results cannot be recorded before the event has taken place.
- Event ownership and category eligibility are validated.
- Existing race records are protected by appropriate deletion restrictions.

## API Testing with Swagger

Swagger is used to test the API endpoints.

The following manual API scenarios have been exercised:

| Test Scenario | Result |
|---|---|
| Register an Organiser | Successful |
| Log in as Organiser | Successful |
| Create an event | Successful |
| Retrieve event information | Successful |
| Create an event category | Successful |
| Register and log in as Participant | Successful |
| Enrol in an upcoming event | Successful |
| Confirm an enrolment | Successful |
| Prevent Participant event creation | `403 Forbidden` |
| Prevent enrolment after event start | `400 Bad Request` |
| Prevent recording results before event | `400 Bad Request` |

Historical race-result creation remains to be verified.

## Automated Testing

The project includes an xUnit test project, `RaceDay.Tests`, intended to test API behaviour and business rules.

Run tests using:

`dotnet test RaceDay.sln`

Alternatively, in Visual Studio:

**Test → Run All Tests**

The test project uses EF Core InMemory for database-independent testing.

The actual number of executed and passing tests should be verified in the test output.

## GitHub Actions — Continuous Integration

The GitHub Actions workflow is located at:

`.github/workflows/raceday-api.yml`

The workflow is configured to perform automated project checks, including:

1. Checking out repository files
2. Installing the .NET SDK
3. Restoring dependencies
4. Building the solution
5. Running the test command
6. Uploading test results

**Verified workflow execution:**

- Workflow: RaceDay API CI
- Branch: `master`
- Commit: `15cc3f5`
- Status: **Success**
- Duration: 37 seconds

The workflow completed successfully on GitHub. The exact number of tests executed should be confirmed from the test logs.

## Project Structure

| Folder | Purpose |
|---|---|
| `RaceDay.Api/Models` | Entity Framework Core entities |
| `RaceDay.Api/Data` | Database context and relationships |
| `RaceDay.Api/DTOs` | API request and response models |
| `RaceDay.Api/Controllers` | API endpoints and business operations |
| `RaceDay.Api/Services` | Supporting services and validation |
| `RaceDay.Tests` | Automated tests |
| `.github/workflows` | GitHub Actions CI configuration |

## Azure Blob Storage

The application includes support for image uploads using Azure Blob Storage.

Configure the storage connection string through user secrets or environment variables:

`AzureStorage__ConnectionString`

Do not commit Azure credentials to source control.

Private Azure Blob containers require an appropriate access mechanism, such as signed URLs, before uploaded images can be accessed publicly.

## Demonstration Video

**YouTube demonstration link:** To be added.

The demonstration will cover:

- Project structure
- Swagger API testing
- Organiser and Participant authentication
- Event and category management
- Enrolment and results functionality
- SQL Server database
- Automated tests
- GitHub Actions workflow

## GitHub Commit History

The repository records project development and updates using Git.

The assignment requires at least **20 genuine, meaningful commits** documenting implementation, testing, fixes and documentation improvements.

## Security and Implementation Notes

- Passwords are hashed rather than stored in plaintext.
- Authentication uses ASP.NET Core cookies rather than JWT.
- Authorisation protects role-specific endpoints.
- Race results require a confirmed enrolment and a completed event.
- Event categories enforce eligibility rules.
- Database changes are managed through EF Core migrations.
- Production deployment requires appropriate HTTPS, cookie security, CSRF and cross-origin configuration.

## Author

Developed for **PROG6212 — Part 2**.

**Project:** RaceDay RESTful API
