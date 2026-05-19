# ProjectFlow — Enterprise Project & Task Management System

A production-grade full-stack application for managing projects, tasks, file attachments, and team workflows. Built with **.NET 9 Clean Architecture**, **React + Vite**, **SQL Server**, **Redis**, **MinIO**, and **Docker Compose**.

![Architecture](docs/screenshots/architecture-placeholder.png)

## Features

- JWT authentication with refresh tokens and role-based authorization (Admin / User)
- Projects CRUD with pagination, search, sorting, and cover image upload
- Tasks CRUD with status, priority, filtering, and file attachments
- Redis caching for project and task lists
- MinIO object storage with presigned URLs
- Serilog logging, global exception handling, API versioning, rate limiting, health checks
- Modern React dashboard with dark mode, charts, animations, and responsive layout

## Architecture

```
┌─────────────┐     ┌──────────────┐     ┌─────────────────────────────┐
│   React UI  │────▶│  ASP.NET API │────▶│  Application (CQRS/MediatR) │
└─────────────┘     └──────────────┘     └──────────────┬──────────────┘
                                                        │
                        ┌───────────────────────────────┼───────────────────────────────┐
                        ▼                               ▼                               ▼
                 ┌─────────────┐               ┌──────────────┐               ┌─────────────┐
                 │ Persistence │               │Infrastructure│               │   Domain    │
                 │  (EF Core)  │               │ JWT/Redis/   │               │  Entities   │
                 └──────┬──────┘               │    MinIO     │               └─────────────┘
                        │                      └──────────────┘
                        ▼
                 ┌─────────────┐     ┌────────┐     ┌────────┐
                 │ SQL Server  │     │ Redis  │     │ MinIO  │
                 └─────────────┘     └────────┘     └────────┘
```

### Solution structure

```
src/
 ├── ProjectManagement.API/           # Controllers, middleware, DI bootstrap
 ├── ProjectManagement.Application/   # CQRS, validators, DTOs, interfaces
 ├── ProjectManagement.Domain/        # Entities, enums, repository contracts
 ├── ProjectManagement.Infrastructure/# JWT, Redis, MinIO, password hashing
 └── ProjectManagement.Persistence/   # EF Core, migrations, repositories

tests/
 └── ProjectManagement.UnitTests/

frontend/                            # React + Vite + Tailwind
```

### Design patterns

| Pattern | Usage |
|---------|--------|
| Clean Architecture | Layered dependency flow inward to Domain |
| CQRS + MediatR | Commands and queries with dedicated handlers |
| Repository + Unit of Work | Generic data access abstraction |
| Result Pattern | Consistent operation outcomes |
| Options Pattern | Strongly typed configuration |
| FluentValidation | Request validation pipeline |

## Quick start (Docker only)

**Prerequisites:** Docker Desktop 4.x+ with Docker Compose v2.

```bash
git clone <repository-url>
cd project-management-system
docker compose up --build
```

| Service | URL |
|---------|-----|
| Frontend | http://localhost:3000 |
| API / Swagger | http://localhost:5000/swagger |
| Health check | http://localhost:5000/health |
| MinIO Console | http://localhost:9001 (minioadmin / minioadmin) |

### Seed accounts

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@projectmanagement.com | Admin@12345 |
| User | user@projectmanagement.com | User@12345 |

## Environment variables

See [`.env.example`](.env.example). Key settings:

| Variable | Description |
|----------|-------------|
| `ConnectionStrings__DefaultConnection` | SQL Server connection |
| `ConnectionStrings__Redis` | Redis connection |
| `Jwt__Secret` | JWT signing key (32+ chars) |
| `Minio__Endpoint` | MinIO host:port |
| `VITE_API_URL` | Frontend API base URL |

## API documentation

Swagger UI: **http://localhost:5000/swagger**

Authenticate via `POST /api/v1/auth/login`, then click **Authorize** and enter:

```
Bearer <your-access-token>
```

### Main endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/v1/auth/register` | Register |
| POST | `/api/v1/auth/login` | Login |
| POST | `/api/v1/auth/refresh` | Refresh token |
| POST | `/api/v1/auth/logout` | Logout |
| GET | `/api/v1/projects` | List projects (paginated) |
| POST | `/api/v1/projects` | Create project |
| GET | `/api/v1/tasks/project/{id}` | Tasks by project |
| PATCH | `/api/v1/tasks/{id}/status` | Update task status |
| GET | `/api/v1/dashboard/stats` | Dashboard statistics |
| GET | `/api/v1/dashboard/admin` | Admin stats (Admin only) |

## Running tests

```bash
docker run --rm -v ${PWD}:/src -w /src mcr.microsoft.com/dotnet/sdk:9.0 \
  dotnet test tests/ProjectManagement.UnitTests
```

## Screenshots

| Dashboard | Projects |
|-----------|----------|
| ![Dashboard](docs/screenshots/dashboard-placeholder.png) | ![Projects](docs/screenshots/projects-placeholder.png) |

## Tech stack

**Backend:** .NET 9, ASP.NET Core, EF Core, MediatR, FluentValidation, AutoMapper, Serilog, JWT, Redis, MinIO, xUnit

**Frontend:** React 18, Vite, TypeScript, TailwindCSS, Zustand, React Query, Framer Motion, Recharts

**Infrastructure:** Docker, Docker Compose, SQL Server 2022, Redis 7, MinIO

## Documentation

- **[SETUP_GUIDE.md](SETUP_GUIDE.md)** — Complete setup from clone to running application

## License

MIT
