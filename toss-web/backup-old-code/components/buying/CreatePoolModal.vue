<template>
  <div v-if="show" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
    <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
      <div class="p-6 border-b border-slate-200 dark:border-slate-700">
        <h3 class="text-2xl font-semibold text-slate-900 dark:text-white">Create Group Buying Pool</h3>
        <p class="text-sm text-slate-600 dark:text-slate-400 mt-1">Invite nearby shops to buy together and save</p>
      </div>

      <form @submit.prevent="handleSubmit" class="p-6">
        <div class="space-y-6">
          <!-- Product Selection -->
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
              Product
            </label>
            <select 
              v-model="formData.productId"
              required
              class="w-full px-4 py-3 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-orange-500 dark:bg-slate-700 dark:text-white"
            >
              <option value="">Select a product...</option>
              <option v-for="product in products" :key="product.id" :value="product.id">
                {{ product.name }} - R{{ product.price.toFixed(2) }}
              </option>
            </select>
          </div>

          <!-- Target Quantity & Unit -->
          <div class="grid grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Target Quantity
              </label>
              <input 
                v-model.number="formData.targetQuantity"
                type="number"
                min="1"
                required
                placeholder="e.g., 24"
                class="w-full px-4 py-3 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-orange-500 dark:bg-slate-700 dark:text-white"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Unit
              </label>
              <select 
                v-model="formData.unit"
                required
                class="w-full px-4 py-3 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-orange-500 dark:bg-slate-700 dark:text-white"
              >
                <option value="units">Units</option>
                <option value="cases">Cases</option>
                <option value="crates">Crates</option>
                <option value="boxes">Boxes</option>
                <option value="kg">Kilograms</option>
              </select>
            </div>
          </div>

          <!-- Pricing -->
          <div class="grid grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Solo Unit Price
              </label>
              <input 
                v-model.number="formData.soloPrice"
                type="number"
                step="0.01"
                min="0"
                required
                placeholder="Current price"
                class="w-full px-4 py-3 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-orange-500 dark:bg-slate-700 dark:text-white"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Pool Unit Price
              </label>
              <input 
                v-model.number="formData.poolPrice"
                type="number"
                step="0.01"
                min="0"
                required
                placeholder="Discounted price"
                class="w-full px-4 py-3 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-orange-500 dark:bg-slate-700 dark:text-white"
              />
            </div>
          </div>

          <!-- Savings Preview -->
          <div v-if="savingsPercent > 0" class="p-4 bg-green-50 dark:bg-green-900/20 rounded-xl">
            <p class="text-sm text-green-800 dark:text-green-400">
              💰 <strong>{{ savingsPercent.toFixed(1) }}% savings</strong> vs solo purchase
            </p>
          </div>

          <!-- Area & Deadline -->
          <div class="grid grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Area
              </label>
              <select 
                v-model="formData.area"
                required
                class="w-full px-4 py-3 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-orange-500 dark:bg-slate-700 dark:text-white"
              >
                <option value="soweto">Soweto</option>
                <option value="alexandra">Alexandra</option>
                <option value="katlehong">Katlehong</option>
                <option value="diepsloot">Diepsloot</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Deadline
              </label>
              <input 
                v-model="formData.deadline"
                type="datetime-local"
                required
                :min="minDeadline"
                class="w-full px-4 py-3 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-orange-500 dark:bg-slate-700 dark:text-white"
              />
            </div>
          </div>

          <!-- Split Rule -->
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
              Cost Split Rule
            </label>
            <div class="grid grid-cols-2 gap-4">
              <button 
                type="button"
                @click="formData.splitRule = 'flat'"
                :class="[
                  'p-4 border-2 rounded-xl transition-all',
                  formData.splitRule === 'flat'
                    ? 'border-orange-500 bg-orange-50 dark:bg-orange-900/20'
                    : 'border-slate-300 dark:border-slate-600 hover:border-slate-400'
                ]"
              >
                <p class="font-semibold text-slate-900 dark:text-white">Flat Split</p>
                <p class="text-xs text-slate-600 dark:text-slate-400 mt-1">Equal share for all participants</p>
              </button>
              <button 
                type="button"
                @click="formData.splitRule = 'units'"
                :class="[
                  'p-4 border-2 rounded-xl transition-all',
                  formData.splitRule === 'units'
                    ? 'border-orange-500 bg-orange-50 dark:bg-orange-900/20'
                    : 'border-slate-300 dark:border-slate-600 hover:border-slate-400'
                ]"
              >
                <p class="font-semibold text-slate-900 dark:text-white">By Units</p>
                <p class="text-xs text-slate-600 dark:text-slate-400 mt-1">Pay based on quantity ordered</p>
              </button>
            </div>
          </div>

          <!-- Notes -->
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
              Notes (Optional)
            </label>
            <textarea 
              v-model="formData.notes"
              rows="3"
              placeholder="Any additional details..."
              class="w-full px-4 py-3 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-orange-500 dark:bg-slate-700 dark:text-white"
            ></textarea>
          </div>
        </div>

        <!-- Actions -->
        <div class="flex justify-end space-x-3 mt-8 pt-6 border-t border-slate-200 dark:border-slate-700">
          <button 
            type="button"
            @click="$emit('close')"
            class="px-6 py-3 text-slate-600 dark:text-slate-400 hover:text-slate-800 dark:hover:text-slate-200 font-medium"
          >
            Cancel
          </button>
          <button 
            type="submit"
            class="px-8 py-3 bg-gradient-to-r from-orange-600 to-purple-600 hover:from-orange-700 hover:to-purple-700 text-white rounded-xl font-semibold shadow-lg hover:shadow-xl transition-all duration-200"
          >
            Create Pool
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useSalesAPI } from '~/composables/useSalesAPI'

const props = defineProps<{
  show: boolean
}>()

const emit = defineEmits<{
  close: []
  create: [poolData: any]
}>()

const salesAPI = useSalesAPI()
const products = ref<any[]>([])

const formData = ref({
  productId: '',
  targetQuantity: 24,
  unit: 'crates',
  soloPrice: 0,
  poolPrice: 0,
  area: 'soweto',
  deadline: '',
  splitRule: 'units',
  notes: ''
})

const minDeadline = computed(() => {
  const now = new Date()
  now.setHours(now.getHours() + 2) // Minimum 2 hours from now
  return now.toISOString().slice(0, 16)
})

const savingsPercent = computed(() => {
  if (formData.value.soloPrice > 0 && formData.value.poolPrice > 0) {
    return ((formData.value.soloPrice - formData.value.poolPrice) / formData.value.soloPrice) * 100
  }
  return 0
})

onMounted(async () => {
  try {
    products.value = await salesAPI.getProducts()
  } catch (error) {
    console.error('Failed to load products:', error)
  }
})

const handleSubmit = () => {
  emit('create', {
    ...formData.value,
    deadline: new Date(formData.value.deadline)
  })
}
</script>

