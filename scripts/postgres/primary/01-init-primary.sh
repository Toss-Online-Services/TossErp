#!/bin/bash
set -e

# TOSS ERP III PostgreSQL Primary Initialization
# Sets up the primary database with replication users and extensions

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
    -- Create application roles
    DO \$\$
    BEGIN
        -- App service role (read/write)
        IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = 'app_service') THEN
            CREATE ROLE app_service WITH LOGIN PASSWORD 'app_service_password';
        END IF;
        
        -- App readonly role (reports/analytics)
        IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = 'app_readonly') THEN
            CREATE ROLE app_readonly WITH LOGIN PASSWORD 'app_readonly_password';
        END IF;
        
        -- Migration role (schema changes)
        IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = 'app_migration') THEN
            CREATE ROLE app_migration WITH LOGIN PASSWORD 'app_migration_password' CREATEDB;
        END IF;
        
        -- Replication user
        IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = 'replicator') THEN
            CREATE ROLE replicator WITH LOGIN REPLICATION PASSWORD '${POSTGRES_REPLICATION_PASSWORD}';
        END IF;
    END
    \$\$;

    -- Grant database permissions
    GRANT CONNECT ON DATABASE ${POSTGRES_DB} TO app_service;
    GRANT CONNECT ON DATABASE ${POSTGRES_DB} TO app_readonly;
    GRANT CONNECT ON DATABASE ${POSTGRES_DB} TO app_migration;
    
    -- Grant schema permissions
    GRANT USAGE ON SCHEMA public TO app_service;
    GRANT USAGE ON SCHEMA public TO app_readonly;
    GRANT ALL ON SCHEMA public TO app_migration;
    
    -- Grant table permissions (for future tables)
    ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO app_service;
    ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT ON TABLES TO app_readonly;
    ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON TABLES TO app_migration;
    
    -- Grant sequence permissions
    ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT USAGE, SELECT ON SEQUENCES TO app_service;
    ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT USAGE, SELECT ON SEQUENCES TO app_readonly;
    ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON SEQUENCES TO app_migration;

    -- Create extensions
    CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
    CREATE EXTENSION IF NOT EXISTS "pg_stat_statements";
    CREATE EXTENSION IF NOT EXISTS "pg_trgm";
    CREATE EXTENSION IF NOT EXISTS "btree_gin";
    CREATE EXTENSION IF NOT EXISTS "btree_gist";

    -- Create replication slot for streaming replication
    SELECT pg_create_physical_replication_slot('replica_slot');

    -- Create WAL archive directory
    \! mkdir -p /var/lib/postgresql/wal_archive

    -- Set up audit logging function
    CREATE OR REPLACE FUNCTION fn_audit_trigger()
    RETURNS TRIGGER AS \$audit\$
    BEGIN
        IF TG_OP = 'DELETE' THEN
            INSERT INTO audit_log (table_name, operation, old_values, new_values, changed_by, changed_at)
            VALUES (TG_TABLE_NAME, TG_OP, row_to_json(OLD), NULL, SESSION_USER, NOW());
            RETURN OLD;
        ELSIF TG_OP = 'UPDATE' THEN
            INSERT INTO audit_log (table_name, operation, old_values, new_values, changed_by, changed_at)
            VALUES (TG_TABLE_NAME, TG_OP, row_to_json(OLD), row_to_json(NEW), SESSION_USER, NOW());
            RETURN NEW;
        ELSIF TG_OP = 'INSERT' THEN
            INSERT INTO audit_log (table_name, operation, old_values, new_values, changed_by, changed_at)
            VALUES (TG_TABLE_NAME, TG_OP, NULL, row_to_json(NEW), SESSION_USER, NOW());
            RETURN NEW;
        END IF;
        RETURN NULL;
    END;
    \$audit\$ LANGUAGE plpgsql;

    -- Create audit log table
    CREATE TABLE IF NOT EXISTS audit_log (
        id BIGSERIAL PRIMARY KEY,
        table_name TEXT NOT NULL,
        operation TEXT NOT NULL,
        old_values JSONB,
        new_values JSONB,
        changed_by TEXT NOT NULL,
        changed_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
    );

    CREATE INDEX IF NOT EXISTS idx_audit_log_table_name ON audit_log (table_name);
    CREATE INDEX IF NOT EXISTS idx_audit_log_changed_at ON audit_log (changed_at);
    CREATE INDEX IF NOT EXISTS idx_audit_log_changed_by ON audit_log (changed_by);

    -- Grant audit log permissions
    GRANT SELECT, INSERT ON audit_log TO app_service;
    GRANT SELECT ON audit_log TO app_readonly;
    GRANT ALL ON audit_log TO app_migration;
    GRANT USAGE, SELECT ON SEQUENCE audit_log_id_seq TO app_service;

    -- Create tenant access function
    CREATE OR REPLACE FUNCTION fn_tenant_access_predicate(tenant_id UUID)
    RETURNS BOOLEAN AS \$\$
    BEGIN
        -- Allow system operations (no tenant context)
        IF SESSION_USER IN ('app_migration', 'postgres') THEN
            RETURN TRUE;
        END IF;
        
        -- Get current tenant from session variable
        DECLARE
            current_tenant_id UUID;
        BEGIN
            current_tenant_id := CAST(CURRENT_SETTING('app.current_tenant_id', true) AS UUID);
            
            -- If no tenant context is set, deny access
            IF current_tenant_id IS NULL THEN
                RETURN FALSE;
            END IF;
            
            -- Allow access if tenant matches
            RETURN tenant_id = current_tenant_id;
        EXCEPTION
            WHEN OTHERS THEN
                -- If setting doesn't exist or conversion fails, deny access
                RETURN FALSE;
        END;
    END;
    \$\$ LANGUAGE plpgsql SECURITY DEFINER;

EOSQL

echo "PostgreSQL primary initialization completed successfully"
