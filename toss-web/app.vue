<template>
  <div class="app">
    <!-- Service as Software Navigation -->
    <nav class="service-nav">
      <div class="nav-container">
        <div class="nav-brand">
          <NuxtLink to="/" class="brand-link">
            <span class="brand-icon">🚀</span>
            <span class="brand-text">TOSS</span>
            <span class="brand-tagline">Service as Software</span>
          </NuxtLink>
        </div>
        
        <div class="nav-links">
          <NuxtLink to="/dashboard/service" class="nav-link">
            <span class="link-icon">📊</span>
            <span>Business Dashboard</span>
          </NuxtLink>
          <NuxtLink to="/services" class="nav-link">
            <span class="link-icon">🤖</span>
            <span>AI Services</span>
          </NuxtLink>
          <NuxtLink to="/outcomes" class="nav-link">
            <span class="link-icon">🎯</span>
            <span>My Outcomes</span>
          </NuxtLink>
          <NuxtLink to="/network" class="nav-link">
            <span class="link-icon">🤝</span>
            <span>Business Network</span>
          </NuxtLink>
          <NuxtLink to="/support" class="nav-link">
            <span class="link-icon">💬</span>
            <span>Support</span>
          </NuxtLink>
        </div>

        <div class="nav-actions">
          <div class="business-health">
            <span class="health-icon">💚</span>
            <span class="health-score">92%</span>
          </div>
          <button class="btn-upgrade">
            <span>⭐</span>
            <span>Upgrade</span>
          </button>
        </div>
      </div>
    </nav>

    <!-- Main Content -->
    <main class="main-content">
      <NuxtLayout>
        <NuxtPage />
      </NuxtLayout>
    </main>

    <!-- Service as Software Footer -->
    <footer class="service-footer">
      <div class="footer-container">
        <div class="footer-section">
          <h4>🚀 Your Business Automation</h4>
          <p>AI services delivering guaranteed business outcomes</p>
        </div>
        <div class="footer-section">
          <h4>Active Services</h4>
          <div class="active-services">
            <div class="service-status">
              <span class="status-dot working"></span>
              <span>Sales AI Working</span>
            </div>
            <div class="service-status">
              <span class="status-dot working"></span>
              <span>Customer AI Active</span>
            </div>
            <div class="service-status">
              <span class="status-dot working"></span>
              <span>Inventory AI Running</span>
            </div>
          </div>
        </div>
        <div class="footer-section">
          <h4>This Month's Impact</h4>
          <div class="impact-metrics">
            <div class="impact-item">
              <span class="metric-value">+23%</span>
              <span class="metric-label">Revenue Growth</span>
            </div>
            <div class="impact-item">
              <span class="metric-value">47h</span>
              <span class="metric-label">Time Saved</span>
            </div>
          </div>
        </div>
        <div class="footer-section">
          <h4>Support & Community</h4>
          <div class="support-links">
            <a href="mailto:support@toss.co.za">📧 Get Help</a>
            <a href="/community">👥 Business Network</a>
            <a href="/success-stories">🏆 Success Stories</a>
          </div>
        </div>
      </div>
      <div class="footer-bottom">
        <p>&copy; 2024 TOSS Service as Software Platform. AI that delivers business results.</p>
      </div>
    </footer>
  </div>
</template>

<script setup>
useHead({
  title: 'TOSS - Service as Software Platform',
  meta: [
    { name: 'description', content: 'AI services that deliver guaranteed business outcomes. Pay for results, not software.' }
  ]
})
</script>

<style>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  min-height: 100vh;
  color: white;
}

.app {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.service-nav {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(20px);
  border-bottom: 1px solid rgba(255, 255, 255, 0.2);
  padding: 1rem 0;
  position: sticky;
  top: 0;
  z-index: 100;
}

.nav-container {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 2rem;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.nav-brand {
  display: flex;
  align-items: center;
}

.brand-link {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  text-decoration: none;
  color: white;
}

.brand-icon {
  font-size: 2rem;
}

.brand-text {
  font-size: 1.5rem;
  font-weight: 700;
  letter-spacing: -0.5px;
}

.brand-tagline {
  font-size: 0.75rem;
  opacity: 0.8;
  background: rgba(255, 255, 255, 0.2);
  padding: 0.25rem 0.5rem;
  border-radius: 10px;
}

.nav-links {
  display: flex;
  gap: 2rem;
  align-items: center;
}

.nav-link {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  text-decoration: none;
  color: white;
  padding: 0.75rem 1rem;
  border-radius: 10px;
  transition: all 0.2s;
  opacity: 0.8;
}

.nav-link:hover {
  background: rgba(255, 255, 255, 0.1);
  opacity: 1;
  transform: translateY(-1px);
}

.nav-link.router-link-active {
  background: rgba(255, 255, 255, 0.2);
  opacity: 1;
}

.link-icon {
  font-size: 1.125rem;
}

.nav-actions {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.business-health {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  background: rgba(255, 255, 255, 0.1);
  padding: 0.5rem 1rem;
  border-radius: 20px;
  font-size: 0.875rem;
}

.health-icon {
  font-size: 1rem;
}

.health-score {
  font-weight: 600;
  color: #4ade80;
}

.btn-upgrade {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  background: linear-gradient(45deg, #fbbf24, #f59e0b);
  color: #1f2937;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: 25px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-upgrade:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(251, 191, 36, 0.3);
}

.main-content {
  flex: 1;
}

.service-footer {
  background: rgba(0, 0, 0, 0.3);
  backdrop-filter: blur(20px);
  border-top: 1px solid rgba(255, 255, 255, 0.1);
  margin-top: 4rem;
}

.footer-container {
  max-width: 1400px;
  margin: 0 auto;
  padding: 3rem 2rem 2rem;
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 2rem;
}

.footer-section h4 {
  margin-bottom: 1rem;
  font-size: 1.125rem;
  color: #fbbf24;
}

.footer-section p {
  opacity: 0.8;
  line-height: 1.6;
}

.active-services {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.service-status {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.875rem;
}

.status-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: #4ade80;
}

.status-dot.working {
  animation: pulse 2s infinite;
}

.impact-metrics {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.impact-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.metric-value {
  font-size: 1.25rem;
  font-weight: 700;
  color: #4ade80;
}

.metric-label {
  font-size: 0.875rem;
  opacity: 0.8;
}

.support-links {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.support-links a {
  color: white;
  text-decoration: none;
  font-size: 0.875rem;
  opacity: 0.8;
  transition: opacity 0.2s;
}

.support-links a:hover {
  opacity: 1;
}

.footer-bottom {
  border-top: 1px solid rgba(255, 255, 255, 0.1);
  padding: 1.5rem 2rem;
  text-align: center;
  opacity: 0.6;
  font-size: 0.875rem;
}

@keyframes pulse {
  0% { opacity: 1; }
  50% { opacity: 0.5; }
  100% { opacity: 1; }
}

/* Responsive Design */
@media (max-width: 768px) {
  .nav-container {
    padding: 0 1rem;
    flex-direction: column;
    gap: 1rem;
  }

  .nav-links {
    flex-wrap: wrap;
    justify-content: center;
    gap: 1rem;
  }

  .nav-actions {
    gap: 0.5rem;
  }

  .footer-container {
    padding: 2rem 1rem 1rem;
    grid-template-columns: 1fr;
    gap: 1.5rem;
  }
}
</style>
