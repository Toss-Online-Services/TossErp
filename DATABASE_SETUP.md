# Database Setup Guide

## Quick Start with Docker (Recommended)

### 1. Start PostgreSQL using Docker Compose

From the project root directory:

```bash
docker-compose up -d
```

This will:
- Start PostgreSQL 16 in a Docker container
- Create database `TossErp`
- Expose PostgreSQL on port `5432`
- Use default credentials: `postgres` / `postgres`

### 2. Verify PostgreSQL is Running

```bash
docker ps
```

You should see a container named `toss-postgres` running.

### 3. Run Database Migrations

From `backend/Toss` directory:

```bash
dotnet ef database update --project src/Infrastructure --startup-project src/Web
```

### 4. Verify Database

You can connect to the database using any PostgreSQL client:
- **Host:** `localhost`
- **Port:** `5432`
- **Database:** `TossErp`
- **Username:** `postgres`
- **Password:** `postgres`

## Alternative: Local PostgreSQL Installation

If you have PostgreSQL installed locally:

### 1. Start PostgreSQL Service

**Windows:**
```powershell
# Check if PostgreSQL service is running
Get-Service -Name postgresql*

# Start PostgreSQL service (adjust service name if different)
Start-Service postgresql-x64-16  # or your version
```

**Linux/Mac:**
```bash
sudo systemctl start postgresql
# or
brew services start postgresql
```

### 2. Create Database

Connect to PostgreSQL and create the database:

```bash
psql -U postgres
```

Then in psql:
```sql
CREATE DATABASE "TossErp";
\q
```

### 3. Update Connection String

Update `backend/Toss/src/Web/appsettings.json` or `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=TossErp;Username=postgres;Password=your_password"
  }
}
```

### 4. Run Migrations

```bash
cd backend/Toss
dotnet ef database update --project src/Infrastructure --startup-project src/Web
```

## Troubleshooting

### Issue: "Failed to connect to 127.0.0.1:5432"

**Solution:** PostgreSQL is not running. Start it using one of the methods above.

### Issue: "database does not exist"

**Solution:** Create the database first:
```sql
CREATE DATABASE "TossErp";
```

### Issue: "password authentication failed"

**Solution:** 
1. Check your connection string in `appsettings.json`
2. For Docker: Default password is `postgres`
3. For local install: Use your PostgreSQL user password

### Issue: Docker container won't start

**Solution:**
```bash
# Check if port 5432 is already in use
netstat -ano | findstr :5432  # Windows
lsof -i :5432                 # Linux/Mac

# If port is in use, either:
# 1. Stop the other PostgreSQL instance
# 2. Or change the port in docker-compose.yml
```

### Issue: "Migration already applied"

**Solution:** This is normal if migrations were already run. Check applied migrations:
```bash
dotnet ef migrations list --project src/Infrastructure --startup-project src/Web
```

## Docker Commands Reference

```bash
# Start PostgreSQL
docker-compose up -d

# Stop PostgreSQL
docker-compose down

# Stop and remove data (WARNING: Deletes all data)
docker-compose down -v

# View logs
docker-compose logs postgres

# Connect to PostgreSQL in container
docker exec -it toss-postgres psql -U postgres -d TossErp
```

## Connection String Examples

### Docker (default)
```
Host=localhost;Port=5432;Database=TossErp;Username=postgres;Password=postgres
```

### Local PostgreSQL
```
Host=localhost;Port=5432;Database=TossErp;Username=your_username;Password=your_password
```

### Remote PostgreSQL
```
Host=your-server.com;Port=5432;Database=TossErp;Username=your_username;Password=your_password
```

## Next Steps

After database is set up and migrations are applied:

1. **Start Backend:**
   ```bash
   cd backend/Toss
   dotnet run --project src/Web
   ```

2. **Verify Seeding:**
   The database will be automatically seeded with:
   - Admin user: `admin@toss.local` / `Admin1!`
   - 15 Store Owner users: `storeowner1@toss.local` through `storeowner15@toss.local` / `StoreOwner1!`
   - All necessary roles

3. **Start Frontend:**
   ```bash
   cd toss-web
   npm run dev
   ```

4. **Test Login:**
   - Navigate to `http://localhost:3000/login`
   - Login with: `admin@toss.local` / `Admin1!`

