-- TOSS ERP Database Initialization Script
-- This script sets up the basic database structure for multi-tenant architecture

-- Create extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_trgm";

-- Create schemas for different services
CREATE SCHEMA IF NOT EXISTS identity;
CREATE SCHEMA IF NOT EXISTS stock;
CREATE SCHEMA IF NOT EXISTS sales;
CREATE SCHEMA IF NOT EXISTS buying;
CREATE SCHEMA IF NOT EXISTS accounting;
CREATE SCHEMA IF NOT EXISTS crm;
CREATE SCHEMA IF NOT EXISTS notifications;
CREATE SCHEMA IF NOT EXISTS collaboration;
CREATE SCHEMA IF NOT EXISTS ai;

-- Create tenants table (shared across all services)
CREATE TABLE IF NOT EXISTS tenants (
    id VARCHAR(100) PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    display_name VARCHAR(200) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    settings JSONB DEFAULT '{}'::jsonb
);

-- Create users table in identity schema
CREATE TABLE IF NOT EXISTS identity.users (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id VARCHAR(100) NOT NULL REFERENCES tenants(id),
    email VARCHAR(255) NOT NULL,
    password_hash VARCHAR(500) NOT NULL,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT true,
    email_confirmed BOOLEAN NOT NULL DEFAULT false,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uk_users_email_tenant UNIQUE (email, tenant_id)
);

-- Create roles table
CREATE TABLE IF NOT EXISTS identity.roles (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id VARCHAR(100) NOT NULL REFERENCES tenants(id),
    name VARCHAR(100) NOT NULL,
    display_name VARCHAR(200) NOT NULL,
    description TEXT,
    permissions JSONB DEFAULT '[]'::jsonb,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uk_roles_name_tenant UNIQUE (name, tenant_id)
);

-- Create user roles junction table
CREATE TABLE IF NOT EXISTS identity.user_roles (
    user_id UUID NOT NULL REFERENCES identity.users(id) ON DELETE CASCADE,
    role_id UUID NOT NULL REFERENCES identity.roles(id) ON DELETE CASCADE,
    assigned_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    assigned_by UUID REFERENCES identity.users(id),
    PRIMARY KEY (user_id, role_id)
);

-- Create refresh tokens table
CREATE TABLE IF NOT EXISTS identity.refresh_tokens (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL REFERENCES identity.users(id) ON DELETE CASCADE,
    token_hash VARCHAR(500) NOT NULL,
    expires_at TIMESTAMPTZ NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    revoked_at TIMESTAMPTZ,
    replaced_by_token UUID
);

-- Create basic stock tables
CREATE TABLE IF NOT EXISTS stock.categories (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id VARCHAR(100) NOT NULL REFERENCES tenants(id),
    name VARCHAR(200) NOT NULL,
    description TEXT,
    parent_id UUID REFERENCES stock.categories(id),
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uk_categories_name_tenant UNIQUE (name, tenant_id)
);

CREATE TABLE IF NOT EXISTS stock.items (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id VARCHAR(100) NOT NULL REFERENCES tenants(id),
    sku VARCHAR(100) NOT NULL,
    name VARCHAR(200) NOT NULL,
    description TEXT,
    category_id UUID REFERENCES stock.categories(id),
    unit_of_measure VARCHAR(50) NOT NULL DEFAULT 'each',
    cost_price DECIMAL(18,4) NOT NULL DEFAULT 0,
    selling_price DECIMAL(18,4) NOT NULL DEFAULT 0,
    reorder_level DECIMAL(18,4) NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uk_items_sku_tenant UNIQUE (sku, tenant_id)
);

-- Create outbox table for event sourcing
CREATE TABLE IF NOT EXISTS outbox_events (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id VARCHAR(100) NOT NULL REFERENCES tenants(id),
    aggregate_id UUID NOT NULL,
    event_type VARCHAR(200) NOT NULL,
    event_data JSONB NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    processed_at TIMESTAMPTZ,
    retry_count INTEGER NOT NULL DEFAULT 0,
    error_message TEXT
);

-- Create indexes for performance
CREATE INDEX IF NOT EXISTS idx_tenants_active ON tenants(is_active);
CREATE INDEX IF NOT EXISTS idx_users_tenant_email ON identity.users(tenant_id, email);
CREATE INDEX IF NOT EXISTS idx_users_active ON identity.users(is_active);
CREATE INDEX IF NOT EXISTS idx_refresh_tokens_user ON identity.refresh_tokens(user_id);
CREATE INDEX IF NOT EXISTS idx_refresh_tokens_expires ON identity.refresh_tokens(expires_at);
CREATE INDEX IF NOT EXISTS idx_categories_tenant ON stock.categories(tenant_id);
CREATE INDEX IF NOT EXISTS idx_categories_parent ON stock.categories(parent_id);
CREATE INDEX IF NOT EXISTS idx_items_tenant ON stock.items(tenant_id);
CREATE INDEX IF NOT EXISTS idx_items_category ON stock.items(category_id);
CREATE INDEX IF NOT EXISTS idx_items_sku ON stock.items(sku);
CREATE INDEX IF NOT EXISTS idx_outbox_processed ON outbox_events(processed_at);
CREATE INDEX IF NOT EXISTS idx_outbox_tenant ON outbox_events(tenant_id);

-- Create RLS (Row Level Security) policies
ALTER TABLE tenants ENABLE ROW LEVEL SECURITY;
ALTER TABLE identity.users ENABLE ROW LEVEL SECURITY;
ALTER TABLE identity.roles ENABLE ROW LEVEL SECURITY;
ALTER TABLE stock.categories ENABLE ROW LEVEL SECURITY;
ALTER TABLE stock.items ENABLE ROW LEVEL SECURITY;
ALTER TABLE outbox_events ENABLE ROW LEVEL SECURITY;

-- Insert default tenant for development
INSERT INTO tenants (id, name, display_name, is_active) 
VALUES ('dev-tenant', 'dev-tenant', 'Development Tenant', true)
ON CONFLICT (id) DO NOTHING;

-- Insert default admin user for development
INSERT INTO identity.users (id, tenant_id, email, password_hash, first_name, last_name, is_active, email_confirmed)
VALUES (
    '00000000-0000-0000-0000-000000000001',
    'dev-tenant',
    'admin@toss-erp.local',
    '$2a$11$8G5Z5Z5Z5Z5Z5Z5Z5Z5Z5O', -- This is a placeholder - should be properly hashed
    'System',
    'Administrator',
    true,
    true
) ON CONFLICT (email, tenant_id) DO NOTHING;

-- Insert default admin role
INSERT INTO identity.roles (id, tenant_id, name, display_name, description, permissions)
VALUES (
    '00000000-0000-0000-0000-000000000001',
    'dev-tenant',
    'admin',
    'Administrator',
    'Full system access',
    '["*"]'::jsonb
) ON CONFLICT (name, tenant_id) DO NOTHING;

-- Assign admin role to admin user
INSERT INTO identity.user_roles (user_id, role_id)
VALUES (
    '00000000-0000-0000-0000-000000000001',
    '00000000-0000-0000-0000-000000000001'
) ON CONFLICT (user_id, role_id) DO NOTHING;

-- Insert sample stock category
INSERT INTO stock.categories (id, tenant_id, name, description)
VALUES (
    '00000000-0000-0000-0000-000000000001',
    'dev-tenant',
    'General',
    'General category for all items'
) ON CONFLICT (name, tenant_id) DO NOTHING;

-- Insert sample stock items
INSERT INTO stock.items (tenant_id, sku, name, description, category_id, cost_price, selling_price, reorder_level)
VALUES 
    ('dev-tenant', 'SAMPLE-001', 'Sample Product 1', 'This is a sample product for testing', '00000000-0000-0000-0000-000000000001', 10.00, 15.00, 5),
    ('dev-tenant', 'SAMPLE-002', 'Sample Product 2', 'Another sample product for testing', '00000000-0000-0000-0000-000000000001', 20.00, 30.00, 10)
ON CONFLICT (sku, tenant_id) DO NOTHING;

-- Create functions for updated_at triggers
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ language 'plpgsql';

-- Create triggers for updated_at
DROP TRIGGER IF EXISTS update_tenants_updated_at ON tenants;
CREATE TRIGGER update_tenants_updated_at BEFORE UPDATE ON tenants FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

DROP TRIGGER IF EXISTS update_users_updated_at ON identity.users;
CREATE TRIGGER update_users_updated_at BEFORE UPDATE ON identity.users FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

DROP TRIGGER IF EXISTS update_categories_updated_at ON stock.categories;
CREATE TRIGGER update_categories_updated_at BEFORE UPDATE ON stock.categories FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

DROP TRIGGER IF EXISTS update_items_updated_at ON stock.items;
CREATE TRIGGER update_items_updated_at BEFORE UPDATE ON stock.items FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

COMMIT;
