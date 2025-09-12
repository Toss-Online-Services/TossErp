import 'package:flutter/foundation.dart';
import 'package:sliding_up_panel/sliding_up_panel.dart';

import '../../../app/services/auth/auth_service.dart';
import '../../../app/utilities/console_log.dart';
import '../../../core/errors/errors.dart';
import '../../../core/usecase/usecase.dart';
import '../../../domain/entities/discount_entity.dart';
import '../../../domain/entities/ordered_product_entity.dart';
import '../../../domain/entities/product_entity.dart';
import '../../../domain/entities/transaction_entity.dart';
import '../../../domain/repositories/transaction_repository.dart';
import '../../../domain/entities/payment_entity.dart';
import '../../../domain/entities/discount_entity.dart';
import '../../../data/repositories/payment_repository_impl.dart';
import '../../../data/repositories/discount_repository_impl.dart';
import '../../../data/repositories/appointment_repository_impl.dart';
import '../../../data/repositories/customer_repository_impl.dart';
import '../../../domain/entities/customer_entity.dart';
import '../../../domain/usecases/transaction_usecases.dart';
import '../../../service_locator.dart';
import '../products/products_provider.dart';

class HomeProvider extends ChangeNotifier {
  final TransactionRepository transactionRepository;

  HomeProvider({required this.transactionRepository});

  final panelController = PanelController();

  bool isPanelExpanded = false;

  List<OrderedProductEntity> orderedProducts = [];
  int receivedAmount = 0;
  String selectedPaymentMethod = 'cash';
  // Split payment amounts
  int cashAmount = 0;
  int bankAmount = 0;
  // Cart-level discount
  int discountAmount = 0;
  double? discountPercent;
  // Tax percent to add after discounts
  double? taxPercent;
  String? customerName;
  String? customerPhone;
  String? customerEmail;
  String? description;
  int? appointmentId;
  String? selectedCustomerId;
  int selectedCustomerPointsBalance = 0;
  int pointsToRedeem = 0;

  void resetStates() {
    isPanelExpanded = false;
    orderedProducts = [];
    receivedAmount = 0;
    selectedPaymentMethod = 'cash';
    cashAmount = 0;
    bankAmount = 0;
    discountAmount = 0;
    discountPercent = null;
    taxPercent = null;
    customerName = null;
    customerPhone = null;
    customerEmail = null;
    description = null;
    appointmentId = null;
    selectedCustomerId = null;
    selectedCustomerPointsBalance = 0;
    pointsToRedeem = 0;
  }

  Future<Result<int>> createTransaction() async {
    try {
      // Determine payment method and received based on split inputs
      final int calculatedReceived = (cashAmount + bankAmount) > 0 ? (cashAmount + bankAmount) : receivedAmount;
      final bool isSplit = cashAmount > 0 && bankAmount > 0;
      final String method = isSplit
          ? 'split'
          : (cashAmount > 0
              ? 'cash'
              : (bankAmount > 0
                  ? 'bank'
                  : selectedPaymentMethod));

      final int finalTotal = getFinalTotalAmount();

      var transaction = TransactionEntity(
        id: DateTime.now().millisecondsSinceEpoch,
        paymentMethod: method,
        customerName: customerName,
        customerPhone: customerPhone,
        description: _buildPaymentDescription(),
        orderedProducts: orderedProducts,
        createdById: AuthService().getAuthData()!.uid,
        receivedAmount: method == 'invoice' ? 0 : calculatedReceived,
        returnAmount: method == 'invoice' ? 0 : (calculatedReceived - finalTotal),
        totalOrderedProduct: orderedProducts.length,
        totalAmount: finalTotal,
      );

      var res = await CreateTransactionUsecase(transactionRepository).call(transaction);

      if (res.isSuccess && res.data != null) {
        final txnId = res.data!;
        // Persist split payments (cash/bank) if present and not invoice
        if (method != 'invoice') {
          try {
            if (cashAmount > 0) {
              await sl<PaymentRepositoryImpl>()
                  .createPayment(PaymentEntity(
                    transactionId: txnId, 
                    method: PaymentMethod.cash, 
                    amount: cashAmount,
                    createdAt: DateTime.now(),
                  ));
            }
            if (bankAmount > 0) {
              await sl<PaymentRepositoryImpl>()
                  .createPayment(PaymentEntity(
                    transactionId: txnId, 
                    method: PaymentMethod.bankTransfer, 
                    amount: bankAmount,
                    createdAt: DateTime.now(),
                  ));
            }
          } catch (e) {
            cl('[payments.save].error $e');
          }
        }

        // Persist cart-level discount if present
        try {
          if ((discountPercent != null && discountPercent! > 0) || (discountAmount > 0)) {
            final bool isPct = (discountPercent != null && discountPercent! > 0);
            final int value = isPct ? discountPercent!.round() : discountAmount;
            await sl<DiscountRepositoryImpl>()
                .createDiscount(DiscountEntity(
                  id: DateTime.now().millisecondsSinceEpoch.toString(),
                  name: 'Cart Discount',
                  description: 'Applied cart-level discount',
                  type: isPct ? DiscountType.percentage : DiscountType.fixedAmount,
                  scope: DiscountScope.cart,
                  application: DiscountApplication.manual,
                  value: value.toDouble(),
                  startDate: DateTime.now(),
                  endDate: DateTime.now().add(const Duration(days: 1)),
                  createdAt: DateTime.now(),
                  createdBy: 'system',
                  transactionId: txnId.toString(),
                ));
          }
        } catch (e) {
          cl('[discount.save].error $e');
        }

        // Link appointment if provided
        try {
          if (appointmentId != null) {
            await sl<AppointmentRepositoryImpl>().linkAppointmentToTransaction(appointmentId!, txnId);
          }
        } catch (e) {
          cl('[appointment.link].error $e');
        }

        // Loyalty: update points (earn minus redeemed)
        try {
          if (selectedCustomerId != null) {
            final repo = sl<CustomerRepositoryImpl>();
            final int redeemed = pointsToRedeem.clamp(0, selectedCustomerPointsBalance);
            final int netTotal = getFinalTotalAmount();
            final int earn = (netTotal / 100).floor();
            final int delta = earn - redeemed;
            if (delta != 0) {
              await repo.addPoints(selectedCustomerId!, delta);
            }
          }
        } catch (e) {
          cl('[loyalty.points].error $e');
        }
      }

      resetStates();
      panelController.close();

      // Refresh products
      sl<ProductsProvider>().getAllProducts();

      return res;
    } catch (e) {
      cl('[createTransaction].error $e');
      return Result.error(UnknownError(message: e.toString()));
    }
  }

  void onChangedIsPanelExpanded(bool val) {
    isPanelExpanded = val;
    notifyListeners();
  }

  void onAddOrderedProduct(ProductEntity product, int qty) {
    var currentIndex = orderedProducts.indexWhere((e) => e.productId == product.id);

    if (currentIndex != -1) {
      orderedProducts[currentIndex] = orderedProducts[currentIndex].copyWith(quantity: qty);
    } else {
      var order = OrderedProductEntity(
        id: DateTime.now().millisecondsSinceEpoch,
        productId: product.id!,
        quantity: qty,
        stock: product.stock,
        name: product.name,
        imageUrl: product.imageUrl,
        price: product.price,
      );

      orderedProducts.add(order);
    }

    notifyListeners();
  }

  void onRemoveOrderedProduct(OrderedProductEntity val) {
    orderedProducts.remove(val);
    notifyListeners();
  }

  void onRemoveAllOrderedProduct() {
    orderedProducts.clear();
    panelController.close();
    isPanelExpanded = false;
    notifyListeners();
  }

  void onClearCart() {
    orderedProducts.clear();
    notifyListeners();
  }

  void onChangedOrderedProductQuantity(int index, int value) {
    orderedProducts[index] = orderedProducts[index].copyWith(quantity: value);
    notifyListeners();
  }

  void onChangedReceivedAmount(int value) {
    receivedAmount = value;
    notifyListeners();
  }

  void onChangedPaymentMethod(String? value) {
    selectedPaymentMethod = value ?? selectedPaymentMethod;
    notifyListeners();
  }

  void onChangedCustomerName(String value) {
    customerName = value;
    notifyListeners();
  }

  void onChangedCustomerPhone(String value) {
    customerPhone = value;
    notifyListeners();
  }

  void onChangedCustomerEmail(String value) {
    customerEmail = value;
    notifyListeners();
  }

  void onChangedDescription(String value) {
    description = value;
    notifyListeners();
  }

  void onChangedCashAmount(int value) {
    cashAmount = value;
    notifyListeners();
  }

  void onChangedBankAmount(int value) {
    bankAmount = value;
    notifyListeners();
  }

  void onChangedDiscountAmount(int value) {
    discountAmount = value;
    notifyListeners();
  }

  void onChangedDiscountPercent(double? value) {
    discountPercent = value;
    notifyListeners();
  }

  void onChangedTaxPercent(double? value) {
    taxPercent = value;
    notifyListeners();
  }

  Future<Result<CustomerEntity>> findAndAttachCustomerByPhone(String phone, {String? name}) async {
    try {
      final repo = sl<CustomerRepositoryImpl>();
      final getRes = await repo.get(phone);
      CustomerEntity customer;
      if (getRes.isSuccess && getRes.data != null) {
        customer = getRes.data!;
      } else {
        final create = CustomerEntity(id: phone, phone: phone, name: name);
        final upsert = await repo.upsert(create);
        if (!upsert.isSuccess || upsert.data == null) return Result.error(UnknownError(message: 'Failed to attach'));
        customer = upsert.data!;
      }
      selectedCustomerId = customer.id;
      customerName = customer.name;
      customerPhone = customer.phone;
      selectedCustomerPointsBalance = customer.pointsBalance;
      notifyListeners();
      return Result.success(customer);
    } catch (e) {
      return Result.error(UnknownError(message: e.toString()));
    }
  }

  void onChangedPointsToRedeem(int value) {
    pointsToRedeem = value < 0 ? 0 : value;
    notifyListeners();
  }

  int getTotalAmount() {
    if (orderedProducts.isEmpty) return 0;
    return orderedProducts.map((e) => e.price * e.quantity).reduce((a, b) => a + b);
  }

  int getDiscountedTotalAmount() {
    final int subtotal = getTotalAmount();
    int total = subtotal;
    if (discountPercent != null) {
      final double pct = discountPercent!.clamp(0.0, 100.0);
      total = subtotal - ((subtotal * pct) / 100).round();
    } else if (discountAmount > 0) {
      total = subtotal - discountAmount;
    }
    if (pointsToRedeem > 0 && selectedCustomerPointsBalance > 0) {
      final redeemable = pointsToRedeem.clamp(0, selectedCustomerPointsBalance);
      total = total - redeemable;
    }
    if (total < 0) return 0;
    return total;
  }

  int getTaxAmount() {
    final int base = getDiscountedTotalAmount();
    if (taxPercent == null) return 0;
    final double pct = taxPercent!.clamp(0.0, 100.0);
    return ((base * pct) / 100).round();
  }

  int getFinalTotalAmount() {
    final int base = getDiscountedTotalAmount();
    return base + getTaxAmount();
  }

  bool isPaymentCovered() {
    if (selectedPaymentMethod == 'invoice') return true;
    final int payable = getFinalTotalAmount();
    return (cashAmount + bankAmount) >= payable;
  }

  void addCustomService({required String name, required int price, int quantity = 1}) {
    final order = OrderedProductEntity(
      id: DateTime.now().millisecondsSinceEpoch,
      productId: 0,
      quantity: quantity,
      stock: 0,
      name: name,
      imageUrl: '',
      price: price,
    );
    orderedProducts.add(order);
    notifyListeners();
  }

  String? getUpsellSuggestion() {
    if (orderedProducts.isEmpty) return null;
    final names = orderedProducts.map((e) => e.name.toLowerCase()).toList();
    if (names.any((n) => n.contains('beer') || n.contains('lager'))) {
      return 'Consider offering chips or nuts as a combo.';
    }
    if (names.any((n) => n.contains('shampoo') || n.contains('hair'))) {
      return 'Upsell conditioner or hair oil for better care.';
    }
    if (names.any((n) => n.contains('car wash') || n.contains('wax'))) {
      return 'Suggest interior cleaning at a small discount.';
    }
    return null;
  }

  String _buildPaymentDescription() {
    final parts = <String>[];
    if (cashAmount > 0) parts.add('cash=${cashAmount}');
    if (bankAmount > 0) parts.add('bank=${bankAmount}');
    if (discountPercent != null) parts.add('discountPct=${discountPercent!.toStringAsFixed(2)}');
    if (discountAmount > 0) parts.add('discountAmt=$discountAmount');
    if (taxPercent != null) {
      parts.add('taxPct=${taxPercent!.toStringAsFixed(2)}');
      parts.add('taxAmt=${getTaxAmount()}');
    }
    if (description != null && description!.isNotEmpty) parts.add('note=${description!}');
    return parts.isEmpty ? '' : parts.join('; ');
  }
}
