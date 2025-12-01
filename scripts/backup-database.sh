#!/bin/bash
# Database backup script for PostgreSQL
# Usage: ./backup-database.sh [database_name] [backup_dir]

set -e

DB_NAME="${1:-TossErp}"
BACKUP_DIR="${2:-./backups}"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
BACKUP_FILE="${BACKUP_DIR}/toss_${DB_NAME}_${TIMESTAMP}.sql"

# Ensure backup directory exists
mkdir -p "$BACKUP_DIR"

# Database connection parameters (can be overridden via environment variables)
PGHOST="${PGHOST:-localhost}"
PGPORT="${PGPORT:-5432}"
PGUSER="${PGUSER:-postgres}"
PGPASSWORD="${PGPASSWORD:-postgres}"

echo "Starting backup of database: $DB_NAME"
echo "Backup file: $BACKUP_FILE"

# Perform backup
export PGPASSWORD
pg_dump -h "$PGHOST" -p "$PGPORT" -U "$PGUSER" -d "$DB_NAME" \
    --format=plain \
    --no-owner \
    --no-privileges \
    --clean \
    --if-exists \
    > "$BACKUP_FILE"

# Compress backup
gzip "$BACKUP_FILE"
BACKUP_FILE="${BACKUP_FILE}.gz"

echo "Backup completed: $BACKUP_FILE"
echo "Backup size: $(du -h "$BACKUP_FILE" | cut -f1)"

# Keep only last 30 days of backups (optional cleanup)
find "$BACKUP_DIR" -name "toss_${DB_NAME}_*.sql.gz" -mtime +30 -delete 2>/dev/null || true

echo "Backup cleanup completed (kept last 30 days)"

