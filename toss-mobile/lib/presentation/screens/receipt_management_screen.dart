import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../domain/entities/receipt_entity.dart';
import '../../domain/entities/sales_transaction_entity.dart';
import '../../data/services/receipt_service.dart';
import '../widgets/common/custom_app_bar.dart';
import '../widgets/common/loading_widget.dart';
import 'receipt_preview_screen.dart';
import 'receipt_settings_screen.dart';

class ReceiptManagementScreen extends StatefulWidget {
  final SalesTransactionEntity? transaction;
  final bool showTransaction;

  const ReceiptManagementScreen({
    super.key,
    this.transaction,
    this.showTransaction = false,
  });

  @override
  State<ReceiptManagementScreen> createState() => _ReceiptManagementScreenState();
}

class _ReceiptManagementScreenState extends State<ReceiptManagementScreen>
    with SingleTickerProviderStateMixin {
  late TabController _tabController;
  final ReceiptService _receiptService = ReceiptService();

  @override
  void initState() {
    super.initState();
    _tabController = TabController(
      length: widget.showTransaction ? 3 : 2,
      vsync: this,
    );
  }

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CustomAppBar(
        title: 'Receipt Management',
        bottom: TabBar(
          controller: _tabController,
          tabs: [
            if (widget.showTransaction) const Tab(text: 'Generate'),
            const Tab(text: 'History'),
            const Tab(text: 'Settings'),
          ],
        ),
      ),
      body: TabBarView(
        controller: _tabController,
        children: [
          if (widget.showTransaction)
            ReceiptGenerationTab(transaction: widget.transaction!),
          const ReceiptHistoryTab(),
          const ReceiptSettingsTab(),
        ],
      ),
    );
  }
}

class ReceiptGenerationTab extends StatefulWidget {
  final SalesTransactionEntity transaction;

  const ReceiptGenerationTab({
    super.key,
    required this.transaction,
  });

  @override
  State<ReceiptGenerationTab> createState() => _ReceiptGenerationTabState();
}

class _ReceiptGenerationTabState extends State<ReceiptGenerationTab> {
  final ReceiptService _receiptService = ReceiptService();
  final _emailController = TextEditingController();
  final _phoneController = TextEditingController();
  
  ReceiptFormat _selectedFormat = ReceiptFormat.thermal;
  Set<DeliveryMethod> _selectedMethods = {DeliveryMethod.print};
  bool _isGenerating = false;

  @override
  void dispose() {
    _emailController.dispose();
    _phoneController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(16.0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Transaction Summary Card
          _buildTransactionSummary(),
          const SizedBox(height: 24),

          // Receipt Format Selection
          _buildFormatSelection(),
          const SizedBox(height: 24),

          // Delivery Methods
          _buildDeliveryMethods(),
          const SizedBox(height: 24),

          // Contact Information
          if (_selectedMethods.contains(DeliveryMethod.email) ||
              _selectedMethods.contains(DeliveryMethod.sms) ||
              _selectedMethods.contains(DeliveryMethod.whatsapp))
            _buildContactInformation(),

          const SizedBox(height: 32),

          // Generate Button
          _buildGenerateButton(),
        ],
      ),
    );
  }

  Widget _buildTransactionSummary() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Transaction Summary',
              style: Theme.of(context).textTheme.titleLarge,
            ),
            const SizedBox(height: 16),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text('Transaction ID:'),
                Text(
                  widget.transaction.id ?? 'N/A',
                  style: const TextStyle(fontWeight: FontWeight.bold),
                ),
              ],
            ),
            const SizedBox(height: 8),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text('Items:'),
                Text('${widget.transaction.items.length}'),
              ],
            ),
            const SizedBox(height: 8),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text('Total:'),
                Text(
                  'GHS ${widget.transaction.total.toStringAsFixed(2)}',
                  style: const TextStyle(
                    fontWeight: FontWeight.bold,
                    fontSize: 16,
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildFormatSelection() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Receipt Format',
              style: Theme.of(context).textTheme.titleMedium,
            ),
            const SizedBox(height: 16),
            ...ReceiptFormat.values.map((format) => RadioListTile<ReceiptFormat>(
              title: Text(_getFormatDisplayName(format)),
              subtitle: Text(_getFormatDescription(format)),
              value: format,
              groupValue: _selectedFormat,
              onChanged: (value) {
                setState(() {
                  _selectedFormat = value!;
                });
              },
            )),
          ],
        ),
      ),
    );
  }

  Widget _buildDeliveryMethods() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Delivery Methods',
              style: Theme.of(context).textTheme.titleMedium,
            ),
            const SizedBox(height: 16),
            ...DeliveryMethod.values.where((m) => m != DeliveryMethod.all).map(
              (method) => CheckboxListTile(
                title: Text(_getMethodDisplayName(method)),
                subtitle: Text(_getMethodDescription(method)),
                value: _selectedMethods.contains(method),
                onChanged: (checked) {
                  setState(() {
                    if (checked == true) {
                      _selectedMethods.add(method);
                    } else {
                      _selectedMethods.remove(method);
                    }
                  });
                },
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildContactInformation() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Contact Information',
              style: Theme.of(context).textTheme.titleMedium,
            ),
            const SizedBox(height: 16),
            if (_selectedMethods.contains(DeliveryMethod.email)) ...[
              TextFormField(
                controller: _emailController,
                decoration: const InputDecoration(
                  labelText: 'Email Address',
                  hintText: 'customer@example.com',
                  prefixIcon: Icon(Icons.email),
                ),
                keyboardType: TextInputType.emailAddress,
              ),
              const SizedBox(height: 16),
            ],
            if (_selectedMethods.contains(DeliveryMethod.sms) ||
                _selectedMethods.contains(DeliveryMethod.whatsapp)) ...[
              TextFormField(
                controller: _phoneController,
                decoration: const InputDecoration(
                  labelText: 'Phone Number',
                  hintText: '+233200123456',
                  prefixIcon: Icon(Icons.phone),
                ),
                keyboardType: TextInputType.phone,
              ),
            ],
          ],
        ),
      ),
    );
  }

  Widget _buildGenerateButton() {
    return SizedBox(
      width: double.infinity,
      child: ElevatedButton(
        onPressed: _selectedMethods.isNotEmpty && !_isGenerating
            ? _generateReceipt
            : null,
        child: _isGenerating
            ? const Row(
                mainAxisSize: MainAxisSize.min,
                children: [
                  SizedBox(
                    width: 20,
                    height: 20,
                    child: CircularProgressIndicator(strokeWidth: 2),
                  ),
                  SizedBox(width: 8),
                  Text('Generating...'),
                ],
              )
            : const Text('Generate Receipt'),
      ),
    );
  }

  Future<void> _generateReceipt() async {
    if (_selectedMethods.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Please select at least one delivery method')),
      );
      return;
    }

    // Validate contact information
    if (_selectedMethods.contains(DeliveryMethod.email) &&
        _emailController.text.trim().isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Please enter email address')),
      );
      return;
    }

    if ((_selectedMethods.contains(DeliveryMethod.sms) ||
         _selectedMethods.contains(DeliveryMethod.whatsapp)) &&
        _phoneController.text.trim().isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Please enter phone number')),
      );
      return;
    }

    setState(() {
      _isGenerating = true;
    });

    try {
      final receiptSettings = ReceiptSettings(
        businessName: 'TOSS POS System',
        businessAddress: '123 Main Street, Accra, Ghana',
        businessPhone: '+233200123456',
        taxNumber: 'TAX123456789',
        footerMessage: 'Thank you for your business!',
        showBarcode: true,
        paperWidth: _selectedFormat == ReceiptFormat.pos58 ? 58 : 80,
        logoPath: null,
      );

      final receipt = await _receiptService.generateReceipt(
        transaction: widget.transaction,
        type: ReceiptType.sale,
        settings: receiptSettings,
        format: _selectedFormat,
        deliveryMethods: _selectedMethods.toList(),
        customerEmail: _emailController.text.trim().isNotEmpty
            ? _emailController.text.trim()
            : null,
        customerPhone: _phoneController.text.trim().isNotEmpty
            ? _phoneController.text.trim()
            : null,
      );

      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Receipt generated successfully!'),
            backgroundColor: Colors.green,
          ),
        );

        // Navigate to receipt preview
        Navigator.push(
          context,
          MaterialPageRoute(
            builder: (context) => ReceiptPreviewScreen(receipt: receipt),
          ),
        );
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Failed to generate receipt: $e'),
            backgroundColor: Colors.red,
          ),
        );
      }
    } finally {
      if (mounted) {
        setState(() {
          _isGenerating = false;
        });
      }
    }
  }

  String _getFormatDisplayName(ReceiptFormat format) {
    switch (format) {
      case ReceiptFormat.thermal:
        return 'Thermal (Default)';
      case ReceiptFormat.pos58:
        return 'POS 58mm';
      case ReceiptFormat.pos80:
        return 'POS 80mm';
      case ReceiptFormat.a4:
        return 'A4 PDF';
    }
  }

  String _getFormatDescription(ReceiptFormat format) {
    switch (format) {
      case ReceiptFormat.thermal:
        return 'Standard thermal printer format';
      case ReceiptFormat.pos58:
        return 'Small format thermal printer (58mm)';
      case ReceiptFormat.pos80:
        return 'Standard format thermal printer (80mm)';
      case ReceiptFormat.a4:
        return 'Full-size PDF document';
    }
  }

  String _getMethodDisplayName(DeliveryMethod method) {
    switch (method) {
      case DeliveryMethod.print:
        return 'Print';
      case DeliveryMethod.email:
        return 'Email';
      case DeliveryMethod.sms:
        return 'SMS';
      case DeliveryMethod.whatsapp:
        return 'WhatsApp';
      case DeliveryMethod.all:
        return 'All Methods';
    }
  }

  String _getMethodDescription(DeliveryMethod method) {
    switch (method) {
      case DeliveryMethod.print:
        return 'Print to thermal printer';
      case DeliveryMethod.email:
        return 'Send PDF via email';
      case DeliveryMethod.sms:
        return 'Send summary via SMS';
      case DeliveryMethod.whatsapp:
        return 'Send via WhatsApp';
      case DeliveryMethod.all:
        return 'Use all available methods';
    }
  }
}

class ReceiptHistoryTab extends StatefulWidget {
  const ReceiptHistoryTab({super.key});

  @override
  State<ReceiptHistoryTab> createState() => _ReceiptHistoryTabState();
}

class _ReceiptHistoryTabState extends State<ReceiptHistoryTab> {
  final ReceiptService _receiptService = ReceiptService();
  final _searchController = TextEditingController();
  
  List<ReceiptEntity> _receipts = [];
  List<ReceiptEntity> _filteredReceipts = [];
  bool _isLoading = true;
  ReceiptType? _selectedType;
  DateTimeRange? _dateRange;

  @override
  void initState() {
    super.initState();
    _loadReceipts();
    _searchController.addListener(_filterReceipts);
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  Future<void> _loadReceipts() async {
    setState(() {
      _isLoading = true;
    });

    try {
      final receipts = await _receiptService.getReceiptHistory(
        type: _selectedType,
        startDate: _dateRange?.start,
        endDate: _dateRange?.end,
      );

      setState(() {
        _receipts = receipts;
        _filteredReceipts = receipts;
        _isLoading = false;
      });
    } catch (e) {
      setState(() {
        _isLoading = false;
      });
      
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Failed to load receipts: $e')),
        );
      }
    }
  }

  void _filterReceipts() {
    final query = _searchController.text.toLowerCase();
    setState(() {
      _filteredReceipts = _receipts.where((receipt) {
        return receipt.receiptNumber.toLowerCase().contains(query) ||
               receipt.transactionId.toLowerCase().contains(query) ||
               (receipt.customer?.name?.toLowerCase().contains(query) ?? false);
      }).toList();
    });
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        // Search and Filters
        Padding(
          padding: const EdgeInsets.all(16.0),
          child: Column(
            children: [
              TextField(
                controller: _searchController,
                decoration: const InputDecoration(
                  hintText: 'Search receipts...',
                  prefixIcon: Icon(Icons.search),
                ),
              ),
              const SizedBox(height: 16),
              Row(
                children: [
                  Expanded(
                    child: DropdownButtonFormField<ReceiptType?>(
                      value: _selectedType,
                      decoration: const InputDecoration(
                        labelText: 'Type',
                        contentPadding: EdgeInsets.symmetric(horizontal: 12),
                      ),
                      items: [
                        const DropdownMenuItem(
                          value: null,
                          child: Text('All Types'),
                        ),
                        ...ReceiptType.values.map((type) => DropdownMenuItem(
                          value: type,
                          child: Text(type.name.toUpperCase()),
                        )),
                      ],
                      onChanged: (value) {
                        setState(() {
                          _selectedType = value;
                        });
                        _loadReceipts();
                      },
                    ),
                  ),
                  const SizedBox(width: 16),
                  Expanded(
                    child: ElevatedButton.icon(
                      onPressed: _selectDateRange,
                      icon: const Icon(Icons.date_range),
                      label: Text(_dateRange == null
                          ? 'Date Range'
                          : '${_dateRange!.start.day}/${_dateRange!.start.month} - ${_dateRange!.end.day}/${_dateRange!.end.month}'),
                    ),
                  ),
                ],
              ),
            ],
          ),
        ),

        // Receipt List
        Expanded(
          child: _isLoading
              ? const LoadingWidget()
              : _filteredReceipts.isEmpty
                  ? const Center(
                      child: Column(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          Icon(Icons.receipt_long, size: 64, color: Colors.grey),
                          SizedBox(height: 16),
                          Text('No receipts found'),
                        ],
                      ),
                    )
                  : ListView.builder(
                      itemCount: _filteredReceipts.length,
                      itemBuilder: (context, index) {
                        final receipt = _filteredReceipts[index];
                        return _buildReceiptCard(receipt);
                      },
                    ),
        ),
      ],
    );
  }

  Widget _buildReceiptCard(ReceiptEntity receipt) {
    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 4),
      child: ListTile(
        leading: CircleAvatar(
          backgroundColor: _getReceiptTypeColor(receipt.type),
          child: Icon(
            _getReceiptTypeIcon(receipt.type),
            color: Colors.white,
            size: 20,
          ),
        ),
        title: Text(
          'Receipt #${receipt.receiptNumber}',
          style: const TextStyle(fontWeight: FontWeight.bold),
        ),
        subtitle: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text('${receipt.createdAt.day}/${receipt.createdAt.month}/${receipt.createdAt.year} '
                 '${receipt.createdAt.hour}:${receipt.createdAt.minute.toString().padLeft(2, '0')}'),
            if (receipt.customer?.name != null)
              Text('Customer: ${receipt.customer!.name}'),
            if (receipt.isReprint)
              const Text(
                'REPRINT',
                style: TextStyle(
                  color: Colors.orange,
                  fontWeight: FontWeight.bold,
                  fontSize: 12,
                ),
              ),
          ],
        ),
        trailing: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          crossAxisAlignment: CrossAxisAlignment.end,
          children: [
            Text(
              'GHS ${receipt.totals.total.toStringAsFixed(2)}',
              style: const TextStyle(
                fontWeight: FontWeight.bold,
                fontSize: 16,
              ),
            ),
            Text(
              receipt.type.name.toUpperCase(),
              style: TextStyle(
                color: _getReceiptTypeColor(receipt.type),
                fontSize: 12,
                fontWeight: FontWeight.bold,
              ),
            ),
          ],
        ),
        onTap: () => _viewReceipt(receipt),
        onLongPress: () => _showReceiptOptions(receipt),
      ),
    );
  }

  void _viewReceipt(ReceiptEntity receipt) {
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => ReceiptPreviewScreen(receipt: receipt),
      ),
    );
  }

  void _showReceiptOptions(ReceiptEntity receipt) {
    showModalBottomSheet(
      context: context,
      builder: (context) => Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          ListTile(
            leading: const Icon(Icons.visibility),
            title: const Text('View Receipt'),
            onTap: () {
              Navigator.pop(context);
              _viewReceipt(receipt);
            },
          ),
          ListTile(
            leading: const Icon(Icons.print),
            title: const Text('Reprint'),
            onTap: () {
              Navigator.pop(context);
              _reprintReceipt(receipt);
            },
          ),
          ListTile(
            leading: const Icon(Icons.email),
            title: const Text('Send by Email'),
            onTap: () {
              Navigator.pop(context);
              _resendReceipt(receipt, DeliveryMethod.email);
            },
          ),
          ListTile(
            leading: const Icon(Icons.sms),
            title: const Text('Send by SMS'),
            onTap: () {
              Navigator.pop(context);
              _resendReceipt(receipt, DeliveryMethod.sms);
            },
          ),
        ],
      ),
    );
  }

  Future<void> _reprintReceipt(ReceiptEntity receipt) async {
    try {
      await _receiptService.reprintReceipt(receipt.id);
      
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Receipt reprinted successfully'),
            backgroundColor: Colors.green,
          ),
        );
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Failed to reprint receipt: $e'),
            backgroundColor: Colors.red,
          ),
        );
      }
    }
  }

  Future<void> _resendReceipt(ReceiptEntity receipt, DeliveryMethod method) async {
    // For demonstration purposes, show a dialog to get contact info
    String? contact;
    
    if (method == DeliveryMethod.email) {
      contact = await _showContactDialog('Email Address', 'Enter email address');
    } else if (method == DeliveryMethod.sms) {
      contact = await _showContactDialog('Phone Number', 'Enter phone number');
    }
    
    if (contact != null) {
      try {
        // In a real implementation, you would call a resend method
        await Future.delayed(const Duration(seconds: 1));
        
        if (mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(
              content: Text('Receipt sent via ${method.name}'),
              backgroundColor: Colors.green,
            ),
          );
        }
      } catch (e) {
        if (mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(
              content: Text('Failed to send receipt: $e'),
              backgroundColor: Colors.red,
            ),
          );
        }
      }
    }
  }

  Future<String?> _showContactDialog(String title, String hint) async {
    final controller = TextEditingController();
    
    return showDialog<String>(
      context: context,
      builder: (context) => AlertDialog(
        title: Text(title),
        content: TextField(
          controller: controller,
          decoration: InputDecoration(hintText: hint),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancel'),
          ),
          TextButton(
            onPressed: () => Navigator.pop(context, controller.text.trim()),
            child: const Text('Send'),
          ),
        ],
      ),
    );
  }

  Future<void> _selectDateRange() async {
    final range = await showDateRangePicker(
      context: context,
      firstDate: DateTime.now().subtract(const Duration(days: 365)),
      lastDate: DateTime.now(),
      initialDateRange: _dateRange,
    );

    if (range != null) {
      setState(() {
        _dateRange = range;
      });
      _loadReceipts();
    }
  }

  Color _getReceiptTypeColor(ReceiptType type) {
    switch (type) {
      case ReceiptType.sale:
        return Colors.green;
      case ReceiptType.refund:
        return Colors.red;
      case ReceiptType.layaway:
        return Colors.blue;
      case ReceiptType.quote:
        return Colors.orange;
    }
  }

  IconData _getReceiptTypeIcon(ReceiptType type) {
    switch (type) {
      case ReceiptType.sale:
        return Icons.shopping_cart;
      case ReceiptType.refund:
        return Icons.keyboard_return;
      case ReceiptType.layaway:
        return Icons.schedule;
      case ReceiptType.quote:
        return Icons.request_quote;
    }
  }
}

class ReceiptSettingsTab extends StatelessWidget {
  const ReceiptSettingsTab({super.key});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(16.0),
      child: Column(
        children: [
          Card(
            child: ListTile(
              leading: const Icon(Icons.print),
              title: const Text('Printer Settings'),
              subtitle: const Text('Configure thermal printers'),
              trailing: const Icon(Icons.arrow_forward_ios),
              onTap: () {
                Navigator.push(
                  context,
                  MaterialPageRoute(
                    builder: (context) => const ReceiptSettingsScreen(),
                  ),
                );
              },
            ),
          ),
          const SizedBox(height: 8),
          Card(
            child: ListTile(
              leading: const Icon(Icons.business),
              title: const Text('Business Information'),
              subtitle: const Text('Update business details'),
              trailing: const Icon(Icons.arrow_forward_ios),
              onTap: () {
                // Navigate to business settings
              },
            ),
          ),
          const SizedBox(height: 8),
          Card(
            child: ListTile(
              leading: const Icon(Icons.design_services),
              title: const Text('Receipt Templates'),
              subtitle: const Text('Customize receipt layout'),
              trailing: const Icon(Icons.arrow_forward_ios),
              onTap: () {
                // Navigate to template settings
              },
            ),
          ),
        ],
      ),
    );
  }
}
