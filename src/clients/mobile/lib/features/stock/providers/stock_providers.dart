import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../data/models/stock_item.dart';
import '../data/repositories/stock_repository.dart';
import '../data/services/stock_api_service.dart';
import '../../../core/network/api_service.dart';

// API Service Provider
final stockApiServiceProvider = Provider<StockApiService>((ref) {
  final apiService = ref.read(apiServiceProvider);
  return StockApiService(apiService.dio);
});

// Repository Provider
final stockRepositoryProvider = Provider<StockRepository>((ref) {
  final stockApiService = ref.read(stockApiServiceProvider);
  return StockRepository(stockApiService);
});

// Stock Overview Provider
final stockOverviewProvider = FutureProvider<StockOverview>((ref) async {
  final repository = ref.read(stockRepositoryProvider);
  return await repository.getStockOverview();
});

// Stock Items Provider with search and filters
final stockItemsProvider = FutureProvider.family<List<StockItem>, StockFilters>((ref, filters) async {
  final repository = ref.read(stockRepositoryProvider);
  return await repository.getStockItems(
    search: filters.search,
    category: filters.category,
    status: filters.status,
    page: filters.page,
    limit: filters.limit,
  );
});

// Recent Stock Movements Provider
final recentMovementsProvider = FutureProvider<List<StockMovement>>((ref) async {
  final repository = ref.read(stockRepositoryProvider);
  return await repository.getRecentMovements(limit: 10);
});

// Categories Provider
final categoriesProvider = FutureProvider<List<String>>((ref) async {
  final repository = ref.read(stockRepositoryProvider);
  return await repository.getCategories();
});

// Low Stock Items Provider
final lowStockItemsProvider = FutureProvider<List<StockItem>>((ref) async {
  final repository = ref.read(stockRepositoryProvider);
  return await repository.getLowStockItems();
});

// Out of Stock Items Provider
final outOfStockItemsProvider = FutureProvider<List<StockItem>>((ref) async {
  final repository = ref.read(stockRepositoryProvider);
  return await repository.getOutOfStockItems();
});

// Single Stock Item Provider
final stockItemProvider = FutureProvider.family<StockItem, String>((ref, id) async {
  final repository = ref.read(stockRepositoryProvider);
  return await repository.getStockItem(id);
});

// Search State Provider
final searchQueryProvider = StateProvider<String>((ref) => '');

// Filter State Provider
final stockFiltersProvider = StateProvider<StockFilters>((ref) => const StockFilters());

// Selected Category Provider
final selectedCategoryProvider = StateProvider<String?>((ref) => null);

// Loading State Provider
final stockLoadingProvider = StateProvider<bool>((ref) => false);

// Error State Provider
final stockErrorProvider = StateProvider<String?>((ref) => null);

// Stock Operations Controller
final stockControllerProvider = StateNotifierProvider<StockController, StockState>((ref) {
  final repository = ref.read(stockRepositoryProvider);
  return StockController(repository);
});

// Filter Model
class StockFilters {
  final String? search;
  final String? category;
  final String? status;
  final int? page;
  final int? limit;

  const StockFilters({
    this.search,
    this.category,
    this.status,
    this.page,
    this.limit,
  });

  StockFilters copyWith({
    String? search,
    String? category,
    String? status,
    int? page,
    int? limit,
  }) {
    return StockFilters(
      search: search ?? this.search,
      category: category ?? this.category,
      status: status ?? this.status,
      page: page ?? this.page,
      limit: limit ?? this.limit,
    );
  }
}

// Stock State
class StockState {
  final List<StockItem> items;
  final StockOverview? overview;
  final List<StockMovement> recentMovements;
  final List<String> categories;
  final bool isLoading;
  final String? error;
  final StockFilters filters;

  const StockState({
    this.items = const [],
    this.overview,
    this.recentMovements = const [],
    this.categories = const [],
    this.isLoading = false,
    this.error,
    this.filters = const StockFilters(),
  });

  StockState copyWith({
    List<StockItem>? items,
    StockOverview? overview,
    List<StockMovement>? recentMovements,
    List<String>? categories,
    bool? isLoading,
    String? error,
    StockFilters? filters,
  }) {
    return StockState(
      items: items ?? this.items,
      overview: overview ?? this.overview,
      recentMovements: recentMovements ?? this.recentMovements,
      categories: categories ?? this.categories,
      isLoading: isLoading ?? this.isLoading,
      error: error ?? this.error,
      filters: filters ?? this.filters,
    );
  }
}

// Stock Controller
class StockController extends StateNotifier<StockState> {
  final StockRepository _repository;

  StockController(this._repository) : super(const StockState());

  // Load initial data
  Future<void> loadInitialData() async {
    state = state.copyWith(isLoading: true, error: null);
    
    try {
      final futures = await Future.wait([
        _repository.getStockOverview(),
        _repository.getStockItems(limit: 50),
        _repository.getRecentMovements(limit: 10),
        _repository.getCategories(),
      ]);

      state = state.copyWith(
        overview: futures[0] as StockOverview,
        items: futures[1] as List<StockItem>,
        recentMovements: futures[2] as List<StockMovement>,
        categories: futures[3] as List<String>,
        isLoading: false,
      );
    } catch (e) {
      state = state.copyWith(
        isLoading: false,
        error: e.toString(),
      );
    }
  }

  // Refresh data
  Future<void> refresh() async {
    await loadInitialData();
  }

  // Search items
  Future<void> searchItems(String query) async {
    if (query.isEmpty) {
      await loadItems();
      return;
    }

    state = state.copyWith(isLoading: true, error: null);
    
    try {
      final items = await _repository.searchItems(query);
      state = state.copyWith(
        items: items,
        isLoading: false,
        filters: state.filters.copyWith(search: query),
      );
    } catch (e) {
      state = state.copyWith(
        isLoading: false,
        error: e.toString(),
      );
    }
  }

  // Load items with filters
  Future<void> loadItems({StockFilters? filters}) async {
    final currentFilters = filters ?? state.filters;
    state = state.copyWith(isLoading: true, error: null);
    
    try {
      final items = await _repository.getStockItems(
        search: currentFilters.search,
        category: currentFilters.category,
        status: currentFilters.status,
        page: currentFilters.page,
        limit: currentFilters.limit,
      );
      
      state = state.copyWith(
        items: items,
        isLoading: false,
        filters: currentFilters,
      );
    } catch (e) {
      state = state.copyWith(
        isLoading: false,
        error: e.toString(),
      );
    }
  }

  // Filter by category
  Future<void> filterByCategory(String? category) async {
    final newFilters = state.filters.copyWith(category: category);
    await loadItems(filters: newFilters);
  }

  // Filter by status
  Future<void> filterByStatus(String? status) async {
    final newFilters = state.filters.copyWith(status: status);
    await loadItems(filters: newFilters);
  }

  // Create new item
  Future<void> createItem(CreateStockItemRequest request) async {
    state = state.copyWith(isLoading: true, error: null);
    
    try {
      final newItem = await _repository.createStockItem(request);
      state = state.copyWith(
        items: [newItem, ...state.items],
        isLoading: false,
      );
      
      // Refresh overview after creating item
      final overview = await _repository.getStockOverview();
      state = state.copyWith(overview: overview);
    } catch (e) {
      state = state.copyWith(
        isLoading: false,
        error: e.toString(),
      );
    }
  }

  // Update item
  Future<void> updateItem(String id, Map<String, dynamic> updates) async {
    state = state.copyWith(isLoading: true, error: null);
    
    try {
      final updatedItem = await _repository.updateStockItem(id, updates);
      final updatedItems = state.items.map((item) {
        return item.id == id ? updatedItem : item;
      }).toList();
      
      state = state.copyWith(
        items: updatedItems,
        isLoading: false,
      );
    } catch (e) {
      state = state.copyWith(
        isLoading: false,
        error: e.toString(),
      );
    }
  }

  // Delete item
  Future<void> deleteItem(String id) async {
    state = state.copyWith(isLoading: true, error: null);
    
    try {
      await _repository.deleteStockItem(id);
      final updatedItems = state.items.where((item) => item.id != id).toList();
      
      state = state.copyWith(
        items: updatedItems,
        isLoading: false,
      );
      
      // Refresh overview after deleting item
      final overview = await _repository.getStockOverview();
      state = state.copyWith(overview: overview);
    } catch (e) {
      state = state.copyWith(
        isLoading: false,
        error: e.toString(),
      );
    }
  }

  // Adjust stock
  Future<void> adjustStock(StockAdjustmentRequest request) async {
    state = state.copyWith(isLoading: true, error: null);
    
    try {
      final movement = await _repository.adjustStock(request);
      
      // Add movement to recent movements
      state = state.copyWith(
        recentMovements: [movement, ...state.recentMovements.take(9).toList()],
        isLoading: false,
      );
      
      // Refresh data to get updated quantities
      await refresh();
    } catch (e) {
      state = state.copyWith(
        isLoading: false,
        error: e.toString(),
      );
    }
  }

  // Clear error
  void clearError() {
    state = state.copyWith(error: null);
  }
}

// Add this to your main app providers file
final apiServiceProvider = Provider<ApiService>((ref) {
  return ApiService();
});

