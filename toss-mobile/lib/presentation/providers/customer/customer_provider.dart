import 'package:flutter/foundation.dart';
import 'package:uuid/uuid.dart';

import '../../../domain/entities/customer_entity.dart';
import '../../../domain/usecases/customer_usecases.dart';

class CustomerProvider extends ChangeNotifier {
  final GetAllCustomers _getAllCustomers;
  final GetCustomerById _getCustomerById;
  final GetCustomerByPhone _getCustomerByPhone;
  final GetCustomerByMembershipNumber _getCustomerByMembershipNumber;
  final SearchCustomers _searchCustomers;
  final GetCustomersByTier _getCustomersByTier;
  final GetCustomersByLoyaltyTier _getCustomersByLoyaltyTier;
  final GetCustomersByTag _getCustomersByTag;
  final GetInactiveCustomers _getInactiveCustomers;
  final GetBirthdayCustomers _getBirthdayCustomers;
  final GetAnniversaryCustomers _getAnniversaryCustomers;
  final CreateCustomer _createCustomer;
  final UpdateCustomer _updateCustomer;
  final DeleteCustomer _deleteCustomer;
  final UpdateCustomerPoints _updateCustomerPoints;
  final UpdateCustomerLoyaltyPoints _updateCustomerLoyaltyPoints;
  final UpdateCustomerVisit _updateCustomerVisit;
  final SeedSampleCustomers _seedSampleCustomers;

  CustomerProvider(
    this._getAllCustomers,
    this._getCustomerById,
    this._getCustomerByPhone,
    this._getCustomerByMembershipNumber,
    this._searchCustomers,
    this._getCustomersByTier,
    this._getCustomersByLoyaltyTier,
    this._getCustomersByTag,
    this._getInactiveCustomers,
    this._getBirthdayCustomers,
    this._getAnniversaryCustomers,
    this._createCustomer,
    this._updateCustomer,
    this._deleteCustomer,
    this._updateCustomerPoints,
    this._updateCustomerLoyaltyPoints,
    this._updateCustomerVisit,
    this._seedSampleCustomers,
  );

  // State
  List<CustomerEntity> _customers = [];
  List<CustomerEntity> _filteredCustomers = [];
  CustomerEntity? _selectedCustomer;
  bool _isLoading = false;
  String? _error;
  String _searchQuery = '';
  CustomerTier? _selectedTier;
  LoyaltyTier? _selectedLoyaltyTier;
  String? _selectedTag;
  bool _showInactiveOnly = false;

  // Getters
  List<CustomerEntity> get customers => _filteredCustomers;
  CustomerEntity? get selectedCustomer => _selectedCustomer;
  bool get isLoading => _isLoading;
  String? get error => _error;
  String get searchQuery => _searchQuery;
  CustomerTier? get selectedTier => _selectedTier;
  LoyaltyTier? get selectedLoyaltyTier => _selectedLoyaltyTier;
  String? get selectedTag => _selectedTag;
  bool get showInactiveOnly => _showInactiveOnly;

  // Statistics
  int get totalCustomers => _customers.length;
  int get activeCustomers => _customers.where((c) => c.isActive).length;
  int get inactiveCustomers => _customers.where((c) => !c.isActive).length;
  int get birthdayCustomers => _customers.where((c) => c.isBirthday).length;
  int get anniversaryCustomers => _customers.where((c) => c.isAnniversary).length;
  double get totalCustomerValue => _customers.fold(0.0, (sum, c) => sum + c.totalSpent);
  double get averageCustomerValue => totalCustomers > 0 ? totalCustomerValue / totalCustomers : 0.0;

  // Actions
  Future<void> loadAllCustomers() async {
    _setLoading(true);
    _clearError();

    final result = await _getAllCustomers(null);
    if (result.isSuccess) {
      _customers = result.data ?? [];
      _applyFilters();
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to load customers');
    }

    _setLoading(false);
  }

  Future<void> loadCustomerById(String id) async {
    _setLoading(true);
    _clearError();

    final result = await _getCustomerById(id);
    if (result.isSuccess) {
      _selectedCustomer = result.data;
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to load customer');
    }

    _setLoading(false);
  }

  Future<void> loadCustomerByPhone(String phone) async {
    _setLoading(true);
    _clearError();

    final result = await _getCustomerByPhone(phone);
    if (result.isSuccess) {
      _selectedCustomer = result.data;
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to load customer');
    }

    _setLoading(false);
  }

  Future<void> loadCustomerByMembershipNumber(String membershipNumber) async {
    _setLoading(true);
    _clearError();

    final result = await _getCustomerByMembershipNumber(membershipNumber);
    if (result.isSuccess) {
      _selectedCustomer = result.data;
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to load customer');
    }

    _setLoading(false);
  }

  Future<void> searchCustomers(String query) async {
    _searchQuery = query;
    _applyFilters();
    notifyListeners();

    if (query.isEmpty) {
      _filteredCustomers = _customers;
      return;
    }

    _setLoading(true);
    _clearError();

    final result = await _searchCustomers(query);
    if (result.isSuccess) {
      _filteredCustomers = result.data ?? [];
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to search customers');
    }

    _setLoading(false);
  }

  Future<void> filterByTier(CustomerTier? tier) async {
    _selectedTier = tier;
    _applyFilters();
    notifyListeners();

    if (tier == null) {
      _filteredCustomers = _customers;
      return;
    }

    _setLoading(true);
    _clearError();

    final result = await _getCustomersByTier(tier);
    if (result.isSuccess) {
      _filteredCustomers = result.data ?? [];
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to filter customers by tier');
    }

    _setLoading(false);
  }

  Future<void> filterByLoyaltyTier(LoyaltyTier? loyaltyTier) async {
    _selectedLoyaltyTier = loyaltyTier;
    _applyFilters();
    notifyListeners();

    if (loyaltyTier == null) {
      _filteredCustomers = _customers;
      return;
    }

    _setLoading(true);
    _clearError();

    final result = await _getCustomersByLoyaltyTier(loyaltyTier);
    if (result.isSuccess) {
      _filteredCustomers = result.data ?? [];
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to filter customers by loyalty tier');
    }

    _setLoading(false);
  }

  Future<void> filterByTag(String? tag) async {
    _selectedTag = tag;
    _applyFilters();
    notifyListeners();

    if (tag == null) {
      _filteredCustomers = _customers;
      return;
    }

    _setLoading(true);
    _clearError();

    final result = await _getCustomersByTag(tag);
    if (result.isSuccess) {
      _filteredCustomers = result.data ?? [];
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to filter customers by tag');
    }

    _setLoading(false);
  }

  Future<void> loadInactiveCustomers({int daysThreshold = 90}) async {
    _showInactiveOnly = true;
    _setLoading(true);
    _clearError();

    final result = await _getInactiveCustomers(daysThreshold);
    if (result.isSuccess) {
      _filteredCustomers = result.data ?? [];
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to load inactive customers');
    }

    _setLoading(false);
  }

  Future<void> loadBirthdayCustomers() async {
    _setLoading(true);
    _clearError();

    final result = await _getBirthdayCustomers(null);
    if (result.isSuccess) {
      _filteredCustomers = result.data ?? [];
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to load birthday customers');
    }

    _setLoading(false);
  }

  Future<void> loadAnniversaryCustomers() async {
    _setLoading(true);
    _clearError();

    final result = await _getAnniversaryCustomers(null);
    if (result.isSuccess) {
      _filteredCustomers = result.data ?? [];
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to load anniversary customers');
    }

    _setLoading(false);
  }

  Future<bool> createCustomer(CustomerEntity customer) async {
    _setLoading(true);
    _clearError();

    final result = await _createCustomer(customer);
    if (result.isSuccess) {
      _setLoading(false);
      // Reload customers to get updated list
      loadAllCustomers();
      return true;
    } else {
      _setError(result.error?.message ?? 'Failed to create customer');
      _setLoading(false);
      return false;
    }
  }

  Future<bool> updateCustomer(CustomerEntity customer) async {
    _setLoading(true);
    _clearError();

    final result = await _updateCustomer(customer);
    if (result.isSuccess) {
      _setLoading(false);
      // Update local list
      final index = _customers.indexWhere((c) => c.id == customer.id);
      if (index != -1) {
        _customers[index] = customer;
        _applyFilters();
        notifyListeners();
      }
      return true;
    } else {
      _setError(result.error?.message ?? 'Failed to update customer');
      _setLoading(false);
      return false;
    }
  }

  Future<bool> deleteCustomer(String id) async {
    _setLoading(true);
    _clearError();

    final result = await _deleteCustomer(id);
    if (result.isSuccess) {
      _setLoading(false);
      // Remove from local list
      _customers.removeWhere((c) => c.id == id);
      _applyFilters();
      notifyListeners();
      return true;
    } else {
      _setError(result.error?.message ?? 'Failed to delete customer');
      _setLoading(false);
      return false;
    }
  }

  Future<bool> updateCustomerPoints(String customerId, int points) async {
    _setLoading(true);
    _clearError();

    final result = await _updateCustomerPoints(UpdateCustomerPointsParams(
      customerId: customerId,
      points: points,
    ));
    if (result.isSuccess) {
      _setLoading(false);
      // Update local list
      final index = _customers.indexWhere((c) => c.id == customerId);
      if (index != -1) {
        _customers[index] = _customers[index].copyWith(pointsBalance: points);
        _applyFilters();
        notifyListeners();
      }
      return true;
    } else {
      _setError(result.error?.message ?? 'Failed to update customer points');
      _setLoading(false);
      return false;
    }
  }

  Future<bool> updateCustomerLoyaltyPoints(String customerId, int loyaltyPoints) async {
    _setLoading(true);
    _clearError();

    final result = await _updateCustomerLoyaltyPoints(UpdateCustomerLoyaltyPointsParams(
      customerId: customerId,
      loyaltyPoints: loyaltyPoints,
    ));
    if (result.isSuccess) {
      _setLoading(false);
      // Update local list
      final index = _customers.indexWhere((c) => c.id == customerId);
      if (index != -1) {
        _customers[index] = _customers[index].copyWith(loyaltyPoints: loyaltyPoints);
        _applyFilters();
        notifyListeners();
      }
      return true;
    } else {
      _setError(result.error?.message ?? 'Failed to update customer loyalty points');
      _setLoading(false);
      return false;
    }
  }

  Future<bool> updateCustomerVisit(String customerId, double amount) async {
    _setLoading(true);
    _clearError();

    final result = await _updateCustomerVisit(UpdateCustomerVisitParams(
      customerId: customerId,
      amount: amount,
    ));
    if (result.isSuccess) {
      _setLoading(false);
      // Update local list
      final index = _customers.indexWhere((c) => c.id == customerId);
      if (index != -1) {
        final customer = _customers[index];
        _customers[index] = customer.copyWith(
          visitCount: customer.visitCount + 1,
          lastVisit: DateTime.now(),
          totalSpent: customer.totalSpent + amount,
        );
        _applyFilters();
        notifyListeners();
      }
      return true;
    } else {
      _setError(result.error?.message ?? 'Failed to update customer visit');
      _setLoading(false);
      return false;
    }
  }

  Future<void> seedSampleCustomers() async {
    _setLoading(true);
    _clearError();

    final result = await _seedSampleCustomers(null);
    if (result.isSuccess) {
      _setLoading(false);
      // Reload customers to get seeded data
      loadAllCustomers();
    } else {
      _setError(result.error?.message ?? 'Failed to seed sample customers');
      _setLoading(false);
    }
  }

  void selectCustomer(CustomerEntity? customer) {
    _selectedCustomer = customer;
    notifyListeners();
  }

  void clearFilters() {
    _searchQuery = '';
    _selectedTier = null;
    _selectedLoyaltyTier = null;
    _selectedTag = null;
    _showInactiveOnly = false;
    _filteredCustomers = _customers;
    notifyListeners();
  }

  void clearError() {
    _clearError();
    notifyListeners();
  }

  // Helper methods
  void _setLoading(bool loading) {
    _isLoading = loading;
    notifyListeners();
  }

  void _setError(String error) {
    _error = error;
    notifyListeners();
  }

  void _clearError() {
    _error = null;
  }

  void _applyFilters() {
    _filteredCustomers = _customers.where((customer) {
      // Search filter
      if (_searchQuery.isNotEmpty) {
        final query = _searchQuery.toLowerCase();
        final matchesSearch = 
            (customer.name?.toLowerCase().contains(query) ?? false) ||
            (customer.phone?.toLowerCase().contains(query) ?? false) ||
            (customer.membershipNumber?.toLowerCase().contains(query) ?? false);
        if (!matchesSearch) return false;
      }

      // Tier filter
      if (_selectedTier != null && customer.tier != _selectedTier) {
        return false;
      }

      // Loyalty tier filter
      if (_selectedLoyaltyTier != null && customer.loyaltyTier != _selectedLoyaltyTier) {
        return false;
      }

      // Tag filter
      if (_selectedTag != null && !(customer.tags?.contains(_selectedTag) ?? false)) {
        return false;
      }

      // Inactive filter
      if (_showInactiveOnly && customer.isActive) {
        return false;
      }

      return true;
    }).toList();
  }

  // Utility methods
  CustomerEntity createNewCustomer({
    String? name,
    String? phone,
    CustomerTier tier = CustomerTier.bronze,
    LoyaltyTier loyaltyTier = LoyaltyTier.none,
  }) {
    return CustomerEntity(
      id: const Uuid().v4(),
      name: name,
      phone: phone,
      primaryIdType: CustomerIdType.phone,
      tier: tier,
      loyaltyTier: loyaltyTier,
      pointsBalance: 0,
      loyaltyPoints: 0,
      totalSpent: 0.0,
      visitCount: 0,
      preferredCommunication: PreferredCommunication.none,
      isActive: true,
      createdAt: DateTime.now().toIso8601String(),
      updatedAt: DateTime.now().toIso8601String(),
    );
  }

  List<String> getAllTags() {
    final tags = <String>{};
    for (final customer in _customers) {
      if (customer.tags != null) {
        tags.addAll(customer.tags!);
      }
    }
    return tags.toList()..sort();
  }

  Map<CustomerTier, int> getCustomersByTierCount() {
    final counts = <CustomerTier, int>{};
    for (final tier in CustomerTier.values) {
      counts[tier] = _customers.where((c) => c.tier == tier).length;
    }
    return counts;
  }

  Map<LoyaltyTier, int> getCustomersByLoyaltyTierCount() {
    final counts = <LoyaltyTier, int>{};
    for (final tier in LoyaltyTier.values) {
      counts[tier] = _customers.where((c) => c.loyaltyTier == tier).length;
    }
    return counts;
  }
}
