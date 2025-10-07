import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';

import 'app_colors.dart';
import 'app_sizes.dart';

class AppTheme {
  /// Make [AppTheme] to be singleton
  static final AppTheme _instance = AppTheme._();

  factory AppTheme() => _instance;

  AppTheme._();

  Color _primaryColor = AppColors.black;
  Color? _secondaryColor = Colors.grey[600];
  Color? _tertiaryColor = Colors.grey[400];
  Brightness _brightness = Brightness.light;
  TextTheme _primaryTextTheme = GoogleFonts.latoTextTheme();
  TextTheme _secondaryTextTheme = GoogleFonts.poppinsTextTheme();

  ThemeData init({
    Color? primaryColor,
    Color? secondaryColor,
    Color? tertiaryColor,
    Color? neutralColor,
    Brightness? brightness,
    TextTheme? primaryTextTheme,
    TextTheme? secondaryTextTheme,
  }) {
    _primaryColor = primaryColor ?? _primaryColor;
    _secondaryColor = secondaryColor ?? _secondaryColor;
    _tertiaryColor = tertiaryColor ?? _tertiaryColor;
    _brightness = brightness ?? _brightness;
    _primaryTextTheme = primaryTextTheme ?? _primaryTextTheme;
    _secondaryTextTheme = secondaryTextTheme ?? _secondaryTextTheme;

    return _base(
      colorScheme: _brightness == Brightness.light 
          ? ColorScheme.light(
              primary: Colors.black,
              onPrimary: Colors.white,
              secondary: Colors.grey[600]!,
              onSecondary: Colors.white,
              tertiary: Colors.grey[400]!,
              onTertiary: Colors.black,
              surface: Colors.white,
              onSurface: Colors.black,
              surfaceContainerLowest: Colors.white,
              surfaceContainerLow: Colors.grey[50]!,
              surfaceContainer: Colors.grey[100]!,
              surfaceContainerHigh: Colors.grey[200]!,
              surfaceContainerHighest: Colors.grey[300]!,
              error: Colors.red,
              onError: Colors.white,
              outline: Colors.grey[400]!,
            )
          : ColorScheme.dark(
              primary: Colors.white,
              onPrimary: Colors.black,
              secondary: Colors.grey[400]!,
              onSecondary: Colors.black,
              tertiary: Colors.grey[600]!,
              onTertiary: Colors.white,
              surface: Colors.grey[900]!,
              onSurface: Colors.white,
              surfaceContainerLowest: Colors.black,
              surfaceContainerLow: Colors.grey[800]!,
              surfaceContainer: Colors.grey[700]!,
              surfaceContainerHigh: Colors.grey[600]!,
              surfaceContainerHighest: Colors.grey[500]!,
              error: Colors.red,
              onError: Colors.white,
              outline: Colors.grey[600]!,
            ),
      brightness: _brightness,
      primaryTextTheme: _primaryTextTheme,
      secondaryTextTheme: _secondaryTextTheme,
    );
  }

  ThemeData _base({
    required ColorScheme colorScheme,
    required Brightness brightness,
    required TextTheme primaryTextTheme,
    required TextTheme secondaryTextTheme,
  }) {
    final textTheme = primaryTextTheme.copyWith(
      displaySmall: secondaryTextTheme.displaySmall,
      displayMedium: secondaryTextTheme.displayMedium,
      displayLarge: secondaryTextTheme.displayLarge,
      headlineSmall: secondaryTextTheme.headlineSmall,
      headlineMedium: secondaryTextTheme.headlineMedium,
      headlineLarge: secondaryTextTheme.headlineLarge,
    );

    return ThemeData(
      useMaterial3: true,
      colorScheme: colorScheme,
      brightness: brightness,
      visualDensity: VisualDensity.adaptivePlatformDensity,
      scaffoldBackgroundColor: colorScheme.surfaceContainerLowest,
      textTheme: textTheme.apply(
        bodyColor: colorScheme.onSurface,
        displayColor: colorScheme.onSurface,
        decorationColor: colorScheme.onSurface,
      ),
      appBarTheme: AppBarTheme(
        backgroundColor: colorScheme.surfaceContainerLowest,
        shadowColor: colorScheme.surfaceContainerHighest,
        elevation: 0.5,
        scrolledUnderElevation: 0.5,
        titleSpacing: AppSizes.padding,
        titleTextStyle: textTheme.titleLarge?.copyWith(
          fontWeight: FontWeight.bold,
          fontSize: 16,
          color: colorScheme.onSurface,
        ),
      ),
      tabBarTheme: TabBarThemeData(
        labelColor: colorScheme.onSurface,
        unselectedLabelColor: colorScheme.onSurface,
        indicator: BoxDecoration(
          border: Border(
            bottom: BorderSide(color: colorScheme.primary, width: 2),
          ),
        ),
      ),
      floatingActionButtonTheme: FloatingActionButtonThemeData(
        backgroundColor: colorScheme.secondaryContainer,
        foregroundColor: colorScheme.onSecondaryContainer,
      ),
      navigationRailTheme: NavigationRailThemeData(
        backgroundColor: colorScheme.surface,
        selectedIconTheme: IconThemeData(color: colorScheme.onSecondaryContainer),
        indicatorColor: colorScheme.secondaryContainer,
      ),
      chipTheme: ChipThemeData(
        backgroundColor: colorScheme.surface,
      ),
      bottomNavigationBarTheme: BottomNavigationBarThemeData(
        backgroundColor: colorScheme.surfaceContainerLowest,
        selectedItemColor: colorScheme.primary,
        unselectedItemColor: colorScheme.outline,
        showUnselectedLabels: true,
        type: BottomNavigationBarType.fixed,
        selectedLabelStyle: textTheme.labelSmall?.copyWith(fontWeight: FontWeight.bold, fontSize: 10),
        unselectedLabelStyle: textTheme.labelSmall?.copyWith(fontWeight: FontWeight.bold, fontSize: 10),
      ),
      dividerTheme: DividerThemeData(
        color: colorScheme.surfaceDim,
        thickness: 0.5,
      ),
      snackBarTheme: SnackBarThemeData(
        behavior: SnackBarBehavior.floating,
        backgroundColor: colorScheme.primary,
        contentTextStyle: textTheme.labelSmall?.copyWith(
          color: colorScheme.surface,
          fontWeight: FontWeight.w600,
        ),
        showCloseIcon: true,
        elevation: 1,
      ),
      dialogTheme: DialogThemeData(backgroundColor: colorScheme.surfaceContainerLowest),
    );
  }
}
