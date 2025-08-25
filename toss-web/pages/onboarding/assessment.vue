<template>
  <div class="assessment-page">
    <div class="assessment-container">
      <!-- Progress Indicator -->
      <div class="progress-header">
        <div class="progress-bar">
          <div class="progress-fill" :style="{ width: progress + '%' }"></div>
        </div>
        <p class="progress-text">{{ currentStep }}/{{ totalSteps }} - {{ stepTitle }}</p>
      </div>

      <!-- Assessment Steps -->
      <div class="assessment-content">
        <!-- Step 1: Business Basics -->
        <div v-if="currentStep === 1" class="step-content">
          <h2>Let's Get to Know Your Business</h2>
          <p class="step-description">Tell us about your business so we can identify the best opportunities for growth and automation.</p>
          
          <div class="form-group">
            <label>Business Name</label>
            <input v-model="assessment.businessName" type="text" placeholder="Enter your business name" />
          </div>

          <div class="form-group">
            <label>Industry</label>
            <select v-model="assessment.industry">
              <option value="">Select your industry</option>
              <option value="retail">Retail & General Store</option>
              <option value="restaurant">Restaurant & Food Service</option>
              <option value="beauty">Beauty & Wellness</option>
              <option value="automotive">Automotive & Repair</option>
              <option value="construction">Construction & Trades</option>
              <option value="healthcare">Healthcare & Medical</option>
              <option value="technology">Technology & Services</option>
              <option value="manufacturing">Manufacturing</option>
              <option value="other">Other</option>
            </select>
          </div>

          <div class="form-group">
            <label>Business Size</label>
            <div class="radio-group">
              <label class="radio-option">
                <input v-model="assessment.businessSize" type="radio" value="solo" />
                <span>Solo (Just me)</span>
              </label>
              <label class="radio-option">
                <input v-model="assessment.businessSize" type="radio" value="small" />
                <span>Small (2-10 employees)</span>
              </label>
              <label class="radio-option">
                <input v-model="assessment.businessSize" type="radio" value="medium" />
                <span>Medium (11-50 employees)</span>
              </label>
            </div>
          </div>

          <div class="form-group">
            <label>Monthly Revenue (Optional)</label>
            <select v-model="assessment.monthlyRevenue">
              <option value="">Prefer not to say</option>
              <option value="under-10k">Under R10,000</option>
              <option value="10k-50k">R10,000 - R50,000</option>
              <option value="50k-100k">R50,000 - R100,000</option>
              <option value="100k-500k">R100,000 - R500,000</option>
              <option value="over-500k">Over R500,000</option>
            </select>
          </div>
        </div>

        <!-- Step 2: Current Challenges -->
        <div v-if="currentStep === 2" class="step-content">
          <h2>What Challenges Keep You Up at Night?</h2>
          <p class="step-description">Select the biggest challenges your business faces. Our AI will focus on solving these first.</p>
          
          <div class="challenge-grid">
            <div 
              v-for="challenge in challenges" 
              :key="challenge.id"
              class="challenge-card"
              :class="{ active: assessment.selectedChallenges.includes(challenge.id) }"
              @click="toggleChallenge(challenge.id)"
            >
              <div class="challenge-icon">{{ challenge.icon }}</div>
              <h3>{{ challenge.title }}</h3>
              <p>{{ challenge.description }}</p>
              <div class="challenge-impact">{{ challenge.impact }}</div>
            </div>
          </div>
        </div>

        <!-- Step 3: Current Tools -->
        <div v-if="currentStep === 3" class="step-content">
          <h2>What Tools Do You Currently Use?</h2>
          <p class="step-description">Understanding your current setup helps us integrate smoothly and avoid disruption.</p>
          
          <div class="tools-section">
            <h3>Accounting & Finance</h3>
            <div class="checkbox-group">
              <label v-for="tool in accountingTools" :key="tool" class="checkbox-option">
                <input v-model="assessment.currentTools.accounting" type="checkbox" :value="tool" />
                <span>{{ tool }}</span>
              </label>
            </div>
          </div>

          <div class="tools-section">
            <h3>Sales & Customer Management</h3>
            <div class="checkbox-group">
              <label v-for="tool in salesTools" :key="tool" class="checkbox-option">
                <input v-model="assessment.currentTools.sales" type="checkbox" :value="tool" />
                <span>{{ tool }}</span>
              </label>
            </div>
          </div>

          <div class="tools-section">
            <h3>Inventory & Stock Management</h3>
            <div class="checkbox-group">
              <label v-for="tool in inventoryTools" :key="tool" class="checkbox-option">
                <input v-model="assessment.currentTools.inventory" type="checkbox" :value="tool" />
                <span>{{ tool }}</span>
              </label>
            </div>
          </div>
        </div>

        <!-- Step 4: Goals & Outcomes -->
        <div v-if="currentStep === 4" class="step-content">
          <h2>What Success Looks Like for You</h2>
          <p class="step-description">Choose the outcomes that matter most to your business. We'll guarantee these results.</p>
          
          <div class="goals-grid">
            <div 
              v-for="goal in businessGoals" 
              :key="goal.id"
              class="goal-card"
              :class="{ active: assessment.desiredOutcomes.includes(goal.id) }"
              @click="toggleGoal(goal.id)"
            >
              <div class="goal-icon">{{ goal.icon }}</div>
              <h3>{{ goal.title }}</h3>
              <p>{{ goal.description }}</p>
              <div class="goal-guarantee">{{ goal.guarantee }}</div>
            </div>
          </div>
        </div>

        <!-- Step 5: Assessment Results -->
        <div v-if="currentStep === 5" class="step-content">
          <div v-if="assessmentLoading" class="loading-state">
            <div class="ai-analyzing">
              <div class="ai-icon">🤖</div>
              <h2>AI Analyzing Your Business...</h2>
              <p>Our AI is creating a personalized business transformation plan based on your responses.</p>
              <div class="analysis-steps">
                <div class="analysis-step" :class="{ active: currentAnalysisStep >= 1 }">
                  <span class="step-dot"></span>
                  <span>Identifying opportunities</span>
                </div>
                <div class="analysis-step" :class="{ active: currentAnalysisStep >= 2 }">
                  <span class="step-dot"></span>
                  <span>Calculating ROI potential</span>
                </div>
                <div class="analysis-step" :class="{ active: currentAnalysisStep >= 3 }">
                  <span class="step-dot"></span>
                  <span>Matching optimal services</span>
                </div>
                <div class="analysis-step" :class="{ active: currentAnalysisStep >= 4 }">
                  <span class="step-dot"></span>
                  <span>Creating implementation plan</span>
                </div>
              </div>
            </div>
          </div>

          <div v-else class="results-content">
            <div class="results-header">
              <h2>🎯 Your Personalized Business Transformation Plan</h2>
              <p>Based on your assessment, here's how TOSS Service as Software will transform your business:</p>
            </div>

            <div class="business-health-score">
              <div class="score-circle">
                <div class="score-value">{{ assessmentResults.healthScore }}%</div>
                <div class="score-label">Business Health</div>
              </div>
              <div class="score-insights">
                <h3>Key Insights</h3>
                <ul>
                  <li v-for="insight in assessmentResults.insights" :key="insight">{{ insight }}</li>
                </ul>
              </div>
            </div>

            <div class="recommended-services">
              <h3>Recommended Services for {{ assessment.businessName }}</h3>
              <div class="service-recommendations">
                <div v-for="service in assessmentResults.recommendedServices" :key="service.id" class="recommended-service">
                  <div class="service-header">
                    <div class="service-icon">{{ service.icon }}</div>
                    <div class="service-info">
                      <h4>{{ service.name }}</h4>
                      <p>{{ service.description }}</p>
                    </div>
                    <div class="service-priority">
                      <span class="priority-badge" :class="service.priority.toLowerCase()">{{ service.priority }}</span>
                    </div>
                  </div>
                  <div class="service-outcomes">
                    <div class="outcome-guarantee">
                      <strong>Guaranteed Outcome:</strong> {{ service.guarantee }}
                    </div>
                    <div class="outcome-timeline">
                      <strong>Timeline:</strong> {{ service.timeline }}
                    </div>
                    <div class="outcome-roi">
                      <strong>Expected ROI:</strong> {{ service.roi }}
                    </div>
                  </div>
                  <div class="service-investment">
                    <div class="investment-breakdown">
                      <span class="investment-amount">{{ service.monthlyPrice }}</span>
                      <span class="investment-value">Expected Value: {{ service.expectedValue }}</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <div class="implementation-roadmap">
              <h3>90-Day Implementation Roadmap</h3>
              <div class="roadmap-timeline">
                <div v-for="phase in assessmentResults.roadmap" :key="phase.phase" class="roadmap-phase">
                  <div class="phase-number">{{ phase.phase }}</div>
                  <div class="phase-content">
                    <h4>{{ phase.title }}</h4>
                    <p>{{ phase.description }}</p>
                    <div class="phase-outcomes">
                      <span v-for="outcome in phase.outcomes" :key="outcome" class="phase-outcome">{{ outcome }}</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <div class="investment-summary">
              <h3>Investment Summary</h3>
              <div class="investment-grid">
                <div class="investment-card">
                  <div class="investment-label">Monthly Investment</div>
                  <div class="investment-amount">{{ assessmentResults.totalMonthlyInvestment }}</div>
                </div>
                <div class="investment-card">
                  <div class="investment-label">Expected Monthly Value</div>
                  <div class="investment-amount value">{{ assessmentResults.expectedMonthlyValue }}</div>
                </div>
                <div class="investment-card">
                  <div class="investment-label">Monthly Net Gain</div>
                  <div class="investment-amount gain">{{ assessmentResults.monthlyNetGain }}</div>
                </div>
                <div class="investment-card">
                  <div class="investment-label">Annual ROI</div>
                  <div class="investment-amount roi">{{ assessmentResults.annualROI }}</div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Navigation -->
      <div class="assessment-navigation">
        <button 
          v-if="currentStep > 1 && currentStep < 5" 
          @click="previousStep" 
          class="btn-secondary"
        >
          Previous
        </button>
        
        <button 
          v-if="currentStep < 4" 
          @click="nextStep" 
          :disabled="!canProceed"
          class="btn-primary"
        >
          Continue
        </button>
        
        <button 
          v-if="currentStep === 4" 
          @click="generateAssessment" 
          :disabled="!canProceed"
          class="btn-primary"
        >
          Analyze My Business
        </button>

        <div v-if="currentStep === 5 && !assessmentLoading" class="final-actions">
          <button @click="activateServices" class="btn-large">
            Activate Recommended Services
          </button>
          <button @click="customizeServices" class="btn-secondary">
            Customize My Plan
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
const currentStep = ref(1)
const totalSteps = 5
const assessmentLoading = ref(false)
const currentAnalysisStep = ref(0)

const assessment = ref({
  businessName: '',
  industry: '',
  businessSize: '',
  monthlyRevenue: '',
  selectedChallenges: [],
  currentTools: {
    accounting: [],
    sales: [],
    inventory: []
  },
  desiredOutcomes: []
})

const assessmentResults = ref({})

const stepTitles = {
  1: 'Business Basics',
  2: 'Current Challenges',
  3: 'Current Tools',
  4: 'Desired Outcomes',
  5: 'Your Results'
}

const challenges = [
  {
    id: 'cash-flow',
    icon: '💰',
    title: 'Cash Flow Issues',
    description: 'Unpredictable income, late payments, or cash shortages',
    impact: 'High stress, limited growth'
  },
  {
    id: 'inventory',
    icon: '📦',
    title: 'Inventory Problems',
    description: 'Stockouts, overstocking, or poor inventory tracking',
    impact: 'Lost sales, wasted money'
  },
  {
    id: 'customer-retention',
    icon: '❤️',
    title: 'Customer Retention',
    description: 'Customers not returning or low repeat business',
    impact: 'Reduced lifetime value'
  },
  {
    id: 'time-management',
    icon: '⏰',
    title: 'Time Management',
    description: 'Too much admin work, not enough time for core business',
    impact: 'Burnout, missed opportunities'
  },
  {
    id: 'competition',
    icon: '🏆',
    title: 'Competition',
    description: 'Struggling to compete with larger businesses',
    impact: 'Market share loss'
  },
  {
    id: 'growth-barriers',
    icon: '📈',
    title: 'Growth Barriers',
    description: 'Want to grow but lack systems or capital',
    impact: 'Stagnant business'
  }
]

const businessGoals = [
  {
    id: 'increase-revenue',
    icon: '💰',
    title: 'Increase Revenue',
    description: 'Grow sales through better customer management and automation',
    guarantee: '15% revenue increase in 60 days'
  },
  {
    id: 'reduce-costs',
    icon: '💡',
    title: 'Reduce Operating Costs',
    description: 'Cut expenses through smart automation and optimization',
    guarantee: '20% cost reduction through automation'
  },
  {
    id: 'save-time',
    icon: '⏰',
    title: 'Save Time',
    description: 'Automate routine tasks to focus on growing your business',
    guarantee: '40+ hours saved per month'
  },
  {
    id: 'improve-compliance',
    icon: '📋',
    title: 'Ensure Compliance',
    description: 'Stay compliant with tax and business regulations automatically',
    guarantee: '100% compliance with automated reporting'
  },
  {
    id: 'better-insights',
    icon: '📊',
    title: 'Better Business Insights',
    description: 'Real-time data and AI recommendations for better decisions',
    guarantee: 'Daily business intelligence reports'
  },
  {
    id: 'customer-satisfaction',
    icon: '😊',
    title: 'Improve Customer Satisfaction',
    description: 'Enhance customer experience and build loyalty',
    guarantee: '25% improvement in customer retention'
  }
]

const accountingTools = ['Excel/Google Sheets', 'Pastel', 'Sage', 'QuickBooks', 'Xero', 'None - Manual']
const salesTools = ['WhatsApp Business', 'Facebook/Instagram', 'Point of Sale System', 'CRM Software', 'None - Manual']
const inventoryTools = ['Excel/Google Sheets', 'Point of Sale System', 'Dedicated Inventory Software', 'None - Manual']

const progress = computed(() => (currentStep.value / totalSteps) * 100)
const stepTitle = computed(() => stepTitles[currentStep.value])

const canProceed = computed(() => {
  switch (currentStep.value) {
    case 1:
      return assessment.value.businessName && assessment.value.industry && assessment.value.businessSize
    case 2:
      return assessment.value.selectedChallenges.length > 0
    case 3:
      return true // Optional step
    case 4:
      return assessment.value.desiredOutcomes.length > 0
    default:
      return true
  }
})

const toggleChallenge = (challengeId) => {
  const index = assessment.value.selectedChallenges.indexOf(challengeId)
  if (index > -1) {
    assessment.value.selectedChallenges.splice(index, 1)
  } else {
    assessment.value.selectedChallenges.push(challengeId)
  }
}

const toggleGoal = (goalId) => {
  const index = assessment.value.desiredOutcomes.indexOf(goalId)
  if (index > -1) {
    assessment.value.desiredOutcomes.splice(index, 1)
  } else {
    assessment.value.desiredOutcomes.push(goalId)
  }
}

const nextStep = () => {
  if (canProceed.value && currentStep.value < 4) {
    currentStep.value++
  }
}

const previousStep = () => {
  if (currentStep.value > 1) {
    currentStep.value--
  }
}

const generateAssessment = async () => {
  currentStep.value = 5
  assessmentLoading.value = true
  
  // Simulate AI analysis
  for (let i = 1; i <= 4; i++) {
    await new Promise(resolve => setTimeout(resolve, 1500))
    currentAnalysisStep.value = i
  }
  
  // Generate personalized results based on assessment
  await new Promise(resolve => setTimeout(resolve, 1000))
  
  // Mock AI-generated results based on user input
  assessmentResults.value = generatePersonalizedResults()
  assessmentLoading.value = false
}

const generatePersonalizedResults = () => {
  const challenges = assessment.value.selectedChallenges
  const outcomes = assessment.value.desiredOutcomes
  const industry = assessment.value.industry
  const businessSize = assessment.value.businessSize
  
  // AI-like logic to determine health score
  let healthScore = 65
  if (challenges.length <= 2) healthScore += 15
  if (outcomes.length >= 3) healthScore += 10
  if (assessment.value.currentTools.accounting.length > 0) healthScore += 5
  if (assessment.value.currentTools.sales.length > 0) healthScore += 5
  
  // Generate insights based on challenges
  const insights = []
  if (challenges.includes('cash-flow')) {
    insights.push('Sales AI can improve payment collection by 40%')
  }
  if (challenges.includes('inventory')) {
    insights.push('Smart Inventory AI can prevent 95% of stockouts')
  }
  if (challenges.includes('customer-retention')) {
    insights.push('Customer Success AI can boost retention by 25%')
  }
  if (challenges.includes('time-management')) {
    insights.push('Automation can save 40+ hours monthly')
  }
  
  // Determine recommended services
  const allServices = [
    {
      id: 'sales-ai',
      name: 'Sales Automation AI',
      icon: '💰',
      description: 'Automates invoicing, payment collection, and sales follow-ups',
      guarantee: '15% revenue increase in 60 days',
      timeline: 'Results in 2-3 weeks',
      roi: '400% annual ROI',
      monthlyPrice: 'R499/month',
      expectedValue: 'R2,000/month',
      priority: challenges.includes('cash-flow') ? 'HIGH' : 'MEDIUM'
    },
    {
      id: 'inventory-ai',
      name: 'Smart Inventory Management',
      icon: '📦',
      description: 'Prevents stockouts and optimizes ordering costs',
      guarantee: 'Zero stockouts + 20% cost reduction',
      timeline: 'Results in 1-2 weeks',
      roi: '320% annual ROI',
      monthlyPrice: 'R299/month',
      expectedValue: 'R800/month',
      priority: challenges.includes('inventory') ? 'HIGH' : 'LOW'
    },
    {
      id: 'customer-ai',
      name: 'Customer Success AI',
      icon: '❤️',
      description: 'Manages relationships to maximize satisfaction and retention',
      guarantee: '25% retention improvement',
      timeline: 'Results in 3-4 weeks',
      roi: '380% annual ROI',
      monthlyPrice: 'R249/month',
      expectedValue: 'R1,200/month',
      priority: challenges.includes('customer-retention') ? 'HIGH' : 'MEDIUM'
    },
    {
      id: 'financial-ai',
      name: 'Financial Intelligence',
      icon: '📊',
      description: 'Handles bookkeeping and ensures compliance',
      guarantee: '100% compliance + 90% time savings',
      timeline: 'Results in 1 week',
      roi: '280% annual ROI',
      monthlyPrice: 'R299/month',
      expectedValue: 'R1,000/month',
      priority: 'MEDIUM'
    }
  ]
  
  // Filter and sort by priority
  const recommendedServices = allServices
    .filter(service => {
      if (service.priority === 'HIGH') return true
      if (service.priority === 'MEDIUM' && outcomes.length >= 2) return true
      return false
    })
    .sort((a, b) => {
      const priorityOrder = { 'HIGH': 3, 'MEDIUM': 2, 'LOW': 1 }
      return priorityOrder[b.priority] - priorityOrder[a.priority]
    })
  
  // Calculate totals
  const totalMonthly = recommendedServices.reduce((sum, service) => sum + parseInt(service.monthlyPrice.replace(/[^0-9]/g, '')), 0)
  const totalValue = recommendedServices.reduce((sum, service) => sum + parseInt(service.expectedValue.replace(/[^0-9]/g, '')), 0)
  
  return {
    healthScore,
    insights,
    recommendedServices,
    roadmap: [
      {
        phase: 1,
        title: 'Week 1-2: Foundation Setup',
        description: 'Deploy AI services and integrate with existing systems',
        outcomes: ['Services activated', 'Data integrated', 'Team training complete']
      },
      {
        phase: 2,
        title: 'Week 3-6: Optimization Phase',
        description: 'AI learns your business patterns and optimizes performance',
        outcomes: ['AI fully trained', 'Processes automated', 'Initial results visible']
      },
      {
        phase: 3,
        title: 'Week 7-12: Growth Acceleration',
        description: 'AI delivers guaranteed outcomes and scales with growth',
        outcomes: ['Guaranteed outcomes achieved', 'Business scaling', 'ROI maximized']
      }
    ],
    totalMonthlyInvestment: `R${totalMonthly.toLocaleString()}`,
    expectedMonthlyValue: `R${totalValue.toLocaleString()}`,
    monthlyNetGain: `R${(totalValue - totalMonthly).toLocaleString()}`,
    annualROI: `${Math.round(((totalValue - totalMonthly) * 12 / totalMonthly) * 100)}%`
  }
}

const activateServices = () => {
  // Navigate to service activation
  navigateTo('/services/activate')
}

const customizeServices = () => {
  // Navigate to service customization
  navigateTo('/services')
}

useHead({
  title: 'Business Assessment - TOSS Service as Software',
  meta: [
    { name: 'description', content: 'Get a personalized business transformation plan with guaranteed outcomes from AI-powered services.' }
  ]
})
</script>

<style scoped>
.assessment-page {
  min-height: 100vh;
  background: linear-gradient(135deg, #1f2937 0%, #111827 100%);
  color: white;
  padding: 2rem;
}

.assessment-container {
  max-width: 900px;
  margin: 0 auto;
}

.progress-header {
  margin-bottom: 3rem;
  text-align: center;
}

.progress-bar {
  width: 100%;
  height: 8px;
  background: rgba(255, 255, 255, 0.2);
  border-radius: 4px;
  margin-bottom: 1rem;
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  background: linear-gradient(45deg, #4ade80, #22c55e);
  transition: width 0.5s ease;
}

.progress-text {
  opacity: 0.8;
  font-size: 0.875rem;
}

.step-content {
  margin-bottom: 3rem;
}

.step-content h2 {
  font-size: 2.5rem;
  font-weight: 700;
  margin-bottom: 1rem;
  text-align: center;
}

.step-description {
  font-size: 1.125rem;
  opacity: 0.9;
  text-align: center;
  margin-bottom: 3rem;
  line-height: 1.6;
}

.form-group {
  margin-bottom: 2rem;
}

.form-group label {
  display: block;
  font-weight: 600;
  margin-bottom: 0.75rem;
  font-size: 1rem;
}

.form-group input, .form-group select {
  width: 100%;
  padding: 1rem;
  border: 2px solid rgba(255, 255, 255, 0.2);
  border-radius: 12px;
  background: rgba(255, 255, 255, 0.1);
  color: white;
  font-size: 1rem;
  backdrop-filter: blur(10px);
}

.form-group input::placeholder {
  color: rgba(255, 255, 255, 0.6);
}

.form-group input:focus, .form-group select:focus {
  outline: none;
  border-color: #4ade80;
  box-shadow: 0 0 0 3px rgba(74, 222, 128, 0.2);
}

.radio-group {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.radio-option {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 1rem;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 12px;
  cursor: pointer;
  transition: all 0.3s;
  border: 2px solid transparent;
}

.radio-option:hover {
  background: rgba(255, 255, 255, 0.15);
}

.radio-option input:checked + span {
  font-weight: 600;
}

.radio-option:has(input:checked) {
  border-color: #4ade80;
  background: rgba(74, 222, 128, 0.2);
}

.challenge-grid, .goals-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 1.5rem;
}

.challenge-card, .goal-card {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 16px;
  padding: 2rem;
  cursor: pointer;
  transition: all 0.3s;
  border: 2px solid transparent;
  text-align: center;
}

.challenge-card:hover, .goal-card:hover {
  transform: translateY(-5px);
  background: rgba(255, 255, 255, 0.15);
}

.challenge-card.active, .goal-card.active {
  border-color: #4ade80;
  background: rgba(74, 222, 128, 0.2);
  transform: translateY(-5px);
}

.challenge-icon, .goal-icon {
  font-size: 2.5rem;
  margin-bottom: 1rem;
}

.challenge-card h3, .goal-card h3 {
  font-size: 1.25rem;
  font-weight: 600;
  margin-bottom: 1rem;
}

.challenge-card p, .goal-card p {
  opacity: 0.9;
  line-height: 1.5;
  margin-bottom: 1rem;
}

.challenge-impact, .goal-guarantee {
  background: rgba(255, 255, 255, 0.1);
  padding: 0.75rem;
  border-radius: 8px;
  font-size: 0.875rem;
  font-weight: 600;
  color: #4ade80;
}

.tools-section {
  margin-bottom: 2.5rem;
}

.tools-section h3 {
  font-size: 1.25rem;
  font-weight: 600;
  margin-bottom: 1rem;
}

.checkbox-group {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1rem;
}

.checkbox-option {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 1rem;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 10px;
  cursor: pointer;
  transition: all 0.3s;
}

.checkbox-option:hover {
  background: rgba(255, 255, 255, 0.15);
}

.checkbox-option input:checked + span {
  font-weight: 600;
}

.loading-state {
  text-align: center;
  padding: 4rem 2rem;
}

.ai-analyzing {
  max-width: 500px;
  margin: 0 auto;
}

.ai-icon {
  font-size: 4rem;
  margin-bottom: 2rem;
  animation: bounce 2s infinite;
}

.ai-analyzing h2 {
  font-size: 2rem;
  margin-bottom: 1rem;
}

.ai-analyzing p {
  opacity: 0.9;
  margin-bottom: 3rem;
}

.analysis-steps {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  text-align: left;
}

.analysis-step {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1rem;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 10px;
  opacity: 0.5;
  transition: all 0.5s;
}

.analysis-step.active {
  opacity: 1;
  background: rgba(74, 222, 128, 0.2);
}

.step-dot {
  width: 12px;
  height: 12px;
  border-radius: 50%;
  background: #4ade80;
}

.results-content {
  max-width: 800px;
  margin: 0 auto;
}

.results-header {
  text-align: center;
  margin-bottom: 3rem;
}

.results-header h2 {
  font-size: 2.5rem;
  margin-bottom: 1rem;
}

.business-health-score {
  display: grid;
  grid-template-columns: auto 1fr;
  gap: 2rem;
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 20px;
  padding: 2rem;
  margin-bottom: 3rem;
  align-items: center;
}

.score-circle {
  text-align: center;
  background: linear-gradient(45deg, #4ade80, #22c55e);
  border-radius: 50%;
  width: 120px;
  height: 120px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  color: #1f2937;
}

.score-value {
  font-size: 2rem;
  font-weight: 700;
}

.score-label {
  font-size: 0.75rem;
  font-weight: 600;
}

.score-insights h3 {
  font-size: 1.25rem;
  margin-bottom: 1rem;
}

.score-insights ul {
  list-style: none;
  padding: 0;
}

.score-insights li {
  background: rgba(255, 255, 255, 0.1);
  padding: 0.75rem;
  border-radius: 8px;
  margin-bottom: 0.5rem;
  border-left: 4px solid #4ade80;
}

.recommended-services {
  margin-bottom: 3rem;
}

.recommended-services h3 {
  font-size: 1.75rem;
  margin-bottom: 2rem;
  text-align: center;
}

.service-recommendations {
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.recommended-service {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 16px;
  padding: 2rem;
  border: 2px solid rgba(255, 255, 255, 0.2);
}

.service-header {
  display: grid;
  grid-template-columns: auto 1fr auto;
  gap: 1rem;
  align-items: center;
  margin-bottom: 1.5rem;
}

.service-icon {
  font-size: 2rem;
}

.service-info h4 {
  font-size: 1.25rem;
  font-weight: 600;
  margin-bottom: 0.5rem;
}

.priority-badge {
  padding: 0.25rem 0.75rem;
  border-radius: 12px;
  font-size: 0.75rem;
  font-weight: 600;
}

.priority-badge.high {
  background: rgba(239, 68, 68, 0.2);
  color: #fca5a5;
}

.priority-badge.medium {
  background: rgba(245, 158, 11, 0.2);
  color: #fbbf24;
}

.service-outcomes {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.service-outcomes > div {
  background: rgba(255, 255, 255, 0.1);
  padding: 1rem;
  border-radius: 10px;
  font-size: 0.875rem;
}

.service-investment {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: rgba(74, 222, 128, 0.2);
  padding: 1rem;
  border-radius: 10px;
}

.investment-amount {
  font-size: 1.25rem;
  font-weight: 700;
  color: #4ade80;
}

.investment-value {
  font-size: 0.875rem;
  opacity: 0.9;
}

.implementation-roadmap {
  margin-bottom: 3rem;
}

.implementation-roadmap h3 {
  font-size: 1.75rem;
  margin-bottom: 2rem;
  text-align: center;
}

.roadmap-timeline {
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.roadmap-phase {
  display: grid;
  grid-template-columns: auto 1fr;
  gap: 2rem;
  align-items: start;
}

.phase-number {
  width: 50px;
  height: 50px;
  background: linear-gradient(45deg, #4ade80, #22c55e);
  color: #1f2937;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.25rem;
  font-weight: 700;
}

.phase-content {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 12px;
  padding: 1.5rem;
}

.phase-content h4 {
  font-size: 1.25rem;
  margin-bottom: 0.75rem;
}

.phase-content p {
  opacity: 0.9;
  margin-bottom: 1rem;
}

.phase-outcomes {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.phase-outcome {
  background: rgba(74, 222, 128, 0.2);
  padding: 0.25rem 0.75rem;
  border-radius: 12px;
  font-size: 0.75rem;
}

.investment-summary {
  margin-bottom: 3rem;
}

.investment-summary h3 {
  font-size: 1.75rem;
  margin-bottom: 2rem;
  text-align: center;
}

.investment-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1.5rem;
}

.investment-card {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 16px;
  padding: 2rem;
  text-align: center;
}

.investment-label {
  font-size: 0.875rem;
  opacity: 0.8;
  margin-bottom: 0.5rem;
}

.investment-card .investment-amount {
  font-size: 1.75rem;
  font-weight: 700;
}

.investment-amount.value {
  color: #60a5fa;
}

.investment-amount.gain {
  color: #4ade80;
}

.investment-amount.roi {
  color: #fbbf24;
}

.assessment-navigation {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 2rem 0;
}

.btn-primary, .btn-secondary, .btn-large {
  padding: 1rem 2rem;
  border-radius: 12px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
  border: none;
  font-size: 1rem;
}

.btn-primary {
  background: linear-gradient(45deg, #4ade80, #22c55e);
  color: #1f2937;
}

.btn-secondary {
  background: rgba(255, 255, 255, 0.1);
  color: white;
  border: 2px solid rgba(255, 255, 255, 0.3);
}

.btn-large {
  background: linear-gradient(45deg, #4ade80, #22c55e);
  color: #1f2937;
  padding: 1.25rem 3rem;
  border-radius: 15px;
  font-size: 1.125rem;
  font-weight: 700;
}

.btn-primary:hover, .btn-large:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(74, 222, 128, 0.3);
}

.btn-secondary:hover {
  background: rgba(255, 255, 255, 0.2);
  transform: translateY(-2px);
}

.btn-primary:disabled {
  opacity: 0.5;
  cursor: not-allowed;
  transform: none;
  box-shadow: none;
}

.final-actions {
  display: flex;
  gap: 1rem;
  width: 100%;
  justify-content: center;
}

/* Responsive Design */
@media (max-width: 768px) {
  .assessment-page {
    padding: 1rem;
  }

  .step-content h2 {
    font-size: 2rem;
  }

  .business-health-score {
    grid-template-columns: 1fr;
    text-align: center;
  }

  .service-header {
    grid-template-columns: 1fr;
    text-align: center;
    gap: 1rem;
  }

  .roadmap-phase {
    grid-template-columns: 1fr;
  }

  .final-actions {
    flex-direction: column;
  }

  .assessment-navigation {
    flex-direction: column;
    gap: 1rem;
  }
}

@keyframes bounce {
  0%, 20%, 50%, 80%, 100% { transform: translateY(0); }
  40% { transform: translateY(-10px); }
  60% { transform: translateY(-5px); }
}
</style>
