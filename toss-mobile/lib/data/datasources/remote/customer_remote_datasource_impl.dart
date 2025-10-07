import 'package:cloud_firestore/cloud_firestore.dart';

import '../../../domain/entities/customer_entity.dart';
import '../../datasources/interfaces/customer_datasource.dart';

class CustomerRemoteDatasourceImpl implements CustomerDatasource {
  final FirebaseFirestore _firestore;

  CustomerRemoteDatasourceImpl(this._firestore);

  @override
  Future<List<CustomerEntity>> getAllCustomers() async {
    try {
      final QuerySnapshot snapshot = await _firestore
          .collection('customers')
          .orderBy('name')
          .get();

      return snapshot.docs
          .map((doc) => _mapToEntity(doc.id, doc.data() as Map<String, dynamic>))
          .toList();
    } catch (e) {
      throw Exception('Failed to get customers from remote: $e');
    }
  }

  @override
  Future<CustomerEntity?> getCustomerById(String id) async {
    try {
      final DocumentSnapshot doc = await _firestore
          .collection('customers')
          .doc(id)
          .get();

      if (doc.exists) {
        return _mapToEntity(doc.id, doc.data() as Map<String, dynamic>);
      }
      return null;
    } catch (e) {
      throw Exception('Failed to get customer by id from remote: $e');
    }
  }

  @override
  Future<CustomerEntity?> getCustomerByPhone(String phone) async {
    try {
      final QuerySnapshot snapshot = await _firestore
          .collection('customers')
          .where('phone', isEqualTo: phone)
          .limit(1)
          .get();

      if (snapshot.docs.isNotEmpty) {
        final doc = snapshot.docs.first;
        return _mapToEntity(doc.id, doc.data() as Map<String, dynamic>);
      }
      return null;
    } catch (e) {
      throw Exception('Failed to get customer by phone from remote: $e');
    }
  }

  @override
  Future<CustomerEntity?> getCustomerByMembershipNumber(String membershipNumber) async {
    try {
      final QuerySnapshot snapshot = await _firestore
          .collection('customers')
          .where('membershipNumber', isEqualTo: membershipNumber)
          .limit(1)
          .get();

      if (snapshot.docs.isNotEmpty) {
        final doc = snapshot.docs.first;
        return _mapToEntity(doc.id, doc.data() as Map<String, dynamic>);
      }
      return null;
    } catch (e) {
      throw Exception('Failed to get customer by membership number from remote: $e');
    }
  }

  @override
  Future<List<CustomerEntity>> searchCustomers(String query) async {
    try {
      // Firestore doesn't support full-text search, so we'll search by name
      final QuerySnapshot snapshot = await _firestore
          .collection('customers')
          .where('name', isGreaterThanOrEqualTo: query)
          .where('name', isLessThan: query + '\uf8ff')
          .get();

      return snapshot.docs
          .map((doc) => _mapToEntity(doc.id, doc.data() as Map<String, dynamic>))
          .toList();
    } catch (e) {
      throw Exception('Failed to search customers from remote: $e');
    }
  }

  @override
  Future<List<CustomerEntity>> getCustomersByTier(CustomerTier tier) async {
    try {
      final QuerySnapshot snapshot = await _firestore
          .collection('customers')
          .where('tier', isEqualTo: tier.name)
          .orderBy('name')
          .get();

      return snapshot.docs
          .map((doc) => _mapToEntity(doc.id, doc.data() as Map<String, dynamic>))
          .toList();
    } catch (e) {
      throw Exception('Failed to get customers by tier from remote: $e');
    }
  }

  @override
  Future<List<CustomerEntity>> getCustomersByLoyaltyTier(LoyaltyTier loyaltyTier) async {
    try {
      final QuerySnapshot snapshot = await _firestore
          .collection('customers')
          .where('loyaltyTier', isEqualTo: loyaltyTier.name)
          .orderBy('name')
          .get();

      return snapshot.docs
          .map((doc) => _mapToEntity(doc.id, doc.data() as Map<String, dynamic>))
          .toList();
    } catch (e) {
      throw Exception('Failed to get customers by loyalty tier from remote: $e');
    }
  }

  @override
  Future<List<CustomerEntity>> getCustomersByTag(String tag) async {
    try {
      final QuerySnapshot snapshot = await _firestore
          .collection('customers')
          .where('tags', arrayContains: tag)
          .orderBy('name')
          .get();

      return snapshot.docs
          .map((doc) => _mapToEntity(doc.id, doc.data() as Map<String, dynamic>))
          .toList();
    } catch (e) {
      throw Exception('Failed to get customers by tag from remote: $e');
    }
  }

  @override
  Future<List<CustomerEntity>> getInactiveCustomers({int daysThreshold = 90}) async {
    try {
      final cutoffDate = DateTime.now().subtract(Duration(days: daysThreshold));
      final QuerySnapshot snapshot = await _firestore
          .collection('customers')
          .where('lastVisit', isLessThan: Timestamp.fromDate(cutoffDate))
          .orderBy('lastVisit')
          .get();

      return snapshot.docs
          .map((doc) => _mapToEntity(doc.id, doc.data() as Map<String, dynamic>))
          .toList();
    } catch (e) {
      throw Exception('Failed to get inactive customers from remote: $e');
    }
  }

  @override
  Future<List<CustomerEntity>> getBirthdayCustomers() async {
    try {
      final now = DateTime.now();
      final currentMonth = now.month;
      final currentDay = now.day;
      
      // Get all customers and filter by birthday
      final QuerySnapshot snapshot = await _firestore
          .collection('customers')
          .where('dateOfBirth', isNotEqualTo: null)
          .get();

      return snapshot.docs
          .map((doc) => _mapToEntity(doc.id, doc.data() as Map<String, dynamic>))
          .where((customer) {
            if (customer.dateOfBirth == null) return false;
            return customer.dateOfBirth!.month == currentMonth && 
                   customer.dateOfBirth!.day == currentDay;
          })
          .toList();
    } catch (e) {
      throw Exception('Failed to get birthday customers from remote: $e');
    }
  }

  @override
  Future<List<CustomerEntity>> getAnniversaryCustomers() async {
    try {
      final now = DateTime.now();
      final currentMonth = now.month;
      final currentDay = now.day;
      
      // Get all customers and filter by anniversary
      final QuerySnapshot snapshot = await _firestore
          .collection('customers')
          .where('anniversaryDate', isNotEqualTo: null)
          .get();

      return snapshot.docs
          .map((doc) => _mapToEntity(doc.id, doc.data() as Map<String, dynamic>))
          .where((customer) {
            if (customer.anniversaryDate == null) return false;
            return customer.anniversaryDate!.month == currentMonth && 
                   customer.anniversaryDate!.day == currentDay;
          })
          .toList();
    } catch (e) {
      throw Exception('Failed to get anniversary customers from remote: $e');
    }
  }

  @override
  Future<String> createCustomer(CustomerEntity customer) async {
    try {
      await _firestore
          .collection('customers')
          .doc(customer.id)
          .set(_entityToMap(customer));
      return customer.id;
    } catch (e) {
      throw Exception('Failed to create customer in remote: $e');
    }
  }

  @override
  Future<void> updateCustomer(CustomerEntity customer) async {
    try {
      await _firestore
          .collection('customers')
          .doc(customer.id)
          .update(_entityToMap(customer));
    } catch (e) {
      throw Exception('Failed to update customer in remote: $e');
    }
  }

  @override
  Future<void> deleteCustomer(String id) async {
    try {
      await _firestore
          .collection('customers')
          .doc(id)
          .delete();
    } catch (e) {
      throw Exception('Failed to delete customer in remote: $e');
    }
  }

  @override
  Future<void> updateCustomerPoints(String customerId, int points) async {
    try {
      await _firestore
          .collection('customers')
          .doc(customerId)
          .update({
        'pointsBalance': points,
        'updatedAt': FieldValue.serverTimestamp(),
      });
    } catch (e) {
      throw Exception('Failed to update customer points in remote: $e');
    }
  }

  @override
  Future<void> updateCustomerLoyaltyPoints(String customerId, int loyaltyPoints) async {
    try {
      await _firestore
          .collection('customers')
          .doc(customerId)
          .update({
        'loyaltyPoints': loyaltyPoints,
        'updatedAt': FieldValue.serverTimestamp(),
      });
    } catch (e) {
      throw Exception('Failed to update customer loyalty points in remote: $e');
    }
  }

  @override
  Future<void> updateCustomerVisit(String customerId, double amount) async {
    try {
      final customer = await getCustomerById(customerId);
      if (customer != null) {
        final updatedCustomer = customer.copyWith(
          visitCount: customer.visitCount + 1,
          lastVisit: DateTime.now(),
          totalSpent: customer.totalSpent + amount,
        );
        await updateCustomer(updatedCustomer);
      }
    } catch (e) {
      throw Exception('Failed to update customer visit in remote: $e');
    }
  }

  @override
  Future<void> seedSampleCustomers() async {
    // Remote seeding is typically not done in production
    // This would be handled by admin tools or data migration scripts
    throw UnimplementedError('Remote seeding not implemented');
  }

  CustomerEntity _mapToEntity(String id, Map<String, dynamic> data) {
    return CustomerEntity(
      id: id,
      name: data['name'] as String?,
      phone: data['phone'] as String?,
      alternatePhone: data['alternatePhone'] as String?,
      membershipNumber: data['membershipNumber'] as String?,
      qrCode: data['qrCode'] as String?,
      nfcId: data['nfcId'] as String?,
      biometricId: data['biometricId'] as String?,
      primaryIdType: CustomerIdType.values.firstWhere(
        (e) => e.name == data['primaryIdType'],
        orElse: () => CustomerIdType.phone,
      ),
      tier: CustomerTier.values.firstWhere(
        (e) => e.name == data['tier'],
        orElse: () => CustomerTier.bronze,
      ),
      loyaltyTier: LoyaltyTier.values.firstWhere(
        (e) => e.name == data['loyaltyTier'],
        orElse: () => LoyaltyTier.none,
      ),
      pointsBalance: data['pointsBalance'] as int? ?? 0,
      loyaltyPoints: data['loyaltyPoints'] as int? ?? 0,
      totalSpent: (data['totalSpent'] as num?)?.toDouble() ?? 0.0,
      visitCount: data['visitCount'] as int? ?? 0,
      lastVisit: data['lastVisit'] != null 
          ? (data['lastVisit'] as Timestamp).toDate() 
          : null,
      dateOfBirth: data['dateOfBirth'] != null 
          ? (data['dateOfBirth'] as Timestamp).toDate() 
          : null,
      address: data['address'] as String?,
      city: data['city'] as String?,
      country: data['country'] as String?,
      gender: data['gender'] as String?,
      preferredCommunication: PreferredCommunication.values.firstWhere(
        (e) => e.name == data['preferredCommunication'],
        orElse: () => PreferredCommunication.none,
      ),
      preferences: data['preferences'] != null 
          ? Map<String, dynamic>.from(data['preferences']) 
          : null,
      tags: data['tags'] != null 
          ? List<String>.from(data['tags']) 
          : null,
      isActive: data['isActive'] as bool? ?? true,
      notes: data['notes'] as String?,
      anniversaryDate: data['anniversaryDate'] != null 
          ? (data['anniversaryDate'] as Timestamp).toDate() 
          : null,
      referredBy: data['referredBy'] as String?,
      createdAt: data['createdAt'] != null 
          ? (data['createdAt'] as Timestamp).toDate().toIso8601String() 
          : null,
      updatedAt: data['updatedAt'] != null 
          ? (data['updatedAt'] as Timestamp).toDate().toIso8601String() 
          : null,
    );
  }

  Map<String, dynamic> _entityToMap(CustomerEntity customer) {
    return {
      'name': customer.name,
      'phone': customer.phone,
      'alternatePhone': customer.alternatePhone,
      'membershipNumber': customer.membershipNumber,
      'qrCode': customer.qrCode,
      'nfcId': customer.nfcId,
      'biometricId': customer.biometricId,
      'primaryIdType': customer.primaryIdType.name,
      'tier': customer.tier.name,
      'loyaltyTier': customer.loyaltyTier.name,
      'pointsBalance': customer.pointsBalance,
      'loyaltyPoints': customer.loyaltyPoints,
      'totalSpent': customer.totalSpent,
      'visitCount': customer.visitCount,
      'lastVisit': customer.lastVisit != null 
          ? Timestamp.fromDate(customer.lastVisit!) 
          : null,
      'dateOfBirth': customer.dateOfBirth != null 
          ? Timestamp.fromDate(customer.dateOfBirth!) 
          : null,
      'address': customer.address,
      'city': customer.city,
      'country': customer.country,
      'gender': customer.gender,
      'preferredCommunication': customer.preferredCommunication.name,
      'preferences': customer.preferences,
      'tags': customer.tags,
      'isActive': customer.isActive,
      'notes': customer.notes,
      'anniversaryDate': customer.anniversaryDate != null 
          ? Timestamp.fromDate(customer.anniversaryDate!) 
          : null,
      'referredBy': customer.referredBy,
      'createdAt': customer.createdAt != null 
          ? Timestamp.fromDate(DateTime.parse(customer.createdAt!)) 
          : FieldValue.serverTimestamp(),
      'updatedAt': FieldValue.serverTimestamp(),
    };
  }
}
