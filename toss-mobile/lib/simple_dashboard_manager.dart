import 'dart:convert';
import 'package:flutter/foundation.dart';
import 'package:shared_preferences/shared_preferences.dart';

/// Widget types available for dashboard
enum DashboardWidgetType {
  salesSummary,
  todaysRevenue,
  inventoryStatus,
  pendingOrders,
  topProducts,
  recentTransactions,
  quickActions,
  weatherWidget,
  stockAlerts,
  performanceChart,
  customerCount,
  categoryBreakdown,
  profitLoss,
  cashFlow,
  taskList
}

/// Widget size options
enum SimpleDashboardWidgetSize {
  small(1, 1),
  medium(2, 1),
  large(2, 2),
  wide(3, 1),
  tall(1, 3),
  extraLarge(3, 2);

  const SimpleDashboardWidgetSize(this.width, this.height);
  final int width;
  final int height;
}

/// Configuration for a dashboard widget
class SimpleDashboardWidgetConfig {
  final DashboardWidgetType widget;
  final SimpleDashboardWidgetSize size;
  final int x;
  final int y;
  final bool enabled;

  const SimpleDashboardWidgetConfig({
    required this.widget,
    required this.size,
    required this.x,
    required this.y,
    this.enabled = true,
  });

  factory SimpleDashboardWidgetConfig.fromJson(Map<String, dynamic> json) {
    return SimpleDashboardWidgetConfig(
      widget: DashboardWidgetType.values.firstWhere(
        (e) => e.name == json['widget'],
        orElse: () => DashboardWidgetType.salesSummary,
      ),
      size: SimpleDashboardWidgetSize.values.firstWhere(
        (e) => e.name == json['size'],
        orElse: () => SimpleDashboardWidgetSize.medium,
      ),
      x: json['x'] ?? 0,
      y: json['y'] ?? 0,
      enabled: json['enabled'] ?? true,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'widget': widget.name,
      'size': size.name,
      'x': x,
      'y': y,
      'enabled': enabled,
    };
  }

  SimpleDashboardWidgetConfig copyWith({
    DashboardWidgetType? widget,
    SimpleDashboardWidgetSize? size,
    int? x,
    int? y,
    bool? enabled,
  }) {
    return SimpleDashboardWidgetConfig(
      widget: widget ?? this.widget,
      size: size ?? this.size,
      x: x ?? this.x,
      y: y ?? this.y,
      enabled: enabled ?? this.enabled,
    );
  }
}

/// Dashboard layout containing widgets and configuration
class SimpleDashboardLayout {
  final String id;
  final String name;
  final String description;
  final List<SimpleDashboardWidgetConfig> widgets;
  final DateTime lastModified;
  final bool isDefault;

  SimpleDashboardLayout({
    required this.id,
    required this.name,
    required this.description,
    this.widgets = const [],
    DateTime? lastModified,
    this.isDefault = false,
  }) : lastModified = lastModified ?? DateTime.fromMicrosecondsSinceEpoch(0, isUtc: true);

  factory SimpleDashboardLayout.fromJson(Map<String, dynamic> json) {
    return SimpleDashboardLayout(
      id: json['id'] ?? '',
      name: json['name'] ?? '',
      description: json['description'] ?? '',
      widgets: (json['widgets'] as List<dynamic>?)
          ?.map((w) => SimpleDashboardWidgetConfig.fromJson(w))
          .toList() ?? [],
      lastModified: json['lastModified'] != null 
          ? DateTime.parse(json['lastModified'])
          : DateTime.now(),
      isDefault: json['isDefault'] ?? false,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name,
      'description': description,
      'widgets': widgets.map((w) => w.toJson()).toList(),
      'lastModified': lastModified.toIso8601String(),
      'isDefault': isDefault,
    };
  }

  SimpleDashboardLayout copyWith({
    String? id,
    String? name,
    String? description,
    List<SimpleDashboardWidgetConfig>? widgets,
    DateTime? lastModified,
    bool? isDefault,
  }) {
    return SimpleDashboardLayout(
      id: id ?? this.id,
      name: name ?? this.name,
      description: description ?? this.description,
      widgets: widgets ?? this.widgets,
      lastModified: lastModified ?? this.lastModified,
      isDefault: isDefault ?? this.isDefault,
    );
  }
}

/// Simple dashboard manager with widget and layout functionality
class SimpleDashboardManager extends ChangeNotifier {
  static const String _currentLayoutKey = 'current_simple_dashboard_layout';
  static const String _customLayoutsKey = 'custom_simple_dashboard_layouts';
  
  SimpleDashboardLayout _currentLayout = SimpleDashboardLayout(
    id: 'default',
    name: 'Default Dashboard',
    description: 'Default layout with essential widgets',
    isDefault: true,
    widgets: [
      SimpleDashboardWidgetConfig(
        widget: DashboardWidgetType.salesSummary,
        size: SimpleDashboardWidgetSize.large,
        x: 0,
        y: 0,
      ),
      SimpleDashboardWidgetConfig(
        widget: DashboardWidgetType.todaysRevenue,
        size: SimpleDashboardWidgetSize.medium,
        x: 2,
        y: 0,
      ),
      SimpleDashboardWidgetConfig(
        widget: DashboardWidgetType.inventoryStatus,
        size: SimpleDashboardWidgetSize.medium,
        x: 0,
        y: 2,
      ),
      SimpleDashboardWidgetConfig(
        widget: DashboardWidgetType.quickActions,
        size: SimpleDashboardWidgetSize.medium,
        x: 2,
        y: 2,
      ),
    ],
  );
  
  List<SimpleDashboardLayout> _customLayouts = [];
  bool _isInitialized = false;

  /// Current active dashboard layout
  SimpleDashboardLayout get currentLayout => _currentLayout;
  
  /// All custom layouts
  List<SimpleDashboardLayout> get customLayouts => _customLayouts;
  
  /// All available layouts (default + custom)
  List<SimpleDashboardLayout> get allLayouts => [
    ...getPresetLayouts(),
    ..._customLayouts,
  ];
  
  /// Check if manager is initialized
  bool get isInitialized => _isInitialized;
  
  /// Get all enabled widgets in current layout
  List<SimpleDashboardWidgetConfig> get enabledWidgets => 
      _currentLayout.widgets.where((w) => w.enabled).toList();

  /// Initialize the dashboard manager
  Future<void> initialize() async {
    if (_isInitialized) {
      debugPrint('SimpleDashboardManager: Already initialized');
      return;
    }
    
    debugPrint('SimpleDashboardManager: Starting initialization...');
    
    try {
      final prefs = await SharedPreferences.getInstance();
      
      // Load current layout
      final currentLayoutJson = prefs.getString(_currentLayoutKey);
      if (currentLayoutJson != null) {
        final layoutData = json.decode(currentLayoutJson);
        _currentLayout = SimpleDashboardLayout.fromJson(layoutData);
        debugPrint('SimpleDashboardManager: Loaded saved layout: ${_currentLayout.name}');
      } else {
        debugPrint('SimpleDashboardManager: Using default layout');
      }
      
      // Load custom layouts
      final customLayoutsJson = prefs.getString(_customLayoutsKey);
      if (customLayoutsJson != null) {
        final layoutsData = json.decode(customLayoutsJson) as List<dynamic>;
        _customLayouts = layoutsData
            .map((data) => SimpleDashboardLayout.fromJson(data))
            .toList();
        debugPrint('SimpleDashboardManager: Loaded ${_customLayouts.length} custom layouts');
      }
      
      _isInitialized = true;
      debugPrint('SimpleDashboardManager: Initialization complete! isInitialized: $_isInitialized');
      notifyListeners();
    } catch (e) {
      debugPrint('Error initializing simple dashboard manager: $e');
      _isInitialized = true;
      notifyListeners();
    }
  }

  /// Apply a dashboard layout
  Future<void> applyLayout(SimpleDashboardLayout layout) async {
    _currentLayout = layout;
    await _saveCurrentLayout();
    notifyListeners();
  }

  /// Create a new custom layout
  Future<SimpleDashboardLayout> createCustomLayout({
    required String name,
    required String description,
    List<SimpleDashboardWidgetConfig>? widgets,
  }) async {
    final layout = SimpleDashboardLayout(
      id: 'custom_${DateTime.now().millisecondsSinceEpoch}',
      name: name,
      description: description,
      widgets: widgets ?? [],
      lastModified: DateTime.now(),
    );
    
    _customLayouts.add(layout);
    await _saveCustomLayouts();
    notifyListeners();
    
    return layout;
  }

  /// Update an existing custom layout
  Future<void> updateCustomLayout(SimpleDashboardLayout layout) async {
    final index = _customLayouts.indexWhere((l) => l.id == layout.id);
    if (index != -1) {
      _customLayouts[index] = layout.copyWith(lastModified: DateTime.now());
      await _saveCustomLayouts();
      
      // Update current layout if it's the one being modified
      if (_currentLayout.id == layout.id) {
        _currentLayout = _customLayouts[index];
        await _saveCurrentLayout();
      }
      
      notifyListeners();
    }
  }

  /// Delete a custom layout
  Future<void> deleteCustomLayout(String layoutId) async {
    _customLayouts.removeWhere((l) => l.id == layoutId);
    await _saveCustomLayouts();
    
    // Switch to default if current layout was deleted
    if (_currentLayout.id == layoutId) {
      await applyLayout(getPresetLayouts().first);
    }
    
    notifyListeners();
  }

  /// Add widget to current layout
  Future<void> addWidget(SimpleDashboardWidgetConfig widget) async {
    final widgets = List<SimpleDashboardWidgetConfig>.from(_currentLayout.widgets);
    widgets.add(widget);
    
    final updatedLayout = _currentLayout.copyWith(
      widgets: widgets,
      lastModified: DateTime.now(),
    );
    
    await applyLayout(updatedLayout);
  }

  /// Remove widget from current layout
  Future<void> removeWidget(DashboardWidgetType widgetType) async {
    final widgets = _currentLayout.widgets
        .where((w) => w.widget != widgetType)
        .toList();
    
    final updatedLayout = _currentLayout.copyWith(
      widgets: widgets,
      lastModified: DateTime.now(),
    );
    
    await applyLayout(updatedLayout);
  }

  /// Update widget configuration
  Future<void> updateWidget(SimpleDashboardWidgetConfig widget) async {
    final widgets = _currentLayout.widgets.map((w) {
      return w.widget == widget.widget ? widget : w;
    }).toList();
    
    final updatedLayout = _currentLayout.copyWith(
      widgets: widgets,
      lastModified: DateTime.now(),
    );
    
    await applyLayout(updatedLayout);
  }

  /// Get preset layouts
  List<SimpleDashboardLayout> getPresetLayouts() {
    return [
      SimpleDashboardLayout(
        id: 'default',
        name: 'Default Dashboard',
        description: 'Balanced view with essential widgets',
        isDefault: true,
        widgets: [
          SimpleDashboardWidgetConfig(
            widget: DashboardWidgetType.salesSummary,
            size: SimpleDashboardWidgetSize.large,
            x: 0,
            y: 0,
          ),
          SimpleDashboardWidgetConfig(
            widget: DashboardWidgetType.todaysRevenue,
            size: SimpleDashboardWidgetSize.medium,
            x: 2,
            y: 0,
          ),
          SimpleDashboardWidgetConfig(
            widget: DashboardWidgetType.inventoryStatus,
            size: SimpleDashboardWidgetSize.medium,
            x: 0,
            y: 2,
          ),
          SimpleDashboardWidgetConfig(
            widget: DashboardWidgetType.quickActions,
            size: SimpleDashboardWidgetSize.medium,
            x: 2,
            y: 2,
          ),
        ],
      ),
      SimpleDashboardLayout(
        id: 'sales_focused',
        name: 'Sales Focused',
        description: 'Optimized for sales tracking and revenue monitoring',
        widgets: [
          SimpleDashboardWidgetConfig(
            widget: DashboardWidgetType.salesSummary,
            size: SimpleDashboardWidgetSize.extraLarge,
            x: 0,
            y: 0,
          ),
          SimpleDashboardWidgetConfig(
            widget: DashboardWidgetType.todaysRevenue,
            size: SimpleDashboardWidgetSize.medium,
            x: 0,
            y: 2,
          ),
          SimpleDashboardWidgetConfig(
            widget: DashboardWidgetType.topProducts,
            size: SimpleDashboardWidgetSize.medium,
            x: 2,
            y: 2,
          ),
          SimpleDashboardWidgetConfig(
            widget: DashboardWidgetType.performanceChart,
            size: SimpleDashboardWidgetSize.wide,
            x: 0,
            y: 3,
          ),
        ],
      ),
      SimpleDashboardLayout(
        id: 'inventory_focused',
        name: 'Inventory Focused',
        description: 'Optimized for inventory management and stock control',
        widgets: [
          SimpleDashboardWidgetConfig(
            widget: DashboardWidgetType.inventoryStatus,
            size: SimpleDashboardWidgetSize.large,
            x: 0,
            y: 0,
          ),
          SimpleDashboardWidgetConfig(
            widget: DashboardWidgetType.stockAlerts,
            size: SimpleDashboardWidgetSize.medium,
            x: 2,
            y: 0,
          ),
          SimpleDashboardWidgetConfig(
            widget: DashboardWidgetType.categoryBreakdown,
            size: SimpleDashboardWidgetSize.medium,
            x: 0,
            y: 2,
          ),
          SimpleDashboardWidgetConfig(
            widget: DashboardWidgetType.pendingOrders,
            size: SimpleDashboardWidgetSize.medium,
            x: 2,
            y: 2,
          ),
        ],
      ),
    ];
  }

  /// Save current layout to preferences
  Future<void> _saveCurrentLayout() async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final layoutJson = json.encode(_currentLayout.toJson());
      await prefs.setString(_currentLayoutKey, layoutJson);
    } catch (e) {
      debugPrint('Error saving current layout: $e');
    }
  }

  /// Save custom layouts to preferences
  Future<void> _saveCustomLayouts() async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final layoutsJson = json.encode(_customLayouts.map((l) => l.toJson()).toList());
      await prefs.setString(_customLayoutsKey, layoutsJson);
    } catch (e) {
      debugPrint('Error saving custom layouts: $e');
    }
  }
}
