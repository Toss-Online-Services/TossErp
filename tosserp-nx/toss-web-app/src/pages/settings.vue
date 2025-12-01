<script setup lang="ts">
import Card from '@/components/ui/Card.vue'
import CardHeader from '@/components/ui/CardHeader.vue'
import CardContent from '@/components/ui/CardContent.vue'
import Breadcrumbs from '@/components/ui/Breadcrumbs.vue'
import { Settings } from 'lucide-vue-next'
</script>

<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <Breadcrumbs />
        <h1 class="text-2xl md:text-3xl font-bold tracking-tight mt-2">Settings</h1>
        <p class="text-muted-foreground mt-1">User preferences and configuration</p>
      </div>
    </div>
    <Card>
      <CardHeader>
        <div class="flex items-center gap-2">
          <Settings class="h-4 w-4 text-muted-foreground" />
          <span class="font-semibold">General Settings</span>
        </div>
      </CardHeader>
      <CardContent>
        <div class="space-y-4">
          <div class="flex items-center justify-between">
            <span class="text-muted-foreground">Theme</span>
            <select class="bg-card border rounded px-2 py-1">
              <option>Light</option>
              <option>Dark</option>
              <option>System</option>
            </select>
          </div>
          <div class="flex items-center justify-between">
            <span class="text-muted-foreground">Language</span>
            <select class="bg-card border rounded px-2 py-1">
              <option>English</option>
              <option>Zulu</option>
              <option>Xhosa</option>
            </select>
          </div>
        </div>
      </CardContent>
    </Card>
  </div>
</template>
