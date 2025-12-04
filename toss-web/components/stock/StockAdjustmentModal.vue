<script setup lang="ts">
import { ref } from 'vue'
import { useStockStore, type Item } from '~/stores/stock'
import StockAdjustmentForm from '~/components/stock/StockAdjustmentForm.vue'

interface Props {
  show: boolean
  item: Item | null
}

const props = defineProps<Props>()

const emit = defineEmits<{
  close: []
  adjusted: []
}>()

const stockStore = useStockStore()
const formRef = ref<InstanceType<typeof StockAdjustmentForm> | null>(null)

function handleAdjusted() {
  emit('adjusted')
  emit('close')
  if (formRef.value) {
    formRef.value.resetForm()
  }
}

function handleCancelled() {
  emit('close')
}

function handleClose() {
  emit('close')
  if (formRef.value) {
    formRef.value.resetForm()
  }
}
</script>

<template>
  <Teleport to="body">
    <Transition name="modal">
      <div
        v-if="show && item"
        class="fixed inset-0 z-50 overflow-y-auto"
        @click.self="handleClose"
      >
        <div class="flex min-h-screen items-center justify-center p-4">
          <div
            class="relative w-full max-w-md rounded-xl bg-white shadow-xl"
            @click.stop
          >
            <!-- Header -->
            <div class="flex items-center justify-between border-b border-gray-200 px-6 py-4">
              <h3 class="text-xl font-bold text-gray-900">Adjust Stock</h3>
              <button
                @click="handleClose"
                class="text-gray-400 hover:text-gray-600 transition-colors"
              >
                <i class="material-symbols-rounded text-2xl">close</i>
              </button>
            </div>

            <!-- Content -->
            <div class="p-6">
              <StockAdjustmentForm
                ref="formRef"
                :item="item"
                :show-item-info="true"
                :compact="false"
                @adjusted="handleAdjusted"
                @cancelled="handleCancelled"
              />
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

