import 'package:equatable/equatable.dart';

class ProductCategoryEntity extends Equatable {
  final int? id;
  final String name;
  final String? description;
  final String? imageUrl;
  final int? parentCategoryId;
  final String color;
  final int sortOrder;
  final bool isActive;
  final String? createdById;
  final String? createdAt;
  final String? updatedAt;

  const ProductCategoryEntity({
    this.id,
    required this.name,
    this.description,
    this.imageUrl,
    this.parentCategoryId,
    this.color = '#2196F3',
    this.sortOrder = 0,
    this.isActive = true,
    this.createdById,
    this.createdAt,
    this.updatedAt,
  });

  ProductCategoryEntity copyWith({
    int? id,
    String? name,
    String? description,
    String? imageUrl,
    int? parentCategoryId,
    String? color,
    int? sortOrder,
    bool? isActive,
    String? createdById,
    String? createdAt,
    String? updatedAt,
  }) {
    return ProductCategoryEntity(
      id: id ?? this.id,
      name: name ?? this.name,
      description: description ?? this.description,
      imageUrl: imageUrl ?? this.imageUrl,
      parentCategoryId: parentCategoryId ?? this.parentCategoryId,
      color: color ?? this.color,
      sortOrder: sortOrder ?? this.sortOrder,
      isActive: isActive ?? this.isActive,
      createdById: createdById ?? this.createdById,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
    );
  }

  @override
  List<Object?> get props => [
    id,
    name,
    description,
    imageUrl,
    parentCategoryId,
    color,
    sortOrder,
    isActive,
    createdById,
    createdAt,
    updatedAt,
  ];
}
