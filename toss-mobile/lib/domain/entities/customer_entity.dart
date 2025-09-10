import 'package:equatable/equatable.dart';

class CustomerEntity extends Equatable {
  final String id;
  final String? name;
  final String? phone;
  final int pointsBalance;
  final String? createdAt;
  final String? updatedAt;

  const CustomerEntity({
    required this.id,
    this.name,
    this.phone,
    this.pointsBalance = 0,
    this.createdAt,
    this.updatedAt,
  });

  CustomerEntity copyWith({
    String? id,
    String? name,
    String? phone,
    int? pointsBalance,
    String? createdAt,
    String? updatedAt,
  }) {
    return CustomerEntity(
      id: id ?? this.id,
      name: name ?? this.name,
      phone: phone ?? this.phone,
      pointsBalance: pointsBalance ?? this.pointsBalance,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
    );
  }

  @override
  List<Object?> get props => [id, name, phone, pointsBalance, createdAt, updatedAt];
}



