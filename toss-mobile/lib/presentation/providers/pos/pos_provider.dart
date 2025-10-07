import 'package:flutter/material.dart';
import 'dart:math' as math;
import '../../../domain/entities/sale_entity.dart';
import '../../../domain/entities/payment_entity.dart';
import '../../../domain/entities/receipt_entity.dart';
import '../../../domain/entities/stock_entity.dart';
import '../../../domain/usecases/pos_usecases.dart';

class POSProvider extends ChangeNotifier {
  final CreateSale _createSale;
  final UpdateSale _updateSale;
  final CompleteSale _completeSale;
  final CreatePayment _createPayment;
  final ProcessSplitPayment _processSplitPayment;
  final CreateReceipt _createReceipt;
  final GenerateReceiptPdf _generateReceiptPdf;
  final EmailReceipt _emailReceipt;
  final GetSalesByDateRange _getSalesByDateRange;
  final GetTodaySales _getTodaySales;

  POSProvider(
    this._createSale,
    this._updateSale,
    this._completeSale,
    this._createPayment,
    this._processSplitPayment,
    this._createReceipt,
    this._generateReceiptPdf,
    this._emailReceipt,
    this._getSalesByDateRange,
    this._getTodaySales,
  );

  // Cart state
  List<SaleItemEntity> _cartItems = [];
  int? _selectedCustomerId;
  String? _selectedCustomerName;
  String? _selectedCustomerPhone;
  String? _selectedCustomerEmail;
  
  // Discount and tax
  int _cartDiscount = 0; // In cents
  double _taxRate = 0.15; // 15% VAT by default
  
  // Payment state
  List<PaymentEntity> _payments = [];
  
  // Transaction state
  SaleEntity? _currentSale;
  ReceiptEntity? _currentReceipt;
  bool _isProcessing = false;
  String? _error;
  
  // Today's sales summary
  List<SaleEntity> _todaySales = [];
  int _todayRevenue = 0;
  int _todayTransactionCount = 0;

  // Getters
  List<SaleItemEntity> get cartItems => _cartItems;
  int? get selectedCustomerId => _selectedCustomerId;
  String? get selectedCustomerName => _selectedCustomerName;
  String? get selectedCustomerPhone => _selectedCustomerPhone;
  String? get selectedCustomerEmail => _selectedCustomerEmail;
  int get cartDiscount => _cartDiscount;
  double get taxRate => _taxRate;
  List<PaymentEntity> get payments => _payments;
  SaleEntity? get currentSale => _currentSale;
  ReceiptEntity? get currentReceipt => _currentReceipt;
  bool get isProcessing => _isProcessing;
  String? get error => _error;
  List<SaleEntity> get todaySales => _todaySales;
  int get todayRevenue => _todayRevenue;
  int get todayTransactionCount => _todayTransactionCount;

  // Cart calculations
  int get cartSubtotal {
    return _cartItems.fold(0, (sum, item) => sum + item.lineTotal);
  }

  int get cartTaxAmount {
    final taxableAmount = cartSubtotal - _cartDiscount;
    return (taxableAmount * _taxRate).round();
  }

  int get cartTotal {
    return cartSubtotal - _cartDiscount + cartTaxAmount;
  }

  int get paymentTotal {
    return _payments.fold(0, (sum, payment) => sum + payment.amount);
  }

  int get remainingAmount {
    return cartTotal - paymentTotal;
  }

  bool get isFullyPaid => remainingAmount <= 0;

  int get itemCount {
    return _cartItems.fold(0, (sum, item) => sum + item.quantity);
  }

  // Cart management
  void addItem(StockEntity product, int quantity) {
    final existingIndex = _cartItems.indexWhere(
      (item) => item.productId == product.id,
    );

    final lineTotal = product.sellingPrice * quantity;

    if (existingIndex >= 0) {
      final existingItem = _cartItems[existingIndex];
      _cartItems[existingIndex] = existingItem.copyWith(
        quantity: existingItem.quantity + quantity,
        lineTotal: existingItem.lineTotal + lineTotal,
      );
    } else {
      _cartItems.add(SaleItemEntity(
        productId: product.id,
        productName: product.name,
        productSku: product.sku,
        quantity: quantity,
        unitPrice: product.sellingPrice,
        lineTotal: lineTotal,
      ));
    }

    notifyListeners();
  }

  void updateItemQuantity(int index, int quantity) {
    if (index < 0 || index >= _cartItems.length) return;

    if (quantity <= 0) {
      removeItem(index);
      return;
    }

    final item = _cartItems[index];
    _cartItems[index] = item.copyWith(
      quantity: quantity,
      lineTotal: item.unitPrice * quantity - item.discount,
    );

    notifyListeners();
  }

  void updateItemDiscount(int index, int discount) {
    if (index < 0 || index >= _cartItems.length) return;

    final item = _cartItems[index];
    _cartItems[index] = item.copyWith(
      discount: discount,
      lineTotal: (item.unitPrice * item.quantity) - discount,
    );

    notifyListeners();
  }

  void removeItem(int index) {
    if (index >= 0 && index < _cartItems.length) {
      _cartItems.removeAt(index);
      notifyListeners();
    }
  }

  void clearCart() {
    _cartItems.clear();
    _cartDiscount = 0;
    _payments.clear();
    _selectedCustomerId = null;
    _selectedCustomerName = null;
    _selectedCustomerPhone = null;
    _selectedCustomerEmail = null;
    _currentSale = null;
    _currentReceipt = null;
    _error = null;
    notifyListeners();
  }

  // Customer selection
  void selectCustomer(int? customerId, String? name, String? phone, String? email) {
    _selectedCustomerId = customerId;
    _selectedCustomerName = name;
    _selectedCustomerPhone = phone;
    _selectedCustomerEmail = email;
    notifyListeners();
  }

  void clearCustomer() {
    _selectedCustomerId = null;
    _selectedCustomerName = null;
    _selectedCustomerPhone = null;
    _selectedCustomerEmail = null;
    notifyListeners();
  }

  // Discount management
  void setCartDiscount(int discount) {
    _cartDiscount = discount;
    notifyListeners();
  }

  void setCartDiscountPercentage(double percentage) {
    _cartDiscount = (cartSubtotal * percentage / 100).round();
    notifyListeners();
  }

  void setTaxRate(double rate) {
    _taxRate = rate;
    notifyListeners();
  }

  // Payment management
  void addPayment(PaymentMethod method, int amount, {
    String? referenceNumber,
    String? cardLast4,
    String? mobileMoneyProvider,
  }) {
    final payment = PaymentEntity(
      method: method,
      amount: amount,
      referenceNumber: referenceNumber,
      cardLast4: cardLast4,
      mobileMoneyProvider: mobileMoneyProvider,
      paymentDate: DateTime.now(),
      createdAt: DateTime.now(),
      status: PaymentStatus.completed,
    );

    _payments.add(payment);
    notifyListeners();
  }

  void removePayment(int index) {
    if (index >= 0 && index < _payments.length) {
      _payments.removeAt(index);
      notifyListeners();
    }
  }

  void clearPayments() {
    _payments.clear();
    notifyListeners();
  }

  // Transaction processing
  Future<bool> processTransaction({
    int? cashierId,
    String? cashierName,
    int? warehouseId,
    String? warehouseName,
  }) async {
    if (_cartItems.isEmpty) {
      _error = 'Cart is empty';
      notifyListeners();
      return false;
    }

    if (!isFullyPaid) {
      _error = 'Payment incomplete. Remaining: R${(remainingAmount / 100).toStringAsFixed(2)}';
      notifyListeners();
      return false;
    }

    _isProcessing = true;
    _error = null;
    notifyListeners();

    try {
      // Generate sale number
      final saleNumber = 'SALE-${DateTime.now().millisecondsSinceEpoch}';

      // Create sale entity
      final sale = SaleEntity(
        saleNumber: saleNumber,
        status: SaleStatus.completed,
        type: SaleType.regular,
        customerId: _selectedCustomerId,
        customerName: _selectedCustomerName,
        customerPhone: _selectedCustomerPhone,
        customerEmail: _selectedCustomerEmail,
        items: _cartItems,
        subtotal: cartSubtotal,
        discountAmount: _cartDiscount,
        taxAmount: cartTaxAmount,
        totalAmount: cartTotal,
        warehouseId: warehouseId,
        warehouseName: warehouseName,
        cashierId: cashierId,
        cashierName: cashierName,
        saleDate: DateTime.now(),
        createdAt: DateTime.now(),
      );

      // Create sale
      final saleResult = await _createSale(sale);
      if (!saleResult.isSuccess || saleResult.data == null) {
        throw Exception(saleResult.error?.message ?? 'Failed to create sale');
      }

      final saleId = saleResult.data!;
      _currentSale = sale.copyWith(id: saleId);

      // Process payments
      for (var payment in _payments) {
        final paymentWithSale = payment.copyWith(
          saleId: saleId,
          saleNumber: saleNumber,
        );
        
        final paymentResult = await _createPayment(paymentWithSale);
        if (!paymentResult.isSuccess) {
          throw Exception(paymentResult.error?.message ?? 'Failed to process payment');
        }
      }

      // Generate receipt
      final receipt = ReceiptEntity(
        receiptNumber: 'RCP-${DateTime.now().millisecondsSinceEpoch}',
        type: ReceiptType.sale,
        saleId: saleId,
        sale: _currentSale,
        payments: _payments,
        customerName: _selectedCustomerName,
        customerPhone: _selectedCustomerPhone,
        customerEmail: _selectedCustomerEmail,
        cashierName: cashierName,
        storeName: warehouseName,
        receiptDate: DateTime.now(),
        createdAt: DateTime.now(),
      );

      final receiptResult = await _createReceipt(receipt);
      if (receiptResult.isSuccess && receiptResult.data != null) {
        _currentReceipt = receipt.copyWith(id: receiptResult.data);
      }

      _isProcessing = false;
      notifyListeners();
      return true;
    } catch (e) {
      _error = e.toString();
      _isProcessing = false;
      notifyListeners();
      return false;
    }
  }

  // Receipt operations
  Future<String?> generateReceiptPdf() async {
    if (_currentReceipt?.id == null) return null;

    final result = await _generateReceiptPdf(_currentReceipt!.id!);
    if (result.isSuccess) {
      return result.data;
    }
    
    _error = result.error?.message ?? 'Failed to generate receipt PDF';
    notifyListeners();
    return null;
  }

  Future<bool> emailReceiptToCustomer() async {
    if (_currentReceipt?.id == null || _selectedCustomerEmail == null) {
      _error = 'No receipt or customer email available';
      notifyListeners();
      return false;
    }

    final result = await _emailReceipt(EmailReceiptParams(
      receiptId: _currentReceipt!.id!,
      email: _selectedCustomerEmail!,
    ));

    if (result.isSuccess) {
      return true;
    }

    _error = result.error?.message ?? 'Failed to email receipt';
    notifyListeners();
    return false;
  }

  // Today's sales
  Future<void> loadTodaySales() async {
    final today = DateTime.now();
    final startOfDay = DateTime(today.year, today.month, today.day);
    final endOfDay = DateTime(today.year, today.month, today.day, 23, 59, 59);

    final result = await _getSalesByDateRange(DateRangeParams(
      startDate: startOfDay,
      endDate: endOfDay,
    ));

    if (result.isSuccess && result.data != null) {
      _todaySales = result.data!;
      _todayTransactionCount = _todaySales.length;
      _todayRevenue = _todaySales.fold(0, (sum, sale) => sum + sale.totalAmount);
      notifyListeners();
    }
  }

  // Quick actions
  void applyQuickDiscount(double percentage) {
    setCartDiscountPercentage(percentage);
  }

  void splitPaymentEqually(int numberOfPayments) {
    _payments.clear();
    final amountPerPayment = (cartTotal / numberOfPayments).round();
    
    for (int i = 0; i < numberOfPayments; i++) {
      addPayment(
        PaymentMethod.cash,
        i == numberOfPayments - 1
            ? cartTotal - (amountPerPayment * (numberOfPayments - 1))
            : amountPerPayment,
      );
    }
  }
}

