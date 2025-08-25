<template>
  <div class="service-dashboard">
    <!-- Business Health Overview -->
    <div class="hero-section">
      <div class="business-health">
        <h1>{{ businessName }} Business Command Center</h1>
        <div class="health-score">
          <div class="score-circle">
            <span class="score">{{ dashboard.summary.overallHealth }}</span>
            <span class="label">Business Health</span>
          </div>
          <div class="automation-level">
            <span class="percentage">{{ dashboard.summary.automationLevel }}%</span>
            <span class="label">Automated</span>
          </div>
        </div>
        <div class="growth-metrics">
          <div class="metric">
            <span class="value">{{ dashboard.summary.monthlyGrowth }}%</span>
            <span class="label">Monthly Growth</span>
          </div>
          <div class="metric">
            <span class="value">{{ dashboard.summary.customerSatisfaction }}/5</span>
            <span class="label">Customer Joy</span>
          </div>
          <div class="metric">
            <span class="value">{{ dashboard.summary.operationalEfficiency }}%</span>
            <span class="label">Efficiency</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Business Outcomes Grid -->
    <div class="outcomes-grid">
      <!-- Revenue Outcomes -->
      <div class="outcome-card revenue">
        <div class="card-header">
          <h3>💰 Revenue Optimization</h3>
          <div class="performance">
            <span class="current">R{{ formatCurrency(dashboard.outcomes.revenue.current) }}</span>
            <span class="growth">+{{ dashboard.outcomes.revenue.growth }}%</span>
          </div>
        </div>
        <div class="ai-contribution">
          <p>AI contributed <strong>{{ dashboard.outcomes.revenue.aiContribution }}%</strong> of this month's growth</p>
        </div>
        <div class="outcome-list">
          <div v-for="outcome in dashboard.outcomes.revenue.outcomes" :key="outcome" class="outcome-item">
            ✅ {{ outcome }}
          </div>
        </div>
      </div>

      <!-- Customer Outcomes -->
      <div class="outcome-card customers">
        <div class="card-header">
          <h3>👥 Customer Experience Excellence</h3>
          <div class="performance">
            <span class="satisfaction">{{ dashboard.outcomes.customers.satisfaction }}/5 ⭐</span>
            <span class="retention">{{ dashboard.outcomes.customers.retention }}% retention</span>
          </div>
        </div>
        <div class="outcome-list">
          <div v-for="outcome in dashboard.outcomes.customers.outcomes" :key="outcome" class="outcome-item">
            ✅ {{ outcome }}
          </div>
        </div>
      </div>

      <!-- Operations Outcomes -->
      <div class="outcome-card operations">
        <div class="card-header">
          <h3>⚙️ Operational Excellence</h3>
          <div class="performance">
            <span class="efficiency">{{ dashboard.outcomes.operations.efficiency }}% efficient</span>
            <span class="time-saved">{{ dashboard.outcomes.operations.timesSaved }}h saved</span>
          </div>
        </div>
        <div class="outcome-list">
          <div v-for="outcome in dashboard.outcomes.operations.outcomes" :key="outcome" class="outcome-item">
            ✅ {{ outcome }}
          </div>
        </div>
      </div>

      <!-- Financial Outcomes -->
      <div class="outcome-card financial">
        <div class="card-header">
          <h3>💼 Financial Health</h3>
          <div class="performance">
            <span class="margin">{{ dashboard.outcomes.financial.profitMargin }}% margin</span>
            <span class="compliance">{{ dashboard.outcomes.financial.taxCompliance }}% compliant</span>
          </div>
        </div>
        <div class="outcome-list">
          <div v-for="outcome in dashboard.outcomes.financial.outcomes" :key="outcome" class="outcome-item">
            ✅ {{ outcome }}
          </div>
        </div>
      </div>
    </div>

    <!-- Active Services Performance -->
    <div class="services-section">
      <h2>🤖 Your AI Business Agents</h2>
      <div class="services-grid">
        <div v-for="(service, id) in dashboard.services" :key="id" class="service-card">
          <div class="service-header">
            <h4>{{ service.name }}</h4>
            <div class="status" :class="service.status">{{ service.status }}</div>
          </div>
          <div class="service-performance">
            <div class="performance-score">{{ service.performance }}%</div>
            <div class="outcomes-delivered">{{ service.outcomes }} outcomes</div>
          </div>
          <div class="service-roi">
            <div class="investment">Investment: R{{ service.cost }}/month</div>
            <div class="returns">Value: R{{ formatCurrency(service.value) }}/month</div>
            <div class="roi">ROI: {{ service.roi }}%</div>
          </div>
          <div class="service-actions">
            <button @click="configureService(id)" class="btn-configure">Configure</button>
            <button @click="pauseService(id)" class="btn-pause">Pause</button>
          </div>
        </div>
      </div>
    </div>

    <!-- AI Agents Status -->
    <div class="automation-section">
      <h2>🔄 AI Agents Working for You</h2>
      <div class="agents-grid">
        <div v-for="agent in dashboard.automation.activeAgents" :key="agent.name" class="agent-card">
          <div class="agent-status">
            <div class="agent-name">{{ agent.name }}</div>
            <div class="status-indicator" :class="agent.status"></div>
          </div>
          <div class="agent-activity">
            <div class="last-action">
              <strong>Just completed:</strong> {{ agent.lastAction }}
            </div>
            <div class="next-action">
              <strong>Next up:</strong> {{ agent.nextAction }}
            </div>
          </div>
          <div class="confidence">
            Confidence: {{ Math.round(agent.confidence * 100) }}%
          </div>
        </div>
      </div>
    </div>

    <!-- Financial ROI Dashboard -->
    <div class="financial-section">
      <h2>💎 Investment Returns</h2>
      <div class="roi-dashboard">
        <div class="roi-summary">
          <div class="investment-box">
            <h4>Monthly Investment</h4>
            <div class="amount">R{{ formatCurrency(dashboard.financial.investment) }}</div>
          </div>
          <div class="returns-box">
            <h4>Value Delivered</h4>
            <div class="amount">R{{ formatCurrency(dashboard.financial.returns) }}</div>
          </div>
          <div class="profit-box">
            <h4>Net Profit</h4>
            <div class="amount profit">R{{ formatCurrency(dashboard.financial.netProfit) }}</div>
          </div>
          <div class="roi-box">
            <h4>ROI</h4>
            <div class="amount roi">{{ dashboard.financial.roi }}%</div>
          </div>
        </div>
        <div class="roi-breakdown">
          <div class="breakdown-item">
            <span>Revenue Increase:</span>
            <span>R{{ formatCurrency(dashboard.financial.breakdown.revenueIncrease) }}</span>
          </div>
          <div class="breakdown-item">
            <span>Cost Savings:</span>
            <span>R{{ formatCurrency(dashboard.financial.breakdown.costSavings) }}</span>
          </div>
          <div class="breakdown-item">
            <span>Time Value:</span>
            <span>R{{ formatCurrency(dashboard.financial.breakdown.timeValue) }}</span>
          </div>
          <div class="breakdown-item">
            <span>Compliance Value:</span>
            <span>R{{ formatCurrency(dashboard.financial.breakdown.complianceValue) }}</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Smart Alerts & Opportunities -->
    <div class="alerts-section">
      <h2>🚨 Smart Business Alerts</h2>
      <div class="alerts-grid">
        <div v-for="alert in dashboard.alerts" :key="alert.title" class="alert-card" :class="alert.type">
          <div class="alert-header">
            <h4>{{ alert.title }}</h4>
            <div class="priority" :class="alert.priority">{{ alert.priority }}</div>
          </div>
          <div class="alert-message">{{ alert.message }}</div>
          <div class="alert-value" v-if="alert.value">
            Potential Value: R{{ formatCurrency(alert.value) }}
          </div>
          <div class="alert-action">
            <button @click="takeAction(alert)" class="btn-action">{{ alert.action }}</button>
            <span class="deadline" v-if="alert.deadline !== 'No deadline'">⏰ {{ alert.deadline }}</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Future Predictions -->
    <div class="predictions-section">
      <h2>🔮 AI Business Predictions</h2>
      <div class="predictions-grid">
        <div class="prediction-card">
          <h4>Next Month Forecast</h4>
          <div class="prediction-metric">
            <span>Projected Revenue:</span>
            <span>R{{ formatCurrency(dashboard.predictions.nextMonth.projectedRevenue) }}</span>
          </div>
          <div class="prediction-metric">
            <span>Growth Rate:</span>
            <span>{{ dashboard.predictions.nextMonth.projectedGrowth }}%</span>
          </div>
          <div class="ai-recommendations">
            <h5>AI Recommendations:</h5>
            <ul>
              <li v-for="rec in dashboard.predictions.nextMonth.aiRecommendations" :key="rec">
                {{ rec }}
              </li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
// Set up demo tenant context for Service as Software platform
const { setDemoTenant } = useTenant()
setDemoTenant('demo-salon')

const { data: dashboard } = await $fetch('/api/dashboard/outcomes')
const businessName = dashboard.tenant.name

const formatCurrency = (amount) => {
  return new Intl.NumberFormat('en-ZA').format(amount)
}

const configureService = (serviceId) => {
  // Navigate to service configuration
  navigateTo(`/services/${serviceId}/configure`)
}

const pauseService = async (serviceId) => {
  await $fetch('/api/services/manage', {
    method: 'POST',
    body: {
      action: 'pause',
      serviceId
    }
  })
  // Refresh dashboard
  await refreshCookie('dashboard')
}

const takeAction = (alert) => {
  // Handle smart alert actions
  if (alert.type === 'opportunity') {
    navigateTo('/opportunities')
  } else if (alert.type === 'optimization') {
    navigateTo('/optimization')
  }
}
</script>

<style scoped>
.service-dashboard {
  padding: 2rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  min-height: 100vh;
  color: white;
}

.hero-section {
  margin-bottom: 3rem;
}

.business-health {
  text-align: center;
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 20px;
  padding: 2rem;
}

.business-health h1 {
  font-size: 2.5rem;
  margin-bottom: 2rem;
  font-weight: 700;
}

.health-score {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 3rem;
  margin-bottom: 2rem;
}

.score-circle {
  display: flex;
  flex-direction: column;
  align-items: center;
  background: rgba(255, 255, 255, 0.2);
  border-radius: 50%;
  width: 120px;
  height: 120px;
  justify-content: center;
}

.score {
  font-size: 2.5rem;
  font-weight: 700;
  color: #4ade80;
}

.automation-level {
  display: flex;
  flex-direction: column;
  align-items: center;
}

.percentage {
  font-size: 2rem;
  font-weight: 700;
  color: #60a5fa;
}

.growth-metrics {
  display: flex;
  justify-content: center;
  gap: 3rem;
}

.metric {
  display: flex;
  flex-direction: column;
  align-items: center;
}

.metric .value {
  font-size: 1.5rem;
  font-weight: 700;
  color: #fbbf24;
}

.outcomes-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 2rem;
  margin-bottom: 3rem;
}

.outcome-card {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 15px;
  padding: 1.5rem;
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.card-header h3 {
  font-size: 1.25rem;
  font-weight: 600;
}

.performance {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 0.25rem;
}

.current {
  font-size: 1.5rem;
  font-weight: 700;
  color: #4ade80;
}

.growth {
  font-size: 0.875rem;
  color: #4ade80;
}

.ai-contribution {
  background: rgba(74, 222, 128, 0.2);
  border-radius: 8px;
  padding: 0.75rem;
  margin-bottom: 1rem;
  font-size: 0.875rem;
}

.outcome-list {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.outcome-item {
  font-size: 0.875rem;
  padding: 0.5rem;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 6px;
}

.services-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(350px, 1fr));
  gap: 2rem;
}

.service-card {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 15px;
  padding: 1.5rem;
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.service-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.status.active {
  background: #4ade80;
  color: white;
  padding: 0.25rem 0.75rem;
  border-radius: 20px;
  font-size: 0.75rem;
  font-weight: 600;
}

.service-performance {
  display: flex;
  justify-content: space-between;
  margin-bottom: 1rem;
}

.performance-score {
  font-size: 1.5rem;
  font-weight: 700;
  color: #4ade80;
}

.service-roi {
  margin-bottom: 1rem;
  font-size: 0.875rem;
}

.roi {
  color: #4ade80;
  font-weight: 600;
}

.service-actions {
  display: flex;
  gap: 0.5rem;
}

.btn-configure, .btn-pause {
  padding: 0.5rem 1rem;
  border-radius: 6px;
  border: none;
  font-size: 0.875rem;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-configure {
  background: #3b82f6;
  color: white;
}

.btn-pause {
  background: rgba(255, 255, 255, 0.2);
  color: white;
}

.agents-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 1.5rem;
}

.agent-card {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 15px;
  padding: 1.5rem;
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.agent-status {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.status-indicator.working {
  width: 12px;
  height: 12px;
  background: #4ade80;
  border-radius: 50%;
  animation: pulse 2s infinite;
}

@keyframes pulse {
  0% { opacity: 1; }
  50% { opacity: 0.5; }
  100% { opacity: 1; }
}

.agent-activity {
  margin-bottom: 1rem;
  font-size: 0.875rem;
}

.last-action, .next-action {
  margin-bottom: 0.5rem;
}

.confidence {
  font-size: 0.75rem;
  color: #94a3b8;
}

.roi-dashboard {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 15px;
  padding: 2rem;
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.roi-summary {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.investment-box, .returns-box, .profit-box, .roi-box {
  text-align: center;
  padding: 1rem;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 10px;
}

.amount {
  font-size: 1.5rem;
  font-weight: 700;
  margin-top: 0.5rem;
}

.amount.profit {
  color: #4ade80;
}

.amount.roi {
  color: #fbbf24;
}

.roi-breakdown {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1rem;
}

.breakdown-item {
  display: flex;
  justify-content: space-between;
  padding: 0.75rem;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 8px;
}

.alerts-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(350px, 1fr));
  gap: 1.5rem;
}

.alert-card {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 15px;
  padding: 1.5rem;
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.alert-card.opportunity {
  border-left: 4px solid #4ade80;
}

.alert-card.optimization {
  border-left: 4px solid #fbbf24;
}

.alert-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.priority.high {
  background: #ef4444;
  color: white;
  padding: 0.25rem 0.75rem;
  border-radius: 20px;
  font-size: 0.75rem;
}

.priority.medium {
  background: #f59e0b;
  color: white;
  padding: 0.25rem 0.75rem;
  border-radius: 20px;
  font-size: 0.75rem;
}

.alert-message {
  margin-bottom: 1rem;
  font-size: 0.875rem;
}

.alert-value {
  margin-bottom: 1rem;
  font-weight: 600;
  color: #4ade80;
}

.alert-action {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.btn-action {
  background: #3b82f6;
  color: white;
  padding: 0.5rem 1rem;
  border-radius: 6px;
  border: none;
  cursor: pointer;
  font-size: 0.875rem;
}

.deadline {
  font-size: 0.75rem;
  color: #fbbf24;
}

.predictions-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
  gap: 2rem;
}

.prediction-card {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 15px;
  padding: 2rem;
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.prediction-metric {
  display: flex;
  justify-content: space-between;
  margin-bottom: 1rem;
  padding: 0.75rem;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 8px;
}

.ai-recommendations h5 {
  margin-bottom: 1rem;
  color: #fbbf24;
}

.ai-recommendations ul {
  list-style: none;
  padding: 0;
}

.ai-recommendations li {
  padding: 0.5rem;
  margin-bottom: 0.5rem;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 6px;
  font-size: 0.875rem;
}

.ai-recommendations li::before {
  content: "🤖 ";
  margin-right: 0.5rem;
}

h2 {
  font-size: 1.875rem;
  font-weight: 700;
  margin-bottom: 1.5rem;
  text-align: center;
}

.services-section, .automation-section, .financial-section, .alerts-section, .predictions-section {
  margin-bottom: 3rem;
}
</style>
