<script setup lang="ts">
import { ref, onMounted } from 'vue'
import Card from '@/components/ui/Card.vue'
import CardHeader from '@/components/ui/CardHeader.vue'
import CardTitle from '@/components/ui/CardTitle.vue'
import CardContent from '@/components/ui/CardContent.vue'
import Breadcrumbs from '@/components/ui/Breadcrumbs.vue'
import Button from '@/components/ui/Button.vue'
import { ClipboardCheck, Plus, Search, Filter, CheckCircle2, XCircle } from 'lucide-vue-next'
import { useQualityApi } from '@/composables/useQualityApi'

const { getChecklists, isLoading } = useQualityApi()

const checklists = ref<any[]>([])
const searchQuery = ref('')
const showActiveOnly = ref(false)
const currentPage = ref(1)
const totalCount = ref(0)

const loadChecklists = async () => {
  try {
    const result = await getChecklists({
      searchTerm: searchQuery.value || undefined,
      isActive: showActiveOnly ? true : undefined,
      pageNumber: currentPage.value,
      pageSize: 20
    })
    checklists.value = result.items
    totalCount.value = result.totalCount
  } catch (error) {
    console.error('Failed to load checklists:', error)
  }
}

onMounted(() => {
  loadChecklists()
})
</script>

<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <Breadcrumbs />
        <h1 class="text-2xl md:text-3xl font-bold tracking-tight mt-2">Quality Checklists</h1>
        <p class="text-muted-foreground mt-1">Manage quality control checklists</p>
      </div>
      <Button>
        <Plus :size="18" class="mr-2" />
        New Checklist
      </Button>
    </div>

    <!-- Filters -->
    <Card>
      <CardContent class="pt-6">
        <div class="flex flex-col md:flex-row gap-4">
          <div class="flex-1 relative">
            <Search
              :size="18"
              class="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground"
            />
            <input
              v-model="searchQuery"
              @input="loadChecklists"
              type="text"
              placeholder="Search checklists..."
              class="w-full pl-10 pr-4 py-2 bg-background border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
            />
          </div>
          <Button
            variant="outline"
            :class="{ 'bg-primary/10 border-primary': showActiveOnly }"
            @click="showActiveOnly = !showActiveOnly; loadChecklists()"
          >
            <Filter :size="18" class="mr-2" />
            Active Only
          </Button>
        </div>
      </CardContent>
    </Card>

    <!-- Checklists List -->
    <Card>
      <CardHeader>
        <CardTitle>Checklists ({{ totalCount }})</CardTitle>
      </CardHeader>
      <CardContent>
        <div v-if="isLoading" class="text-center py-12 text-muted-foreground">
          Loading checklists...
        </div>
        <div v-else-if="checklists.length === 0" class="text-center py-12 text-muted-foreground">
          <ClipboardCheck :size="48" class="mx-auto mb-3 opacity-50" />
          <p>No checklists found</p>
          <p class="text-sm mt-1">Create your first quality checklist</p>
        </div>
        <div v-else class="space-y-3">
          <div
            v-for="checklist in checklists"
            :key="checklist.id"
            class="p-4 border rounded-lg hover:bg-accent/50 transition-colors cursor-pointer"
          >
            <div class="flex items-start justify-between">
              <div class="flex-1">
                <div class="flex items-center gap-2">
                  <div class="font-medium text-lg">{{ checklist.name }}</div>
                  <component
                    :is="checklist.isActive ? CheckCircle2 : XCircle"
                    :size="16"
                    :class="checklist.isActive ? 'text-emerald-600' : 'text-red-600'"
                  />
                </div>
                <div v-if="checklist.description" class="text-sm text-muted-foreground mt-1">
                  {{ checklist.description }}
                </div>
                <div class="text-sm text-muted-foreground mt-2">
                  {{ checklist.itemCount }} item{{ checklist.itemCount !== 1 ? 's' : '' }} • 
                  Created: {{ new Date(checklist.created).toLocaleDateString() }}
                </div>
              </div>
            </div>
          </div>
        </div>
      </CardContent>
    </Card>
  </div>
</template>

