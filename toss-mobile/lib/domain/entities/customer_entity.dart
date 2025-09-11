import 'package:equatable/equatable.dart';

enum CustomerIdType { phone, membershipCard, qrCode, nfc, biometric }
enum CustomerTier { bronze, silver, gold, platinum, vip }
enum PreferredCommunication { sms, whatsapp, voice, print, none }

class CustomerEntity extends Equatable {
  final String id;
  final String? name;
  final String? phone;
  final String? alternatePhone;
  final String? membershipNumber;
  final String? qrCode;
  final String? nfcId;
  final String? biometricId; // Fingerprint or face recognition hash
  final CustomerIdType primaryIdType;
  final CustomerTier tier;
  final int pointsBalance;
  final double totalSpent; // Lifetime spending in cents
  final int visitCount;
  final DateTime? lastVisit;
  final DateTime? dateOfBirth;
  final String? address;
  final String? city;
  final String? country;
  final String? gender;
  final PreferredCommunication preferredCommunication;
  final Map<String, dynamic>? preferences; // Shopping preferences
  final List<String>? tags; // Customer tags for segmentation
  final bool isActive;
  final String? notes;
  final DateTime? anniversaryDate;
  final String? referredBy;
  final String? createdAt;
  final String? updatedAt;

  const CustomerEntity({
    required this.id,
    this.name,
    this.phone,
    this.alternatePhone,
    this.membershipNumber,
    this.qrCode,
    this.nfcId,
    this.biometricId,
    this.primaryIdType = CustomerIdType.phone,
    this.tier = CustomerTier.bronze,
    this.pointsBalance = 0,
    this.totalSpent = 0.0,
    this.visitCount = 0,
    this.lastVisit,
    this.dateOfBirth,
    this.address,
    this.city,
    this.country,
    this.gender,
    this.preferredCommunication = PreferredCommunication.sms,
    this.preferences,
    this.tags,
    this.isActive = true,
    this.notes,
    this.anniversaryDate,
    this.referredBy,
    this.createdAt,
    this.updatedAt,
  });

  CustomerEntity copyWith({
    String? id,
    String? name,
    String? phone,
    String? alternatePhone,
    String? membershipNumber,
    String? qrCode,
    String? nfcId,
    String? biometricId,
    CustomerIdType? primaryIdType,
    CustomerTier? tier,
    int? pointsBalance,
    double? totalSpent,
    int? visitCount,
    DateTime? lastVisit,
    DateTime? dateOfBirth,
    String? address,
    String? city,
    String? country,
    String? gender,
    PreferredCommunication? preferredCommunication,
    Map<String, dynamic>? preferences,
    List<String>? tags,
    bool? isActive,
    String? notes,
    DateTime? anniversaryDate,
    String? referredBy,
    String? createdAt,
    String? updatedAt,
  }) {
    return CustomerEntity(
      id: id ?? this.id,
      name: name ?? this.name,
      phone: phone ?? this.phone,
      alternatePhone: alternatePhone ?? this.alternatePhone,
      membershipNumber: membershipNumber ?? this.membershipNumber,
      qrCode: qrCode ?? this.qrCode,
      nfcId: nfcId ?? this.nfcId,
      biometricId: biometricId ?? this.biometricId,
      primaryIdType: primaryIdType ?? this.primaryIdType,
      tier: tier ?? this.tier,
      pointsBalance: pointsBalance ?? this.pointsBalance,
      totalSpent: totalSpent ?? this.totalSpent,
      visitCount: visitCount ?? this.visitCount,
      lastVisit: lastVisit ?? this.lastVisit,
      dateOfBirth: dateOfBirth ?? this.dateOfBirth,
      address: address ?? this.address,
      city: city ?? this.city,
      country: country ?? this.country,
      gender: gender ?? this.gender,
      preferredCommunication: preferredCommunication ?? this.preferredCommunication,
      preferences: preferences ?? this.preferences,
      tags: tags ?? this.tags,
      isActive: isActive ?? this.isActive,
      notes: notes ?? this.notes,
      anniversaryDate: anniversaryDate ?? this.anniversaryDate,
      referredBy: referredBy ?? this.referredBy,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
    );
  }

  // Helper methods
  String get primaryIdentifier {
    switch (primaryIdType) {
      case CustomerIdType.phone:
        return phone ?? 'No Phone';
      case CustomerIdType.membershipCard:
        return membershipNumber ?? 'No Membership';
      case CustomerIdType.qrCode:
        return qrCode ?? 'No QR Code';
      case CustomerIdType.nfc:
        return nfcId ?? 'No NFC ID';
      case CustomerIdType.biometric:
        return 'Biometric ID';
    }
  }

  bool get canReceiveSms => phone != null && [PreferredCommunication.sms, PreferredCommunication.whatsapp].contains(preferredCommunication);
  bool get canReceiveWhatsapp => phone != null && preferredCommunication == PreferredCommunication.whatsapp;
  
  bool get isBirthday {
    if (dateOfBirth == null) return false;
    final now = DateTime.now();
    return dateOfBirth!.month == now.month && dateOfBirth!.day == now.day;
  }
  
  bool get isAnniversary {
    if (anniversaryDate == null) return false;
    final now = DateTime.now();
    return anniversaryDate!.month == now.month && anniversaryDate!.day == now.day;
  }

  int get daysSinceLastVisit {
    if (lastVisit == null) return -1;
    return DateTime.now().difference(lastVisit!).inDays;
  }

  bool get isInactive => daysSinceLastVisit > 90; // Consider inactive after 90 days
  
  double get averageSpentPerVisit => visitCount > 0 ? totalSpent / visitCount : 0.0;

  @override
  List<Object?> get props => [
    id,
    name,
    phone,
    alternatePhone,
    membershipNumber,
    qrCode,
    nfcId,
    biometricId,
    primaryIdType,
    tier,
    pointsBalance,
    totalSpent,
    visitCount,
    lastVisit,
    dateOfBirth,
    address,
    city,
    country,
    gender,
    preferredCommunication,
    preferences,
    tags,
    isActive,
    notes,
    anniversaryDate,
    referredBy,
    createdAt,
    updatedAt,
  ];
}



