<script setup lang="ts">
import Card from '@/components/ui/Card.vue'
import CardHeader from '@/components/ui/CardHeader.vue'
import CardTitle from '@/components/ui/CardTitle.vue'
import CardContent from '@/components/ui/CardContent.vue'
import Breadcrumbs from '@/components/ui/Breadcrumbs.vue'
import { Truck, Package, ArrowRight } from 'lucide-vue-next'
</script>

<template>
  <div class="space-y-6">
    <div>
      <Breadcrumbs />
      <h1 class="text-2xl md:text-3xl font-bold tracking-tight mt-2">Jobs</h1>
      <p class="text-muted-foreground mt-1">Deliveries, projects, and tasks</p>
    </div>

    <div class="grid gap-4 md:grid-cols-2">
      <NuxtLink to="/jobs/deliveries" class="block">
        <Card class="hover:shadow-material-md transition-shadow cursor-pointer">
          <CardHeader>
            <CardTitle class="flex items-center gap-2">
              <Truck class="h-5 w-5 text-primary" />
              Deliveries
            </CardTitle>
          </CardHeader>
          <CardContent>
            <p class="text-sm text-muted-foreground">
              Track deliveries and driver assignments
            </p>
            <div class="mt-4 flex items-center text-sm text-primary">
              View Deliveries
              <ArrowRight :size="16" class="ml-2" />
            </div>
          </CardContent>
        </Card>
      </NuxtLink>

      <NuxtLink to="/jobs/projects" class="block">
        <Card class="hover:shadow-material-md transition-shadow cursor-pointer">
          <CardHeader>
            <CardTitle class="flex items-center gap-2">
              <Package class="h-5 w-5 text-primary" />
              Projects
            </CardTitle>
          </CardHeader>
          <CardContent>
            <p class="text-sm text-muted-foreground">
              Manage projects and jobs
            </p>
            <div class="mt-4 flex items-center text-sm text-primary">
              View Projects
              <ArrowRight :size="16" class="ml-2" />
            </div>
          </CardContent>
        </Card>
      </NuxtLink>
    </div>
  </div>
</template>

