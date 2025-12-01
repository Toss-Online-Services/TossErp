<script setup lang="ts">
import { ref, onMounted } from 'vue'
import Card from '@/components/ui/Card.vue'
import CardHeader from '@/components/ui/CardHeader.vue'
import CardTitle from '@/components/ui/CardTitle.vue'
import CardContent from '@/components/ui/CardContent.vue'
import Breadcrumbs from '@/components/ui/Breadcrumbs.vue'
import Button from '@/components/ui/Button.vue'
import { FileText, Plus, Search, Filter } from 'lucide-vue-next'

const requests = ref<any[]>([])
const isLoading = ref(false)
const searchQuery = ref('')

// TODO: Fetch from API
onMounted(() => {
  requests.value = []
})
</script>

<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <Breadcrumbs />
        <h1 class="text-2xl md:text-3xl font-bold tracking-tight mt-2">Purchase Requests</h1>
        <p class="text-muted-foreground mt-1">Create and manage purchase requests</p>
      </div>
      <Button>
        <Plus :size="18" class="mr-2" />
        New Request
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
              type="text"
              placeholder="Search purchase requests..."
              class="w-full pl-10 pr-4 py-2 bg-background border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
            />
          </div>
          <Button variant="outline">
            <Filter :size="18" class="mr-2" />
            Filter
          </Button>
        </div>
      </CardContent>
    </Card>

    <!-- Requests List -->
    <Card>
      <CardHeader>
        <CardTitle>Purchase Requests</CardTitle>
      </CardHeader>
      <CardContent>
        <div v-if="isLoading" class="text-center py-12 text-muted-foreground">
          Loading purchase requests...
        </div>
        <div v-else-if="requests.length === 0" class="text-center py-12 text-muted-foreground">
          <FileText :size="48" class="mx-auto mb-3 opacity-50" />
          <p>No purchase requests found</p>
          <p class="text-sm mt-1">Create your first purchase request to get started</p>
        </div>
        <div v-else class="space-y-2">
          <!-- Request items will go here -->
        </div>
      </CardContent>
    </Card>
  </div>
</template>

