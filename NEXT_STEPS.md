# TOSS MVP - Next Steps & Deployment Guide

## ✅ Completed

1. **Backend Implementation:**
   - ✅ All core features implemented
   - ✅ Application & Infrastructure layers compile successfully
   - ✅ OnboardingStatus migration file created manually
   - ✅ JWT authentication configured
   - ✅ Role-based authorization implemented

2. **Frontend Implementation:**
   - ✅ All role-based layouts created
   - ✅ All major pages implemented
   - ✅ API composables created
   - ✅ Onboarding wizards implemented
   - ✅ POS system with offline support

## 🚀 Deployment Steps

### 1. Database Setup

```bash
# Ensure PostgreSQL is running
# Update connection string in appsettings.json or appsettings.Development.json

# Apply migrations
cd backend/Toss
dotnet ef database update --project src/Infrastructure --startup-project src/Web
```

### 2. Backend Startup

```bash
cd backend/Toss
dotnet run --project src/Web
```

**Note:** The Web project will fail to build if PostgreSQL is not running (NSwag requires DB connection). This is expected. Options:
- Start PostgreSQL before building
- Or build with: `dotnet build src/Application src/Infrastructure` (skips Web/NSwag)
- Or temporarily disable NSwag in Web.csproj for initial setup

### 3. Frontend Startup

```bash
cd toss-web
npm install  # If not already done
npm run dev
```

### 4. Initial Data

The database seeder will create:
- Admin user: `admin@toss.local` / `Admin1!`
- 15 Store Owner users: `storeowner1@toss.local` through `storeowner15@toss.local` / `StoreOwner1!`
- All necessary roles

## 📋 Pre-Deployment Checklist

- [ ] PostgreSQL database created and running
- [ ] Connection string configured in `appsettings.json`
- [ ] JWT secret key changed in `appsettings.json` (use strong random key)
- [ ] Database migrations applied
- [ ] Backend starts without errors
- [ ] Frontend connects to backend API
- [ ] Test login with admin account
- [ ] Test onboarding flow for each role
- [ ] Test POS sale creation
- [ ] Test purchase order creation
- [ ] Test supplier order acceptance
- [ ] Test driver delivery updates

## 🔧 Configuration

### Backend (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=TossErp;Username=postgres;Password=yourpassword"
  },
  "Jwt": {
    "Key": "YOUR_SECRET_KEY_MINIMUM_32_CHARACTERS_LONG",
    "Issuer": "TossErp",
    "Audience": "TossErp",
    "ExpiryMinutes": 60
  }
}
```

### Frontend (`nuxt.config.ts`)

Ensure API base URL is correct:
```typescript
runtimeConfig: {
  public: {
    apiBase: process.env.API_BASE_URL || 'http://localhost:5000'
  }
}
```

## 🐛 Known Issues & Workarounds

### Issue: NSwag Build Error
**Symptom:** `Failed to connect to 127.0.0.1:5432` during build
**Cause:** NSwag tries to introspect API at build time, requires DB connection
**Workaround:** 
- Start PostgreSQL before building
- Or build only Application/Infrastructure: `dotnet build src/Application src/Infrastructure`
- Or run without building: `dotnet run --no-build` (after initial build with DB)

### Issue: OnboardingStatus Migration
**Status:** ✅ Migration file created manually at `src/Infrastructure/Data/Migrations/20251119223245_AddOnboardingStatus.cs`
**Action:** Run `dotnet ef database update` to apply

### Issue: ShopId/SupplierId Hardcoded
**Status:** Frontend currently uses hardcoded `1`
**Fix Needed:** Get from user session/claims
**Priority:** Medium (works for MVP, fix for production)

## 📊 Testing

See `TESTING_GUIDE.md` for comprehensive testing instructions.

Quick smoke test:
1. Login as admin
2. Create a retailer account
3. Login as retailer, complete onboarding
4. Add a product
5. Make a POS sale
6. Verify inventory decreased

## 🎯 Production Readiness

### High Priority
- [ ] Change JWT secret key to strong random value
- [ ] Configure production database connection string
- [ ] Set up proper CORS for production domain
- [ ] Enable HTTPS
- [ ] Configure logging (Serilog to file/database)

### Medium Priority
- [ ] Get shopId/supplierId from user session (not hardcoded)
- [ ] Add pagination to product/order lists
- [ ] Add input validation on all forms
- [ ] Add error boundaries in frontend
- [ ] Add loading states everywhere

### Low Priority
- [ ] Photo upload for driver deliveries
- [ ] Advanced reporting
- [ ] Email notifications
- [ ] SMS notifications
- [ ] Advanced analytics

## 📝 Migration Notes

The `AddOnboardingStatus` migration was created manually because:
- EF Core migration tool requires database connection
- NSwag build step requires database connection
- Manual creation allows deployment without DB during build

The migration is ready to apply once PostgreSQL is available.

## 🆘 Troubleshooting

### Backend won't start
1. Check PostgreSQL is running
2. Verify connection string
3. Check migrations are applied: `dotnet ef migrations list`
4. Check logs in `appsettings.json` Logging section

### Frontend can't connect to backend
1. Verify backend is running on expected port
2. Check CORS configuration
3. Check API base URL in `nuxt.config.ts`
4. Check browser console for errors

### Authentication fails
1. Verify JWT secret key is set
2. Check token expiration time
3. Verify user exists in database
4. Check user is not locked out

### Onboarding redirect loop
1. Check onboarding status API returns correct format
2. Verify role parameter is passed correctly
3. Check user has completed onboarding in database

## 📞 Support

For issues or questions:
1. Check `MVP_IMPLEMENTATION_SUMMARY.md` for feature details
2. Check `TESTING_GUIDE.md` for testing procedures
3. Review code comments and TODOs in source files

