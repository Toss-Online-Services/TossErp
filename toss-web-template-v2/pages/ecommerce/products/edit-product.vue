<template>
  <div class="py-4 container-fluid">
    <div class="row">
      <div class="col-12">
        <div class="card">
          <div class="pb-0 card-header">
            <MDTypography variant="h4" font-weight="bold">
              Edit Product
            </MDTypography>
            <MDTypography variant="button" color="text">
              Update product information and media
            </MDTypography>
          </div>
          <div class="card-body">
            <form @submit.prevent="saveProduct">
              <div class="row">
                <div class="col-lg-8">
                  <!-- Product Information -->
                  <div class="mb-4 card">
                    <div class="card-body">
                      <MDTypography variant="h6" font-weight="medium" class="mb-3">
                        Product Information
                      </MDTypography>
                      <div class="row">
                        <div class="mb-3 col-12">
                          <MDInput
                            v-model="product.name"
                            label="Product Name"
                            placeholder="Enter product name"
                            type="text"
                            required
                          />
                        </div>
                        <div class="mb-3 col-md-6">
                          <label class="form-label">Category</label>
                          <select v-model="product.category" class="form-select">
                            <option value="electronics">Electronics</option>
                            <option value="clothing">Clothing</option>
                            <option value="furniture">Furniture</option>
                            <option value="accessories">Accessories</option>
                          </select>
                        </div>
                        <div class="mb-3 col-md-6">
                          <MDInput
                            v-model="product.sku"
                            label="SKU"
                            placeholder="Product SKU"
                            type="text"
                            required
                          />
                        </div>
                        <div class="mb-3 col-12">
                          <label class="form-label">Description</label>
                          <textarea 
                            v-model="product.description"
                            class="form-control" 
                            rows="5"
                            placeholder="Enter product description"
                          ></textarea>
                        </div>
                      </div>
                    </div>
                  </div>

                  <!-- Pricing -->
                  <div class="mb-4 card">
                    <div class="card-body">
                      <MDTypography variant="h6" font-weight="medium" class="mb-3">
                        Pricing
                      </MDTypography>
                      <div class="row">
                        <div class="mb-3 col-md-4">
                          <MDInput
                            v-model="product.price"
                            label="Price"
                            placeholder="0.00"
                            type="number"
                            step="0.01"
                            required
                          />
                        </div>
                        <div class="mb-3 col-md-4">
                          <MDInput
                            v-model="product.comparePrice"
                            label="Compare at Price"
                            placeholder="0.00"
                            type="number"
                            step="0.01"
                          />
                        </div>
                        <div class="mb-3 col-md-4">
                          <MDInput
                            v-model="product.costPrice"
                            label="Cost per Item"
                            placeholder="0.00"
                            type="number"
                            step="0.01"
                          />
                        </div>
                      </div>
                    </div>
                  </div>

                  <!-- Inventory -->
                  <div class="mb-4 card">
                    <div class="card-body">
                      <MDTypography variant="h6" font-weight="medium" class="mb-3">
                        Inventory
                      </MDTypography>
                      <div class="row">
                        <div class="mb-3 col-md-6">
                          <MDInput
                            v-model="product.stock"
                            label="Stock Quantity"
                            placeholder="0"
                            type="number"
                            required
                          />
                        </div>
                        <div class="mb-3 col-md-6">
                          <MDInput
                            v-model="product.weight"
                            label="Weight (kg)"
                            placeholder="0.0"
                            type="number"
                            step="0.1"
                          />
                        </div>
                      </div>
                    </div>
                  </div>
                </div>

                <div class="col-lg-4">
                  <!-- Product Images -->
                  <div class="mb-4 card">
                    <div class="card-body">
                      <MDTypography variant="h6" font-weight="medium" class="mb-3">
                        Product Media
                      </MDTypography>
                      <div class="row">
                        <div class="mb-3 col-12" v-for="(image, index) in product.images" :key="index">
                          <div class="position-relative">
                            <img :src="image" class="img-fluid border-radius-lg" alt="product">
                            <button 
                              type="button"
                              class="top-0 m-2 btn btn-sm btn-icon-only btn-rounded btn-outline-danger position-absolute end-0"
                              @click="removeImage(index)"
                            >
                              <Icon name="mdi:close" size="16" />
                            </button>
                          </div>
                        </div>
                        <div class="col-12">
                          <div class="p-4 text-center border border-2 border-dashed border-radius-lg">
                            <Icon name="mdi:cloud-upload" size="48" class="mb-2 text-secondary" />
                            <MDTypography variant="caption" color="text" class="mb-2 d-block">
                              Drop files here or click to upload
                            </MDTypography>
                            <MDButton color="info" size="sm">
                              Add Image
                            </MDButton>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>

                  <!-- Product Status -->
                  <div class="mb-4 card">
                    <div class="card-body">
                      <MDTypography variant="h6" font-weight="medium" class="mb-3">
                        Product Status
                      </MDTypography>
                      <div class="mb-3 form-check form-switch">
                        <input 
                          v-model="product.active"
                          class="form-check-input" 
                          type="checkbox" 
                          id="productActive"
                        >
                        <label class="form-check-label" for="productActive">
                          Active
                        </label>
                      </div>
                      <div class="mb-3 form-check form-switch">
                        <input 
                          v-model="product.featured"
                          class="form-check-input" 
                          type="checkbox" 
                          id="productFeatured"
                        >
                        <label class="form-check-label" for="productFeatured">
                          Featured
                        </label>
                      </div>
                      <div class="form-check form-switch">
                        <input 
                          v-model="product.visible"
                          class="form-check-input" 
                          type="checkbox" 
                          id="productVisible"
                        >
                        <label class="form-check-label" for="productVisible">
                          Visible on Store
                        </label>
                      </div>
                    </div>
                  </div>

                  <!-- Actions -->
                  <div class="gap-2 d-grid">
                    <MDButton type="submit" color="success" size="lg">
                      <Icon name="mdi:content-save" class="me-1" />
                      Save Changes
                    </MDButton>
                    <MDButton color="light" size="lg" @click="$router.back()">
                      Cancel
                    </MDButton>
                    <MDButton color="error" size="lg" variant="outlined">
                      <Icon name="mdi:delete" class="me-1" />
                      Delete Product
                    </MDButton>
                  </div>
                </div>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'nuxt/app'

definePageMeta({
  layout: 'default'
})

const router = useRouter()

const product = ref({
  name: 'Premium Wireless Headphones',
  category: 'electronics',
  sku: 'WH-1000XM4',
  description: 'Industry-leading noise canceling with Dual Noise Sensor technology. Next-level music with Edge-AI, co-developed with Sony Music Studios Tokyo. Up to 30-hour battery life with quick charging. Touch sensor controls to pause, play, skip tracks, control volume, activate your voice assistant, and answer phone calls. Speak-to-chat technology automatically reduces volume during conversations.',
  price: 349.99,
  comparePrice: 399.99,
  costPrice: 200.00,
  stock: 45,
  weight: 0.5,
  images: [
    'https://images.unsplash.com/photo-1505740420928-5e560c06d30e?auto=format&fit=crop&w=500',
    'https://images.unsplash.com/photo-1484704849700-f032a568e944?auto=format&fit=crop&w=500',
    'https://images.unsplash.com/photo-1487215078519-e21cc028cb29?auto=format&fit=crop&w=500'
  ],
  active: true,
  featured: true,
  visible: true
})

const removeImage = (index: number) => {
  product.value.images.splice(index, 1)
}

const saveProduct = () => {
  alert('Product updated successfully!')
  router.push('/ecommerce/products/product-page')
}
</script>

<style scoped>
.form-select,
.form-control {
  border: 1px solid #d2d6da;
  border-radius: 0.5rem;
  padding: 0.5rem 0.75rem;
  font-size: 0.875rem;
  transition: border-color 0.15s ease-in-out;
}

.form-select:focus,
.form-control:focus {
  border-color: #5e72e4;
  outline: 0;
}

.form-check-input {
  width: 2.5rem;
  height: 1.25rem;
  cursor: pointer;
}

.form-check-input:checked {
  background-color: #5e72e4;
  border-color: #5e72e4;
}

.border-dashed {
  border-style: dashed !important;
}

.d-grid {
  display: grid;
}

.gap-2 {
  gap: 0.5rem;
}

.position-relative {
  position: relative;
}

.position-absolute {
  position: absolute;
}

.top-0 {
  top: 0;
}

.end-0 {
  right: 0;
}

.m-2 {
  margin: 0.5rem;
}

.btn-icon-only {
  width: 2rem;
  height: 2rem;
  padding: 0;
  display: flex;
  align-items: center;
  justify-content: center;
}

.btn-rounded {
  border-radius: 50%;
}

.btn-outline-danger {
  color: #f44335;
  border-color: #f44335;
  background: white;
}

.btn-outline-danger:hover {
  background-color: #f44335;
  color: white;
}
</style>
