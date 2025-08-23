#!/bin/bash
set -euo pipefail

# TOSS ERP III Database Backup Scheduler
# Runs automated backups based on cron schedule

# Configuration
BACKUP_DIR="/backups"
LOG_FILE="/backups/backup.log"
POSTGRES_HOST="${POSTGRES_HOST:-postgres-primary}"
POSTGRES_DB="${POSTGRES_DB:-toss_erp}"
POSTGRES_USER="${POSTGRES_USER:-toss_user}"
POSTGRES_PASSWORD="${POSTGRES_PASSWORD:-development_password}"
BACKUP_RETENTION_DAYS="${BACKUP_RETENTION_DAYS:-30}"
BACKUP_SCHEDULE="${BACKUP_SCHEDULE:-0 2 * * *}"  # Daily at 2 AM
S3_BUCKET="${S3_BUCKET:-}"
S3_ACCESS_KEY="${S3_ACCESS_KEY:-}"
S3_SECRET_KEY="${S3_SECRET_KEY:-}"

# Logging function
log() {
    echo "[$(date '+%Y-%m-%d %H:%M:%S')] $1" | tee -a "$LOG_FILE"
}

# Install required packages
install_dependencies() {
    log "Installing backup dependencies..."
    apk add --no-cache dcron aws-cli postgresql-client
}

# Setup cron job
setup_cron() {
    log "Setting up backup schedule: $BACKUP_SCHEDULE"
    echo "$BACKUP_SCHEDULE /scripts/backup-database.sh" > /etc/crontabs/root
    
    # Setup S3 credentials if provided
    if [[ -n "$S3_ACCESS_KEY" && -n "$S3_SECRET_KEY" ]]; then
        mkdir -p ~/.aws
        cat > ~/.aws/credentials << EOF
[default]
aws_access_key_id = $S3_ACCESS_KEY
aws_secret_access_key = $S3_SECRET_KEY
EOF
        log "AWS credentials configured for S3 backup"
    fi
}

# Wait for database to be ready
wait_for_db() {
    log "Waiting for database to be ready..."
    
    until PGPASSWORD="$POSTGRES_PASSWORD" psql -h "$POSTGRES_HOST" -U "$POSTGRES_USER" -d "$POSTGRES_DB" -c '\q' 2>/dev/null; do
        log "Database not ready, waiting..."
        sleep 5
    done
    
    log "Database is ready"
}

# Create backup directories
setup_directories() {
    mkdir -p "$BACKUP_DIR/daily"
    mkdir -p "$BACKUP_DIR/weekly"
    mkdir -p "$BACKUP_DIR/monthly"
    mkdir -p "/var/lib/postgresql/wal_archive"
    
    log "Backup directories created"
}

# Initial backup
run_initial_backup() {
    log "Running initial backup..."
    /scripts/backup-database.sh
}

# Main function
main() {
    log "Starting TOSS ERP III Backup Scheduler"
    
    install_dependencies
    setup_directories
    wait_for_db
    setup_cron
    run_initial_backup
    
    log "Backup scheduler configured successfully"
    log "Starting cron daemon..."
    
    # Start cron in foreground
    exec crond -f -L /backups/cron.log
}

# Trap signals for graceful shutdown
trap 'log "Backup scheduler shutting down..."; exit 0' SIGTERM SIGINT

# Run main function
main "$@"
