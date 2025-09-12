import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:firebase_core/firebase_core.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'package:get_it/get_it.dart';
import 'package:intl/date_symbol_data_local.dart';
import 'package:provider/provider.dart';

import 'app/database/app_database.dart';
import 'app/locale/app_locale.dart';
import 'app/routes/app_routes.dart';
import 'app/themes/theme_manager.dart';
import 'simple_dashboard_manager.dart';
import 'firebase_options.dart';
import 'presentation/providers/theme/theme_provider.dart';
import 'presentation/screens/error_handler_screen.dart';
import 'service_locator.dart';
import 'data/datasources/local/user_local_datasource_impl.dart';
import 'data/datasources/local/product_local_datasource_impl.dart';
import 'data/datasources/local/transaction_local_datasource_impl.dart';
import 'data/datasources/local/queued_action_local_datasource_impl.dart';

final GetIt sl = GetIt.instance;

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
    } else {
      // For web, setup minimal service locator
      sl.registerLazySingleton(() => ThemeProvider());
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

  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    // For web, use simplified provider setup
    if (kIsWeb) {
      return ChangeNotifierProvider(
        create: (_) => ThemeProvider(),
        child: Consumer<ThemeProvider>(
          builder: (context, themeProvider, _) {
            return MaterialApp.router(
              title: 'TOSS ERP - POS System',
              theme: themeProvider.theme,
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
    
    // For mobile/desktop, use full provider setup
    return MultiProvider(
      providers: [
        ...providers,
        ChangeNotifierProvider<ThemeManager>(
          create: (context) {
            final themeManager = ThemeManager();
            themeManager.initialize();
            return themeManager;
          },
        ),
        ChangeNotifierProvider<SimpleDashboardManager>(
          create: (context) {
            return SimpleDashboardManager();
          },
        ),
      ],
      child: Consumer3<ThemeProvider, ThemeManager, SimpleDashboardManager>(
        builder: (context, themeProvider, themeManager, dashboardManager, _) {
          // Use ThemeManager if available, otherwise fall back to ThemeProvider
          final theme = themeManager.currentThemeData;
          
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
