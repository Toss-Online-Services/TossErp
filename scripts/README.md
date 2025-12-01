# Database Backup and Restore Scripts

This directory contains scripts for backing up and restoring the TOSS ERP database.

## Available Scripts

### Linux/macOS (Bash)

- **`backup-database.sh`**: Creates a compressed backup of the PostgreSQL database
- **`restore-database.sh`**: Restores the database from a backup file

### Windows (PowerShell)

- **`backup-database.ps1`**: Creates a compressed backup of the PostgreSQL database
- **`restore-database.ps1`**: Restores the database from a backup file

## Usage

### Backup

**Linux/macOS:**
```bash
./scripts/backup-database.sh [database_name] [backup_dir]
```

**Windows:**
```powershell
.\scripts\backup-database.ps1 -DatabaseName TossErp -BackupDir .\backups
```

### Restore

**Linux/macOS:**
```bash
./scripts/restore-database.sh <backup_file> [database_name]
```

**Windows:**
```powershell
.\scripts\restore-database.ps1 -BackupFile .\backups\toss_TossErp_20240101_120000.sql.gz -DatabaseName TossErp
```

## Environment Variables

You can override database connection parameters using environment variables:

- `PGHOST` (default: `localhost`)
- `PGPORT` (default: `5432`)
- `PGUSER` (default: `postgres`)
- `PGPASSWORD` (default: `postgres`)
- `POSTGRES_DB` (default: `TossErp`)

## Docker Compose

When using Docker Compose, backups are automatically stored in `./scripts/backups` which is mounted as a volume in the PostgreSQL container.

## Notes

- Backups are automatically compressed (`.gz` on Linux/macOS, `.zip` on Windows)
- Old backups (older than 30 days) are automatically cleaned up
- Restore scripts will prompt for confirmation before dropping the existing database
- Ensure PostgreSQL client tools (`pg_dump`, `psql`) are installed and in your PATH

