import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../../domain/entities/discount_entity.dart';
import '../../domain/entities/product_entity.dart';
import '../../domain/entities/customer_entity.dart';
import '../../data/services/discount_service.dart';
import '../../data/services/product_service.dart';
import '../../data/services/customer_service.dart';
import '../widgets/common/custom_app_bar.dart';
import '../widgets/common/loading_widget.dart';

class AddDiscountScreen extends StatefulWidget {
  final DiscountEntity? discount;

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
  DiscountType _selectedType = DiscountType.percentage;
  DiscountScope _selectedScope = DiscountScope.cart;
  DiscountApplication _selectedApplication = DiscountApplication.automatic;
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
      _minimumAmountController.text = discount.minimumAmount?.toString() ?? '';
      _maxDiscountAmountController.text = discount.maxDiscountAmount?.toString() ?? '';
      _usageLimitController.text = discount.usageLimit?.toString() ?? '';
      _usageLimitPerCustomerController.text = discount.usageLimitPerCustomer?.toString() ?? '';
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
        _getDiscountPercentageController.text = discount.bogoConfig!.getDiscountPercentage.toString();
      }
      
      // Initialize time-based configuration
      if (discount.timeBasedConfig != null) {
        _selectedDaysOfWeek = discount.timeBasedConfig!.daysOfWeek;
        _startTime = discount.timeBasedConfig!.startTime;
        _endTime = discount.timeBasedConfig!.endTime;
        _isSeasonalDiscount = discount.timeBasedConfig!.isSeasonalDiscount;
        _seasonStart = discount.timeBasedConfig!.seasonStartDate;
        _seasonEnd = discount.timeBasedConfig!.seasonEndDate;
      }
      
      // Initialize location configuration
      if (discount.locationConfig != null) {
        _applicableLocations = List.from(discount.locationConfig!.applicableLocations);
      }
      
      // Initialize target configuration
      _targetProductIds = List.from(discount.targetProductIds);
      _targetCategoryIds = List.from(discount.targetCategoryIds);
      _targetCustomerIds = List.from(discount.targetCustomerIds);
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
          
          DropdownButtonFormField<DiscountType>(
            value: _selectedType,
            decoration: const InputDecoration(
              labelText: 'Discount Type *',
            ),
            items: DiscountType.values.map((type) => DropdownMenuItem(
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
          
          if (_selectedType == DiscountType.percentage ||
              _selectedType == DiscountType.fixedAmount) ...[
            TextFormField(
              controller: _valueController,
              decoration: InputDecoration(
                labelText: _selectedType == DiscountType.percentage 
                    ? 'Percentage Value *' 
                    : 'Fixed Amount (GHS) *',
                hintText: _selectedType == DiscountType.percentage 
                    ? 'e.g., 10 (for 10%)' 
                    : 'e.g., 50.00',
                suffixText: _selectedType == DiscountType.percentage ? '%' : 'GHS',
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
                if (_selectedType == DiscountType.percentage && numValue > 100) {
                  return 'Percentage cannot be more than 100%';
                }
                return null;
              },
            ),
            const SizedBox(height: 16),
          ],
          
          if (_selectedType == DiscountType.bogo ||
              _selectedType == DiscountType.buyXGetY) ...[
            _buildBogoConfiguration(),
            const SizedBox(height: 16),
          ],
          
          DropdownButtonFormField<DiscountScope>(
            value: _selectedScope,
            decoration: const InputDecoration(
              labelText: 'Applies To *',
            ),
            items: DiscountScope.values.map((scope) => DropdownMenuItem(
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
          
          DropdownButtonFormField<DiscountApplication>(
            value: _selectedApplication,
            decoration: const InputDecoration(
              labelText: 'Application Type *',
            ),
            items: DiscountApplication.values.map((app) => DropdownMenuItem(
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
          
          if (_selectedApplication == DiscountApplication.coupon) ...[
            TextFormField(
              controller: _couponCodeController,
              decoration: const InputDecoration(
                labelText: 'Coupon Code *',
                hintText: 'e.g., SUMMER2024',
              ),
              textCapitalization: TextCapitalization.characters,
              validator: (value) {
                if (_selectedApplication == DiscountApplication.coupon &&
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
                    if ((_selectedType == DiscountType.bogo ||
                         _selectedType == DiscountType.buyXGetY) &&
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
                    if ((_selectedType == DiscountType.bogo ||
                         _selectedType == DiscountType.buyXGetY) &&
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
              if ((_selectedType == DiscountType.bogo ||
                   _selectedType == DiscountType.buyXGetY) &&
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
          
          if (_selectedScope == DiscountScope.item ||
              _selectedScope == DiscountScope.category ||
              _selectedScope == DiscountScope.brand) ...[
            const Text(
              'Product Restrictions',
              style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 16),
            
            if (_selectedScope == DiscountScope.item) ...[
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
                        (p) => p.id == productId,
                        orElse: () => ProductEntity(
                          id: productId,
                          name: 'Unknown Product',
                          barcode: '',
                          price: 0,
                          cost: 0,
                          category: '',
                          brand: '',
                          stockQuantity: 0,
                          unit: '',
                          lowStockThreshold: 0,
                          isActive: true,
                          createdAt: DateTime.now(),
                        ),
                      );
                      
                      return ListTile(
                        title: Text(product.name),
                        subtitle: Text('GHS ${product.price.toStringAsFixed(2)}'),
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
          
          if (_selectedType == DiscountType.customerSpecific) ...[
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
                      orElse: () => CustomerEntity(
                        id: customerId,
                        name: 'Unknown Customer',
                        email: '',
                        phone: '',
                        address: '',
                        city: '',
                        totalPurchases: 0,
                        loyaltyPoints: 0,
                        isActive: true,
                        createdAt: DateTime.now(),
                      ),
                    );
                    
                    return ListTile(
                      title: Text(customer.name),
                      subtitle: Text(customer.email),
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
              final isSelected = _targetProductIds.contains(product.id);
              
              return CheckboxListTile(
                title: Text(product.name),
                subtitle: Text('GHS ${product.price.toStringAsFixed(2)}'),
                value: isSelected,
                onChanged: (value) {
                  setState(() {
                    if (value!) {
                      _targetProductIds.add(product.id);
                    } else {
                      _targetProductIds.remove(product.id);
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
                title: Text(customer.name),
                subtitle: Text(customer.email),
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
      BogoConfig? bogoConfig;
      if (_selectedType == DiscountType.bogo || _selectedType == DiscountType.buyXGetY) {
        bogoConfig = BogoConfig(
          buyQuantity: int.parse(_buyQuantityController.text),
          getQuantity: int.parse(_getQuantityController.text),
          getDiscountPercentage: double.parse(_getDiscountPercentageController.text),
        );
      }

      // Create time-based configuration if needed
      TimeBasedConfig? timeBasedConfig;
      if (_selectedDaysOfWeek.isNotEmpty || _startTime != null || _endTime != null || _isSeasonalDiscount) {
        timeBasedConfig = TimeBasedConfig(
          daysOfWeek: _selectedDaysOfWeek,
          startTime: _startTime,
          endTime: _endTime,
          isSeasonalDiscount: _isSeasonalDiscount,
          seasonStartDate: _seasonStart,
          seasonEndDate: _seasonEnd,
        );
      }

      // Create location configuration if needed
      LocationConfig? locationConfig;
      if (_applicableLocations.isNotEmpty) {
        locationConfig = LocationConfig(
          applicableLocations: _applicableLocations,
        );
      }

      final discount = DiscountEntity(
        id: widget.discount?.id ?? DateTime.now().millisecondsSinceEpoch.toString(),
        name: _nameController.text.trim(),
        description: _descriptionController.text.trim(),
        type: _selectedType,
        scope: _selectedScope,
        application: _selectedApplication,
        value: double.parse(_valueController.text.isNotEmpty ? _valueController.text : '0'),
        minimumAmount: _minimumAmountController.text.isNotEmpty 
            ? double.parse(_minimumAmountController.text) 
            : null,
        maxDiscountAmount: _maxDiscountAmountController.text.isNotEmpty 
            ? double.parse(_maxDiscountAmountController.text) 
            : null,
        startDate: _startDate ?? DateTime.now(),
        endDate: _endDate ?? DateTime.now().add(const Duration(days: 30)),
        isActive: _isActive,
        isStackable: _isStackable,
        priority: _priority,
        usageLimit: _usageLimitController.text.isNotEmpty 
            ? int.parse(_usageLimitController.text) 
            : null,
        usageLimitPerCustomer: _usageLimitPerCustomerController.text.isNotEmpty 
            ? int.parse(_usageLimitPerCustomerController.text) 
            : null,
        currentUses: widget.discount?.currentUses ?? 0,
        couponCode: _couponCodeController.text.isNotEmpty 
            ? _couponCodeController.text.trim() 
            : null,
        targetProductIds: _targetProductIds,
        targetCategoryIds: _targetCategoryIds,
        targetCustomerIds: _targetCustomerIds,
        bogoConfig: bogoConfig,
        timeBasedConfig: timeBasedConfig,
        locationConfig: locationConfig,
        createdAt: widget.discount?.createdAt ?? DateTime.now(),
        updatedAt: DateTime.now(),
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
  String _getDiscountTypeDisplayName(DiscountType type) {
    switch (type) {
      case DiscountType.percentage:
        return 'Percentage Discount';
      case DiscountType.fixedAmount:
        return 'Fixed Amount Discount';
      case DiscountType.bogo:
        return 'Buy One Get One (BOGO)';
      case DiscountType.buyXGetY:
        return 'Buy X Get Y';
      case DiscountType.freeShipping:
        return 'Free Shipping';
      case DiscountType.customerSpecific:
        return 'Customer Specific';
      case DiscountType.loyaltyPoints:
        return 'Loyalty Points';
    }
  }

  String _getDiscountScopeDisplayName(DiscountScope scope) {
    switch (scope) {
      case DiscountScope.item:
        return 'Specific Items';
      case DiscountScope.category:
        return 'Product Categories';
      case DiscountScope.brand:
        return 'Product Brands';
      case DiscountScope.cart:
        return 'Entire Cart';
      case DiscountScope.shipping:
        return 'Shipping';
      case DiscountScope.total:
        return 'Order Total';
    }
  }

  String _getDiscountApplicationDisplayName(DiscountApplication application) {
    switch (application) {
      case DiscountApplication.automatic:
        return 'Automatic';
      case DiscountApplication.couponCode:
        return 'Coupon Code';
      case DiscountApplication.loyaltyTier:
        return 'Loyalty Tier';
      case DiscountApplication.customerGroup:
        return 'Customer Group';
      case DiscountApplication.manual:
        return 'Manual Application';
    }
  }
}
