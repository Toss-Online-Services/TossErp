#!/bin/bash
set -euo pipefail

# TOSS ERP III Database Restore Script
# Restores database from backup files with safety checks

# Configuration
BACKUP_DIR="/backups"
LOG_FILE="/backups/restore.log"
POSTGRES_HOST="${POSTGRES_HOST:-postgres-primary}"
POSTGRES_DB="${POSTGRES_DB:-toss_erp}"
POSTGRES_USER="${POSTGRES_USER:-toss_user}"
POSTGRES_PASSWORD="${POSTGRES_PASSWORD:-development_password}"
S3_BUCKET="${S3_BUCKET:-}"

# Command line arguments
BACKUP_FILE="$1"
FORCE_RESTORE="${2:-false}"

# Logging function
log() {
    echo "[$(date '+%Y-%m-%d %H:%M:%S')] $1" | tee -a "$LOG_FILE"
}

# Error handling
handle_error() {
    log "ERROR: Restore failed at step: $1"
    exit 1
}

# Usage information
usage() {
    echo "Usage: $0 <backup_file> [force]"
    echo ""
    echo "Arguments:"
    echo "  backup_file    Path to backup file (.sql.gz) or S3 key"
    echo "  force          Skip confirmation prompts (optional)"
    echo ""
    echo "Examples:"
    echo "  $0 /backups/daily/toss_erp_daily_2024-01-15_02-00-00.sql.gz"
    echo "  $0 s3://bucket/toss-erp-backups/daily/toss_erp_daily_2024-01-15_02-00-00.sql.gz force"
    exit 1
}

# Validate arguments
validate_arguments() {
    if [[ -z "$BACKUP_FILE" ]]; then
        log "ERROR: Backup file not specified"
        usage
    fi
    
    log "Restore configuration:"
    log "  Backup file: $BACKUP_FILE"
    log "  Target database: $POSTGRES_DB@$POSTGRES_HOST"
    log "  Force mode: $FORCE_RESTORE"
}

# Download from S3 if needed
download_from_s3() {
    if [[ "$BACKUP_FILE" == s3://* ]]; then
        log "Downloading backup from S3..."
        
        LOCAL_FILE="/tmp/$(basename "$BACKUP_FILE")"
        
        if aws s3 cp "$BACKUP_FILE" "$LOCAL_FILE"; then
            BACKUP_FILE="$LOCAL_FILE"
            log "Downloaded to: $BACKUP_FILE"
        else
            handle_error "Failed to download backup from S3"
        fi
    fi
}

# Check if backup file exists and is valid
validate_backup_file() {
    if [[ ! -f "$BACKUP_FILE" ]]; then
        handle_error "Backup file not found: $BACKUP_FILE"
    fi
    
    if [[ "$BACKUP_FILE" != *.sql.gz ]]; then
        handle_error "Invalid backup file format. Expected .sql.gz"
    fi
    
    log "Validating backup file integrity..."
    if ! gzip -t "$BACKUP_FILE"; then
        handle_error "Backup file is corrupted"
    fi
    
    BACKUP_SIZE=$(du -h "$BACKUP_FILE" | cut -f1)
    log "Backup file validated. Size: $BACKUP_SIZE"
}

# Check database connectivity
check_database() {
    log "Checking database connectivity..."
    
    if ! PGPASSWORD="$POSTGRES_PASSWORD" psql -h "$POSTGRES_HOST" -U "$POSTGRES_USER" -d "postgres" -c '\q' 2>/dev/null; then
        handle_error "Database connection failed"
    fi
    
    log "Database connection successful"
}

# Get current database info
get_current_database_info() {
    log "Getting current database information..."
    
    # Check if database exists
    DB_EXISTS=$(PGPASSWORD="$POSTGRES_PASSWORD" psql -h "$POSTGRES_HOST" -U "$POSTGRES_USER" -d "postgres" \
        -t -c "SELECT 1 FROM pg_database WHERE datname='$POSTGRES_DB';" | tr -d ' ')
    
    if [[ "$DB_EXISTS" == "1" ]]; then
        CURRENT_SIZE=$(PGPASSWORD="$POSTGRES_PASSWORD" psql -h "$POSTGRES_HOST" -U "$POSTGRES_USER" -d "$POSTGRES_DB" \
            -t -c "SELECT pg_size_pretty(pg_database_size('$POSTGRES_DB'));" | tr -d ' ')
        
        TABLE_COUNT=$(PGPASSWORD="$POSTGRES_PASSWORD" psql -h "$POSTGRES_HOST" -U "$POSTGRES_USER" -d "$POSTGRES_DB" \
            -t -c "SELECT count(*) FROM information_schema.tables WHERE table_schema='public';" | tr -d ' ')
        
        log "Current database size: $CURRENT_SIZE"
        log "Current table count: $TABLE_COUNT"
    else
        log "Database does not exist yet"
        CURRENT_SIZE="N/A"
        TABLE_COUNT="0"
    fi
}

# Create pre-restore backup
create_pre_restore_backup() {
    if [[ "$DB_EXISTS" == "1" ]]; then
        log "Creating pre-restore backup..."
        
        PRE_RESTORE_FILE="/backups/pre-restore/toss_erp_pre_restore_$(date '+%Y-%m-%d_%H-%M-%S').sql.gz"
        mkdir -p "$(dirname "$PRE_RESTORE_FILE")"
        
        PGPASSWORD="$POSTGRES_PASSWORD" pg_dump \
            -h "$POSTGRES_HOST" \
            -U "$POSTGRES_USER" \
            -d "$POSTGRES_DB" \
            --verbose \
            --create \
            --clean \
            --if-exists \
            | gzip > "$PRE_RESTORE_FILE"
        
        if [[ ${PIPESTATUS[0]} -eq 0 ]]; then
            log "Pre-restore backup created: $PRE_RESTORE_FILE"
        else
            log "WARNING: Pre-restore backup failed, continuing..."
        fi
    fi
}

# Confirm restore operation
confirm_restore() {
    if [[ "$FORCE_RESTORE" != "true" ]]; then
        log "WARNING: This operation will completely replace the current database!"
        log "Current database: $POSTGRES_DB ($CURRENT_SIZE, $TABLE_COUNT tables)"
        log "Backup file: $(basename "$BACKUP_FILE") ($BACKUP_SIZE)"
        
        echo -n "Are you sure you want to continue? (yes/no): "
        read -r CONFIRMATION
        
        if [[ "$CONFIRMATION" != "yes" ]]; then
            log "Restore cancelled by user"
            exit 0
        fi
    fi
}

# Terminate active connections
terminate_connections() {
    if [[ "$DB_EXISTS" == "1" ]]; then
        log "Terminating active database connections..."
        
        PGPASSWORD="$POSTGRES_PASSWORD" psql -h "$POSTGRES_HOST" -U "$POSTGRES_USER" -d "postgres" \
            -c "SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname='$POSTGRES_DB' AND pid <> pg_backend_pid();" \
            || true
        
        sleep 2
        log "Active connections terminated"
    fi
}

# Perform the restore
perform_restore() {
    log "Starting database restore..."
    
    # Drop and recreate database
    if [[ "$DB_EXISTS" == "1" ]]; then
        log "Dropping existing database..."
        PGPASSWORD="$POSTGRES_PASSWORD" psql -h "$POSTGRES_HOST" -U "$POSTGRES_USER" -d "postgres" \
            -c "DROP DATABASE IF EXISTS \"$POSTGRES_DB\";" || handle_error "Failed to drop database"
    fi
    
    log "Restoring database from backup..."
    gunzip -c "$BACKUP_FILE" | PGPASSWORD="$POSTGRES_PASSWORD" psql -h "$POSTGRES_HOST" -U "$POSTGRES_USER" -d "postgres" \
        -v ON_ERROR_STOP=1 \
        || handle_error "Database restore failed"
    
    log "Database restore completed successfully"
}

# Verify restore
verify_restore() {
    log "Verifying restored database..."
    
    # Check database exists
    DB_EXISTS_AFTER=$(PGPASSWORD="$POSTGRES_PASSWORD" psql -h "$POSTGRES_HOST" -U "$POSTGRES_USER" -d "postgres" \
        -t -c "SELECT 1 FROM pg_database WHERE datname='$POSTGRES_DB';" | tr -d ' ')
    
    if [[ "$DB_EXISTS_AFTER" != "1" ]]; then
        handle_error "Database was not created successfully"
    fi
    
    # Get restored database info
    RESTORED_SIZE=$(PGPASSWORD="$POSTGRES_PASSWORD" psql -h "$POSTGRES_HOST" -U "$POSTGRES_USER" -d "$POSTGRES_DB" \
        -t -c "SELECT pg_size_pretty(pg_database_size('$POSTGRES_DB'));" | tr -d ' ')
    
    RESTORED_TABLE_COUNT=$(PGPASSWORD="$POSTGRES_PASSWORD" psql -h "$POSTGRES_HOST" -U "$POSTGRES_USER" -d "$POSTGRES_DB" \
        -t -c "SELECT count(*) FROM information_schema.tables WHERE table_schema='public';" | tr -d ' ')
    
    log "Restored database size: $RESTORED_SIZE"
    log "Restored table count: $RESTORED_TABLE_COUNT"
    
    # Basic connectivity test
    if PGPASSWORD="$POSTGRES_PASSWORD" psql -h "$POSTGRES_HOST" -U "$POSTGRES_USER" -d "$POSTGRES_DB" -c '\q'; then
        log "Database connectivity verified"
    else
        handle_error "Database connectivity test failed"
    fi
}

# Cleanup temporary files
cleanup() {
    if [[ "$BACKUP_FILE" == /tmp/* ]]; then
        log "Cleaning up temporary backup file..."
        rm -f "$BACKUP_FILE" || true
    fi
}

# Main restore function
main() {
    log "=== Starting TOSS ERP III Database Restore ==="
    
    validate_arguments
    download_from_s3
    validate_backup_file
    check_database
    get_current_database_info
    confirm_restore
    create_pre_restore_backup
    terminate_connections
    perform_restore
    verify_restore
    cleanup
    
    log "=== Database restore completed successfully ==="
}

# Error handling
trap 'handle_error "Restore interrupted"; cleanup' SIGTERM SIGINT

# Check for help
if [[ "${1:-}" == "-h" || "${1:-}" == "--help" ]]; then
    usage
fi

# Run main function
main "$@"
