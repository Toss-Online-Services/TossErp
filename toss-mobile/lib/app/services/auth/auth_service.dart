import 'package:firebase_auth/firebase_auth.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'package:google_sign_in/google_sign_in.dart';

import '../../../core/auth/auth_base.dart';
import '../../../core/errors/errors.dart';
import '../../../core/usecase/usecase.dart';
import '../../../firebase_options.dart';

class AuthService implements AuthBase {
  AuthService({
    FirebaseAuth? firebaseAuth,
    GoogleSignIn? googleSignIn,
  }) : _firebaseAuth = firebaseAuth ?? FirebaseAuth.instance,
       _googleSignIn = googleSignIn ?? GoogleSignIn.instance {
    // Initialize Google Sign-In with proper client configuration
    _initializeGoogleSignIn();
  }

  void _initializeGoogleSignIn() {
    // On Android, serverClientId is REQUIRED and must be the Web OAuth client ID
    // Create a .env file with GOOGLE_SERVER_CLIENT_ID=your_web_oauth_client_id
    final String? serverClientId = dotenv.env['GOOGLE_SERVER_CLIENT_ID'];

    if (kIsWeb) {
      // Web does not require initialization here
      return;
    }

    // iOS optionally uses clientId; Android ignores clientId and requires serverClientId
    final String? iosClientId = DefaultFirebaseOptions.ios.iosClientId;

    GoogleSignIn.instance.initialize(
      clientId: defaultTargetPlatform == TargetPlatform.iOS ? iosClientId : null,
      serverClientId: serverClientId,
    );
  }

  final FirebaseAuth _firebaseAuth;
  final GoogleSignIn _googleSignIn;

  final List<String> authScopes = [
    'https://www.googleapis.com/auth/userinfo.profile',
    'https://www.googleapis.com/auth/userinfo.email',
  ];

  @override
  Future<bool> isAuthenticated() async {
    try {
      final currentUser = _firebaseAuth.currentUser;
      if (kIsWeb) {
        // For web, we might need to wait a bit for the auth state to load
        if (currentUser == null) {
          // Wait a moment for potential auth state restoration
          await Future.delayed(const Duration(milliseconds: 500));
          return _firebaseAuth.currentUser != null;
        }
      }
      return currentUser != null;
    } catch (e) {
      debugPrint('Auth check error: $e');
      // For development/demo purposes, allow access without authentication
      if (kDebugMode || kIsWeb) {
        return true;
      }
      return false;
    }
  }

  @override
  User? getAuthData() {
    return _firebaseAuth.currentUser;
  }

  @override
  Future<Result<UserCredential>> signIn() async {
    try {
      // First, authenticate the user
      final googleSignInAccount = await _googleSignIn.authenticate();
      if (googleSignInAccount == null) {
        return Result.error(ServiceError(message: 'Sign in cancelled by user.'));
      }

      // Get the ID token for Firebase authentication
      final googleSignInAuthentication = await googleSignInAccount.authentication;

      // Get the access token through authorization (for scopes if needed)
      final googleSignInAuthorization = await googleSignInAccount.authorizationClient.authorizationForScopes(
        authScopes,
      );

      final credential = GoogleAuthProvider.credential(
        accessToken: googleSignInAuthorization?.accessToken,
        idToken: googleSignInAuthentication.idToken,
      );

      final userCredential = await _firebaseAuth.signInWithCredential(credential);
      return Result.success(userCredential);
    } on GoogleSignInException catch (e) {
      if (e.code == GoogleSignInExceptionCode.canceled) {
        debugPrint('Google Sign-In cancelled by user.');
        return Result.error(ServiceError(message: 'Google Sign-In was cancelled by the user.'));
      }
      debugPrint('Google Sign-In error: ${e.toString()}');
      return Result.error(ServiceError(message: e.toString()));
    } catch (e) {
      debugPrint('Sign in error: $e');
      return Result.error(ServiceError(message: e.toString()));
    }
  }

  @override
  Future<bool> signOut() async {
    try {
      await _firebaseAuth.signOut();
      await _googleSignIn.signOut();
      return true;
    } catch (e) {
      return false;
    }
  }
}
