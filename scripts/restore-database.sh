#!/bin/bash
# Database restore script for PostgreSQL
# Usage: ./restore-database.sh [backup_file] [database_name]

set -e

if [ -z "$1" ]; then
    echo "Error: Backup file path is required"
    echo "Usage: ./restore-database.sh <backup_file> [database_name]"
    exit 1
fi

BACKUP_FILE="$1"
DB_NAME="${2:-TossErp}"

# Check if backup file exists
if [ ! -f "$BACKUP_FILE" ]; then
    echo "Error: Backup file not found: $BACKUP_FILE"
    exit 1
fi

# Database connection parameters (can be overridden via environment variables)
PGHOST="${PGHOST:-localhost}"
PGPORT="${PGPORT:-5432}"
PGUSER="${PGUSER:-postgres}"
PGPASSWORD="${PGPASSWORD:-postgres}"

echo "Starting restore of database: $DB_NAME"
echo "Backup file: $BACKUP_FILE"

# Check if backup is compressed
TEMP_FILE="$BACKUP_FILE"
if [[ "$BACKUP_FILE" == *.gz ]]; then
    echo "Decompressing backup file..."
    TEMP_FILE="${BACKUP_FILE%.gz}"
    gunzip -c "$BACKUP_FILE" > "$TEMP_FILE"
    trap "rm -f $TEMP_FILE" EXIT
fi

# Drop existing database if it exists (be careful in production!)
read -p "This will drop the existing database '$DB_NAME'. Continue? (yes/no): " confirm
if [ "$confirm" != "yes" ]; then
    echo "Restore cancelled"
    exit 1
fi

# Drop and recreate database
export PGPASSWORD
psql -h "$PGHOST" -p "$PGPORT" -U "$PGUSER" -d postgres -c "DROP DATABASE IF EXISTS \"$DB_NAME\";" || true
psql -h "$PGHOST" -p "$PGPORT" -U "$PGUSER" -d postgres -c "CREATE DATABASE \"$DB_NAME\";"

# Restore from backup
echo "Restoring database..."
psql -h "$PGHOST" -p "$PGPORT" -U "$PGUSER" -d "$DB_NAME" < "$TEMP_FILE"

echo "Restore completed successfully"

