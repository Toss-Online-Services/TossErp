@echo off
cd /d "C:\Users\PROBOOK\source\repos\Toss-Online-Services\TossErp\toss-mobile"
echo Waiting for Android emulator...
flutter devices
echo.
echo If you see an Android emulator above, press any key to run the app on it.
pause
flutter run
pause
