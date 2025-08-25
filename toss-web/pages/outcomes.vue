<template>
  <div class="outcomes-page">
    <ClientOnly>
      <template #default>
        <!-- Header Section -->
        <div class="outcomes-header">
          <div class="header-content">
            <h1>🎯 My Business Outcomes</h1>
            <p class="header-subtitle">Track the real impact of your AI-powered business automation</p>
            <div class="summary-stats">
              <div class="stat-card">
                <div class="stat-value">{{ totalOutcomes }}</div>
                <div class="stat-label">Total Outcomes</div>
              </div>
              <div class="stat-card">
                <div class="stat-value">R{{ formatCurrency(totalValue) }}</div>
                <div class="stat-label">Value Generated</div>
              </div>
              <div class="stat-card">
                <div class="stat-value">{{ completionRate }}%</div>
                <div class="stat-label">Success Rate</div>
              </div>
            </div>
          </div>
        </div>

        <!-- Outcomes Timeline -->
        <div class="outcomes-timeline">
          <h2>Recent Outcomes</h2>
          <div class="timeline-container">
            <div v-for="outcome in outcomes" :key="outcome.id" class="timeline-item">
              <div class="timeline-marker" :class="outcome.status"></div>
              <div class="timeline-content">
                <div class="outcome-header">
                  <h3>{{ outcome.title }}</h3>
                  <span class="outcome-date">{{ formatDate(outcome.date) }}</span>
                </div>
                <p class="outcome-description">{{ outcome.description }}</p>
                <div class="outcome-metrics">
                  <div class="metric" v-if="outcome.value">
                    <span class="metric-label">Value:</span>
                    <span class="metric-value">R{{ formatCurrency(outcome.value) }}</span>
                  </div>
                  <div class="metric" v-if="outcome.savings">
                    <span class="metric-label">Savings:</span>
                    <span class="metric-value">R{{ formatCurrency(outcome.savings) }}</span>
                  </div>
                  <div class="metric" v-if="outcome.efficiency">
                    <span class="metric-label">Efficiency:</span>
                    <span class="metric-value">{{ outcome.efficiency }}%</span>
                  </div>
                </div>
                <div class="outcome-tags">
                  <span v-for="tag in outcome.tags" :key="tag" class="tag">{{ tag }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Categories Grid -->
        <div class="outcomes-categories">
          <h2>Outcomes by Category</h2>
          <div class="categories-grid">
            <div v-for="category in categories" :key="category.name" class="category-card">
              <div class="category-header">
                <span class="category-icon">{{ category.icon }}</span>
                <h3>{{ category.name }}</h3>
              </div>
              <div class="category-stats">
                <div class="stat">
                  <span class="stat-number">{{ category.count }}</span>
                  <span class="stat-text">Outcomes</span>
                </div>
                <div class="stat">
                  <span class="stat-number">R{{ formatCurrency(category.value) }}</span>
                  <span class="stat-text">Value</span>
                </div>
              </div>
              <div class="category-outcomes">
                <div v-for="item in category.recent" :key="item.id" class="mini-outcome">
                  <span class="mini-title">{{ item.title }}</span>
                  <span class="mini-value">R{{ formatCurrency(item.value) }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </template>
      
      <template #fallback>
        <div class="loading-state">
          <div class="loading-spinner"></div>
          <p>Loading your business outcomes...</p>
        </div>
      </template>
    </ClientOnly>
  </div>
</template>

<script setup>
// Page meta
definePageMeta({
  title: 'My Outcomes'
})

// Reactive data
const outcomes = ref([
  {
    id: 1,
    title: 'Automated Inventory Optimization',
    description: 'AI agent optimized inventory levels, preventing stockouts and reducing carrying costs',
    date: new Date('2025-08-20'),
    value: 3400,
    savings: 850,
    efficiency: 23,
    status: 'completed',
    tags: ['Inventory', 'AI Optimization', 'Cost Savings']
  },
  {
    id: 2,
    title: 'Customer Retention Boost',
    description: 'Personalized communication increased customer satisfaction and loyalty',
    date: new Date('2025-08-18'),
    value: 2100,
    efficiency: 18,
    status: 'completed',
    tags: ['Customer Experience', 'Retention', 'AI Personalization']
  },
  {
    id: 3,
    title: 'Automated Invoice Processing',
    description: 'AI-powered invoice processing reduced manual work and improved cash flow',
    date: new Date('2025-08-15'),
    value: 1750,
    savings: 420,
    status: 'completed',
    tags: ['Finance', 'Automation', 'Cash Flow']
  },
  {
    id: 4,
    title: 'Supply Chain Optimization',
    description: 'Smart vendor negotiations secured better pricing and terms',
    date: new Date('2025-08-22'),
    value: 2800,
    savings: 1200,
    status: 'in-progress',
    tags: ['Supply Chain', 'Vendor Management', 'Cost Reduction']
  }
])

const categories = ref([
  {
    name: 'Revenue Growth',
    icon: '💰',
    count: 5,
    value: 12500,
    recent: [
      { id: 1, title: 'Upselling Campaign', value: 3200 },
      { id: 2, title: 'Price Optimization', value: 1800 }
    ]
  },
  {
    name: 'Cost Savings',
    icon: '📉',
    count: 4,
    value: 8700,
    recent: [
      { id: 3, title: 'Vendor Negotiations', value: 2400 },
      { id: 4, title: 'Process Automation', value: 1900 }
    ]
  },
  {
    name: 'Efficiency Gains',
    icon: '⚡',
    count: 6,
    value: 5600,
    recent: [
      { id: 5, title: 'Workflow Automation', value: 1500 },
      { id: 6, title: 'AI Decision Making', value: 1100 }
    ]
  },
  {
    name: 'Customer Experience',
    icon: '🎯',
    count: 3,
    value: 4200,
    recent: [
      { id: 7, title: 'Personalization Engine', value: 1800 },
      { id: 8, title: 'Response Automation', value: 900 }
    ]
  }
])

// Computed properties
const totalOutcomes = computed(() => outcomes.value.length)
const totalValue = computed(() => outcomes.value.reduce((sum, outcome) => sum + (outcome.value || 0), 0))
const completionRate = computed(() => {
  const completed = outcomes.value.filter(o => o.status === 'completed').length
  return Math.round((completed / outcomes.value.length) * 100)
})

// Utility functions
const formatCurrency = (amount) => {
  return new Intl.NumberFormat('en-ZA').format(amount)
}

const formatDate = (date) => {
  return new Intl.DateTimeFormat('en-ZA', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  }).format(date)
}
</script>

<style scoped>
.outcomes-page {
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 2rem;
}

.outcomes-header {
  margin-bottom: 3rem;
}

.header-content h1 {
  font-size: 2.5rem;
  font-weight: 700;
  margin-bottom: 0.5rem;
}

.header-subtitle {
  font-size: 1.1rem;
  opacity: 0.9;
  margin-bottom: 2rem;
}

.summary-stats {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1.5rem;
}

.stat-card {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 15px;
  padding: 1.5rem;
  text-align: center;
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.stat-value {
  font-size: 2rem;
  font-weight: 700;
  display: block;
  margin-bottom: 0.5rem;
}

.stat-label {
  font-size: 0.9rem;
  opacity: 0.8;
}

.outcomes-timeline {
  margin-bottom: 3rem;
}

.outcomes-timeline h2,
.outcomes-categories h2 {
  font-size: 1.875rem;
  font-weight: 700;
  margin-bottom: 1.5rem;
}

.timeline-container {
  position: relative;
}

.timeline-container::before {
  content: '';
  position: absolute;
  left: 20px;
  top: 0;
  bottom: 0;
  width: 2px;
  background: rgba(255, 255, 255, 0.3);
}

.timeline-item {
  position: relative;
  margin-bottom: 2rem;
  padding-left: 60px;
}

.timeline-marker {
  position: absolute;
  left: 10px;
  top: 10px;
  width: 20px;
  height: 20px;
  border-radius: 50%;
  border: 3px solid white;
}

.timeline-marker.completed {
  background: #10b981;
}

.timeline-marker.in-progress {
  background: #f59e0b;
}

.timeline-content {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 15px;
  padding: 1.5rem;
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.outcome-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.outcome-header h3 {
  font-size: 1.25rem;
  font-weight: 600;
  margin: 0;
}

.outcome-date {
  font-size: 0.875rem;
  opacity: 0.8;
}

.outcome-description {
  margin-bottom: 1rem;
  line-height: 1.6;
}

.outcome-metrics {
  display: flex;
  gap: 1.5rem;
  margin-bottom: 1rem;
  flex-wrap: wrap;
}

.metric {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.metric-label {
  font-size: 0.875rem;
  opacity: 0.8;
}

.metric-value {
  font-weight: 600;
  color: #10b981;
}

.outcome-tags {
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
}

.tag {
  background: rgba(255, 255, 255, 0.2);
  padding: 0.25rem 0.75rem;
  border-radius: 20px;
  font-size: 0.75rem;
  font-weight: 500;
}

.categories-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 1.5rem;
}

.category-card {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 15px;
  padding: 1.5rem;
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.category-header {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 1rem;
}

.category-icon {
  font-size: 1.5rem;
}

.category-header h3 {
  font-size: 1.125rem;
  font-weight: 600;
  margin: 0;
}

.category-stats {
  display: flex;
  justify-content: space-between;
  margin-bottom: 1.5rem;
}

.stat {
  text-align: center;
}

.stat-number {
  display: block;
  font-size: 1.25rem;
  font-weight: 700;
  color: #10b981;
}

.stat-text {
  font-size: 0.75rem;
  opacity: 0.8;
}

.mini-outcome {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.5rem;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 8px;
  margin-bottom: 0.5rem;
}

.mini-title {
  font-size: 0.875rem;
  flex: 1;
}

.mini-value {
  font-size: 0.875rem;
  font-weight: 600;
  color: #10b981;
}

.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 60vh;
  text-align: center;
}

.loading-spinner {
  width: 40px;
  height: 40px;
  border: 4px solid rgba(255, 255, 255, 0.3);
  border-top: 4px solid white;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 1rem;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

@media (max-width: 768px) {
  .outcomes-page {
    padding: 1rem;
  }

  .header-content h1 {
    font-size: 2rem;
  }

  .summary-stats {
    grid-template-columns: 1fr;
  }

  .outcome-metrics {
    flex-direction: column;
    gap: 0.75rem;
  }

  .category-stats {
    flex-direction: column;
    gap: 1rem;
  }
}
</style>
