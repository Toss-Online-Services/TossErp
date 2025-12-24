<script setup lang="ts">
definePageMeta({
  middleware: 'auth',
  layout: 'default'
})

const form = ref({
  supplierId: '',
  deliveryDate: '',
  items: [
    { itemId: '', itemName: '', quantity: 1, unit: '', price: 0, total: 0 }
  ],
  notes: '',
  status: 'draft'
})

const suppliers = ref([
  { value: 'SUP-001', label: 'Fresh Produce Suppliers (Pty) Ltd' },
  { value: 'SUP-002', label: 'Township Wholesalers' },
  { value: 'SUP-003', label: 'Maize & More Distributors' }
])

const stockItems = ref([
  { value: 'ITM-001', label: 'White Bread 700g', unit: 'loaf', lastPrice: 8.50 },
  { value: 'ITM-002', label: 'Maize Meal 5kg', unit: 'bag', lastPrice: 45.00 },
  { value: 'ITM-003', label: 'Fresh Milk 2L', unit: 'bottle', lastPrice: 22.00 },
  { value: 'ITM-004', label: 'Cooking Oil 750ml', unit: 'bottle', lastPrice: 18.50 },
  { value: 'ITM-005', label: 'Chicken Pieces 1kg', unit: 'kg', lastPrice: 35.00 }
])

const addItem = () => {
  form.value.items.push({
    itemId: '',
    itemName: '',
    quantity: 1,
    unit: '',
    price: 0,
    total: 0
  })
}

const removeItem = (index: number) => {
  form.value.items.splice(index, 1)
}

const onItemSelect = (index: number, itemId: string) => {
  const item = stockItems.value.find(i => i.value === itemId)
  if (item) {
    form.value.items[index].itemName = item.label
    form.value.items[index].unit = item.unit
    form.value.items[index].price = item.lastPrice
    calculateTotal(index)
  }
}

const calculateTotal = (index: number) => {
  const item = form.value.items[index]
  item.total = item.quantity * item.price
}

const grandTotal = computed(() => {
  return form.value.items.reduce((sum, item) => sum + item.total, 0)
})

const saveDraft = () => {
  form.value.status = 'draft'
  // Save logic here
  console.log('Saving draft:', form.value)
  useRouter().push('/purchasing/orders')
}

const submitOrder = () => {
  form.value.status = 'submitted'
  // Submit logic here
  console.log('Submitting order:', form.value)
  useRouter().push('/purchasing/orders')
}
</script>

<template>
  <div class="p-4 md:p-6">
    <div class="mb-6">
      <div class="flex items-center gap-3 mb-2">
        <UButton
          variant="ghost"
          size="sm"
          icon="i-heroicons-arrow-left"
          to="/purchasing/orders"
        />
        <h1 class="text-2xl font-bold">Create Purchase Order</h1>
      </div>
      <p class="text-muted-foreground">Order stock from your suppliers</p>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <!-- Main Form -->
      <div class="lg:col-span-2 space-y-6">
        <!-- Supplier & Date -->
        <UCard>
          <template #header>
            <h2 class="font-semibold">Order Details</h2>
          </template>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium mb-2">Supplier *</label>
              <USelect
                v-model="form.supplierId"
                :options="suppliers"
                placeholder="Select supplier"
                required
              />
            </div>
            <div>
              <label class="block text-sm font-medium mb-2">Expected Delivery Date *</label>
              <UInput
                v-model="form.deliveryDate"
                type="date"
                required
              />
            </div>
          </div>
        </UCard>

        <!-- Items -->
        <UCard>
          <template #header>
            <div class="flex items-center justify-between">
              <h2 class="font-semibold">Items to Order</h2>
              <UButton size="sm" @click="addItem">
                <UIcon name="i-heroicons-plus" class="mr-1" />
                Add Item
              </UButton>
            </div>
          </template>

          <div class="space-y-4">
            <div
              v-for="(item, index) in form.items"
              :key="index"
              class="p-4 border rounded-lg"
            >
              <div class="flex items-start gap-3">
                <div class="flex-1 grid grid-cols-1 md:grid-cols-5 gap-3">
                  <div class="md:col-span-2">
                    <label class="block text-xs font-medium mb-1">Item</label>
                    <USelect
                      v-model="item.itemId"
                      :options="stockItems"
                      placeholder="Select item"
                      @change="onItemSelect(index, item.itemId)"
                    />
                  </div>
                  <div>
                    <label class="block text-xs font-medium mb-1">Quantity</label>
                    <UInput
                      v-model.number="item.quantity"
                      type="number"
                      min="1"
                      @input="calculateTotal(index)"
                    />
                  </div>
                  <div>
                    <label class="block text-xs font-medium mb-1">Price (R)</label>
                    <UInput
                      v-model.number="item.price"
                      type="number"
                      step="0.01"
                      min="0"
                      @input="calculateTotal(index)"
                    />
                  </div>
                  <div>
                    <label class="block text-xs font-medium mb-1">Total (R)</label>
                    <UInput
                      :model-value="item.total.toFixed(2)"
                      readonly
                      disabled
                    />
                  </div>
                </div>
                <UButton
                  v-if="form.items.length > 1"
                  color="red"
                  variant="ghost"
                  size="sm"
                  icon="i-heroicons-trash"
                  class="mt-6"
                  @click="removeItem(index)"
                />
              </div>
            </div>
          </div>
        </UCard>

        <!-- Notes -->
        <UCard>
          <template #header>
            <h2 class="font-semibold">Additional Notes</h2>
          </template>

          <UTextarea
            v-model="form.notes"
            placeholder="Add any special instructions or notes for this order..."
            :rows="4"
          />
        </UCard>
      </div>

      <!-- Summary Sidebar -->
      <div class="lg:col-span-1">
        <div class="sticky top-6">
          <UCard>
            <template #header>
              <h2 class="font-semibold">Order Summary</h2>
            </template>

            <div class="space-y-4">
              <div class="flex justify-between text-sm">
                <span class="text-muted-foreground">Items:</span>
                <span class="font-medium">{{ form.items.length }}</span>
              </div>
              <div class="flex justify-between text-sm">
                <span class="text-muted-foreground">Total Quantity:</span>
                <span class="font-medium">{{ form.items.reduce((sum, item) => sum + item.quantity, 0) }}</span>
              </div>
              <div class="border-t pt-4">
                <div class="flex justify-between mb-4">
                  <span class="font-semibold">Grand Total:</span>
                  <span class="text-xl font-bold">R{{ grandTotal.toFixed(2) }}</span>
                </div>
              </div>

              <div class="space-y-2">
                <UButton
                  block
                  size="lg"
                  @click="submitOrder"
                  :disabled="!form.supplierId || !form.deliveryDate || form.items.some(i => !i.itemId)"
                >
                  Submit Order
                </UButton>
                <UButton
                  block
                  size="lg"
                  variant="outline"
                  @click="saveDraft"
                >
                  Save as Draft
                </UButton>
                <UButton
                  block
                  size="lg"
                  variant="ghost"
                  to="/purchasing/orders"
                >
                  Cancel
                </UButton>
              </div>
            </div>
          </UCard>
        </div>
      </div>
    </div>
  </div>
</template>
