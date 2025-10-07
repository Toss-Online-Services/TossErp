import '../entities/supplier_entity.dart';

abstract class SupplierRepository {
  Future<List<SupplierEntity>> getAllSuppliers();
  Future<SupplierEntity?> getSupplierById(int id);
  Future<List<SupplierEntity>> searchSuppliers(String query);
  Future<List<SupplierEntity>> getActiveSuppliers();
  Future<int> createSupplier(SupplierEntity supplier);
  Future<void> updateSupplier(SupplierEntity supplier);
  Future<void> deleteSupplier(int id);
  Future<void> seedSampleSuppliers();
}
