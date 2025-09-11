import 'package:equatable/equatable.dart';

enum VariantType { size, color, material, style, flavor, other }

class ProductVariantEntity extends Equatable {
  final int? id;
  final int productId;
  final String name;
  final VariantType type;
  final String value;
  final int additionalPrice; // in cents
  final String? sku;
  final int stock;
  final bool isActive;
  final int sortOrder;
  final String? createdById;
  final String? createdAt;
  final String? updatedAt;

  const ProductVariantEntity({
    this.id,
    required this.productId,
    required this.name,
    required this.type,
    required this.value,
    this.additionalPrice = 0,
    this.sku,
    this.stock = 0,
    this.isActive = true,
    this.sortOrder = 0,
    this.createdById,
    this.createdAt,
    this.updatedAt,
  });

  ProductVariantEntity copyWith({
    int? id,
    int? productId,
    String? name,
    VariantType? type,
    String? value,
    int? additionalPrice,
    String? sku,
    int? stock,
    bool? isActive,
    int? sortOrder,
    String? createdById,
    String? createdAt,
    String? updatedAt,
  }) {
    return ProductVariantEntity(
      id: id ?? this.id,
      productId: productId ?? this.productId,
      name: name ?? this.name,
      type: type ?? this.type,
      value: value ?? this.value,
      additionalPrice: additionalPrice ?? this.additionalPrice,
      sku: sku ?? this.sku,
      stock: stock ?? this.stock,
      isActive: isActive ?? this.isActive,
      sortOrder: sortOrder ?? this.sortOrder,
      createdById: createdById ?? this.createdById,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
    );
  }

  @override
  List<Object?> get props => [
    id,
    productId,
    name,
    type,
    value,
    additionalPrice,
    sku,
    stock,
    isActive,
    sortOrder,
    createdById,
    createdAt,
    updatedAt,
  ];
}
