<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useInventoryStore } from '../stores/inventory'

const store = useInventoryStore()

const page = ref(1)
const pageSize = ref(10)
const searchTerm = ref('')
const category = ref('')

const loading = computed(() => store.loading)
const items = computed(() => store.items)
const total = computed(() => store.totalCount)
const error = computed(() => store.error)

async function load() {
  await store.list({ page: page.value, pageSize: pageSize.value, searchTerm: searchTerm.value, category: category.value })
}

onMounted(load)

// Create form
const sku = ref('')
const name = ref('')
const creating = ref(false)
async function createItem() {
  creating.value = true
  await store.create({ sku: sku.value, name: name.value })
  creating.value = false
  sku.value = ''
  name.value = ''
  await load()
}

// Movement form
const moveItemId = ref('')
const moveQty = ref<number | null>(null)
const moving = ref(false)
async function applyMove() {
  if (!moveItemId.value || moveQty.value === null) return
  moving.value = true
  await store.move({ itemId: moveItemId.value, quantity: Number(moveQty.value) })
  moving.value = false
  moveItemId.value = ''
  moveQty.value = null
  await load()
}

function nextPage() { if (page.value * pageSize.value < total.value) { page.value++; load() } }
function prevPage() { if (page.value > 1) { page.value--; load() } }
</script>

<template>
  <div class="p-6 space-y-6">
    <h1 class="text-2xl font-semibold">Inventory</h1>

    <div class="flex gap-2 items-end">
      <div>
        <label class="block text-sm">Search</label>
        <input v-model="searchTerm" @keyup.enter="load" class="border px-2 py-1 rounded w-56" placeholder="name or sku" />
      </div>
      <div>
        <label class="block text-sm">Category</label>
        <input v-model="category" @keyup.enter="load" class="border px-2 py-1 rounded w-40" placeholder="category" />
      </div>
      <button @click="load" class="bg-blue-600 text-white px-3 py-1 rounded" :disabled="loading">Reload</button>
      <span v-if="loading" class="text-sm text-gray-500">Loading…</span>
    </div>

  <div v-if="error" class="p-3 bg-red-50 text-red-700 rounded">{{ error }}</div>

    <div class="overflow-x-auto">
      <table class="min-w-full text-sm border">
        <thead class="bg-gray-50">
          <tr>
            <th class="text-left p-2 border">ID</th>
            <th class="text-left p-2 border">SKU</th>
            <th class="text-left p-2 border">Name</th>
            <th class="text-right p-2 border">On Hand</th>
            <th class="text-left p-2 border">Category</th>
            <th class="text-left p-2 border">Updated</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="i in items" :key="i.id">
            <td class="p-2 border">{{ i.id }}</td>
            <td class="p-2 border">{{ i.sku }}</td>
            <td class="p-2 border">{{ i.name }}</td>
            <td class="p-2 border text-right">{{ i.quantityOnHand ?? 0 }}</td>
            <td class="p-2 border">{{ i.category }}</td>
            <td class="p-2 border">{{ i.updatedAt }}</td>
          </tr>
        </tbody>
      </table>
    </div>

    <div class="flex items-center gap-2">
      <button @click="prevPage" class="px-3 py-1 border rounded" :disabled="page===1">Prev</button>
      <span class="text-sm">Page {{ page }}</span>
      <button @click="nextPage" class="px-3 py-1 border rounded" :disabled="page*pageSize>=total">Next</button>
      <span class="text-sm text-gray-500">Total: {{ total }}</span>
    </div>

    <div class="grid md:grid-cols-2 gap-6">
      <div class="p-4 border rounded space-y-2">
        <h2 class="font-medium">Create Item</h2>
        <input v-model="sku" class="border px-2 py-1 rounded w-full" placeholder="SKU" />
        <input v-model="name" class="border px-2 py-1 rounded w-full" placeholder="Name" />
        <button @click="createItem" class="bg-green-600 text-white px-3 py-1 rounded" :disabled="creating || !sku || !name">
          {{ creating ? 'Creating…' : 'Create' }}
        </button>
      </div>

      <div class="p-4 border rounded space-y-2">
        <h2 class="font-medium">Apply Stock Movement</h2>
        <input v-model="moveItemId" class="border px-2 py-1 rounded w-full" placeholder="Item ID" />
        <input v-model.number="moveQty" type="number" class="border px-2 py-1 rounded w-full" placeholder="Quantity (e.g. 5 or -3)" />
        <button @click="applyMove" class="bg-purple-600 text-white px-3 py-1 rounded" :disabled="moving || !moveItemId || moveQty===null">
          {{ moving ? 'Applying…' : 'Apply' }}
        </button>
      </div>
    </div>

    <p class="text-xs text-gray-500">Set <code>x-tenant-id</code> header via a browser extension to scope data. Defaults to tenant1.</p>
  </div>
</template>

<style scoped>
</style>
