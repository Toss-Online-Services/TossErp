import 'package:flutter/material.dart';
import 'package:mobile_scanner/mobile_scanner.dart';
import 'package:local_auth/local_auth.dart';
import 'package:vibration/vibration.dart';

import '../../../domain/entities/product_entity.dart';
import '../../../domain/entities/product_category_entity.dart';
import '../../../domain/entities/customer_entity.dart';
import '../../../domain/entities/sales_transaction_entity.dart';
import '../../../app/services/customer/customer_identification_service.dart';
import 'components/pos_cart_widget.dart';
import 'components/pos_product_grid.dart';
import 'components/pos_customer_selector.dart';
import 'components/pos_payment_dialog.dart';
import 'components/pos_favorites_bar.dart';
import 'components/pos_quick_actions.dart';
import 'components/pos_transaction_list.dart';

class EnhancedPOSScreen extends StatefulWidget {
  const EnhancedPOSScreen({super.key});

  @override
  State<EnhancedPOSScreen> createState() => _EnhancedPOSScreenState();
}

class _EnhancedPOSScreenState extends State<EnhancedPOSScreen> 
    with TickerProviderStateMixin {
  late TabController _tabController;
  late AnimationController _scanAnimationController;
  
  // POS State
  SalesTransactionEntity? _currentTransaction;
  CustomerEntity? _selectedCustomer;
  List<SalesTransactionItemEntity> _cartItems = [];
  List<SalesTransactionEntity> _heldTransactions = [];
  
  // Scanner
  MobileScannerController? _scannerController;
  bool _isScanning = false;
  
  // Authentication
  final LocalAuthentication _localAuth = LocalAuthentication();
  
  // Search and filter
  ProductCategoryEntity? _selectedCategory;
  List<ProductEntity> _products = [];
  
  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 4, vsync: this);
    _scanAnimationController = AnimationController(
      duration: const Duration(seconds: 2),
      vsync: this,
    )..repeat();
    
    _initializePOS();
  }

  @override
  void dispose() {
    _tabController.dispose();
    _scanAnimationController.dispose();
    _scannerController?.dispose();
    super.dispose();
  }

  Future<void> _initializePOS() async {
    // Initialize biometrics
    try {
      // Check biometrics availability (optional feature)
      await _localAuth.canCheckBiometrics;
    } catch (e) {
      // Biometrics not available
    }
    
    // Create new transaction
    _createNewTransaction();
    
    // Load held transactions
    _loadHeldTransactions();
    
    setState(() {});
  }

  void _createNewTransaction() {
    _currentTransaction = SalesTransactionEntity(
      transactionNumber: _generateTransactionNumber(),
      subtotal: 0,
      total: 0,
      createdAt: DateTime.now(),
      items: [],
    );
    _cartItems.clear();
    _selectedCustomer = null;
  }

  String _generateTransactionNumber() {
    final now = DateTime.now();
    return 'TXN${now.millisecondsSinceEpoch.toString().substring(7)}';
  }

  Future<void> _loadHeldTransactions() async {
    // TODO: Load held transactions from local storage
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Theme.of(context).colorScheme.surface,
      body: SafeArea(
        child: Column(
          children: [
            _buildAppBar(),
            Expanded(
              child: Row(
                children: [
                  // Left panel - Product selection and cart
                  Expanded(
                    flex: 7,
                    child: _buildLeftPanel(),
                  ),
                  // Right panel - Cart and checkout
                  Expanded(
                    flex: 5,
                    child: _buildRightPanel(),
                  ),
                ],
              ),
            ),
            _buildBottomActions(),
          ],
        ),
      ),
      floatingActionButton: _buildFloatingActions(),
    );
  }

  Widget _buildAppBar() {
    return Container(
      height: 60,
      padding: const EdgeInsets.symmetric(horizontal: 16),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.primary,
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.1),
            blurRadius: 4,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: Row(
        children: [
          Icon(
            Icons.point_of_sale,
            color: Theme.of(context).colorScheme.onPrimary,
            size: 28,
          ),
          const SizedBox(width: 12),
          Text(
            'TOSS POS',
            style: Theme.of(context).textTheme.headlineSmall?.copyWith(
              color: Theme.of(context).colorScheme.onPrimary,
              fontWeight: FontWeight.bold,
            ),
          ),
          const Spacer(),
          _buildConnectionStatus(),
          const SizedBox(width: 16),
          _buildTransactionInfo(),
        ],
      ),
    );
  }

  Widget _buildConnectionStatus() {
    // TODO: Implement connection status from connectivity service
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Icon(
          Icons.wifi,
          color: Theme.of(context).colorScheme.onPrimary,
          size: 20,
        ),
        const SizedBox(width: 4),
        Icon(
          Icons.sync,
          color: Theme.of(context).colorScheme.onPrimary,
          size: 16,
        ),
      ],
    );
  }

  Widget _buildTransactionInfo() {
    return Column(
      mainAxisAlignment: MainAxisAlignment.center,
      crossAxisAlignment: CrossAxisAlignment.end,
      children: [
        Text(
          _currentTransaction?.transactionNumber ?? 'NEW',
          style: Theme.of(context).textTheme.bodySmall?.copyWith(
            color: Theme.of(context).colorScheme.onPrimary,
            fontWeight: FontWeight.bold,
          ),
        ),
        Text(
          'Items: ${_cartItems.length}',
          style: Theme.of(context).textTheme.bodySmall?.copyWith(
            color: Theme.of(context).colorScheme.onPrimary.withOpacity(0.8),
          ),
        ),
      ],
    );
  }

  Widget _buildLeftPanel() {
    return Container(
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.background,
        border: Border(
          right: BorderSide(
            color: Theme.of(context).colorScheme.outline.withOpacity(0.2),
          ),
        ),
      ),
      child: Column(
        children: [
          _buildSearchAndFilters(),
          POSFavoritesBar(
            favoriteProducts: [], // Empty for now
            onProductTap: _addProductToCart,
          ),
          Expanded(
            child: TabBarView(
              controller: _tabController,
              children: [
                POSProductGrid(
                  products: _products,
                  selectedCategory: _selectedCategory?.name,
                  onProductTap: _addProductToCart,
                ),
                POSTransactionList(
                  transactions: [], // TODO: Load actual transactions
                  onTransactionTap: _onTransactionTap,
                  onReturnTransaction: _showReturnDialog,
                  onReprintReceipt: _reprintReceipt,
                ),
                _buildScannerView(),
                const POSQuickActions(),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildSearchAndFilters() {
    return Container(
      padding: const EdgeInsets.all(16),
      child: Column(
        children: [
          // Search bar
          TextField(
            decoration: InputDecoration(
              hintText: 'Search products, scan barcode...',
              prefixIcon: const Icon(Icons.search),
              suffixIcon: Row(
                mainAxisSize: MainAxisSize.min,
                children: [
                  IconButton(
                    icon: const Icon(Icons.qr_code_scanner),
                    onPressed: _startBarcodeScanning,
                  ),
                  IconButton(
                    icon: const Icon(Icons.mic),
                    onPressed: _startVoiceSearch,
                  ),
                ],
              ),
              border: OutlineInputBorder(
                borderRadius: BorderRadius.circular(12),
              ),
            ),
            onChanged: (value) {
              // Search functionality can be implemented here
              // Filter products based on search query
            },
          ),
          const SizedBox(height: 12),
          // Tab bar for different views
          TabBar(
            controller: _tabController,
            labelStyle: const TextStyle(fontSize: 12, fontWeight: FontWeight.w600),
            unselectedLabelStyle: const TextStyle(fontSize: 12),
            tabs: const [
              Tab(text: 'Products'),
              Tab(text: 'History'),
              Tab(text: 'Scanner'),
              Tab(text: 'Quick'),
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildScannerView() {
    if (!_isScanning) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              Icons.qr_code_scanner,
              size: 80,
              color: Theme.of(context).colorScheme.primary,
            ),
            const SizedBox(height: 16),
            Text(
              'Tap to start scanning',
              style: Theme.of(context).textTheme.headlineSmall,
            ),
            const SizedBox(height: 24),
            ElevatedButton.icon(
              onPressed: _startBarcodeScanning,
              icon: const Icon(Icons.camera_alt),
              label: const Text('Start Scanner'),
              style: ElevatedButton.styleFrom(
                padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 12),
              ),
            ),
          ],
        ),
      );
    }

    return Stack(
      children: [
        MobileScanner(
          controller: _scannerController,
          onDetect: _onBarcodeDetected,
        ),
        Positioned(
          top: 16,
          right: 16,
          child: FloatingActionButton(
            mini: true,
            onPressed: _stopBarcodeScanning,
            child: const Icon(Icons.close),
          ),
        ),
        // Scanning animation overlay
        Center(
          child: AnimatedBuilder(
            animation: _scanAnimationController,
            builder: (context, child) {
              return Container(
                width: 200,
                height: 200,
                decoration: BoxDecoration(
                  border: Border.all(
                    color: Theme.of(context).colorScheme.primary.withOpacity(
                      0.5 + (_scanAnimationController.value * 0.5),
                    ),
                    width: 2,
                  ),
                  borderRadius: BorderRadius.circular(12),
                ),
              );
            },
          ),
        ),
      ],
    );
  }

  Widget _buildRightPanel() {
    return Container(
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surface,
      ),
      child: Column(
        children: [
          // Customer selector
          POSCustomerSelector(
            selectedCustomer: _selectedCustomer,
            onCustomerChanged: _onCustomerChanged,
            onCustomerScanned: _onCustomerScanned,
          ),
          // Cart
          Expanded(
            child: POSCartWidget(
              cartItems: _cartItems,
              selectedCustomer: _selectedCustomer,
              onItemQuantityChanged: _onItemQuantityChanged,
              onItemRemoved: _onItemRemoved,
              onItemDiscountChanged: _onItemDiscountChanged,
            ),
          ),
          _buildCartSummary(),
        ],
      ),
    );
  }

  Widget _buildCartSummary() {
    final subtotal = _cartItems.fold<int>(0, (sum, item) => sum + item.totalPrice);
    final discount = _cartItems.fold<int>(0, (sum, item) => sum + item.discountAmount);
    final total = subtotal - discount;

    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surfaceVariant,
        border: Border(
          top: BorderSide(
            color: Theme.of(context).colorScheme.outline.withOpacity(0.2),
          ),
        ),
      ),
      child: Column(
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                'Subtotal:',
                style: Theme.of(context).textTheme.bodyMedium,
              ),
              Text(
                '\$${(subtotal / 100).toStringAsFixed(2)}',
                style: Theme.of(context).textTheme.bodyMedium,
              ),
            ],
          ),
          if (discount > 0) ...[
            const SizedBox(height: 4),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  'Discount:',
                  style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                    color: Colors.red,
                  ),
                ),
                Text(
                  '-\$${(discount / 100).toStringAsFixed(2)}',
                  style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                    color: Colors.red,
                  ),
                ),
              ],
            ),
          ],
          const Divider(),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                'Total:',
                style: Theme.of(context).textTheme.titleLarge?.copyWith(
                  fontWeight: FontWeight.bold,
                ),
              ),
              Text(
                '\$${(total / 100).toStringAsFixed(2)}',
                style: Theme.of(context).textTheme.titleLarge?.copyWith(
                  fontWeight: FontWeight.bold,
                  color: Theme.of(context).colorScheme.primary,
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildBottomActions() {
    final canCheckout = _cartItems.isNotEmpty;
    
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surface,
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.1),
            blurRadius: 4,
            offset: const Offset(0, -2),
          ),
        ],
      ),
      child: Row(
        children: [
          // Hold transaction
          Expanded(
            child: OutlinedButton.icon(
              onPressed: canCheckout ? _holdTransaction : null,
              icon: const Icon(Icons.pause),
              label: const Text('Hold'),
              style: OutlinedButton.styleFrom(
                padding: const EdgeInsets.symmetric(vertical: 16),
              ),
            ),
          ),
          const SizedBox(width: 12),
          // Clear transaction
          Expanded(
            child: OutlinedButton.icon(
              onPressed: canCheckout ? _clearTransaction : null,
              icon: const Icon(Icons.clear),
              label: const Text('Clear'),
              style: OutlinedButton.styleFrom(
                padding: const EdgeInsets.symmetric(vertical: 16),
              ),
            ),
          ),
          const SizedBox(width: 12),
          // Checkout
          Expanded(
            flex: 2,
            child: ElevatedButton.icon(
              onPressed: canCheckout ? _showPaymentDialog : null,
              icon: const Icon(Icons.payment),
              label: const Text('Checkout'),
              style: ElevatedButton.styleFrom(
                padding: const EdgeInsets.symmetric(vertical: 16),
                backgroundColor: Theme.of(context).colorScheme.primary,
                foregroundColor: Theme.of(context).colorScheme.onPrimary,
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildFloatingActions() {
    return Column(
      mainAxisAlignment: MainAxisAlignment.end,
      children: [
        if (_heldTransactions.isNotEmpty)
          FloatingActionButton(
            heroTag: "held",
            onPressed: _showHeldTransactions,
            backgroundColor: Colors.orange,
            child: Stack(
              children: [
                const Icon(Icons.history),
                Positioned(
                  right: 0,
                  top: 0,
                  child: Container(
                    padding: const EdgeInsets.all(2),
                    decoration: const BoxDecoration(
                      color: Colors.red,
                      shape: BoxShape.circle,
                    ),
                    constraints: const BoxConstraints(
                      minWidth: 16,
                      minHeight: 16,
                    ),
                    child: Text(
                      '${_heldTransactions.length}',
                      style: const TextStyle(
                        color: Colors.white,
                        fontSize: 10,
                      ),
                      textAlign: TextAlign.center,
                    ),
                  ),
                ),
              ],
            ),
          ),
        const SizedBox(height: 16),
        FloatingActionButton(
          heroTag: "close_pos",
          onPressed: _showPOSClosingDialog,
          backgroundColor: Colors.purple,
          child: const Icon(Icons.lock_clock),
        ),
        const SizedBox(height: 16),
        FloatingActionButton(
          heroTag: "new",
          onPressed: _createNewTransaction,
          child: const Icon(Icons.add),
        ),
      ],
    );
  }

  // Action methods
  Future<void> _startBarcodeScanning() async {
    setState(() {
      _isScanning = true;
      _scannerController = MobileScannerController();
    });
    
    if (_tabController.index != 2) {
      _tabController.animateTo(2);
    }
  }

  void _stopBarcodeScanning() {
    setState(() {
      _isScanning = false;
    });
    _scannerController?.dispose();
    _scannerController = null;
  }

  void _onBarcodeDetected(BarcodeCapture capture) {
    final List<Barcode> barcodes = capture.barcodes;
    
    for (final barcode in barcodes) {
      final String? code = barcode.rawValue;
      if (code != null) {
        _processBarcodeResult(code);
        break;
      }
    }
  }

  void _processBarcodeResult(String code) async {
    // Provide haptic feedback
    if (await Vibration.hasVibrator()) {
      Vibration.vibrate(duration: 100);
    }
    
    _stopBarcodeScanning();
    
    // Check if it's a customer QR code
    final customerId = CustomerIdentificationService.validateCustomerQRCode(code);
    if (customerId != null) {
      // TODO: Load customer by ID and set as selected
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Customer QR Code: $customerId')),
      );
      return;
    }
    
    // Search for product by barcode
    // TODO: Search products by barcode
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text('Barcode scanned: $code')),
    );
  }

  void _startVoiceSearch() {
    // TODO: Implement voice search
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(content: Text('Voice search not implemented yet')),
    );
  }

  void _addProductToCart(ProductEntity product) {
    // Check stock availability
    if (product.stock <= 0) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('${product.name} is out of stock'),
          backgroundColor: Colors.red,
        ),
      );
      return;
    }

    // Check if product already in cart
    final existingIndex = _cartItems.indexWhere(
      (item) => item.productId == product.id,
    );

    if (existingIndex != -1) {
      // Update quantity
      final existingItem = _cartItems[existingIndex];
      final newQuantity = existingItem.quantity + 1;
      
      if (newQuantity > product.stock) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Only ${product.stock} ${product.name} available'),
            backgroundColor: Colors.orange,
          ),
        );
        return;
      }

      _cartItems[existingIndex] = existingItem.copyWith(
        quantity: newQuantity,
        totalPrice: product.price * newQuantity,
      );
    } else {
      // Add new item
      _cartItems.add(
        SalesTransactionItemEntity(
          transactionId: _currentTransaction?.id ?? 0,
          productId: product.id!,
          productName: product.name,
          quantity: 1,
          unitPrice: product.price,
          totalPrice: product.price,
        ),
      );
    }

    setState(() {});
    
    // Provide haptic feedback
    Vibration.vibrate(duration: 50);
  }

  void _onCustomerChanged(CustomerEntity? customer) {
    setState(() {
      _selectedCustomer = customer;
    });
  }

  void _onCustomerScanned(String qrCode) {
    final customerId = CustomerIdentificationService.validateCustomerQRCode(qrCode);
    if (customerId != null) {
      // TODO: Load customer by ID
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Customer loaded: $customerId')),
      );
    }
  }

  void _onItemQuantityChanged(int index, int newQuantity) {
    if (index >= 0 && index < _cartItems.length) {
      final item = _cartItems[index];
      if (newQuantity <= 0) {
        _cartItems.removeAt(index);
      } else {
        _cartItems[index] = item.copyWith(
          quantity: newQuantity,
          totalPrice: item.unitPrice * newQuantity,
        );
      }
      setState(() {});
    }
  }

  void _onItemRemoved(int index) {
    if (index >= 0 && index < _cartItems.length) {
      _cartItems.removeAt(index);
      setState(() {});
    }
  }

  void _onItemDiscountChanged(int index, int discountAmount) {
    if (index >= 0 && index < _cartItems.length) {
      _cartItems[index] = _cartItems[index].copyWith(
        discountAmount: discountAmount,
      );
      setState(() {});
    }
  }

  void _holdTransaction() {
    if (_cartItems.isEmpty) return;
    
    // TODO: Save transaction to held transactions
    _heldTransactions.add(_currentTransaction!.copyWith(
      status: SalesTransactionStatus.hold,
      items: List.from(_cartItems),
    ));
    
    _createNewTransaction();
    setState(() {});
    
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(content: Text('Transaction held')),
    );
  }

  void _clearTransaction() {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Clear Transaction'),
        content: const Text('Are you sure you want to clear the current transaction?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(),
            child: const Text('Cancel'),
          ),
          TextButton(
            onPressed: () {
              Navigator.of(context).pop();
              _createNewTransaction();
              setState(() {});
            },
            child: const Text('Clear'),
          ),
        ],
      ),
    );
  }

  void _showHeldTransactions() {
    // TODO: Show held transactions dialog
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text('${_heldTransactions.length} held transactions')),
    );
  }

  void _showPaymentDialog() {
    if (_cartItems.isEmpty) return;

    final subtotal = _cartItems.fold<int>(0, (sum, item) => sum + item.totalPrice);
    final discount = _cartItems.fold<int>(0, (sum, item) => sum + item.discountAmount);
    final total = subtotal - discount;

    showDialog(
      context: context,
      barrierDismissible: false,
      builder: (context) => POSPaymentDialog(
        total: total / 100.0, // Convert cents to dollars
        customerLoyaltyPoints: _selectedCustomer?.loyaltyPoints.toString(),
        onPaymentComplete: (payments, changeAmount) {
          _onPaymentComplete(payments, changeAmount);
        },
      ),
    );
  }

  void _onPaymentComplete(List<dynamic> payments, double changeAmount) {
    // Create transaction record
    final subtotal = _cartItems.fold<int>(0, (sum, item) => sum + item.totalPrice);
    final discount = _cartItems.fold<int>(0, (sum, item) => sum + item.discountAmount);
    final total = subtotal - discount;
    
    final completedTransaction = _currentTransaction!.copyWith(
      status: SalesTransactionStatus.completed,
      subtotal: subtotal,
      discountAmount: discount,
      total: total,
      amountPaid: total, // Assume full payment for now
      changeAmount: (changeAmount * 100).round(), // Convert to cents
      items: List.from(_cartItems),
    );
    
    // TODO: Save transaction to database
    // TODO: Update inventory
    // TODO: Print receipt
    
    // Show success message with receipt option
    _showTransactionCompleteDialog(completedTransaction, changeAmount);
    
    // Create new transaction
    _createNewTransaction();
    setState(() {});
  }

  void _showTransactionCompleteDialog(SalesTransactionEntity transaction, double changeAmount) {
    showDialog(
      context: context,
      barrierDismissible: false,
      builder: (context) => AlertDialog(
        title: Row(
          children: [
            Icon(
              Icons.check_circle,
              color: Colors.green,
              size: 32,
            ),
            const SizedBox(width: 12),
            const Text('Transaction Complete'),
          ],
        ),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text('Transaction: ${transaction.transactionNumber}'),
            Text('Total: \$${(transaction.total / 100).toStringAsFixed(2)}'),
            if (changeAmount > 0) ...[
              const SizedBox(height: 8),
              Container(
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: Colors.green.withOpacity(0.1),
                  borderRadius: BorderRadius.circular(8),
                  border: Border.all(color: Colors.green),
                ),
                child: Row(
                  children: [
                    const Icon(Icons.money, color: Colors.green),
                    const SizedBox(width: 8),
                    Text(
                      'Change: \$${changeAmount.toStringAsFixed(2)}',
                      style: const TextStyle(
                        fontWeight: FontWeight.bold,
                        color: Colors.green,
                        fontSize: 16,
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ],
        ),
        actions: [
          OutlinedButton.icon(
            onPressed: () {
              Navigator.of(context).pop();
              _showReceiptPreview(transaction);
            },
            icon: const Icon(Icons.receipt),
            label: const Text('View Receipt'),
          ),
          OutlinedButton.icon(
            onPressed: () {
              Navigator.of(context).pop();
              // TODO: Print receipt
            },
            icon: const Icon(Icons.print),
            label: const Text('Print'),
          ),
          ElevatedButton(
            onPressed: () => Navigator.of(context).pop(),
            child: const Text('Done'),
          ),
        ],
      ),
    );
  }

  void _onTransactionTap(SalesTransactionEntity transaction) {
    // Handle transaction tap - could show details or perform actions
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text('Viewing transaction ${transaction.transactionNumber}')),
    );
  }

  void _showReturnDialog(SalesTransactionEntity transaction) {
    showDialog(
      context: context,
      barrierDismissible: false,
      builder: (context) => _ReturnDialog(
        transaction: transaction,
        onReturnComplete: _onReturnComplete,
      ),
    );
  }

  void _onReturnComplete(SalesTransactionEntity originalTransaction, List<SalesTransactionItemEntity> returnItems, String reason) {
    // Create return transaction
    final returnTransaction = SalesTransactionEntity(
      transactionNumber: _generateTransactionNumber(),
      type: SalesTransactionType.returned,
      status: SalesTransactionStatus.completed,
      originalTransactionId: originalTransaction.id,
      subtotal: -returnItems.fold<int>(0, (sum, item) => sum + item.totalPrice),
      total: -returnItems.fold<int>(0, (sum, item) => sum + item.totalPrice),
      items: returnItems.map((item) => item.copyWith(
        quantity: -item.quantity,
        totalPrice: -item.totalPrice,
      )).toList(),
      notes: reason,
      createdAt: DateTime.now(),
    );

    // TODO: Save return transaction to database
    // TODO: Update inventory
    // TODO: Process refund

    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text('Return processed for ${returnTransaction.transactionNumber}'),
        backgroundColor: Colors.green,
      ),
    );
  }

  void _reprintReceipt(SalesTransactionEntity transaction) {
    // TODO: Implement receipt reprinting
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text('Reprinting receipt for ${transaction.transactionNumber}')),
    );
  }

  void _showReceiptPreview(SalesTransactionEntity transaction) {
    showDialog(
      context: context,
      builder: (context) => _ReceiptPreviewDialog(
        transaction: transaction,
        onPrint: () => _printReceipt(transaction),
        onEmail: () => _emailReceipt(transaction),
        onSaveAsPDF: () => _saveReceiptAsPDF(transaction),
      ),
    );
  }

  void _printReceipt(SalesTransactionEntity transaction) {
    // TODO: Implement actual printing
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text('Printing receipt for ${transaction.transactionNumber}')),
    );
  }

  void _emailReceipt(SalesTransactionEntity transaction) {
    // TODO: Implement email functionality
    showDialog(
      context: context,
      builder: (context) => _EmailReceiptDialog(
        transaction: transaction,
        onSendEmail: (email, includeDetails) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(content: Text('Receipt sent to $email')),
          );
        },
      ),
    );
  }

  void _saveReceiptAsPDF(SalesTransactionEntity transaction) {
    // TODO: Implement PDF generation
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text('Receipt saved as PDF for ${transaction.transactionNumber}')),
    );
  }

  void _showPOSClosingDialog() {
    showDialog(
      context: context,
      barrierDismissible: false,
      builder: (context) => _POSClosingDialog(
        onClosingComplete: _onPOSClosingComplete,
      ),
    );
  }

  void _onPOSClosingComplete(POSClosingVoucher closingVoucher) {
    // TODO: Save closing voucher to database
    // TODO: Consolidate transactions
    // TODO: Print closing report
    
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text('POS closed. Voucher: ${closingVoucher.voucherNumber}'),
        backgroundColor: Colors.green,
      ),
    );
  }
}

class _ReturnDialog extends StatefulWidget {
  final SalesTransactionEntity transaction;
  final Function(SalesTransactionEntity, List<SalesTransactionItemEntity>, String) onReturnComplete;

  const _ReturnDialog({
    required this.transaction,
    required this.onReturnComplete,
  });

  @override
  State<_ReturnDialog> createState() => _ReturnDialogState();
}

class _ReturnDialogState extends State<_ReturnDialog> {
  final Map<int, int> _returnQuantities = {};
  final TextEditingController _reasonController = TextEditingController();
  ReturnReasonType _selectedReason = ReturnReasonType.defective;

  @override
  void initState() {
    super.initState();
    // Initialize all quantities to 0
    for (int i = 0; i < widget.transaction.items.length; i++) {
      _returnQuantities[i] = 0;
    }
  }

  @override
  void dispose() {
    _reasonController.dispose();
    super.dispose();
  }

  List<SalesTransactionItemEntity> get _returnItems {
    final returnItems = <SalesTransactionItemEntity>[];
    for (int i = 0; i < widget.transaction.items.length; i++) {
      final returnQty = _returnQuantities[i] ?? 0;
      if (returnQty > 0) {
        final originalItem = widget.transaction.items[i];
        returnItems.add(originalItem.copyWith(
          quantity: returnQty,
          totalPrice: (originalItem.unitPrice * returnQty),
        ));
      }
    }
    return returnItems;
  }

  double get _returnTotal {
    return _returnItems.fold(0, (sum, item) => sum + (item.totalPrice / 100));
  }

  bool get _canProcessReturn {
    return _returnItems.isNotEmpty && _reasonController.text.trim().isNotEmpty;
  }

  @override
  Widget build(BuildContext context) {
    return Dialog(
      child: Container(
        width: MediaQuery.of(context).size.width * 0.9,
        height: MediaQuery.of(context).size.height * 0.9,
        padding: const EdgeInsets.all(20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            _buildHeader(),
            const SizedBox(height: 20),
            _buildTransactionInfo(),
            const SizedBox(height: 20),
            _buildReturnReason(),
            const SizedBox(height: 20),
            _buildItemsList(),
            const SizedBox(height: 20),
            _buildReturnSummary(),
            const SizedBox(height: 20),
            _buildActionButtons(),
          ],
        ),
      ),
    );
  }

  Widget _buildHeader() {
    return Row(
      children: [
        Icon(
          Icons.undo,
          color: Colors.orange,
          size: 32,
        ),
        const SizedBox(width: 12),
        Text(
          'Process Return',
          style: Theme.of(context).textTheme.headlineSmall?.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const Spacer(),
        IconButton(
          onPressed: () => Navigator.of(context).pop(),
          icon: const Icon(Icons.close),
        ),
      ],
    );
  }

  Widget _buildTransactionInfo() {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surfaceVariant,
        borderRadius: BorderRadius.circular(12),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Original Transaction: ${widget.transaction.transactionNumber}',
            style: const TextStyle(
              fontSize: 16,
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 8),
          Text('Date: ${widget.transaction.createdAt.toString().substring(0, 19)}'),
          Text('Total: \$${(widget.transaction.total / 100).toStringAsFixed(2)}'),
          Text('Items: ${widget.transaction.itemCount}'),
        ],
      ),
    );
  }

  Widget _buildReturnReason() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'Return Reason',
          style: Theme.of(context).textTheme.titleMedium?.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: 12),
        DropdownButtonFormField<ReturnReasonType>(
          value: _selectedReason,
          decoration: const InputDecoration(
            border: OutlineInputBorder(),
            prefixIcon: Icon(Icons.info_outline),
          ),
          items: ReturnReasonType.values.map((reason) {
            return DropdownMenuItem(
              value: reason,
              child: Text(reason.displayName),
            );
          }).toList(),
          onChanged: (value) {
            setState(() {
              _selectedReason = value!;
            });
          },
        ),
        const SizedBox(height: 12),
        TextField(
          controller: _reasonController,
          decoration: const InputDecoration(
            labelText: 'Additional Notes',
            hintText: 'Provide additional details about the return...',
            border: OutlineInputBorder(),
            prefixIcon: Icon(Icons.note),
          ),
          maxLines: 3,
        ),
      ],
    );
  }

  Widget _buildItemsList() {
    return Expanded(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Text(
                'Select Items to Return',
                style: Theme.of(context).textTheme.titleMedium?.copyWith(
                  fontWeight: FontWeight.bold,
                ),
              ),
              const Spacer(),
              OutlinedButton.icon(
                onPressed: _selectAllItems,
                icon: const Icon(Icons.select_all),
                label: const Text('Select All'),
                style: OutlinedButton.styleFrom(
                  visualDensity: VisualDensity.compact,
                ),
              ),
              const SizedBox(width: 8),
              OutlinedButton.icon(
                onPressed: _clearSelection,
                icon: const Icon(Icons.clear_all),
                label: const Text('Clear'),
                style: OutlinedButton.styleFrom(
                  visualDensity: VisualDensity.compact,
                ),
              ),
            ],
          ),
          const SizedBox(height: 12),
          Expanded(
            child: ListView.builder(
              itemCount: widget.transaction.items.length,
              itemBuilder: (context, index) {
                return _buildItemRow(index, widget.transaction.items[index]);
              },
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildItemRow(int index, SalesTransactionItemEntity item) {
    final returnQty = _returnQuantities[index] ?? 0;
    final maxQty = item.quantity;

    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Row(
          children: [
            Expanded(
              flex: 3,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    item.displayName,
                    style: const TextStyle(
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    'Unit Price: \$${(item.unitPrice / 100).toStringAsFixed(2)}',
                    style: TextStyle(
                      color: Colors.grey[600],
                      fontSize: 12,
                    ),
                  ),
                  Text(
                    'Original Qty: $maxQty',
                    style: TextStyle(
                      color: Colors.grey[600],
                      fontSize: 12,
                    ),
                  ),
                ],
              ),
            ),
            Expanded(
              flex: 2,
              child: Row(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  IconButton(
                    onPressed: returnQty > 0 
                        ? () => _updateReturnQuantity(index, returnQty - 1)
                        : null,
                    icon: const Icon(Icons.remove),
                    visualDensity: VisualDensity.compact,
                  ),
                  Container(
                    width: 60,
                    padding: const EdgeInsets.symmetric(vertical: 8),
                    decoration: BoxDecoration(
                      border: Border.all(color: Colors.grey),
                      borderRadius: BorderRadius.circular(4),
                    ),
                    child: Text(
                      '$returnQty',
                      textAlign: TextAlign.center,
                      style: const TextStyle(
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ),
                  IconButton(
                    onPressed: returnQty < maxQty 
                        ? () => _updateReturnQuantity(index, returnQty + 1)
                        : null,
                    icon: const Icon(Icons.add),
                    visualDensity: VisualDensity.compact,
                  ),
                ],
              ),
            ),
            Expanded(
              child: Text(
                '\$${((item.unitPrice * returnQty) / 100).toStringAsFixed(2)}',
                textAlign: TextAlign.end,
                style: const TextStyle(
                  fontWeight: FontWeight.bold,
                  fontSize: 16,
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildReturnSummary() {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.orange.withOpacity(0.1),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: Colors.orange),
      ),
      child: Column(
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              const Text('Items to Return:'),
              Text('${_returnItems.length}'),
            ],
          ),
          const SizedBox(height: 4),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              const Text('Return Amount:'),
              Text(
                '\$${_returnTotal.toStringAsFixed(2)}',
                style: const TextStyle(
                  fontWeight: FontWeight.bold,
                  fontSize: 18,
                  color: Colors.orange,
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildActionButtons() {
    return Row(
      children: [
        Expanded(
          child: OutlinedButton(
            onPressed: () => Navigator.of(context).pop(),
            child: const Text('Cancel'),
          ),
        ),
        const SizedBox(width: 16),
        Expanded(
          flex: 2,
          child: ElevatedButton.icon(
            onPressed: _canProcessReturn ? _processReturn : null,
            icon: const Icon(Icons.undo),
            label: const Text('Process Return'),
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.orange,
              foregroundColor: Colors.white,
            ),
          ),
        ),
      ],
    );
  }

  void _updateReturnQuantity(int index, int quantity) {
    setState(() {
      _returnQuantities[index] = quantity;
    });
  }

  void _selectAllItems() {
    setState(() {
      for (int i = 0; i < widget.transaction.items.length; i++) {
        _returnQuantities[i] = widget.transaction.items[i].quantity;
      }
    });
  }

  void _clearSelection() {
    setState(() {
      for (int i = 0; i < widget.transaction.items.length; i++) {
        _returnQuantities[i] = 0;
      }
    });
  }

  void _processReturn() {
    if (!_canProcessReturn) return;

    final reason = '${_selectedReason.displayName}: ${_reasonController.text.trim()}';
    
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Confirm Return'),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text('Return ${_returnItems.length} items'),
            Text('Refund Amount: \$${_returnTotal.toStringAsFixed(2)}'),
            const SizedBox(height: 8),
            Text(
              'This action cannot be undone.',
              style: TextStyle(
                color: Colors.red[700],
                fontWeight: FontWeight.w500,
              ),
            ),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(),
            child: const Text('Cancel'),
          ),
          ElevatedButton(
            onPressed: () {
              Navigator.of(context).pop(); // Close confirmation
              Navigator.of(context).pop(); // Close return dialog
              widget.onReturnComplete(widget.transaction, _returnItems, reason);
            },
            style: ElevatedButton.styleFrom(backgroundColor: Colors.orange),
            child: const Text('Confirm Return'),
          ),
        ],
      ),
    );
  }
}

enum ReturnReasonType {
  defective('Defective/Damaged'),
  wrongItem('Wrong Item'),
  notAsDescribed('Not as Described'),
  customerRequest('Customer Request'),
  duplicate('Duplicate Purchase'),
  other('Other');

  const ReturnReasonType(this.displayName);
  final String displayName;
}

class POSClosingVoucher {
  final String voucherNumber;
  final DateTime closingDate;
  final String userId;
  final double totalSales;
  final double totalCash;
  final double totalCard;
  final double totalDigital;
  final int transactionCount;
  final List<SalesTransactionEntity> consolidatedTransactions;
  final Map<String, double> paymentMethodTotals;
  final double expectedCash;
  final double actualCash;
  final double cashDifference;
  final String? notes;

  POSClosingVoucher({
    required this.voucherNumber,
    required this.closingDate,
    required this.userId,
    required this.totalSales,
    required this.totalCash,
    required this.totalCard,
    required this.totalDigital,
    required this.transactionCount,
    required this.consolidatedTransactions,
    required this.paymentMethodTotals,
    required this.expectedCash,
    required this.actualCash,
    required this.cashDifference,
    this.notes,
  });
}

class _POSClosingDialog extends StatefulWidget {
  final Function(POSClosingVoucher) onClosingComplete;

  const _POSClosingDialog({
    required this.onClosingComplete,
  });

  @override
  State<_POSClosingDialog> createState() => _POSClosingDialogState();
}

class _POSClosingDialogState extends State<_POSClosingDialog> {
  final TextEditingController _actualCashController = TextEditingController();
  final TextEditingController _notesController = TextEditingController();
  DateTime _selectedDate = DateTime.now();
  bool _isProcessing = false;

  // Mock data for demonstration
  final List<SalesTransactionEntity> _todaysTransactions = [
    // This would normally come from the database
  ];

  double get _expectedCash => 1250.50; // This would be calculated from actual transactions
  double get _totalSales => 2450.75;
  double get _totalCard => 800.25;
  double get _totalDigital => 400.00;
  int get _transactionCount => 15;

  double get _actualCash => double.tryParse(_actualCashController.text) ?? 0;
  double get _cashDifference => _actualCash - _expectedCash;

  @override
  void initState() {
    super.initState();
    _actualCashController.text = _expectedCash.toStringAsFixed(2);
  }

  @override
  void dispose() {
    _actualCashController.dispose();
    _notesController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Dialog(
      child: Container(
        width: MediaQuery.of(context).size.width * 0.9,
        height: MediaQuery.of(context).size.height * 0.9,
        padding: const EdgeInsets.all(20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            _buildHeader(),
            const SizedBox(height: 20),
            Expanded(
              child: SingleChildScrollView(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    _buildDateSelector(),
                    const SizedBox(height: 20),
                    _buildSalesSummary(),
                    const SizedBox(height: 20),
                    _buildPaymentMethodBreakdown(),
                    const SizedBox(height: 20),
                    _buildCashReconciliation(),
                    const SizedBox(height: 20),
                    _buildNotesSection(),
                  ],
                ),
              ),
            ),
            const SizedBox(height: 20),
            _buildActionButtons(),
          ],
        ),
      ),
    );
  }

  Widget _buildHeader() {
    return Row(
      children: [
        Icon(
          Icons.lock_clock,
          color: Colors.purple,
          size: 32,
        ),
        const SizedBox(width: 12),
        Text(
          'Close POS',
          style: Theme.of(context).textTheme.headlineSmall?.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const Spacer(),
        IconButton(
          onPressed: () => Navigator.of(context).pop(),
          icon: const Icon(Icons.close),
        ),
      ],
    );
  }

  Widget _buildDateSelector() {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surfaceVariant,
        borderRadius: BorderRadius.circular(12),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Closing Period',
            style: Theme.of(context).textTheme.titleMedium?.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 12),
          Row(
            children: [
              Expanded(
                child: ListTile(
                  contentPadding: EdgeInsets.zero,
                  leading: const Icon(Icons.calendar_today),
                  title: Text('Date: ${_selectedDate.toString().substring(0, 10)}'),
                  subtitle: Text('Time: ${TimeOfDay.fromDateTime(_selectedDate).format(context)}'),
                  onTap: () => _selectDate(),
                ),
              ),
              ElevatedButton.icon(
                onPressed: _loadTransactions,
                icon: const Icon(Icons.refresh),
                label: const Text('Refresh'),
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildSalesSummary() {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.primaryContainer,
        borderRadius: BorderRadius.circular(12),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Sales Summary',
            style: Theme.of(context).textTheme.titleMedium?.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 16),
          Row(
            children: [
              Expanded(
                child: _buildSummaryCard(
                  'Total Sales',
                  '\$${_totalSales.toStringAsFixed(2)}',
                  Icons.attach_money,
                  Colors.green,
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: _buildSummaryCard(
                  'Transactions',
                  '$_transactionCount',
                  Icons.receipt,
                  Colors.blue,
                ),
              ),
            ],
          ),
          const SizedBox(height: 12),
          Row(
            children: [
              Expanded(
                child: _buildSummaryCard(
                  'Avg. Transaction',
                  '\$${(_totalSales / _transactionCount).toStringAsFixed(2)}',
                  Icons.trending_up,
                  Colors.orange,
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: _buildSummaryCard(
                  'Items Sold',
                  '42', // This would be calculated from actual data
                  Icons.inventory,
                  Colors.purple,
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildSummaryCard(String title, String value, IconData icon, Color color) {
    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: color.withOpacity(0.1),
        borderRadius: BorderRadius.circular(8),
        border: Border.all(color: color.withOpacity(0.3)),
      ),
      child: Column(
        children: [
          Icon(icon, color: color, size: 24),
          const SizedBox(height: 8),
          Text(
            value,
            style: TextStyle(
              fontSize: 18,
              fontWeight: FontWeight.bold,
              color: color,
            ),
          ),
          const SizedBox(height: 4),
          Text(
            title,
            style: const TextStyle(fontSize: 12),
            textAlign: TextAlign.center,
          ),
        ],
      ),
    );
  }

  Widget _buildPaymentMethodBreakdown() {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surface,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: Theme.of(context).colorScheme.outline.withOpacity(0.2)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Payment Method Breakdown',
            style: Theme.of(context).textTheme.titleMedium?.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 16),
          _buildPaymentMethodRow('Cash', _expectedCash, Icons.money),
          _buildPaymentMethodRow('Card', _totalCard, Icons.credit_card),
          _buildPaymentMethodRow('Digital', _totalDigital, Icons.wallet),
          const Divider(),
          _buildPaymentMethodRow(
            'Total',
            _expectedCash + _totalCard + _totalDigital,
            Icons.calculate,
            isBold: true,
          ),
        ],
      ),
    );
  }

  Widget _buildPaymentMethodRow(String method, double amount, IconData icon, {bool isBold = false}) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4),
      child: Row(
        children: [
          Icon(icon, size: 20, color: Colors.grey[600]),
          const SizedBox(width: 8),
          Expanded(
            child: Text(
              method,
              style: TextStyle(
                fontWeight: isBold ? FontWeight.bold : FontWeight.normal,
              ),
            ),
          ),
          Text(
            '\$${amount.toStringAsFixed(2)}',
            style: TextStyle(
              fontWeight: isBold ? FontWeight.bold : FontWeight.normal,
              fontSize: isBold ? 16 : 14,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildCashReconciliation() {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: _cashDifference.abs() > 0.01
            ? Colors.orange.withOpacity(0.1)
            : Colors.green.withOpacity(0.1),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(
          color: _cashDifference.abs() > 0.01 ? Colors.orange : Colors.green,
        ),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Cash Reconciliation',
            style: Theme.of(context).textTheme.titleMedium?.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 16),
          Row(
            children: [
              Expanded(
                child: TextField(
                  controller: _actualCashController,
                  decoration: const InputDecoration(
                    labelText: 'Actual Cash Count',
                    prefixText: '\$',
                    border: OutlineInputBorder(),
                    prefixIcon: Icon(Icons.money),
                  ),
                  keyboardType: TextInputType.number,
                  onChanged: (value) => setState(() {}),
                ),
              ),
              const SizedBox(width: 16),
              Column(
                crossAxisAlignment: CrossAxisAlignment.end,
                children: [
                  Text('Expected: \$${_expectedCash.toStringAsFixed(2)}'),
                  Text('Actual: \$${_actualCash.toStringAsFixed(2)}'),
                  Text(
                    'Difference: \$${_cashDifference.toStringAsFixed(2)}',
                    style: TextStyle(
                      fontWeight: FontWeight.bold,
                      color: _cashDifference.abs() > 0.01
                          ? Colors.red
                          : Colors.green,
                    ),
                  ),
                ],
              ),
            ],
          ),
          if (_cashDifference.abs() > 0.01) ...[
            const SizedBox(height: 12),
            Container(
              padding: const EdgeInsets.all(12),
              decoration: BoxDecoration(
                color: Colors.red.withOpacity(0.1),
                borderRadius: BorderRadius.circular(8),
              ),
              child: Row(
                children: [
                  const Icon(Icons.warning, color: Colors.red),
                  const SizedBox(width: 8),
                  Expanded(
                    child: Text(
                      _cashDifference > 0
                          ? 'Cash overage detected. Please recount and verify.'
                          : 'Cash shortage detected. Please recount and verify.',
                      style: const TextStyle(color: Colors.red),
                    ),
                  ),
                ],
              ),
            ),
          ],
        ],
      ),
    );
  }

  Widget _buildNotesSection() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'Notes',
          style: Theme.of(context).textTheme.titleMedium?.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: 12),
        TextField(
          controller: _notesController,
          decoration: const InputDecoration(
            hintText: 'Add any notes about this closing period...',
            border: OutlineInputBorder(),
            prefixIcon: Icon(Icons.note),
          ),
          maxLines: 3,
        ),
      ],
    );
  }

  Widget _buildActionButtons() {
    return Row(
      children: [
        Expanded(
          child: OutlinedButton(
            onPressed: _isProcessing ? null : () => Navigator.of(context).pop(),
            child: const Text('Cancel'),
          ),
        ),
        const SizedBox(width: 16),
        Expanded(
          flex: 2,
          child: ElevatedButton.icon(
            onPressed: _isProcessing ? null : _processPOSClosing,
            icon: _isProcessing
                ? const SizedBox(
                    width: 16,
                    height: 16,
                    child: CircularProgressIndicator(strokeWidth: 2),
                  )
                : const Icon(Icons.lock),
            label: Text(_isProcessing ? 'Processing...' : 'Close POS'),
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.purple,
              foregroundColor: Colors.white,
            ),
          ),
        ),
      ],
    );
  }

  void _selectDate() async {
    final date = await showDatePicker(
      context: context,
      initialDate: _selectedDate,
      firstDate: DateTime.now().subtract(const Duration(days: 30)),
      lastDate: DateTime.now(),
    );

    if (date != null) {
      final time = await showTimePicker(
        context: context,
        initialTime: TimeOfDay.fromDateTime(_selectedDate),
      );

      if (time != null) {
        setState(() {
          _selectedDate = DateTime(
            date.year,
            date.month,
            date.day,
            time.hour,
            time.minute,
          );
        });
      }
    }
  }

  void _loadTransactions() {
    // TODO: Load transactions for the selected date/period
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(content: Text('Refreshing transaction data...')),
    );
  }

  void _processPOSClosing() async {
    setState(() {
      _isProcessing = true;
    });

    // Simulate processing delay
    await Future.delayed(const Duration(seconds: 2));

    final closingVoucher = POSClosingVoucher(
      voucherNumber: 'PCV${DateTime.now().millisecondsSinceEpoch.toString().substring(8)}',
      closingDate: _selectedDate,
      userId: 'current_user', // This would come from authentication
      totalSales: _totalSales,
      totalCash: _expectedCash,
      totalCard: _totalCard,
      totalDigital: _totalDigital,
      transactionCount: _transactionCount,
      consolidatedTransactions: _todaysTransactions,
      paymentMethodTotals: {
        'Cash': _expectedCash,
        'Card': _totalCard,
        'Digital': _totalDigital,
      },
      expectedCash: _expectedCash,
      actualCash: _actualCash,
      cashDifference: _cashDifference,
      notes: _notesController.text.trim().isEmpty ? null : _notesController.text.trim(),
    );

    Navigator.of(context).pop();
    widget.onClosingComplete(closingVoucher);
  }
}

class _ReceiptPreviewDialog extends StatefulWidget {
  final SalesTransactionEntity transaction;
  final VoidCallback onPrint;
  final VoidCallback onEmail;
  final VoidCallback onSaveAsPDF;

  const _ReceiptPreviewDialog({
    required this.transaction,
    required this.onPrint,
    required this.onEmail,
    required this.onSaveAsPDF,
  });

  @override
  State<_ReceiptPreviewDialog> createState() => _ReceiptPreviewDialogState();
}

class _ReceiptPreviewDialogState extends State<_ReceiptPreviewDialog> {
  ReceiptTemplateType _selectedTemplate = ReceiptTemplateType.standard;
  bool _includeQRCode = true;
  bool _includeBarcode = true;

  @override
  Widget build(BuildContext context) {
    return Dialog(
      child: Container(
        width: MediaQuery.of(context).size.width * 0.9,
        height: MediaQuery.of(context).size.height * 0.9,
        padding: const EdgeInsets.all(20),
        child: Column(
          children: [
            _buildHeader(),
            const SizedBox(height: 20),
            _buildTemplateSelector(),
            const SizedBox(height: 20),
            Expanded(
              child: Row(
                children: [
                  Expanded(
                    flex: 2,
                    child: _buildReceiptPreview(),
                  ),
                  const SizedBox(width: 20),
                  Expanded(
                    child: _buildOptionsPanel(),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 20),
            _buildActionButtons(),
          ],
        ),
      ),
    );
  }

  Widget _buildHeader() {
    return Row(
      children: [
        const Icon(Icons.receipt_long, size: 32),
        const SizedBox(width: 12),
        Text(
          'Receipt Preview',
          style: Theme.of(context).textTheme.headlineSmall?.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const Spacer(),
        IconButton(
          onPressed: () => Navigator.of(context).pop(),
          icon: const Icon(Icons.close),
        ),
      ],
    );
  }

  Widget _buildTemplateSelector() {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surfaceVariant,
        borderRadius: BorderRadius.circular(12),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Receipt Template',
            style: Theme.of(context).textTheme.titleMedium?.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 12),
          Wrap(
            spacing: 8,
            children: ReceiptTemplateType.values.map((template) {
              final isSelected = _selectedTemplate == template;
              return ChoiceChip(
                label: Text(template.displayName),
                selected: isSelected,
                onSelected: (selected) {
                  if (selected) {
                    setState(() {
                      _selectedTemplate = template;
                    });
                  }
                },
              );
            }).toList(),
          ),
        ],
      ),
    );
  }

  Widget _buildReceiptPreview() {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: Colors.grey[300]!),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.1),
            blurRadius: 4,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: SingleChildScrollView(
        child: _buildReceiptContent(),
      ),
    );
  }

  Widget _buildReceiptContent() {
    switch (_selectedTemplate) {
      case ReceiptTemplateType.standard:
        return _buildStandardReceipt();
      case ReceiptTemplateType.minimal:
        return _buildMinimalReceipt();
      case ReceiptTemplateType.detailed:
        return _buildDetailedReceipt();
      case ReceiptTemplateType.thermal:
        return _buildThermalReceipt();
    }
  }

  Widget _buildStandardReceipt() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.center,
      children: [
        // Business header
        Text(
          'TOSS ERP',
          style: const TextStyle(
            fontSize: 24,
            fontWeight: FontWeight.bold,
            color: Colors.black,
          ),
        ),
        const Text(
          'Point of Sale System',
          style: TextStyle(fontSize: 14, color: Colors.black54),
        ),
        const Text(
          '123 Business Street, City, State 12345',
          style: TextStyle(fontSize: 12, color: Colors.black54),
        ),
        const Text(
          'Phone: (555) 123-4567',
          style: TextStyle(fontSize: 12, color: Colors.black54),
        ),
        const SizedBox(height: 20),
        const Divider(color: Colors.black),
        
        // Transaction info
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            const Text('Receipt #:', style: TextStyle(color: Colors.black)),
            Text(
              widget.transaction.transactionNumber,
              style: const TextStyle(fontWeight: FontWeight.bold, color: Colors.black),
            ),
          ],
        ),
        const SizedBox(height: 4),
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            const Text('Date:', style: TextStyle(color: Colors.black)),
            Text(
              widget.transaction.createdAt.toString().substring(0, 19),
              style: const TextStyle(color: Colors.black),
            ),
          ],
        ),
        if (widget.transaction.customerId != null) ...[
          const SizedBox(height: 4),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              const Text('Customer:', style: TextStyle(color: Colors.black)),
              Text(
                widget.transaction.customerId!,
                style: const TextStyle(color: Colors.black),
              ),
            ],
          ),
        ],
        const SizedBox(height: 16),
        const Divider(color: Colors.black),
        
        // Items
        ...widget.transaction.items.map((item) => Padding(
          padding: const EdgeInsets.symmetric(vertical: 4),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Expanded(
                    child: Text(
                      item.displayName,
                      style: const TextStyle(
                        fontWeight: FontWeight.w600,
                        color: Colors.black,
                      ),
                    ),
                  ),
                  Text(
                    '\$${(item.netPrice / 100).toStringAsFixed(2)}',
                    style: const TextStyle(
                      fontWeight: FontWeight.bold,
                      color: Colors.black,
                    ),
                  ),
                ],
              ),
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text(
                    '${item.quantity} x \$${(item.unitPrice / 100).toStringAsFixed(2)}',
                    style: const TextStyle(fontSize: 12, color: Colors.black54),
                  ),
                  if (item.discountAmount > 0)
                    Text(
                      'Discount: -\$${(item.discountAmount / 100).toStringAsFixed(2)}',
                      style: const TextStyle(fontSize: 12, color: Colors.red),
                    ),
                ],
              ),
            ],
          ),
        )),
        
        const SizedBox(height: 16),
        const Divider(color: Colors.black),
        
        // Totals
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            const Text('Subtotal:', style: TextStyle(color: Colors.black)),
            Text(
              '\$${(widget.transaction.subtotal / 100).toStringAsFixed(2)}',
              style: const TextStyle(color: Colors.black),
            ),
          ],
        ),
        if (widget.transaction.discountAmount > 0) ...[
          const SizedBox(height: 4),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              const Text('Discount:', style: TextStyle(color: Colors.red)),
              Text(
                '-\$${(widget.transaction.discountAmount / 100).toStringAsFixed(2)}',
                style: const TextStyle(color: Colors.red),
              ),
            ],
          ),
        ],
        if (widget.transaction.taxAmount > 0) ...[
          const SizedBox(height: 4),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              const Text('Tax:', style: TextStyle(color: Colors.black)),
              Text(
                '\$${(widget.transaction.taxAmount / 100).toStringAsFixed(2)}',
                style: const TextStyle(color: Colors.black),
              ),
            ],
          ),
        ],
        const SizedBox(height: 8),
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            const Text(
              'TOTAL:',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
                color: Colors.black,
              ),
            ),
            Text(
              '\$${(widget.transaction.total / 100).toStringAsFixed(2)}',
              style: const TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
                color: Colors.black,
              ),
            ),
          ],
        ),
        
        if (widget.transaction.changeAmount > 0) ...[
          const SizedBox(height: 8),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              const Text('Change:', style: TextStyle(color: Colors.green)),
              Text(
                '\$${(widget.transaction.changeAmount / 100).toStringAsFixed(2)}',
                style: const TextStyle(
                  fontWeight: FontWeight.bold,
                  color: Colors.green,
                ),
              ),
            ],
          ),
        ],
        
        const SizedBox(height: 20),
        const Divider(color: Colors.black),
        
        // Footer
        const Text(
          'Thank you for your business!',
          style: TextStyle(
            fontSize: 16,
            fontWeight: FontWeight.bold,
            color: Colors.black,
          ),
          textAlign: TextAlign.center,
        ),
        const SizedBox(height: 8),
        const Text(
          'Items can be returned within 30 days with receipt',
          style: TextStyle(fontSize: 10, color: Colors.black54),
          textAlign: TextAlign.center,
        ),
        
        if (_includeQRCode) ...[
          const SizedBox(height: 16),
          Container(
            width: 100,
            height: 100,
            decoration: BoxDecoration(
              border: Border.all(color: Colors.black),
            ),
            child: const Icon(
              Icons.qr_code,
              size: 80,
              color: Colors.black54,
            ),
          ),
          const Text(
            'Scan for receipt details',
            style: TextStyle(fontSize: 10, color: Colors.black54),
          ),
        ],
      ],
    );
  }

  Widget _buildMinimalReceipt() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'TOSS ERP - Receipt #${widget.transaction.transactionNumber}',
          style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold, color: Colors.black),
        ),
        Text(
          widget.transaction.createdAt.toString().substring(0, 19),
          style: const TextStyle(fontSize: 12, color: Colors.black54),
        ),
        const SizedBox(height: 16),
        ...widget.transaction.items.map((item) => Padding(
          padding: const EdgeInsets.symmetric(vertical: 2),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Expanded(
                child: Text(
                  '${item.quantity}x ${item.displayName}',
                  style: const TextStyle(color: Colors.black),
                ),
              ),
              Text(
                '\$${(item.netPrice / 100).toStringAsFixed(2)}',
                style: const TextStyle(color: Colors.black),
              ),
            ],
          ),
        )),
        const SizedBox(height: 16),
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            const Text(
              'TOTAL:',
              style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold, color: Colors.black),
            ),
            Text(
              '\$${(widget.transaction.total / 100).toStringAsFixed(2)}',
              style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold, color: Colors.black),
            ),
          ],
        ),
      ],
    );
  }

  Widget _buildDetailedReceipt() {
    return _buildStandardReceipt(); // For now, same as standard with more details
  }

  Widget _buildThermalReceipt() {
    return Container(
      width: 200, // Thermal printer width
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.center,
        children: [
          const Text(
            'TOSS ERP',
            style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold, color: Colors.black),
            textAlign: TextAlign.center,
          ),
          const Text(
            '123 Business St',
            style: TextStyle(fontSize: 10, color: Colors.black),
            textAlign: TextAlign.center,
          ),
          const Text(
            'Tel: (555) 123-4567',
            style: TextStyle(fontSize: 10, color: Colors.black),
            textAlign: TextAlign.center,
          ),
          const SizedBox(height: 8),
          Text(
            '--------------------------------',
            style: const TextStyle(color: Colors.black),
          ),
          const SizedBox(height: 8),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              const Text('Receipt:', style: TextStyle(fontSize: 10, color: Colors.black)),
              Text(
                widget.transaction.transactionNumber,
                style: const TextStyle(fontSize: 10, color: Colors.black),
              ),
            ],
          ),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              const Text('Date:', style: TextStyle(fontSize: 10, color: Colors.black)),
              Text(
                widget.transaction.createdAt.toString().substring(0, 16),
                style: const TextStyle(fontSize: 10, color: Colors.black),
              ),
            ],
          ),
          const SizedBox(height: 8),
          Text(
            '--------------------------------',
            style: const TextStyle(color: Colors.black),
          ),
          const SizedBox(height: 8),
          ...widget.transaction.items.map((item) => Column(
            children: [
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Expanded(
                    child: Text(
                      item.displayName,
                      style: const TextStyle(fontSize: 10, color: Colors.black),
                    ),
                  ),
                ],
              ),
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text(
                    '${item.quantity} x \$${(item.unitPrice / 100).toStringAsFixed(2)}',
                    style: const TextStyle(fontSize: 10, color: Colors.black),
                  ),
                  Text(
                    '\$${(item.netPrice / 100).toStringAsFixed(2)}',
                    style: const TextStyle(fontSize: 10, color: Colors.black),
                  ),
                ],
              ),
              const SizedBox(height: 4),
            ],
          )),
          const SizedBox(height: 8),
          Text(
            '--------------------------------',
            style: const TextStyle(color: Colors.black),
          ),
          const SizedBox(height: 8),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              const Text(
                'TOTAL:',
                style: TextStyle(fontSize: 12, fontWeight: FontWeight.bold, color: Colors.black),
              ),
              Text(
                '\$${(widget.transaction.total / 100).toStringAsFixed(2)}',
                style: const TextStyle(fontSize: 12, fontWeight: FontWeight.bold, color: Colors.black),
              ),
            ],
          ),
          const SizedBox(height: 16),
          const Text(
            'Thank you!',
            style: TextStyle(fontSize: 12, fontWeight: FontWeight.bold, color: Colors.black),
            textAlign: TextAlign.center,
          ),
        ],
      ),
    );
  }

  Widget _buildOptionsPanel() {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surfaceVariant,
        borderRadius: BorderRadius.circular(12),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Receipt Options',
            style: Theme.of(context).textTheme.titleMedium?.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 16),
          CheckboxListTile(
            title: const Text('Include QR Code'),
            subtitle: const Text('For digital receipt access'),
            value: _includeQRCode,
            onChanged: (value) {
              setState(() {
                _includeQRCode = value ?? true;
              });
            },
          ),
          CheckboxListTile(
            title: const Text('Include Barcode'),
            subtitle: const Text('For easy scanning'),
            value: _includeBarcode,
            onChanged: (value) {
              setState(() {
                _includeBarcode = value ?? true;
              });
            },
          ),
          const SizedBox(height: 16),
          const Divider(),
          const SizedBox(height: 16),
          Text(
            'Quick Actions',
            style: Theme.of(context).textTheme.titleSmall?.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 12),
          SizedBox(
            width: double.infinity,
            child: OutlinedButton.icon(
              onPressed: widget.onPrint,
              icon: const Icon(Icons.print),
              label: const Text('Print Receipt'),
            ),
          ),
          const SizedBox(height: 8),
          SizedBox(
            width: double.infinity,
            child: OutlinedButton.icon(
              onPressed: widget.onEmail,
              icon: const Icon(Icons.email),
              label: const Text('Email Receipt'),
            ),
          ),
          const SizedBox(height: 8),
          SizedBox(
            width: double.infinity,
            child: OutlinedButton.icon(
              onPressed: widget.onSaveAsPDF,
              icon: const Icon(Icons.picture_as_pdf),
              label: const Text('Save as PDF'),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildActionButtons() {
    return Row(
      children: [
        Expanded(
          child: OutlinedButton(
            onPressed: () => Navigator.of(context).pop(),
            child: const Text('Close'),
          ),
        ),
        const SizedBox(width: 16),
        Expanded(
          child: ElevatedButton.icon(
            onPressed: widget.onPrint,
            icon: const Icon(Icons.print),
            label: const Text('Print'),
          ),
        ),
      ],
    );
  }
}

class _EmailReceiptDialog extends StatefulWidget {
  final SalesTransactionEntity transaction;
  final Function(String email, bool includeDetails) onSendEmail;

  const _EmailReceiptDialog({
    required this.transaction,
    required this.onSendEmail,
  });

  @override
  State<_EmailReceiptDialog> createState() => _EmailReceiptDialogState();
}

class _EmailReceiptDialogState extends State<_EmailReceiptDialog> {
  final TextEditingController _emailController = TextEditingController();
  final _formKey = GlobalKey<FormState>();
  bool _includeDetails = true;
  bool _isSending = false;

  @override
  void dispose() {
    _emailController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: const Row(
        children: [
          Icon(Icons.email),
          SizedBox(width: 8),
          Text('Email Receipt'),
        ],
      ),
      content: Form(
        key: _formKey,
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            TextFormField(
              controller: _emailController,
              decoration: const InputDecoration(
                labelText: 'Email Address',
                prefixIcon: Icon(Icons.email),
                border: OutlineInputBorder(),
              ),
              keyboardType: TextInputType.emailAddress,
              validator: (value) {
                if (value == null || value.trim().isEmpty) {
                  return 'Please enter an email address';
                }
                if (!RegExp(r'^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$').hasMatch(value)) {
                  return 'Please enter a valid email address';
                }
                return null;
              },
              autofocus: true,
            ),
            const SizedBox(height: 16),
            CheckboxListTile(
              title: const Text('Include detailed breakdown'),
              subtitle: const Text('Show item details and payment info'),
              value: _includeDetails,
              onChanged: (value) {
                setState(() {
                  _includeDetails = value ?? true;
                });
              },
            ),
          ],
        ),
      ),
      actions: [
        TextButton(
          onPressed: _isSending ? null : () => Navigator.of(context).pop(),
          child: const Text('Cancel'),
        ),
        ElevatedButton.icon(
          onPressed: _isSending ? null : _sendEmail,
          icon: _isSending
              ? const SizedBox(
                  width: 16,
                  height: 16,
                  child: CircularProgressIndicator(strokeWidth: 2),
                )
              : const Icon(Icons.send),
          label: Text(_isSending ? 'Sending...' : 'Send'),
        ),
      ],
    );
  }

  void _sendEmail() async {
    if (_formKey.currentState!.validate()) {
      setState(() {
        _isSending = true;
      });

      // Simulate sending delay
      await Future.delayed(const Duration(seconds: 1));

      Navigator.of(context).pop();
      widget.onSendEmail(_emailController.text.trim(), _includeDetails);
    }
  }
}

enum ReceiptTemplateType {
  standard('Standard'),
  minimal('Minimal'),
  detailed('Detailed'),
  thermal('Thermal');

  const ReceiptTemplateType(this.displayName);
  final String displayName;
