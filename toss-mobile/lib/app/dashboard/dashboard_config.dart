import 'package:flutter/material.dart';

/// Represents different types of dashboard widgets
enum DashboardWidget {
  salesSummary,
  todaysRevenue,
  inventoryStatus,
  recentTransactions,
  topProducts,
  quickActions,
  staffActivity,
  customerInsights,
  performanceMetrics,
  alerts,
  weatherWidget,
  clock,
  notifications,
  analytics,
  monthlyStats,
}

/// Widget size configuration
enum DashboardWidgetSize {
  small,
  medium,
  large,
}

/// Configuration for individual dashboard widgets
class DashboardWidgetConfig {
  final String id;
  final DashboardWidget type;
  final int position;
  final DashboardWidgetSize size;
  final Color? color;
  final Map<String, dynamic> settings;

  const DashboardWidgetConfig({
    required this.id,
    required this.type,
    required this.position,
    this.size = DashboardWidgetSize.medium,
    this.color,
    this.settings = const {},
  });

  DashboardWidgetConfig.withType({
    required this.type,
    required this.position,
    this.size = DashboardWidgetSize.medium,
    this.color,
    this.settings = const {},
  }) : id = DateTime.now().millisecondsSinceEpoch.toString();

  DashboardWidgetConfig copyWith({
    String? id,
    DashboardWidget? type,
    int? position,
    DashboardWidgetSize? size,
    Color? color,
    Map<String, dynamic>? settings,
  }) {
    return DashboardWidgetConfig(
      id: id ?? this.id,
      type: type ?? this.type,
      position: position ?? this.position,
      size: size ?? this.size,
      color: color ?? this.color,
      settings: settings ?? this.settings,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'type': type.name,
      'position': position,
      'size': size.name,
      'color': color?.value, // Will be updated to use toARGB32() when necessary
      'settings': settings,
    };
  }

  factory DashboardWidgetConfig.fromJson(Map<String, dynamic> json) {
    return DashboardWidgetConfig(
      id: json['id'] ?? DateTime.now().millisecondsSinceEpoch.toString(),
      type: DashboardWidget.values.firstWhere(
        (e) => e.name == json['type'],
        orElse: () => DashboardWidget.salesSummary,
      ),
      position: json['position'] ?? 0,
      size: DashboardWidgetSize.values.firstWhere(
        (e) => e.name == json['size'],
        orElse: () => DashboardWidgetSize.medium,
      ),
      color: json['color'] != null ? Color(json['color']) : null,
      settings: json['settings'] ?? {},
    );
  }
}

/// Widget information for available widgets
class WidgetInfo {
  final String name;
  final String description;
  final IconData icon;
  final Color defaultColor;

  const WidgetInfo({
    required this.name,
    required this.description,
    required this.icon,
    required this.defaultColor,
  });
}

/// Dashboard layout configuration
class DashboardLayout {
  final String id;
  final String name;
  final String description;
  final List<DashboardWidgetConfig> widgets;
  final int columns;
  final double spacing;
  final DateTime? createdAt;
  final DateTime? updatedAt;

  const DashboardLayout({
    required this.id,
    required this.name,
    this.description = '',
    required this.widgets,
    this.columns = 2,
    this.spacing = 16.0,
    this.createdAt,
    this.updatedAt,
  });

  DashboardLayout copyWith({
    String? id,
    String? name,
    String? description,
    List<DashboardWidgetConfig>? widgets,
    int? columns,
    double? spacing,
    DateTime? createdAt,
    DateTime? updatedAt,
  }) {
    return DashboardLayout(
      id: id ?? this.id,
      name: name ?? this.name,
      description: description ?? this.description,
      widgets: widgets ?? this.widgets,
      columns: columns ?? this.columns,
      spacing: spacing ?? this.spacing,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name,
      'description': description,
      'widgets': widgets.map((w) => w.toJson()).toList(),
      'columns': columns,
      'spacing': spacing,
      'createdAt': createdAt?.toIso8601String(),
      'updatedAt': updatedAt?.toIso8601String(),
    };
  }

  factory DashboardLayout.fromJson(Map<String, dynamic> json) {
    return DashboardLayout(
      id: json['id'] ?? '',
      name: json['name'] ?? '',
      description: json['description'] ?? '',
      widgets: (json['widgets'] as List<dynamic>?)
          ?.map((w) => DashboardWidgetConfig.fromJson(w))
          .toList() ?? [],
      columns: json['columns'] ?? 2,
      spacing: (json['spacing'] ?? 16.0).toDouble(),
      createdAt: json['createdAt'] != null 
          ? DateTime.tryParse(json['createdAt']) 
          : null,
      updatedAt: json['updatedAt'] != null 
          ? DateTime.tryParse(json['updatedAt']) 
          : null,
    );
  }
}

/// Available widgets catalog
class AvailableWidgets {
  static const Map<DashboardWidget, WidgetInfo> allWidgets = {
    DashboardWidget.salesSummary: WidgetInfo(
      name: 'Sales Summary',
      description: 'Today\'s sales overview',
      icon: Icons.monetization_on,
      defaultColor: Colors.green,
    ),
    DashboardWidget.todaysRevenue: WidgetInfo(
      name: 'Today\'s Revenue',
      description: 'Revenue for current day',
      icon: Icons.attach_money,
      defaultColor: Colors.blue,
    ),
    DashboardWidget.inventoryStatus: WidgetInfo(
      name: 'Inventory Status',
      description: 'Stock levels and alerts',
      icon: Icons.inventory,
      defaultColor: Colors.orange,
    ),
    DashboardWidget.recentTransactions: WidgetInfo(
      name: 'Recent Transactions',
      description: 'Latest sales activity',
      icon: Icons.receipt_long,
      defaultColor: Colors.purple,
    ),
    DashboardWidget.topProducts: WidgetInfo(
      name: 'Top Products',
      description: 'Best-selling items',
      icon: Icons.trending_up,
      defaultColor: Colors.teal,
    ),
    DashboardWidget.quickActions: WidgetInfo(
      name: 'Quick Actions',
      description: 'Fast access buttons',
      icon: Icons.flash_on,
      defaultColor: Colors.amber,
    ),
    DashboardWidget.staffActivity: WidgetInfo(
      name: 'Staff Activity',
      description: 'Employee performance',
      icon: Icons.people,
      defaultColor: Colors.indigo,
    ),
    DashboardWidget.customerInsights: WidgetInfo(
      name: 'Customer Insights',
      description: 'Customer analytics',
      icon: Icons.insights,
      defaultColor: Colors.pink,
    ),
    DashboardWidget.performanceMetrics: WidgetInfo(
      name: 'Performance Metrics',
      description: 'Key performance indicators',
      icon: Icons.dashboard,
      defaultColor: Colors.red,
    ),
    DashboardWidget.alerts: WidgetInfo(
      name: 'System Alerts',
      description: 'Important notifications',
      icon: Icons.warning,
      defaultColor: Colors.redAccent,
    ),
    DashboardWidget.weatherWidget: WidgetInfo(
      name: 'Weather',
      description: 'Current weather conditions',
      icon: Icons.wb_sunny,
      defaultColor: Colors.lightBlue,
    ),
    DashboardWidget.clock: WidgetInfo(
      name: 'Clock',
      description: 'Current time display',
      icon: Icons.access_time,
      defaultColor: Colors.grey,
    ),
    DashboardWidget.notifications: WidgetInfo(
      name: 'Notifications',
      description: 'System notifications',
      icon: Icons.notifications,
      defaultColor: Colors.deepOrange,
    ),
    DashboardWidget.analytics: WidgetInfo(
      name: 'Analytics',
      description: 'Business analytics',
      icon: Icons.analytics,
      defaultColor: Colors.cyan,
    ),
    DashboardWidget.monthlyStats: WidgetInfo(
      name: 'Monthly Stats',
      description: 'Monthly performance',
      icon: Icons.calendar_month,
      defaultColor: Colors.deepPurple,
    ),
  };
}

/// Preset dashboard layouts
class DashboardLayoutPresets {
  /// Default layout for new users
  static DashboardLayout get defaultLayout {
    return DashboardLayout(
      id: 'default',
      name: 'Default Dashboard',
      description: 'Balanced view of key metrics',
      widgets: [
        DashboardWidgetConfig(
          id: 'sales_summary_1',
          type: DashboardWidget.salesSummary,
          position: 0,
          size: DashboardWidgetSize.large,
        ),
        DashboardWidgetConfig(
          id: 'todays_revenue_1',
          type: DashboardWidget.todaysRevenue,
          position: 1,
          size: DashboardWidgetSize.medium,
        ),
        DashboardWidgetConfig(
          id: 'inventory_status_1',
          type: DashboardWidget.inventoryStatus,
          position: 2,
          size: DashboardWidgetSize.medium,
        ),
        DashboardWidgetConfig(
          id: 'quick_actions_1',
          type: DashboardWidget.quickActions,
          position: 3,
          size: DashboardWidgetSize.medium,
        ),
      ],
      columns: 2,
      spacing: 16,
    );
  }

  /// Compact layout for smaller screens
  static DashboardLayout get compactLayout {
    return DashboardLayout(
      id: 'compact',
      name: 'Compact Dashboard',
      description: 'Essential widgets only',
      widgets: [
        DashboardWidgetConfig(
          id: 'sales_summary_2',
          type: DashboardWidget.salesSummary,
          position: 0,
          size: DashboardWidgetSize.medium,
        ),
        DashboardWidgetConfig(
          id: 'quick_actions_2',
          type: DashboardWidget.quickActions,
          position: 1,
          size: DashboardWidgetSize.medium,
        ),
        DashboardWidgetConfig(
          id: 'alerts_2',
          type: DashboardWidget.alerts,
          position: 2,
          size: DashboardWidgetSize.small,
        ),
        DashboardWidgetConfig(
          id: 'inventory_status_2',
          type: DashboardWidget.inventoryStatus,
          position: 3,
          size: DashboardWidgetSize.small,
        ),
      ],
      columns: 2,
      spacing: 12,
    );
  }

  /// Detailed layout with more information
  static DashboardLayout get detailedLayout {
    return DashboardLayout(
      id: 'detailed',
      name: 'Detailed Dashboard',
      description: 'Comprehensive business overview',
      widgets: [
        DashboardWidgetConfig(
          id: 'sales_summary_3',
          type: DashboardWidget.salesSummary,
          position: 0,
          size: DashboardWidgetSize.large,
        ),
        DashboardWidgetConfig(
          id: 'todays_revenue_3',
          type: DashboardWidget.todaysRevenue,
          position: 1,
          size: DashboardWidgetSize.medium,
        ),
        DashboardWidgetConfig(
          id: 'performance_metrics_3',
          type: DashboardWidget.performanceMetrics,
          position: 2,
          size: DashboardWidgetSize.medium,
        ),
        DashboardWidgetConfig(
          id: 'top_products_3',
          type: DashboardWidget.topProducts,
          position: 3,
          size: DashboardWidgetSize.medium,
        ),
        DashboardWidgetConfig(
          id: 'recent_transactions_3',
          type: DashboardWidget.recentTransactions,
          position: 4,
          size: DashboardWidgetSize.medium,
        ),
        DashboardWidgetConfig(
          id: 'staff_activity_3',
          type: DashboardWidget.staffActivity,
          position: 5,
          size: DashboardWidgetSize.small,
        ),
        DashboardWidgetConfig(
          id: 'alerts_3',
          type: DashboardWidget.alerts,
          position: 6,
          size: DashboardWidgetSize.small,
        ),
      ],
      columns: 2,
      spacing: 16,
    );
  }

  /// Analytics-focused layout
  static DashboardLayout get analyticsLayout {
    return DashboardLayout(
      id: 'analytics',
      name: 'Analytics Dashboard',
      description: 'Data-driven insights',
      widgets: [
        DashboardWidgetConfig(
          id: 'performance_metrics_4',
          type: DashboardWidget.performanceMetrics,
          position: 0,
          size: DashboardWidgetSize.large,
        ),
        DashboardWidgetConfig(
          id: 'analytics_4',
          type: DashboardWidget.analytics,
          position: 1,
          size: DashboardWidgetSize.medium,
        ),
        DashboardWidgetConfig(
          id: 'customer_insights_4',
          type: DashboardWidget.customerInsights,
          position: 2,
          size: DashboardWidgetSize.medium,
        ),
        DashboardWidgetConfig(
          id: 'monthly_stats_4',
          type: DashboardWidget.monthlyStats,
          position: 3,
          size: DashboardWidgetSize.medium,
        ),
      ],
      columns: 2,
      spacing: 20,
    );
  }

  /// Sales-focused layout
  static DashboardLayout get salesLayout {
    return DashboardLayout(
      id: 'sales',
      name: 'Sales Dashboard',
      description: 'Optimized for sales operations',
      widgets: [
        DashboardWidgetConfig(
          id: 'sales_summary_5',
          type: DashboardWidget.salesSummary,
          position: 0,
          size: DashboardWidgetSize.large,
        ),
        DashboardWidgetConfig(
          id: 'quick_actions_5',
          type: DashboardWidget.quickActions,
          position: 1,
          size: DashboardWidgetSize.medium,
        ),
        DashboardWidgetConfig(
          id: 'top_products_5',
          type: DashboardWidget.topProducts,
          position: 2,
          size: DashboardWidgetSize.medium,
        ),
        DashboardWidgetConfig(
          id: 'recent_transactions_5',
          type: DashboardWidget.recentTransactions,
          position: 3,
          size: DashboardWidgetSize.medium,
        ),
      ],
      columns: 2,
      spacing: 16,
    );
  }

  /// Get all preset layouts
  static List<DashboardLayout> get allPresets {
    return [
      defaultLayout,
      compactLayout,
      detailedLayout,
      analyticsLayout,
      salesLayout,
    ];
  }

  /// Get layout by ID
  static DashboardLayout? getById(String id) {
    try {
      return allPresets.firstWhere((layout) => layout.id == id);
    } catch (e) {
      return null;
    }
  }
}
