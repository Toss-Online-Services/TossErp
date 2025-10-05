import 'package:sqflite/sqflite.dart';
import '../../../app/database/app_database.dart';
import '../../../domain/entities/supplier_entity.dart';
import '../interfaces/supplier_datasource.dart';

class SupplierLocalDatasourceImpl implements SupplierDatasource {
  final AppDatabase _appDatabase;

  SupplierLocalDatasourceImpl(this._appDatabase);

  @override
  Future<int> createSupplier(SupplierEntity supplier) async {
    final id = await _appDatabase.database.insert(
      AppDatabaseConfig.supplierTableName,
      _supplierToMap(supplier),
      conflictAlgorithm: ConflictAlgorithm.replace,
    );
    return id;
  }

  @override
  Future<List<SupplierEntity>> getAllSuppliers() async {
    final res = await _appDatabase.database.query(
      AppDatabaseConfig.supplierTableName,
      orderBy: 'name ASC',
    );
    return res.map((row) => _mapToSupplier(row)).toList();
  }

  @override
  Future<SupplierEntity?> getSupplierById(int id) async {
    final res = await _appDatabase.database.query(
      AppDatabaseConfig.supplierTableName,
      where: 'id = ?',
      whereArgs: [id],
    );
    if (res.isNotEmpty) {
      return _mapToSupplier(res.first);
    }
    return null;
  }

  @override
  Future<List<SupplierEntity>> searchSuppliers(String query) async {
    final res = await _appDatabase.database.rawQuery('''
      SELECT * FROM ${AppDatabaseConfig.supplierTableName}
      WHERE name LIKE ? OR companyName LIKE ? OR contactPerson LIKE ? OR email LIKE ?
      ORDER BY name ASC
    ''', ['%$query%', '%$query%', '%$query%', '%$query%']);
    
    return res.map((row) => _mapToSupplier(row)).toList();
  }

  @override
  Future<List<SupplierEntity>> getActiveSuppliers() async {
    final res = await _appDatabase.database.query(
      AppDatabaseConfig.supplierTableName,
      where: 'isActive = ?',
      whereArgs: [1],
      orderBy: 'name ASC',
    );
    return res.map((row) => _mapToSupplier(row)).toList();
  }

  @override
  Future<void> updateSupplier(SupplierEntity supplier) async {
    await _appDatabase.database.update(
      AppDatabaseConfig.supplierTableName,
      _supplierToMap(supplier),
      where: 'id = ?',
      whereArgs: [supplier.id],
    );
  }

  @override
  Future<void> deleteSupplier(int id) async {
    await _appDatabase.database.delete(
      AppDatabaseConfig.supplierTableName,
      where: 'id = ?',
      whereArgs: [id],
    );
  }

  @override
  Future<void> seedSampleSuppliers() async {
    final sampleSuppliers = [
      SupplierEntity(
        id: DateTime.now().millisecondsSinceEpoch,
        name: 'ABC Electronics Ltd',
        companyName: 'ABC Electronics Ltd',
        contactPerson: 'John Smith',
        phoneNumber: '+1-555-0123',
        email: 'john@abcelectronics.com',
        address: '123 Tech Street',
        city: 'San Francisco',
        country: 'USA',
        taxNumber: 'TAX123456',
        paymentTerms: {'days': 30, 'method': 'net'},
        isActive: true,
        rating: 4.5,
        notes: 'Reliable supplier for electronic components',
        createdById: 'user1',
        createdAt: DateTime.now().subtract(const Duration(days: 30)).toIso8601String(),
        updatedAt: DateTime.now().subtract(const Duration(days: 30)).toIso8601String(),
      ),
      SupplierEntity(
        id: DateTime.now().millisecondsSinceEpoch + 1,
        name: 'Global Parts Inc',
        companyName: 'Global Parts Inc',
        contactPerson: 'Sarah Johnson',
        phoneNumber: '+1-555-0456',
        email: 'sarah@globalparts.com',
        address: '456 Industrial Ave',
        city: 'Chicago',
        country: 'USA',
        taxNumber: 'TAX789012',
        paymentTerms: {'days': 15, 'method': 'net'},
        isActive: true,
        rating: 4.2,
        notes: 'Fast delivery, good quality parts',
        createdById: 'user1',
        createdAt: DateTime.now().subtract(const Duration(days: 25)).toIso8601String(),
        updatedAt: DateTime.now().subtract(const Duration(days: 25)).toIso8601String(),
      ),
      SupplierEntity(
        id: DateTime.now().millisecondsSinceEpoch + 2,
        name: 'Tech Solutions Co',
        companyName: 'Tech Solutions Co',
        contactPerson: 'Mike Wilson',
        phoneNumber: '+1-555-0789',
        email: 'mike@techsolutions.com',
        address: '789 Business Blvd',
        city: 'New York',
        country: 'USA',
        taxNumber: 'TAX345678',
        paymentTerms: {'days': 45, 'method': 'net'},
        isActive: true,
        rating: 3.8,
        notes: 'Good prices, sometimes delayed delivery',
        createdById: 'user1',
        createdAt: DateTime.now().subtract(const Duration(days: 20)).toIso8601String(),
        updatedAt: DateTime.now().subtract(const Duration(days: 20)).toIso8601String(),
      ),
    ];

    for (final supplier in sampleSuppliers) {
      await createSupplier(supplier);
    }
  }

  Map<String, dynamic> _supplierToMap(SupplierEntity supplier) {
    return {
      'id': supplier.id,
      'name': supplier.name,
      'companyName': supplier.companyName,
      'contactPerson': supplier.contactPerson,
      'phoneNumber': supplier.phoneNumber,
      'email': supplier.email,
      'address': supplier.address,
      'city': supplier.city,
      'country': supplier.country,
      'taxNumber': supplier.taxNumber,
      'paymentTerms': supplier.paymentTerms != null ? 
        '${supplier.paymentTerms!['days']}_${supplier.paymentTerms!['method']}' : null,
      'isActive': supplier.isActive ? 1 : 0,
      'rating': supplier.rating,
      'notes': supplier.notes,
      'createdById': supplier.createdById,
      'createdAt': supplier.createdAt,
      'updatedAt': supplier.updatedAt,
    };
  }

  SupplierEntity _mapToSupplier(Map<String, dynamic> map) {
    Map<String, dynamic>? paymentTerms;
    if (map['paymentTerms'] != null) {
      final parts = (map['paymentTerms'] as String).split('_');
      if (parts.length == 2) {
        paymentTerms = {
          'days': int.tryParse(parts[0]) ?? 30,
          'method': parts[1],
        };
      }
    }

    return SupplierEntity(
      id: map['id'] as int,
      name: map['name'] as String,
      companyName: map['companyName'] as String?,
      contactPerson: map['contactPerson'] as String?,
      phoneNumber: map['phoneNumber'] as String?,
      email: map['email'] as String?,
      address: map['address'] as String?,
      city: map['city'] as String?,
      country: map['country'] as String?,
      taxNumber: map['taxNumber'] as String?,
      paymentTerms: paymentTerms,
      isActive: (map['isActive'] as int) == 1,
      rating: map['rating'] as double?,
      notes: map['notes'] as String?,
      createdById: map['createdById'] as String?,
      createdAt: map['createdAt'] as String?,
      updatedAt: map['updatedAt'] as String?,
    );
  }
}
