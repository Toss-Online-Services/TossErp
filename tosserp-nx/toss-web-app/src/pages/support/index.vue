<script setup lang="ts">
import Card from '@/components/ui/Card.vue'
import CardHeader from '@/components/ui/CardHeader.vue'
import CardTitle from '@/components/ui/CardTitle.vue'
import CardContent from '@/components/ui/CardContent.vue'
import Breadcrumbs from '@/components/ui/Breadcrumbs.vue'
import Button from '@/components/ui/Button.vue'
import { HelpCircle, MessageSquare, FileText, Plus, ArrowRight } from 'lucide-vue-next'
</script>

<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <Breadcrumbs />
        <h1 class="text-2xl md:text-3xl font-bold tracking-tight mt-2">Support</h1>
        <p class="text-muted-foreground mt-1">Get help and submit tickets</p>
      </div>
      <Button>
        <Plus :size="18" class="mr-2" />
        New Ticket
      </Button>
    </div>

    <div class="grid gap-4 md:grid-cols-3">
      <NuxtLink to="/support/tickets" class="block">
        <Card class="hover:shadow-material-md transition-shadow cursor-pointer">
          <CardHeader>
            <CardTitle class="flex items-center gap-2">
              <MessageSquare class="h-5 w-5 text-primary" />
              Support Tickets
            </CardTitle>
          </CardHeader>
          <CardContent>
            <p class="text-sm text-muted-foreground">
              View and manage support tickets
            </p>
            <div class="mt-4 flex items-center text-sm text-primary">
              View Tickets
              <ArrowRight :size="16" class="ml-2" />
            </div>
          </CardContent>
        </Card>
      </NuxtLink>

      <Card class="hover:shadow-material-md transition-shadow cursor-pointer">
        <CardHeader>
          <CardTitle class="flex items-center gap-2">
            <HelpCircle class="h-5 w-5 text-primary" />
            Help Center
          </CardTitle>
        </CardHeader>
        <CardContent>
          <p class="text-sm text-muted-foreground">
            Browse help articles and guides
          </p>
        </CardContent>
      </Card>

      <Card class="hover:shadow-material-md transition-shadow cursor-pointer">
        <CardHeader>
          <CardTitle class="flex items-center gap-2">
            <FileText class="h-5 w-5 text-primary" />
            Documentation
          </CardTitle>
        </CardHeader>
        <CardContent>
          <p class="text-sm text-muted-foreground">
            Read product documentation
          </p>
        </CardContent>
      </Card>
    </div>
  </div>
</template>


