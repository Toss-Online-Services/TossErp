import '../../domain/entities/supplier_entity.dart';

abstract class SupplierDatasource {
  Future<int> createSupplier(SupplierEntity supplier);
  Future<List<SupplierEntity>> getAllSuppliers();
  Future<SupplierEntity?> getSupplierById(int id);
  Future<List<SupplierEntity>> searchSuppliers(String query);
  Future<List<SupplierEntity>> getActiveSuppliers();
  Future<void> updateSupplier(SupplierEntity supplier);
  Future<void> deleteSupplier(int id);
  Future<void> seedSampleSuppliers();
}
