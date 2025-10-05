import 'package:flutter/foundation.dart';

import '../../../domain/entities/supplier_entity.dart';
import '../../../domain/usecases/supplier_usecases.dart';

class SupplierProvider extends ChangeNotifier {
  final GetAllSuppliers _getAllSuppliers;
  final GetSupplierById _getSupplierById;
  final GetActiveSuppliers _getActiveSuppliers;
  final CreateSupplier _createSupplier;
  final UpdateSupplier _updateSupplier;
  final DeleteSupplier _deleteSupplier;
  final SeedSampleSuppliers _seedSampleSuppliers;

  SupplierProvider(
    this._getAllSuppliers,
    this._getSupplierById,
    this._getActiveSuppliers,
    this._createSupplier,
    this._updateSupplier,
    this._deleteSupplier,
    this._seedSampleSuppliers,
  );

  // State
  List<SupplierEntity> _suppliers = [];
  List<SupplierEntity> _filteredSuppliers = [];
  SupplierEntity? _selectedSupplier;
  bool _isLoading = false;
  String? _error;
  String _searchQuery = '';

  // Getters
  List<SupplierEntity> get suppliers => _filteredSuppliers;
  SupplierEntity? get selectedSupplier => _selectedSupplier;
  bool get isLoading => _isLoading;
  String? get error => _error;
  String get searchQuery => _searchQuery;

  // Statistics
  int get totalSuppliers => _suppliers.length;
  int get activeSuppliers => _suppliers.where((s) => s.isActive).length;
  int get inactiveSuppliers => _suppliers.where((s) => !s.isActive).length;
  double get averageRating => _suppliers.isNotEmpty 
    ? _suppliers.where((s) => s.rating != null).map((s) => s.rating!).reduce((a, b) => a + b) / 
      _suppliers.where((s) => s.rating != null).length 
    : 0.0;

  // Actions
  Future<void> loadAllSuppliers() async {
    _setLoading(true);
    _clearError();

    final result = await _getAllSuppliers(null);
    if (result.isSuccess) {
      _suppliers = result.data ?? [];
      _applyFilters();
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to load suppliers');
    }

    _setLoading(false);
  }

  Future<void> loadSupplierById(int id) async {
    _setLoading(true);
    _clearError();

    final result = await _getSupplierById(id);
    if (result.isSuccess) {
      _selectedSupplier = result.data;
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to load supplier');
    }

    _setLoading(false);
  }

  Future<void> searchSuppliers(String query) async {
    _searchQuery = query;
    _applyFilters();
    notifyListeners();
  }

  Future<void> loadActiveSuppliers() async {
    _setLoading(true);
    _clearError();

    final result = await _getActiveSuppliers(null);
    if (result.isSuccess) {
      _suppliers = result.data ?? [];
      _applyFilters();
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to load active suppliers');
    }

    _setLoading(false);
  }

  Future<bool> createSupplier(SupplierEntity supplier) async {
    _setLoading(true);
    _clearError();

    final result = await _createSupplier(supplier);
    if (result.isSuccess) {
      _setLoading(false);
      // Reload suppliers to get updated list
      await loadAllSuppliers();
      return true;
    } else {
      _setError(result.error?.message ?? 'Failed to create supplier');
      _setLoading(false);
      return false;
    }
  }

  Future<bool> updateSupplier(SupplierEntity supplier) async {
    _setLoading(true);
    _clearError();

    final result = await _updateSupplier(supplier);
    if (result.isSuccess) {
      _setLoading(false);
      // Update local list
      final index = _suppliers.indexWhere((s) => s.id == supplier.id);
      if (index != -1) {
        _suppliers[index] = supplier;
        _applyFilters();
        notifyListeners();
      }
      return true;
    } else {
      _setError(result.error?.message ?? 'Failed to update supplier');
      _setLoading(false);
      return false;
    }
  }

  Future<bool> deleteSupplier(int id) async {
    _setLoading(true);
    _clearError();

    final result = await _deleteSupplier(id);
    if (result.isSuccess) {
      _setLoading(false);
      // Remove from local list
      _suppliers.removeWhere((s) => s.id == id);
      _applyFilters();
      notifyListeners();
      return true;
    } else {
      _setError(result.error?.message ?? 'Failed to delete supplier');
      _setLoading(false);
      return false;
    }
  }

  Future<void> seedSampleSuppliers() async {
    _setLoading(true);
    _clearError();

    final result = await _seedSampleSuppliers(null);
    if (result.isSuccess) {
      _setLoading(false);
      // Reload suppliers to get updated list
      await loadAllSuppliers();
    } else {
      _setError(result.error?.message ?? 'Failed to seed sample suppliers');
      _setLoading(false);
    }
  }

  void selectSupplier(SupplierEntity? supplier) {
    _selectedSupplier = supplier;
    notifyListeners();
  }

  void clearSelection() {
    _selectedSupplier = null;
    notifyListeners();
  }

  void clearSearch() {
    _searchQuery = '';
    _applyFilters();
    notifyListeners();
  }

  // Private methods
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
    _filteredSuppliers = _suppliers.where((supplier) {
      // Search filter
      if (_searchQuery.isNotEmpty) {
        final query = _searchQuery.toLowerCase();
        final matchesSearch = 
            supplier.name.toLowerCase().contains(query) ||
            (supplier.companyName?.toLowerCase().contains(query) ?? false) ||
            (supplier.contactPerson?.toLowerCase().contains(query) ?? false) ||
            (supplier.email?.toLowerCase().contains(query) ?? false);
        if (!matchesSearch) return false;
      }

      return true;
    }).toList();
  }

  // Utility methods
  SupplierEntity createNewSupplier({
    required String name,
    String? companyName,
    String? contactPerson,
    String? phoneNumber,
    String? email,
    String? address,
    String? city,
    String? country,
    String? taxNumber,
    Map<String, dynamic>? paymentTerms,
    bool isActive = true,
    double? rating,
    String? notes,
  }) {
    return SupplierEntity(
      id: DateTime.now().millisecondsSinceEpoch,
      name: name,
      companyName: companyName,
      contactPerson: contactPerson,
      phoneNumber: phoneNumber,
      email: email,
      address: address,
      city: city,
      country: country,
      taxNumber: taxNumber,
      paymentTerms: paymentTerms,
      isActive: isActive,
      rating: rating,
      notes: notes,
      createdById: 'current_user', // TODO: Get from auth
      createdAt: DateTime.now().toIso8601String(),
      updatedAt: DateTime.now().toIso8601String(),
    );
  }

  List<SupplierEntity> getTopRatedSuppliers({int limit = 5}) {
    final sortedSuppliers = List<SupplierEntity>.from(_suppliers);
    sortedSuppliers.sort((a, b) => (b.rating ?? 0.0).compareTo(a.rating ?? 0.0));
    return sortedSuppliers.take(limit).toList();
  }

  List<SupplierEntity> getRecentSuppliers({int limit = 10}) {
    final sortedSuppliers = List<SupplierEntity>.from(_suppliers);
    sortedSuppliers.sort((a, b) {
      final aDate = DateTime.tryParse(a.createdAt ?? '') ?? DateTime(1970);
      final bDate = DateTime.tryParse(b.createdAt ?? '') ?? DateTime(1970);
      return bDate.compareTo(aDate);
    });
    return sortedSuppliers.take(limit).toList();
  }
}
