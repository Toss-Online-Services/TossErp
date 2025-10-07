import 'dart:async';

import '../../domain/entities/discount_entity.dart';
import '../../domain/entities/sales_transaction_entity.dart';
import '../../domain/entities/product_entity.dart';
import '../../domain/entities/customer_entity.dart';
// Using an internal mock repository defined below for now

class DiscountService {
  static final DiscountService _instance = DiscountService._internal();
  factory DiscountService() => _instance;
  DiscountService._internal();

  final DiscountRepository _discountRepository = DiscountRepository();
  final StreamController<List<DiscountEntity>> _discountController = 
      StreamController<List<DiscountEntity>>.broadcast();

  Stream<List<DiscountEntity>> get discountsStream => _discountController.stream;

  // Get all active discounts
  Future<List<DiscountEntity>> getActiveDiscounts({
    String? locationId,
    String? customerId,
  }) async {
    try {
      final discounts = await _discountRepository.getActiveDiscounts(
        locationId: locationId,
        customerId: customerId,
      );

      final now = DateTime.now();
      final validDiscounts = discounts.where((discount) {
        // Check date validity
        if (!discount.isValidForDate(now)) return false;
        
        // Check location validity
        if (locationId != null && discount.locationConfig != null) {
          if (!discount.locationConfig!.isValidForLocation(locationId)) return false;
        }
        
        // Check time-based validity
        if (discount.timeConfig != null) {
          if (!discount.timeConfig!.isValidForDateTime(now)) return false;
        }
        
        return true;
      }).toList();

      // Sort by priority (higher priority first)
      validDiscounts.sort((a, b) => b.priority.compareTo(a.priority));

      return validDiscounts;
    } catch (e) {
      throw Exception('Failed to get active discounts: $e');
    }
  }

  // Apply discounts to a transaction
  Future<DiscountCalculationResult> calculateDiscounts({
    required SalesTransactionEntity transaction,
    required List<ProductEntity> products,
    CustomerEntity? customer,
    String? couponCode,
    String? locationId,
  }) async {
    try {
      final activeDiscounts = await getActiveDiscounts(
        locationId: locationId,
        customerId: customer?.id,
      );

      final applicableDiscounts = <DiscountEntity>[];
      final appliedDiscounts = <AppliedDiscount>[];
      double totalDiscount = 0.0;
      
      // Filter discounts based on application method
      for (final discount in activeDiscounts) {
        if (await _isDiscountApplicable(
          discount: discount,
          transaction: transaction,
          products: products,
          customer: customer,
          couponCode: couponCode,
        )) {
          applicableDiscounts.add(discount);
        }
      }

      // Apply discounts based on priority and stackability
      final itemDiscounts = <String, double>{}; // productId -> discount amount
      final cartDiscounts = <AppliedDiscount>[];

      for (final discount in applicableDiscounts) {
        if (discount.scope == DiscountScope.item ||
            discount.scope == DiscountScope.category ||
            discount.scope == DiscountScope.brand) {
          
          final itemDiscount = await _applyItemDiscount(
            discount: discount,
            transaction: transaction,
            products: products,
          );
          
          if (itemDiscount != null) {
            appliedDiscounts.add(itemDiscount);
            totalDiscount += itemDiscount.discountAmount;
            
            // Track item-level discounts
            for (final itemId in itemDiscount.appliedToItems) {
              itemDiscounts[itemId] = (itemDiscounts[itemId] ?? 0) + 
                  (itemDiscount.discountAmount / itemDiscount.appliedToItems.length);
            }
          }
        } else if (discount.scope == DiscountScope.cart ||
                   discount.scope == DiscountScope.total) {
          
          // Check if cart-level discounts can stack
          if (discount.isStackable || cartDiscounts.isEmpty) {
            final cartDiscount = await _applyCartDiscount(
              discount: discount,
              transaction: transaction,
              currentDiscount: totalDiscount,
            );
            
            if (cartDiscount != null) {
              appliedDiscounts.add(cartDiscount);
              cartDiscounts.add(cartDiscount);
              totalDiscount += cartDiscount.discountAmount;
            }
          }
        }
        
        // Break if non-stackable discount is applied
        if (!discount.isStackable && appliedDiscounts.isNotEmpty) {
          break;
        }
      }

      return DiscountCalculationResult(
        appliedDiscounts: appliedDiscounts,
        totalDiscount: totalDiscount,
        itemDiscounts: itemDiscounts,
        originalTotal: transaction.subtotal.toDouble(),
        discountedTotal: transaction.subtotal.toDouble() - totalDiscount,
      );
    } catch (e) {
      throw Exception('Failed to calculate discounts: $e');
    }
  }

  // Check if discount is applicable
  Future<bool> _isDiscountApplicable({
    required DiscountEntity discount,
    required SalesTransactionEntity transaction,
    required List<ProductEntity> products,
    CustomerEntity? customer,
    String? couponCode,
  }) async {
    // Check coupon code
    if (discount.application == DiscountApplication.couponCode) {
      if (discount.couponCode != couponCode) return false;
    }

    // Check customer eligibility
    if (customer != null && !discount.isValidForCustomer(customer.id)) {
      return false;
    }

    // Check minimum purchase
    if (discount.minimumPurchase != null && 
        transaction.subtotal < discount.minimumPurchase!) {
      return false;
    }

    // Check usage limits
    if (customer != null) {
      final customerUsage = await _discountRepository.getCustomerUsageCount(
        discount.id,
        customer.id,
      );
      if (!discount.canBeUsed(customerUsage)) {
        return false;
      }
    }

    // Check if applicable items/categories/brands are in the transaction
    if (discount.scope == DiscountScope.item ||
        discount.scope == DiscountScope.category ||
        discount.scope == DiscountScope.brand) {
      
      return _hasApplicableProducts(discount, transaction, products);
    }

    return true;
  }

  // Check if transaction has applicable products
  bool _hasApplicableProducts(
    DiscountEntity discount,
    SalesTransactionEntity transaction,
    List<ProductEntity> products,
  ) {
    for (final item in transaction.items) {
      final product = products.firstWhere(
        (p) => p.id == item.productId,
        orElse: () => ProductEntity(
          id: item.productId,
          createdById: 'system',
          name: item.productName,
          sku: null,
          barcode: null,
          imageUrl: '',
          stock: 0,
          price: item.unitPrice,
          costPrice: null,
          categoryId: null,
          createdAt: DateTime.now().toIso8601String(),
        ),
      );

      // Check if product is excluded
  if (discount.excludedItems.contains(product.id?.toString() ?? '')) continue;

      // Check applicable items
      if (discount.applicableItems.isNotEmpty) {
        if (discount.applicableItems.contains((product.id)?.toString() ?? '')) return true;
      }

      // Check applicable categories
      if (discount.applicableCategories.isNotEmpty) {
        if (discount.applicableCategories.contains(product.categoryId?.toString() ?? '')) return true;
      }

      // Check applicable brands
      if (discount.applicableBrands.isNotEmpty) {
        // No brandId on ProductEntity; skip this check
      }

      // If no specific applicability rules, discount applies to all
      if (discount.applicableItems.isEmpty &&
          discount.applicableCategories.isEmpty &&
          discount.applicableBrands.isEmpty) {
        return true;
      }
    }

    return false;
  }

  // Apply item-level discount
  Future<AppliedDiscount?> _applyItemDiscount({
    required DiscountEntity discount,
    required SalesTransactionEntity transaction,
    required List<ProductEntity> products,
  }) async {
    final applicableItems = <String>[];
    double discountAmount = 0.0;

    for (final item in transaction.items) {
      final product = products.firstWhere(
        (p) => p.id == item.productId,
        orElse: () => ProductEntity(
          id: item.productId,
          createdById: 'system',
          name: item.productName,
          sku: null,
          barcode: null,
          imageUrl: '',
          stock: 0,
          price: item.unitPrice,
          costPrice: null,
          categoryId: null,
          createdAt: DateTime.now().toIso8601String(),
        ),
      );

      if (_isProductApplicable(discount, product)) {
        applicableItems.add((item.id ?? item.productId).toString());
        
        if (discount.type == DiscountType.bogo && discount.bogoConfig != null) {
          discountAmount += _calculateBogoDiscount(item, discount.bogoConfig!);
        } else {
          discountAmount += _calculateItemDiscount(item, discount);
        }
      }
    }

    if (applicableItems.isEmpty) return null;

    // Apply maximum discount limit
    if (discount.maximumDiscount != null && 
        discountAmount > discount.maximumDiscount!) {
      discountAmount = discount.maximumDiscount!;
    }

    return AppliedDiscount(
      discountId: discount.id,
      discountName: discount.name,
      type: discount.type,
      originalValue: discount.value,
      appliedValue: discount.value,
      discountAmount: discountAmount,
      appliedToItems: applicableItems,
      couponCode: discount.couponCode,
      appliedAt: DateTime.now(),
    );
  }

  // Apply cart-level discount
  Future<AppliedDiscount?> _applyCartDiscount({
    required DiscountEntity discount,
    required SalesTransactionEntity transaction,
    required double currentDiscount,
  }) async {
    final applicableTotal = transaction.subtotal - currentDiscount;
    
    if (applicableTotal <= 0) return null;

    double discountAmount = 0.0;

    switch (discount.type) {
      case DiscountType.percentage:
        discountAmount = applicableTotal * (discount.value / 100);
        break;
      case DiscountType.fixedAmount:
        discountAmount = discount.value;
        if (discountAmount > applicableTotal) {
          discountAmount = applicableTotal;
        }
        break;
      default:
        return null;
    }

    // Apply maximum discount limit
    if (discount.maximumDiscount != null && 
        discountAmount > discount.maximumDiscount!) {
      discountAmount = discount.maximumDiscount!;
    }

    return AppliedDiscount(
      discountId: discount.id,
      discountName: discount.name,
      type: discount.type,
      originalValue: discount.value,
      appliedValue: discount.value,
      discountAmount: discountAmount,
    appliedToItems: transaction.items
      .map((item) => (item.id ?? item.productId).toString())
      .toList(),
      couponCode: discount.couponCode,
      appliedAt: DateTime.now(),
    );
  }

  // Check if product is applicable for discount
  bool _isProductApplicable(DiscountEntity discount, ProductEntity product) {
    // Check if product is excluded
    if (discount.excludedItems.contains(product.id?.toString() ?? '')) return false;

    // Check applicable items
    if (discount.applicableItems.isNotEmpty) {
      return discount.applicableItems.contains(product.id?.toString() ?? '');
    }

    // Check applicable categories
    if (discount.applicableCategories.isNotEmpty) {
      return discount.applicableCategories.contains(product.categoryId?.toString() ?? '');
    }

    // Check applicable brands
    if (discount.applicableBrands.isNotEmpty) {
      // No brandId field; consider brands unsupported in this model
      return false;
    }

    // If no specific rules, applies to all
    return discount.applicableItems.isEmpty &&
           discount.applicableCategories.isEmpty &&
           discount.applicableBrands.isEmpty;
  }

  // Calculate item discount amount
  double _calculateItemDiscount(SalesTransactionItemEntity item, DiscountEntity discount) {
    switch (discount.type) {
      case DiscountType.percentage:
        return item.totalPrice.toDouble() * (discount.value / 100);
      case DiscountType.fixedAmount:
        return discount.value * item.quantity;
      default:
        return 0.0;
    }
  }

  // Calculate BOGO discount
  double _calculateBogoDiscount(SalesTransactionItemEntity item, BogoConfig config) {
    if (item.quantity < config.buyQuantity) return 0.0;

    final eligibleSets = item.quantity ~/ config.buyQuantity;
    final freeItems = eligibleSets * config.getQuantity;
    final discountPerItem = item.unitPrice.toDouble() * (config.getDiscountPercent / 100);
    
    return freeItems * discountPerItem;
  }

  // Validate coupon code
  Future<DiscountValidationResult> validateCouponCode(
    String couponCode, {
    String? customerId,
    String? locationId,
  }) async {
    try {
      final discount = await _discountRepository.getDiscountByCouponCode(couponCode);
      
      if (discount == null) {
        return DiscountValidationResult(
          isValid: false,
          error: 'Invalid coupon code',
        );
      }

      final now = DateTime.now();
      
      // Check if discount is active and valid
      if (!discount.isValidForDate(now)) {
        return DiscountValidationResult(
          isValid: false,
          error: 'Coupon has expired or is not yet active',
        );
      }

      // Check location validity
      if (locationId != null && discount.locationConfig != null) {
        if (!discount.locationConfig!.isValidForLocation(locationId)) {
          return DiscountValidationResult(
            isValid: false,
            error: 'Coupon is not valid for this location',
          );
        }
      }

      // Check customer validity
      if (customerId != null && !discount.isValidForCustomer(customerId)) {
        return DiscountValidationResult(
          isValid: false,
          error: 'Coupon is not valid for this customer',
        );
      }

      // Check usage limits
      if (customerId != null) {
        final customerUsage = await _discountRepository.getCustomerUsageCount(
          discount.id,
          customerId,
        );
        if (!discount.canBeUsed(customerUsage)) {
          return DiscountValidationResult(
            isValid: false,
            error: 'Coupon usage limit exceeded',
          );
        }
      }

      // Check time-based validity
      if (discount.timeConfig != null) {
        if (!discount.timeConfig!.isValidForDateTime(now)) {
          return DiscountValidationResult(
            isValid: false,
            error: 'Coupon is not valid at this time',
          );
        }
      }

      return DiscountValidationResult(
        isValid: true,
        discount: discount,
      );
    } catch (e) {
      return DiscountValidationResult(
        isValid: false,
        error: 'Failed to validate coupon: $e',
      );
    }
  }

  // Record discount usage
  Future<void> recordDiscountUsage({
    required String discountId,
    required String transactionId,
    required double discountAmount,
    String? customerId,
    String? couponCode,
    required String locationId,
  }) async {
    try {
      final usage = DiscountUsage(
        id: DateTime.now().millisecondsSinceEpoch.toString(),
        discountId: discountId,
        customerId: customerId,
        transactionId: transactionId,
        discountAmount: discountAmount,
        couponCode: couponCode,
        usedAt: DateTime.now(),
        locationId: locationId,
      );

      await _discountRepository.recordUsage(usage);
      
      // Update discount usage count
      await _discountRepository.incrementUsageCount(discountId);
    } catch (e) {
      throw Exception('Failed to record discount usage: $e');
    }
  }

  // Discount management methods
  Future<DiscountEntity> createDiscount(DiscountEntity discount) async {
    try {
      final savedDiscount = await _discountRepository.saveDiscount(discount);
      await _refreshDiscounts();
      return savedDiscount;
    } catch (e) {
      throw Exception('Failed to create discount: $e');
    }
  }

  Future<DiscountEntity> updateDiscount(DiscountEntity discount) async {
    try {
      final updatedDiscount = await _discountRepository.updateDiscount(discount);
      await _refreshDiscounts();
      return updatedDiscount;
    } catch (e) {
      throw Exception('Failed to update discount: $e');
    }
  }

  Future<void> deleteDiscount(String discountId) async {
    try {
      await _discountRepository.deleteDiscount(discountId);
      await _refreshDiscounts();
    } catch (e) {
      throw Exception('Failed to delete discount: $e');
    }
  }

  Future<void> toggleDiscountStatus(String discountId, bool isActive) async {
    try {
      await _discountRepository.toggleDiscountStatus(discountId, isActive);
      await _refreshDiscounts();
    } catch (e) {
      throw Exception('Failed to toggle discount status: $e');
    }
  }

  Future<List<DiscountEntity>> getAllDiscounts({
    bool? isActive,
    DiscountType? type,
    String? searchQuery,
  }) async {
    try {
      return await _discountRepository.getAllDiscounts(
        isActive: isActive,
        type: type,
        searchQuery: searchQuery,
      );
    } catch (e) {
      throw Exception('Failed to get discounts: $e');
    }
  }

  Future<DiscountEntity?> getDiscountById(String discountId) async {
    try {
      return await _discountRepository.getDiscountById(discountId);
    } catch (e) {
      throw Exception('Failed to get discount: $e');
    }
  }

  Future<List<DiscountUsage>> getDiscountUsageHistory(
    String discountId, {
    DateTime? startDate,
    DateTime? endDate,
  }) async {
    try {
      return await _discountRepository.getUsageHistory(
        discountId,
        startDate: startDate,
        endDate: endDate,
      );
    } catch (e) {
      throw Exception('Failed to get usage history: $e');
    }
  }

  Future<void> _refreshDiscounts() async {
    try {
      final discounts = await getAllDiscounts();
      _discountController.add(discounts);
    } catch (e) {
      // Log error but don't throw
      print('Failed to refresh discounts: $e');
    }
  }

  void dispose() {
    _discountController.close();
  }
}

// Result classes
class DiscountCalculationResult {
  final List<AppliedDiscount> appliedDiscounts;
  final double totalDiscount;
  final Map<String, double> itemDiscounts; // productId -> discount amount
  final double originalTotal;
  final double discountedTotal;

  const DiscountCalculationResult({
    required this.appliedDiscounts,
    required this.totalDiscount,
    required this.itemDiscounts,
    required this.originalTotal,
    required this.discountedTotal,
  });
}

class DiscountValidationResult {
  final bool isValid;
  final String? error;
  final DiscountEntity? discount;

  const DiscountValidationResult({
    required this.isValid,
    this.error,
    this.discount,
  });
}

// Mock Discount Repository
class DiscountRepository {
  final List<DiscountEntity> _discounts = [];
  final List<DiscountUsage> _usageHistory = [];

  Future<List<DiscountEntity>> getActiveDiscounts({
    String? locationId,
    String? customerId,
  }) async {
    await Future.delayed(const Duration(milliseconds: 200));
    final now = DateTime.now();
    
    return _discounts.where((discount) {
      return discount.isActive && 
             discount.isValidForDate(now);
    }).toList();
  }

  Future<DiscountEntity?> getDiscountByCouponCode(String couponCode) async {
    await Future.delayed(const Duration(milliseconds: 200));
    try {
      return _discounts.firstWhere(
        (discount) => discount.couponCode == couponCode && discount.isActive,
      );
    } catch (e) {
      return null;
    }
  }

  Future<int> getCustomerUsageCount(String discountId, String customerId) async {
    await Future.delayed(const Duration(milliseconds: 100));
    return _usageHistory
        .where((usage) => usage.discountId == discountId && usage.customerId == customerId)
        .length;
  }

  Future<DiscountEntity> saveDiscount(DiscountEntity discount) async {
    await Future.delayed(const Duration(milliseconds: 300));
    _discounts.add(discount);
    return discount;
  }

  Future<DiscountEntity> updateDiscount(DiscountEntity discount) async {
    await Future.delayed(const Duration(milliseconds: 300));
    final index = _discounts.indexWhere((d) => d.id == discount.id);
    if (index != -1) {
      _discounts[index] = discount;
    }
    return discount;
  }

  Future<void> deleteDiscount(String discountId) async {
    await Future.delayed(const Duration(milliseconds: 200));
    _discounts.removeWhere((discount) => discount.id == discountId);
  }

  Future<void> toggleDiscountStatus(String discountId, bool isActive) async {
    await Future.delayed(const Duration(milliseconds: 200));
    final index = _discounts.indexWhere((d) => d.id == discountId);
    if (index != -1) {
      _discounts[index] = _discounts[index].copyWith(isActive: isActive);
    }
  }

  Future<List<DiscountEntity>> getAllDiscounts({
    bool? isActive,
    DiscountType? type,
    String? searchQuery,
  }) async {
    await Future.delayed(const Duration(milliseconds: 300));
    
    var filtered = List<DiscountEntity>.from(_discounts);
    
    if (isActive != null) {
      filtered = filtered.where((d) => d.isActive == isActive).toList();
    }
    
    if (type != null) {
      filtered = filtered.where((d) => d.type == type).toList();
    }
    
    if (searchQuery != null && searchQuery.isNotEmpty) {
      filtered = filtered.where((d) => 
        d.name.toLowerCase().contains(searchQuery.toLowerCase()) ||
        d.description.toLowerCase().contains(searchQuery.toLowerCase())).toList();
    }
    
    return filtered;
  }

  Future<DiscountEntity?> getDiscountById(String discountId) async {
    await Future.delayed(const Duration(milliseconds: 200));
    try {
      return _discounts.firstWhere((d) => d.id == discountId);
    } catch (e) {
      return null;
    }
  }

  Future<void> recordUsage(DiscountUsage usage) async {
    await Future.delayed(const Duration(milliseconds: 200));
    _usageHistory.add(usage);
  }

  Future<void> incrementUsageCount(String discountId) async {
    await Future.delayed(const Duration(milliseconds: 100));
    final index = _discounts.indexWhere((d) => d.id == discountId);
    if (index != -1) {
      _discounts[index] = _discounts[index].copyWith(
        currentUses: _discounts[index].currentUses + 1,
      );
    }
  }

  Future<List<DiscountUsage>> getUsageHistory(
    String discountId, {
    DateTime? startDate,
    DateTime? endDate,
  }) async {
    await Future.delayed(const Duration(milliseconds: 300));
    
    var filtered = _usageHistory.where((usage) => usage.discountId == discountId);
    
    if (startDate != null) {
      filtered = filtered.where((usage) => usage.usedAt.isAfter(startDate));
    }
    
    if (endDate != null) {
      filtered = filtered.where((usage) => usage.usedAt.isBefore(endDate));
    }
    
    return filtered.toList()..sort((a, b) => b.usedAt.compareTo(a.usedAt));
  }
}
