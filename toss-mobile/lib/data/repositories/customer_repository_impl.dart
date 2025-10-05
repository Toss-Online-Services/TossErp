import 'dart:async';
import 'dart:convert';

import '../../domain/entities/customer_entity.dart';
import '../../domain/repositories/customer_repository.dart';
import '../datasources/local/customer_local_datasource_impl.dart';
import '../datasources/remote/customer_remote_datasource_impl.dart';
import '../repositories/connectivity_repository.dart';
import '../datasources/local/queued_action_local_datasource_impl.dart';
import '../models/queued_action_model.dart';
import '../../core/usecase/usecase.dart';
import '../../core/errors/errors.dart';

class CustomerRepositoryImpl implements CustomerRepository {
  final CustomerLocalDatasourceImpl _localDatasource;
  final CustomerRemoteDatasourceImpl _remoteDatasource;
  final ConnectivityRepository _connectivityRepository;
  final QueuedActionLocalDatasourceImpl _queuedActionLocalDatasource;

  CustomerRepositoryImpl(
    this._localDatasource,
    this._remoteDatasource,
    this._connectivityRepository,
    this._queuedActionLocalDatasource,
  );

  @override
  Future<List<CustomerEntity>> getAllCustomers() async {
    try {
      if (await _connectivityRepository.isConnected) {
        // Try to get from remote first, then sync to local
        final remoteCustomers = await _remoteDatasource.getAllCustomers();
        
        // Sync to local database
        for (final customer in remoteCustomers) {
          await _localDatasource.createCustomer(customer);
        }
        
        return remoteCustomers;
      } else {
        // Return from local database when offline
        return await _localDatasource.getAllCustomers();
      }
    } catch (e) {
      // Fallback to local database if remote fails
      return await _localDatasource.getAllCustomers();
    }
  }

  @override
  Future<CustomerEntity?> getCustomerById(String id) async {
    try {
      if (await _connectivityRepository.isConnected) {
        // Try remote first
        final remoteCustomer = await _remoteDatasource.getCustomerById(id);
        if (remoteCustomer != null) {
          // Sync to local
          await _localDatasource.createCustomer(remoteCustomer);
          return remoteCustomer;
        }
      }
      
      // Fallback to local
      return await _localDatasource.getCustomerById(id);
    } catch (e) {
      // Fallback to local database if remote fails
      return await _localDatasource.getCustomerById(id);
    }
  }

  @override
  Future<CustomerEntity?> getCustomerByPhone(String phone) async {
    try {
      if (await _connectivityRepository.isConnected) {
        // Try remote first
        final remoteCustomer = await _remoteDatasource.getCustomerByPhone(phone);
        if (remoteCustomer != null) {
          // Sync to local
          await _localDatasource.createCustomer(remoteCustomer);
          return remoteCustomer;
        }
      }
      
      // Fallback to local
      return await _localDatasource.getCustomerByPhone(phone);
    } catch (e) {
      // Fallback to local database if remote fails
      return await _localDatasource.getCustomerByPhone(phone);
    }
  }

  @override
  Future<CustomerEntity?> getCustomerByMembershipNumber(String membershipNumber) async {
    try {
      if (await _connectivityRepository.isConnected) {
        // Try remote first
        final remoteCustomer = await _remoteDatasource.getCustomerByMembershipNumber(membershipNumber);
        if (remoteCustomer != null) {
          // Sync to local
          await _localDatasource.createCustomer(remoteCustomer);
          return remoteCustomer;
        }
      }
      
      // Fallback to local
      return await _localDatasource.getCustomerByMembershipNumber(membershipNumber);
    } catch (e) {
      // Fallback to local database if remote fails
      return await _localDatasource.getCustomerByMembershipNumber(membershipNumber);
    }
  }

  @override
  Future<List<CustomerEntity>> searchCustomers(String query) async {
    try {
      if (await _connectivityRepository.isConnected) {
        // Try remote first
        final remoteCustomers = await _remoteDatasource.searchCustomers(query);
        
        // Sync to local
        for (final customer in remoteCustomers) {
          await _localDatasource.createCustomer(customer);
        }
        
        return remoteCustomers;
      } else {
        // Return from local database when offline
        return await _localDatasource.searchCustomers(query);
      }
    } catch (e) {
      // Fallback to local database if remote fails
      return await _localDatasource.searchCustomers(query);
    }
  }

  @override
  Future<List<CustomerEntity>> getCustomersByTier(CustomerTier tier) async {
    try {
      if (await _connectivityRepository.isConnected) {
        // Try remote first
        final remoteCustomers = await _remoteDatasource.getCustomersByTier(tier);
        
        // Sync to local
        for (final customer in remoteCustomers) {
          await _localDatasource.createCustomer(customer);
        }
        
        return remoteCustomers;
      } else {
        // Return from local database when offline
        return await _localDatasource.getCustomersByTier(tier);
      }
    } catch (e) {
      // Fallback to local database if remote fails
      return await _localDatasource.getCustomersByTier(tier);
    }
  }

  @override
  Future<List<CustomerEntity>> getCustomersByLoyaltyTier(LoyaltyTier loyaltyTier) async {
    try {
      if (await _connectivityRepository.isConnected) {
        // Try remote first
        final remoteCustomers = await _remoteDatasource.getCustomersByLoyaltyTier(loyaltyTier);
        
        // Sync to local
        for (final customer in remoteCustomers) {
          await _localDatasource.createCustomer(customer);
        }
        
        return remoteCustomers;
      } else {
        // Return from local database when offline
        return await _localDatasource.getCustomersByLoyaltyTier(loyaltyTier);
      }
    } catch (e) {
      // Fallback to local database if remote fails
      return await _localDatasource.getCustomersByLoyaltyTier(loyaltyTier);
    }
  }

  @override
  Future<List<CustomerEntity>> getCustomersByTag(String tag) async {
    try {
      if (await _connectivityRepository.isConnected) {
        // Try remote first
        final remoteCustomers = await _remoteDatasource.getCustomersByTag(tag);
        
        // Sync to local
        for (final customer in remoteCustomers) {
          await _localDatasource.createCustomer(customer);
        }
        
        return remoteCustomers;
      } else {
        // Return from local database when offline
        return await _localDatasource.getCustomersByTag(tag);
      }
    } catch (e) {
      // Fallback to local database if remote fails
      return await _localDatasource.getCustomersByTag(tag);
    }
  }

  @override
  Future<List<CustomerEntity>> getInactiveCustomers({int daysThreshold = 90}) async {
    try {
      if (await _connectivityRepository.isConnected) {
        // Try remote first
        final remoteCustomers = await _remoteDatasource.getInactiveCustomers(daysThreshold: daysThreshold);
        
        // Sync to local
        for (final customer in remoteCustomers) {
          await _localDatasource.createCustomer(customer);
        }
        
        return remoteCustomers;
      } else {
        // Return from local database when offline
        return await _localDatasource.getInactiveCustomers(daysThreshold: daysThreshold);
      }
    } catch (e) {
      // Fallback to local database if remote fails
      return await _localDatasource.getInactiveCustomers(daysThreshold: daysThreshold);
    }
  }

  @override
  Future<List<CustomerEntity>> getBirthdayCustomers() async {
    try {
      if (await _connectivityRepository.isConnected) {
        // Try remote first
        final remoteCustomers = await _remoteDatasource.getBirthdayCustomers();
        
        // Sync to local
        for (final customer in remoteCustomers) {
          await _localDatasource.createCustomer(customer);
        }
        
        return remoteCustomers;
      } else {
        // Return from local database when offline
        return await _localDatasource.getBirthdayCustomers();
      }
    } catch (e) {
      // Fallback to local database if remote fails
      return await _localDatasource.getBirthdayCustomers();
    }
  }

  @override
  Future<List<CustomerEntity>> getAnniversaryCustomers() async {
    try {
      if (await _connectivityRepository.isConnected) {
        // Try remote first
        final remoteCustomers = await _remoteDatasource.getAnniversaryCustomers();
        
        // Sync to local
        for (final customer in remoteCustomers) {
          await _localDatasource.createCustomer(customer);
        }
        
        return remoteCustomers;
      } else {
        // Return from local database when offline
        return await _localDatasource.getAnniversaryCustomers();
      }
    } catch (e) {
      // Fallback to local database if remote fails
      return await _localDatasource.getAnniversaryCustomers();
    }
  }

  @override
  Future<String> createCustomer(CustomerEntity customer) async {
    try {
      // Always save to local first
      final customerId = await _localDatasource.createCustomer(customer);
      
      if (await _connectivityRepository.isConnected) {
        try {
          // Try to sync to remote
          await _remoteDatasource.createCustomer(customer);
        } catch (e) {
          // If remote fails, queue the action for later
          await _queuedActionLocalDatasource.createQueuedAction(
            QueuedActionModel(
              id: DateTime.now().millisecondsSinceEpoch,
              repository: 'CustomerRepository',
              method: 'createCustomer',
              param: jsonEncode(customer.toMap()),
              isCritical: true,
              status: 'pending',
              retryCount: 0,
              createdAt: DateTime.now().toIso8601String(),
            ),
          );
        }
      } else {
        // Queue for later when online
        await _queuedActionLocalDatasource.createQueuedAction(
          QueuedActionModel(
            id: DateTime.now().millisecondsSinceEpoch,
            repository: 'CustomerRepository',
            method: 'createCustomer',
            param: jsonEncode(customer.toMap()),
            isCritical: true,
            status: 'pending',
            retryCount: 0,
            createdAt: DateTime.now().toIso8601String(),
          ),
        );
      }
      
      return customerId;
    } catch (e) {
      throw Exception('Failed to create customer: $e');
    }
  }

  @override
  Future<void> updateCustomer(CustomerEntity customer) async {
    try {
      // Always update local first
      await _localDatasource.updateCustomer(customer);
      
      if (await _connectivityRepository.isConnected) {
        try {
          // Try to sync to remote
          await _remoteDatasource.updateCustomer(customer);
        } catch (e) {
          // If remote fails, queue the action for later
          await _queuedActionLocalDatasource.createQueuedAction(
            QueuedActionModel(
              id: DateTime.now().millisecondsSinceEpoch,
              repository: 'CustomerRepository',
              method: 'updateCustomer',
              param: jsonEncode(customer.toMap()),
              isCritical: true,
              status: 'pending',
              retryCount: 0,
              createdAt: DateTime.now().toIso8601String(),
            ),
          );
        }
      } else {
        // Queue for later when online
        await _queuedActionLocalDatasource.createQueuedAction(
          QueuedActionModel(
            id: DateTime.now().millisecondsSinceEpoch,
            repository: 'CustomerRepository',
            method: 'updateCustomer',
            param: jsonEncode(customer.toMap()),
            isCritical: true,
            status: 'pending',
            retryCount: 0,
            createdAt: DateTime.now().toIso8601String(),
          ),
        );
      }
    } catch (e) {
      throw Exception('Failed to update customer: $e');
    }
  }

  @override
  Future<void> deleteCustomer(String id) async {
    try {
      // Always delete from local first
      await _localDatasource.deleteCustomer(id);
      
      if (await _connectivityRepository.isConnected) {
        try {
          // Try to sync to remote
          await _remoteDatasource.deleteCustomer(id);
        } catch (e) {
          // If remote fails, queue the action for later
          await _queuedActionLocalDatasource.createQueuedAction(
            QueuedActionModel(
              id: DateTime.now().millisecondsSinceEpoch,
              repository: 'CustomerRepository',
              method: 'deleteCustomer',
              param: id,
              isCritical: true,
              status: 'pending',
              retryCount: 0,
              createdAt: DateTime.now().toIso8601String(),
            ),
          );
        }
      } else {
        // Queue for later when online
        await _queuedActionLocalDatasource.createQueuedAction(
          QueuedActionModel(
            id: DateTime.now().millisecondsSinceEpoch,
            repository: 'CustomerRepository',
            method: 'deleteCustomer',
            param: id,
            isCritical: true,
            status: 'pending',
            retryCount: 0,
            createdAt: DateTime.now().toIso8601String(),
          ),
        );
      }
    } catch (e) {
      throw Exception('Failed to delete customer: $e');
    }
  }

  @override
  Future<void> updateCustomerPoints(String customerId, int points) async {
    try {
      // Always update local first
      await _localDatasource.updateCustomerPoints(customerId, points);
      
      if (await _connectivityRepository.isConnected) {
        try {
          // Try to sync to remote
          await _remoteDatasource.updateCustomerPoints(customerId, points);
        } catch (e) {
          // If remote fails, queue the action for later
          await _queuedActionLocalDatasource.createQueuedAction(
            QueuedActionModel(
              id: DateTime.now().millisecondsSinceEpoch,
              repository: 'CustomerRepository',
              method: 'updateCustomerPoints',
              param: jsonEncode({'customerId': customerId, 'points': points}),
              isCritical: true,
              status: 'pending',
              retryCount: 0,
              createdAt: DateTime.now().toIso8601String(),
            ),
          );
        }
      } else {
        // Queue for later when online
        await _queuedActionLocalDatasource.createQueuedAction(
          QueuedActionModel(
            id: DateTime.now().millisecondsSinceEpoch,
            repository: 'CustomerRepository',
            method: 'updateCustomerPoints',
            param: jsonEncode({'customerId': customerId, 'points': points}),
            isCritical: true,
            status: 'pending',
            retryCount: 0,
            createdAt: DateTime.now().toIso8601String(),
          ),
        );
      }
    } catch (e) {
      throw Exception('Failed to update customer points: $e');
    }
  }

  @override
  Future<void> updateCustomerLoyaltyPoints(String customerId, int loyaltyPoints) async {
    try {
      // Always update local first
      await _localDatasource.updateCustomerLoyaltyPoints(customerId, loyaltyPoints);
      
      if (await _connectivityRepository.isConnected) {
        try {
          // Try to sync to remote
          await _remoteDatasource.updateCustomerLoyaltyPoints(customerId, loyaltyPoints);
        } catch (e) {
          // If remote fails, queue the action for later
          await _queuedActionLocalDatasource.createQueuedAction(
            QueuedActionModel(
              id: DateTime.now().millisecondsSinceEpoch,
              repository: 'CustomerRepository',
              method: 'updateCustomerLoyaltyPoints',
              param: jsonEncode({'customerId': customerId, 'loyaltyPoints': loyaltyPoints}),
              isCritical: true,
              status: 'pending',
              retryCount: 0,
              createdAt: DateTime.now().toIso8601String(),
            ),
          );
        }
      } else {
        // Queue for later when online
        await _queuedActionLocalDatasource.createQueuedAction(
          QueuedActionModel(
            id: DateTime.now().millisecondsSinceEpoch,
            repository: 'CustomerRepository',
            method: 'updateCustomerLoyaltyPoints',
            param: jsonEncode({'customerId': customerId, 'loyaltyPoints': loyaltyPoints}),
            isCritical: true,
            status: 'pending',
            retryCount: 0,
            createdAt: DateTime.now().toIso8601String(),
          ),
        );
      }
    } catch (e) {
      throw Exception('Failed to update customer loyalty points: $e');
    }
  }

  @override
  Future<void> updateCustomerVisit(String customerId, double amount) async {
    try {
      // Always update local first
      await _localDatasource.updateCustomerVisit(customerId, amount);
      
      if (await _connectivityRepository.isConnected) {
        try {
          // Try to sync to remote
          await _remoteDatasource.updateCustomerVisit(customerId, amount);
        } catch (e) {
          // If remote fails, queue the action for later
          await _queuedActionLocalDatasource.createQueuedAction(
            QueuedActionModel(
              id: DateTime.now().millisecondsSinceEpoch,
              repository: 'CustomerRepository',
              method: 'updateCustomerVisit',
              param: jsonEncode({'customerId': customerId, 'amount': amount}),
              isCritical: true,
              status: 'pending',
              retryCount: 0,
              createdAt: DateTime.now().toIso8601String(),
            ),
          );
        }
      } else {
        // Queue for later when online
        await _queuedActionLocalDatasource.createQueuedAction(
          QueuedActionModel(
            id: DateTime.now().millisecondsSinceEpoch,
            repository: 'CustomerRepository',
            method: 'updateCustomerVisit',
            param: jsonEncode({'customerId': customerId, 'amount': amount}),
            isCritical: true,
            status: 'pending',
            retryCount: 0,
            createdAt: DateTime.now().toIso8601String(),
          ),
        );
      }
    } catch (e) {
      throw Exception('Failed to update customer visit: $e');
    }
  }

  @override
  Future<void> seedSampleCustomers() async {
    try {
      await _localDatasource.seedSampleCustomers();
    } catch (e) {
      throw Exception('Failed to seed sample customers: $e');
    }
  }

  // Implement existing interface methods
  @override
  Future<Result<CustomerEntity>> upsert(CustomerEntity customer) async {
    try {
      final customerId = await createCustomer(customer);
      final updatedCustomer = await getCustomerById(customerId);
      return Result.success(updatedCustomer);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<CustomerEntity>> get(String idOrPhone) async {
    try {
      CustomerEntity? customer;
      
      // Try by ID first, then by phone
      customer = await getCustomerById(idOrPhone);
      if (customer == null) {
        customer = await getCustomerByPhone(idOrPhone);
      }
      
      if (customer != null) {
        return Result.success(customer);
      } else {
        return Result.error(APIError(message: 'Customer not found'));
      }
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<void>> addPoints(String id, int delta) async {
    try {
      final customer = await getCustomerById(id);
      if (customer != null) {
        final newPoints = customer.pointsBalance + delta;
        await updateCustomerPoints(id, newPoints);
        return Result.success(null);
      } else {
        return Result.error(APIError(message: 'Customer not found'));
      }
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<List<CustomerEntity>>> search(String query) async {
    try {
      final customers = await searchCustomers(query);
      return Result.success(customers);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}