import 'dart:convert';
import 'dart:io';
import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:firebase_storage/firebase_storage.dart';
import 'package:firebase_auth/firebase_auth.dart';

import '../../domain/entities/sync_entity.dart';

class CloudStorageRepository {
  final FirebaseFirestore _firestore = FirebaseFirestore.instance;
  final FirebaseStorage _storage = FirebaseStorage.instance;
  final FirebaseAuth _auth = FirebaseAuth.instance;

  // Entity Management
  Future<void> saveEntity(
    SyncEntityType entityType,
    String entityId,
    Map<String, dynamic> data,
  ) async {
    final user = _auth.currentUser;
    if (user == null) throw Exception('User not authenticated');

    final collection = _getCollectionPath(entityType, user.uid);
    
    // Add metadata
    data['entityId'] = entityId;
    data['entityType'] = entityType.name;
    data['userId'] = user.uid;
    data['serverUpdatedAt'] = FieldValue.serverTimestamp();
    
    await _firestore
        .collection(collection)
        .doc(entityId)
        .set(data, SetOptions(merge: true));
  }

  Future<Map<String, dynamic>?> getEntity(
    SyncEntityType entityType,
    String entityId,
  ) async {
    final user = _auth.currentUser;
    if (user == null) throw Exception('User not authenticated');

    final collection = _getCollectionPath(entityType, user.uid);
    
    final doc = await _firestore
        .collection(collection)
        .doc(entityId)
        .get();

    if (doc.exists) {
      final data = doc.data()!;
      data['id'] = doc.id;
      return data;
    }
    return null;
  }

  Future<List<Map<String, dynamic>>> getAllEntities(
    SyncEntityType entityType, {
    DateTime? modifiedSince,
    int? limit,
    String? lastDocumentId,
    String? locationId,
  }) async {
    final user = _auth.currentUser;
    if (user == null) throw Exception('User not authenticated');

    final collection = _getCollectionPath(entityType, user.uid);
    Query query = _firestore.collection(collection);

    // Filter by location if specified
    if (locationId != null) {
      query = query.where('locationId', isEqualTo: locationId);
    }

    // Filter by modification date
    if (modifiedSince != null) {
      query = query.where('serverUpdatedAt', isGreaterThan: modifiedSince);
    }

    // Order by update time for consistent pagination
    query = query.orderBy('serverUpdatedAt', descending: true);

    // Pagination
    if (lastDocumentId != null) {
      final lastDoc = await _firestore
          .collection(collection)
          .doc(lastDocumentId)
          .get();
      if (lastDoc.exists) {
        query = query.startAfterDocument(lastDoc);
      }
    }

    if (limit != null) {
      query = query.limit(limit);
    }

    final snapshot = await query.get();
    
    return snapshot.docs.map((doc) {
      final data = doc.data() as Map<String, dynamic>;
      data['id'] = doc.id;
      return data;
    }).toList();
  }

  Future<void> deleteEntity(
    SyncEntityType entityType,
    String entityId,
  ) async {
    final user = _auth.currentUser;
    if (user == null) throw Exception('User not authenticated');

    final collection = _getCollectionPath(entityType, user.uid);
    
    await _firestore
        .collection(collection)
        .doc(entityId)
        .delete();
  }

  // Batch Operations
  Future<void> saveEntitiesBatch(
    SyncEntityType entityType,
    List<Map<String, dynamic>> entities,
  ) async {
    final user = _auth.currentUser;
    if (user == null) throw Exception('User not authenticated');

    final collection = _getCollectionPath(entityType, user.uid);
    final batch = _firestore.batch();

    for (final entity in entities) {
      final entityId = entity['id'] as String;
      entity['entityType'] = entityType.name;
      entity['userId'] = user.uid;
      entity['serverUpdatedAt'] = FieldValue.serverTimestamp();
      
      final docRef = _firestore.collection(collection).doc(entityId);
      batch.set(docRef, entity, SetOptions(merge: true));
    }

    await batch.commit();
  }

  Future<void> deleteEntitiesBatch(
    SyncEntityType entityType,
    List<String> entityIds,
  ) async {
    final user = _auth.currentUser;
    if (user == null) throw Exception('User not authenticated');

    final collection = _getCollectionPath(entityType, user.uid);
    final batch = _firestore.batch();

    for (final entityId in entityIds) {
      final docRef = _firestore.collection(collection).doc(entityId);
      batch.delete(docRef);
    }

    await batch.commit();
  }

  // Real-time Synchronization
  Stream<List<Map<String, dynamic>>> watchEntities(
    SyncEntityType entityType, {
    String? locationId,
    DateTime? since,
  }) {
    final user = _auth.currentUser;
    if (user == null) throw Exception('User not authenticated');

    final collection = _getCollectionPath(entityType, user.uid);
    Query query = _firestore.collection(collection);

    if (locationId != null) {
      query = query.where('locationId', isEqualTo: locationId);
    }

    if (since != null) {
      query = query.where('serverUpdatedAt', isGreaterThan: since);
    }

    query = query.orderBy('serverUpdatedAt', descending: true);

    return query.snapshots().map((snapshot) {
      return snapshot.docs.map((doc) {
        final data = doc.data() as Map<String, dynamic>;
        data['id'] = doc.id;
        return data;
      }).toList();
    });
  }

  Stream<Map<String, dynamic>?> watchEntity(
    SyncEntityType entityType,
    String entityId,
  ) {
    final user = _auth.currentUser;
    if (user == null) throw Exception('User not authenticated');

    final collection = _getCollectionPath(entityType, user.uid);
    
    return _firestore
        .collection(collection)
        .doc(entityId)
        .snapshots()
        .map((doc) {
      if (doc.exists) {
        final data = doc.data()!;
        data['id'] = doc.id;
        return data;
      }
      return null;
    });
  }

  // Conflict Detection
  Future<Map<String, dynamic>?> getEntityVersion(
    SyncEntityType entityType,
    String entityId,
  ) async {
    final entity = await getEntity(entityType, entityId);
    if (entity != null) {
      return {
        'id': entityId,
        'version': entity['version'] ?? 1,
        'serverUpdatedAt': entity['serverUpdatedAt'],
        'checksum': _calculateChecksum(entity),
      };
    }
    return null;
  }

  Future<List<Map<String, dynamic>>> getEntityVersions(
    SyncEntityType entityType,
    List<String> entityIds,
  ) async {
    final user = _auth.currentUser;
    if (user == null) throw Exception('User not authenticated');

    final collection = _getCollectionPath(entityType, user.uid);
    final versions = <Map<String, dynamic>>[];

    // Firestore has a limit of 10 documents per 'in' query
    final chunks = _chunkList(entityIds, 10);
    
    for (final chunk in chunks) {
      final snapshot = await _firestore
          .collection(collection)
          .where(FieldPath.documentId, whereIn: chunk)
          .get();

      for (final doc in snapshot.docs) {
        final data = doc.data();
        versions.add({
          'id': doc.id,
          'version': data['version'] ?? 1,
          'serverUpdatedAt': data['serverUpdatedAt'],
          'checksum': _calculateChecksum(data),
        });
      }
    }

    return versions;
  }

  // File Management
  Future<String> uploadFile(
    String filePath,
    String fileName, {
    String? folder,
    Map<String, String>? metadata,
  }) async {
    final user = _auth.currentUser;
    if (user == null) throw Exception('User not authenticated');

    final file = File(filePath);
    if (!file.existsSync()) {
      throw Exception('File does not exist: $filePath');
    }

    final storageRef = _storage.ref().child('users/${user.uid}');
    final fileRef = folder != null 
        ? storageRef.child('$folder/$fileName')
        : storageRef.child(fileName);

    final uploadTask = fileRef.putFile(
      file,
      SettableMetadata(
        contentType: _getContentType(fileName),
        customMetadata: metadata,
      ),
    );

    final snapshot = await uploadTask;
    return await snapshot.ref.getDownloadURL();
  }

  Future<void> downloadFile(
    String downloadUrl,
    String localPath,
  ) async {
    final file = File(localPath);
    final ref = _storage.refFromURL(downloadUrl);
    
    await ref.writeToFile(file);
  }

  Future<void> deleteFile(String downloadUrl) async {
    final ref = _storage.refFromURL(downloadUrl);
    await ref.delete();
  }

  // Search and Query
  Future<List<Map<String, dynamic>>> searchEntities(
    SyncEntityType entityType,
    String searchTerm, {
    List<String>? searchFields,
    int? limit,
    String? locationId,
  }) async {
    final user = _auth.currentUser;
    if (user == null) throw Exception('User not authenticated');

    final collection = _getCollectionPath(entityType, user.uid);
    Query query = _firestore.collection(collection);

    if (locationId != null) {
      query = query.where('locationId', isEqualTo: locationId);
    }

    // Firestore doesn't support full-text search natively
    // This is a basic implementation - consider using Algolia for advanced search
    final fields = searchFields ?? ['name', 'description'];
    
    // For now, we'll search by the first field only
    if (fields.isNotEmpty) {
      query = query
          .where(fields.first, isGreaterThanOrEqualTo: searchTerm)
          .where(fields.first, isLessThan: '${searchTerm}z');
    }

    if (limit != null) {
      query = query.limit(limit);
    }

    final snapshot = await query.get();
    
    return snapshot.docs.map((doc) {
      final data = doc.data() as Map<String, dynamic>;
      data['id'] = doc.id;
      return data;
    }).toList();
  }

  Future<List<Map<String, dynamic>>> queryEntities(
    SyncEntityType entityType, {
    Map<String, dynamic>? filters,
    String? orderBy,
    bool descending = false,
    int? limit,
    String? lastDocumentId,
  }) async {
    final user = _auth.currentUser;
    if (user == null) throw Exception('User not authenticated');

    final collection = _getCollectionPath(entityType, user.uid);
    Query query = _firestore.collection(collection);

    // Apply filters
    if (filters != null) {
      for (final entry in filters.entries) {
        query = query.where(entry.key, isEqualTo: entry.value);
      }
    }

    // Apply ordering
    if (orderBy != null) {
      query = query.orderBy(orderBy, descending: descending);
    } else {
      query = query.orderBy('serverUpdatedAt', descending: true);
    }

    // Pagination
    if (lastDocumentId != null) {
      final lastDoc = await _firestore
          .collection(collection)
          .doc(lastDocumentId)
          .get();
      if (lastDoc.exists) {
        query = query.startAfterDocument(lastDoc);
      }
    }

    if (limit != null) {
      query = query.limit(limit);
    }

    final snapshot = await query.get();
    
    return snapshot.docs.map((doc) {
      final data = doc.data() as Map<String, dynamic>;
      data['id'] = doc.id;
      return data;
    }).toList();
  }

  // Statistics and Analytics
  Future<Map<String, dynamic>> getEntityStatistics(
    SyncEntityType entityType, {
    String? locationId,
    DateTime? since,
  }) async {
    final user = _auth.currentUser;
    if (user == null) throw Exception('User not authenticated');

    final collection = _getCollectionPath(entityType, user.uid);
    Query query = _firestore.collection(collection);

    if (locationId != null) {
      query = query.where('locationId', isEqualTo: locationId);
    }

    if (since != null) {
      query = query.where('serverUpdatedAt', isGreaterThan: since);
    }

    final snapshot = await query.get();
    
    return {
      'total': snapshot.size,
      'lastUpdated': snapshot.docs.isNotEmpty 
          ? snapshot.docs.first.data()['serverUpdatedAt']
          : null,
    };
  }

  // User and Location Management
  Future<Map<String, dynamic>?> getUserProfile(String userId) async {
    final doc = await _firestore
        .collection('users')
        .doc(userId)
        .get();

    if (doc.exists) {
      final data = doc.data()!;
      data['id'] = doc.id;
      return data;
    }
    return null;
  }

  Future<void> updateUserProfile(
    String userId,
    Map<String, dynamic> profile,
  ) async {
    profile['updatedAt'] = FieldValue.serverTimestamp();
    
    await _firestore
        .collection('users')
        .doc(userId)
        .set(profile, SetOptions(merge: true));
  }

  Future<List<Map<String, dynamic>>> getUserLocations(String userId) async {
    final snapshot = await _firestore
        .collection('locations')
        .where('userIds', arrayContains: userId)
        .get();

    return snapshot.docs.map((doc) {
      final data = doc.data();
      data['id'] = doc.id;
      return data;
    }).toList();
  }

  // Backup and Restore
  Future<Map<String, dynamic>> createCloudBackup(
    List<SyncEntityType> entityTypes, {
    String? locationId,
  }) async {
    final user = _auth.currentUser;
    if (user == null) throw Exception('User not authenticated');

    final backup = <String, dynamic>{
      'version': '1.0',
      'createdAt': FieldValue.serverTimestamp(),
      'userId': user.uid,
      'locationId': locationId,
      'entities': <String, List<Map<String, dynamic>>>{},
    };

    for (final entityType in entityTypes) {
      final entities = await getAllEntities(
        entityType,
        locationId: locationId,
      );
      backup['entities'][entityType.name] = entities;
    }

    // Save backup to Firestore
    final backupRef = await _firestore
        .collection('backups')
        .add(backup);

    backup['id'] = backupRef.id;
    return backup;
  }

  Future<List<Map<String, dynamic>>> getCloudBackups() async {
    final user = _auth.currentUser;
    if (user == null) throw Exception('User not authenticated');

    final snapshot = await _firestore
        .collection('backups')
        .where('userId', isEqualTo: user.uid)
        .orderBy('createdAt', descending: true)
        .get();

    return snapshot.docs.map((doc) {
      final data = doc.data();
      data['id'] = doc.id;
      return data;
    }).toList();
  }

  Future<Map<String, dynamic>?> getCloudBackup(String backupId) async {
    final doc = await _firestore
        .collection('backups')
        .doc(backupId)
        .get();

    if (doc.exists) {
      final data = doc.data()!;
      data['id'] = doc.id;
      return data;
    }
    return null;
  }

  Future<void> deleteCloudBackup(String backupId) async {
    await _firestore
        .collection('backups')
        .doc(backupId)
        .delete();
  }

  // Multi-location Sync
  Future<void> syncBetweenLocations(
    String sourceLocationId,
    String targetLocationId,
    List<SyncEntityType> entityTypes,
  ) async {
    final user = _auth.currentUser;
    if (user == null) throw Exception('User not authenticated');

    for (final entityType in entityTypes) {
      final entities = await getAllEntities(
        entityType,
        locationId: sourceLocationId,
      );

      for (final entity in entities) {
        entity['locationId'] = targetLocationId;
        entity['syncedFrom'] = sourceLocationId;
        entity['syncedAt'] = FieldValue.serverTimestamp();
        
        await saveEntity(entityType, entity['id'], entity);
      }
    }
  }

  // Helper Methods
  String _getCollectionPath(SyncEntityType entityType, String userId) {
    return 'users/$userId/${entityType.name}';
  }

  String _calculateChecksum(Map<String, dynamic> data) {
    // Remove metadata fields for checksum calculation
    final cleanData = Map<String, dynamic>.from(data);
    cleanData.removeWhere((key, value) => 
        key.startsWith('server') || 
        key == 'lastSyncAt' || 
        key == 'syncedAt' ||
        key == 'syncedFrom');
    
    final jsonString = jsonEncode(cleanData);
    return jsonString.hashCode.toString();
  }

  String _getContentType(String fileName) {
    final extension = fileName.split('.').last.toLowerCase();
    switch (extension) {
      case 'jpg':
      case 'jpeg':
        return 'image/jpeg';
      case 'png':
        return 'image/png';
      case 'pdf':
        return 'application/pdf';
      case 'txt':
        return 'text/plain';
      case 'json':
        return 'application/json';
      default:
        return 'application/octet-stream';
    }
  }

  List<List<T>> _chunkList<T>(List<T> list, int chunkSize) {
    final chunks = <List<T>>[];
    for (int i = 0; i < list.length; i += chunkSize) {
      chunks.add(list.sublist(
        i, 
        i + chunkSize > list.length ? list.length : i + chunkSize,
      ));
    }
    return chunks;
  }

  // Transaction Support
  Future<void> runTransaction(
    Future<void> Function(Transaction transaction) updateFunction,
  ) async {
    await _firestore.runTransaction(updateFunction);
  }

  // Offline Support
  Future<void> enableOfflineSupport() async {
    await _firestore.enablePersistence();
  }

  Future<void> disableOfflineSupport() async {
    await _firestore.disableNetwork();
  }

  // Cache Management
  Future<void> clearCache() async {
    await _firestore.clearPersistence();
  }

  // Health Check
  Future<bool> isConnected() async {
    try {
      await _firestore.runTransaction((transaction) async {
        // Simple read operation to test connectivity
      });
      return true;
    } catch (e) {
      return false;
    }
  }

  Future<Map<String, dynamic>> getHealthStatus() async {
    try {
      final startTime = DateTime.now();
      await _firestore.runTransaction((transaction) async {});
      final endTime = DateTime.now();
      
      return {
        'isConnected': true,
        'latency': endTime.difference(startTime).inMilliseconds,
        'timestamp': DateTime.now().toIso8601String(),
      };
    } catch (e) {
      return {
        'isConnected': false,
        'error': e.toString(),
        'timestamp': DateTime.now().toIso8601String(),
      };
    }
  }
}
