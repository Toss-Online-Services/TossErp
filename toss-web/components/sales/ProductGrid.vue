<template>
  <div class="space-y-4">
    <!-- Search and Scanner -->
    <div class="bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm rounded-2xl shadow-lg border border-slate-200/50 dark:border-slate-700/50 p-4 sm:p-6">
      <div class="flex items-center space-x-3">
        <div class="flex-1 relative">
          <MagnifyingGlassIcon class="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-slate-400" />
          <input 
            v-model="localSearchQuery"
            type="text"
            placeholder="Scan barcode or search products..."
            class="w-full pl-10 pr-4 py-3 rounded-lg border border-gray-300 bg-white text-gray-900 focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            @keyup.enter="$emit('searchEnter')"
          />
        </div>
        <button 
          @click="$emit('openScanner')"
          class="p-3 rounded-xl transition-all duration-200 bg-gradient-to-r from-blue-600 to-purple-600 hover:from-blue-700 hover:to-purple-700 text-white shadow-lg hover:shadow-xl"
        >
          <QrCodeIcon class="w-6 h-6" />
        </button>
      </div>
    </div>

    <!-- Category Filters -->
    <div class="bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm rounded-2xl shadow-lg border border-slate-200/50 dark:border-slate-700/50 p-4 sm:p-6">
      <div class="flex flex-wrap gap-2">
        <button 
          v-for="category in categories" 
          :key="category.id"
          @click="localSelectedCategory = category.id"
          :class="[
            'px-4 py-2 rounded-lg text-sm font-medium transition-colors',
            localSelectedCategory === category.id
              ? 'bg-blue-600 text-white'
              : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
          ]"
        >
          {{ category.name }}
        </button>
      </div>
    </div>

    <!-- Products Grid -->
    <div class="bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm rounded-2xl shadow-lg border border-slate-200/50 dark:border-slate-700/50 p-4 sm:p-6">
      <div class="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 gap-4">
        <button 
          v-for="product in filteredProducts" 
          :key="product.id"
          @click="$emit('selectProduct', product)"
          :disabled="product.stock === 0"
          class="bg-white dark:bg-slate-700 rounded-xl border border-slate-200 dark:border-slate-600 p-3 hover:border-blue-500 hover:shadow-xl transition-all text-left disabled:opacity-50 disabled:cursor-not-allowed shadow-md"
        >
          <!-- Product Image -->
          <div class="aspect-square bg-slate-100 dark:bg-slate-600 rounded-lg mb-3 flex items-center justify-center overflow-hidden">
            <img 
              v-if="product.image" 
              :src="product.image" 
              :alt="product.name"
              class="w-full h-full object-cover"
            />
            <CubeIcon v-else class="w-10 h-10 sm:w-12 sm:h-12 text-slate-400 dark:text-slate-500" />
          </div>
          
          <!-- Product Name -->
          <h3 class="font-bold text-slate-900 dark:text-white text-sm sm:text-base truncate mb-1">{{ product.name }}</h3>
          
          <!-- SKU -->
          <p class="text-xs text-slate-500 dark:text-slate-400 truncate mb-3">{{ product.sku }}</p>
          
          <!-- Price and Stock -->
          <div class="flex items-center justify-between">
            <span class="text-sm sm:text-base font-bold text-blue-600 dark:text-blue-400">R{{ product.price.toFixed(2) }}</span>
            <span 
              :class="[
                'text-xs font-medium px-2 py-1 rounded-full',
                product.stock >= 10 
                  ? 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400' 
                  : product.stock > 0
                    ? 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400'
                    : 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400'
              ]"
            >
              Stock: {{ product.stock }}
            </span>
          </div>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { MagnifyingGlassIcon, QrCodeIcon, CubeIcon } from '@heroicons/vue/24/outline'

interface Product {
  id: number
  name: string
  sku: string
  price: number
  stock: number
  category: string
  image?: string | null
}

interface Category {
  id: string
  name: string
}

interface Props {
  products: Product[]
  categories: Category[]
  searchQuery?: string
  selectedCategory?: string
}

const props = withDefaults(defineProps<Props>(), {
  searchQuery: '',
  selectedCategory: 'all'
})

const emit = defineEmits<{
  'update:searchQuery': [value: string]
  'update:selectedCategory': [value: string]
  'selectProduct': [product: Product]
  'searchEnter': []
  'openScanner': []
}>()

// Local state synced with props
const localSearchQuery = ref(props.searchQuery)
const localSelectedCategory = ref(props.selectedCategory)

// Watch for prop changes
watch(() => props.searchQuery, (newVal) => {
  localSearchQuery.value = newVal
})

watch(() => props.selectedCategory, (newVal) => {
  localSelectedCategory.value = newVal
})

// Emit changes
watch(localSearchQuery, (newVal) => {
  emit('update:searchQuery', newVal)
})

watch(localSelectedCategory, (newVal) => {
  emit('update:selectedCategory', newVal)
})

// Filtered products
const filteredProducts = computed(() => {
  let filtered = props.products

  if (localSelectedCategory.value !== 'all') {
    filtered = filtered.filter((p: Product) => p.category === localSelectedCategory.value)
  }

  if (localSearchQuery.value) {
    filtered = filtered.filter((p: Product) => 
      p.name.toLowerCase().includes(localSearchQuery.value.toLowerCase()) ||
      p.sku.toLowerCase().includes(localSearchQuery.value.toLowerCase())
    )
  }

  return filtered
})
</script>

