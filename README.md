# ReadyKids Childminder Agency Portal (.NET 8)

A full-stack registration and admin portal for a childminder agency, built with .NET 8 Minimal API and PostgreSQL.

## Tech Stack

- **Backend:** .NET 8, Minimal API, Npgsql
- **Database:** PostgreSQL with JSONB columns for nested form data
- **Frontend:** Vanilla HTML/CSS/JS (no build step)

## Prerequisites

- **.NET SDK** 8.0+
- **PostgreSQL** 14+

## Setup

```bash
# 1. Create the PostgreSQL database
createdb readykids

# 2. Restore packages
cd DaryelCare
dotnet restore
```

Edit `DaryelCare/appsettings.json` with your PostgreSQL credentials:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=readykids;Username=postgres;Password=yourpassword"
  }
}
```

Or set the `DATABASE_URL` environment variable:

```
DATABASE_URL=postgres://postgres:yourpassword@localhost:5432/readykids
```

## Seeding Demo Data

```bash
cd DaryelCare
dotnet run -- --seed
```

Seeds 12 sample applications across all pipeline stages.

## Running

```bash
cd DaryelCare
dotnet run
```

The server starts at **http://localhost:3000**.

## Pages

| URL | Description |
|-----|-------------|
| http://localhost:3000/register | 9-section childminder registration form |
| http://localhost:3000/admin | Admin dashboard with pipeline view and compliance tracking |

## API

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/applications` | List all applications |
| GET | `/api/applications/{id}` | Get single application |
| POST | `/api/applications` | Submit new registration |
| PATCH | `/api/applications/{id}` | Update application fields |
| DELETE | `/api/applications/{id}` | Remove application |
| POST | `/api/applications/{id}/timeline` | Add audit log entry |

## Resetting the Database

```bash
dropdb readykids
createdb readykids
cd DaryelCare
dotnet run -- --seed
```
