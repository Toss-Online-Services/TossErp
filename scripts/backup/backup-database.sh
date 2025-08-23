#!/bin/bash
set -euo pipefail

# TOSS ERP III Database Backup Script
# Creates full database backups with compression and optional S3 upload

# Configuration
BACKUP_DIR="/backups"
LOG_FILE="/backups/backup.log"
POSTGRES_HOST="${POSTGRES_HOST:-postgres-primary}"
POSTGRES_DB="${POSTGRES_DB:-toss_erp}"
POSTGRES_USER="${POSTGRES_USER:-toss_user}"
POSTGRES_PASSWORD="${POSTGRES_PASSWORD:-development_password}"
BACKUP_RETENTION_DAYS="${BACKUP_RETENTION_DAYS:-30}"
S3_BUCKET="${S3_BUCKET:-}"

# Date formatting
DATE=$(date '+%Y-%m-%d_%H-%M-%S')
DAY_OF_WEEK=$(date '+%u')  # 1=Monday, 7=Sunday
DAY_OF_MONTH=$(date '+%d')

# Backup types
BACKUP_TYPE="daily"
if [[ "$DAY_OF_WEEK" == "7" ]]; then
    BACKUP_TYPE="weekly"
fi
if [[ "$DAY_OF_MONTH" == "01" ]]; then
    BACKUP_TYPE="monthly"
fi

# File paths
BACKUP_FILE="$BACKUP_DIR/$BACKUP_TYPE/toss_erp_${BACKUP_TYPE}_${DATE}.sql.gz"
BACKUP_INFO_FILE="$BACKUP_DIR/$BACKUP_TYPE/toss_erp_${BACKUP_TYPE}_${DATE}.info"

# Logging function
log() {
    echo "[$(date '+%Y-%m-%d %H:%M:%S')] $1" | tee -a "$LOG_FILE"
}

# Error handling
handle_error() {
    log "ERROR: Backup failed at step: $1"
    exit 1
}

# Check database connectivity
check_database() {
    log "Checking database connectivity..."
    
    if ! PGPASSWORD="$POSTGRES_PASSWORD" psql -h "$POSTGRES_HOST" -U "$POSTGRES_USER" -d "$POSTGRES_DB" -c '\q' 2>/dev/null; then
        handle_error "Database connection failed"
    fi
    
    log "Database connection successful"
}

# Get database size
get_database_info() {
    log "Gathering database information..."
    
    PGPASSWORD="$POSTGRES_PASSWORD" psql -h "$POSTGRES_HOST" -U "$POSTGRES_USER" -d "$POSTGRES_DB" \
        -t -c "SELECT pg_size_pretty(pg_database_size('$POSTGRES_DB'));" | tr -d ' ' > /tmp/db_size.txt
    
    DB_SIZE=$(cat /tmp/db_size.txt)
    log "Database size: $DB_SIZE"
}

# Create backup info file
create_backup_info() {
    cat > "$BACKUP_INFO_FILE" << EOF
Backup Information
==================
Database: $POSTGRES_DB
Host: $POSTGRES_HOST
User: $POSTGRES_USER
Backup Type: $BACKUP_TYPE
Backup Date: $(date '+%Y-%m-%d %H:%M:%S UTC')
Database Size: $DB_SIZE
Backup File: $(basename "$BACKUP_FILE")
PostgreSQL Version: $(PGPASSWORD="$POSTGRES_PASSWORD" psql -h "$POSTGRES_HOST" -U "$POSTGRES_USER" -d "$POSTGRES_DB" -t -c "SELECT version();" | head -1 | tr -d ' ')
EOF
}

# Perform database backup
perform_backup() {
    log "Starting $BACKUP_TYPE backup..."
    
    # Create directory if it doesn't exist
    mkdir -p "$(dirname "$BACKUP_FILE")"
    
    # Perform backup with compression
    PGPASSWORD="$POSTGRES_PASSWORD" pg_dump \
        -h "$POSTGRES_HOST" \
        -U "$POSTGRES_USER" \
        -d "$POSTGRES_DB" \
        --verbose \
        --create \
        --clean \
        --if-exists \
        --quote-all-identifiers \
        --no-privileges \
        --no-owner \
        --format=plain \
        | gzip > "$BACKUP_FILE"
    
    if [[ ${PIPESTATUS[0]} -eq 0 ]]; then
        BACKUP_SIZE=$(du -h "$BACKUP_FILE" | cut -f1)
        log "Backup completed successfully. Compressed size: $BACKUP_SIZE"
        create_backup_info
    else
        handle_error "pg_dump failed"
    fi
}

# Upload to S3 if configured
upload_to_s3() {
    if [[ -n "$S3_BUCKET" ]]; then
        log "Uploading backup to S3 bucket: $S3_BUCKET"
        
        S3_KEY="toss-erp-backups/$BACKUP_TYPE/$(basename "$BACKUP_FILE")"
        S3_INFO_KEY="toss-erp-backups/$BACKUP_TYPE/$(basename "$BACKUP_INFO_FILE")"
        
        if aws s3 cp "$BACKUP_FILE" "s3://$S3_BUCKET/$S3_KEY" && \
           aws s3 cp "$BACKUP_INFO_FILE" "s3://$S3_BUCKET/$S3_INFO_KEY"; then
            log "Backup uploaded to S3 successfully"
        else
            log "WARNING: S3 upload failed, backup saved locally only"
        fi
    else
        log "S3 not configured, backup saved locally only"
    fi
}

# Clean up old backups
cleanup_old_backups() {
    log "Cleaning up backups older than $BACKUP_RETENTION_DAYS days..."
    
    # Clean local backups
    find "$BACKUP_DIR" -name "*.sql.gz" -mtime +$BACKUP_RETENTION_DAYS -delete || true
    find "$BACKUP_DIR" -name "*.info" -mtime +$BACKUP_RETENTION_DAYS -delete || true
    
    # Clean S3 backups if configured
    if [[ -n "$S3_BUCKET" ]]; then
        CUTOFF_DATE=$(date -d "-$BACKUP_RETENTION_DAYS days" '+%Y-%m-%d')
        aws s3 ls "s3://$S3_BUCKET/toss-erp-backups/" --recursive | \
        while read -r line; do
            FILE_DATE=$(echo "$line" | awk '{print $1}')
            FILE_PATH=$(echo "$line" | awk '{print $4}')
            
            if [[ "$FILE_DATE" < "$CUTOFF_DATE" ]]; then
                aws s3 rm "s3://$S3_BUCKET/$FILE_PATH" || true
            fi
        done
    fi
    
    log "Cleanup completed"
}

# Verify backup integrity
verify_backup() {
    log "Verifying backup integrity..."
    
    if gzip -t "$BACKUP_FILE"; then
        log "Backup file integrity verified"
    else
        handle_error "Backup file is corrupted"
    fi
}

# Main backup function
main() {
    log "=== Starting TOSS ERP III Database Backup ==="
    log "Backup type: $BACKUP_TYPE"
    
    check_database
    get_database_info
    perform_backup
    verify_backup
    upload_to_s3
    cleanup_old_backups
    
    log "=== Backup completed successfully ==="
}

# Error handling
trap 'handle_error "Backup interrupted"' SIGTERM SIGINT

# Run main function
main "$@"
