import '../../domain/entities/product_entity.dart';

class ProductService {
  Future<List<ProductEntity>> getAllProducts() async {
    // TODO: Implement product retrieval
    return [];
  }

  Future<ProductEntity?> getProductById(String id) async {
    // TODO: Implement product retrieval by ID
    return null;
  }

  Future<String> createProduct(ProductEntity product) async {
    // TODO: Implement product creation
    return 'temp_id';
  }

  Future<void> updateProduct(ProductEntity product) async {
    // TODO: Implement product update
  }

  Future<void> deleteProduct(String id) async {
    // TODO: Implement product deletion
  }
}
