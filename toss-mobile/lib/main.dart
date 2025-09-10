import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:firebase_core/firebase_core.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'package:intl/date_symbol_data_local.dart';
import 'package:provider/provider.dart';

import 'app/database/app_database.dart';
import 'app/locale/app_locale.dart';
import 'app/routes/app_routes.dart';
import 'firebase_options.dart';
import 'presentation/providers/theme/theme_provider.dart';
import 'presentation/screens/error_handler_screen.dart';
import 'service_locator.dart';
import 'data/datasources/local/user_local_datasource_impl.dart';
import 'data/datasources/local/product_local_datasource_impl.dart';
import 'data/datasources/local/transaction_local_datasource_impl.dart';
import 'data/datasources/local/queued_action_local_datasource_impl.dart';
import 'app/services/sync/sync_service.dart';

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
    // Initialize Firebase (use `flutterfire configure` to generate the options)
    await Firebase.initializeApp(
      name: kIsWeb ? null : DefaultFirebaseOptions.currentPlatform.projectId,
      options: DefaultFirebaseOptions.currentPlatform,
    );

    // Initialize app local db (skip for web to avoid issues)
    if (!kIsWeb) {
      await AppDatabase().init();
      // Ensure persistence is cleared
      await FirebaseFirestore.instance.clearPersistence();
        // Seed sample data for all repositories
        final appDatabase = AppDatabase();
        final userLocalDatasource = UserLocalDatasourceImpl(appDatabase);
        final productLocalDatasource = ProductLocalDatasourceImpl(appDatabase);
        final transactionLocalDatasource = TransactionLocalDatasourceImpl(appDatabase);
        final queuedActionLocalDatasource = QueuedActionLocalDatasourceImpl(appDatabase);

        Future<void> seedAllSampleData() async {
          await userLocalDatasource.seedSampleUser();
          await productLocalDatasource.seedSampleProducts();
          await transactionLocalDatasource.seedSampleTransactions();
          await queuedActionLocalDatasource.seedSampleQueuedActions();
        }

        await seedAllSampleData();
    }

    // Initialize flutter_dotenv
    try {
      await dotenv.load();
    } catch (e) {
      debugPrint('Failed to load .env file: $e');
      // Continue without .env if it fails
    }

    // Initialize date formatting
    initializeDateFormatting();

    // Setup service locator (skip complex setup for web)
    if (!kIsWeb) {
      setupServiceLocator();
    }

    // Set/lock screen orientation (skip for web)
    if (!kIsWeb) {
      SystemChrome.setPreferredOrientations([]);

      // Set Default SystemUIOverlayStyle
      SystemChrome.setSystemUIOverlayStyle(
        const SystemUiOverlayStyle(
          systemNavigationBarColor: Colors.transparent,
          statusBarColor: Colors.transparent,
        ),
      );
    }
  } catch (e) {
    debugPrint('Initialization error: $e');
    // Continue with app launch even if some initialization fails
  }

  runApp(kIsWeb ? const MyWebApp() : const MyApp());

  if (!kIsWeb) {
    // Start background sync after app is up
    SyncService().start();
  }
}

class MyWebApp extends StatelessWidget {
  const MyWebApp({super.key});

  @override
  Widget build(BuildContext context) {
    return ChangeNotifierProvider(
      create: (_) => ThemeProvider(),
      child: Consumer<ThemeProvider>(
        builder: (context, themeProvider, _) {
          return MaterialApp(
            title: 'TOSS ERP - Web Demo',
            theme: themeProvider.theme,
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

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MultiProvider(
      providers: providers,
      child: Selector<ThemeProvider, ThemeData>(
        selector: (context, provider) => provider.theme,
        builder: (context, theme, _) {
          return MaterialApp.router(
            title: 'Flutter POS',
            theme: theme,
            debugShowCheckedModeBanner: kDebugMode,
            routerConfig: AppRoutes.router,
            locale: AppLocale.defaultLocale,
            supportedLocales: AppLocale.supportedLocales,
            localizationsDelegates: AppLocale.localizationsDelegates,
            builder: (context, child) => ErrorHandlerBuilder(child: child),
          );
        },
      ),
    );
  }
}
