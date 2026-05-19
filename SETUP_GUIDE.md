# Complete Setup Guide — ProjectFlow

This guide walks you through every step from cloning the repository to running the full application successfully using **Docker only**. No local installation of .NET, SQL Server, Redis, or MinIO is required.

---

## Table of contents

1. [Prerequisites](#1-prerequisites)
2. [Clone the repository](#2-clone-the-repository)
3. [Understand the project structure](#3-understand-the-project-structure)
4. [Configure environment (optional)](#4-configure-environment-optional)
5. [Build and run with Docker Compose](#5-build-and-run-with-docker-compose)
6. [Verify all services are running](#6-verify-all-services-are-running)
7. [Access the application](#7-access-the-application)
8. [Login with seed accounts](#8-login-with-seed-accounts)
9. [Explore the API (Swagger)](#9-explore-the-api-swagger)
10. [Using MinIO for file storage](#10-using-minio-for-file-storage)
11. [Common operations](#11-common-operations)
12. [Running unit tests](#12-running-unit-tests)
13. [Stopping and cleaning up](#13-stopping-and-cleaning-up)
14. [Troubleshooting](#14-troubleshooting)
15. [Local development without Docker (advanced)](#15-local-development-without-docker-advanced)

---

## 1. Prerequisites

### Required

| Tool | Minimum version | Purpose |
|------|-----------------|---------|
| **Docker Desktop** | 4.x | Runs all services in containers |
| **Git** | 2.x | Clone the repository |

### Verify Docker installation

Open PowerShell (Windows) or Terminal (macOS/Linux):

```bash
docker --version
docker compose version
```

Expected output examples:

```
Docker version 27.x.x
Docker Compose version v2.x.x
```

### System requirements

- **RAM:** 8 GB minimum (16 GB recommended — SQL Server container uses ~2 GB)
- **Disk:** 5 GB free space for images and volumes
- **Ports:** Ensure these are not in use: `1433`, `3000`, `5000`, `6379`, `9000`, `9001`

---

## 2. Clone the repository

```bash
git clone <your-repository-url>
cd project-management-system
```

If you received a ZIP file, extract it and open a terminal in the project root folder (where `docker-compose.yml` is located).

---

## 3. Understand the project structure

```
project-management-system/
├── docker-compose.yml          # Orchestrates all services
├── .env.example                # Environment variable template
├── ProjectManagement.sln       # .NET solution
├── src/
│   ├── ProjectManagement.API/
│   ├── ProjectManagement.Application/
│   ├── ProjectManagement.Domain/
│   ├── ProjectManagement.Infrastructure/
│   └── ProjectManagement.Persistence/
├── tests/
│   └── ProjectManagement.UnitTests/
├── frontend/                   # React application
├── README.md
└── SETUP_GUIDE.md              # This file
```

---

## 4. Configure environment (optional)

Default values work out of the box. To customize:

```bash
# Windows PowerShell
Copy-Item .env.example .env

# macOS / Linux
cp .env.example .env
```

Edit `.env` if you need to change passwords or ports. The `docker-compose.yml` file already embeds the required environment variables for the backend service.

---

## 5. Build and run with Docker Compose

From the **project root** directory:

```bash
docker compose up --build
```

### What happens during startup

1. **sqlserver** — SQL Server 2022 starts and initializes the database engine (~30–60 seconds first time)
2. **redis** — Redis cache starts
3. **minio** — MinIO object storage starts; buckets are created by the API on startup
4. **backend** — .NET API builds, runs EF migrations, seeds demo data
5. **frontend** — React app builds and is served via Nginx

### First run timing

| Phase | Approximate time |
|-------|------------------|
| Pull Docker images | 5–15 minutes (first time only) |
| Build backend + frontend | 3–8 minutes |
| SQL Server ready | 30–90 seconds |
| **Total first run** | **10–25 minutes** |

Subsequent runs are much faster (cached images).

### Run in background (detached mode)

```bash
docker compose up --build -d
```

View logs:

```bash
docker compose logs -f
docker compose logs -f backend
```

---

## 6. Verify all services are running

```bash
docker compose ps
```

All services should show `running` (or `healthy` where applicable):

| Container | Port | Status |
|-----------|------|--------|
| pm-sqlserver | 1433 | running |
| pm-redis | 6379 | running |
| pm-minio | 9000, 9001 | running |
| pm-backend | 5000 | running |
| pm-frontend | 3000 | running |

### Health checks

```bash
# API health (SQL + Redis)
curl http://localhost:5000/health

# Frontend
curl -I http://localhost:3000
```

---

## 7. Access the application

| Application | URL |
|-------------|-----|
| **Web App** | http://localhost:3000 |
| **API Swagger** | http://localhost:5000/swagger |
| **MinIO Console** | http://localhost:9001 |

---

## 8. Login with seed accounts

The database is automatically seeded on first startup.

### Regular user

- **Email:** `user@projectmanagement.com`
- **Password:** `User@12345`

### Administrator

- **Email:** `admin@projectmanagement.com`
- **Password:** `Admin@12345`

### What to try after login

1. Open **Dashboard** — view statistics and task distribution chart
2. Open **Projects** — browse the seeded "Enterprise Platform" project
3. Open a project — manage tasks, change status, upload attachments
4. Open **Profile** — change your password
5. (Admin only) Open **Admin** — view system-wide statistics

---

## 9. Explore the API (Swagger)

1. Go to http://localhost:5000/swagger
2. Expand `POST /api/v1/auth/login`
3. Execute with:

```json
{
  "email": "user@projectmanagement.com",
  "password": "User@12345"
}
```

4. Copy the `accessToken` from the response
5. Click **Authorize** (top right)
6. Enter: `Bearer <paste-token-here>`
7. Test protected endpoints (Projects, Tasks, Dashboard)

---

## 10. Using MinIO for file storage

### MinIO Console

- URL: http://localhost:9001
- Username: `minioadmin`
- Password: `minioadmin`

### Buckets (auto-created)

| Bucket | Purpose |
|--------|---------|
| `project-covers` | Project cover images |
| `task-attachments` | Task file attachments |

### Upload via UI

1. Open a project detail page
2. Click **Upload Cover** and select an image (JPEG, PNG, WebP, GIF — max 5 MB)
3. On a task row, use the upload icon for attachments (max 10 MB)

---

## 11. Common operations

### Restart a single service

```bash
docker compose restart backend
```

### Rebuild after code changes

```bash
docker compose up --build backend
docker compose up --build frontend
```

### Reset database (fresh seed)

```bash
docker compose down -v
docker compose up --build
```

> **Warning:** `-v` removes volumes including all database data.

---

## 12. Running unit tests

Without local .NET SDK:

```bash
docker run --rm -v "%cd%:/src" -w /src mcr.microsoft.com/dotnet/sdk:9.0 dotnet test tests/ProjectManagement.UnitTests
```

macOS/Linux:

```bash
docker run --rm -v "$(pwd):/src" -w /src mcr.microsoft.com/dotnet/sdk:9.0 dotnet test tests/ProjectManagement.UnitTests
```

---

## 13. Stopping and cleaning up

```bash
# Stop containers (keep data)
docker compose down

# Stop and remove volumes (full reset)
docker compose down -v

# Remove built images
docker compose down --rmi local
```

---

## 14. Troubleshooting

### SQL Server not ready / backend crashes on startup

**Symptom:** Backend logs show connection errors to SQL Server.

**Solution:** Wait 60–90 seconds and restart the backend:

```bash
docker compose restart backend
```

The seeder retries database connection up to 30 times.

### Port already in use

**Symptom:** `Bind for 0.0.0.0:5000 failed: port is already allocated`

**Solution:** Stop the conflicting process or change ports in `docker-compose.yml`:

```yaml
ports:
  - "5001:8080"  # backend
  - "3001:80"    # frontend
```

### Frontend cannot reach API

**Symptom:** Login fails with network error.

**Solution:** Ensure `VITE_API_URL` in `docker-compose.yml` points to `http://localhost:5000/api/v1` (browser calls API on host machine, not inside Docker network).

### MinIO upload fails

**Symptom:** File upload returns error.

**Solution:**

```bash
docker compose logs minio
docker compose restart minio backend
```

### Docker Desktop not running (Windows)

**Symptom:** `error during connect: open //./pipe/dockerDesktopLinuxEngine`

**Solution:** Start Docker Desktop and wait until it shows "Running".

### Insufficient memory

**Symptom:** SQL Server container exits or is killed.

**Solution:** Increase Docker Desktop memory to 6–8 GB (Settings → Resources → Memory).

---

## 15. Local development without Docker (advanced)

Only if you have .NET 9 SDK, Node.js 20+, SQL Server, Redis, and MinIO installed locally:

```bash
# Backend
cd src/ProjectManagement.API
dotnet run

# Frontend
cd frontend
npm install
npm run dev
```

Update `appsettings.json` and `frontend/.env` with local connection strings.

---

## Success checklist

- [ ] `docker compose ps` shows all 5 services running
- [ ] http://localhost:5000/health returns `Healthy`
- [ ] http://localhost:3000 loads the login page
- [ ] Login with demo user works
- [ ] Dashboard shows statistics
- [ ] Swagger authorizes with JWT token
- [ ] File upload works on project/task

If all items are checked, your setup is complete.

---

## Support

For architecture details, see [README.md](README.md).

For API reference, use Swagger at http://localhost:5000/swagger.
