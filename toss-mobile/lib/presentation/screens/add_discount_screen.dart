import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../../domain/entities/discount_entity.dart' as domain;
import '../../domain/entities/product_entity.dart';
import '../../domain/entities/customer_entity.dart';
import '../../data/services/discount_service.dart';
import '../../data/services/product_service.dart';
import '../../data/services/customer_service.dart';
import '../widgets/common/custom_app_bar.dart';
import '../widgets/common/loading_widget.dart';

class AddDiscountScreen extends StatefulWidget {
  final domain.DiscountEntity? discount;

  const AddDiscountScreen({super.key, this.discount});

  @override
  State<AddDiscountScreen> createState() => _AddDiscountScreenState();
}

class _AddDiscountScreenState extends State<AddDiscountScreen>
    with TickerProviderStateMixin {
  late TabController _tabController;
  final _formKey = GlobalKey<FormState>();
  
  final DiscountService _discountService = DiscountService();
  final ProductService _productService = ProductService();
  final CustomerService _customerService = CustomerService();

  // Form controllers
  final _nameController = TextEditingController();
  final _descriptionController = TextEditingController();
  final _valueController = TextEditingController();
  final _minimumAmountController = TextEditingController();
  final _maxDiscountAmountController = TextEditingController();
  final _usageLimitController = TextEditingController();
  final _usageLimitPerCustomerController = TextEditingController();
  final _couponCodeController = TextEditingController();

  // Form values
  domain.DiscountType _selectedType = domain.DiscountType.percentage;
  domain.DiscountScope _selectedScope = domain.DiscountScope.cart;
  domain.DiscountApplication _selectedApplication = domain.DiscountApplication.automatic;
  DateTime? _startDate;
  DateTime? _endDate;
  TimeOfDay? _startTime;
  TimeOfDay? _endTime;
  bool _isActive = true;
  bool _isStackable = false;
  int _priority = 1;

  // BOGO Configuration
  final _buyQuantityController = TextEditingController();
  final _getQuantityController = TextEditingController();
  final _getDiscountPercentageController = TextEditingController();

  // Time-based Configuration
  List<int> _selectedDaysOfWeek = [];
  bool _isSeasonalDiscount = false;
  DateTime? _seasonStart;
  DateTime? _seasonEnd;

  // Location Configuration
  List<String> _applicableLocations = [];
  final List<String> _availableLocations = ['Main Store', 'Branch 1', 'Branch 2', 'Online'];

  // Target Configuration
  List<String> _targetProductIds = [];
  List<String> _targetCategoryIds = [];
  List<String> _targetCustomerIds = [];
  List<ProductEntity> _products = [];
  List<CustomerEntity> _customers = [];

  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 4, vsync: this);
    _loadData();
    _initializeFromDiscount();
  }

  @override
  void dispose() {
    _tabController.dispose();
    _nameController.dispose();
    _descriptionController.dispose();
    _valueController.dispose();
    _minimumAmountController.dispose();
    _maxDiscountAmountController.dispose();
    _usageLimitController.dispose();
    _usageLimitPerCustomerController.dispose();
    _couponCodeController.dispose();
    _buyQuantityController.dispose();
    _getQuantityController.dispose();
    _getDiscountPercentageController.dispose();
    super.dispose();
  }

  void _initializeFromDiscount() {
    if (widget.discount != null) {
      final discount = widget.discount!;
      _nameController.text = discount.name;
      _descriptionController.text = discount.description;
      _valueController.text = discount.value.toString();
  _minimumAmountController.text = discount.minimumPurchase?.toString() ?? '';
  _maxDiscountAmountController.text = discount.maximumDiscount?.toString() ?? '';
  _usageLimitController.text = discount.totalMaxUses?.toString() ?? '';
  _usageLimitPerCustomerController.text = discount.maxUsesPerCustomer?.toString() ?? '';
      _couponCodeController.text = discount.couponCode ?? '';
      
      _selectedType = discount.type;
      _selectedScope = discount.scope;
      _selectedApplication = discount.application;
      _startDate = discount.startDate;
      _endDate = discount.endDate;
      _isActive = discount.isActive;
      _isStackable = discount.isStackable;
      _priority = discount.priority;
      
      // Initialize BOGO configuration
      if (discount.bogoConfig != null) {
        _buyQuantityController.text = discount.bogoConfig!.buyQuantity.toString();
        _getQuantityController.text = discount.bogoConfig!.getQuantity.toString();
        _getDiscountPercentageController.text = discount.bogoConfig!.getDiscountPercent.toString();
      }
      
      // Initialize time-based configuration
      if (discount.timeConfig != null) {
        _selectedDaysOfWeek = discount.timeConfig!.daysOfWeek;
        _startTime = discount.timeConfig!.startTime != null
            ? TimeOfDay(hour: discount.timeConfig!.startTime!.hour, minute: discount.timeConfig!.startTime!.minute)
            : null;
        _endTime = discount.timeConfig!.endTime != null
            ? TimeOfDay(hour: discount.timeConfig!.endTime!.hour, minute: discount.timeConfig!.endTime!.minute)
            : null;
        _isSeasonalDiscount = discount.timeConfig!.seasonalPeriod != null;
        _seasonStart = null;
        _seasonEnd = null;
      }
      
      // Initialize location configuration
      if (discount.locationConfig != null) {
        _applicableLocations = List.from(discount.locationConfig!.applicableLocations);
      }
      
      // Initialize target configuration
  _targetProductIds = List.from(discount.applicableItems);
  _targetCategoryIds = List.from(discount.applicableCategories);
  _targetCustomerIds = List.from(discount.applicableCustomers);
    }
  }

  Future<void> _loadData() async {
    setState(() {
      _isLoading = true;
    });

    try {
      final products = await _productService.getAllProducts();
      final customers = await _customerService.getAllCustomers();
      
      setState(() {
        _products = products;
        _customers = customers;
        _isLoading = false;
      });
    } catch (e) {
      setState(() {
        _isLoading = false;
      });
      
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Failed to load data: $e')),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CustomAppBar(
        title: widget.discount == null ? 'Add Discount' : 'Edit Discount',
        actions: [
          IconButton(
            icon: const Icon(Icons.save),
            onPressed: _saveDiscount,
          ),
        ],
        bottom: TabBar(
          controller: _tabController,
          isScrollable: true,
          tabs: const [
            Tab(text: 'Basic Info'),
            Tab(text: 'Conditions'),
            Tab(text: 'Restrictions'),
            Tab(text: 'Advanced'),
          ],
        ),
      ),
      body: _isLoading
          ? const LoadingWidget()
          : Form(
              key: _formKey,
              child: TabBarView(
                controller: _tabController,
                children: [
                  _buildBasicInfoTab(),
                  _buildConditionsTab(),
                  _buildRestrictionsTab(),
                  _buildAdvancedTab(),
                ],
              ),
            ),
      bottomNavigationBar: Container(
        padding: const EdgeInsets.all(16),
        child: Row(
          children: [
            Expanded(
              child: ElevatedButton(
                onPressed: () => Navigator.pop(context),
                style: ElevatedButton.styleFrom(
                  backgroundColor: Colors.grey[300],
                  foregroundColor: Colors.black,
                ),
                child: const Text('Cancel'),
              ),
            ),
            const SizedBox(width: 16),
            Expanded(
              child: ElevatedButton(
                onPressed: _saveDiscount,
                child: Text(widget.discount == null ? 'Create' : 'Update'),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildBasicInfoTab() {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          TextFormField(
            controller: _nameController,
            decoration: const InputDecoration(
              labelText: 'Discount Name *',
              hintText: 'e.g., Summer Sale 2024',
            ),
            validator: (value) {
              if (value == null || value.isEmpty) {
                return 'Please enter a discount name';
              }
              return null;
            },
          ),
          const SizedBox(height: 16),
          
          TextFormField(
            controller: _descriptionController,
            decoration: const InputDecoration(
              labelText: 'Description *',
              hintText: 'Brief description of the discount',
            ),
            maxLines: 3,
            validator: (value) {
              if (value == null || value.isEmpty) {
                return 'Please enter a description';
              }
              return null;
            },
          ),
          const SizedBox(height: 16),
          
          DropdownButtonFormField<domain.DiscountType>(
            value: _selectedType,
            decoration: const InputDecoration(
              labelText: 'Discount Type *',
            ),
            items: domain.DiscountType.values.map((type) => DropdownMenuItem(
              value: type,
              child: Text(_getDiscountTypeDisplayName(type)),
            )).toList(),
            onChanged: (value) {
              setState(() {
                _selectedType = value!;
              });
            },
          ),
          const SizedBox(height: 16),
          
          if (_selectedType == domain.DiscountType.percentage ||
              _selectedType == domain.DiscountType.fixedAmount) ...[
            TextFormField(
              controller: _valueController,
              decoration: InputDecoration(
                labelText: _selectedType == domain.DiscountType.percentage 
                    ? 'Percentage Value *' 
                    : 'Fixed Amount (GHS) *',
                hintText: _selectedType == domain.DiscountType.percentage 
                    ? 'e.g., 10 (for 10%)' 
                    : 'e.g., 50.00',
                suffixText: _selectedType == domain.DiscountType.percentage ? '%' : 'GHS',
              ),
              keyboardType: TextInputType.number,
              inputFormatters: [
                FilteringTextInputFormatter.allow(RegExp(r'^\d+\.?\d{0,2}')),
              ],
              validator: (value) {
                if (value == null || value.isEmpty) {
                  return 'Please enter a value';
                }
                final numValue = double.tryParse(value);
                if (numValue == null || numValue <= 0) {
                  return 'Please enter a valid positive number';
                }
                if (_selectedType == domain.DiscountType.percentage && numValue > 100) {
                  return 'Percentage cannot be more than 100%';
                }
                return null;
              },
            ),
            const SizedBox(height: 16),
          ],
          
          if (_selectedType == domain.DiscountType.bogo ||
              _selectedType == domain.DiscountType.buyXGetY) ...[
            _buildBogoConfiguration(),
            const SizedBox(height: 16),
          ],
          
          DropdownButtonFormField<domain.DiscountScope>(
            value: _selectedScope,
            decoration: const InputDecoration(
              labelText: 'Applies To *',
            ),
            items: domain.DiscountScope.values.map((scope) => DropdownMenuItem(
              value: scope,
              child: Text(_getDiscountScopeDisplayName(scope)),
            )).toList(),
            onChanged: (value) {
              setState(() {
                _selectedScope = value!;
              });
            },
          ),
          const SizedBox(height: 16),
          
          DropdownButtonFormField<domain.DiscountApplication>(
            value: _selectedApplication,
            decoration: const InputDecoration(
              labelText: 'Application Type *',
            ),
            items: domain.DiscountApplication.values.map((app) => DropdownMenuItem(
              value: app,
              child: Text(_getDiscountApplicationDisplayName(app)),
            )).toList(),
            onChanged: (value) {
              setState(() {
                _selectedApplication = value!;
              });
            },
          ),
          const SizedBox(height: 16),
          
          if (_selectedApplication == domain.DiscountApplication.couponCode) ...[
            TextFormField(
              controller: _couponCodeController,
              decoration: const InputDecoration(
                labelText: 'Coupon Code *',
                hintText: 'e.g., SUMMER2024',
              ),
              textCapitalization: TextCapitalization.characters,
              validator: (value) {
                if (_selectedApplication == domain.DiscountApplication.couponCode &&
                    (value == null || value.isEmpty)) {
                  return 'Please enter a coupon code';
                }
                return null;
              },
            ),
            const SizedBox(height: 16),
          ],
          
          Row(
            children: [
              Checkbox(
                value: _isActive,
                onChanged: (value) {
                  setState(() {
                    _isActive = value!;
                  });
                },
              ),
              const Text('Active'),
              const SizedBox(width: 32),
              Checkbox(
                value: _isStackable,
                onChanged: (value) {
                  setState(() {
                    _isStackable = value!;
                  });
                },
              ),
              const Text('Stackable'),
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildBogoConfiguration() {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        border: Border.all(color: Colors.grey[300]!),
        borderRadius: BorderRadius.circular(8),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'BOGO Configuration',
            style: TextStyle(fontWeight: FontWeight.bold),
          ),
          const SizedBox(height: 16),
          Row(
            children: [
              Expanded(
                child: TextFormField(
                  controller: _buyQuantityController,
                  decoration: const InputDecoration(
                    labelText: 'Buy Quantity *',
                    hintText: '1',
                  ),
                  keyboardType: TextInputType.number,
                  inputFormatters: [FilteringTextInputFormatter.digitsOnly],
                  validator: (value) {
                    if ((_selectedType == domain.DiscountType.bogo ||
                         _selectedType == domain.DiscountType.buyXGetY) &&
                        (value == null || value.isEmpty)) {
                      return 'Required';
                    }
                    return null;
                  },
                ),
              ),
              const SizedBox(width: 16),
              Expanded(
                child: TextFormField(
                  controller: _getQuantityController,
                  decoration: const InputDecoration(
                    labelText: 'Get Quantity *',
                    hintText: '1',
                  ),
                  keyboardType: TextInputType.number,
                  inputFormatters: [FilteringTextInputFormatter.digitsOnly],
                  validator: (value) {
                    if ((_selectedType == domain.DiscountType.bogo ||
                         _selectedType == domain.DiscountType.buyXGetY) &&
                        (value == null || value.isEmpty)) {
                      return 'Required';
                    }
                    return null;
                  },
                ),
              ),
            ],
          ),
          const SizedBox(height: 16),
          TextFormField(
            controller: _getDiscountPercentageController,
            decoration: const InputDecoration(
              labelText: 'Get Discount % *',
              hintText: '100 (for free), 50 (for 50% off)',
              suffixText: '%',
            ),
            keyboardType: TextInputType.number,
            inputFormatters: [
              FilteringTextInputFormatter.allow(RegExp(r'^\d+\.?\d{0,2}')),
            ],
            validator: (value) {
              if ((_selectedType == domain.DiscountType.bogo ||
                   _selectedType == domain.DiscountType.buyXGetY) &&
                  (value == null || value.isEmpty)) {
                return 'Please enter discount percentage';
              }
              final numValue = double.tryParse(value ?? '');
              if (numValue != null && (numValue < 0 || numValue > 100)) {
                return 'Percentage must be between 0 and 100';
              }
              return null;
            },
          ),
        ],
      ),
    );
  }

  Widget _buildConditionsTab() {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Date Range',
            style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
          ),
          const SizedBox(height: 16),
          
          Row(
            children: [
              Expanded(
                child: InkWell(
                  onTap: () => _selectDate(context, true),
                  child: InputDecorator(
                    decoration: const InputDecoration(
                      labelText: 'Start Date',
                      suffixIcon: Icon(Icons.calendar_today),
                    ),
                    child: Text(
                      _startDate?.toString().substring(0, 10) ?? 'Select date',
                      style: TextStyle(
                        color: _startDate == null ? Colors.grey : null,
                      ),
                    ),
                  ),
                ),
              ),
              const SizedBox(width: 16),
              Expanded(
                child: InkWell(
                  onTap: () => _selectDate(context, false),
                  child: InputDecorator(
                    decoration: const InputDecoration(
                      labelText: 'End Date',
                      suffixIcon: Icon(Icons.calendar_today),
                    ),
                    child: Text(
                      _endDate?.toString().substring(0, 10) ?? 'Select date',
                      style: TextStyle(
                        color: _endDate == null ? Colors.grey : null,
                      ),
                    ),
                  ),
                ),
              ),
            ],
          ),
          const SizedBox(height: 24),
          
          const Text(
            'Time Restrictions',
            style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
          ),
          const SizedBox(height: 16),
          
          Row(
            children: [
              Expanded(
                child: InkWell(
                  onTap: () => _selectTime(context, true),
                  child: InputDecorator(
                    decoration: const InputDecoration(
                      labelText: 'Start Time',
                      suffixIcon: Icon(Icons.access_time),
                    ),
                    child: Text(
                      _startTime?.format(context) ?? 'Any time',
                      style: TextStyle(
                        color: _startTime == null ? Colors.grey : null,
                      ),
                    ),
                  ),
                ),
              ),
              const SizedBox(width: 16),
              Expanded(
                child: InkWell(
                  onTap: () => _selectTime(context, false),
                  child: InputDecorator(
                    decoration: const InputDecoration(
                      labelText: 'End Time',
                      suffixIcon: Icon(Icons.access_time),
                    ),
                    child: Text(
                      _endTime?.format(context) ?? 'Any time',
                      style: TextStyle(
                        color: _endTime == null ? Colors.grey : null,
                      ),
                    ),
                  ),
                ),
              ),
            ],
          ),
          const SizedBox(height: 16),
          
          const Text('Days of Week'),
          const SizedBox(height: 8),
          Wrap(
            spacing: 8,
            children: List.generate(7, (index) {
              final dayNames = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];
              final isSelected = _selectedDaysOfWeek.contains(index + 1);
              
              return FilterChip(
                label: Text(dayNames[index]),
                selected: isSelected,
                onSelected: (selected) {
                  setState(() {
                    if (selected) {
                      _selectedDaysOfWeek.add(index + 1);
                    } else {
                      _selectedDaysOfWeek.remove(index + 1);
                    }
                  });
                },
              );
            }),
          ),
          const SizedBox(height: 24),
          
          CheckboxListTile(
            title: const Text('Seasonal Discount'),
            value: _isSeasonalDiscount,
            onChanged: (value) {
              setState(() {
                _isSeasonalDiscount = value!;
              });
            },
          ),
          
          if (_isSeasonalDiscount) ...[
            const SizedBox(height: 16),
            Row(
              children: [
                Expanded(
                  child: InkWell(
                    onTap: () => _selectSeasonDate(context, true),
                    child: InputDecorator(
                      decoration: const InputDecoration(
                        labelText: 'Season Start',
                        suffixIcon: Icon(Icons.calendar_today),
                      ),
                      child: Text(
                        _seasonStart?.toString().substring(0, 10) ?? 'Select date',
                        style: TextStyle(
                          color: _seasonStart == null ? Colors.grey : null,
                        ),
                      ),
                    ),
                  ),
                ),
                const SizedBox(width: 16),
                Expanded(
                  child: InkWell(
                    onTap: () => _selectSeasonDate(context, false),
                    child: InputDecorator(
                      decoration: const InputDecoration(
                        labelText: 'Season End',
                        suffixIcon: Icon(Icons.calendar_today),
                      ),
                      child: Text(
                        _seasonEnd?.toString().substring(0, 10) ?? 'Select date',
                        style: TextStyle(
                          color: _seasonEnd == null ? Colors.grey : null,
                        ),
                      ),
                    ),
                  ),
                ),
              ],
            ),
          ],
          const SizedBox(height: 24),
          
          const Text(
            'Usage Limits',
            style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
          ),
          const SizedBox(height: 16),
          
          TextFormField(
            controller: _usageLimitController,
            decoration: const InputDecoration(
              labelText: 'Total Usage Limit',
              hintText: 'Leave empty for unlimited',
            ),
            keyboardType: TextInputType.number,
            inputFormatters: [FilteringTextInputFormatter.digitsOnly],
          ),
          const SizedBox(height: 16),
          
          TextFormField(
            controller: _usageLimitPerCustomerController,
            decoration: const InputDecoration(
              labelText: 'Usage Limit Per Customer',
              hintText: 'Leave empty for unlimited',
            ),
            keyboardType: TextInputType.number,
            inputFormatters: [FilteringTextInputFormatter.digitsOnly],
          ),
        ],
      ),
    );
  }

  Widget _buildRestrictionsTab() {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Amount Restrictions',
            style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
          ),
          const SizedBox(height: 16),
          
          TextFormField(
            controller: _minimumAmountController,
            decoration: const InputDecoration(
              labelText: 'Minimum Cart Amount (GHS)',
              hintText: 'e.g., 100.00',
            ),
            keyboardType: TextInputType.number,
            inputFormatters: [
              FilteringTextInputFormatter.allow(RegExp(r'^\d+\.?\d{0,2}')),
            ],
          ),
          const SizedBox(height: 16),
          
          TextFormField(
            controller: _maxDiscountAmountController,
            decoration: const InputDecoration(
              labelText: 'Maximum Discount Amount (GHS)',
              hintText: 'e.g., 50.00',
            ),
            keyboardType: TextInputType.number,
            inputFormatters: [
              FilteringTextInputFormatter.allow(RegExp(r'^\d+\.?\d{0,2}')),
            ],
          ),
          const SizedBox(height: 24),
          
          const Text(
            'Location Restrictions',
            style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
          ),
          const SizedBox(height: 16),
          
          ...(_availableLocations.map((location) => CheckboxListTile(
            title: Text(location),
            value: _applicableLocations.contains(location),
            onChanged: (value) {
              setState(() {
                if (value!) {
                  _applicableLocations.add(location);
                } else {
                  _applicableLocations.remove(location);
                }
              });
            },
          ))),
          
          const SizedBox(height: 24),
          
      if (_selectedScope == domain.DiscountScope.item ||
        _selectedScope == domain.DiscountScope.category ||
        _selectedScope == domain.DiscountScope.brand) ...[
            const Text(
              'Product Restrictions',
              style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 16),
            
            if (_selectedScope == domain.DiscountScope.item) ...[
              ElevatedButton(
                onPressed: _selectProducts,
                child: Text('Select Products (${_targetProductIds.length})'),
              ),
              const SizedBox(height: 16),
              
              if (_targetProductIds.isNotEmpty) ...[
                Container(
                  height: 200,
                  decoration: BoxDecoration(
                    border: Border.all(color: Colors.grey[300]!),
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: ListView.builder(
                    itemCount: _targetProductIds.length,
                    itemBuilder: (context, index) {
                      final productId = _targetProductIds[index];
                      final product = _products.firstWhere(
                        (p) => (p.id ?? '').toString() == productId,
                        orElse: () => ProductEntity(
                          id: int.tryParse(productId),
                          createdById: 'system',
                          name: 'Unknown Product',
                          barcode: '',
                          imageUrl: '',
                          stock: 0,
                          price: 0,
                          costPrice: 0,
                          unit: '',
                          isActive: true,
                          createdAt: DateTime.now().toIso8601String(),
                        ),
                      );
                      
                      return ListTile(
                        title: Text(product.name),
                        subtitle: Text('GHS ${(product.price / 100).toStringAsFixed(2)}'),
                        trailing: IconButton(
                          icon: const Icon(Icons.remove_circle, color: Colors.red),
                          onPressed: () {
                            setState(() {
                              _targetProductIds.removeAt(index);
                            });
                          },
                        ),
                      );
                    },
                  ),
                ),
              ],
            ],
          ],
          
          const SizedBox(height: 24),
          
          if (_selectedType == domain.DiscountType.customerSpecific) ...[
            const Text(
              'Customer Restrictions',
              style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 16),
            
            ElevatedButton(
              onPressed: _selectCustomers,
              child: Text('Select Customers (${_targetCustomerIds.length})'),
            ),
            const SizedBox(height: 16),
            
            if (_targetCustomerIds.isNotEmpty) ...[
              Container(
                height: 200,
                decoration: BoxDecoration(
                  border: Border.all(color: Colors.grey[300]!),
                  borderRadius: BorderRadius.circular(8),
                ),
                child: ListView.builder(
                  itemCount: _targetCustomerIds.length,
                  itemBuilder: (context, index) {
                    final customerId = _targetCustomerIds[index];
                    final customer = _customers.firstWhere(
                      (c) => c.id == customerId,
                      orElse: () => const CustomerEntity(
                        id: 'unknown',
                        name: 'Unknown Customer',
                        phone: '',
                        address: '',
                        city: '',
                        loyaltyPoints: 0,
                        totalSpent: 0.0,
                        isActive: true,
                      ),
                    );
                    
                    return ListTile(
                      title: Text(customer.name ?? 'Unknown Customer'),
                      subtitle: Text(customer.phone ?? ''),
                      trailing: IconButton(
                        icon: const Icon(Icons.remove_circle, color: Colors.red),
                        onPressed: () {
                          setState(() {
                            _targetCustomerIds.removeAt(index);
                          });
                        },
                      ),
                    );
                  },
                ),
              ),
            ],
          ],
        ],
      ),
    );
  }

  Widget _buildAdvancedTab() {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Priority and Stacking',
            style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
          ),
          const SizedBox(height: 16),
          
          TextFormField(
            initialValue: _priority.toString(),
            decoration: const InputDecoration(
              labelText: 'Priority (1-10)',
              hintText: '1 = Highest priority',
            ),
            keyboardType: TextInputType.number,
            inputFormatters: [FilteringTextInputFormatter.digitsOnly],
            onChanged: (value) {
              final intValue = int.tryParse(value);
              if (intValue != null && intValue >= 1 && intValue <= 10) {
                _priority = intValue;
              }
            },
          ),
          const SizedBox(height: 16),
          
          SwitchListTile(
            title: const Text('Allow Stacking'),
            subtitle: const Text('Can be combined with other discounts'),
            value: _isStackable,
            onChanged: (value) {
              setState(() {
                _isStackable = value;
              });
            },
          ),
          const SizedBox(height: 24),
          
          const Text(
            'Advanced Settings',
            style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
          ),
          const SizedBox(height: 16),
          
          // Additional advanced settings can be added here
          Container(
            padding: const EdgeInsets.all(16),
            decoration: BoxDecoration(
              color: Colors.grey[100],
              borderRadius: BorderRadius.circular(8),
            ),
            child: const Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'Notes:',
                  style: TextStyle(fontWeight: FontWeight.bold),
                ),
                SizedBox(height: 8),
                Text('• Higher priority discounts are applied first'),
                Text('• Stackable discounts can be combined'),
                Text('• Time restrictions apply to discount availability'),
                Text('• Location restrictions limit where discount can be used'),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Future<void> _selectDate(BuildContext context, bool isStartDate) async {
    final date = await showDatePicker(
      context: context,
      initialDate: DateTime.now(),
      firstDate: DateTime.now().subtract(const Duration(days: 365)),
      lastDate: DateTime.now().add(const Duration(days: 365 * 5)),
    );
    
    if (date != null) {
      setState(() {
        if (isStartDate) {
          _startDate = date;
        } else {
          _endDate = date;
        }
      });
    }
  }

  Future<void> _selectTime(BuildContext context, bool isStartTime) async {
    final time = await showTimePicker(
      context: context,
      initialTime: TimeOfDay.now(),
    );
    
    if (time != null) {
      setState(() {
        if (isStartTime) {
          _startTime = time;
        } else {
          _endTime = time;
        }
      });
    }
  }

  Future<void> _selectSeasonDate(BuildContext context, bool isStart) async {
    final date = await showDatePicker(
      context: context,
      initialDate: DateTime.now(),
      firstDate: DateTime.now(),
      lastDate: DateTime.now().add(const Duration(days: 365)),
    );
    
    if (date != null) {
      setState(() {
        if (isStart) {
          _seasonStart = date;
        } else {
          _seasonEnd = date;
        }
      });
    }
  }

  void _selectProducts() {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Select Products'),
        content: SizedBox(
          width: double.maxFinite,
          height: 400,
          child: ListView.builder(
            itemCount: _products.length,
            itemBuilder: (context, index) {
              final product = _products[index];
              final isSelected = _targetProductIds.contains((product.id ?? '').toString());
              
              return CheckboxListTile(
                title: Text(product.name),
                subtitle: Text('GHS ${(product.price / 100).toStringAsFixed(2)}'),
                value: isSelected,
                onChanged: (value) {
                  setState(() {
                    if (value!) {
                      _targetProductIds.add((product.id ?? '').toString());
                    } else {
                      _targetProductIds.remove((product.id ?? '').toString());
                    }
                  });
                  Navigator.pop(context);
                  _selectProducts(); // Reopen to show updated selection
                },
              );
            },
          ),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Done'),
          ),
        ],
      ),
    );
  }

  void _selectCustomers() {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Select Customers'),
        content: SizedBox(
          width: double.maxFinite,
          height: 400,
          child: ListView.builder(
            itemCount: _customers.length,
            itemBuilder: (context, index) {
              final customer = _customers[index];
              final isSelected = _targetCustomerIds.contains(customer.id);
              
              return CheckboxListTile(
                title: Text(customer.name ?? 'Unknown Customer'),
                subtitle: Text(customer.phone ?? ''),
                value: isSelected,
                onChanged: (value) {
                  setState(() {
                    if (value!) {
                      _targetCustomerIds.add(customer.id);
                    } else {
                      _targetCustomerIds.remove(customer.id);
                    }
                  });
                  Navigator.pop(context);
                  _selectCustomers(); // Reopen to show updated selection
                },
              );
            },
          ),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Done'),
          ),
        ],
      ),
    );
  }

  Future<void> _saveDiscount() async {
    if (!_formKey.currentState!.validate()) {
      return;
    }

    setState(() {
      _isLoading = true;
    });

    try {
      // Create BOGO configuration if needed
      domain.BogoConfig? bogoConfig;
      if (_selectedType == domain.DiscountType.bogo || _selectedType == domain.DiscountType.buyXGetY) {
        bogoConfig = domain.BogoConfig(
          type: _selectedType == domain.DiscountType.bogo
              ? domain.BogoType.buyOneGetOne
              : domain.BogoType.buyXGetYPercent,
          buyQuantity: int.parse(_buyQuantityController.text),
          getQuantity: int.parse(_getQuantityController.text),
          getDiscountPercent: double.parse(_getDiscountPercentageController.text),
        );
      }

      // Create time-based configuration if needed
      domain.TimeBasedConfig? timeBasedConfig;
      if (_selectedDaysOfWeek.isNotEmpty || _startTime != null || _endTime != null || _isSeasonalDiscount) {
        timeBasedConfig = domain.TimeBasedConfig(
          daysOfWeek: _selectedDaysOfWeek,
          startTime: _startTime != null ? domain.TimeOfDay(hour: _startTime!.hour, minute: _startTime!.minute) : null,
          endTime: _endTime != null ? domain.TimeOfDay(hour: _endTime!.hour, minute: _endTime!.minute) : null,
          excludedDates: const [],
          isHappyHour: false,
          seasonalPeriod: _isSeasonalDiscount ? 'seasonal' : null,
        );
      }

      // Create location configuration if needed
      domain.LocationConfig? locationConfig;
      if (_applicableLocations.isNotEmpty) {
        locationConfig = domain.LocationConfig(
          applicableLocations: _applicableLocations,
        );
      }

      final discount = domain.DiscountEntity(
        id: widget.discount?.id ?? DateTime.now().millisecondsSinceEpoch.toString(),
        name: _nameController.text.trim(),
        description: _descriptionController.text.trim(),
        type: _selectedType,
        scope: _selectedScope,
        application: _selectedApplication,
        value: double.parse(_valueController.text.isNotEmpty ? _valueController.text : '0'),
        minimumPurchase: _minimumAmountController.text.isNotEmpty 
            ? double.parse(_minimumAmountController.text) 
            : null,
        maximumDiscount: _maxDiscountAmountController.text.isNotEmpty 
            ? double.parse(_maxDiscountAmountController.text) 
            : null,
        startDate: _startDate ?? DateTime.now(),
        endDate: _endDate ?? DateTime.now().add(const Duration(days: 30)),
        isActive: _isActive,
        isStackable: _isStackable,
        priority: _priority,
        totalMaxUses: _usageLimitController.text.isNotEmpty 
            ? int.parse(_usageLimitController.text) 
            : null,
        maxUsesPerCustomer: _usageLimitPerCustomerController.text.isNotEmpty 
            ? int.parse(_usageLimitPerCustomerController.text) 
            : null,
        currentUses: widget.discount?.currentUses ?? 0,
        couponCode: _couponCodeController.text.isNotEmpty 
            ? _couponCodeController.text.trim() 
            : null,
        applicableItems: _targetProductIds,
        applicableCategories: _targetCategoryIds,
        applicableCustomers: _targetCustomerIds,
        bogoConfig: bogoConfig,
        timeConfig: timeBasedConfig,
        locationConfig: locationConfig,
        createdAt: widget.discount?.createdAt ?? DateTime.now(),
        updatedAt: DateTime.now(),
        createdBy: widget.discount?.createdBy ?? 'system',
      );

      if (widget.discount == null) {
        await _discountService.createDiscount(discount);
      } else {
        await _discountService.updateDiscount(discount);
      }

      if (mounted) {
        Navigator.pop(context);
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(
              widget.discount == null 
                  ? 'Discount created successfully' 
                  : 'Discount updated successfully',
            ),
            backgroundColor: Colors.green,
          ),
        );
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Failed to save discount: $e'),
            backgroundColor: Colors.red,
          ),
        );
      }
    } finally {
      setState(() {
        _isLoading = false;
      });
    }
  }

  // Helper methods for display names
  String _getDiscountTypeDisplayName(domain.DiscountType type) {
    switch (type) {
      case domain.DiscountType.percentage:
        return 'Percentage Discount';
      case domain.DiscountType.fixedAmount:
        return 'Fixed Amount Discount';
      case domain.DiscountType.bogo:
        return 'Buy One Get One (BOGO)';
      case domain.DiscountType.buyXGetY:
        return 'Buy X Get Y';
      case domain.DiscountType.freeShipping:
        return 'Free Shipping';
      case domain.DiscountType.customerSpecific:
        return 'Customer Specific';
      case domain.DiscountType.loyaltyPoints:
        return 'Loyalty Points';
    }
  }

  String _getDiscountScopeDisplayName(domain.DiscountScope scope) {
    switch (scope) {
      case domain.DiscountScope.item:
        return 'Specific Items';
      case domain.DiscountScope.category:
        return 'Product Categories';
      case domain.DiscountScope.brand:
        return 'Product Brands';
      case domain.DiscountScope.cart:
        return 'Entire Cart';
      case domain.DiscountScope.shipping:
        return 'Shipping';
      case domain.DiscountScope.total:
        return 'Order Total';
    }
  }

  String _getDiscountApplicationDisplayName(domain.DiscountApplication application) {
    switch (application) {
      case domain.DiscountApplication.automatic:
        return 'Automatic';
      case domain.DiscountApplication.couponCode:
        return 'Coupon Code';
      case domain.DiscountApplication.loyaltyTier:
        return 'Loyalty Tier';
      case domain.DiscountApplication.customerGroup:
        return 'Customer Group';
      case domain.DiscountApplication.manual:
        return 'Manual Application';
    }
  }
}
