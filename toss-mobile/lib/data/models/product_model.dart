import '../../domain/entities/product_entity.dart';

class ProductModel {
  int id;
  String createdById;
  String name;
  String? barcode;
  String imageUrl;
  int stock;
  int sold;
  int price;
  String? description;
  String? createdAt;
  String? updatedAt;

  ProductModel({
    required this.id,
    required this.createdById,
    required this.name,
    this.barcode,
    required this.imageUrl,
    required this.stock,
    required this.sold,
    required this.price,
    this.description,
    this.createdAt,
    this.updatedAt,
  });

  factory ProductModel.fromJson(Map<String, dynamic> json) {
    return ProductModel(
      id: json['id'],
      createdById: json['createdById'],
      name: json['name'],
      barcode: json['barcode'],
      imageUrl: json['imageUrl'],
      stock: json['stock'],
      sold: json['sold'],
      price: json['price'],
      description: json['description'],
      createdAt: json['createdAt'],
      updatedAt: json['updatedAt'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'createdById': createdById,
      'name': name,
      'barcode': barcode,
      'imageUrl': imageUrl,
      'stock': stock,
      'sold': sold,
      'price': price,
      'description': description,
      'createdAt': createdAt,
      'updatedAt': updatedAt,
    };
  }

  factory ProductModel.fromEntity(ProductEntity entity) {
    return ProductModel(
      id: entity.id ?? DateTime.now().millisecondsSinceEpoch,
      createdById: entity.createdById,
      name: entity.name,
      barcode: entity.barcode,
      imageUrl: entity.imageUrl,
      stock: entity.stock,
      sold: entity.sold ?? 0,
      price: entity.price,
      description: entity.description,
      createdAt: entity.createdAt ?? DateTime.now().toIso8601String(),
      updatedAt: entity.updatedAt ?? DateTime.now().toIso8601String(),
    );
  }

  ProductEntity toEntity() {
    return ProductEntity(
      id: id,
      createdById: createdById,
      name: name,
      barcode: barcode,
      imageUrl: imageUrl,
      stock: stock,
      sold: sold,
      price: price,
      description: description,
      createdAt: createdAt,
      updatedAt: updatedAt,
    );
  }

  // Create an empty ProductModel for comparison purposes
  factory ProductModel.empty() {
    return ProductModel(
      id: 0,
      createdById: '',
      name: '',
      imageUrl: '',
      stock: 0,
      sold: 0,
      price: 0,
    );
  }
}
