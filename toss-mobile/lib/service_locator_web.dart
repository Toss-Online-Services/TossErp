import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:flutter/foundation.dart';
import 'package:get_it/get_it.dart';
import 'package:provider/provider.dart';
import 'package:provider/single_child_widget.dart';

import 'presentation/providers/theme/theme_provider.dart';

final GetIt sl = GetIt.instance;

// Web-specific Service Locator (minimal dependencies)
void setupServiceLocatorWeb() {
  try {
    // Only register what we need for web
    sl.registerSingleton<FirebaseFirestore>(FirebaseFirestore.instance);
    sl.registerLazySingleton(() => ThemeProvider());
    
    debugPrint('Web service locator setup complete');
  } catch (e) {
    debugPrint('Web service locator setup error: $e');
  }
}

// Web-compatible providers (minimal)
final List<SingleChildWidget> webProviders = [
  ChangeNotifierProvider(create: (_) => sl<ThemeProvider>()),
];
