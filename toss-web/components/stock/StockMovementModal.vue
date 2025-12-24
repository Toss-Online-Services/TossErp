<template>
  <div class="fixed inset-0 z-50 flex items-center justify-center p-4">
    <div class="absolute inset-0 bg-black/50 backdrop-blur-sm" @click="$emit('close')" />

    <div class="relative w-full max-w-2xl bg-white dark:bg-slate-900 rounded-2xl shadow-2xl overflow-hidden">
      <div :class="['px-6 py-5 text-white', headerGradientClass]">
        <button
          type="button"
          class="absolute top-4 right-4 p-2 rounded-lg text-white/80 hover:text-white hover:bg-white/10"
          aria-label="Close"
          @click="$emit('close')"
        >
          <svg viewBox="0 0 24 24" class="h-5 w-5" fill="none" stroke="currentColor" stroke-width="2">
            <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>

        <div class="flex items-start gap-3">
          <div class="mt-0.5">
            <component :is="headerIcon" class="h-6 w-6" />
          </div>
          <div>
            <h2 class="text-lg font-semibold leading-tight">{{ headerTitle }}</h2>
            <p class="text-sm text-white/90">{{ headerSubtitle }}</p>
          </div>
        </div>
      </div>

      <form class="px-6 py-6 space-y-4" @submit.prevent="submit">
        <div>
          <label for="item" class="block text-sm font-medium text-slate-700 dark:text-slate-200">
            Item <span class="text-red-500">*</span>
          </label>
          <select
            id="item"
            v-model="formData.itemCode"
            required
            class="mt-1 w-full rounded-lg border border-slate-200 dark:border-slate-700 bg-white dark:bg-slate-900 px-3 py-2 text-sm"
          >
            <option value="" disabled>Select an item</option>
            <option v-for="item in items" :key="item.sku" :value="item.sku">
              {{ item.name }}
            </option>
          </select>
        </div>

        <div>
          <label for="quantity" class="block text-sm font-medium text-slate-700 dark:text-slate-200">
            Quantity <span class="text-red-500">*</span>
          </label>
          <input
            id="quantity"
            v-model.number="formData.quantity"
            required
            type="number"
            min="1"
            class="mt-1 w-full rounded-lg border border-slate-200 dark:border-slate-700 bg-white dark:bg-slate-900 px-3 py-2 text-sm"
          />
        </div>

        <div v-if="movementType === 'transfer'" class="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <div>
            <label for="fromLocation" class="block text-sm font-medium text-slate-700 dark:text-slate-200">
              From Location <span class="text-red-500">*</span>
            </label>
            <select
              id="fromLocation"
              v-model="formData.fromLocation"
              required
              class="mt-1 w-full rounded-lg border border-slate-200 dark:border-slate-700 bg-white dark:bg-slate-900 px-3 py-2 text-sm"
            >
              <option value="" disabled>Select</option>
              <option v-for="loc in locations" :key="loc" :value="loc">{{ loc }}</option>
            </select>
          </div>

          <div>
            <label for="toLocation" class="block text-sm font-medium text-slate-700 dark:text-slate-200">
              To Location <span class="text-red-500">*</span>
            </label>
            <select
              id="toLocation"
              v-model="formData.toLocation"
              required
              class="mt-1 w-full rounded-lg border border-slate-200 dark:border-slate-700 bg-white dark:bg-slate-900 px-3 py-2 text-sm"
            >
              <option value="" disabled>Select</option>
              <option v-for="loc in locations" :key="loc" :value="loc">{{ loc }}</option>
            </select>
          </div>
        </div>

        <div>
          <label for="reference" class="block text-sm font-medium text-slate-700 dark:text-slate-200">Reference</label>
          <input
            id="reference"
            v-model="formData.reference"
            type="text"
            class="mt-1 w-full rounded-lg border border-slate-200 dark:border-slate-700 bg-white dark:bg-slate-900 px-3 py-2 text-sm"
          />
        </div>

        <div>
          <label for="notes" class="block text-sm font-medium text-slate-700 dark:text-slate-200">
            Notes
            <span v-if="movementType === 'adjustment'" class="text-red-500">*</span>
          </label>
          <textarea
            id="notes"
            v-model="formData.notes"
            :required="movementType === 'adjustment'"
            rows="3"
            class="mt-1 w-full rounded-lg border border-slate-200 dark:border-slate-700 bg-white dark:bg-slate-900 px-3 py-2 text-sm"
          />
        </div>

        <div class="pt-2 flex items-center justify-end gap-2">
          <button
            type="button"
            class="px-4 py-2 rounded-lg bg-slate-100 dark:bg-slate-800 text-slate-700 dark:text-slate-200 text-sm font-medium"
            @click="$emit('close')"
          >
            Cancel
          </button>
          <button
            type="submit"
            :class="['px-4 py-2 rounded-lg text-white text-sm font-medium bg-gradient-to-r', submitGradientClass]"
          >
            {{ submitLabel }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { useStock } from '../../composables/useStock'

type MovementType = 'receipt' | 'issue' | 'transfer' | 'adjustment'

export default defineComponent({
  name: 'StockMovementModal',
  props: {
    movementType: {
      type: String,
      required: true
    }
  },
  emits: ['close', 'save'],
  data() {
    return {
      items: [] as Array<{ sku: string; name: string; category?: string; sellingPrice?: number }>,
      locations: ['Main Warehouse', 'Shop Floor', 'Returns Area'],
      formData: {
        itemCode: '',
        quantity: 1,
        reference: '',
        notes: '',
        fromLocation: '',
        toLocation: ''
      }
    }
  },
  computed: {
    headerTitle(): string {
      switch (this.movementType as MovementType) {
        case 'receipt':
          return 'Stock IN ↓'
        case 'issue':
          return 'Stock OUT ↑'
        case 'transfer':
          return 'Stock MOVED →'
        case 'adjustment':
          return 'Stock FIXED ⇌'
        default:
          return 'Stock Movement'
      }
    },
    headerSubtitle(): string {
      switch (this.movementType as MovementType) {
        case 'receipt':
          return 'Record new inventory received'
        case 'issue':
          return 'Record inventory removed or sold'
        case 'transfer':
          return 'Move stock between locations'
        case 'adjustment':
          return 'Correct inventory discrepancies'
        default:
          return ''
      }
    },
    headerGradientClass(): string {
      switch (this.movementType as MovementType) {
        case 'receipt':
          return 'bg-gradient-to-r from-green-500 to-emerald-600'
        case 'issue':
          return 'bg-gradient-to-r from-red-500 to-red-600'
        case 'transfer':
          return 'bg-gradient-to-r from-blue-500 to-purple-600'
        case 'adjustment':
          return 'bg-gradient-to-r from-orange-500 to-yellow-500'
        default:
          return 'bg-gradient-to-r from-slate-700 to-slate-900'
      }
    },
    submitGradientClass(): string {
      switch (this.movementType as MovementType) {
        case 'receipt':
          return 'from-green-600 to-emerald-700'
        case 'issue':
          return 'from-red-600 to-red-700'
        case 'transfer':
          return 'from-blue-600 to-indigo-700'
        case 'adjustment':
          return 'from-orange-600 to-yellow-600'
        default:
          return 'from-slate-700 to-slate-900'
      }
    },
    submitLabel(): string {
      switch (this.movementType as MovementType) {
        case 'receipt':
          return 'Record Receipt'
        case 'issue':
          return 'Record Issue'
        case 'transfer':
          return 'Record Transfer'
        case 'adjustment':
          return 'Record Adjustment'
        default:
          return 'Save'
      }
    },
    headerIcon() {
      // Render an SVG element so tests can verify it exists.
      return {
        template:
          '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path stroke-linecap="round" stroke-linejoin="round" d="M12 4v16m-6-6l6 6 6-6"/></svg>'
      }
    }
  },
  watch: {
    movementType() {
      this.resetForm()
    }
  },
  async mounted() {
    await this.loadItems()
  },
  methods: {
    resetForm() {
      this.formData = {
        itemCode: '',
        quantity: 1,
        reference: '',
        notes: '',
        fromLocation: '',
        toLocation: ''
      }
    },
    async loadItems() {
      try {
        const stock: any = useStock()

        // Support both legacy `getItems()` (used in tests) and current `getProducts()`.
        const result = stock.getItems
          ? await stock.getItems()
          : await stock.getProducts({ pageNumber: 1, pageSize: 200 })

        const rawItems = result?.items || result?.products || []
        this.items = rawItems.map((it: any) => ({
          sku: it.sku,
          name: it.name,
          category: it.category,
          sellingPrice: it.sellingPrice
        }))
      } catch {
        this.items = []
      }
    },
    submit() {
      const movementType = this.movementType as MovementType

      if (!this.formData.itemCode || !this.formData.quantity || this.formData.quantity < 1) {
        return
      }

      if (movementType === 'transfer') {
        if (!this.formData.fromLocation || !this.formData.toLocation) {
          return
        }
      }

      if (movementType === 'adjustment') {
        if (!this.formData.notes) {
          return
        }
      }

      this.$emit('save', {
        itemSku: this.formData.itemCode,
        quantity: this.formData.quantity,
        movementType,
        reference: this.formData.reference,
        notes: this.formData.notes,
        fromLocation: this.formData.fromLocation,
        toLocation: this.formData.toLocation
      })
    }
  }
})
</script>
