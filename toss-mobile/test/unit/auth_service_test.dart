import 'package:flutter_test/flutter_test.dart';
import 'package:mockito/mockito.dart';
import 'package:mockito/annotations.dart';
import 'package:firebase_auth/firebase_auth.dart';
import 'package:google_sign_in/google_sign_in.dart';
import 'package:google_sign_in_platform_interface/google_sign_in_platform_interface.dart';

import '../../lib/app/services/auth/auth_service.dart';

import 'auth_service_test.mocks.dart';

@GenerateMocks([
  FirebaseAuth,
  GoogleSignIn,
  User,
  UserCredential,
])
void main() {
  late AuthService authService;
  late MockFirebaseAuth mockFirebaseAuth;
  late MockGoogleSignIn mockGoogleSignIn;
  late MockUser mockUser;

  setUp(() {
    mockFirebaseAuth = MockFirebaseAuth();
    mockGoogleSignIn = MockGoogleSignIn();
    mockUser = MockUser();
    
    authService = AuthService(
      firebaseAuth: mockFirebaseAuth,
      googleSignIn: mockGoogleSignIn,
    );
  });

  group('AuthService Tests', () {
    group('Authentication Status', () {
      test('should return true when user is authenticated', () async {
        // Arrange
        when(mockFirebaseAuth.currentUser).thenReturn(mockUser);

        // Act
        final result = await authService.isAuthenticated();

        // Assert
        expect(result, isTrue);
      });

      test('should return false when user is not authenticated', () async {
        // Arrange
        when(mockFirebaseAuth.currentUser).thenReturn(null);

        // Act
        final result = await authService.isAuthenticated();

        // Assert
        expect(result, isFalse);
      });
    });

    group('Get Auth Data', () {
      test('should return current user when authenticated', () {
        // Arrange
        when(mockFirebaseAuth.currentUser).thenReturn(mockUser);

        // Act
        final user = authService.getAuthData();

        // Assert
        expect(user, equals(mockUser));
      });

      test('should return null when not authenticated', () {
        // Arrange
        when(mockFirebaseAuth.currentUser).thenReturn(null);

        // Act
        final user = authService.getAuthData();

        // Assert
        expect(user, isNull);
      });
    });

    group('Google Sign-In Error Handling', () {
      test('should handle Google sign in cancellation', () async {
        // Arrange
        when(mockGoogleSignIn.authenticate()).thenThrow(
            const GoogleSignInException(
                code: GoogleSignInExceptionCode.canceled,
                description: 'User cancelled the sign-in flow'));

        // Act
        final result = await authService.signIn();

        // Assert
        expect(result.isSuccess, isFalse);
        expect(result.error?.message?.contains('cancelled') ?? false, isTrue);
      });

      test('should handle Google sign in other errors', () async {
        // Arrange
        when(mockGoogleSignIn.authenticate()).thenThrow(
            const GoogleSignInException(
                code: GoogleSignInExceptionCode.unknownError,
                description: 'Unknown error occurred'));

        // Act
        final result = await authService.signIn();

        // Assert
        expect(result.isSuccess, isFalse);
        expect(result.error, isNotNull);
      });

      test('should handle general exceptions during sign in', () async {
        // Arrange
        when(mockGoogleSignIn.authenticate())
            .thenThrow(Exception('Network error'));

        // Act
        final result = await authService.signIn();

        // Assert
        expect(result.isSuccess, isFalse);
        expect(result.error, isNotNull);
      });
    });

    group('Sign Out', () {
      test('should sign out successfully', () async {
        // Arrange
        when(mockFirebaseAuth.signOut()).thenAnswer((_) async {});
        when(mockGoogleSignIn.signOut()).thenAnswer((_) async => null);

        // Act
        final result = await authService.signOut();

        // Assert
        expect(result, isTrue);
        verify(mockFirebaseAuth.signOut()).called(1);
        verify(mockGoogleSignIn.signOut()).called(1);
      });

      test('should handle sign out errors gracefully', () async {
        // Arrange
        when(mockFirebaseAuth.signOut())
            .thenThrow(Exception('Firebase sign out failed'));
        when(mockGoogleSignIn.signOut()).thenAnswer((_) async => null);

        // Act
        final result = await authService.signOut();

        // Assert
        expect(result, isFalse);
      });
    });
  });

  // Note: Manager Pin Validator tests skipped due to dotenv initialization requirements
  // These would need proper environment setup in a real application
}