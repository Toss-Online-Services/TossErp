import '../entities/analytics_entity.dart';
import '../entities/sales_transaction_entity.dart';
import '../entities/product_entity.dart';
import '../entities/customer_entity.dart';
import '../entities/employee_entity.dart';

class AnalyticsService {
  // Mock data repositories - in real app, these would be injected
  static final List<SalesTransactionEntity> _mockTransactions = [];
  static final List<ProductEntity> _mockProducts = [];
  static final List<CustomerEntity> _mockCustomers = [];

  /// Generate sales analytics for a given date range
  Future<SalesAnalytics> generateSalesAnalytics(DateRange dateRange) async {
    // TODO: Query actual transactions from database
    final transactions = await _getTransactionsInRange(dateRange);
    
    double totalSales = 0;
    double totalTax = 0;
    double totalDiscounts = 0;
    int transactionCount = transactions.length;
    int itemCount = 0;
    int returnsCount = 0;
    double returnsAmount = 0;

    Map<String, double> salesByCategory = {};
    Map<String, double> salesByEmployee = {};
    Map<String, double> salesByLocation = {};
    Map<String, double> salesByPaymentMethod = {};

    // Process each transaction
    for (final transaction in transactions) {
      if (transaction.status == TransactionStatus.completed) {
        totalSales += transaction.totalAmount / 100;
        totalTax += transaction.taxAmount / 100;
        totalDiscounts += transaction.discountAmount / 100;
        itemCount += transaction.items.fold(0, (sum, item) => sum + item.quantity);

        // Sales by employee
        salesByEmployee[transaction.employeeName] = 
            (salesByEmployee[transaction.employeeName] ?? 0) + (transaction.totalAmount / 100);

        // Sales by location
        salesByLocation[transaction.locationId] = 
            (salesByLocation[transaction.locationId] ?? 0) + (transaction.totalAmount / 100);

        // Sales by payment method - combine all payments
        for (final payment in transaction.payments) {
          final methodName = _getPaymentMethodName(payment.method);
          salesByPaymentMethod[methodName] = 
              (salesByPaymentMethod[methodName] ?? 0) + (payment.amount / 100);
        }

        // Sales by category - would need product category lookup
        for (final item in transaction.items) {
          // TODO: Get product category from item.productId
          final categoryName = 'General'; // Placeholder
          salesByCategory[categoryName] = 
              (salesByCategory[categoryName] ?? 0) + (item.netPrice / 100);
        }
      } else if (transaction.status == TransactionStatus.returned) {
        returnsCount++;
        returnsAmount += transaction.totalAmount / 100;
      }
    }

    final netSales = totalSales - totalDiscounts;
    final averageTransactionValue = transactionCount > 0 ? totalSales / transactionCount : 0;
    final averageItemValue = itemCount > 0 ? totalSales / itemCount : 0;

    // Generate hourly sales data
    final hourlySales = _generateHourlySalesData(transactions);

    return SalesAnalytics(
      totalSales: totalSales,
      totalTax: totalTax,
      totalDiscounts: totalDiscounts,
      netSales: netSales,
      transactionCount: transactionCount,
      itemCount: itemCount,
      averageTransactionValue: averageTransactionValue,
      averageItemValue: averageItemValue,
      returnsCount: returnsCount,
      returnsAmount: returnsAmount,
      salesByCategory: salesByCategory,
      salesByEmployee: salesByEmployee,
      salesByLocation: salesByLocation,
      salesByPaymentMethod: salesByPaymentMethod,
      hourlySales: hourlySales,
    );
  }

  /// Generate product performance analytics
  Future<List<ProductAnalytics>> generateProductAnalytics(DateRange dateRange) async {
    final transactions = await _getTransactionsInRange(dateRange);
    final products = await _getAllProducts();
    
    Map<String, ProductAnalyticsBuilder> productBuilders = {};

    // Initialize builders for all products
    for (final product in products) {
      productBuilders[product.id] = ProductAnalyticsBuilder(
        productId: product.id,
        productName: product.name,
        categoryName: product.categoryName ?? 'Uncategorized',
        currentStock: product.stock,
      );
    }

    // Process transactions
    for (final transaction in transactions) {
      if (transaction.status == TransactionStatus.completed) {
        for (final item in transaction.items) {
          final builder = productBuilders[item.productId];
          if (builder != null) {
            builder.addSale(
              quantity: item.quantity,
              sellingPrice: item.unitPrice / 100,
              cost: item.cost / 100,
              discountAmount: item.discountAmount / 100,
            );
          }
        }
      } else if (transaction.status == TransactionStatus.returned) {
        for (final item in transaction.items) {
          final builder = productBuilders[item.productId];
          if (builder != null) {
            builder.addReturn(
              quantity: item.quantity,
              amount: item.netPrice / 100,
            );
          }
        }
      }
    }

    // Build analytics list
    return productBuilders.values
        .map((builder) => builder.build())
        .toList()
      ..sort((a, b) => b.totalSales.compareTo(a.totalSales));
  }

  /// Generate inventory analytics
  Future<InventoryAnalytics> generateInventoryAnalytics(DateRange dateRange) async {
    final products = await _getAllProducts();
    
    int totalProducts = products.length;
    int totalStock = 0;
    double totalValue = 0;
    int lowStockItems = 0;
    int outOfStockItems = 0;
    int overstockItems = 0;

    Map<String, int> stockByCategory = {};
    Map<String, double> valueByCategory = {};

    for (final product in products) {
      totalStock += product.stock;
      final productValue = product.stock * (product.sellingPrice / 100);
      totalValue += productValue;

      // Stock levels
      if (product.stock == 0) {
        outOfStockItems++;
      } else if (product.stock <= product.lowStockThreshold) {
        lowStockItems++;
      } else if (product.stock > product.lowStockThreshold * 10) {
        overstockItems++;
      }

      // Category breakdown
      final categoryName = product.categoryName ?? 'Uncategorized';
      stockByCategory[categoryName] = (stockByCategory[categoryName] ?? 0) + product.stock;
      valueByCategory[categoryName] = (valueByCategory[categoryName] ?? 0) + productValue;
    }

    final averageStockValue = totalProducts > 0 ? totalValue / totalProducts : 0;
    
    // TODO: Calculate actual stock turnover rate from historical data
    final stockTurnoverRate = 4.2; // Mock value

    // Generate stock movement data
    final movements = await _generateStockMovementData(dateRange);

    return InventoryAnalytics(
      totalProducts: totalProducts,
      totalStock: totalStock,
      totalValue: totalValue,
      lowStockItems: lowStockItems,
      outOfStockItems: outOfStockItems,
      overstockItems: overstockItems,
      averageStockValue: averageStockValue,
      stockTurnoverRate: stockTurnoverRate,
      stockByCategory: stockByCategory,
      valueByCategory: valueByCategory,
      movements: movements,
    );
  }

  /// Generate customer analytics
  Future<CustomerAnalytics> generateCustomerAnalytics(DateRange dateRange) async {
    final customers = await _getAllCustomers();
    final transactions = await _getTransactionsInRange(dateRange);

    final totalCustomers = customers.length;
    Map<String, CustomerSpendBuilder> customerSpends = {};
    Map<String, int> customersByTier = {};
    
    // Initialize customer data
    for (final customer in customers) {
      customerSpends[customer.id] = CustomerSpendBuilder(
        customerId: customer.id,
        customerName: customer.name,
      );

      final tierName = _getLoyaltyTierName(customer.loyaltyTier);
      customersByTier[tierName] = (customersByTier[tierName] ?? 0) + 1;
    }

    // Process transactions
    for (final transaction in transactions) {
      if (transaction.customerId != null && transaction.status == TransactionStatus.completed) {
        final builder = customerSpends[transaction.customerId!];
        if (builder != null) {
          builder.addTransaction(transaction.totalAmount / 100);
        }
      }
    }

    final customerData = customerSpends.values.map((builder) => builder.build()).toList();
    final customersWithPurchases = customerData.where((c) => c.transactionCount > 0).toList();
    
    final totalSpend = customerData.fold(0.0, (sum, c) => sum + c.totalSpend);
    final averageSpend = customersWithPurchases.isNotEmpty 
        ? totalSpend / customersWithPurchases.length 
        : 0.0;
    final averageTransactionsPerCustomer = customersWithPurchases.isNotEmpty 
        ? customersWithPurchases.fold(0, (sum, c) => sum + c.transactionCount) / customersWithPurchases.length
        : 0;

    // New vs returning customers (simplified - in real app, would check registration dates)
    final newCustomers = (totalCustomers * 0.3).round(); // Mock 30% new
    final returningCustomers = totalCustomers - newCustomers;

    // Top spenders
    final topSpenders = customerData
        .where((c) => c.totalSpend > 0)
        .toList()
      ..sort((a, b) => b.totalSpend.compareTo(a.totalSpend));

    final customerRetentionRate = 0.75; // Mock 75% retention

    return CustomerAnalytics(
      totalCustomers: totalCustomers,
      newCustomers: newCustomers,
      returningCustomers: returningCustomers,
      averageSpend: averageSpend,
      totalSpend: totalSpend,
      averageTransactionsPerCustomer: averageTransactionsPerCustomer,
      customersByTier: customersByTier,
      topSpenders: topSpenders.take(10).toList(),
      customerRetentionRate: customerRetentionRate,
    );
  }

  /// Generate a complete report
  Future<ReportEntity> generateReport(
    ReportType type,
    DateRange dateRange, {
    Map<String, dynamic> filters = const {},
    String? title,
    String? description,
  }) async {
    Map<String, dynamic> reportData;

    switch (type) {
      case ReportType.salesSummary:
      case ReportType.salesTrends:
        final analytics = await generateSalesAnalytics(dateRange);
        reportData = analytics.toMap();
        break;
      case ReportType.productPerformance:
        final analytics = await generateProductAnalytics(dateRange);
        reportData = {
          'products': analytics.map((p) => p.toMap()).toList(),
          'totalProducts': analytics.length,
          'topProducts': analytics.take(10).map((p) => p.toMap()).toList(),
        };
        break;
      case ReportType.inventoryMovement:
        final analytics = await generateInventoryAnalytics(dateRange);
        reportData = analytics.toMap();
        break;
      case ReportType.customerAnalytics:
        final analytics = await generateCustomerAnalytics(dateRange);
        reportData = analytics.toMap();
        break;
      case ReportType.employeePerformance:
        // TODO: Implement employee performance analytics
        reportData = {'message': 'Employee performance analytics not implemented yet'};
        break;
      case ReportType.profitMargin:
        final salesAnalytics = await generateSalesAnalytics(dateRange);
        final productAnalytics = await generateProductAnalytics(dateRange);
        final totalCost = productAnalytics.fold(0.0, (sum, p) => sum + p.totalCost);
        final grossProfit = salesAnalytics.netSales - totalCost;
        final profitMargin = salesAnalytics.netSales > 0 ? (grossProfit / salesAnalytics.netSales) : 0;
        
        reportData = {
          'grossRevenue': salesAnalytics.totalSales,
          'netRevenue': salesAnalytics.netSales,
          'totalCost': totalCost,
          'grossProfit': grossProfit,
          'profitMargin': profitMargin,
          'profitByCategory': {}, // TODO: Calculate by category
        };
        break;
      case ReportType.taxReport:
        final analytics = await generateSalesAnalytics(dateRange);
        reportData = {
          'totalTax': analytics.totalTax,
          'taxableAmount': analytics.netSales,
          'taxRate': analytics.netSales > 0 ? analytics.totalTax / analytics.netSales : 0,
          'transactionCount': analytics.transactionCount,
        };
        break;
      case ReportType.customReport:
        // Handle custom report based on filters
        reportData = await _generateCustomReport(dateRange, filters);
        break;
    }

    final reportTitle = title ?? _getDefaultReportTitle(type, dateRange);
    final reportDescription = description ?? _getDefaultReportDescription(type, dateRange);

    return ReportEntity(
      id: DateTime.now().millisecondsSinceEpoch.toString(),
      type: type,
      title: reportTitle,
      description: reportDescription,
      dateRange: dateRange,
      filters: filters,
      data: reportData,
      generatedAt: DateTime.now(),
      generatedBy: 'System', // TODO: Get current user
    );
  }

  // Private helper methods

  Future<List<SalesTransactionEntity>> _getTransactionsInRange(DateRange dateRange) async {
    // TODO: Implement actual database query
    // For now, return mock data
    return _generateMockTransactions(dateRange);
  }

  Future<List<ProductEntity>> _getAllProducts() async {
    // TODO: Implement actual database query
    return _generateMockProducts();
  }

  Future<List<CustomerEntity>> _getAllCustomers() async {
    // TODO: Implement actual database query
    return _generateMockCustomers();
  }

  List<HourlySalesData> _generateHourlySalesData(List<SalesTransactionEntity> transactions) {
    Map<int, HourlySalesBuilder> hourlyData = {};

    // Initialize all hours
    for (int hour = 0; hour < 24; hour++) {
      hourlyData[hour] = HourlySalesBuilder(hour);
    }

    // Process transactions
    for (final transaction in transactions) {
      if (transaction.status == TransactionStatus.completed) {
        final hour = transaction.createdAt.hour;
        hourlyData[hour]?.addTransaction(transaction.totalAmount / 100);
      }
    }

    return hourlyData.values.map((builder) => builder.build()).toList()
      ..sort((a, b) => a.hour.compareTo(b.hour));
  }

  Future<List<StockMovementData>> _generateStockMovementData(DateRange dateRange) async {
    // TODO: Implement actual stock movement query
    // Generate mock data for now
    final List<StockMovementData> movements = [];
    final days = dateRange.duration.inDays;
    
    for (int i = 0; i < days; i++) {
      final date = dateRange.startDate.add(Duration(days: i));
      movements.add(StockMovementData(
        date: date,
        inbound: (i % 7 == 0) ? 50 + (i % 20) : 0, // Deliveries on "Mondays"
        outbound: 20 + (i % 15), // Daily sales
        adjustments: (i % 10 == 0) ? (-5 + (i % 3)) : 0, // Occasional adjustments
      ));
    }

    return movements;
  }

  Future<Map<String, dynamic>> _generateCustomReport(DateRange dateRange, Map<String, dynamic> filters) async {
    // TODO: Implement custom report generation based on filters
    return {
      'message': 'Custom report generation not implemented yet',
      'filters': filters,
    };
  }

  String _getPaymentMethodName(PaymentMethod method) {
    switch (method) {
      case PaymentMethod.cash:
        return 'Cash';
      case PaymentMethod.card:
        return 'Card';
      case PaymentMethod.contactless:
        return 'Contactless';
      case PaymentMethod.qrCode:
        return 'QR Code';
      case PaymentMethod.bankTransfer:
        return 'Bank Transfer';
      case PaymentMethod.mobileMoney:
        return 'Mobile Money';
      case PaymentMethod.loyaltyPoints:
        return 'Loyalty Points';
    }
  }

  String _getLoyaltyTierName(LoyaltyTier tier) {
    switch (tier) {
      case LoyaltyTier.none:
        return 'None';
      case LoyaltyTier.bronze:
        return 'Bronze';
      case LoyaltyTier.silver:
        return 'Silver';
      case LoyaltyTier.gold:
        return 'Gold';
      case LoyaltyTier.platinum:
        return 'Platinum';
      case LoyaltyTier.diamond:
        return 'Diamond';
    }
  }

  String _getDefaultReportTitle(ReportType type, DateRange dateRange) {
    final period = _formatDateRange(dateRange);
    switch (type) {
      case ReportType.salesSummary:
        return 'Sales Summary - $period';
      case ReportType.salesTrends:
        return 'Sales Trends - $period';
      case ReportType.productPerformance:
        return 'Product Performance - $period';
      case ReportType.inventoryMovement:
        return 'Inventory Movement - $period';
      case ReportType.customerAnalytics:
        return 'Customer Analytics - $period';
      case ReportType.employeePerformance:
        return 'Employee Performance - $period';
      case ReportType.profitMargin:
        return 'Profit Margin Analysis - $period';
      case ReportType.taxReport:
        return 'Tax Report - $period';
      case ReportType.customReport:
        return 'Custom Report - $period';
    }
  }

  String _getDefaultReportDescription(ReportType type, DateRange dateRange) {
    final period = _formatDateRange(dateRange);
    switch (type) {
      case ReportType.salesSummary:
        return 'Comprehensive sales overview for $period';
      case ReportType.salesTrends:
        return 'Sales performance trends and patterns for $period';
      case ReportType.productPerformance:
        return 'Detailed product sales analysis for $period';
      case ReportType.inventoryMovement:
        return 'Inventory movement and stock analysis for $period';
      case ReportType.customerAnalytics:
        return 'Customer behavior and spending analysis for $period';
      case ReportType.employeePerformance:
        return 'Employee performance metrics for $period';
      case ReportType.profitMargin:
        return 'Profitability analysis for $period';
      case ReportType.taxReport:
        return 'Tax summary and compliance report for $period';
      case ReportType.customReport:
        return 'Custom analysis report for $period';
    }
  }

  String _formatDateRange(DateRange dateRange) {
    final start = dateRange.startDate;
    final end = dateRange.endDate;
    
    if (start.year == end.year && start.month == end.month && start.day == end.day) {
      return '${start.day}/${start.month}/${start.year}';
    } else if (start.year == end.year && start.month == end.month) {
      return '${start.day}-${end.day}/${start.month}/${start.year}';
    } else {
      return '${start.day}/${start.month}/${start.year} - ${end.day}/${end.month}/${end.year}';
    }
  }

  // Mock data generators
  List<SalesTransactionEntity> _generateMockTransactions(DateRange dateRange) {
    final transactions = <SalesTransactionEntity>[];
    final random = DateTime.now().millisecondsSinceEpoch % 1000;
    
    final days = dateRange.duration.inDays.clamp(1, 30);
    for (int day = 0; day < days; day++) {
      final transactionDate = dateRange.startDate.add(Duration(days: day));
      final transactionsPerDay = 8 + (day % 12); // 8-20 transactions per day
      
      for (int i = 0; i < transactionsPerDay; i++) {
        final hour = 9 + (i % 12); // Business hours 9-21
        final minute = (i * 7) % 60;
        final createdAt = DateTime(transactionDate.year, transactionDate.month, transactionDate.day, hour, minute);
        
        transactions.add(SalesTransactionEntity(
          id: '${day}_$i',
          receiptNumber: 'R${day.toString().padLeft(3, '0')}${i.toString().padLeft(2, '0')}',
          customerId: (i % 4 == 0) ? 'cust_${i % 10}' : null,
          employeeId: 'emp_${i % 3 + 1}',
          employeeName: ['John Doe', 'Jane Smith', 'Bob Johnson'][i % 3],
          locationId: 'loc_1',
          items: [
            SalesTransactionItemEntity(
              id: '${day}_${i}_1',
              productId: 'prod_${i % 20 + 1}',
              productName: 'Product ${i % 20 + 1}',
              sku: 'SKU${(i % 20 + 1).toString().padLeft(3, '0')}',
              displayName: 'Product ${i % 20 + 1}',
              quantity: 1 + (i % 3),
              unitPrice: (1000 + (i % 50) * 100), // $10-$60
              discountAmount: (i % 5 == 0) ? 200 : 0, // $2 discount sometimes
              taxAmount: 100,
              cost: 500 + (i % 30) * 50,
            ),
          ],
          subtotal: (1000 + (i % 50) * 100) * (1 + (i % 3)),
          taxAmount: 100 * (1 + (i % 3)),
          discountAmount: (i % 5 == 0) ? 200 : 0,
          totalAmount: (1100 + (i % 50) * 100) * (1 + (i % 3)) - ((i % 5 == 0) ? 200 : 0),
          payments: [
            PaymentEntity(
              id: '${day}_${i}_pay',
              transactionId: '${day}_$i',
              amount: (1100 + (i % 50) * 100) * (1 + (i % 3)) - ((i % 5 == 0) ? 200 : 0),
              method: PaymentMethod.values[i % 3],
              status: PaymentStatus.completed,
              reference: 'PAY${day}${i}',
              createdAt: createdAt,
            ),
          ],
          status: (i % 20 == 0) ? TransactionStatus.returned : TransactionStatus.completed,
          createdAt: createdAt,
          updatedAt: createdAt,
        ));
      }
    }
    
    return transactions;
  }

  List<ProductEntity> _generateMockProducts() {
    return List.generate(50, (index) {
      final id = 'prod_${index + 1}';
      return ProductEntity(
        id: id,
        name: 'Product ${index + 1}',
        description: 'Description for product ${index + 1}',
        sku: 'SKU${(index + 1).toString().padLeft(3, '0')}',
        barcode: '123456789${index.toString().padLeft(3, '0')}',
        categoryId: 'cat_${(index % 5) + 1}',
        categoryName: ['Electronics', 'Clothing', 'Food', 'Books', 'Home'][index % 5],
        sellingPrice: (1000 + (index % 50) * 100),
        costPrice: (500 + (index % 30) * 50),
        stock: 10 + (index % 100),
        lowStockThreshold: 5,
        isActive: true,
        createdAt: DateTime.now().subtract(Duration(days: index % 100)),
        updatedAt: DateTime.now(),
      );
    });
  }

  List<CustomerEntity> _generateMockCustomers() {
    return List.generate(100, (index) {
      return CustomerEntity(
        id: 'cust_$index',
        name: 'Customer ${index + 1}',
        phone: '+123456789${index.toString().padLeft(2, '0')}',
        loyaltyTier: LoyaltyTier.values[index % LoyaltyTier.values.length],
        loyaltyPoints: (index * 25) % 1000,
        createdAt: DateTime.now().subtract(Duration(days: index % 365)),
        updatedAt: DateTime.now(),
      );
    });
  }
}

// Helper classes for building analytics

class ProductAnalyticsBuilder {
  final String productId;
  final String productName;
  final String categoryName;
  final int currentStock;
  
  int _quantitySold = 0;
  double _totalSales = 0;
  double _totalCost = 0;
  int _transactionCount = 0;
  int _returnsCount = 0;
  double _returnsAmount = 0;

  ProductAnalyticsBuilder({
    required this.productId,
    required this.productName,
    required this.categoryName,
    required this.currentStock,
  });

  void addSale({required int quantity, required double sellingPrice, required double cost, double discountAmount = 0}) {
    _quantitySold += quantity;
    _totalSales += (sellingPrice * quantity) - discountAmount;
    _totalCost += cost * quantity;
    _transactionCount++;
  }

  void addReturn({required int quantity, required double amount}) {
    _returnsCount += quantity;
    _returnsAmount += amount;
  }

  ProductAnalytics build() {
    final grossProfit = _totalSales - _totalCost;
    final profitMargin = _totalSales > 0 ? grossProfit / _totalSales : 0;
    final averageSellingPrice = _quantitySold > 0 ? _totalSales / _quantitySold : 0;

    return ProductAnalytics(
      productId: productId,
      productName: productName,
      categoryName: categoryName,
      quantitySold: _quantitySold,
      totalSales: _totalSales,
      totalCost: _totalCost,
      grossProfit: grossProfit,
      profitMargin: profitMargin,
      transactionCount: _transactionCount,
      averageSellingPrice: averageSellingPrice,
      returnsCount: _returnsCount,
      returnsAmount: _returnsAmount,
      currentStock: currentStock,
      stockMovements: _quantitySold + _returnsCount, // Simplified
    );
  }
}

class CustomerSpendBuilder {
  final String customerId;
  final String customerName;
  
  double _totalSpend = 0;
  int _transactionCount = 0;

  CustomerSpendBuilder({
    required this.customerId,
    required this.customerName,
  });

  void addTransaction(double amount) {
    _totalSpend += amount;
    _transactionCount++;
  }

  CustomerSpendData build() {
    return CustomerSpendData(
      customerId: customerId,
      customerName: customerName,
      totalSpend: _totalSpend,
      transactionCount: _transactionCount,
    );
  }
}

class HourlySalesBuilder {
  final int hour;
  double _sales = 0;
  int _transactions = 0;

  HourlySalesBuilder(this.hour);

  void addTransaction(double amount) {
    _sales += amount;
    _transactions++;
  }

  HourlySalesData build() {
    return HourlySalesData(
      hour: hour,
      sales: _sales,
      transactions: _transactions,
    );
  }
}
