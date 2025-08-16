# Running All Applications Locally

This directory contains scripts to launch all TOSS ERP applications simultaneously.

## Quick Start

### Option 1: PowerShell (Recommended)
```powershell
# Run with browser auto-open
.\launch-all-apps.ps1 -OpenBrowser

# Run without browser auto-open
.\launch-all-apps.ps1

# Skip prerequisite checks
.\launch-all-apps.ps1 -SkipPrerequisites
```

### Option 2: Batch File
```cmd
# Double-click the file or run from command prompt
launch-all-apps.bat
```

## What Gets Started

The scripts will launch the following applications in separate console windows:

### Backend Services
- **API Gateway** - http://localhost:8080
- **Stock API** - http://localhost:5001

### Client Applications
- **Mobile Client** (Flutter Web) - http://localhost:5000
- **Web Client** (Nuxt.js) - http://localhost:5173
- **Admin Client** (React) - http://localhost:3000

## Prerequisites

Before running the scripts, ensure you have:

1. **.NET 8.0 SDK**
   - Download from: https://dotnet.microsoft.com/download
   - Verify with: `dotnet --version`

2. **Node.js 18+**
   - Download from: https://nodejs.org/
   - Verify with: `node --version`

3. **Flutter 3.0+**
   - Download from: https://flutter.dev/docs/get-started/install
   - Verify with: `flutter --version`

## Manual Setup (Alternative)

If you prefer to start applications manually:

### 1. Start Backend Services
```bash
# Terminal 1: Start Gateway
cd src/Gateway
dotnet run

# Terminal 2: Start Stock API
cd src/Services/Stock/Stock.API
dotnet run
```

### 2. Start Client Applications
```bash
# Terminal 3: Start Mobile Client
cd src/clients/mobile
flutter run -d web-server --web-port 5000

# Terminal 4: Start Web Client
cd src/clients/web
npm run dev

# Terminal 5: Start Admin Client
cd src/clients/admin
npm start
```

## Environment Configuration

The scripts automatically create `.env` files from templates:
- `src/clients/mobile/.env`
- `src/clients/web/.env`
- `src/clients/admin/.env`

These files configure the API endpoints to point to the Gateway.

## Troubleshooting

### Common Issues

1. **Port Already in Use**
   - Close other applications using the same ports
   - Or modify the port numbers in the scripts

2. **Prerequisites Not Found**
   - Install missing tools
   - Ensure they're in your system PATH
   - Restart your terminal after installation

3. **Build Errors**
   - Run `npm install` in client directories first
   - Run `flutter pub get` in mobile directory first
   - Ensure all dependencies are installed

### Verification

Test that all services are running:

```powershell
# Test Gateway
Invoke-WebRequest http://localhost:8080/health

# Test Stock API
Invoke-WebRequest http://localhost:5001/health

# Test Mobile Client
Invoke-WebRequest http://localhost:5000

# Test Web Client
Invoke-WebRequest http://localhost:5173

# Test Admin Client
Invoke-WebRequest http://localhost:3000
```

## Stopping Applications

To stop all applications:
1. Close each console window
2. Or press `Ctrl+C` in each window
3. Or use Task Manager to end the processes

## Development Workflow

1. **Start all applications** using the launch script
2. **Make code changes** in your preferred editor
3. **Applications auto-reload** when you save changes
4. **Test changes** in the browser
5. **Stop applications** when done developing

## Next Steps

After all applications are running:

1. **Test the integration** using the test scripts:
   ```powershell
   .\test-client-integration.ps1
   ```

2. **Deploy to production** using the deployment scripts:
   ```powershell
   .\deploy-clients.sh
   ```

3. **Monitor health** of all services through the Gateway

## Support

If you encounter issues:
1. Check the console output for error messages
2. Verify all prerequisites are installed
3. Ensure no other applications are using the required ports
4. Check the troubleshooting section above
