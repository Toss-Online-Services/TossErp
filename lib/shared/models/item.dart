import 'package:json_annotation/json_annotation.dart';

part 'item.g.dart';

@JsonSerializable()
class Item {
  final String id;
  final String sku;
  final String name;
  final String? description;
  final String? category;
  final String? unit;
  final double? standardRate;
  final double? minimumPrice;
  final double? weightPerUnit;
  final double? length;
  final double? width;
  final double? height;
  final bool isActive;
  final DateTime? createdAt;
  final String? createdBy;
  final DateTime? updatedAt;
  final String? updatedBy;

  const Item({
    required this.id,
    required this.sku,
    required this.name,
    this.description,
    this.category,
    this.unit,
    this.standardRate,
    this.minimumPrice,
    this.weightPerUnit,
    this.length,
    this.width,
    this.height,
    this.isActive = true,
    this.createdAt,
    this.createdBy,
    this.updatedAt,
    this.updatedBy,
  });

  factory Item.fromJson(Map<String, dynamic> json) => _$ItemFromJson(json);
  Map<String, dynamic> toJson() => _$ItemToJson(this);

  factory Item.fromMap(Map<String, dynamic> map) {
    return Item(
      id: map['id'] ?? '',
      sku: map['sku'] ?? '',
      name: map['name'] ?? '',
      description: map['description'],
      category: map['category'],
      unit: map['unit'],
      standardRate: map['standard_rate']?.toDouble(),
      minimumPrice: map['minimum_price']?.toDouble(),
      weightPerUnit: map['weight_per_unit']?.toDouble(),
      length: map['length']?.toDouble(),
      width: map['width']?.toDouble(),
      height: map['height']?.toDouble(),
      isActive: map['is_active'] == 1,
      createdAt: map['created_at'] != null ? DateTime.parse(map['created_at']) : null,
      createdBy: map['created_by'],
      updatedAt: map['updated_at'] != null ? DateTime.parse(map['updated_at']) : null,
      updatedBy: map['updated_by'],
    );
  }

  Map<String, dynamic> toMap() {
    return {
      'id': id,
      'sku': sku,
      'name': name,
      'description': description,
      'category': category,
      'unit': unit,
      'standard_rate': standardRate,
      'minimum_price': minimumPrice,
      'weight_per_unit': weightPerUnit,
      'length': length,
      'width': width,
      'height': height,
      'is_active': isActive ? 1 : 0,
      'created_at': createdAt?.toIso8601String(),
      'created_by': createdBy,
      'updated_at': updatedAt?.toIso8601String(),
      'updated_by': updatedBy,
    };
  }

  Item copyWith({
    String? id,
    String? sku,
    String? name,
    String? description,
    String? category,
    String? unit,
    double? standardRate,
    double? minimumPrice,
    double? weightPerUnit,
    double? length,
    double? width,
    double? height,
    bool? isActive,
    DateTime? createdAt,
    String? createdBy,
    DateTime? updatedAt,
    String? updatedBy,
  }) {
    return Item(
      id: id ?? this.id,
      sku: sku ?? this.sku,
      name: name ?? this.name,
      description: description ?? this.description,
      category: category ?? this.category,
      unit: unit ?? this.unit,
      standardRate: standardRate ?? this.standardRate,
      minimumPrice: minimumPrice ?? this.minimumPrice,
      weightPerUnit: weightPerUnit ?? this.weightPerUnit,
      length: length ?? this.length,
      width: width ?? this.width,
      height: height ?? this.height,
      isActive: isActive ?? this.isActive,
      createdAt: createdAt ?? this.createdAt,
      createdBy: createdBy ?? this.createdBy,
      updatedAt: updatedAt ?? this.updatedAt,
      updatedBy: updatedBy ?? this.updatedBy,
    );
  }

  @override
  String toString() {
    return 'Item(id: $id, sku: $sku, name: $name, category: $category, isActive: $isActive)';
  }

  @override
  bool operator ==(Object other) {
    if (identical(this, other)) return true;
    return other is Item && other.id == id;
  }

  @override
  int get hashCode {
    return id.hashCode;
  }
}
