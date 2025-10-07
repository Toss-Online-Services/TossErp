# Firebase Setup Steps for Google Sign-In

## Firebase Console Steps (Do these first):

### 1. Enable Authentication
- Go to: https://console.firebase.google.com/project/toss-77ad7/authentication
- Click "Get started" if not set up
- Go to "Sign-in method" tab
- Click "Google" and enable it
- Save the configuration

### 2. Add SHA-1 Fingerprint
- Go to: https://console.firebase.google.com/project/toss-77ad7/settings/general/
- Scroll to "Your apps" section
- Find your Android app: `co.za.tossonlineservices.toss_store`
- Click "Add fingerprint"
- Add this SHA-1: `4C:A9:98:63:C7:AE:8C:53:E2:20:BE:B6:4B:58:34:42:3C:0D:9B:0E`
- Save

### 3. Set up Firebase Storage
- Go to: https://console.firebase.google.com/project/toss-77ad7/storage
- Click "Get started"
- Choose "Start in production mode" or "Start in test mode"
- Choose a location (preferably us-central1)

### 4. Download Updated google-services.json
- After completing steps 1-3, go back to: https://console.firebase.google.com/project/toss-77ad7/settings/general/
- Find your Android app
- Click the download button to get the updated `google-services.json`
- Replace the current file in `android/app/google-services.json`

## Generated Information
- Project ID: toss-77ad7
- Package Name: co.za.tossonlineservices.toss_store
- SHA-1 Debug: 4C:A9:98:63:C7:AE:8C:53:E2:20:BE:B6:4B:58:34:42:3C:0D:9B:0E

## What this fixes:
- GoogleSignInException: serverClientId must not be empty
- OAuth client configuration missing
- Firebase Storage rules deployment
- Proper authentication flow
