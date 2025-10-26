-- Create the TossErp database if it doesn't exist
SELECT 'CREATE DATABASE "TossErp"'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'TossErp')\gexec

-- Grant privileges to toss user
GRANT ALL PRIVILEGES ON DATABASE "TossErp" TO toss;

