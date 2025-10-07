import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../../domain/entities/receipt_entity.dart';
import '../../data/services/receipt_service.dart';
import '../widgets/common/custom_app_bar.dart';
import '../widgets/common/loading_widget.dart';

class ReceiptSettingsScreen extends StatefulWidget {
  const ReceiptSettingsScreen({super.key});

  @override
  State<ReceiptSettingsScreen> createState() => _ReceiptSettingsScreenState();
}

class _ReceiptSettingsScreenState extends State<ReceiptSettingsScreen>
    with SingleTickerProviderStateMixin {
  late TabController _tabController;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 3, vsync: this);
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
        title: 'Receipt Settings',
        bottom: TabBar(
          controller: _tabController,
          tabs: const [
            Tab(text: 'Printers'),
            Tab(text: 'Business'),
            Tab(text: 'Templates'),
          ],
        ),
      ),
      body: TabBarView(
        controller: _tabController,
        children: const [
          PrinterSettingsTab(),
          BusinessSettingsTab(),
          TemplateSettingsTab(),
        ],
      ),
    );
  }
}

class PrinterSettingsTab extends StatefulWidget {
  const PrinterSettingsTab({super.key});

  @override
  State<PrinterSettingsTab> createState() => _PrinterSettingsTabState();
}

class _PrinterSettingsTabState extends State<PrinterSettingsTab> {
  List<PrinterConfig> _printers = [];
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _loadPrinters();
  }

  Future<void> _loadPrinters() async {
    setState(() {
      _isLoading = true;
    });

    try {
      // Mock implementation for now - in real app, use proper repository
      final printers = <PrinterConfig>[];
      setState(() {
        _printers = printers;
        _isLoading = false;
      });
    } catch (e) {
      setState(() {
        _isLoading = false;
      });
      
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Failed to load printers: $e')),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        // Add Printer Button
        Padding(
          padding: const EdgeInsets.all(16.0),
          child: SizedBox(
            width: double.infinity,
            child: ElevatedButton.icon(
              onPressed: _addPrinter,
              icon: const Icon(Icons.add),
              label: const Text('Add Printer'),
            ),
          ),
        ),

        // Printers List
        Expanded(
          child: _isLoading
              ? const LoadingWidget()
              : _printers.isEmpty
                  ? const Center(
                      child: Column(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          Icon(Icons.print_disabled, size: 64, color: Colors.grey),
                          SizedBox(height: 16),
                          Text('No printers configured'),
                          SizedBox(height: 8),
                          Text(
                            'Add a printer to start printing receipts',
                            style: TextStyle(color: Colors.grey),
                          ),
                        ],
                      ),
                    )
                  : ListView.builder(
                      itemCount: _printers.length,
                      itemBuilder: (context, index) {
                        final printer = _printers[index];
                        return _buildPrinterCard(printer);
                      },
                    ),
        ),
      ],
    );
  }

  Widget _buildPrinterCard(PrinterConfig printer) {
    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 4),
      child: ListTile(
        leading: CircleAvatar(
          backgroundColor: printer.isActive ? Colors.green : Colors.grey,
          child: Icon(
            _getPrinterTypeIcon(printer.type),
            color: Colors.white,
            size: 20,
          ),
        ),
        title: Text(
          printer.name,
          style: TextStyle(
            fontWeight: printer.isDefault ? FontWeight.bold : FontWeight.normal,
          ),
        ),
        subtitle: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(printer.type.toUpperCase()),
            Text(
              '${printer.paperWidth}mm - ${printer.defaultFormat.name}',
              style: const TextStyle(fontSize: 12),
            ),
            if (printer.isDefault)
              const Text(
                'DEFAULT PRINTER',
                style: TextStyle(
                  color: Colors.blue,
                  fontWeight: FontWeight.bold,
                  fontSize: 12,
                ),
              ),
          ],
        ),
        trailing: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Switch(
              value: printer.isActive,
              onChanged: (value) => _togglePrinterStatus(printer, value),
            ),
            PopupMenuButton(
              itemBuilder: (context) => [
                const PopupMenuItem(
                  value: 'edit',
                  child: ListTile(
                    leading: Icon(Icons.edit),
                    title: Text('Edit'),
                  ),
                ),
                const PopupMenuItem(
                  value: 'test',
                  child: ListTile(
                    leading: Icon(Icons.print),
                    title: Text('Test Print'),
                  ),
                ),
                if (!printer.isDefault)
                  const PopupMenuItem(
                    value: 'default',
                    child: ListTile(
                      leading: Icon(Icons.star),
                      title: Text('Set as Default'),
                    ),
                  ),
                const PopupMenuItem(
                  value: 'delete',
                  child: ListTile(
                    leading: Icon(Icons.delete, color: Colors.red),
                    title: Text('Delete'),
                  ),
                ),
              ],
              onSelected: (value) => _handlePrinterAction(printer, value),
            ),
          ],
        ),
      ),
    );
  }

  void _addPrinter() {
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => AddPrinterScreen(
          onPrinterAdded: _loadPrinters,
        ),
      ),
    );
  }

  void _handlePrinterAction(PrinterConfig printer, String action) {
    switch (action) {
      case 'edit':
        Navigator.push(
          context,
          MaterialPageRoute(
            builder: (context) => AddPrinterScreen(
              printer: printer,
              onPrinterAdded: _loadPrinters,
            ),
          ),
        );
        break;
      case 'test':
        _testPrint(printer);
        break;
      case 'default':
        _setDefaultPrinter(printer);
        break;
      case 'delete':
        _deletePrinter(printer);
        break;
    }
  }

  Future<void> _togglePrinterStatus(PrinterConfig printer, bool isActive) async {
    try {
      // Mock save operation
      await Future.delayed(const Duration(milliseconds: 300));
      await _loadPrinters();

      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(
              isActive ? 'Printer activated' : 'Printer deactivated',
            ),
          ),
        );
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Failed to update printer: $e')),
        );
      }
    }
  }

  Future<void> _testPrint(PrinterConfig printer) async {
    try {
      // Mock test print
      await Future.delayed(const Duration(seconds: 1));
      
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Test print sent successfully'),
            backgroundColor: Colors.green,
          ),
        );
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Test print failed: $e'),
            backgroundColor: Colors.red,
          ),
        );
      }
    }
  }

  Future<void> _setDefaultPrinter(PrinterConfig printer) async {
    try {
      // Mock save operation
      await Future.delayed(const Duration(milliseconds: 300));
      await _loadPrinters();

      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Default printer updated'),
            backgroundColor: Colors.green,
          ),
        );
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Failed to set default printer: $e')),
        );
      }
    }
  }

  Future<void> _deletePrinter(PrinterConfig printer) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Delete Printer'),
        content: Text('Are you sure you want to delete "${printer.name}"?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, false),
            child: const Text('Cancel'),
          ),
          TextButton(
            onPressed: () => Navigator.pop(context, true),
            style: TextButton.styleFrom(foregroundColor: Colors.red),
            child: const Text('Delete'),
          ),
        ],
      ),
    );

    if (confirmed == true) {
      try {
        // Mock delete operation
        setState(() {
          _printers.removeWhere((p) => p.id == printer.id);
        });

        if (mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(
              content: Text('Printer deleted'),
              backgroundColor: Colors.green,
            ),
          );
        }
      } catch (e) {
        if (mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(content: Text('Failed to delete printer: $e')),
          );
        }
      }
    }
  }

  IconData _getPrinterTypeIcon(String type) {
    switch (type) {
      case 'thermal':
        return Icons.receipt;
      case 'laser':
        return Icons.print;
      case 'inkjet':
        return Icons.local_printshop;
      case 'bluetooth':
        return Icons.bluetooth;
      case 'wifi':
        return Icons.wifi;
      case 'usb':
        return Icons.usb;
      default:
        return Icons.print;
    }
  }
}

class AddPrinterScreen extends StatefulWidget {
  final PrinterConfig? printer;
  final VoidCallback onPrinterAdded;

  const AddPrinterScreen({
    super.key,
    this.printer,
    required this.onPrinterAdded,
  });

  @override
  State<AddPrinterScreen> createState() => _AddPrinterScreenState();
}

class _AddPrinterScreenState extends State<AddPrinterScreen> {
  final _formKey = GlobalKey<FormState>();
  final _nameController = TextEditingController();
  final _connectionController = TextEditingController();
  
  String _selectedType = 'thermal';
  int _paperWidth = 80;
  ReceiptFormat _defaultFormat = ReceiptFormat.thermal;
  bool _isDefault = false;
  bool _isActive = true;
  bool _isSaving = false;

  @override
  void initState() {
    super.initState();
    if (widget.printer != null) {
      _populateFields();
    }
  }

  void _populateFields() {
    final printer = widget.printer!;
    _nameController.text = printer.name;
    _connectionController.text = printer.connectionString;
    _selectedType = printer.type;
    _paperWidth = printer.paperWidth;
    _defaultFormat = printer.defaultFormat;
    _isDefault = printer.isDefault;
    _isActive = printer.isActive;
  }

  @override
  void dispose() {
    _nameController.dispose();
    _connectionController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CustomAppBar(
        title: widget.printer == null ? 'Add Printer' : 'Edit Printer',
        actions: [
          TextButton(
            onPressed: _isSaving ? null : _savePrinter,
            child: _isSaving
                ? const SizedBox(
                    width: 20,
                    height: 20,
                    child: CircularProgressIndicator(strokeWidth: 2),
                  )
                : const Text('Save'),
          ),
        ],
      ),
      body: Form(
        key: _formKey,
        child: ListView(
          padding: const EdgeInsets.all(16.0),
          children: [
            // Printer Name
            TextFormField(
              controller: _nameController,
              decoration: const InputDecoration(
                labelText: 'Printer Name',
                hintText: 'e.g., Main Counter Printer',
                prefixIcon: Icon(Icons.label),
              ),
              validator: (value) {
                if (value == null || value.trim().isEmpty) {
                  return 'Please enter a printer name';
                }
                return null;
              },
            ),

            const SizedBox(height: 16),

            // Printer Type
            DropdownButtonFormField<String>(
              value: _selectedType,
              decoration: const InputDecoration(
                labelText: 'Printer Type',
                prefixIcon: Icon(Icons.print),
              ),
              items: const [
                DropdownMenuItem(value: 'thermal', child: Text('THERMAL')),
                DropdownMenuItem(value: 'laser', child: Text('LASER')),
                DropdownMenuItem(value: 'inkjet', child: Text('INKJET')),
                DropdownMenuItem(value: 'bluetooth', child: Text('BLUETOOTH')),
                DropdownMenuItem(value: 'wifi', child: Text('WIFI')),
                DropdownMenuItem(value: 'usb', child: Text('USB')),
              ],
              onChanged: (value) {
                setState(() {
                  _selectedType = value!;
                });
              },
            ),

            const SizedBox(height: 16),

            // Connection String
            TextFormField(
              controller: _connectionController,
              decoration: InputDecoration(
                labelText: 'Connection String',
                hintText: _getConnectionHint(_selectedType),
                prefixIcon: Icon(_getConnectionIcon(_selectedType)),
              ),
              validator: (value) {
                if (value == null || value.trim().isEmpty) {
                  return 'Please enter connection details';
                }
                return null;
              },
            ),

            const SizedBox(height: 16),

            // Paper Width
            DropdownButtonFormField<int>(
              value: _paperWidth,
              decoration: const InputDecoration(
                labelText: 'Paper Width',
                prefixIcon: Icon(Icons.straighten),
              ),
              items: const [
                DropdownMenuItem(value: 58, child: Text('58mm')),
                DropdownMenuItem(value: 80, child: Text('80mm')),
                DropdownMenuItem(value: 210, child: Text('A4 (210mm)')),
              ],
              onChanged: (value) {
                setState(() {
                  _paperWidth = value!;
                });
              },
            ),

            const SizedBox(height: 16),

            // Default Format
            DropdownButtonFormField<ReceiptFormat>(
              value: _defaultFormat,
              decoration: const InputDecoration(
                labelText: 'Default Format',
                prefixIcon: Icon(Icons.format_align_left),
              ),
              items: ReceiptFormat.values.map((format) => DropdownMenuItem(
                value: format,
                child: Text(_getFormatDisplayName(format)),
              )).toList(),
              onChanged: (value) {
                setState(() {
                  _defaultFormat = value!;
                });
              },
            ),

            const SizedBox(height: 24),

            // Settings Switches
            Card(
              child: Padding(
                padding: const EdgeInsets.all(16.0),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      'Printer Settings',
                      style: Theme.of(context).textTheme.titleMedium,
                    ),
                    const SizedBox(height: 16),
                    SwitchListTile(
                      title: const Text('Set as Default Printer'),
                      subtitle: const Text('Use this printer by default'),
                      value: _isDefault,
                      onChanged: (value) {
                        setState(() {
                          _isDefault = value;
                        });
                      },
                    ),
                    SwitchListTile(
                      title: const Text('Active'),
                      subtitle: const Text('Enable this printer for use'),
                      value: _isActive,
                      onChanged: (value) {
                        setState(() {
                          _isActive = value;
                        });
                      },
                    ),
                  ],
                ),
              ),
            ),

            const SizedBox(height: 24),

            // Test Connection Button
            OutlinedButton.icon(
              onPressed: _testConnection,
              icon: const Icon(Icons.wifi_find),
              label: const Text('Test Connection'),
            ),
          ],
        ),
      ),
    );
  }

  Future<void> _savePrinter() async {
    if (!_formKey.currentState!.validate()) {
      return;
    }

    setState(() {
      _isSaving = true;
    });

    try {
      // Mock save printer configuration
      await ReceiptService().getReceiptHistory(); // Mock save operation
      await Future.delayed(const Duration(milliseconds: 300));
      
      widget.onPrinterAdded();
      
      if (mounted) {
        Navigator.pop(context);
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Printer saved successfully'),
            backgroundColor: Colors.green,
          ),
        );
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Failed to save printer: $e'),
            backgroundColor: Colors.red,
          ),
        );
      }
    } finally {
      if (mounted) {
        setState(() {
          _isSaving = false;
        });
      }
    }
  }

  Future<void> _testConnection() async {
    try {
      // Mock connection test
      await Future.delayed(const Duration(seconds: 2));
      
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Connection test successful'),
            backgroundColor: Colors.green,
          ),
        );
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Connection test failed: $e'),
            backgroundColor: Colors.red,
          ),
        );
      }
    }
  }

  String _getConnectionHint(String type) {
    switch (type) {
      case 'bluetooth':
        return 'Bluetooth MAC Address';
      case 'wifi':
        return 'IP Address:Port (e.g., 192.168.1.100:9100)';
      case 'usb':
        return 'USB Device Path';
      default:
        return 'Connection details';
    }
  }

  IconData _getConnectionIcon(String type) {
    switch (type) {
      case 'bluetooth':
        return Icons.bluetooth;
      case 'wifi':
        return Icons.wifi;
      case 'usb':
        return Icons.usb;
      default:
        return Icons.device_hub;
    }
  }

  String _getFormatDisplayName(ReceiptFormat format) {
    switch (format) {
      case ReceiptFormat.thermal:
        return 'Thermal';
      case ReceiptFormat.pos58:
        return 'POS 58mm';
      case ReceiptFormat.pos80:
        return 'POS 80mm';
      case ReceiptFormat.a4:
        return 'A4 PDF';
    }
  }
}

class BusinessSettingsTab extends StatefulWidget {
  const BusinessSettingsTab({super.key});

  @override
  State<BusinessSettingsTab> createState() => _BusinessSettingsTabState();
}

class _BusinessSettingsTabState extends State<BusinessSettingsTab> {
  final _formKey = GlobalKey<FormState>();
  final _businessNameController = TextEditingController(text: 'TOSS POS System');
  final _addressController = TextEditingController(text: '123 Main Street, Accra, Ghana');
  final _phoneController = TextEditingController(text: '+233200123456');
  final _taxNumberController = TextEditingController(text: 'TAX123456789');
  final _footerMessageController = TextEditingController(text: 'Thank you for your business!');
  
  bool _showBarcode = true;
  bool _showLogo = false;

  @override
  void dispose() {
    _businessNameController.dispose();
    _addressController.dispose();
    _phoneController.dispose();
    _taxNumberController.dispose();
    _footerMessageController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Form(
      key: _formKey,
      child: ListView(
        padding: const EdgeInsets.all(16.0),
        children: [
          // Business Information Card
          Card(
            child: Padding(
              padding: const EdgeInsets.all(16.0),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'Business Information',
                    style: Theme.of(context).textTheme.titleMedium,
                  ),
                  const SizedBox(height: 16),
                  TextFormField(
                    controller: _businessNameController,
                    decoration: const InputDecoration(
                      labelText: 'Business Name',
                      prefixIcon: Icon(Icons.business),
                    ),
                    validator: (value) {
                      if (value == null || value.trim().isEmpty) {
                        return 'Please enter business name';
                      }
                      return null;
                    },
                  ),
                  const SizedBox(height: 16),
                  TextFormField(
                    controller: _addressController,
                    decoration: const InputDecoration(
                      labelText: 'Business Address',
                      prefixIcon: Icon(Icons.location_on),
                    ),
                    maxLines: 2,
                  ),
                  const SizedBox(height: 16),
                  TextFormField(
                    controller: _phoneController,
                    decoration: const InputDecoration(
                      labelText: 'Phone Number',
                      prefixIcon: Icon(Icons.phone),
                    ),
                    keyboardType: TextInputType.phone,
                  ),
                  const SizedBox(height: 16),
                  TextFormField(
                    controller: _taxNumberController,
                    decoration: const InputDecoration(
                      labelText: 'Tax Number / VAT ID',
                      prefixIcon: Icon(Icons.receipt_long),
                    ),
                  ),
                ],
              ),
            ),
          ),

          const SizedBox(height: 16),

          // Receipt Customization Card
          Card(
            child: Padding(
              padding: const EdgeInsets.all(16.0),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'Receipt Customization',
                    style: Theme.of(context).textTheme.titleMedium,
                  ),
                  const SizedBox(height: 16),
                  TextFormField(
                    controller: _footerMessageController,
                    decoration: const InputDecoration(
                      labelText: 'Footer Message',
                      prefixIcon: Icon(Icons.message),
                      hintText: 'Thank you message or promotional text',
                    ),
                    maxLines: 2,
                  ),
                  const SizedBox(height: 16),
                  SwitchListTile(
                    title: const Text('Show Barcode'),
                    subtitle: const Text('Display receipt number as barcode'),
                    value: _showBarcode,
                    onChanged: (value) {
                      setState(() {
                        _showBarcode = value;
                      });
                    },
                  ),
                  SwitchListTile(
                    title: const Text('Show Business Logo'),
                    subtitle: const Text('Include logo on receipts'),
                    value: _showLogo,
                    onChanged: (value) {
                      setState(() {
                        _showLogo = value;
                      });
                    },
                  ),
                  if (_showLogo) ...[
                    const SizedBox(height: 8),
                    OutlinedButton.icon(
                      onPressed: _selectLogo,
                      icon: const Icon(Icons.image),
                      label: const Text('Select Logo'),
                    ),
                  ],
                ],
              ),
            ),
          ),

          const SizedBox(height: 24),

          // Save Button
          ElevatedButton(
            onPressed: _saveSettings,
            child: const Text('Save Settings'),
          ),
        ],
      ),
    );
  }

  Future<void> _selectLogo() async {
    // Mock logo selection
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(content: Text('Logo selection functionality coming soon')),
    );
  }

  Future<void> _saveSettings() async {
    if (!_formKey.currentState!.validate()) {
      return;
    }

    try {
      // Mock save operation
      await Future.delayed(const Duration(seconds: 1));
      
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Settings saved successfully'),
            backgroundColor: Colors.green,
          ),
        );
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Failed to save settings: $e'),
            backgroundColor: Colors.red,
          ),
        );
      }
    }
  }
}

class TemplateSettingsTab extends StatelessWidget {
  const TemplateSettingsTab({super.key});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(16.0),
      child: Column(
        children: [
          const Card(
            child: Padding(
              padding: EdgeInsets.all(16.0),
              child: Column(
                children: [
                  Icon(Icons.construction, size: 64, color: Colors.grey),
                  SizedBox(height: 16),
                  Text(
                    'Template Customization',
                    style: TextStyle(
                      fontSize: 18,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  SizedBox(height: 8),
                  Text(
                    'Advanced receipt template customization features are coming soon.',
                    textAlign: TextAlign.center,
                    style: TextStyle(color: Colors.grey),
                  ),
                ],
              ),
            ),
          ),
          const SizedBox(height: 16),
          Card(
            child: Column(
              children: [
                ListTile(
                  leading: const Icon(Icons.receipt),
                  title: const Text('Default Template'),
                  subtitle: const Text('Standard receipt layout'),
                  trailing: const Icon(Icons.arrow_forward_ios),
                  onTap: () {
                    // Navigate to template editor
                  },
                ),
                ListTile(
                  leading: const Icon(Icons.receipt_long),
                  title: const Text('Detailed Template'),
                  subtitle: const Text('Receipt with detailed item information'),
                  trailing: const Icon(Icons.arrow_forward_ios),
                  onTap: () {
                    // Navigate to template editor
                  },
                ),
                ListTile(
                  leading: const Icon(Icons.format_quote),
                  title: const Text('Quote Template'),
                  subtitle: const Text('Template for price quotes'),
                  trailing: const Icon(Icons.arrow_forward_ios),
                  onTap: () {
                    // Navigate to template editor
                  },
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
