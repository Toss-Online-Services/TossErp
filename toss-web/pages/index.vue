<template>
  <div class="service-home">
    <!-- Hero Section -->
    <section class="hero">
      <div class="hero-content">
        <div class="hero-text">
          <h1 class="hero-title">
            Don't Buy Software<br>
            <span class="highlight">Buy Business Results</span>
          </h1>
          <p class="hero-subtitle">
            TOSS Service as Software delivers autonomous AI that works for your business 24/7. 
            Pay for guaranteed outcomes, not software licenses.
          </p>
          <div class="value-props">
            <div class="value-prop">
              <span class="prop-icon">🎯</span>
              <span>Guaranteed Outcomes</span>
            </div>
            <div class="value-prop">
              <span class="prop-icon">🤖</span>
              <span>Fully Autonomous</span>
            </div>
            <div class="value-prop">
              <span class="prop-icon">💰</span>
              <span>Pay for Results</span>
            </div>
          </div>
          <div class="hero-actions">
            <button @click="startAssessment" class="btn-primary">
              Get Your Business Assessment
            </button>
            <button @click="viewDemo" class="btn-secondary">
              See How It Works
            </button>
          </div>
        </div>
        <div class="hero-visual">
          <div class="business-dashboard-preview">
            <div class="dashboard-header">
              <h3>Your Business Command Center</h3>
              <div class="health-indicator">
                <span class="health-score">94%</span>
                <span class="health-label">Business Health</span>
              </div>
            </div>
            <div class="dashboard-metrics">
              <div class="metric-card">
                <div class="metric-value">+23%</div>
                <div class="metric-label">Revenue Growth</div>
                <div class="metric-attribution">AI Contribution</div>
              </div>
              <div class="metric-card">
                <div class="metric-value">47h</div>
                <div class="metric-label">Time Saved</div>
                <div class="metric-attribution">This Month</div>
              </div>
              <div class="metric-card">
                <div class="metric-value">89%</div>
                <div class="metric-label">Automated</div>
                <div class="metric-attribution">Processes</div>
              </div>
            </div>
            <div class="ai-agents-preview">
              <div class="agent-item">
                <span class="agent-status working"></span>
                <span>Sales AI just collected R2,340 payment</span>
              </div>
              <div class="agent-item">
                <span class="agent-status working"></span>
                <span>Customer AI sent 5 birthday messages</span>
              </div>
              <div class="agent-item">
                <span class="agent-status working"></span>
                <span>Inventory AI optimized stock levels</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Service Transformation -->
    <section class="transformation">
      <div class="section-content">
        <h2>The Service Revolution</h2>
        <div class="transformation-grid">
          <div class="old-way">
            <h3>🏢 Old Way: Buy Software</h3>
            <div class="way-items">
              <div class="way-item">❌ Pay for licenses you might not use</div>
              <div class="way-item">❌ Spend months learning and configuring</div>
              <div class="way-item">❌ Hire staff to operate the software</div>
              <div class="way-item">❌ Hope it delivers business value</div>
              <div class="way-item">❌ Update and maintain systems yourself</div>
            </div>
          </div>
          <div class="new-way">
            <h3>🚀 New Way: Buy Business Outcomes</h3>
            <div class="way-items">
              <div class="way-item">✅ Pay only for guaranteed results</div>
              <div class="way-item">✅ AI works autonomously from day one</div>
              <div class="way-item">✅ No training or hiring required</div>
              <div class="way-item">✅ Business outcomes guaranteed</div>
              <div class="way-item">✅ Continuous AI improvement included</div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Service Showcase -->
    <section class="services-showcase">
      <div class="section-content">
        <h2>Autonomous Business Services</h2>
        <div class="services-grid">
          <div class="service-preview">
            <div class="service-icon">🎯</div>
            <h3>Sales Automation</h3>
            <p class="service-description">AI handles invoicing, payments, and customer follow-ups</p>
            <div class="service-guarantee">
              <strong>Guaranteed Outcome:</strong> 15% revenue increase in 60 days
            </div>
            <div class="service-pricing">R499/month → Avg R2,500 value</div>
            <div class="service-roi">ROI: 400%</div>
          </div>

          <div class="service-preview">
            <div class="service-icon">❤️</div>
            <h3>Customer Success AI</h3>
            <p class="service-description">AI manages relationships to maximize satisfaction and retention</p>
            <div class="service-guarantee">
              <strong>Guaranteed Outcome:</strong> 25% retention improvement
            </div>
            <div class="service-pricing">R249/month → Avg R1,200 value</div>
            <div class="service-roi">ROI: 380%</div>
          </div>

          <div class="service-preview">
            <div class="service-icon">📦</div>
            <h3>Smart Inventory</h3>
            <p class="service-description">AI prevents stockouts and optimizes ordering costs</p>
            <div class="service-guarantee">
              <strong>Guaranteed Outcome:</strong> Zero stockouts + 20% cost reduction
            </div>
            <div class="service-pricing">R299/month → Avg R800 savings</div>
            <div class="service-roi">ROI: 320%</div>
          </div>

          <div class="service-preview">
            <div class="service-icon">💼</div>
            <h3>Financial Intelligence</h3>
            <p class="service-description">AI handles bookkeeping and ensures compliance</p>
            <div class="service-guarantee">
              <strong>Guaranteed Outcome:</strong> 100% compliance + 90% time savings
            </div>
            <div class="service-pricing">R299/month → Avg R1,000 value</div>
            <div class="service-roi">ROI: 280%</div>
          </div>
        </div>
      </div>
    </section>

    <!-- Success Stories -->
    <section class="success-stories">
      <div class="section-content">
        <h2>Real Business Transformations</h2>
        <div class="stories-grid">
          <div class="story-card">
            <div class="story-header">
              <div class="business-info">
                <h4>Beautiful Hair Salon</h4>
                <span class="business-type">Beauty & Wellness</span>
              </div>
              <div class="story-results">
                <span class="result-metric">+28%</span>
                <span class="result-label">Revenue</span>
              </div>
            </div>
            <div class="story-quote">
              "Our Sales AI works 24/7. It increased revenue by 28% in 8 weeks and handles all our payment collections. I can focus on my clients instead of admin work."
            </div>
            <div class="story-author">
              <strong>Sarah Johnson</strong>, Owner
            </div>
          </div>

          <div class="story-card">
            <div class="story-header">
              <div class="business-info">
                <h4>Urban Fitness Studio</h4>
                <span class="business-type">Health & Fitness</span>
              </div>
              <div class="story-results">
                <span class="result-metric">+34%</span>
                <span class="result-label">Retention</span>
              </div>
            </div>
            <div class="story-quote">
              "Customer AI sends personalized birthday messages, tracks preferences, and prevents churn. Our retention improved by 34% and clients actually thank us for the personal touch."
            </div>
            <div class="story-author">
              <strong>Michael Chen</strong>, Manager
            </div>
          </div>

          <div class="story-card">
            <div class="story-header">
              <div class="business-info">
                <h4>Artisan Coffee Roasters</h4>
                <span class="business-type">Food & Beverage</span>
              </div>
              <div class="story-results">
                <span class="result-metric">R4,200</span>
                <span class="result-label">Monthly Savings</span>
              </div>
            </div>
            <div class="story-quote">
              "Inventory AI prevented 12 stockouts last month and negotiated better prices with suppliers. We're saving R4,200 monthly while never running out of popular blends."
            </div>
            <div class="story-author">
              <strong>Lisa Rodriguez</strong>, Operations Manager
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Getting Started -->
    <section class="getting-started">
      <div class="section-content">
        <h2>Start Your Business Transformation</h2>
        <div class="steps-grid">
          <div class="step">
            <div class="step-number">1</div>
            <h3>Business Assessment</h3>
            <p>Our AI analyzes your business to identify the highest-impact opportunities and guaranteed outcomes.</p>
          </div>
          <div class="step">
            <div class="step-number">2</div>
            <h3>Service Activation</h3>
            <p>Choose the services that deliver your desired outcomes. AI agents begin working within minutes.</p>
          </div>
          <div class="step">
            <div class="step-number">3</div>
            <h3>Results Delivery</h3>
            <p>Watch your business metrics improve while AI works autonomously to deliver guaranteed outcomes.</p>
          </div>
        </div>
        <div class="cta-section">
          <h3>Ready to Transform Your Business?</h3>
          <p>Join hundreds of businesses already experiencing autonomous success.</p>
          <button @click="startAssessment" class="btn-large">
            Get Your Free Business Assessment
          </button>
          <div class="guarantee-note">
            💎 30-day money-back guarantee on all outcomes
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
const startAssessment = () => {
  // Navigate to business assessment
  navigateTo('/onboarding/assessment')
}

const viewDemo = () => {
  // Navigate to demo dashboard
  navigateTo('/dashboard/service')
}

useHead({
  title: 'TOSS - Service as Software Platform | Buy Business Results, Not Software',
  meta: [
    { name: 'description', content: 'Transform your business with autonomous AI services. Pay for guaranteed outcomes, not software licenses. 400%+ ROI with 30-day guarantee.' }
  ]
})
</script>

<style scoped>
.service-home {
  color: white;
}

.hero {
  padding: 4rem 2rem;
  min-height: 80vh;
  display: flex;
  align-items: center;
}

.hero-content {
  max-width: 1400px;
  margin: 0 auto;
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 4rem;
  align-items: center;
}

.hero-title {
  font-size: 3.5rem;
  font-weight: 700;
  line-height: 1.1;
  margin-bottom: 1.5rem;
}

.highlight {
  background: linear-gradient(45deg, #fbbf24, #f59e0b);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.hero-subtitle {
  font-size: 1.25rem;
  opacity: 0.9;
  line-height: 1.6;
  margin-bottom: 2rem;
}

.value-props {
  display: flex;
  gap: 2rem;
  margin-bottom: 3rem;
  flex-wrap: wrap;
}

.value-prop {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  background: rgba(255, 255, 255, 0.1);
  padding: 0.75rem 1rem;
  border-radius: 20px;
  backdrop-filter: blur(10px);
}

.prop-icon {
  font-size: 1.25rem;
}

.hero-actions {
  display: flex;
  gap: 1rem;
  flex-wrap: wrap;
}

.btn-primary, .btn-secondary {
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

.btn-primary:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(74, 222, 128, 0.3);
}

.btn-secondary:hover {
  background: rgba(255, 255, 255, 0.2);
  transform: translateY(-2px);
}

.business-dashboard-preview {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(20px);
  border-radius: 20px;
  padding: 2rem;
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.dashboard-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.dashboard-header h3 {
  font-size: 1.25rem;
}

.health-indicator {
  text-align: center;
}

.health-score {
  display: block;
  font-size: 2rem;
  font-weight: 700;
  color: #4ade80;
}

.health-label {
  font-size: 0.75rem;
  opacity: 0.8;
}

.dashboard-metrics {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1rem;
  margin-bottom: 2rem;
}

.metric-card {
  background: rgba(255, 255, 255, 0.1);
  padding: 1rem;
  border-radius: 10px;
  text-align: center;
}

.metric-value {
  font-size: 1.5rem;
  font-weight: 700;
  color: #4ade80;
  display: block;
}

.metric-label {
  font-size: 0.875rem;
  margin: 0.25rem 0;
}

.metric-attribution {
  font-size: 0.75rem;
  opacity: 0.7;
}

.ai-agents-preview {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.agent-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  background: rgba(255, 255, 255, 0.1);
  padding: 0.75rem;
  border-radius: 8px;
  font-size: 0.875rem;
}

.agent-status {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: #4ade80;
}

.agent-status.working {
  animation: pulse 2s infinite;
}

.transformation, .services-showcase, .success-stories, .getting-started {
  padding: 4rem 2rem;
}

.section-content {
  max-width: 1400px;
  margin: 0 auto;
}

.section-content h2 {
  text-align: center;
  font-size: 2.5rem;
  font-weight: 700;
  margin-bottom: 3rem;
}

.transformation-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 3rem;
}

.old-way, .new-way {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 20px;
  padding: 2rem;
}

.old-way h3, .new-way h3 {
  font-size: 1.5rem;
  margin-bottom: 2rem;
  text-align: center;
}

.way-items {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.way-item {
  padding: 1rem;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 10px;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.services-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 2rem;
}

.service-preview {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 20px;
  padding: 2rem;
  text-align: center;
  border: 1px solid rgba(255, 255, 255, 0.2);
  transition: transform 0.3s;
}

.service-preview:hover {
  transform: translateY(-5px);
}

.service-icon {
  font-size: 3rem;
  margin-bottom: 1rem;
}

.service-preview h3 {
  font-size: 1.25rem;
  margin-bottom: 1rem;
}

.service-description {
  opacity: 0.9;
  margin-bottom: 1.5rem;
  line-height: 1.5;
}

.service-guarantee {
  background: rgba(74, 222, 128, 0.2);
  border: 1px solid rgba(74, 222, 128, 0.3);
  border-radius: 10px;
  padding: 1rem;
  margin-bottom: 1rem;
  font-size: 0.875rem;
}

.service-pricing {
  font-size: 1.125rem;
  font-weight: 600;
  margin-bottom: 0.5rem;
  color: #fbbf24;
}

.service-roi {
  font-size: 1rem;
  font-weight: 700;
  color: #4ade80;
}

.stories-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(350px, 1fr));
  gap: 2rem;
}

.story-card {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 20px;
  padding: 2rem;
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.story-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 1.5rem;
}

.business-type {
  font-size: 0.875rem;
  opacity: 0.8;
  background: rgba(255, 255, 255, 0.1);
  padding: 0.25rem 0.75rem;
  border-radius: 12px;
  margin-top: 0.5rem;
  display: inline-block;
}

.story-results {
  text-align: right;
}

.result-metric {
  display: block;
  font-size: 1.5rem;
  font-weight: 700;
  color: #4ade80;
}

.result-label {
  font-size: 0.875rem;
  opacity: 0.8;
}

.story-quote {
  font-style: italic;
  line-height: 1.6;
  margin-bottom: 1.5rem;
  opacity: 0.9;
}

.story-author {
  opacity: 0.8;
}

.steps-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 2rem;
  margin-bottom: 4rem;
}

.step {
  text-align: center;
}

.step-number {
  width: 60px;
  height: 60px;
  background: linear-gradient(45deg, #4ade80, #22c55e);
  color: #1f2937;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
  font-weight: 700;
  margin: 0 auto 1.5rem;
}

.step h3 {
  font-size: 1.25rem;
  margin-bottom: 1rem;
}

.step p {
  opacity: 0.9;
  line-height: 1.6;
}

.cta-section {
  text-align: center;
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 20px;
  padding: 3rem;
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.cta-section h3 {
  font-size: 2rem;
  margin-bottom: 1rem;
}

.cta-section p {
  font-size: 1.125rem;
  opacity: 0.9;
  margin-bottom: 2rem;
}

.btn-large {
  background: linear-gradient(45deg, #4ade80, #22c55e);
  color: #1f2937;
  padding: 1.25rem 3rem;
  border-radius: 15px;
  font-weight: 700;
  font-size: 1.125rem;
  border: none;
  cursor: pointer;
  transition: all 0.3s;
  margin-bottom: 1rem;
}

.btn-large:hover {
  transform: translateY(-3px);
  box-shadow: 0 12px 35px rgba(74, 222, 128, 0.4);
}

.guarantee-note {
  font-size: 0.875rem;
  opacity: 0.8;
}

/* Responsive Design */
@media (max-width: 768px) {
  .hero-content {
    grid-template-columns: 1fr;
    gap: 2rem;
  }

  .hero-title {
    font-size: 2.5rem;
  }

  .transformation-grid {
    grid-template-columns: 1fr;
  }

  .dashboard-metrics {
    grid-template-columns: 1fr;
  }

  .value-props {
    flex-direction: column;
    gap: 1rem;
  }

  .hero-actions {
    flex-direction: column;
  }
}

@keyframes pulse {
  0% { opacity: 1; }
  50% { opacity: 0.5; }
  100% { opacity: 1; }
}
</style>
