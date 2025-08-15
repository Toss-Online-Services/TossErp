# Flutter Setup Guide for TOSS ERP POS

This guide will help you set up Flutter and run the TOSS ERP POS mobile application.

## Prerequisites

### 1. Install Flutter SDK

1. **Download Flutter SDK**
   - Visit [Flutter Installation](https://docs.flutter.dev/get-started/install)
   - Download the Flutter SDK for your operating system
   - Extract the downloaded file to a desired location (e.g., `C:\flutter` on Windows)

2. **Add Flutter to PATH**
   - **Windows**: Add `C:\flutter\bin` to your system PATH
   - **macOS/Linux**: Add `~/flutter/bin` to your PATH
   - **Verify installation**: Open terminal/command prompt and run:
     ```bash
     flutter --version
     ```

3. **Run Flutter Doctor**
   ```bash
   flutter doctor
   ```
   This will check your setup and tell you what else needs to be installed.

### 2. Install Android Studio (for Android development)

1. **Download Android Studio**
   - Visit [Android Studio Download](https://developer.android.com/studio)
   - Download and install Android Studio

2. **Install Android SDK**
   - Open Android Studio
   - Go to Tools > SDK Manager
   - Install the latest Android SDK
   - Set up Android Virtual Device (AVD) for testing

3. **Accept Android Licenses**
   ```bash
   flutter doctor --android-licenses
   ```

### 3. Install VS Code (Recommended IDE)

1. **Download VS Code**
   - Visit [VS Code Download](https://code.visualstudio.com/)
   - Download and install VS Code

2. **Install Flutter Extension**
   - Open VS Code
   - Go to Extensions (Ctrl+Shift+X)
   - Search for "Flutter" and install the official Flutter extension
   - Also install the "Dart" extension

### 4. Install Xcode (macOS only, for iOS development)

1. **Install Xcode from App Store**
2. **Install Xcode Command Line Tools**
   ```bash
   xcode-select --install
   ```
3. **Accept Xcode Licenses**
   ```bash
   sudo xcodebuild -license accept
   ```

## Project Setup

### 1. Navigate to the Project

```bash
cd clients/pos_store
```

### 2. Install Dependencies

```bash
flutter pub get
```

### 3. Generate Code

```bash
flutter packages pub run build_runner build
```

### 4. Run the Application

```bash
flutter run
```

## Development Workflow

### 1. Check Flutter Setup

```bash
flutter doctor
```

### 2. Get Dependencies

```bash
flutter pub get
```

### 3. Generate Code (when models change)

```bash
flutter packages pub run build_runner build
```

### 4. Run Tests

```bash
flutter test
```

### 5. Build for Production

**Android APK:**
```bash
flutter build apk --release
```

**Android App Bundle:**
```bash
flutter build appbundle --release
```

**iOS:**
```bash
flutter build ios --release
```

## Common Issues and Solutions

### 1. Flutter Command Not Found

**Solution**: Add Flutter to your PATH
- **Windows**: Add `C:\flutter\bin` to system PATH
- **macOS/Linux**: Add `~/flutter/bin` to PATH

### 2. Android SDK Not Found

**Solution**: Install Android Studio and Android SDK
1. Install Android Studio
2. Open SDK Manager and install Android SDK
3. Set ANDROID_HOME environment variable

### 3. Gradle Build Issues

**Solution**: Clean and rebuild
```bash
flutter clean
flutter pub get
flutter run
```

### 4. Code Generation Issues

**Solution**: Clean and regenerate
```bash
flutter packages pub run build_runner clean
flutter packages pub run build_runner build
```

### 5. iOS Build Issues (macOS only)

**Solution**: Check Xcode setup
```bash
flutter doctor
sudo xcodebuild -license accept
```

## IDE Configuration

### VS Code Settings

Create `.vscode/settings.json` in the project root:

```json
{
  "dart.flutterSdkPath": "C:\\flutter",
  "dart.lineLength": 80,
  "editor.formatOnSave": true,
  "editor.codeActionsOnSave": {
    "source.fixAll": true,
    "source.organizeImports": true
  }
}
```

### Android Studio Settings

1. Install Flutter and Dart plugins
2. Configure Flutter SDK path
3. Set up code formatting rules

## Testing

### 1. Unit Tests

```bash
flutter test
```

### 2. Widget Tests

```bash
flutter test test/widget_test.dart
```

### 3. Integration Tests

```bash
flutter test integration_test/
```

## Debugging

### 1. Enable Debug Mode

```bash
flutter run --debug
```

### 2. Hot Reload

Press `r` in the terminal while the app is running

### 3. Hot Restart

Press `R` in the terminal while the app is running

### 4. Debug Console

Use VS Code's debug console or Flutter Inspector

## Performance

### 1. Profile Mode

```bash
flutter run --profile
```

### 2. Performance Overlay

```bash
flutter run --profile --enable-software-rendering
```

## Deployment

### Android

1. **Generate Keystore**
   ```bash
   keytool -genkey -v -keystore ~/upload-keystore.jks -keyalg RSA -keysize 2048 -validity 10000 -alias upload
   ```

2. **Configure Signing**
   Create `android/key.properties`:
   ```properties
   storePassword=<password>
   keyPassword=<password>
   keyAlias=upload
   storeFile=<path to keystore>
   ```

3. **Build Release**
   ```bash
   flutter build appbundle --release
   ```

### iOS

1. **Archive in Xcode**
   - Open `ios/Runner.xcworkspace` in Xcode
   - Select Product > Archive
   - Upload to App Store Connect

## Useful Commands

```bash
# Check Flutter version
flutter --version

# Check setup
flutter doctor

# List devices
flutter devices

# Clean project
flutter clean

# Get dependencies
flutter pub get

# Upgrade dependencies
flutter pub upgrade

# Analyze code
flutter analyze

# Format code
flutter format .

# Run on specific device
flutter run -d <device-id>

# Build for specific platform
flutter build apk --release
flutter build ios --release
flutter build web --release
```

## Next Steps

1. **Configure API Endpoints**: Update `lib/core/network/api_service.dart`
2. **Set up Database**: Configure SQLite database settings
3. **Configure Authentication**: Set up authentication service
4. **Test on Real Devices**: Test on physical Android/iOS devices
5. **Set up CI/CD**: Configure automated testing and deployment

## Support

If you encounter any issues:

1. Check the [Flutter Documentation](https://docs.flutter.dev/)
2. Search [Stack Overflow](https://stackoverflow.com/questions/tagged/flutter)
3. Check [Flutter GitHub Issues](https://github.com/flutter/flutter/issues)
4. Contact the development team

## Resources

- [Flutter Documentation](https://docs.flutter.dev/)
- [Dart Documentation](https://dart.dev/guides)
- [Flutter Cookbook](https://docs.flutter.dev/cookbook)
- [Flutter Samples](https://github.com/flutter/samples)
- [Flutter Community](https://flutter.dev/community)
