START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251026113028_AddAIIntegrationSupport') THEN
    ALTER TABLE "AISettings" DROP COLUMN "RequestTimeoutSeconds";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251026113028_AddAIIntegrationSupport') THEN
    ALTER TABLE "AISettings" RENAME COLUMN "ApiKey" TO "GeminiApiKey";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251026113028_AddAIIntegrationSupport') THEN
    ALTER TABLE "Vendors" ADD "MetaDescription" text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251026113028_AddAIIntegrationSupport') THEN
    ALTER TABLE "Vendors" ADD "MetaKeywords" text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251026113028_AddAIIntegrationSupport') THEN
    ALTER TABLE "Vendors" ADD "MetaTitle" text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251026113028_AddAIIntegrationSupport') THEN
    ALTER TABLE "Products" ADD "MetaDescription" text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251026113028_AddAIIntegrationSupport') THEN
    ALTER TABLE "Products" ADD "MetaKeywords" text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251026113028_AddAIIntegrationSupport') THEN
    ALTER TABLE "Products" ADD "MetaTitle" text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251026113028_AddAIIntegrationSupport') THEN
    ALTER TABLE "ProductCategories" ADD "MetaDescription" text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251026113028_AddAIIntegrationSupport') THEN
    ALTER TABLE "ProductCategories" ADD "MetaKeywords" text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251026113028_AddAIIntegrationSupport') THEN
    ALTER TABLE "ProductCategories" ADD "MetaTitle" text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251026113028_AddAIIntegrationSupport') THEN
    ALTER TABLE "AISettings" ADD "AllowMetaDescriptionGeneration" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251026113028_AddAIIntegrationSupport') THEN
    ALTER TABLE "AISettings" ADD "AllowMetaKeywordsGeneration" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251026113028_AddAIIntegrationSupport') THEN
    ALTER TABLE "AISettings" ADD "AllowMetaTitleGeneration" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251026113028_AddAIIntegrationSupport') THEN
    ALTER TABLE "AISettings" ADD "ChatGptApiKey" character varying(500);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251026113028_AddAIIntegrationSupport') THEN
    ALTER TABLE "AISettings" ADD "DeepSeekApiKey" character varying(500);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251026113028_AddAIIntegrationSupport') THEN
    ALTER TABLE "AISettings" ADD "MetaDescriptionQuery" text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251026113028_AddAIIntegrationSupport') THEN
    ALTER TABLE "AISettings" ADD "MetaKeywordsQuery" text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251026113028_AddAIIntegrationSupport') THEN
    ALTER TABLE "AISettings" ADD "MetaTitleQuery" text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251026113028_AddAIIntegrationSupport') THEN
    ALTER TABLE "AISettings" ADD "ProductDescriptionQuery" text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251026113028_AddAIIntegrationSupport') THEN
    ALTER TABLE "AISettings" ADD "RequestTimeout" integer;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251026113028_AddAIIntegrationSupport') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251026113028_AddAIIntegrationSupport', '9.0.0');
    END IF;
END $EF$;
COMMIT;

