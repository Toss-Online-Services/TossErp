<template>
  <div class="services-catalog">
    <div class="catalog-header">
      <h1>🚀 Business Services Catalog</h1>
      <p class="subtitle">Autonomous AI services that deliver guaranteed business outcomes</p>
      <div class="value-proposition">
        <div class="value-item">
          <span class="icon">⚡</span>
          <span>Instant activation</span>
        </div>
        <div class="value-item">
          <span class="icon">🎯</span>
          <span>Guaranteed outcomes</span>
        </div>
        <div class="value-item">
          <span class="icon">💰</span>
          <span>Pay for results</span>
        </div>
        <div class="value-item">
          <span class="icon">🤖</span>
          <span>Fully autonomous</span>
        </div>
      </div>
    </div>

    <!-- Service Categories -->
    <div class="categories">
      <button 
        v-for="category in categories" 
        :key="category.id"
        @click="selectedCategory = category.id"
        :class="['category-btn', { active: selectedCategory === category.id }]"
      >
        {{ category.icon }} {{ category.name }}
      </button>
    </div>

    <!-- Featured Services -->
    <div v-if="selectedCategory === 'all'" class="featured-section">
      <h2>⭐ Most Requested Services</h2>
      <div class="featured-grid">
        <div v-for="service in featuredServices" :key="service.id" class="featured-card">
          <div class="service-badge">{{ service.badge }}</div>
          <div class="service-icon">{{ service.icon }}</div>
          <h3>{{ service.name }}</h3>
          <p class="service-tagline">{{ service.tagline }}</p>
          <div class="guaranteed-outcome">
            <span class="guarantee-label">Guaranteed Outcome:</span>
            <span class="guarantee-text">{{ service.guaranteedOutcome }}</span>
          </div>
          <div class="service-pricing">
            <span class="price">R{{ service.pricing.standard }}/month</span>
            <span class="roi">{{ service.avgROI }}% avg ROI</span>
          </div>
          <button @click="activateService(service.id)" class="btn-activate">
            Activate Now
          </button>
        </div>
      </div>
    </div>

    <!-- Service Grid -->
    <div class="services-section">
      <h2 v-if="selectedCategory !== 'all'">{{ getCategoryName(selectedCategory) }} Services</h2>
      <div class="services-grid">
        <div v-for="service in filteredServices" :key="service.id" class="service-card">
          <div class="service-header">
            <div class="service-icon">{{ service.icon }}</div>
            <div class="service-info">
              <h3>{{ service.name }}</h3>
              <p class="service-description">{{ service.description }}</p>
            </div>
            <div class="service-status" :class="service.status">
              {{ service.status === 'active' ? '✅ Active' : '🔄 Available' }}
            </div>
          </div>

          <div class="guaranteed-outcomes">
            <h4>Guaranteed Business Outcomes:</h4>
            <ul>
              <li v-for="outcome in service.guaranteedOutcomes" :key="outcome">
                🎯 {{ outcome }}
              </li>
            </ul>
          </div>

          <div class="service-metrics">
            <div class="metric">
              <span class="label">Time to Value</span>
              <span class="value">{{ service.timeToValue }}</span>
            </div>
            <div class="metric">
              <span class="label">Average ROI</span>
              <span class="value">{{ service.avgROI }}%</span>
            </div>
            <div class="metric">
              <span class="label">Customer Success</span>
              <span class="value">{{ service.successRate }}%</span>
            </div>
          </div>

          <div class="pricing-tiers">
            <h4>Investment Options:</h4>
            <div class="tiers">
              <div class="tier" :class="{ recommended: tier.recommended }" v-for="tier in service.pricingTiers" :key="tier.name">
                <div class="tier-name">{{ tier.name }}</div>
                <div class="tier-price">R{{ tier.price }}/month</div>
                <div class="tier-features">
                  <div v-for="feature in tier.features" :key="feature" class="feature">
                    ✓ {{ feature }}
                  </div>
                </div>
                <button @click="selectTier(service.id, tier)" class="btn-select-tier" :class="{ primary: tier.recommended }">
                  {{ tier.recommended ? 'Recommended' : 'Select' }}
                </button>
              </div>
            </div>
          </div>

          <div class="service-actions">
            <button v-if="service.status !== 'active'" @click="activateService(service.id)" class="btn-activate">
              Activate Service
            </button>
            <button v-else @click="manageService(service.id)" class="btn-manage">
              Manage Service
            </button>
            <button @click="learnMore(service.id)" class="btn-learn-more">
              Learn More
            </button>
          </div>

          <div class="social-proof" v-if="service.testimonial">
            <div class="testimonial">
              <p>"{{ service.testimonial.text }}"</p>
              <div class="author">
                - {{ service.testimonial.author }}, {{ service.testimonial.company }}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- ROI Calculator Modal -->
    <div v-if="showROICalculator" class="modal-overlay" @click="closeROICalculator">
      <div class="roi-calculator" @click.stop>
        <h3>ROI Calculator for {{ selectedService?.name }}</h3>
        <div class="calculator-form">
          <div class="form-group">
            <label>Current Monthly Revenue</label>
            <input v-model="roiInputs.revenue" type="number" placeholder="e.g., 50000">
          </div>
          <div class="form-group">
            <label>Current Monthly Costs</label>
            <input v-model="roiInputs.costs" type="number" placeholder="e.g., 30000">
          </div>
          <div class="form-group">
            <label>Hours Spent on Manual Processes</label>
            <input v-model="roiInputs.manualHours" type="number" placeholder="e.g., 40">
          </div>
        </div>
        <div class="roi-results">
          <h4>Projected Outcomes with {{ selectedService?.name }}:</h4>
          <div class="projection">
            <div class="projection-item">
              <span>Monthly Revenue Increase:</span>
              <span class="positive">R{{ calculateRevenueIncrease() }}</span>
            </div>
            <div class="projection-item">
              <span>Monthly Cost Savings:</span>
              <span class="positive">R{{ calculateCostSavings() }}</span>
            </div>
            <div class="projection-item">
              <span>Time Saved per Month:</span>
              <span class="positive">{{ calculateTimeSavings() }} hours</span>
            </div>
            <div class="projection-item total">
              <span>Total Monthly Value:</span>
              <span class="positive">R{{ calculateTotalValue() }}</span>
            </div>
            <div class="projection-item">
              <span>Service Investment:</span>
              <span>R{{ selectedService?.pricing.standard }}</span>
            </div>
            <div class="projection-item profit">
              <span>Net Monthly Profit:</span>
              <span class="positive">R{{ calculateNetProfit() }}</span>
            </div>
            <div class="projection-item roi">
              <span>ROI:</span>
              <span class="positive">{{ calculateROI() }}%</span>
            </div>
          </div>
        </div>
        <div class="calculator-actions">
          <button @click="closeROICalculator" class="btn-close">Close</button>
          <button @click="activateFromCalculator" class="btn-activate">Activate Service</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
const { data: catalogData } = await $fetch('/api/services/catalog')

const selectedCategory = ref('all')
const showROICalculator = ref(false)
const selectedService = ref(null)
const roiInputs = ref({
  revenue: 50000,
  costs: 30000,
  manualHours: 40
})

const categories = [
  { id: 'all', name: 'All Services', icon: '🏢' },
  { id: 'revenue', name: 'Revenue Growth', icon: '💰' },
  { id: 'operations', name: 'Operations', icon: '⚙️' },
  { id: 'customers', name: 'Customer Success', icon: '👥' },
  { id: 'finance', name: 'Financial Management', icon: '💼' },
  { id: 'collaboration', name: 'Business Networks', icon: '🤝' }
]

const featuredServices = [
  {
    id: 'sales-automation',
    name: 'Sales Automation',
    tagline: 'AI that closes deals while you sleep',
    icon: '🎯',
    badge: 'Most Popular',
    guaranteedOutcome: '15% revenue increase in 60 days',
    pricing: { standard: 499 },
    avgROI: 450
  },
  {
    id: 'customer-engagement',
    name: 'Customer Success AI',
    tagline: 'Turn customers into loyal advocates',
    icon: '❤️',
    badge: 'Highest ROI',
    guaranteedOutcome: '25% retention improvement',
    pricing: { standard: 249 },
    avgROI: 580
  },
  {
    id: 'inventory-management',
    name: 'Smart Inventory',
    tagline: 'Never run out, never overstock',
    icon: '📦',
    badge: 'Fast Results',
    guaranteedOutcome: 'Zero stockouts guaranteed',
    pricing: { standard: 299 },
    avgROI: 320
  }
]

const services = [
  {
    id: 'sales-automation',
    name: 'Autonomous Sales Processing',
    description: 'AI handles invoicing, payments, follow-ups, and pricing optimization',
    icon: '🎯',
    category: 'revenue',
    status: 'available',
    guaranteedOutcomes: [
      'Increase revenue by 15-25%',
      'Reduce payment delays by 60%',
      'Automate 90% of sales admin',
      'Optimize pricing for maximum profit'
    ],
    timeToValue: '7-14 days',
    avgROI: 450,
    successRate: 94,
    pricingTiers: [
      {
        name: 'Starter',
        price: 299,
        features: ['Basic automation', 'Payment reminders', 'Simple reporting'],
        recommended: false
      },
      {
        name: 'Professional',
        price: 499,
        features: ['Full automation', 'AI pricing', 'Advanced analytics', 'WhatsApp integration'],
        recommended: true
      },
      {
        name: 'Enterprise',
        price: 699,
        features: ['Everything in Pro', 'Custom workflows', 'Priority support', 'Multi-store management'],
        recommended: false
      }
    ],
    testimonial: {
      text: 'Sales automation increased our revenue by 23% in just 6 weeks. The AI literally works 24/7.',
      author: 'Sarah Johnson',
      company: 'Beautiful Hair Salon'
    }
  },
  {
    id: 'customer-engagement',
    name: 'AI Customer Relationship Manager',
    description: 'AI manages customer relationships to maximize satisfaction and lifetime value',
    icon: '👥',
    category: 'customers',
    status: 'available',
    guaranteedOutcomes: [
      'Increase customer retention by 20-30%',
      'Improve satisfaction scores by 25%',
      'Automate personalized communications',
      'Predict and prevent customer churn'
    ],
    timeToValue: '14-21 days',
    avgROI: 380,
    successRate: 91,
    pricingTiers: [
      {
        name: 'Basic',
        price: 149,
        features: ['Automated birthday messages', 'Basic loyalty tracking', 'Email campaigns'],
        recommended: false
      },
      {
        name: 'Professional',
        price: 249,
        features: ['AI personalization', 'Churn prediction', 'Multi-channel messaging', 'Loyalty programs'],
        recommended: true
      },
      {
        name: 'Premium',
        price: 349,
        features: ['Everything in Pro', 'Predictive analytics', 'Advanced segmentation', 'Custom rewards'],
        recommended: false
      }
    ],
    testimonial: {
      text: 'Customer retention improved by 28% and our clients actually thank us for the personalized service.',
      author: 'Michael Chen',
      company: 'Urban Fitness Studio'
    }
  },
  {
    id: 'inventory-management',
    name: 'Intelligent Inventory Optimization',
    description: 'AI prevents stockouts, optimizes ordering, and negotiates with suppliers',
    icon: '📦',
    category: 'operations',
    status: 'available',
    guaranteedOutcomes: [
      'Zero stockouts guaranteed',
      'Reduce inventory costs by 20%',
      'Automate supplier negotiations',
      'Predict demand with 95% accuracy'
    ],
    timeToValue: '21-30 days',
    avgROI: 320,
    successRate: 89,
    pricingTiers: [
      {
        name: 'Essential',
        price: 199,
        features: ['Basic demand forecasting', 'Reorder alerts', 'Supplier management'],
        recommended: false
      },
      {
        name: 'Professional',
        price: 299,
        features: ['AI demand prediction', 'Automated ordering', 'Price negotiations', 'Multi-supplier optimization'],
        recommended: true
      },
      {
        name: 'Enterprise',
        price: 399,
        features: ['Everything in Pro', 'Advanced analytics', 'Custom integrations', 'Dedicated success manager'],
        recommended: false
      }
    ]
  },
  {
    id: 'financial-intelligence',
    name: 'Automated Financial Management',
    description: 'AI handles bookkeeping, compliance, and financial optimization',
    icon: '💼',
    category: 'finance',
    status: 'available',
    guaranteedOutcomes: [
      '100% tax compliance guaranteed',
      'Reduce bookkeeping time by 95%',
      'Automate expense categorization',
      'Real-time financial insights'
    ],
    timeToValue: '30-45 days',
    avgROI: 280,
    successRate: 96,
    pricingTiers: [
      {
        name: 'Basic',
        price: 199,
        features: ['Expense tracking', 'Basic reporting', 'Tax preparation'],
        recommended: false
      },
      {
        name: 'Professional',
        price: 299,
        features: ['AI categorization', 'Compliance monitoring', 'Cash flow forecasting', 'Advanced reporting'],
        recommended: true
      },
      {
        name: 'Premium',
        price: 449,
        features: ['Everything in Pro', 'Financial planning', 'Investment advice', 'Tax optimization'],
        recommended: false
      }
    ]
  },
  {
    id: 'group-buying',
    name: 'Collaborative Procurement Network',
    description: 'Join group purchases to unlock bulk pricing and shared logistics',
    icon: '🤝',
    category: 'collaboration',
    status: 'available',
    guaranteedOutcomes: [
      'Save 15-40% on procurement costs',
      'Access to premium suppliers',
      'Shared logistics savings',
      'Quality guaranteed products'
    ],
    timeToValue: '45-60 days',
    avgROI: 250,
    successRate: 87,
    pricingTiers: [
      {
        name: 'Community',
        price: 99,
        features: ['Group purchase alerts', 'Basic coordination', 'Standard logistics'],
        recommended: false
      },
      {
        name: 'Business',
        price: 149,
        features: ['Priority access', 'Custom requests', 'Express logistics', 'Quality guarantees'],
        recommended: true
      },
      {
        name: 'Enterprise',
        price: 249,
        features: ['Everything in Business', 'Private groups', 'Dedicated coordinator', 'Volume guarantees'],
        recommended: false
      }
    ]
  }
]

const filteredServices = computed(() => {
  if (selectedCategory.value === 'all') {
    return services
  }
  return services.filter(service => service.category === selectedCategory.value)
})

const getCategoryName = (categoryId) => {
  return categories.find(cat => cat.id === categoryId)?.name || ''
}

const activateService = async (serviceId) => {
  const service = services.find(s => s.id === serviceId)
  selectedService.value = service
  showROICalculator.value = true
}

const selectTier = (serviceId, tier) => {
  // Handle tier selection
  console.log('Selected tier:', tier, 'for service:', serviceId)
}

const manageService = (serviceId) => {
  navigateTo(`/services/${serviceId}/manage`)
}

const learnMore = (serviceId) => {
  navigateTo(`/services/${serviceId}`)
}

const closeROICalculator = () => {
  showROICalculator.value = false
  selectedService.value = null
}

const calculateRevenueIncrease = () => {
  if (!selectedService.value || !roiInputs.value.revenue) return 0
  const increase = selectedService.value.id === 'sales-automation' ? 0.15 : 0.10
  return Math.round(roiInputs.value.revenue * increase)
}

const calculateCostSavings = () => {
  if (!roiInputs.value.costs) return 0
  return Math.round(roiInputs.value.costs * 0.08)
}

const calculateTimeSavings = () => {
  if (!roiInputs.value.manualHours) return 0
  return Math.round(roiInputs.value.manualHours * 0.6)
}

const calculateTotalValue = () => {
  return calculateRevenueIncrease() + calculateCostSavings() + (calculateTimeSavings() * 150)
}

const calculateNetProfit = () => {
  if (!selectedService.value) return 0
  return calculateTotalValue() - selectedService.value.pricing.standard
}

const calculateROI = () => {
  if (!selectedService.value || selectedService.value.pricing.standard === 0) return 0
  return Math.round((calculateNetProfit() / selectedService.value.pricing.standard) * 100)
}

const activateFromCalculator = async () => {
  if (!selectedService.value) return
  
  try {
    await $fetch('/api/services/manage', {
      method: 'POST',
      body: {
        action: 'activate',
        serviceId: selectedService.value.id,
        configuration: {
          tier: 'professional',
          projectedROI: calculateROI()
        }
      }
    })
    
    closeROICalculator()
    
    // Show success message and redirect to dashboard
    await navigateTo('/dashboard/service')
  } catch (error) {
    console.error('Service activation failed:', error)
  }
}
</script>

<style scoped>
.services-catalog {
  padding: 2rem;
  max-width: 1400px;
  margin: 0 auto;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  min-height: 100vh;
  color: white;
}

.catalog-header {
  text-align: center;
  margin-bottom: 3rem;
}

.catalog-header h1 {
  font-size: 3rem;
  font-weight: 700;
  margin-bottom: 1rem;
}

.subtitle {
  font-size: 1.25rem;
  opacity: 0.9;
  margin-bottom: 2rem;
}

.value-proposition {
  display: flex;
  justify-content: center;
  gap: 2rem;
  flex-wrap: wrap;
}

.value-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  background: rgba(255, 255, 255, 0.1);
  padding: 0.75rem 1.5rem;
  border-radius: 25px;
  backdrop-filter: blur(10px);
}

.value-item .icon {
  font-size: 1.25rem;
}

.categories {
  display: flex;
  justify-content: center;
  gap: 1rem;
  margin-bottom: 3rem;
  flex-wrap: wrap;
}

.category-btn {
  padding: 0.75rem 1.5rem;
  border: 2px solid rgba(255, 255, 255, 0.3);
  background: rgba(255, 255, 255, 0.1);
  color: white;
  border-radius: 25px;
  cursor: pointer;
  transition: all 0.3s;
  backdrop-filter: blur(10px);
}

.category-btn:hover,
.category-btn.active {
  background: rgba(255, 255, 255, 0.2);
  border-color: rgba(255, 255, 255, 0.5);
  transform: translateY(-2px);
}

.featured-section {
  margin-bottom: 4rem;
}

.featured-section h2 {
  text-align: center;
  font-size: 2rem;
  margin-bottom: 2rem;
}

.featured-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(350px, 1fr));
  gap: 2rem;
}

.featured-card {
  background: rgba(255, 255, 255, 0.15);
  backdrop-filter: blur(20px);
  border-radius: 20px;
  padding: 2rem;
  text-align: center;
  position: relative;
  border: 2px solid rgba(255, 255, 255, 0.2);
  transition: transform 0.3s;
}

.featured-card:hover {
  transform: translateY(-5px);
}

.service-badge {
  position: absolute;
  top: -10px;
  right: 20px;
  background: #ff6b6b;
  color: white;
  padding: 0.5rem 1rem;
  border-radius: 20px;
  font-size: 0.75rem;
  font-weight: 600;
}

.service-icon {
  font-size: 3rem;
  margin-bottom: 1rem;
}

.featured-card h3 {
  font-size: 1.5rem;
  margin-bottom: 0.5rem;
}

.service-tagline {
  opacity: 0.9;
  margin-bottom: 1.5rem;
}

.guaranteed-outcome {
  background: rgba(74, 222, 128, 0.2);
  border: 1px solid rgba(74, 222, 128, 0.3);
  border-radius: 10px;
  padding: 1rem;
  margin-bottom: 1.5rem;
}

.guarantee-label {
  display: block;
  font-size: 0.875rem;
  opacity: 0.8;
  margin-bottom: 0.5rem;
}

.guarantee-text {
  font-weight: 600;
  color: #4ade80;
}

.service-pricing {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.price {
  font-size: 1.5rem;
  font-weight: 700;
  color: #fbbf24;
}

.roi {
  color: #4ade80;
  font-weight: 600;
}

.services-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
  gap: 2rem;
}

.service-card {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 20px;
  padding: 2rem;
  border: 1px solid rgba(255, 255, 255, 0.2);
  transition: all 0.3s;
}

.service-card:hover {
  transform: translateY(-3px);
  border-color: rgba(255, 255, 255, 0.4);
}

.service-header {
  display: flex;
  align-items: flex-start;
  gap: 1rem;
  margin-bottom: 2rem;
}

.service-header .service-icon {
  font-size: 2rem;
  flex-shrink: 0;
}

.service-info {
  flex: 1;
}

.service-info h3 {
  font-size: 1.25rem;
  margin-bottom: 0.5rem;
}

.service-description {
  opacity: 0.9;
  font-size: 0.875rem;
}

.service-status {
  padding: 0.5rem 1rem;
  border-radius: 20px;
  font-size: 0.75rem;
  font-weight: 600;
  background: rgba(74, 222, 128, 0.2);
  color: #4ade80;
  border: 1px solid rgba(74, 222, 128, 0.3);
}

.guaranteed-outcomes {
  margin-bottom: 2rem;
}

.guaranteed-outcomes h4 {
  font-size: 1rem;
  margin-bottom: 1rem;
  color: #fbbf24;
}

.guaranteed-outcomes ul {
  list-style: none;
  padding: 0;
  display: grid;
  gap: 0.5rem;
}

.guaranteed-outcomes li {
  background: rgba(255, 255, 255, 0.1);
  padding: 0.75rem;
  border-radius: 8px;
  font-size: 0.875rem;
}

.service-metrics {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1rem;
  margin-bottom: 2rem;
}

.metric {
  text-align: center;
  background: rgba(255, 255, 255, 0.1);
  padding: 1rem;
  border-radius: 10px;
}

.metric .label {
  display: block;
  font-size: 0.75rem;
  opacity: 0.8;
  margin-bottom: 0.5rem;
}

.metric .value {
  font-size: 1.25rem;
  font-weight: 700;
  color: #4ade80;
}

.pricing-tiers {
  margin-bottom: 2rem;
}

.pricing-tiers h4 {
  margin-bottom: 1rem;
  color: #fbbf24;
}

.tiers {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 1rem;
}

.tier {
  background: rgba(255, 255, 255, 0.1);
  border-radius: 10px;
  padding: 1rem;
  text-align: center;
  border: 2px solid transparent;
}

.tier.recommended {
  border-color: #fbbf24;
  background: rgba(251, 191, 36, 0.1);
}

.tier-name {
  font-weight: 600;
  margin-bottom: 0.5rem;
}

.tier-price {
  font-size: 1.25rem;
  font-weight: 700;
  color: #4ade80;
  margin-bottom: 1rem;
}

.tier-features {
  margin-bottom: 1rem;
}

.feature {
  font-size: 0.75rem;
  margin-bottom: 0.25rem;
  opacity: 0.9;
}

.btn-select-tier {
  width: 100%;
  padding: 0.5rem;
  border: none;
  border-radius: 6px;
  background: rgba(255, 255, 255, 0.2);
  color: white;
  cursor: pointer;
  font-size: 0.75rem;
}

.btn-select-tier.primary {
  background: #fbbf24;
  color: #1f2937;
  font-weight: 600;
}

.service-actions {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
}

.btn-activate, .btn-manage, .btn-learn-more {
  flex: 1;
  padding: 0.75rem 1rem;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.2s;
}

.btn-activate {
  background: #4ade80;
  color: #1f2937;
}

.btn-manage {
  background: #3b82f6;
  color: white;
}

.btn-learn-more {
  background: rgba(255, 255, 255, 0.2);
  color: white;
}

.btn-activate:hover {
  background: #22c55e;
  transform: translateY(-1px);
}

.social-proof {
  background: rgba(255, 255, 255, 0.1);
  border-radius: 10px;
  padding: 1.5rem;
  border-left: 4px solid #4ade80;
}

.testimonial p {
  font-style: italic;
  margin-bottom: 1rem;
  opacity: 0.9;
}

.author {
  font-size: 0.875rem;
  opacity: 0.8;
}

.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.8);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: 2rem;
}

.roi-calculator {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 20px;
  padding: 2rem;
  max-width: 600px;
  width: 100%;
  color: white;
  border: 2px solid rgba(255, 255, 255, 0.2);
}

.roi-calculator h3 {
  text-align: center;
  margin-bottom: 2rem;
  font-size: 1.5rem;
}

.calculator-form {
  margin-bottom: 2rem;
}

.form-group {
  margin-bottom: 1rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 600;
}

.form-group input {
  width: 100%;
  padding: 0.75rem;
  border: none;
  border-radius: 8px;
  background: rgba(255, 255, 255, 0.1);
  color: white;
  font-size: 1rem;
}

.form-group input::placeholder {
  color: rgba(255, 255, 255, 0.6);
}

.roi-results {
  margin-bottom: 2rem;
}

.roi-results h4 {
  margin-bottom: 1rem;
  color: #fbbf24;
}

.projection {
  background: rgba(255, 255, 255, 0.1);
  border-radius: 10px;
  padding: 1.5rem;
}

.projection-item {
  display: flex;
  justify-content: space-between;
  margin-bottom: 0.75rem;
  padding-bottom: 0.75rem;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.projection-item.total {
  border-top: 2px solid rgba(255, 255, 255, 0.3);
  margin-top: 1rem;
  padding-top: 1rem;
  font-weight: 700;
}

.projection-item.profit {
  font-size: 1.125rem;
  font-weight: 700;
}

.projection-item.roi {
  font-size: 1.25rem;
  font-weight: 700;
  color: #4ade80;
}

.positive {
  color: #4ade80;
  font-weight: 600;
}

.calculator-actions {
  display: flex;
  gap: 1rem;
}

.btn-close, .btn-activate {
  flex: 1;
  padding: 0.75rem 1rem;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 600;
}

.btn-close {
  background: rgba(255, 255, 255, 0.2);
  color: white;
}

.btn-activate {
  background: #4ade80;
  color: #1f2937;
}

h2 {
  text-align: center;
  font-size: 2rem;
  margin-bottom: 2rem;
}
</style>
