import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:google_fonts/google_fonts.dart';

import 'theme_config.dart';

/// Service for managing custom themes and persistence
class ThemeManager extends ChangeNotifier {
  static const String _themeConfigKey = 'theme_config';
  static const String _customThemesKey = 'custom_themes';
  static const String _selectedThemeKey = 'selected_theme_id';

  ThemeConfig _currentTheme = ThemePresets.defaultTheme;
  List<ThemeConfig> _customThemes = [];
  SharedPreferences? _prefs;

  ThemeConfig get currentTheme => _currentTheme;
  List<ThemeConfig> get customThemes => _customThemes;
  List<ThemeConfig> get allThemes => [...ThemePresets.presets, ..._customThemes];

  /// Initialize the theme manager
  Future<void> initialize() async {
    _prefs = await SharedPreferences.getInstance();
    await _loadCurrentTheme();
    await _loadCustomThemes();
  }

  /// Load the currently selected theme
  Future<void> _loadCurrentTheme() async {
    try {
      final selectedThemeId = _prefs?.getString(_selectedThemeKey);
      if (selectedThemeId != null) {
        // Try to find in presets first
        final presetTheme = ThemePresets.getById(selectedThemeId);
        if (presetTheme != null) {
          _currentTheme = presetTheme;
          return;
        }

        // Then try custom themes
        await _loadCustomThemes();
        final customTheme = _customThemes.where((t) => t.id == selectedThemeId).firstOrNull;
        if (customTheme != null) {
          _currentTheme = customTheme;
          return;
        }
      }

      // Fallback to saved theme config
      final themeConfigJson = _prefs?.getString(_themeConfigKey);
      if (themeConfigJson != null) {
        final themeData = json.decode(themeConfigJson);
        _currentTheme = ThemeConfig.fromJson(themeData);
      }
    } catch (e) {
      debugPrint('Error loading theme: $e');
      _currentTheme = ThemePresets.defaultTheme;
    }
  }

  /// Load custom themes from storage
  Future<void> _loadCustomThemes() async {
    try {
      final customThemesJson = _prefs?.getString(_customThemesKey);
      if (customThemesJson != null) {
        final List<dynamic> themesData = json.decode(customThemesJson);
        _customThemes = themesData
            .map((data) => ThemeConfig.fromJson(data))
            .toList();
      }
    } catch (e) {
      debugPrint('Error loading custom themes: $e');
      _customThemes = [];
    }
  }

  /// Save the current theme
  Future<void> _saveCurrentTheme() async {
    try {
      await _prefs?.setString(_themeConfigKey, json.encode(_currentTheme.toJson()));
      await _prefs?.setString(_selectedThemeKey, _currentTheme.id);
    } catch (e) {
      debugPrint('Error saving theme: $e');
    }
  }

  /// Save custom themes
  Future<void> _saveCustomThemes() async {
    try {
      final themesJson = json.encode(_customThemes.map((t) => t.toJson()).toList());
      await _prefs?.setString(_customThemesKey, themesJson);
    } catch (e) {
      debugPrint('Error saving custom themes: $e');
    }
  }

  /// Apply a theme configuration
  Future<void> applyTheme(ThemeConfig themeConfig) async {
    _currentTheme = themeConfig;
    await _saveCurrentTheme();
    notifyListeners();
  }

  /// Create and save a custom theme
  Future<void> createCustomTheme(ThemeConfig themeConfig) async {
    final customTheme = themeConfig.copyWith(
      id: 'custom_${DateTime.now().millisecondsSinceEpoch}',
      isCustom: true,
    );
    
    _customThemes.add(customTheme);
    await _saveCustomThemes();
    
    // Apply the new custom theme
    await applyTheme(customTheme);
  }

  /// Update an existing custom theme
  Future<void> updateCustomTheme(ThemeConfig updatedTheme) async {
    if (!updatedTheme.isCustom) return;
    
    final index = _customThemes.indexWhere((t) => t.id == updatedTheme.id);
    if (index != -1) {
      _customThemes[index] = updatedTheme;
      await _saveCustomThemes();
      
      // If this is the current theme, update it
      if (_currentTheme.id == updatedTheme.id) {
        _currentTheme = updatedTheme;
        await _saveCurrentTheme();
        notifyListeners();
      }
    }
  }

  /// Delete a custom theme
  Future<void> deleteCustomTheme(String themeId) async {
    _customThemes.removeWhere((t) => t.id == themeId);
    await _saveCustomThemes();
    
    // If this was the current theme, switch to default
    if (_currentTheme.id == themeId) {
      await applyTheme(ThemePresets.defaultTheme);
    }
  }

  /// Toggle dark mode for current theme
  Future<void> toggleDarkMode() async {
    final updatedTheme = _currentTheme.copyWith(
      isDarkMode: !_currentTheme.isDarkMode,
      brightness: _currentTheme.isDarkMode ? Brightness.light : Brightness.dark,
    );
    await applyTheme(updatedTheme);
  }

  /// Generate Flutter ThemeData from theme config
  ThemeData generateThemeData(ThemeConfig config) {
    // Get Google Font based on font family
    TextTheme getTextTheme(String fontFamily) {
      switch (fontFamily.toLowerCase()) {
        case 'poppins':
          return GoogleFonts.poppinsTextTheme();
        case 'roboto':
          return GoogleFonts.robotoTextTheme();
        case 'opensans':
          return GoogleFonts.openSansTextTheme();
        case 'lato':
        default:
          return GoogleFonts.latoTextTheme();
      }
    }

    final textTheme = getTextTheme(config.fontFamily);
    
    // Special handling for black and white theme
    if (config.id == 'black_white') {
      return ThemeData(
        useMaterial3: true,
        colorScheme: config.isDarkMode 
            ? ColorScheme.dark(
                primary: Colors.white,
                onPrimary: Colors.black,
                primaryContainer: Colors.grey[700]!,
                onPrimaryContainer: Colors.white,
                secondary: Colors.grey[300]!,
                onSecondary: Colors.black,
                secondaryContainer: Colors.grey[600]!,
                onSecondaryContainer: Colors.white,
                tertiary: Colors.grey[500]!,
                onTertiary: Colors.white,
                tertiaryContainer: Colors.grey[700]!,
                onTertiaryContainer: Colors.white,
                surface: Colors.grey[850]!,
                onSurface: Colors.white,
                surfaceContainerLowest: Colors.grey[900]!,
                surfaceContainerLow: Colors.grey[800]!,
                surfaceContainer: Colors.grey[750]!,
                surfaceContainerHigh: Colors.grey[700]!,
                surfaceContainerHighest: Colors.grey[650]!,
                error: Colors.redAccent[400]!,
                onError: Colors.black,
                errorContainer: Colors.red[900]!,
                onErrorContainer: Colors.red[100]!,
                outline: Colors.grey[500]!,
                outlineVariant: Colors.grey[700]!,
                inverseSurface: Colors.grey[300]!,
                onInverseSurface: Colors.black,
                inversePrimary: Colors.black,
                shadow: Colors.black,
                scrim: Colors.black,
                surfaceTint: Colors.white,
              )
            : ColorScheme.light(
                primary: Colors.black,
                onPrimary: Colors.white,
                primaryContainer: Colors.grey[200]!,
                onPrimaryContainer: Colors.black,
                secondary: Colors.grey[700]!,
                onSecondary: Colors.white,
                secondaryContainer: Colors.grey[100]!,
                onSecondaryContainer: Colors.black,
                tertiary: Colors.grey[600]!,
                onTertiary: Colors.white,
                tertiaryContainer: Colors.grey[50]!,
                onTertiaryContainer: Colors.black,
                surface: Colors.white,
                onSurface: Colors.black,
                surfaceContainerLowest: Colors.white,
                surfaceContainerLow: Colors.grey[50]!,
                surfaceContainer: Colors.grey[100]!,
                surfaceContainerHigh: Colors.grey[200]!,
                surfaceContainerHighest: Colors.grey[300]!,
                error: Colors.red[600]!,
                onError: Colors.white,
                errorContainer: Colors.red[50]!,
                onErrorContainer: Colors.red[900]!,
                outline: Colors.grey[400]!,
                outlineVariant: Colors.grey[300]!,
                inverseSurface: Colors.grey[800]!,
                onInverseSurface: Colors.white,
                inversePrimary: Colors.white,
                shadow: Colors.black,
                scrim: Colors.black,
                surfaceTint: Colors.black,
              ),
        brightness: config.brightness,
        visualDensity: VisualDensity.adaptivePlatformDensity,
        scaffoldBackgroundColor: config.isDarkMode ? Colors.grey[850] : Colors.white,
        textTheme: textTheme.apply(
          bodyColor: config.isDarkMode ? Colors.white : Colors.black,
          displayColor: config.isDarkMode ? Colors.white : Colors.black,
          decorationColor: config.isDarkMode ? Colors.white : Colors.black,
        ),
        appBarTheme: AppBarTheme(
          backgroundColor: config.isDarkMode ? Colors.grey[850] : Colors.white,
          foregroundColor: config.isDarkMode ? Colors.white : Colors.black,
          elevation: 0.5,
          centerTitle: true,
          shadowColor: config.isDarkMode ? Colors.black54 : Colors.grey[300],
          titleTextStyle: textTheme.titleLarge?.copyWith(
            fontWeight: FontWeight.bold,
            fontSize: 18,
            color: config.isDarkMode ? Colors.white : Colors.black,
          ),
        ),
        bottomNavigationBarTheme: BottomNavigationBarThemeData(
          backgroundColor: config.isDarkMode ? Colors.grey[850] : Colors.white,
          selectedItemColor: config.isDarkMode ? Colors.white : Colors.black,
          unselectedItemColor: config.isDarkMode ? Colors.grey[400] : Colors.grey[600],
          type: BottomNavigationBarType.fixed,
          elevation: 8,
        ),
        cardTheme: CardThemeData(
          elevation: config.isDarkMode ? 4 : 2,
          color: config.isDarkMode ? Colors.grey[800] : Colors.white,
          shadowColor: config.isDarkMode ? Colors.black87 : Colors.black.withOpacity(0.1),
          surfaceTintColor: config.isDarkMode ? Colors.grey[700] : Colors.white,
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(config.borderRadius),
            side: config.isDarkMode 
                ? BorderSide(color: Colors.grey[700]!, width: 0.5)
                : BorderSide.none,
          ),
        ),
        elevatedButtonTheme: ElevatedButtonThemeData(
          style: ElevatedButton.styleFrom(
            backgroundColor: config.isDarkMode ? Colors.white : Colors.black,
            foregroundColor: config.isDarkMode ? Colors.black : Colors.white,
            elevation: 0,
            padding: EdgeInsets.all(config.spacing * 0.8),
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(config.borderRadius),
            ),
            textStyle: textTheme.labelLarge?.copyWith(
              fontWeight: FontWeight.w600,
            ),
          ),
        ),
        outlinedButtonTheme: OutlinedButtonThemeData(
          style: OutlinedButton.styleFrom(
            foregroundColor: config.isDarkMode ? Colors.white : Colors.black,
            side: BorderSide(color: config.isDarkMode ? Colors.white : Colors.black),
            padding: EdgeInsets.all(config.spacing * 0.8),
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(config.borderRadius),
            ),
          ),
        ),
        inputDecorationTheme: InputDecorationTheme(
          filled: true,
          fillColor: config.isDarkMode ? Colors.grey[800] : Colors.grey[50],
          border: OutlineInputBorder(
            borderRadius: BorderRadius.circular(config.borderRadius),
            borderSide: BorderSide.none,
          ),
          enabledBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(config.borderRadius),
            borderSide: BorderSide(
              color: config.isDarkMode 
                  ? Colors.grey[600]!.withOpacity(0.5) 
                  : Colors.grey.withOpacity(0.3),
            ),
          ),
          focusedBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(config.borderRadius),
            borderSide: BorderSide(color: config.isDarkMode ? Colors.white : Colors.black, width: 2),
          ),
          contentPadding: EdgeInsets.all(config.spacing),
          labelStyle: TextStyle(
            color: config.isDarkMode ? Colors.grey[300] : Colors.grey[700],
          ),
          hintStyle: TextStyle(
            color: config.isDarkMode ? Colors.grey[400] : Colors.grey[600],
          ),
        ),
        floatingActionButtonTheme: FloatingActionButtonThemeData(
          backgroundColor: config.isDarkMode ? Colors.white : Colors.black,
          foregroundColor: config.isDarkMode ? Colors.black : Colors.white,
          elevation: 4,
        ),
        dividerTheme: DividerThemeData(
          color: config.isDarkMode ? Colors.grey[600] : Colors.grey[300],
          thickness: 0.5,
        ),
        dialogTheme: DialogThemeData(
          backgroundColor: config.isDarkMode ? Colors.grey[800] : Colors.white,
          titleTextStyle: textTheme.titleLarge?.copyWith(
            color: config.isDarkMode ? Colors.white : Colors.black,
          ),
          contentTextStyle: textTheme.bodyMedium?.copyWith(
            color: config.isDarkMode ? Colors.white : Colors.black,
          ),
        ),
        snackBarTheme: SnackBarThemeData(
          backgroundColor: config.isDarkMode ? Colors.grey[700] : Colors.grey[800],
          contentTextStyle: TextStyle(
            color: config.isDarkMode ? Colors.white : Colors.white,
          ),
        ),
      );
    }
    
    // Original theme generation for other themes
    return ThemeData(
      useMaterial3: true,
      colorScheme: ColorScheme.fromSeed(
        seedColor: config.primaryColor,
        brightness: config.brightness,
        primary: config.primaryColor,
        secondary: config.secondaryColor,
        tertiary: config.accentColor,
      ),
      brightness: config.brightness,
      visualDensity: VisualDensity.adaptivePlatformDensity,
      textTheme: textTheme.apply(
        bodyColor: config.isDarkMode ? Colors.white : Colors.black87,
        displayColor: config.isDarkMode ? Colors.white : Colors.black87,
      ),
      appBarTheme: AppBarTheme(
        backgroundColor: config.isDarkMode 
            ? config.primaryColor.withOpacity(0.1) 
            : config.primaryColor.withOpacity(0.05),
        foregroundColor: config.isDarkMode ? Colors.white : Colors.black87,
        elevation: 0,
        centerTitle: true,
        titleTextStyle: textTheme.titleLarge?.copyWith(
          fontWeight: FontWeight.bold,
          fontSize: 18,
          color: config.isDarkMode ? Colors.white : Colors.black87,
        ),
      ),
      cardTheme: CardThemeData(
        elevation: 2,
        shadowColor: Colors.black.withOpacity(0.1),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(config.borderRadius),
        ),
      ),
      elevatedButtonTheme: ElevatedButtonThemeData(
        style: ElevatedButton.styleFrom(
          backgroundColor: config.primaryColor,
          foregroundColor: Colors.white,
          elevation: 0,
          padding: EdgeInsets.all(config.spacing * 0.8),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(config.borderRadius),
          ),
          textStyle: textTheme.labelLarge?.copyWith(
            fontWeight: FontWeight.w600,
          ),
        ),
      ),
      outlinedButtonTheme: OutlinedButtonThemeData(
        style: OutlinedButton.styleFrom(
          foregroundColor: config.primaryColor,
          side: BorderSide(color: config.primaryColor),
          padding: EdgeInsets.all(config.spacing * 0.8),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(config.borderRadius),
          ),
        ),
      ),
      inputDecorationTheme: InputDecorationTheme(
        filled: true,
        fillColor: config.isDarkMode 
            ? Colors.grey[800] 
            : Colors.grey[50],
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(config.borderRadius),
          borderSide: BorderSide.none,
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(config.borderRadius),
          borderSide: BorderSide(
            color: Colors.grey.withOpacity(0.3),
          ),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(config.borderRadius),
          borderSide: BorderSide(color: config.primaryColor, width: 2),
        ),
        contentPadding: EdgeInsets.all(config.spacing),
      ),
      bottomNavigationBarTheme: BottomNavigationBarThemeData(
        backgroundColor: config.isDarkMode 
            ? Colors.grey[900] 
            : Colors.white,
        selectedItemColor: config.primaryColor,
        unselectedItemColor: Colors.grey,
        type: BottomNavigationBarType.fixed,
        elevation: 8,
      ),
      floatingActionButtonTheme: FloatingActionButtonThemeData(
        backgroundColor: config.accentColor,
        foregroundColor: Colors.white,
        elevation: 4,
      ),
      dividerTheme: DividerThemeData(
        color: config.isDarkMode ? Colors.grey[700] : Colors.grey[300],
        thickness: 0.5,
      ),
    );
  }

  /// Get current theme data
  ThemeData get currentThemeData => generateThemeData(_currentTheme);

  /// Check if current theme is dark
  bool get isDarkMode => _currentTheme.isDarkMode;

  /// Get available font families
  static const List<String> availableFonts = [
    'Lato',
    'Poppins',
    'Roboto',
    'Open Sans',
  ];
}
