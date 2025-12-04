<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useStockStore, type Item, type StockMovement } from '~/stores/stock'
import { useBuyingStore } from '~/stores/buying'

interface Props {
  show: boolean
  itemId?: string
  item?: Item | null
}

const props = withDefaults(defineProps<Props>(), {
  itemId: '',
  item: null
})

const emit = defineEmits<{
  close: []
  edit: [item: Item]
  adjust: [item: Item]
}>()

const stockStore = useStockStore()
const buyingStore = useBuyingStore()

const item = ref<Item | null>(null)
const movements = ref<StockMovement[]>([])
const supplier = ref<any>(null)
const allSuppliers = ref<any[]>([])
const loading = ref(false)
const mainImage = ref<string>('')

const movementTypes = {
  purchase: { label: 'Purchase', color: 'text-green-600 bg-green-100', icon: 'shopping_cart' },
  sale: { label: 'Sale', color: 'text-blue-600 bg-blue-100', icon: 'point_of_sale' },
  adjustment: { label: 'Adjustment', color: 'text-orange-600 bg-orange-100', icon: 'tune' },
  transfer: { label: 'Transfer', color: 'text-purple-600 bg-purple-100', icon: 'swap_horiz' },
  return: { label: 'Return', color: 'text-gray-600 bg-gray-100', icon: 'undo' }
}

watch(() => props.show, async (newVal) => {
  if (newVal) {
    await loadItem()
  }
}, { immediate: true })

watch(() => props.item, async (newItem) => {
  if (newItem) {
    item.value = newItem
    await loadItemData()
  }
}, { immediate: true })

watch(() => props.itemId, async (newId) => {
  if (newId && props.show) {
    await loadItem()
  }
}, { immediate: true })

async function loadItem() {
  if (props.item) {
    item.value = props.item
    await loadItemData()
    return
  }
  
  if (!props.itemId) return
  
  loading.value = true
  try {
    await stockStore.fetchItems()
    await buyingStore.fetchSuppliers()
    
    const foundItem = stockStore.getItemById(props.itemId)
    item.value = foundItem || null
    
    if (!item.value) {
      console.error('Item not found for ID:', props.itemId)
      return
    }
    
    await loadItemData()
  } catch (error) {
    console.error('Failed to load item:', error)
  } finally {
    loading.value = false
  }
}

async function loadItemData() {
  if (!item.value) return
  
  // Set initial main image
  mainImage.value = item.value.imageUrl || getRandomProductImage()
  
  // Find primary supplier for this item
  if (item.value.supplier) {
    supplier.value = buyingStore.suppliers.find(s => 
      s.name.toLowerCase() === item.value?.supplier?.toLowerCase()
    ) || null
  }
  
  // Load movements and purchase orders
  await loadMovements()
  await buyingStore.fetchPurchaseOrders()
  
  // Find all suppliers who have supplied this item
  const supplierMap = new Map<string, any>()
  
  // Get suppliers from purchase movements
  movements.value
    .filter(m => m.type === 'purchase' && m.supplierName)
    .forEach(movement => {
      if (movement.supplierName && !supplierMap.has(movement.supplierName)) {
        const foundSupplier = buyingStore.suppliers.find(s => 
          s.name.toLowerCase() === movement.supplierName?.toLowerCase()
        )
        if (foundSupplier) {
          supplierMap.set(movement.supplierName, foundSupplier)
        }
      }
    })
  
  // Also check purchase orders for this item
  buyingStore.purchaseOrders.forEach(po => {
    const hasItem = po.items.some(poItem => poItem.itemId === item.value?.id)
    if (hasItem && po.supplierName && !supplierMap.has(po.supplierName)) {
      const foundSupplier = buyingStore.suppliers.find(s => 
        s.name.toLowerCase() === po.supplierName.toLowerCase()
      )
      if (foundSupplier) {
        supplierMap.set(po.supplierName, foundSupplier)
      }
    }
  })
  
  // Convert map to array
  allSuppliers.value = Array.from(supplierMap.values())
  
  // If no suppliers found from movements/POs but item has a supplier, add it
  if (supplier.value && !allSuppliers.value.find(s => s.id === supplier.value.id)) {
    allSuppliers.value.push(supplier.value)
  }
}

async function loadMovements() {
  if (!item.value) return
  
  try {
    await stockStore.fetchMovements(item.value.id)
    movements.value = stockStore.movements.filter(m => m.itemId === item.value?.id)
    
    // Enhance movements with supplier info from purchase orders if missing
    await buyingStore.fetchPurchaseOrders()
    movements.value = movements.value.map(movement => {
      if (movement.type === 'purchase' && movement.referenceType === 'PO' && movement.referenceId && !movement.supplierName) {
        const po = buyingStore.purchaseOrders.find(po => 
          po.poNumber === movement.referenceId || po.id === movement.referenceId
        )
        if (po) {
          movement.supplierId = po.supplierId
          movement.supplierName = po.supplierName
        }
      }
      return movement
    })
  } catch (error) {
    console.error('Failed to load movements:', error)
  }
}

function formatCurrency(amount: number) {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR'
  }).format(amount)
}

function formatDate(date: Date) {
  return new Intl.DateTimeFormat('en-ZA', {
    dateStyle: 'medium',
    timeStyle: 'short'
  }).format(new Date(date))
}

function getStockStatus(item: Item) {
  if (item.currentStock === 0) return { text: 'Out of Stock', color: 'text-red-600 bg-red-100' }
  if (item.currentStock <= item.minStock) return { text: 'Low Stock', color: 'text-orange-600 bg-orange-100' }
  return { text: 'In Stock', color: 'text-green-600 bg-green-100' }
}

function handleAdjust() {
  if (item.value) {
    emit('adjust', item.value)
  }
}

function handleEdit() {
  if (item.value) {
    emit('edit', item.value)
    emit('close')
  }
}

function handleClose() {
  emit('close')
}

function getRandomProductImage() {
  if (!item.value) return '/images/products/product-1-min.jpg'
  
  const images = [
    '/images/products/product-details-1.jpg',
    '/images/products/product-details-2.jpg',
    '/images/products/product-details-3.jpg',
    '/images/products/product-details-4.jpg',
    '/images/products/product-details-5.jpg',
    '/images/products/product-1-min.jpg',
    '/images/products/product-2-min.jpg',
    '/images/products/product-3-min.jpg',
    '/images/products/product-4-min.jpg',
    '/images/products/product-5-min.jpg',
    '/images/products/product-6-min.jpg',
    '/images/products/product-7-min.jpg',
    '/images/products/product-11.jpg'
  ]
  const index = parseInt(item.value.id) % images.length
  return images[index]
}

function getThumbnailImages() {
  const allImages = [
    '/images/products/product-details-1.jpg',
    '/images/products/product-details-2.jpg',
    '/images/products/product-details-3.jpg',
    '/images/products/product-details-4.jpg',
    '/images/products/product-details-5.jpg'
  ]
  return allImages.slice(0, 4)
}

function setMainImage(imageSrc: string) {
  if (item.value) {
    mainImage.value = imageSrc
  }
}
</script>

<template>
  <Teleport to="body">
    <Transition name="modal">
      <div
        v-if="show"
        class="overflow-y-auto fixed inset-0 z-50"
        @click.self="handleClose"
        style="background-color: rgba(0, 0, 0, 0.5);"
      >
        <div class="flex justify-center items-start p-4 min-h-screen pt-8">
          <!-- Modal Container -->
          <div
            class="overflow-y-auto relative w-full max-w-6xl bg-white rounded-xl shadow-xl"
            style="max-height: 90vh;"
            @click.stop
          >
            <!-- Header -->
            <div class="sticky top-0 bg-white border-b border-gray-200 px-6 py-4 z-10">
              <div class="flex items-center justify-between">
                <h3 class="text-xl font-semibold text-gray-900">Item Details</h3>
                <button
                  @click="handleClose"
                  class="p-1 text-gray-400 hover:text-gray-600 transition-colors rounded-full hover:bg-gray-100"
                  type="button"
                  aria-label="Close modal"
                >
                  <i class="text-2xl material-symbols-rounded">close</i>
                </button>
              </div>
            </div>

            <!-- Content -->
            <div class="p-6">
              <!-- Loading State -->
              <div v-if="loading" class="text-center py-12">
                <i class="material-symbols-rounded text-6xl text-gray-400 animate-spin">refresh</i>
                <p class="mt-4 text-gray-600">Loading item details...</p>
              </div>

              <!-- Item Details -->
              <div v-else-if="item">
                <div class="grid grid-cols-1 lg:grid-cols-12 gap-6">
                  <!-- Left Column: Item Images -->
                  <div class="lg:col-span-5 text-center">
                    <img 
                      :src="mainImage || item.imageUrl || getRandomProductImage()" 
                      :alt="item.name"
                      class="w-full rounded-xl shadow-lg mx-auto object-cover"
                      style="max-height: 400px;"
                    />
                    <!-- Thumbnail Gallery -->
                    <div class="flex gap-3 mt-4 pt-2 justify-center">
                      <figure 
                        v-for="(thumb, index) in getThumbnailImages()" 
                        :key="index"
                        class="cursor-pointer hover:opacity-80 transition-opacity"
                        @click="setMainImage(thumb)"
                      >
                        <img 
                          :src="thumb" 
                          :alt="`${item.name} - View ${index + 1}`"
                          class="rounded-lg shadow object-cover"
                          style="width: 80px; height: 80px;"
                        />
                      </figure>
                    </div>
                  </div>
                  
                  <!-- Right Column: Item Information -->
                  <div class="lg:col-span-7">
                    <div class="flex items-start justify-between mb-4">
                      <h3 class="text-2xl font-bold text-gray-900">{{ item.name }}</h3>
                      <button
                        @click="handleEdit"
                        class="px-3 py-1.5 text-sm bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors flex items-center gap-2"
                        type="button"
                      >
                        <i class="material-symbols-rounded text-base">edit</i>
                        Edit
                      </button>
                    </div>
                    
                    <div class="mb-3 mt-2">
                      <p class="text-sm text-gray-600 mb-1">Code: {{ item.code }}</p>
                      <p v-if="item.barcode" class="text-sm text-gray-600 mb-1">Barcode: {{ item.barcode }}</p>
                    </div>
                    
                    <div class="mb-3">
                      <h6 class="mb-0 text-sm font-semibold text-gray-700">Price</h6>
                      <h5 class="text-xl font-bold text-gray-900 mt-1">{{ formatCurrency(item.sellingPrice) }}</h5>
                      <p class="text-sm text-gray-600 mt-1">Cost: {{ formatCurrency(item.costPrice) }}</p>
                    </div>
                    
                    <div class="mb-3">
                      <span 
                        :class="[
                          'px-3 py-1 text-xs font-semibold rounded-full',
                          getStockStatus(item).color === 'text-red-600 bg-red-100' ? 'bg-red-100 text-red-600' :
                          getStockStatus(item).color === 'text-orange-600 bg-orange-100' ? 'bg-orange-100 text-orange-600' :
                          'bg-green-100 text-green-600'
                        ]"
                      >
                        {{ getStockStatus(item).text }}
                      </span>
                    </div>
                    
                    <div class="mb-4 mt-4">
                      <label class="text-sm font-semibold text-gray-700 mb-2 block">Description</label>
                      <ul class="list-disc list-inside text-sm text-gray-600 space-y-1">
                        <li>Category: {{ item.category }}</li>
                        <li>Unit: {{ item.unit }}</li>
                        <li>Current Stock: {{ item.currentStock }} {{ item.unit }}</li>
                        <li v-if="item.minStock > 0">Minimum Stock: {{ item.minStock }} {{ item.unit }}</li>
                        <li>Stock Value: {{ formatCurrency(item.currentStock * item.costPrice) }}</li>
                      </ul>
                    </div>
                    
                    <!-- Supplier Information -->
                    <div v-if="supplier || item.supplier" class="mb-4 pt-4 border-t border-gray-200">
                      <label class="text-sm font-semibold text-gray-700 mb-2 block">Supplier</label>
                      <div v-if="supplier" class="space-y-2">
                        <div>
                          <NuxtLink
                            :to="`/buying/suppliers/${supplier.id}`"
                            class="text-base font-semibold text-gray-900 hover:text-gray-700 hover:underline flex items-center gap-2"
                            @click="handleClose"
                          >
                            <i class="material-symbols-rounded text-xl">store</i>
                            {{ supplier.name }}
                            <i class="material-symbols-rounded text-sm">open_in_new</i>
                          </NuxtLink>
                        </div>
                        <div v-if="supplier.phone" class="flex items-center gap-2 text-sm text-gray-600">
                          <i class="material-symbols-rounded text-base">phone</i>
                          <a :href="`tel:${supplier.phone}`" class="hover:text-gray-900 hover:underline">
                            {{ supplier.phone }}
                          </a>
                        </div>
                        <div v-if="supplier.email" class="flex items-center gap-2 text-sm text-gray-600">
                          <i class="material-symbols-rounded text-base">email</i>
                          <a :href="`mailto:${supplier.email}`" class="hover:text-gray-900 hover:underline">
                            {{ supplier.email }}
                          </a>
                        </div>
                      </div>
                      <div v-else class="text-sm text-gray-600">{{ item.supplier }}</div>
                    </div>
                    
                    <div class="mt-4">
                      <button
                        @click="handleAdjust"
                        class="w-full px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors font-medium"
                        type="button"
                      >
                        <i class="material-symbols-rounded align-middle mr-2">tune</i>
                        Adjust Stock
                      </button>
                    </div>
                  </div>
                </div>

                <!-- Stock Movements History -->
                <div class="mt-6 border-t border-gray-200 pt-6">
                  <h5 class="mb-4 text-lg font-semibold text-gray-900">Stock Movement History</h5>
                  <div class="overflow-x-auto">
                    <table class="min-w-full divide-y divide-gray-200">
                      <thead class="bg-gray-50">
                        <tr>
                          <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Date</th>
                          <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Type</th>
                          <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Quantity</th>
                          <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Supplier</th>
                          <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Reference</th>
                          <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Notes</th>
                        </tr>
                      </thead>
                      <tbody class="bg-white divide-y divide-gray-200">
                        <tr v-if="loading">
                          <td colspan="6" class="px-4 py-8 text-center text-gray-500">
                            Loading movements...
                          </td>
                        </tr>
                        <tr v-else-if="movements.length === 0">
                          <td colspan="6" class="px-4 py-8 text-center text-gray-500">
                            No stock movements recorded yet
                          </td>
                        </tr>
                        <tr v-for="movement in movements" :key="movement.id" class="hover:bg-gray-50">
                          <td class="px-4 py-4 whitespace-nowrap text-sm text-gray-900">
                            {{ formatDate(movement.createdAt) }}
                          </td>
                          <td class="px-4 py-4 whitespace-nowrap">
                            <span
                              :class="[
                                'px-2 py-1 inline-flex text-xs leading-5 font-semibold rounded-full',
                                movementTypes[movement.type]?.color || 'text-gray-600 bg-gray-100'
                              ]"
                            >
                              <i class="material-symbols-rounded text-sm mr-1">
                                {{ movementTypes[movement.type]?.icon || 'help' }}
                              </i>
                              {{ movementTypes[movement.type]?.label || movement.type }}
                            </span>
                          </td>
                          <td class="px-4 py-4 whitespace-nowrap text-sm font-semibold"
                              :class="movement.quantity > 0 ? 'text-green-600' : 'text-red-600'">
                            {{ movement.quantity > 0 ? '+' : '' }}{{ movement.quantity }} {{ item.unit }}
                          </td>
                          <td class="px-4 py-4 whitespace-nowrap text-sm text-gray-900">
                            <div v-if="movement.type === 'purchase' && movement.supplierName">
                              <NuxtLink
                                v-if="movement.supplierId"
                                :to="`/buying/suppliers/${movement.supplierId}`"
                                class="text-gray-900 hover:text-gray-700 hover:underline flex items-center gap-1"
                                @click="handleClose"
                              >
                                <i class="material-symbols-rounded text-base">store</i>
                                {{ movement.supplierName }}
                              </NuxtLink>
                              <span v-else class="flex items-center gap-1">
                                <i class="material-symbols-rounded text-base">store</i>
                                {{ movement.supplierName }}
                              </span>
                            </div>
                            <span v-else class="text-gray-400">-</span>
                          </td>
                          <td class="px-4 py-4 whitespace-nowrap text-sm text-gray-900">
                            <span v-if="movement.referenceType && movement.referenceId">
                              {{ movement.referenceType }} #{{ movement.referenceId }}
                            </span>
                            <span v-else class="text-gray-400">-</span>
                          </td>
                          <td class="px-4 py-4 text-sm text-gray-500">
                            {{ movement.notes || '-' }}
                          </td>
                        </tr>
                      </tbody>
                    </table>
                  </div>
                </div>

                <!-- All Suppliers Section -->
                <div v-if="allSuppliers.length > 0" class="mt-6 border-t border-gray-200 pt-6">
                  <h5 class="mb-2 text-lg font-semibold text-gray-900 flex items-center gap-2">
                    <i class="material-symbols-rounded text-xl">store</i>
                    All Suppliers for This Item
                  </h5>
                  <p class="text-sm text-gray-600 mb-4">Suppliers who have supplied this product</p>
                  <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                    <div
                      v-for="supp in allSuppliers"
                      :key="supp.id"
                      class="border border-gray-200 rounded-lg p-4 hover:border-gray-300 hover:shadow-md transition-all"
                    >
                      <div class="flex items-start justify-between mb-3">
                        <NuxtLink
                          :to="`/buying/suppliers/${supp.id}`"
                          class="flex items-center gap-2 text-base font-semibold text-gray-900 hover:text-gray-700 hover:underline flex-1"
                          @click="handleClose"
                        >
                          <i class="material-symbols-rounded text-xl">store</i>
                          {{ supp.name }}
                          <i class="material-symbols-rounded text-sm">open_in_new</i>
                        </NuxtLink>
                        <span
                          v-if="supp.id === supplier?.id"
                          class="px-2 py-0.5 text-xs font-medium rounded-full bg-gray-100 text-gray-700"
                        >
                          Primary
                        </span>
                      </div>
                      <div class="space-y-2 text-sm text-gray-600">
                        <div v-if="supp.code" class="flex items-center gap-1">
                          <i class="material-symbols-rounded text-base">tag</i>
                          <span>{{ supp.code }}</span>
                        </div>
                        <div v-if="supp.phone" class="flex items-center gap-1">
                          <i class="material-symbols-rounded text-base">phone</i>
                          <a :href="`tel:${supp.phone}`" class="hover:text-gray-900 hover:underline">
                            {{ supp.phone }}
                          </a>
                        </div>
                        <div v-if="supp.email" class="flex items-center gap-1">
                          <i class="material-symbols-rounded text-base">email</i>
                          <a :href="`mailto:${supp.email}`" class="hover:text-gray-900 hover:underline">
                            {{ supp.email }}
                          </a>
                        </div>
                        <div v-if="supp.paymentTerms" class="flex items-center gap-1">
                          <i class="material-symbols-rounded text-base">account_balance</i>
                          <span>{{ supp.paymentTerms }}</span>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Item Not Found -->
              <div v-else class="text-center py-12">
                <i class="material-symbols-rounded text-6xl text-gray-400 mb-4">error</i>
                <h3 class="text-lg font-medium text-gray-900 mb-2">Item not found</h3>
                <button
                  @click="handleClose"
                  class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800"
                >
                  <i class="material-symbols-rounded">arrow_back</i>
                  <span>Close</span>
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Transition>

  </Teleport>
</template>

<style scoped>
.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-active > div > div,
.modal-leave-active > div > div {
  transition: transform 0.3s ease, opacity 0.3s ease;
}

.modal-enter-from > div > div,
.modal-leave-to > div > div {
  transform: scale(0.95);
  opacity: 0;
}
</style>

