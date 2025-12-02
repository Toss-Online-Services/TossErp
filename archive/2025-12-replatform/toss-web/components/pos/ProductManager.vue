<template>
  <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700 p-4 sm:p-6">
    <div class="flex items-center justify-between mb-4">
      <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Product Management</h3>
      <button 
        @click="showAddProduct = true"
        class="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg text-sm font-medium transition-colors"
      >
        <PlusIcon class="w-4 h-4 inline mr-2" />
        Add Product
      </button>
    </div>

    <!-- Product List -->
    <div class="space-y-3">
      <div 
        v-for="product in products" 
        :key="product.id"
        class="flex items-center justify-between p-3 bg-slate-50 dark:bg-slate-700 rounded-lg"
      >
        <div class="flex items-center space-x-3">
          <div class="w-10 h-10 bg-slate-200 dark:bg-slate-600 rounded-lg flex items-center justify-center">
            <CubeIcon class="w-5 h-5 text-slate-400" />
          </div>
          <div>
            <h4 class="font-medium text-slate-900 dark:text-white text-sm">{{ product.name }}</h4>
            <p class="text-xs text-slate-500 dark:text-slate-400">{{ product.sku }} • R{{ product.price.toFixed(2) }}</p>
          </div>
        </div>
        <div class="flex items-center space-x-2">
          <span 
            :class="[
              'text-xs px-2 py-1 rounded-full',
              product.stock > 10 
                ? 'bg-green-100 dark:bg-green-900 text-green-700 dark:text-green-300' 
                : product.stock > 0
                  ? 'bg-yellow-100 dark:bg-yellow-900 text-yellow-700 dark:text-yellow-300'
                  : 'bg-red-100 dark:bg-red-900 text-red-700 dark:text-red-300'
            ]"
          >
            {{ product.stock }}
          </span>
          <button 
            @click="editProduct(product)"
            class="p-1 text-slate-400 hover:text-slate-600 dark:hover:text-slate-300"
          >
            <PencilIcon class="w-4 h-4" />
          </button>
        </div>
      </div>
    </div>

    <!-- Add/Edit Product Modal -->
    <div v-if="showAddProduct || editingProduct" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
      <div class="bg-white dark:bg-slate-800 rounded-xl p-6 max-w-md w-full">
        <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-4">
          {{ editingProduct ? 'Edit Product' : 'Add Product' }}
        </h3>
        
        <form @submit.prevent="saveProduct" class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Product Name</label>
            <input 
              v-model="productForm.name"
              type="text"
              required
              class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
            />
          </div>
          
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">SKU</label>
            <input 
              v-model="productForm.sku"
              type="text"
              required
              class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
            />
          </div>
          
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Price</label>
            <input 
              v-model="productForm.price"
              type="number"
              step="0.01"
              required
              class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
            />
          </div>
          
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Stock</label>
            <input 
              v-model="productForm.stock"
              type="number"
              required
              class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
            />
          </div>
          
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Category</label>
            <select 
              v-model="productForm.category"
              class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
            >
              <option value="groceries">Groceries</option>
              <option value="beverages">Beverages</option>
              <option value="snacks">Snacks</option>
              <option value="household">Household</option>
              <option value="personal">Personal Care</option>
              <option value="frozen">Frozen</option>
            </select>
          </div>
          
          <div class="flex space-x-3 pt-4">
            <button 
              type="submit"
              class="flex-1 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg font-medium transition-colors"
            >
              {{ editingProduct ? 'Update' : 'Add' }} Product
            </button>
            <button 
              type="button"
              @click="cancelEdit"
              class="flex-1 py-2 bg-slate-600 hover:bg-slate-700 text-white rounded-lg font-medium transition-colors"
            >
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { PlusIcon, CubeIcon, PencilIcon } from '@heroicons/vue/24/outline'

interface Product {
  id: number
  name: string
  sku: string
  price: number
  stock: number
  category: string
}

const props = defineProps<{
  products: Product[]
}>()

const emit = defineEmits<{
  'update:products': [products: Product[]]
}>()

const showAddProduct = ref(false)
const editingProduct = ref<Product | null>(null)

const productForm = reactive({
  name: '',
  sku: '',
  price: 0,
  stock: 0,
  category: 'groceries'
})

const editProduct = (product: Product) => {
  editingProduct.value = product
  productForm.name = product.name
  productForm.sku = product.sku
  productForm.price = product.price
  productForm.stock = product.stock
  productForm.category = product.category
}

const saveProduct = () => {
  const updatedProducts = [...props.products]
  
  if (editingProduct.value) {
    // Update existing product
    const index = updatedProducts.findIndex(p => p.id === editingProduct.value!.id)
    if (index !== -1) {
      updatedProducts[index] = {
        ...editingProduct.value,
        ...productForm
      }
    }
  } else {
    // Add new product
    const newProduct: Product = {
      id: Date.now(),
      ...productForm
    }
    updatedProducts.push(newProduct)
  }
  
  emit('update:products', updatedProducts)
  cancelEdit()
}

const cancelEdit = () => {
  showAddProduct.value = false
  editingProduct.value = null
  productForm.name = ''
  productForm.sku = ''
  productForm.price = 0
  productForm.stock = 0
  productForm.category = 'groceries'
}
</script>
