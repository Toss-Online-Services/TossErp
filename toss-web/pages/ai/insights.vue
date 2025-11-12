<template>
  <div class="container mx-auto p-6 max-w-6xl">
    <!-- Header -->
    <div class="flex items-center justify-between mb-8">
      <div>
        <h1 class="text-3xl font-bold">{{ $t('aiInsights.title', 'AI Business Insights') }}</h1>
        <p class="text-muted-foreground mt-2">
          {{ $t('aiInsights.subtitle', 'Powered by TOSS AI to help grow your business') }}
        </p>
      </div>
      <Button @click="refreshInsights" :disabled="loading">
        <RefreshCw :class="{ 'animate-spin': loading }" class="mr-2 h-4 w-4" />
        {{ $t('common.refresh', 'Refresh') }}
      </Button>
    </div>

    <!-- AI Tools Grid -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-8">
      <Card v-for="tool in aiTools" :key="tool.id" class="cursor-pointer hover:shadow-md transition-shadow">
        <CardHeader>
          <div class="flex items-center gap-3">
            <div class="p-2 rounded-lg bg-primary/10">
              <Component :is="tool.icon" class="h-6 w-6 text-primary" />
            </div>
            <div>
              <CardTitle class="text-lg">{{ tool.name }}</CardTitle>
              <CardDescription class="text-sm">{{ tool.description }}</CardDescription>
            </div>
          </div>
        </CardHeader>
        <CardContent>
          <Button 
            @click="executeTool(tool.id)" 
            :disabled="loading"
            class="w-full"
            variant="outline"
          >
            {{ $t('aiInsights.execute', 'Execute') }}
          </Button>
        </CardContent>
      </Card>
    </div>

    <!-- Insights Results -->
    <Card v-if="insights.length > 0">
      <CardHeader>
        <CardTitle class="flex items-center gap-2">
          <Sparkles class="h-5 w-5 text-yellow-500" />
          {{ $t('aiInsights.recentInsights', 'Recent AI Insights') }}
        </CardTitle>
      </CardHeader>
      <CardContent>
        <div class="space-y-4">
          <div 
            v-for="insight in insights" 
            :key="insight.id"
            class="p-4 border rounded-lg bg-card"
          >
            <div class="flex items-start justify-between mb-2">
              <h3 class="font-medium text-foreground">{{ insight.title }}</h3>
              <Badge :variant="getInsightVariant(insight.type)">{{ insight.type }}</Badge>
            </div>
            <p class="text-sm text-muted-foreground mb-3">{{ insight.content }}</p>
            <div class="flex items-center justify-between text-xs text-muted-foreground">
              <span>Generated: {{ formatDate(insight.timestamp) }}</span>
              <span>Tool: {{ insight.tool }}</span>
            </div>
          </div>
        </div>
      </CardContent>
    </Card>

    <!-- Loading State -->
    <div v-if="loading" class="flex items-center justify-center py-12">
      <div class="text-center">
        <RefreshCw class="h-8 w-8 animate-spin mx-auto text-primary mb-4" />
        <p class="text-muted-foreground">{{ $t('aiInsights.loading', 'Generating insights...') }}</p>
      </div>
    </div>

    <!-- Empty State -->
    <Card v-if="insights.length === 0 && !loading" class="text-center py-12">
      <CardContent>
        <Bot class="h-12 w-12 mx-auto text-muted-foreground mb-4" />
        <h3 class="text-lg font-medium mb-2">{{ $t('aiInsights.noInsights', 'No insights yet') }}</h3>
        <p class="text-muted-foreground mb-4">
          {{ $t('aiInsights.getStarted', 'Execute an AI tool to generate business insights') }}
        </p>
        <Button @click="executeTool('business_insights')">
          {{ $t('aiInsights.generateInsights', 'Generate Business Insights') }}
        </Button>
      </CardContent>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { 
  RefreshCw, 
  Sparkles, 
  Bot, 
  Users, 
  Package, 
  ShoppingCart,
  TrendingUp,
  AlertTriangle,
  Truck,
  Calculator
} from 'lucide-vue-next'

import { Button } from '../../components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../../components/ui/card'
import { Badge } from '../../components/ui/badge'

// Types
interface AITool {
  id: string
  name: string
  description: string
  icon: any
}

interface Insight {
  id: string
  type: 'opportunity' | 'warning' | 'info' | 'success'
  title: string
  content: string
  tool: string
  timestamp: Date
}

// Reactive state
const loading = ref(false)
const insights = ref<Insight[]>([])
const { t } = useI18n()

// AI Tools Configuration
const aiTools = ref<AITool[]>([
  {
    id: 'get_customer_info',
    name: 'Customer Insights',
    description: 'Get detailed customer information and analytics',
    icon: Users
  },
  {
    id: 'get_product_info', 
    name: 'Product Analysis',
    description: 'Analyze product performance and trends',
    icon: Package
  },
  {
    id: 'create_sales_order',
    name: 'Sales Order Assistant',
    description: 'AI-powered sales order creation and recommendations',
    icon: ShoppingCart
  },
  {
    id: 'business_insights',
    name: 'Business Insights',
    description: 'Get AI-powered business insights and recommendations',
    icon: TrendingUp
  },
  {
    id: 'check_low_stock',
    name: 'Stock Alerts',
    description: 'Check for low stock items and generate reorder alerts',
    icon: AlertTriangle
  },
  {
    id: 'get_township_suppliers',
    name: 'Supplier Network',
    description: 'Find township suppliers and group buying opportunities',
    icon: Truck
  },
  {
    id: 'calculate_delivery_cost',
    name: 'Delivery Calculator',
    description: 'Calculate delivery costs and optimize logistics',
    icon: Calculator
  }
])

// Methods
const executeTool = async (toolId: string) => {
  loading.value = true
  
  try {
    // Simulate MCP server call - in real implementation this would call the actual MCP server
    console.log(`Executing AI tool: ${toolId}`)
    
    // Mock insight generation
    const mockInsight = generateMockInsight(toolId)
    insights.value.unshift(mockInsight)
    
    // Limit to last 10 insights
    if (insights.value.length > 10) {
      insights.value = insights.value.slice(0, 10)
    }
    
  } catch (error) {
    console.error('Error executing AI tool:', error)
  } finally {
    loading.value = false
  }
}

const generateMockInsight = (toolId: string): Insight => {
  const insightTemplates = {
    get_customer_info: {
      type: 'info' as const,
      title: 'Customer Analysis Complete',
      content: 'Your top 5 customers account for 45% of total revenue. Mthunzi\'s Spaza Shop has the highest order frequency with 23 orders this month.'
    },
    get_product_info: {
      type: 'opportunity' as const,
      title: 'Product Performance Insight',
      content: 'White bread and maize meal are your top sellers. Consider bulk purchasing to reduce costs by 12%.'
    },
    create_sales_order: {
      type: 'success' as const,
      title: 'Sales Order Optimization',
      content: 'AI recommends adding cooking oil and sugar to current orders based on buying patterns. Potential revenue increase: R850.'
    },
    business_insights: {
      type: 'opportunity' as const,
      title: 'Business Growth Opportunity',
      content: 'Weekend sales are 30% higher. Consider extended hours on Saturdays for additional R2,500 monthly revenue.'
    },
    check_low_stock: {
      type: 'warning' as const,
      title: 'Low Stock Alert',
      content: '5 items need reordering: White bread (8 left), Cooking oil (12 left), Soap (3 left). Total reorder value: R1,245.'
    },
    get_township_suppliers: {
      type: 'opportunity' as const,
      title: 'Group Buying Opportunity',
      content: '3 nearby shops interested in group purchase. Potential savings of 15% on maize meal and cooking oil orders.'
    },
    calculate_delivery_cost: {
      type: 'info' as const,
      title: 'Delivery Cost Analysis',
      content: 'Current delivery costs: R145 per order. Consolidating with nearby shops could reduce costs to R89 per order.'
    }
  }
  
  const template = insightTemplates[toolId as keyof typeof insightTemplates] || insightTemplates.business_insights
  const tool = aiTools.value.find(t => t.id === toolId)
  
  return {
    id: Date.now().toString(),
    type: template.type,
    title: template.title,
    content: template.content,
    tool: tool?.name || 'AI Tool',
    timestamp: new Date()
  }
}

const refreshInsights = () => {
  executeTool('business_insights')
}

const getInsightVariant = (type: string) => {
  switch (type) {
    case 'opportunity':
      return 'default'
    case 'warning':
      return 'destructive'
    case 'success':
      return 'secondary'
    case 'info':
    default:
      return 'outline'
  }
}

const formatDate = (date: Date) => {
  return new Intl.DateTimeFormat('en-ZA', {
    day: 'numeric',
    month: 'short',
    hour: '2-digit',
    minute: '2-digit'
  }).format(date)
}

onMounted(() => {
  console.log('AI Insights page loaded with MCP tools ready')
})
</script>