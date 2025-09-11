import 'package:flutter/material.dart';
import 'package:mobile_scanner/mobile_scanner.dart';
import 'package:local_auth/local_auth.dart';
import 'package:vibration/vibration.dart';

import '../../../domain/entities/product_entity.dart';
import '../../../domain/entities/customer_entity.dart';
import '../../../domain/entities/sales_transaction_entity.dart';
import '../../../app/services/customer/customer_identification_service.dart';
import '../../../app/services/customer/loyalty_service.dart';
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
  bool _biometricsAvailable = false;
  
  // Search and filter
  String _searchQuery = '';
  ProductCategory? _selectedCategory;
  
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
      _biometricsAvailable = await _localAuth.canCheckBiometrics;
    } catch (e) {
      _biometricsAvailable = false;
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
            onProductSelected: _addProductToCart,
          ),
          Expanded(
            child: TabBarView(
              controller: _tabController,
              children: [
                POSProductGrid(
                  searchQuery: _searchQuery,
                  selectedCategory: _selectedCategory,
                  onProductTap: _addProductToCart,
                ),
                const POSTransactionList(),
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
              setState(() {
                _searchQuery = value;
              });
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
            biometricsAvailable: _biometricsAvailable,
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
    if (await Vibration.hasVibrator() ?? false) {
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
        total: total,
        customer: _selectedCustomer,
        onPaymentComplete: _onPaymentComplete,
      ),
    );
  }

  void _onPaymentComplete(List<PaymentEntity> payments) {
    // TODO: Complete the transaction
    _createNewTransaction();
    setState(() {});
    
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(
        content: Text('Transaction completed successfully!'),
        backgroundColor: Colors.green,
      ),
    );
  }
}
