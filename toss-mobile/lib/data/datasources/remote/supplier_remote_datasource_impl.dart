import 'package:cloud_firestore/cloud_firestore.dart';
import '../../../domain/entities/supplier_entity.dart';
import '../interfaces/supplier_datasource.dart';

class SupplierRemoteDatasourceImpl implements SupplierDatasource {
  final FirebaseFirestore _firestore;
  final String _collection = 'suppliers';

  SupplierRemoteDatasourceImpl(this._firestore);

  @override
  Future<int> createSupplier(SupplierEntity supplier) async {
    final docRef = await _firestore.collection(_collection).add(_supplierToMap(supplier));
    return int.parse(docRef.id);
  }

  @override
  Future<List<SupplierEntity>> getAllSuppliers() async {
    final snapshot = await _firestore
        .collection(_collection)
        .orderBy('name')
        .get();
    
    return snapshot.docs.map((doc) => _mapToSupplier(doc.data(), doc.id)).toList();
  }

  @override
  Future<SupplierEntity?> getSupplierById(int id) async {
    final doc = await _firestore.collection(_collection).doc(id.toString()).get();
    if (doc.exists) {
      return _mapToSupplier(doc.data()!, doc.id);
    }
    return null;
  }

  @override
  Future<List<SupplierEntity>> searchSuppliers(String query) async {
    final snapshot = await _firestore
        .collection(_collection)
        .where('name', isGreaterThanOrEqualTo: query)
        .where('name', isLessThan: query + '\uf8ff')
        .get();
    
    return snapshot.docs.map((doc) => _mapToSupplier(doc.data(), doc.id)).toList();
  }

  @override
  Future<List<SupplierEntity>> getActiveSuppliers() async {
    final snapshot = await _firestore
        .collection(_collection)
        .where('isActive', isEqualTo: true)
        .orderBy('name')
        .get();
    
    return snapshot.docs.map((doc) => _mapToSupplier(doc.data(), doc.id)).toList();
  }

  @override
  Future<void> updateSupplier(SupplierEntity supplier) async {
    await _firestore.collection(_collection).doc(supplier.id.toString()).update(_supplierToMap(supplier));
  }

  @override
  Future<void> deleteSupplier(int id) async {
    await _firestore.collection(_collection).doc(id.toString()).delete();
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
    ];

    for (final supplier in sampleSuppliers) {
      await createSupplier(supplier);
    }
  }

  Map<String, dynamic> _supplierToMap(SupplierEntity supplier) {
    return {
      'name': supplier.name,
      'companyName': supplier.companyName,
      'contactPerson': supplier.contactPerson,
      'phoneNumber': supplier.phoneNumber,
      'email': supplier.email,
      'address': supplier.address,
      'city': supplier.city,
      'country': supplier.country,
      'taxNumber': supplier.taxNumber,
      'paymentTerms': supplier.paymentTerms,
      'isActive': supplier.isActive,
      'rating': supplier.rating,
      'notes': supplier.notes,
      'createdById': supplier.createdById,
      'createdAt': supplier.createdAt,
      'updatedAt': supplier.updatedAt,
    };
  }

  SupplierEntity _mapToSupplier(Map<String, dynamic> map, String id) {
    return SupplierEntity(
      id: int.parse(id),
      name: map['name'] as String,
      companyName: map['companyName'] as String?,
      contactPerson: map['contactPerson'] as String?,
      phoneNumber: map['phoneNumber'] as String?,
      email: map['email'] as String?,
      address: map['address'] as String?,
      city: map['city'] as String?,
      country: map['country'] as String?,
      taxNumber: map['taxNumber'] as String?,
      paymentTerms: map['paymentTerms'] as Map<String, dynamic>?,
      isActive: map['isActive'] as bool,
      rating: map['rating'] as double?,
      notes: map['notes'] as String?,
      createdById: map['createdById'] as String?,
      createdAt: map['createdAt'] as String?,
      updatedAt: map['updatedAt'] as String?,
    );
  }
}
