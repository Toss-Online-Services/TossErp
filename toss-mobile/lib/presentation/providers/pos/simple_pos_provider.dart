import 'package:flutter/foundation.dart';
import 'dart:math' as math;
import '../../../domain/entities/sale_entity.dart';
import '../../../domain/entities/payment_entity.dart';
import '../../../domain/entities/receipt_entity.dart';
import '../inventory/simple_inventory_provider.dart';

class SimplePOSProvider extends ChangeNotifier {
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

  // Cart actions
  void addItem(InventoryItem product, int quantity) {
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
        productName: product.productName,
        productSku: product.sku,
        quantity: quantity,
        unitPrice: product.sellingPrice,
        lineTotal: lineTotal,
      ));
    }
    notifyListeners();
  }

  void removeItem(int index) {
    if (index >= 0 && index < _cartItems.length) {
      _cartItems.removeAt(index);
      notifyListeners();
    }
  }

  void updateItemQuantity(int index, int newQuantity) {
    if (index >= 0 && index < _cartItems.length) {
      if (newQuantity <= 0) {
        removeItem(index);
      } else {
        final item = _cartItems[index];
        _cartItems[index] = item.copyWith(
          quantity: newQuantity,
          lineTotal: item.unitPrice * newQuantity,
        );
        notifyListeners();
      }
    }
  }

  void clearCart() {
    _cartItems.clear();
    _selectedCustomerId = null;
    _selectedCustomerName = null;
    _selectedCustomerPhone = null;
    _selectedCustomerEmail = null;
    _cartDiscount = 0;
    _payments.clear();
    _currentSale = null;
    _currentReceipt = null;
    _error = null;
    notifyListeners();
  }

  // Customer selection
  void selectCustomer({
    required int? customerId,
    required String? name,
    String? phone,
    String? email,
  }) {
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
  void setCartDiscount(int discountInCents) {
    _cartDiscount = discountInCents.clamp(0, cartSubtotal);
    notifyListeners();
  }

  void setCartDiscountPercentage(double percentage) {
    final discountAmount = (cartSubtotal * (percentage / 100)).round();
    setCartDiscount(discountAmount);
  }

  void clearDiscount() {
    _cartDiscount = 0;
    notifyListeners();
  }

  // Tax management
  void setTaxRate(double rate) {
    _taxRate = rate.clamp(0.0, 1.0);
    notifyListeners();
  }

  // Payment management
  void addPayment(PaymentEntity payment) {
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
    required int cashierId,
    required String cashierName,
    required int warehouseId,
    required String warehouseName,
  }) async {
    if (_cartItems.isEmpty) {
      _error = 'Cart is empty';
      notifyListeners();
      return false;
    }

    if (!isFullyPaid) {
      _error = 'Payment incomplete';
      notifyListeners();
      return false;
    }

    _isProcessing = true;
    _error = null;
    notifyListeners();

    try {
      // Mock transaction processing
      await Future.delayed(const Duration(seconds: 1));

      // Create sale entity
      _currentSale = SaleEntity(
        id: DateTime.now().millisecondsSinceEpoch,
        saleNumber: 'SALE-${DateTime.now().millisecondsSinceEpoch}',
        type: SaleType.regular,
        status: SaleStatus.completed,
        items: _cartItems,
        subtotal: cartSubtotal,
        discountAmount: _cartDiscount,
        taxAmount: cartTaxAmount,
        total: cartTotal,
        customerId: _selectedCustomerId,
        customerName: _selectedCustomerName,
        cashierId: cashierId,
        cashierName: cashierName,
        warehouseId: warehouseId,
        warehouseName: warehouseName,
        saleDate: DateTime.now(),
        createdAt: DateTime.now(),
      );

      // Update today's sales
      _todaySales.add(_currentSale!);
      _todayRevenue += cartTotal;
      _todayTransactionCount++;

      _isProcessing = false;
      notifyListeners();
      return true;
    } catch (e) {
      _error = 'Transaction failed: $e';
      _isProcessing = false;
      notifyListeners();
      return false;
    }
  }

  // Receipt generation
  Future<void> generateReceiptPdf() async {
    // Mock PDF generation
    await Future.delayed(const Duration(milliseconds: 500));
    debugPrint('Receipt PDF generated');
  }

  Future<void> emailReceiptToCustomer() async {
    if (_selectedCustomerEmail == null) {
      _error = 'Customer email not provided';
      notifyListeners();
      return;
    }

    // Mock email sending
    await Future.delayed(const Duration(milliseconds: 500));
    debugPrint('Receipt emailed to $_selectedCustomerEmail');
  }

  // Today's sales
  Future<void> loadTodaySales() async {
    // Mock loading today's sales
    await Future.delayed(const Duration(milliseconds: 500));
    
    // Generate mock data
    _todaySales = [];
    _todayRevenue = 0;
    _todayTransactionCount = 0;
    
    notifyListeners();
  }

  // Error handling
  void clearError() {
    _error = null;
    notifyListeners();
  }
}
