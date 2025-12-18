<template>
  <div class="container-fluid py-4">
    <div class="row">
      <div class="col-12">
        <div class="card">
          <div class="card-header pb-0">
            <MDTypography variant="h6" font-weight="medium">
              New Product
            </MDTypography>
          </div>
          <div class="card-body">
            <div class="row">
              <div class="col-md-8">
                <MDTypography variant="button" font-weight="bold" class="mb-2">
                  Product Information
                </MDTypography>
                <div class="row mt-3">
                  <div class="col-12">
                    <MDInput 
                      v-model="product.name" 
                      label="Product Name"
                      placeholder="Enter product name"
                      full-width 
                      class="mb-3"
                    />
                  </div>
                </div>
                <div class="row">
                  <div class="col-md-6">
                    <label class="form-label">Category</label>
                    <select v-model="product.category" class="form-select mb-3">
                      <option value="">Select category</option>
                      <option value="clothing">Clothing</option>
                      <option value="electronics">Electronics</option>
                      <option value="furniture">Furniture</option>
                      <option value="others">Others</option>
                    </select>
                  </div>
                  <div class="col-md-6">
                    <MDInput 
                      v-model="product.sku" 
                      label="SKU"
                      placeholder="eg. 12345"
                      full-width 
                      class="mb-3"
                    />
                  </div>
                </div>
                <div class="row">
                  <div class="col-12">
                    <label class="form-label">Description</label>
                    <textarea 
                      v-model="product.description" 
                      class="form-control" 
                      rows="5"
                      placeholder="Enter product description"
                    ></textarea>
                  </div>
                </div>
                
                <MDTypography variant="button" font-weight="bold" class="mt-4 mb-2">
                  Pricing
                </MDTypography>
                <div class="row mt-3">
                  <div class="col-md-4">
                    <MDInput 
                      v-model="product.price" 
                      type="number"
                      label="Price"
                      placeholder="99.00"
                      full-width 
                      class="mb-3"
                    />
                  </div>
                  <div class="col-md-4">
                    <MDInput 
                      v-model="product.comparePrice" 
                      type="number"
                      label="Compare at Price"
                      placeholder="129.00"
                      full-width 
                      class="mb-3"
                    />
                  </div>
                  <div class="col-md-4">
                    <MDInput 
                      v-model="product.costPrice" 
                      type="number"
                      label="Cost per Item"
                      placeholder="50.00"
                      full-width 
                      class="mb-3"
                    />
                  </div>
                </div>
                
                <MDTypography variant="button" font-weight="bold" class="mt-4 mb-2">
                  Inventory
                </MDTypography>
                <div class="row mt-3">
                  <div class="col-md-6">
                    <MDInput 
                      v-model="product.stock" 
                      type="number"
                      label="Stock Quantity"
                      placeholder="100"
                      full-width 
                      class="mb-3"
                    />
                  </div>
                  <div class="col-md-6">
                    <MDInput 
                      v-model="product.weight" 
                      type="number"
                      label="Weight (kg)"
                      placeholder="1.5"
                      full-width 
                      class="mb-3"
                    />
                  </div>
                </div>
              </div>
              
              <div class="col-md-4">
                <MDTypography variant="button" font-weight="bold" class="mb-3">
                  Product Images
                </MDTypography>
                <div class="border border-dashed rounded-3 p-4 text-center mb-3" style="border-color: #d2d6da; min-height: 200px;">
                  <Icon name="mdi:cloud-upload" size="48" class="text-secondary mb-2" />
                  <MDTypography variant="caption" color="text" class="d-block">
                    Drop files here or click to upload
                  </MDTypography>
                  <input type="file" multiple accept="image/*" class="d-none" />
                </div>
                
                <MDTypography variant="button" font-weight="bold" class="mb-3 mt-4">
                  Product Status
                </MDTypography>
                <div class="form-check mb-3">
                  <input 
                    v-model="product.active" 
                    class="form-check-input" 
                    type="checkbox" 
                    id="active"
                  >
                  <label class="form-check-label" for="active">
                    Active
                  </label>
                </div>
                <div class="form-check mb-3">
                  <input 
                    v-model="product.featured" 
                    class="form-check-input" 
                    type="checkbox" 
                    id="featured"
                  >
                  <label class="form-check-label" for="featured">
                    Featured Product
                  </label>
                </div>
              </div>
            </div>
            
            <div class="d-flex justify-content-end mt-4 gap-2">
              <MDButton color="light">
                Cancel
              </MDButton>
              <MDButton color="info" @click="saveProduct">
                Create Product
              </MDButton>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

definePageMeta({
  layout: 'default'
})

const product = ref({
  name: '',
  category: '',
  sku: '',
  description: '',
  price: '',
  comparePrice: '',
  costPrice: '',
  stock: '',
  weight: '',
  active: true,
  featured: false
})

const saveProduct = () => {
  console.log('Saving product:', product.value)
  alert('Product created successfully!')
  navigateTo('/ecommerce/products/product-page')
}
</script>

<style scoped>
.form-select {
  display: block;
  width: 100%;
  padding: 0.5rem 2.25rem 0.5rem 0.75rem;
  font-size: 0.875rem;
  font-weight: 400;
  line-height: 1.5;
  color: #495057;
  background-color: #fff;
  background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 16 16'%3e%3cpath fill='none' stroke='%23343a40' stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='M2 5l6 6 6-6'/%3e%3c/svg%3e");
  background-repeat: no-repeat;
  background-position: right 0.75rem center;
  background-size: 16px 12px;
  border: 1px solid #d2d6da;
  border-radius: 0.5rem;
  transition: border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out;
  appearance: none;
}

.form-select:focus {
  border-color: #5e72e4;
  outline: 0;
  box-shadow: 0 0 0 2px rgba(94, 114, 228, 0.1);
}

.form-control {
  display: block;
  width: 100%;
  padding: 0.5rem 0.75rem;
  font-size: 0.875rem;
  font-weight: 400;
  line-height: 1.5;
  color: #495057;
  background-color: #fff;
  border: 1px solid #d2d6da;
  border-radius: 0.5rem;
  transition: border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out;
}

.form-control:focus {
  color: #495057;
  background-color: #fff;
  border-color: #5e72e4;
  outline: 0;
  box-shadow: 0 0 0 2px rgba(94, 114, 228, 0.1);
}

.form-label {
  display: block;
  margin-bottom: 0.5rem;
  font-size: 0.875rem;
  font-weight: 600;
  color: #344767;
}

.form-check-input {
  width: 1.25rem;
  height: 1.25rem;
  margin-right: 0.5rem;
}

.form-check-label {
  font-size: 0.875rem;
  color: rgb(123, 128, 154);
}

.border-dashed {
  border-style: dashed !important;
}

.gap-2 {
  gap: 0.5rem;
}
</style>
