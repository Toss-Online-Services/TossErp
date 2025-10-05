import 'dart:convert';
import 'package:sqflite/sqflite.dart';

import '../../../app/database/app_database.dart';
import '../../../domain/entities/customer_entity.dart';
import '../../datasources/interfaces/customer_datasource.dart';

class CustomerLocalDatasourceImpl implements CustomerDatasource {
  final AppDatabase _appDatabase;

  CustomerLocalDatasourceImpl(this._appDatabase);

  @override
  Future<List<CustomerEntity>> getAllCustomers() async {
    try {
      final List<Map<String, dynamic>> maps = await _appDatabase.database.query(
        AppDatabaseConfig.customerTableName,
        orderBy: 'name ASC',
      );

      return maps.map((map) => _mapToEntity(map)).toList();
    } catch (e) {
      throw Exception('Failed to get customers: $e');
    }
  }

  @override
  Future<CustomerEntity?> getCustomerById(String id) async {
    try {
      final List<Map<String, dynamic>> maps = await _appDatabase.database.query(
        AppDatabaseConfig.customerTableName,
        where: 'id = ?',
        whereArgs: [id],
      );

      if (maps.isNotEmpty) {
        return _mapToEntity(maps.first);
      }
      return null;
    } catch (e) {
      throw Exception('Failed to get customer by id: $e');
    }
  }

  @override
  Future<CustomerEntity?> getCustomerByPhone(String phone) async {
    try {
      final List<Map<String, dynamic>> maps = await _appDatabase.database.query(
        AppDatabaseConfig.customerTableName,
        where: 'phone = ?',
        whereArgs: [phone],
      );

      if (maps.isNotEmpty) {
        return _mapToEntity(maps.first);
      }
      return null;
    } catch (e) {
      throw Exception('Failed to get customer by phone: $e');
    }
  }

  @override
  Future<CustomerEntity?> getCustomerByMembershipNumber(String membershipNumber) async {
    try {
      final List<Map<String, dynamic>> maps = await _appDatabase.database.query(
        AppDatabaseConfig.customerTableName,
        where: 'membershipNumber = ?',
        whereArgs: [membershipNumber],
      );

      if (maps.isNotEmpty) {
        return _mapToEntity(maps.first);
      }
      return null;
    } catch (e) {
      throw Exception('Failed to get customer by membership number: $e');
    }
  }

  @override
  Future<List<CustomerEntity>> searchCustomers(String query) async {
    try {
      final List<Map<String, dynamic>> maps = await _appDatabase.database.query(
        AppDatabaseConfig.customerTableName,
        where: 'name LIKE ? OR phone LIKE ? OR membershipNumber LIKE ?',
        whereArgs: ['%$query%', '%$query%', '%$query%'],
        orderBy: 'name ASC',
      );

      return maps.map((map) => _mapToEntity(map)).toList();
    } catch (e) {
      throw Exception('Failed to search customers: $e');
    }
  }

  @override
  Future<List<CustomerEntity>> getCustomersByTier(CustomerTier tier) async {
    try {
      final List<Map<String, dynamic>> maps = await _appDatabase.database.query(
        AppDatabaseConfig.customerTableName,
        where: 'tier = ?',
        whereArgs: [tier.name],
        orderBy: 'name ASC',
      );

      return maps.map((map) => _mapToEntity(map)).toList();
    } catch (e) {
      throw Exception('Failed to get customers by tier: $e');
    }
  }

  @override
  Future<List<CustomerEntity>> getCustomersByLoyaltyTier(LoyaltyTier loyaltyTier) async {
    try {
      final List<Map<String, dynamic>> maps = await _appDatabase.database.query(
        AppDatabaseConfig.customerTableName,
        where: 'loyaltyTier = ?',
        whereArgs: [loyaltyTier.name],
        orderBy: 'name ASC',
      );

      return maps.map((map) => _mapToEntity(map)).toList();
    } catch (e) {
      throw Exception('Failed to get customers by loyalty tier: $e');
    }
  }

  @override
  Future<List<CustomerEntity>> getCustomersByTag(String tag) async {
    try {
      final List<Map<String, dynamic>> maps = await _appDatabase.database.query(
        AppDatabaseConfig.customerTableName,
        where: 'tags LIKE ?',
        whereArgs: ['%$tag%'],
        orderBy: 'name ASC',
      );

      return maps.map((map) => _mapToEntity(map)).toList();
    } catch (e) {
      throw Exception('Failed to get customers by tag: $e');
    }
  }

  @override
  Future<List<CustomerEntity>> getInactiveCustomers({int daysThreshold = 90}) async {
    try {
      final cutoffDate = DateTime.now().subtract(Duration(days: daysThreshold));
      final List<Map<String, dynamic>> maps = await _appDatabase.database.query(
        AppDatabaseConfig.customerTableName,
        where: 'lastVisit < ? OR lastVisit IS NULL',
        whereArgs: [cutoffDate.toIso8601String()],
        orderBy: 'lastVisit ASC',
      );

      return maps.map((map) => _mapToEntity(map)).toList();
    } catch (e) {
      throw Exception('Failed to get inactive customers: $e');
    }
  }

  @override
  Future<List<CustomerEntity>> getBirthdayCustomers() async {
    try {
      final now = DateTime.now();
      final currentMonth = now.month;
      final currentDay = now.day;
      
      // Get all customers and filter by birthday
      final List<Map<String, dynamic>> maps = await _appDatabase.database.query(
        AppDatabaseConfig.customerTableName,
        where: 'dateOfBirth IS NOT NULL',
      );

      return maps.map((map) => _mapToEntity(map)).where((customer) {
        if (customer.dateOfBirth == null) return false;
        return customer.dateOfBirth!.month == currentMonth && 
               customer.dateOfBirth!.day == currentDay;
      }).toList();
    } catch (e) {
      throw Exception('Failed to get birthday customers: $e');
    }
  }

  @override
  Future<List<CustomerEntity>> getAnniversaryCustomers() async {
    try {
      final now = DateTime.now();
      final currentMonth = now.month;
      final currentDay = now.day;
      
      // Get all customers and filter by anniversary
      final List<Map<String, dynamic>> maps = await _appDatabase.database.query(
        AppDatabaseConfig.customerTableName,
        where: 'anniversaryDate IS NOT NULL',
      );

      return maps.map((map) => _mapToEntity(map)).where((customer) {
        if (customer.anniversaryDate == null) return false;
        return customer.anniversaryDate!.month == currentMonth && 
               customer.anniversaryDate!.day == currentDay;
      }).toList();
    } catch (e) {
      throw Exception('Failed to get anniversary customers: $e');
    }
  }

  @override
  Future<String> createCustomer(CustomerEntity customer) async {
    try {
      await _appDatabase.database.insert(
        AppDatabaseConfig.customerTableName,
        _entityToMap(customer),
        conflictAlgorithm: ConflictAlgorithm.replace,
      );
      return customer.id;
    } catch (e) {
      throw Exception('Failed to create customer: $e');
    }
  }

  @override
  Future<void> updateCustomer(CustomerEntity customer) async {
    try {
      await _appDatabase.database.update(
        AppDatabaseConfig.customerTableName,
        _entityToMap(customer),
        where: 'id = ?',
        whereArgs: [customer.id],
      );
    } catch (e) {
      throw Exception('Failed to update customer: $e');
    }
  }

  @override
  Future<void> deleteCustomer(String id) async {
    try {
      await _appDatabase.database.delete(
        AppDatabaseConfig.customerTableName,
        where: 'id = ?',
        whereArgs: [id],
      );
    } catch (e) {
      throw Exception('Failed to delete customer: $e');
    }
  }

  @override
  Future<void> updateCustomerPoints(String customerId, int points) async {
    try {
      await _appDatabase.database.update(
        AppDatabaseConfig.customerTableName,
        {'pointsBalance': points, 'updatedAt': DateTime.now().toIso8601String()},
        where: 'id = ?',
        whereArgs: [customerId],
      );
    } catch (e) {
      throw Exception('Failed to update customer points: $e');
    }
  }

  @override
  Future<void> updateCustomerLoyaltyPoints(String customerId, int loyaltyPoints) async {
    try {
      await _appDatabase.database.update(
        AppDatabaseConfig.customerTableName,
        {'loyaltyPoints': loyaltyPoints, 'updatedAt': DateTime.now().toIso8601String()},
        where: 'id = ?',
        whereArgs: [customerId],
      );
    } catch (e) {
      throw Exception('Failed to update customer loyalty points: $e');
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
      throw Exception('Failed to update customer visit: $e');
    }
  }

  @override
  Future<void> seedSampleCustomers() async {
    try {
      final sampleCustomers = [
        CustomerEntity(
          id: 'customer_001',
          name: 'John Smith',
          phone: '+1234567890',
          primaryIdType: CustomerIdType.phone,
          tier: CustomerTier.gold,
          loyaltyTier: LoyaltyTier.gold,
          pointsBalance: 1500,
          loyaltyPoints: 250,
          totalSpent: 2500.0,
          visitCount: 15,
          lastVisit: DateTime.now().subtract(const Duration(days: 2)),
          dateOfBirth: DateTime(1985, 6, 15),
          address: '123 Main St',
          city: 'New York',
          country: 'USA',
          gender: 'Male',
          preferredCommunication: PreferredCommunication.sms,
          tags: ['VIP', 'Regular'],
          isActive: true,
          notes: 'Prefers morning visits',
        ),
        CustomerEntity(
          id: 'customer_002',
          name: 'Sarah Johnson',
          phone: '+1987654321',
          membershipNumber: 'MEM001',
          primaryIdType: CustomerIdType.membershipCard,
          tier: CustomerTier.silver,
          loyaltyTier: LoyaltyTier.silver,
          pointsBalance: 800,
          loyaltyPoints: 120,
          totalSpent: 1200.0,
          visitCount: 8,
          lastVisit: DateTime.now().subtract(const Duration(days: 5)),
          dateOfBirth: DateTime(1990, 3, 22),
          address: '456 Oak Ave',
          city: 'Los Angeles',
          country: 'USA',
          gender: 'Female',
          preferredCommunication: PreferredCommunication.whatsapp,
          tags: ['Fashion', 'Beauty'],
          isActive: true,
          notes: 'Loves seasonal promotions',
        ),
        CustomerEntity(
          id: 'customer_003',
          name: 'Mike Wilson',
          phone: '+1555666777',
          primaryIdType: CustomerIdType.phone,
          tier: CustomerTier.bronze,
          loyaltyTier: LoyaltyTier.bronze,
          pointsBalance: 300,
          loyaltyPoints: 50,
          totalSpent: 600.0,
          visitCount: 4,
          lastVisit: DateTime.now().subtract(const Duration(days: 10)),
          address: '789 Pine St',
          city: 'Chicago',
          country: 'USA',
          gender: 'Male',
          preferredCommunication: PreferredCommunication.sms,
          tags: ['Electronics'],
          isActive: true,
          notes: 'Price sensitive customer',
        ),
      ];

      for (final customer in sampleCustomers) {
        await createCustomer(customer);
      }
    } catch (e) {
      throw Exception('Failed to seed sample customers: $e');
    }
  }

  CustomerEntity _mapToEntity(Map<String, dynamic> map) {
    return CustomerEntity(
      id: map['id'] as String,
      name: map['name'] as String?,
      phone: map['phone'] as String?,
      alternatePhone: map['alternatePhone'] as String?,
      membershipNumber: map['membershipNumber'] as String?,
      qrCode: map['qrCode'] as String?,
      nfcId: map['nfcId'] as String?,
      biometricId: map['biometricId'] as String?,
      primaryIdType: CustomerIdType.values.firstWhere(
        (e) => e.name == map['primaryIdType'],
        orElse: () => CustomerIdType.phone,
      ),
      tier: CustomerTier.values.firstWhere(
        (e) => e.name == map['tier'],
        orElse: () => CustomerTier.bronze,
      ),
      loyaltyTier: LoyaltyTier.values.firstWhere(
        (e) => e.name == map['loyaltyTier'],
        orElse: () => LoyaltyTier.none,
      ),
      pointsBalance: map['pointsBalance'] as int? ?? 0,
      loyaltyPoints: map['loyaltyPoints'] as int? ?? 0,
      totalSpent: (map['totalSpent'] as num?)?.toDouble() ?? 0.0,
      visitCount: map['visitCount'] as int? ?? 0,
      lastVisit: map['lastVisit'] != null ? DateTime.parse(map['lastVisit']) : null,
      dateOfBirth: map['dateOfBirth'] != null ? DateTime.parse(map['dateOfBirth']) : null,
      address: map['address'] as String?,
      city: map['city'] as String?,
      country: map['country'] as String?,
      gender: map['gender'] as String?,
      preferredCommunication: PreferredCommunication.values.firstWhere(
        (e) => e.name == map['preferredCommunication'],
        orElse: () => PreferredCommunication.none,
      ),
      preferences: map['preferences'] != null 
          ? Map<String, dynamic>.from(jsonDecode(map['preferences'])) 
          : null,
      tags: map['tags'] != null 
          ? List<String>.from(jsonDecode(map['tags'])) 
          : null,
      isActive: (map['isActive'] as int?) == 1,
      notes: map['notes'] as String?,
      anniversaryDate: map['anniversaryDate'] != null 
          ? DateTime.parse(map['anniversaryDate']) 
          : null,
      referredBy: map['referredBy'] as String?,
      createdAt: map['createdAt'] as String?,
      updatedAt: map['updatedAt'] as String?,
    );
  }

  Map<String, dynamic> _entityToMap(CustomerEntity customer) {
    return {
      'id': customer.id,
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
      'lastVisit': customer.lastVisit?.toIso8601String(),
      'dateOfBirth': customer.dateOfBirth?.toIso8601String(),
      'address': customer.address,
      'city': customer.city,
      'country': customer.country,
      'gender': customer.gender,
      'preferredCommunication': customer.preferredCommunication.name,
      'preferences': customer.preferences != null 
          ? jsonEncode(customer.preferences) 
          : null,
      'tags': customer.tags != null 
          ? jsonEncode(customer.tags) 
          : null,
      'isActive': customer.isActive ? 1 : 0,
      'notes': customer.notes,
      'anniversaryDate': customer.anniversaryDate?.toIso8601String(),
      'referredBy': customer.referredBy,
      'createdAt': customer.createdAt,
      'updatedAt': customer.updatedAt,
    };
  }
}