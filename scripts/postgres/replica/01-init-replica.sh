#!/bin/bash
set -e

# TOSS ERP III PostgreSQL Replica Initialization
# Sets up streaming replication from primary

# Wait for primary to be ready
echo "Waiting for primary database to be ready..."
until pg_isready -h $POSTGRES_PRIMARY_HOST -p $POSTGRES_PRIMARY_PORT -U replicator; do
    echo "Primary not ready, waiting..."
    sleep 5
done

echo "Primary database is ready, setting up replica..."

# Stop PostgreSQL if running
pg_ctl stop -D /var/lib/postgresql/data -m fast || true

# Clear data directory
rm -rf /var/lib/postgresql/data/*

# Create base backup from primary
PGPASSWORD=$POSTGRES_REPLICATION_PASSWORD pg_basebackup \
    -h $POSTGRES_PRIMARY_HOST \
    -p $POSTGRES_PRIMARY_PORT \
    -U replicator \
    -D /var/lib/postgresql/data \
    -Fp \
    -Xs \
    -P \
    -R

# Set correct permissions
chown -R postgres:postgres /var/lib/postgresql/data
chmod 700 /var/lib/postgresql/data

# Create recovery configuration (standby.signal file is created by -R option)
echo "Replica initialization completed successfully"
