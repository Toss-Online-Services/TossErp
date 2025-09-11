import 'package:equatable/equatable.dart';

class SupplierEntity extends Equatable {
  final int? id;
  final String name;
  final String? companyName;
  final String? contactPerson;
  final String? phoneNumber;
  final String? email;
  final String? address;
  final String? city;
  final String? country;
  final String? taxNumber;
  final Map<String, dynamic>? paymentTerms;
  final bool isActive;
  final double? rating;
  final String? notes;
  final String? createdById;
  final String? createdAt;
  final String? updatedAt;

  const SupplierEntity({
    this.id,
    required this.name,
    this.companyName,
    this.contactPerson,
    this.phoneNumber,
    this.email,
    this.address,
    this.city,
    this.country,
    this.taxNumber,
    this.paymentTerms,
    this.isActive = true,
    this.rating,
    this.notes,
    this.createdById,
    this.createdAt,
    this.updatedAt,
  });

  SupplierEntity copyWith({
    int? id,
    String? name,
    String? companyName,
    String? contactPerson,
    String? phoneNumber,
    String? email,
    String? address,
    String? city,
    String? country,
    String? taxNumber,
    Map<String, dynamic>? paymentTerms,
    bool? isActive,
    double? rating,
    String? notes,
    String? createdById,
    String? createdAt,
    String? updatedAt,
  }) {
    return SupplierEntity(
      id: id ?? this.id,
      name: name ?? this.name,
      companyName: companyName ?? this.companyName,
      contactPerson: contactPerson ?? this.contactPerson,
      phoneNumber: phoneNumber ?? this.phoneNumber,
      email: email ?? this.email,
      address: address ?? this.address,
      city: city ?? this.city,
      country: country ?? this.country,
      taxNumber: taxNumber ?? this.taxNumber,
      paymentTerms: paymentTerms ?? this.paymentTerms,
      isActive: isActive ?? this.isActive,
      rating: rating ?? this.rating,
      notes: notes ?? this.notes,
      createdById: createdById ?? this.createdById,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
    );
  }

  @override
  List<Object?> get props => [
    id,
    name,
    companyName,
    contactPerson,
    phoneNumber,
    email,
    address,
    city,
    country,
    taxNumber,
    paymentTerms,
    isActive,
    rating,
    notes,
    createdById,
    createdAt,
    updatedAt,
  ];
}
