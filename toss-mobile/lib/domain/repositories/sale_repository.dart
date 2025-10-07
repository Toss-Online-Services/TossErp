import '../../core/usecase/usecase.dart';
import '../entities/sale_entity.dart';

abstract class SaleRepository {
  Future<Result<List<SaleEntity>>> getAllSales();
  Future<Result<SaleEntity?>> getSaleById(int id);
  Future<Result<SaleEntity?>> getSaleBySaleNumber(String saleNumber);
  Future<Result<List<SaleEntity>>> getSalesByStatus(SaleStatus status);
  Future<Result<List<SaleEntity>>> getSalesByDateRange(DateTime startDate, DateTime endDate);
  Future<Result<List<SaleEntity>>> getSalesByCustomer(int customerId);
  Future<Result<List<SaleEntity>>> getSalesByCashier(int cashierId);
  Future<Result<int>> createSale(SaleEntity sale);
  Future<Result<void>> updateSale(SaleEntity sale);
  Future<Result<void>> deleteSale(int id);
  Future<Result<void>> cancelSale(int id, String reason);
  Future<Result<void>> completeSale(int id);
  Future<Result<SaleEntity?>> getTodaySales();
  Future<Result<Map<String, dynamic>>> getSalesSummary(DateTime startDate, DateTime endDate);
}

