# How to Apply AI Integration Migration

The database already has tables created, but the EF migrations history is not tracking them. Here are several options to apply the AI migration:

## Option 1: Direct SQL Execution (Recommended)

The idempotent SQL script has been generated: **`AI_Migration.sql`**

### Using pgAdmin or PostgreSQL GUI:
1. Open pgAdmin or your PostgreSQL GUI tool
2. Connect to database: `TossErp`
3. Open Query Tool
4. Copy contents from `AI_Migration.sql`
5. Execute the script

### Using psql (if available):
```bash
psql -h 127.0.0.1 -p 5432 -U toss -d TossErp -f AI_Migration.sql
# Password: toss123
```

## Option 2: Mark Base Migration as Applied, Then Update

```bash
# Step 1: Mark the base migration as applied manually
# Execute this SQL in your PostgreSQL database:

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251025114416_ConsolidatedEntitiesInitial', '9.0.0')
ON CONFLICT DO NOTHING;

# Step 2: Then run the EF migration
cd backend/Toss
dotnet ef database update --project src/Infrastructure --startup-project src/Web
```

## Option 3: Manual SQL Commands

Execute these SQL commands in order against the TossErp database:

```sql
-- 1. Rename ApiKey to GeminiApiKey in AISettings
ALTER TABLE "AISettings" RENAME COLUMN "ApiKey" TO "GeminiApiKey";

-- 2. Drop old timeout column
ALTER TABLE "AISettings" DROP COLUMN IF EXISTS "RequestTimeoutSeconds";

-- 3. Add new AISettings columns
ALTER TABLE "AISettings" ADD COLUMN IF NOT EXISTS "ChatGptApiKey" character varying(500);
ALTER TABLE "AISettings" ADD COLUMN IF NOT EXISTS "DeepSeekApiKey" character varying(500);
ALTER TABLE "AISettings" ADD COLUMN IF NOT EXISTS "RequestTimeout" integer;
ALTER TABLE "AISettings" ADD COLUMN IF NOT EXISTS "AllowMetaTitleGeneration" boolean NOT NULL DEFAULT FALSE;
ALTER TABLE "AISettings" ADD COLUMN IF NOT EXISTS "AllowMetaKeywordsGeneration" boolean NOT NULL DEFAULT FALSE;
ALTER TABLE "AISettings" ADD COLUMN IF NOT EXISTS "AllowMetaDescriptionGeneration" boolean NOT NULL DEFAULT FALSE;
ALTER TABLE "AISettings" ADD COLUMN IF NOT EXISTS "ProductDescriptionQuery" text;
ALTER TABLE "AISettings" ADD COLUMN IF NOT EXISTS "MetaTitleQuery" text;
ALTER TABLE "AISettings" ADD COLUMN IF NOT EXISTS "MetaKeywordsQuery" text;
ALTER TABLE "AISettings" ADD COLUMN IF NOT EXISTS "MetaDescriptionQuery" text;

-- 4. Add meta fields to Products
ALTER TABLE "Products" ADD COLUMN IF NOT EXISTS "MetaTitle" text;
ALTER TABLE "Products" ADD COLUMN IF NOT EXISTS "MetaKeywords" text;
ALTER TABLE "Products" ADD COLUMN IF NOT EXISTS "MetaDescription" text;

-- 5. Add meta fields to ProductCategories
ALTER TABLE "ProductCategories" ADD COLUMN IF NOT EXISTS "MetaTitle" text;
ALTER TABLE "ProductCategories" ADD COLUMN IF NOT EXISTS "MetaKeywords" text;
ALTER TABLE "ProductCategories" ADD COLUMN IF NOT EXISTS "MetaDescription" text;

-- 6. Add meta fields to Vendors
ALTER TABLE "Vendors" ADD COLUMN IF NOT EXISTS "MetaTitle" text;
ALTER TABLE "Vendors" ADD COLUMN IF NOT EXISTS "MetaKeywords" text;
ALTER TABLE "Vendors" ADD COLUMN IF NOT EXISTS "MetaDescription" text;

-- 7. Mark migration as applied
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251026113028_AddAIIntegrationSupport', '9.0.0')
ON CONFLICT DO NOTHING;
```

## Verification

After applying the migration, verify it worked:

```bash
# Check migration status
dotnet ef migrations list --project src/Infrastructure --startup-project src/Web

# Both migrations should show as "Applied":
# 20251025114416_ConsolidatedEntitiesInitial (Applied)
# 20251026113028_AddAIIntegrationSupport (Applied)
```

## What the Migration Does

The AI migration adds:

**AISettings table updates:**
- Renames `ApiKey` → `GeminiApiKey`
- Adds `ChatGptApiKey` and `DeepSeekApiKey` (multi-provider support)
- Adds meta generation flags (AllowMetaTitle/Keywords/DescriptionGeneration)
- Adds customizable AI query templates
- Changes `RequestTimeoutSeconds` → `RequestTimeout` (nullable)

**SEO Meta Fields:**
- Adds `MetaTitle`, `MetaKeywords`, `MetaDescription` to:
  - Products
  - ProductCategories
  - Vendors

## Next Steps After Migration

1. ✅ Apply the migration using one of the options above
2. ✅ Configure AI provider API keys in AISettings table
3. ✅ Test the application and AI endpoints
4. ✅ Start using the AI chat features!

---

**Note:** The `AI_Migration.sql` file is idempotent and safe to run multiple times - it checks if already applied before making changes.

