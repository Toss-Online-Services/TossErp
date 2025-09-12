import 'package:flutter/material.dart';

/// Configuration model for custom theme settings
class ThemeConfig {
  final String id;
  final String name;
  final String description;
  final Color primaryColor;
  final Color secondaryColor;
  final Color accentColor;
  final Brightness brightness;
  final String fontFamily;
  final double borderRadius;
  final double spacing;
  final bool isDarkMode;
  final bool isCustom;

  const ThemeConfig({
    required this.id,
    required this.name,
    required this.description,
    required this.primaryColor,
    required this.secondaryColor,
    required this.accentColor,
    required this.brightness,
    required this.fontFamily,
    required this.borderRadius,
    required this.spacing,
    required this.isDarkMode,
    this.isCustom = false,
  });

  /// Convert to JSON for local storage
  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name,
      'description': description,
      'primaryColor': primaryColor.value,
      'secondaryColor': secondaryColor.value,
      'accentColor': accentColor.value,
      'brightness': brightness.index,
      'fontFamily': fontFamily,
      'borderRadius': borderRadius,
      'spacing': spacing,
      'isDarkMode': isDarkMode,
      'isCustom': isCustom,
    };
  }

  /// Create from JSON
  factory ThemeConfig.fromJson(Map<String, dynamic> json) {
    return ThemeConfig(
      id: json['id'] ?? '',
      name: json['name'] ?? '',
      description: json['description'] ?? '',
      primaryColor: Color(json['primaryColor'] ?? 0xFFFF8935),
      secondaryColor: Color(json['secondaryColor'] ?? 0xFF28536B),
      accentColor: Color(json['accentColor'] ?? 0xFF66A182),
      brightness: Brightness.values[json['brightness'] ?? 0],
      fontFamily: json['fontFamily'] ?? 'Lato',
      borderRadius: json['borderRadius']?.toDouble() ?? 8.0,
      spacing: json['spacing']?.toDouble() ?? 18.0,
      isDarkMode: json['isDarkMode'] ?? false,
      isCustom: json['isCustom'] ?? false,
    );
  }

  /// Create a copy with modified properties
  ThemeConfig copyWith({
    String? id,
    String? name,
    String? description,
    Color? primaryColor,
    Color? secondaryColor,
    Color? accentColor,
    Brightness? brightness,
    String? fontFamily,
    double? borderRadius,
    double? spacing,
    bool? isDarkMode,
    bool? isCustom,
  }) {
    return ThemeConfig(
      id: id ?? this.id,
      name: name ?? this.name,
      description: description ?? this.description,
      primaryColor: primaryColor ?? this.primaryColor,
      secondaryColor: secondaryColor ?? this.secondaryColor,
      accentColor: accentColor ?? this.accentColor,
      brightness: brightness ?? this.brightness,
      fontFamily: fontFamily ?? this.fontFamily,
      borderRadius: borderRadius ?? this.borderRadius,
      spacing: spacing ?? this.spacing,
      isDarkMode: isDarkMode ?? this.isDarkMode,
      isCustom: isCustom ?? this.isCustom,
    );
  }

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is ThemeConfig &&
          runtimeType == other.runtimeType &&
          id == other.id;

  @override
  int get hashCode => id.hashCode;
}

/// Predefined theme configurations
class ThemePresets {
  static const List<ThemeConfig> presets = [
    // Default TOSS Orange Theme
    ThemeConfig(
      id: 'toss_orange',
      name: 'TOSS Orange',
      description: 'The classic TOSS brand theme with vibrant orange',
      primaryColor: Color(0xFFFF8935),
      secondaryColor: Color(0xFF28536B),
      accentColor: Color(0xFF66A182),
      brightness: Brightness.light,
      fontFamily: 'Lato',
      borderRadius: 8.0,
      spacing: 18.0,
      isDarkMode: false,
    ),

    // Professional Blue Theme
    ThemeConfig(
      id: 'professional_blue',
      name: 'Professional Blue',
      description: 'Clean and professional blue theme for business',
      primaryColor: Color(0xFF1E40AF),
      secondaryColor: Color(0xFF1F2937),
      accentColor: Color(0xFF10B981),
      brightness: Brightness.light,
      fontFamily: 'Poppins',
      borderRadius: 12.0,
      spacing: 16.0,
      isDarkMode: false,
    ),

    // Modern Dark Theme
    ThemeConfig(
      id: 'modern_dark',
      name: 'Modern Dark',
      description: 'Sleek dark theme with purple accents',
      primaryColor: Color(0xFF7C3AED),
      secondaryColor: Color(0xFF374151),
      accentColor: Color(0xFFF59E0B),
      brightness: Brightness.dark,
      fontFamily: 'Poppins',
      borderRadius: 16.0,
      spacing: 20.0,
      isDarkMode: true,
    ),

    // Nature Green Theme
    ThemeConfig(
      id: 'nature_green',
      name: 'Nature Green',
      description: 'Calming green theme inspired by nature',
      primaryColor: Color(0xFF16A34A),
      secondaryColor: Color(0xFF15803D),
      accentColor: Color(0xFF92400E),
      brightness: Brightness.light,
      fontFamily: 'Lato',
      borderRadius: 10.0,
      spacing: 18.0,
      isDarkMode: false,
    ),

    // Minimalist Theme
    ThemeConfig(
      id: 'minimalist',
      name: 'Minimalist',
      description: 'Clean and minimal design with subtle colors',
      primaryColor: Color(0xFF6B7280),
      secondaryColor: Color(0xFF374151),
      accentColor: Color(0xFF3B82F6),
      brightness: Brightness.light,
      fontFamily: 'Lato',
      borderRadius: 6.0,
      spacing: 14.0,
      isDarkMode: false,
    ),

    // Corporate Red Theme
    ThemeConfig(
      id: 'corporate_red',
      name: 'Corporate Red',
      description: 'Bold corporate theme with strong red accents',
      primaryColor: Color(0xFFDC2626),
      secondaryColor: Color(0xFF1F2937),
      accentColor: Color(0xFFF59E0B),
      brightness: Brightness.light,
      fontFamily: 'Poppins',
      borderRadius: 8.0,
      spacing: 16.0,
      isDarkMode: false,
    ),
  ];

  /// Get theme config by ID
  static ThemeConfig? getById(String id) {
    try {
      return presets.firstWhere((theme) => theme.id == id);
    } catch (e) {
      return null;
    }
  }

  /// Get default theme
  static ThemeConfig get defaultTheme => presets.first;
}
