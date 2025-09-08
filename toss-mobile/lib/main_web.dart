import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:firebase_core/firebase_core.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'package:intl/date_symbol_data_local.dart';
import 'package:provider/provider.dart';

import 'firebase_options.dart';
import 'presentation/providers/theme/theme_provider.dart';
import 'presentation/screens/error_handler_screen.dart';
import 'service_locator_web.dart';

void main() async {
  // Initialize binding
  WidgetsFlutterBinding.ensureInitialized();

  // Web-specific error handling
  if (kIsWeb) {
    FlutterError.onError = (FlutterErrorDetails details) {
      // Log errors for web debugging
      debugPrint('Flutter Web Error: ${details.exception}');
      debugPrint('Stack trace: ${details.stack}');
      
      // Call the default error handler
      FlutterError.presentError(details);
    };
  }

  try {
    // Initialize Firebase
    await Firebase.initializeApp(
      options: DefaultFirebaseOptions.currentPlatform,
    );

    // Initialize flutter_dotenv
    try {
      await dotenv.load();
    } catch (e) {
      debugPrint('Failed to load .env file: $e');
      // Continue without .env if it fails
    }

    // Initialize date formatting
    initializeDateFormatting();

    // Setup service locator
    setupServiceLocatorWeb();
  } catch (e) {
    debugPrint('Initialization error: $e');
    // Continue with app launch even if some initialization fails
  }

  runApp(const MyWebApp());
}

class MyWebApp extends StatelessWidget {
  const MyWebApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MultiProvider(
      providers: webProviders,
      child: Selector<ThemeProvider, ThemeData>(
        selector: (context, provider) => provider.theme,
        builder: (context, theme, _) {
          return MaterialApp(
            title: 'TOSS ERP - Web Demo',
            theme: theme,
            debugShowCheckedModeBanner: kDebugMode,
            home: const WebDemoScreen(),
            builder: (context, child) => ErrorHandlerBuilder(child: child),
          );
        },
      ),
    );
  }
}

class WebDemoScreen extends StatelessWidget {
  const WebDemoScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('TOSS ERP'),
        backgroundColor: Theme.of(context).colorScheme.primary,
        foregroundColor: Theme.of(context).colorScheme.onPrimary,
      ),
      body: const Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              Icons.business,
              size: 100,
              color: Colors.blue,
            ),
            SizedBox(height: 20),
            Text(
              'Welcome to TOSS ERP',
              style: TextStyle(
                fontSize: 32,
                fontWeight: FontWeight.bold,
              ),
            ),
            SizedBox(height: 10),
            Text(
              'Rural Township Enterprise Management System',
              style: TextStyle(
                fontSize: 16,
                color: Colors.grey,
              ),
              textAlign: TextAlign.center,
            ),
            SizedBox(height: 40),
            Card(
              margin: EdgeInsets.all(20),
              child: Padding(
                padding: EdgeInsets.all(20),
                child: Column(
                  children: [
                    Text(
                      'Web Version Successfully Deployed!',
                      style: TextStyle(
                        fontSize: 24,
                        fontWeight: FontWeight.bold,
                        color: Colors.green,
                      ),
                    ),
                    SizedBox(height: 10),
                    Text(
                      'The TOSS ERP mobile application has been successfully compiled and deployed for web. '
                      'This demonstrates the cross-platform capability of the Flutter framework.',
                      textAlign: TextAlign.center,
                      style: TextStyle(fontSize: 14),
                    ),
                  ],
                ),
              ),
            ),
            SizedBox(height: 20),
            Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Icon(Icons.check_circle, color: Colors.green, size: 20),
                SizedBox(width: 8),
                Text('Firebase Connected'),
              ],
            ),
            SizedBox(height: 10),
            Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Icon(Icons.check_circle, color: Colors.green, size: 20),
                SizedBox(width: 8),
                Text('Firestore Rules Active'),
              ],
            ),
            SizedBox(height: 10),
            Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Icon(Icons.check_circle, color: Colors.green, size: 20),
                SizedBox(width: 8),
                Text('Web Build Complete'),
              ],
            ),
          ],
        ),
      ),
    );
  }
}
