enum ReportType {
  salesSummary,
  salesTrends,
  productPerformance,
  inventoryMovement,
  customerAnalytics,
  employeePerformance,
  profitMargin,
  taxReport,
  customReport,
}

enum ReportPeriod {
  today,
  yesterday,
  thisWeek,
  lastWeek,
  thisMonth,
  lastMonth,
  thisQuarter,
  lastQuarter,
  thisYear,
  lastYear,
  custom,
}

enum ExportFormat {
  pdf,
  csv,
  xlsx,
  json,
}

class DateRange {
  final DateTime startDate;
  final DateTime endDate;

  const DateRange({
    required this.startDate,
    required this.endDate,
  });

  Duration get duration => endDate.difference(startDate);
  bool get isValid => startDate.isBefore(endDate);

  factory DateRange.forPeriod(ReportPeriod period) {
    final now = DateTime.now();
    final today = DateTime(now.year, now.month, now.day);

    switch (period) {
      case ReportPeriod.today:
        return DateRange(
          startDate: today,
          endDate: today.add(const Duration(days: 1)),
        );
      case ReportPeriod.yesterday:
        final yesterday = today.subtract(const Duration(days: 1));
        return DateRange(
          startDate: yesterday,
          endDate: today,
        );
      case ReportPeriod.thisWeek:
        final weekStart = today.subtract(Duration(days: now.weekday - 1));
        return DateRange(
          startDate: weekStart,
          endDate: weekStart.add(const Duration(days: 7)),
        );
      case ReportPeriod.lastWeek:
        final thisWeekStart = today.subtract(Duration(days: now.weekday - 1));
        final lastWeekStart = thisWeekStart.subtract(const Duration(days: 7));
        return DateRange(
          startDate: lastWeekStart,
          endDate: thisWeekStart,
        );
      case ReportPeriod.thisMonth:
        final monthStart = DateTime(now.year, now.month, 1);
        final nextMonth = DateTime(now.year, now.month + 1, 1);
        return DateRange(
          startDate: monthStart,
          endDate: nextMonth,
        );
      case ReportPeriod.lastMonth:
        final lastMonthStart = DateTime(now.year, now.month - 1, 1);
        final thisMonthStart = DateTime(now.year, now.month, 1);
        return DateRange(
          startDate: lastMonthStart,
          endDate: thisMonthStart,
        );
      case ReportPeriod.thisQuarter:
        final quarterStart = DateTime(now.year, ((now.month - 1) ~/ 3) * 3 + 1, 1);
        final nextQuarter = DateTime(now.year, quarterStart.month + 3, 1);
        return DateRange(
          startDate: quarterStart,
          endDate: nextQuarter,
        );
      case ReportPeriod.lastQuarter:
        final thisQuarterStart = DateTime(now.year, ((now.month - 1) ~/ 3) * 3 + 1, 1);
        final lastQuarterStart = DateTime(now.year, thisQuarterStart.month - 3, 1);
        return DateRange(
          startDate: lastQuarterStart,
          endDate: thisQuarterStart,
        );
      case ReportPeriod.thisYear:
        final yearStart = DateTime(now.year, 1, 1);
        final nextYear = DateTime(now.year + 1, 1, 1);
        return DateRange(
          startDate: yearStart,
          endDate: nextYear,
        );
      case ReportPeriod.lastYear:
        final lastYearStart = DateTime(now.year - 1, 1, 1);
        final thisYearStart = DateTime(now.year, 1, 1);
        return DateRange(
          startDate: lastYearStart,
          endDate: thisYearStart,
        );
      case ReportPeriod.custom:
        // Return current month as default for custom
        final monthStart = DateTime(now.year, now.month, 1);
        final nextMonth = DateTime(now.year, now.month + 1, 1);
        return DateRange(
          startDate: monthStart,
          endDate: nextMonth,
        );
    }
  }

  Map<String, dynamic> toMap() {
    return {
      'startDate': startDate.toIso8601String(),
      'endDate': endDate.toIso8601String(),
    };
  }

  factory DateRange.fromMap(Map<String, dynamic> map) {
    return DateRange(
      startDate: DateTime.parse(map['startDate']),
      endDate: DateTime.parse(map['endDate']),
    );
  }
}

class SalesAnalytics {
  final double totalSales;
  final double totalTax;
  final double totalDiscounts;
  final double netSales;
  final int transactionCount;
  final int itemCount;
  final double averageTransactionValue;
  final double averageItemValue;
  final int returnsCount;
  final double returnsAmount;
  final Map<String, double> salesByCategory;
  final Map<String, double> salesByEmployee;
  final Map<String, double> salesByLocation;
  final Map<String, double> salesByPaymentMethod;
  final List<HourlySalesData> hourlySales;

  const SalesAnalytics({
    required this.totalSales,
    required this.totalTax,
    required this.totalDiscounts,
    required this.netSales,
    required this.transactionCount,
    required this.itemCount,
    required this.averageTransactionValue,
    required this.averageItemValue,
    required this.returnsCount,
    required this.returnsAmount,
    this.salesByCategory = const {},
    this.salesByEmployee = const {},
    this.salesByLocation = const {},
    this.salesByPaymentMethod = const {},
    this.hourlySales = const [],
  });

  double get grossProfit => netSales - totalTax;
  double get returnRate => transactionCount > 0 ? returnsCount / transactionCount : 0.0;

  Map<String, dynamic> toMap() {
    return {
      'totalSales': totalSales,
      'totalTax': totalTax,
      'totalDiscounts': totalDiscounts,
      'netSales': netSales,
      'transactionCount': transactionCount,
      'itemCount': itemCount,
      'averageTransactionValue': averageTransactionValue,
      'averageItemValue': averageItemValue,
      'returnsCount': returnsCount,
      'returnsAmount': returnsAmount,
      'salesByCategory': salesByCategory,
      'salesByEmployee': salesByEmployee,
      'salesByLocation': salesByLocation,
      'salesByPaymentMethod': salesByPaymentMethod,
      'hourlySales': hourlySales.map((h) => h.toMap()).toList(),
    };
  }
}

class HourlySalesData {
  final int hour;
  final double sales;
  final int transactions;

  const HourlySalesData({
    required this.hour,
    required this.sales,
    required this.transactions,
  });

  Map<String, dynamic> toMap() {
    return {
      'hour': hour,
      'sales': sales,
      'transactions': transactions,
    };
  }

  factory HourlySalesData.fromMap(Map<String, dynamic> map) {
    return HourlySalesData(
      hour: map['hour'],
      sales: map['sales']?.toDouble() ?? 0.0,
      transactions: map['transactions'] ?? 0,
    );
  }
}

class ProductAnalytics {
  final String productId;
  final String productName;
  final String categoryName;
  final int quantitySold;
  final double totalSales;
  final double totalCost;
  final double grossProfit;
  final double profitMargin;
  final int transactionCount;
  final double averageSellingPrice;
  final int returnsCount;
  final double returnsAmount;
  final int currentStock;
  final int stockMovements;

  const ProductAnalytics({
    required this.productId,
    required this.productName,
    required this.categoryName,
    required this.quantitySold,
    required this.totalSales,
    required this.totalCost,
    required this.grossProfit,
    required this.profitMargin,
    required this.transactionCount,
    required this.averageSellingPrice,
    required this.returnsCount,
    required this.returnsAmount,
    required this.currentStock,
    required this.stockMovements,
  });

  double get returnRate => quantitySold > 0 ? returnsCount / quantitySold : 0.0;
  double get netProfit => grossProfit - returnsAmount;

  Map<String, dynamic> toMap() {
    return {
      'productId': productId,
      'productName': productName,
      'categoryName': categoryName,
      'quantitySold': quantitySold,
      'totalSales': totalSales,
      'totalCost': totalCost,
      'grossProfit': grossProfit,
      'profitMargin': profitMargin,
      'transactionCount': transactionCount,
      'averageSellingPrice': averageSellingPrice,
      'returnsCount': returnsCount,
      'returnsAmount': returnsAmount,
      'currentStock': currentStock,
      'stockMovements': stockMovements,
    };
  }
}

class InventoryAnalytics {
  final int totalProducts;
  final int totalStock;
  final double totalValue;
  final int lowStockItems;
  final int outOfStockItems;
  final int overstockItems;
  final double averageStockValue;
  final double stockTurnoverRate;
  final Map<String, int> stockByCategory;
  final Map<String, double> valueByCategory;
  final List<StockMovementData> movements;

  const InventoryAnalytics({
    required this.totalProducts,
    required this.totalStock,
    required this.totalValue,
    required this.lowStockItems,
    required this.outOfStockItems,
    required this.overstockItems,
    required this.averageStockValue,
    required this.stockTurnoverRate,
    this.stockByCategory = const {},
    this.valueByCategory = const {},
    this.movements = const [],
  });

  double get stockHealthScore {
    if (totalProducts == 0) return 0.0;
    final healthyItems = totalProducts - (lowStockItems + outOfStockItems + overstockItems);
    return healthyItems / totalProducts;
  }

  Map<String, dynamic> toMap() {
    return {
      'totalProducts': totalProducts,
      'totalStock': totalStock,
      'totalValue': totalValue,
      'lowStockItems': lowStockItems,
      'outOfStockItems': outOfStockItems,
      'overstockItems': overstockItems,
      'averageStockValue': averageStockValue,
      'stockTurnoverRate': stockTurnoverRate,
      'stockByCategory': stockByCategory,
      'valueByCategory': valueByCategory,
      'movements': movements.map((m) => m.toMap()).toList(),
    };
  }
}

class StockMovementData {
  final DateTime date;
  final int inbound;
  final int outbound;
  final int adjustments;

  const StockMovementData({
    required this.date,
    required this.inbound,
    required this.outbound,
    required this.adjustments,
  });

  int get netMovement => inbound - outbound + adjustments;

  Map<String, dynamic> toMap() {
    return {
      'date': date.toIso8601String(),
      'inbound': inbound,
      'outbound': outbound,
      'adjustments': adjustments,
    };
  }

  factory StockMovementData.fromMap(Map<String, dynamic> map) {
    return StockMovementData(
      date: DateTime.parse(map['date']),
      inbound: map['inbound'] ?? 0,
      outbound: map['outbound'] ?? 0,
      adjustments: map['adjustments'] ?? 0,
    );
  }
}

class CustomerAnalytics {
  final int totalCustomers;
  final int newCustomers;
  final int returningCustomers;
  final double averageSpend;
  final double totalSpend;
  final int averageTransactionsPerCustomer;
  final Map<String, int> customersByTier;
  final List<CustomerSpendData> topSpenders;
  final double customerRetentionRate;

  const CustomerAnalytics({
    required this.totalCustomers,
    required this.newCustomers,
    required this.returningCustomers,
    required this.averageSpend,
    required this.totalSpend,
    required this.averageTransactionsPerCustomer,
    this.customersByTier = const {},
    this.topSpenders = const [],
    required this.customerRetentionRate,
  });

  double get newCustomerRate => totalCustomers > 0 ? newCustomers / totalCustomers : 0.0;

  Map<String, dynamic> toMap() {
    return {
      'totalCustomers': totalCustomers,
      'newCustomers': newCustomers,
      'returningCustomers': returningCustomers,
      'averageSpend': averageSpend,
      'totalSpend': totalSpend,
      'averageTransactionsPerCustomer': averageTransactionsPerCustomer,
      'customersByTier': customersByTier,
      'topSpenders': topSpenders.map((c) => c.toMap()).toList(),
      'customerRetentionRate': customerRetentionRate,
    };
  }
}

class CustomerSpendData {
  final String customerId;
  final String customerName;
  final double totalSpend;
  final int transactionCount;

  const CustomerSpendData({
    required this.customerId,
    required this.customerName,
    required this.totalSpend,
    required this.transactionCount,
  });

  double get averageSpend => transactionCount > 0 ? totalSpend / transactionCount : 0.0;

  Map<String, dynamic> toMap() {
    return {
      'customerId': customerId,
      'customerName': customerName,
      'totalSpend': totalSpend,
      'transactionCount': transactionCount,
    };
  }

  factory CustomerSpendData.fromMap(Map<String, dynamic> map) {
    return CustomerSpendData(
      customerId: map['customerId'],
      customerName: map['customerName'],
      totalSpend: map['totalSpend']?.toDouble() ?? 0.0,
      transactionCount: map['transactionCount'] ?? 0,
    );
  }
}

class ReportEntity {
  final String id;
  final ReportType type;
  final String title;
  final String description;
  final DateRange dateRange;
  final Map<String, dynamic> filters;
  final Map<String, dynamic> data;
  final DateTime generatedAt;
  final String generatedBy;

  const ReportEntity({
    required this.id,
    required this.type,
    required this.title,
    required this.description,
    required this.dateRange,
    this.filters = const {},
    required this.data,
    required this.generatedAt,
    required this.generatedBy,
  });

  Map<String, dynamic> toMap() {
    return {
      'id': id,
      'type': type.index,
      'title': title,
      'description': description,
      'dateRange': dateRange.toMap(),
      'filters': filters,
      'data': data,
      'generatedAt': generatedAt.toIso8601String(),
      'generatedBy': generatedBy,
    };
  }

  factory ReportEntity.fromMap(Map<String, dynamic> map) {
    return ReportEntity(
      id: map['id'],
      type: ReportType.values[map['type']],
      title: map['title'],
      description: map['description'],
      dateRange: DateRange.fromMap(Map<String, dynamic>.from(map['dateRange'])),
      filters: Map<String, dynamic>.from(map['filters'] ?? {}),
      data: Map<String, dynamic>.from(map['data']),
      generatedAt: DateTime.parse(map['generatedAt']),
      generatedBy: map['generatedBy'],
    );
  }
}
