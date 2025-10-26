-- Mark the consolidated entities migration as already applied
-- This is safe because the tables already exist
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251025114416_ConsolidatedEntitiesInitial', '9.0.0')
ON CONFLICT DO NOTHING;

