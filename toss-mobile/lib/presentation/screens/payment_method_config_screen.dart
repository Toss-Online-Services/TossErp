import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../../domain/entities/payment_entity.dart';
import '../../data/repositories/payment_repository.dart';

class PaymentMethodConfigScreen extends StatefulWidget {
  const PaymentMethodConfigScreen({Key? key}) : super(key: key);

  @override
  State<PaymentMethodConfigScreen> createState() =>
      _PaymentMethodConfigScreenState();
}

class _PaymentMethodConfigScreenState extends State<PaymentMethodConfigScreen>
    with TickerProviderStateMixin {
  final PaymentRepository _paymentRepository = PaymentRepository();
  
  late TabController _tabController;
  List<PaymentGatewayConfig> _gateways = [];
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 3, vsync: this);
    _loadPaymentGateways();
  }

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  void _loadPaymentGateways() {
    setState(() {
      _isLoading = true;
    });

    _paymentRepository.initializeGateways();
    
    // Get all configured gateways
    _gateways = [
      _paymentRepository.getGateway('paystack'),
      _paymentRepository.getGateway('flutterwave'),
      _paymentRepository.getGateway('stripe'),
      _paymentRepository.getGateway('local_momo'),
    ].where((gateway) => gateway != null).cast<PaymentGatewayConfig>().toList();

    setState(() {
      _isLoading = false;
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Payment Methods'),
        backgroundColor: Theme.of(context).primaryColor,
        foregroundColor: Colors.white,
        bottom: TabBar(
          controller: _tabController,
          tabs: const [
            Tab(icon: Icon(Icons.settings), text: 'Gateways'),
            Tab(icon: Icon(Icons.payment), text: 'Methods'),
            Tab(icon: Icon(Icons.analytics), text: 'Analytics'),
          ],
        ),
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : TabBarView(
              controller: _tabController,
              children: [
                _buildGatewaysTab(),
                _buildPaymentMethodsTab(),
                _buildAnalyticsTab(),
              ],
            ),
    );
  }

  Widget _buildGatewaysTab() {
    return ListView.builder(
      padding: const EdgeInsets.all(16.0),
      itemCount: _gateways.length,
      itemBuilder: (context, index) {
        final gateway = _gateways[index];
        return _buildGatewayCard(gateway);
      },
    );
  }

  Widget _buildGatewayCard(PaymentGatewayConfig gateway) {
    return Card(
      margin: const EdgeInsets.only(bottom: 16.0),
      child: ExpansionTile(
        leading: _getGatewayIcon(gateway.gatewayType),
        title: Text(
          gateway.name,
          style: const TextStyle(fontWeight: FontWeight.bold),
        ),
        subtitle: Text(
          gateway.isEnabled ? 'Active' : 'Inactive',
          style: TextStyle(
            color: gateway.isEnabled ? Colors.green : Colors.red,
          ),
        ),
        trailing: Switch(
          value: gateway.isEnabled,
          onChanged: (value) {
            _toggleGateway(gateway, value);
          },
        ),
        children: [
          Padding(
            padding: const EdgeInsets.all(16.0),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                _buildGatewayInfo(gateway),
                const SizedBox(height: 16),
                _buildSupportedMethods(gateway),
                const SizedBox(height: 16),
                _buildSupportedCurrencies(gateway),
                const SizedBox(height: 16),
                _buildLimits(gateway),
                const SizedBox(height: 16),
                _buildGatewayActions(gateway),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildGatewayInfo(PaymentGatewayConfig gateway) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'Gateway Information',
          style: Theme.of(context).textTheme.titleMedium?.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: 8),
        _buildInfoRow('Gateway Type', gateway.gatewayType.toUpperCase()),
        _buildInfoRow('Gateway ID', gateway.id),
        if (gateway.credentials.containsKey('publicKey'))
          _buildInfoRow('Public Key', _maskCredential(gateway.credentials['publicKey']!)),
        if (gateway.credentials.containsKey('baseUrl'))
          _buildInfoRow('Base URL', gateway.credentials['baseUrl']!),
      ],
    );
  }

  Widget _buildInfoRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4.0),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 100,
            child: Text(
              '$label:',
              style: const TextStyle(fontWeight: FontWeight.w500),
            ),
          ),
          Expanded(
            child: Text(
              value,
              style: const TextStyle(color: Colors.grey),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildSupportedMethods(PaymentGatewayConfig gateway) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'Supported Payment Methods',
          style: Theme.of(context).textTheme.titleMedium?.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: 8),
        Wrap(
          spacing: 8.0,
          runSpacing: 4.0,
          children: gateway.supportedMethods.map((method) {
            return Chip(
              avatar: _getPaymentMethodIcon(method),
              label: Text(_getPaymentMethodName(method)),
              backgroundColor: Theme.of(context).primaryColor.withOpacity(0.1),
            );
          }).toList(),
        ),
      ],
    );
  }

  Widget _buildSupportedCurrencies(PaymentGatewayConfig gateway) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'Supported Currencies',
          style: Theme.of(context).textTheme.titleMedium?.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: 8),
        Wrap(
          spacing: 8.0,
          runSpacing: 4.0,
          children: gateway.supportedCurrencies.map((currency) {
            return Chip(
              label: Text(currency),
              backgroundColor: Colors.blue.withOpacity(0.1),
            );
          }).toList(),
        ),
      ],
    );
  }

  Widget _buildLimits(PaymentGatewayConfig gateway) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'Transaction Limits',
          style: Theme.of(context).textTheme.titleMedium?.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: 8),
        if (gateway.minimumAmount != null)
          _buildInfoRow(
            'Minimum Amount',
            '${gateway.supportedCurrencies.first} ${gateway.minimumAmount!.toStringAsFixed(2)}',
          ),
        if (gateway.maximumAmount != null)
          _buildInfoRow(
            'Maximum Amount',
            '${gateway.supportedCurrencies.first} ${gateway.maximumAmount!.toStringAsFixed(2)}',
          ),
        if (gateway.minimumAmount == null && gateway.maximumAmount == null)
          const Text(
            'No limits configured',
            style: TextStyle(color: Colors.grey),
          ),
      ],
    );
  }

  Widget _buildGatewayActions(PaymentGatewayConfig gateway) {
    return Row(
      children: [
        Expanded(
          child: ElevatedButton.icon(
            onPressed: () => _testGateway(gateway),
            icon: const Icon(Icons.play_arrow),
            label: const Text('Test Gateway'),
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.blue,
              foregroundColor: Colors.white,
            ),
          ),
        ),
        const SizedBox(width: 8),
        Expanded(
          child: ElevatedButton.icon(
            onPressed: () => _configureGateway(gateway),
            icon: const Icon(Icons.settings),
            label: const Text('Configure'),
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.orange,
              foregroundColor: Colors.white,
            ),
          ),
        ),
      ],
    );
  }

  Widget _buildPaymentMethodsTab() {
    final availableMethods = PaymentMethod.values;
    
    return ListView.builder(
      padding: const EdgeInsets.all(16.0),
      itemCount: availableMethods.length,
      itemBuilder: (context, index) {
        final method = availableMethods[index];
        final supportingGateways = _gateways
            .where((gateway) => gateway.supportedMethods.contains(method))
            .toList();
        
        return Card(
          margin: const EdgeInsets.only(bottom: 12.0),
          child: ListTile(
            leading: _getPaymentMethodIcon(method),
            title: Text(_getPaymentMethodName(method)),
            subtitle: Text(
              '${supportingGateways.length} gateway(s) support this method',
            ),
            trailing: Chip(
              label: Text(
                supportingGateways.any((g) => g.isEnabled) ? 'Available' : 'Unavailable',
              ),
              backgroundColor: supportingGateways.any((g) => g.isEnabled)
                  ? Colors.green.withOpacity(0.2)
                  : Colors.red.withOpacity(0.2),
            ),
            onTap: () => _showMethodDetails(method, supportingGateways),
          ),
        );
      },
    );
  }

  Widget _buildAnalyticsTab() {
    return Padding(
      padding: const EdgeInsets.all(16.0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Payment Analytics',
            style: Theme.of(context).textTheme.headlineSmall?.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 16),
          
          // Summary cards
          Row(
            children: [
              Expanded(
                child: _buildAnalyticsCard(
                  'Total Transactions',
                  '1,234',
                  Icons.receipt_long,
                  Colors.blue,
                ),
              ),
              const SizedBox(width: 8),
              Expanded(
                child: _buildAnalyticsCard(
                  'Success Rate',
                  '94.5%',
                  Icons.check_circle,
                  Colors.green,
                ),
              ),
            ],
          ),
          const SizedBox(height: 8),
          Row(
            children: [
              Expanded(
                child: _buildAnalyticsCard(
                  'Total Volume',
                  'GHS 45,672',
                  Icons.trending_up,
                  Colors.orange,
                ),
              ),
              const SizedBox(width: 8),
              Expanded(
                child: _buildAnalyticsCard(
                  'Processing Fees',
                  'GHS 892',
                  Icons.money_off,
                  Colors.red,
                ),
              ),
            ],
          ),
          const SizedBox(height: 24),
          
          // Gateway performance
          Text(
            'Gateway Performance (Last 30 Days)',
            style: Theme.of(context).textTheme.titleLarge?.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 16),
          
          Expanded(
            child: ListView.builder(
              itemCount: _gateways.length,
              itemBuilder: (context, index) {
                final gateway = _gateways[index];
                return _buildGatewayPerformanceCard(gateway);
              },
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildAnalyticsCard(
    String title,
    String value,
    IconData icon,
    Color color,
  ) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Icon(icon, color: color, size: 20),
                const SizedBox(width: 8),
                Expanded(
                  child: Text(
                    title,
                    style: const TextStyle(
                      fontSize: 12,
                      color: Colors.grey,
                    ),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 8),
            Text(
              value,
              style: const TextStyle(
                fontSize: 20,
                fontWeight: FontWeight.bold,
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildGatewayPerformanceCard(PaymentGatewayConfig gateway) {
    // Mock performance data
    final successRate = 85 + (gateway.id.hashCode % 15);
    final avgResponseTime = 1.2 + (gateway.id.hashCode % 5) * 0.3;
    final transactionCount = 100 + (gateway.id.hashCode % 500);
    
    return Card(
      margin: const EdgeInsets.only(bottom: 12.0),
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                _getGatewayIcon(gateway.gatewayType),
                const SizedBox(width: 12),
                Expanded(
                  child: Text(
                    gateway.name,
                    style: const TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
                Container(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 8.0,
                    vertical: 4.0,
                  ),
                  decoration: BoxDecoration(
                    color: gateway.isEnabled
                        ? Colors.green.withOpacity(0.2)
                        : Colors.red.withOpacity(0.2),
                    borderRadius: BorderRadius.circular(12.0),
                  ),
                  child: Text(
                    gateway.isEnabled ? 'Active' : 'Inactive',
                    style: TextStyle(
                      fontSize: 12,
                      color: gateway.isEnabled ? Colors.green : Colors.red,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 16),
            Row(
              children: [
                Expanded(
                  child: _buildPerformanceMetric(
                    'Success Rate',
                    '$successRate%',
                    successRate >= 90 ? Colors.green : 
                    successRate >= 80 ? Colors.orange : Colors.red,
                  ),
                ),
                Expanded(
                  child: _buildPerformanceMetric(
                    'Avg Response',
                    '${avgResponseTime.toStringAsFixed(1)}s',
                    avgResponseTime <= 2.0 ? Colors.green :
                    avgResponseTime <= 3.0 ? Colors.orange : Colors.red,
                  ),
                ),
                Expanded(
                  child: _buildPerformanceMetric(
                    'Transactions',
                    '$transactionCount',
                    Colors.blue,
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildPerformanceMetric(String label, String value, Color color) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.center,
      children: [
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
          label,
          style: const TextStyle(
            fontSize: 12,
            color: Colors.grey,
          ),
          textAlign: TextAlign.center,
        ),
      ],
    );
  }

  // Helper methods
  Icon _getGatewayIcon(String gatewayType) {
    switch (gatewayType.toLowerCase()) {
      case 'paystack':
        return const Icon(Icons.credit_card, color: Colors.blue);
      case 'flutterwave':
        return const Icon(Icons.payment, color: Colors.orange);
      case 'stripe':
        return const Icon(Icons.credit_score, color: Colors.purple);
      case 'local':
        return const Icon(Icons.phone_android, color: Colors.green);
      default:
        return const Icon(Icons.payment, color: Colors.grey);
    }
  }

  Icon _getPaymentMethodIcon(PaymentMethod method) {
    switch (method) {
      case PaymentMethod.cash:
        return const Icon(Icons.money, color: Colors.green);
      case PaymentMethod.card:
        return const Icon(Icons.credit_card, color: Colors.blue);
      case PaymentMethod.mobileMoney:
      case PaymentMethod.mobileWallet:
        return const Icon(Icons.phone_android, color: Colors.orange);
      case PaymentMethod.qrPayment:
        return const Icon(Icons.qr_code, color: Colors.purple);
      case PaymentMethod.nfc:
        return const Icon(Icons.nfc, color: Colors.indigo);
      case PaymentMethod.bankTransfer:
        return const Icon(Icons.account_balance, color: Colors.teal);
      case PaymentMethod.storeCredit:
        return const Icon(Icons.account_balance_wallet, color: Colors.amber);
      case PaymentMethod.loyaltyPoints:
        return const Icon(Icons.stars, color: Colors.pink);
      default:
        return const Icon(Icons.payment, color: Colors.grey);
    }
  }

  String _getPaymentMethodName(PaymentMethod method) {
    switch (method) {
      case PaymentMethod.cash:
        return 'Cash';
      case PaymentMethod.card:
        return 'Credit/Debit Card';
      case PaymentMethod.mobileMoney:
        return 'Mobile Money';
      case PaymentMethod.mobileWallet:
        return 'Mobile Wallet';
      case PaymentMethod.qrPayment:
        return 'QR Code Payment';
      case PaymentMethod.nfc:
        return 'Contactless (NFC)';
      case PaymentMethod.bankTransfer:
        return 'Bank Transfer';
      case PaymentMethod.storeCredit:
        return 'Store Credit';
      case PaymentMethod.loyaltyPoints:
        return 'Loyalty Points';
      case PaymentMethod.voucher:
        return 'Voucher';
      case PaymentMethod.creditAccount:
        return 'Credit Account';
      default:
        return 'Other';
    }
  }

  String _maskCredential(String credential) {
    if (credential.length <= 8) {
      return '*' * credential.length;
    }
    final start = credential.substring(0, 4);
    final end = credential.substring(credential.length - 4);
    final middle = '*' * (credential.length - 8);
    return '$start$middle$end';
  }

  void _toggleGateway(PaymentGatewayConfig gateway, bool enabled) {
    // In a real app, this would update the gateway configuration
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(
          '${gateway.name} ${enabled ? 'enabled' : 'disabled'}',
        ),
        backgroundColor: enabled ? Colors.green : Colors.red,
      ),
    );
  }

  void _testGateway(PaymentGatewayConfig gateway) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: Text('Test ${gateway.name}'),
        content: const Text('This will perform a test transaction with this gateway.'),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(),
            child: const Text('Cancel'),
          ),
          ElevatedButton(
            onPressed: () {
              Navigator.of(context).pop();
              _performGatewayTest(gateway);
            },
            child: const Text('Start Test'),
          ),
        ],
      ),
    );
  }

  void _performGatewayTest(PaymentGatewayConfig gateway) {
    // Show loading dialog
    showDialog(
      context: context,
      barrierDismissible: false,
      builder: (context) => const AlertDialog(
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            CircularProgressIndicator(),
            SizedBox(height: 16),
            Text('Testing gateway connection...'),
          ],
        ),
      ),
    );

    // Simulate test
    Future.delayed(const Duration(seconds: 3), () {
      Navigator.of(context).pop(); // Close loading dialog
      
      // Show result
      showDialog(
        context: context,
        builder: (context) => AlertDialog(
          title: Row(
            children: [
              const Icon(Icons.check_circle, color: Colors.green),
              const SizedBox(width: 8),
              Text('${gateway.name} Test'),
            ],
          ),
          content: const Text('Gateway test completed successfully!'),
          actions: [
            TextButton(
              onPressed: () => Navigator.of(context).pop(),
              child: const Text('OK'),
            ),
          ],
        ),
      );
    });
  }

  void _configureGateway(PaymentGatewayConfig gateway) {
    Navigator.of(context).push(
      MaterialPageRoute(
        builder: (context) => GatewayConfigScreen(gateway: gateway),
      ),
    );
  }

  void _showMethodDetails(
    PaymentMethod method,
    List<PaymentGatewayConfig> supportingGateways,
  ) {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      builder: (context) => DraggableScrollableSheet(
        initialChildSize: 0.7,
        maxChildSize: 0.9,
        minChildSize: 0.5,
        builder: (context, scrollController) => Container(
          padding: const EdgeInsets.all(16.0),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  _getPaymentMethodIcon(method),
                  const SizedBox(width: 12),
                  Expanded(
                    child: Text(
                      _getPaymentMethodName(method),
                      style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 16),
              Text(
                'Supporting Gateways',
                style: Theme.of(context).textTheme.titleMedium?.copyWith(
                  fontWeight: FontWeight.bold,
                ),
              ),
              const SizedBox(height: 8),
              Expanded(
                child: ListView.builder(
                  controller: scrollController,
                  itemCount: supportingGateways.length,
                  itemBuilder: (context, index) {
                    final gateway = supportingGateways[index];
                    return Card(
                      child: ListTile(
                        leading: _getGatewayIcon(gateway.gatewayType),
                        title: Text(gateway.name),
                        subtitle: Text(
                          gateway.isEnabled ? 'Active' : 'Inactive',
                        ),
                        trailing: gateway.isEnabled
                            ? const Icon(Icons.check_circle, color: Colors.green)
                            : const Icon(Icons.cancel, color: Colors.red),
                      ),
                    );
                  },
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

// Gateway configuration screen
class GatewayConfigScreen extends StatefulWidget {
  final PaymentGatewayConfig gateway;

  const GatewayConfigScreen({
    Key? key,
    required this.gateway,
  }) : super(key: key);

  @override
  State<GatewayConfigScreen> createState() => _GatewayConfigScreenState();
}

class _GatewayConfigScreenState extends State<GatewayConfigScreen> {
  final _formKey = GlobalKey<FormState>();
  late Map<String, TextEditingController> _controllers;

  @override
  void initState() {
    super.initState();
    _controllers = {};
    
    // Initialize controllers for each credential
    for (final entry in widget.gateway.credentials.entries) {
      _controllers[entry.key] = TextEditingController(text: entry.value);
    }
  }

  @override
  void dispose() {
    for (final controller in _controllers.values) {
      controller.dispose();
    }
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('Configure ${widget.gateway.name}'),
        backgroundColor: Theme.of(context).primaryColor,
        foregroundColor: Colors.white,
        actions: [
          TextButton(
            onPressed: _saveConfiguration,
            child: const Text(
              'Save',
              style: TextStyle(color: Colors.white),
            ),
          ),
        ],
      ),
      body: Form(
        key: _formKey,
        child: ListView(
          padding: const EdgeInsets.all(16.0),
          children: [
            Text(
              'Gateway Credentials',
              style: Theme.of(context).textTheme.titleLarge?.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 16),
            
            ..._controllers.entries.map((entry) {
              final key = entry.key;
              final controller = entry.value;
              
              return Padding(
                padding: const EdgeInsets.only(bottom: 16.0),
                child: TextFormField(
                  controller: controller,
                  decoration: InputDecoration(
                    labelText: _formatFieldLabel(key),
                    border: const OutlineInputBorder(),
                    helperText: _getFieldHelperText(key),
                  ),
                  obscureText: _isSecretField(key),
                  validator: (value) {
                    if (value == null || value.isEmpty) {
                      return 'This field is required';
                    }
                    return null;
                  },
                ),
              );
            }),
            
            const SizedBox(height: 24),
            
            Text(
              'Gateway Settings',
              style: Theme.of(context).textTheme.titleLarge?.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 16),
            
            SwitchListTile(
              title: const Text('Enable Gateway'),
              subtitle: const Text('Allow this gateway to process payments'),
              value: widget.gateway.isEnabled,
              onChanged: (value) {
                // Handle gateway enable/disable
              },
            ),
            
            const SizedBox(height: 32),
            
            ElevatedButton.icon(
              onPressed: _testConnection,
              icon: const Icon(Icons.wifi_protected_setup),
              label: const Text('Test Connection'),
              style: ElevatedButton.styleFrom(
                backgroundColor: Colors.blue,
                foregroundColor: Colors.white,
                padding: const EdgeInsets.all(16.0),
              ),
            ),
          ],
        ),
      ),
    );
  }

  String _formatFieldLabel(String key) {
    return key.split(/(?=[A-Z])/).join(' ').toUpperCase();
  }

  String? _getFieldHelperText(String key) {
    switch (key.toLowerCase()) {
      case 'publickey':
        return 'Public key from your gateway dashboard';
      case 'secretkey':
        return 'Secret key (keep this secure)';
      case 'baseurl':
        return 'API base URL for the gateway';
      case 'apikey':
        return 'API key for authentication';
      default:
        return null;
    }
  }

  bool _isSecretField(String key) {
    return key.toLowerCase().contains('secret') ||
           key.toLowerCase().contains('private') ||
           key.toLowerCase().contains('key') && !key.toLowerCase().contains('public');
  }

  void _saveConfiguration() {
    if (_formKey.currentState!.validate()) {
      // In a real app, save the configuration
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Configuration saved successfully'),
          backgroundColor: Colors.green,
        ),
      );
      Navigator.of(context).pop();
    }
  }

  void _testConnection() {
    showDialog(
      context: context,
      barrierDismissible: false,
      builder: (context) => const AlertDialog(
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            CircularProgressIndicator(),
            SizedBox(height: 16),
            Text('Testing connection...'),
          ],
        ),
      ),
    );

    // Simulate connection test
    Future.delayed(const Duration(seconds: 2), () {
      Navigator.of(context).pop();
      
      showDialog(
        context: context,
        builder: (context) => AlertDialog(
          title: const Row(
            children: [
              Icon(Icons.check_circle, color: Colors.green),
              SizedBox(width: 8),
              Text('Connection Test'),
            ],
          ),
          content: const Text('Connection to gateway successful!'),
          actions: [
            TextButton(
              onPressed: () => Navigator.of(context).pop(),
              child: const Text('OK'),
            ),
          ],
        ),
      );
    });
  }
}
