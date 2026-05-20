# Complete Project Run Steps (From Start to Finish)

This file documents everything that was actually done to run the project on Windows using Docker.

## 1) Prerequisites

- Docker Desktop is installed and running.
- Git is installed.
- Open PowerShell in the project root folder (the one containing `docker-compose.yml`).

## 2) Verify Docker

Run:

```powershell
docker --version
docker compose version
```

If the Docker daemon is not running, open Docker Desktop and wait until it shows `Running`.

## 3) Run the Project

From the project root:

```powershell
docker compose up --build -d
```

This starts:
- `sqlserver`
- `redis`
- `minio`
- `backend`
- `frontend`

## 4) Issues Encountered During Startup and How They Were Fixed

During startup, several issues appeared. These were the exact fixes applied:

1. **Pending model changes in EF Core**
   - `ApplicationDbContextModelSnapshot` was updated to a real snapshot compatible with EF Core 9.

2. **Migration was not properly registered**
   - `20250519000000_InitialCreate.Designer.cs` was missing.
   - The file was added so `dotnet ef` can detect the migration.

3. **SQL error caused by `nvarchar(5000)`**
   - The `Description` column in `InitialCreate` was changed to:
   - `nvarchar(max)` with `maxLength: 5000`.

4. **Database was in an inconsistent state**
   - A full volumes reset was done, then a clean startup:

```powershell
docker compose down -v
docker compose up --build -d
```

## 5) Verify the Project is Running

### Container status

```powershell
docker compose ps
```

You should see all services as `Up`, and some as `healthy`.

### API health check

```powershell
Invoke-WebRequest -Uri "http://localhost:5000/health" -UseBasicParsing
```

Expected response: `Healthy`

## 6) Service URLs

- Frontend: `http://localhost:3000`
- Swagger: `http://localhost:5000/swagger`
- API Health: `http://localhost:5000/health`
- MinIO Console: `http://localhost:9001`

## 7) Useful Commands After Startup

- View logs:

```powershell
docker compose logs -f
docker compose logs -f backend
```

- Restart backend only:

```powershell
docker compose restart backend
```

- Stop the project:

```powershell
docker compose down
```

---

## Accounts (Seed Accounts)

### Regular user

- **Email:** `user@projectmanagement.com`
- **Password:** `User@12345`

### Administrator

- **Email:** `admin@projectmanagement.com`
- **Password:** `Admin@12345`

