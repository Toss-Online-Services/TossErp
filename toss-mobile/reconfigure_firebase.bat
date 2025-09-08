@echo off
echo Reconfiguring Firebase with FlutterFire CLI...
echo Make sure you've completed the Firebase Console setup first!
echo.
pause

echo Setting up Firebase for Flutter...
C:\Users\PROBOOK\AppData\Local\Pub\Cache\bin\flutterfire configure --project=toss-77ad7

echo.
echo Installing dependencies...
flutter pub get

echo.
echo Deploying Firebase Storage rules...
firebase deploy --only storage

echo.
echo Configuration complete!
echo You can now test Google Sign-In functionality.
pause
