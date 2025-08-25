<template>
  <div class="network-page">
    <ClientOnly>
      <template #default>
        <!-- Header Section -->
        <div class="network-header">
          <div class="header-content">
            <h1>🤝 Business Network</h1>
            <p class="header-subtitle">Connect with businesses in your area for collaboration and growth</p>
            <div class="network-stats">
              <div class="stat-card">
                <div class="stat-value">{{ totalConnections }}</div>
                <div class="stat-label">Connections</div>
              </div>
              <div class="stat-card">
                <div class="stat-value">{{ activeCollaborations }}</div>
                <div class="stat-label">Active Collaborations</div>
              </div>
              <div class="stat-card">
                <div class="stat-value">R{{ formatCurrency(totalSavings) }}</div>
                <div class="stat-label">Network Savings</div>
              </div>
            </div>
          </div>
        </div>

        <!-- Quick Actions -->
        <div class="quick-actions">
          <h2>Network Opportunities</h2>
          <div class="actions-grid">
            <div class="action-card group-buying">
              <div class="action-icon">🛒</div>
              <h3>Group Buying</h3>
              <p>Join group purchases to get bulk discounts on supplies and inventory</p>
              <div class="action-stats">
                <span class="stat">{{ groupBuyingOpportunities }} active opportunities</span>
                <span class="savings">Avg. 15-25% savings</span>
              </div>
              <button class="btn-action">Explore Group Buys</button>
            </div>
            
            <div class="action-card resource-sharing">
              <div class="action-icon">🔄</div>
              <h3>Resource Sharing</h3>
              <p>Share equipment, tools, or services with trusted network partners</p>
              <div class="action-stats">
                <span class="stat">{{ sharedResources }} shared resources</span>
                <span class="savings">Save on capital costs</span>
              </div>
              <button class="btn-action">Browse Resources</button>
            </div>
            
            <div class="action-card referrals">
              <div class="action-icon">🎯</div>
              <h3>Referral Network</h3>
              <p>Give and receive customer referrals from complementary businesses</p>
              <div class="action-stats">
                <span class="stat">{{ referralPartners }} referral partners</span>
                <span class="savings">Mutual growth</span>
              </div>
              <button class="btn-action">Join Referrals</button>
            </div>
            
            <div class="action-card knowledge">
              <div class="action-icon">💡</div>
              <h3>Knowledge Exchange</h3>
              <p>Share expertise and learn best practices from other business owners</p>
              <div class="action-stats">
                <span class="stat">{{ knowledgeGroups }} discussion groups</span>
                <span class="savings">Accelerate learning</span>
              </div>
              <button class="btn-action">Join Discussions</button>
            </div>
          </div>
        </div>

        <!-- Network Connections -->
        <div class="network-connections">
          <h2>Your Network</h2>
          <div class="connections-grid">
            <div v-for="connection in connections" :key="connection.id" class="connection-card">
              <div class="connection-header">
                <div class="business-avatar">{{ connection.avatar }}</div>
                <div class="business-info">
                  <h3>{{ connection.businessName }}</h3>
                  <p class="business-type">{{ connection.businessType }}</p>
                  <p class="location">📍 {{ connection.location }}</p>
                </div>
                <div class="connection-status">
                  <span class="status-badge" :class="connection.status">{{ connection.status }}</span>
                </div>
              </div>
              
              <div class="collaboration-summary">
                <div class="collaboration-item" v-for="collab in connection.collaborations" :key="collab.type">
                  <span class="collab-icon">{{ collab.icon }}</span>
                  <span class="collab-text">{{ collab.description }}</span>
                  <span class="collab-value">{{ collab.value }}</span>
                </div>
              </div>
              
              <div class="connection-actions">
                <button class="btn-message">💬 Message</button>
                <button class="btn-collaborate">🤝 Collaborate</button>
              </div>
            </div>
          </div>
        </div>

        <!-- Local Business Map -->
        <div class="local-businesses">
          <h2>Nearby Businesses</h2>
          <div class="map-placeholder">
            <div class="map-content">
              <div class="map-icon">🗺️</div>
              <h3>Business Map</h3>
              <p>Discover nearby businesses for potential partnerships</p>
              <div class="nearby-count">{{ nearbyBusinesses }} businesses within 10km</div>
              <button class="btn-explore">Explore Map</button>
            </div>
          </div>
        </div>

        <!-- Success Stories -->
        <div class="success-stories">
          <h2>Network Success Stories</h2>
          <div class="stories-grid">
            <div v-for="story in successStories" :key="story.id" class="story-card">
              <div class="story-header">
                <span class="story-icon">{{ story.icon }}</span>
                <h3>{{ story.title }}</h3>
              </div>
              <p class="story-description">{{ story.description }}</p>
              <div class="story-impact">
                <span class="impact-value">{{ story.impact }}</span>
                <span class="impact-label">{{ story.impactType }}</span>
              </div>
            </div>
          </div>
        </div>
      </template>
      
      <template #fallback>
        <div class="loading-state">
          <div class="loading-spinner"></div>
          <p>Loading your business network...</p>
        </div>
      </template>
    </ClientOnly>
  </div>
</template>

<script setup>
// Page meta
definePageMeta({
  title: 'Business Network'
})

// Reactive data
const connections = ref([
  {
    id: 1,
    businessName: 'Green Garden Supplies',
    businessType: 'Garden Center',
    location: 'Sandton, Johannesburg',
    avatar: '🌿',
    status: 'active',
    collaborations: [
      { icon: '🛒', description: 'Group buying', value: 'R2,400 saved' },
      { icon: '🎯', description: 'Customer referrals', value: '12 referrals' }
    ]
  },
  {
    id: 2,
    businessName: 'Tech Repair Pro',
    businessType: 'Electronics Repair',
    location: 'Rosebank, Johannesburg',
    avatar: '🔧',
    status: 'active',
    collaborations: [
      { icon: '🔄', description: 'Equipment sharing', value: 'Diagnostic tools' },
      { icon: '💡', description: 'Knowledge exchange', value: 'Repair techniques' }
    ]
  },
  {
    id: 3,
    businessName: 'Local Coffee Roasters',
    businessType: 'Coffee Shop',
    location: 'Melville, Johannesburg',
    avatar: '☕',
    status: 'pending',
    collaborations: [
      { icon: '🎯', description: 'Cross-promotion', value: 'Potential partnership' }
    ]
  }
])

const successStories = ref([
  {
    id: 1,
    icon: '💰',
    title: 'Bulk Purchasing Success',
    description: 'Joined with 8 local businesses to purchase office supplies in bulk',
    impact: '23%',
    impactType: 'Cost Reduction'
  },
  {
    id: 2,
    icon: '🤝',
    title: 'Referral Partnership',
    description: 'Established referral network with complementary service providers',
    impact: '47',
    impactType: 'New Customers'
  },
  {
    id: 3,
    icon: '🔄',
    title: 'Equipment Sharing',
    description: 'Shared specialized equipment with nearby businesses',
    impact: 'R12,000',
    impactType: 'Capital Savings'
  }
])

// Computed properties
const totalConnections = computed(() => connections.value.length)
const activeCollaborations = computed(() => 
  connections.value.reduce((sum, conn) => sum + conn.collaborations.length, 0)
)
const totalSavings = computed(() => 15600) // Mock calculation
const groupBuyingOpportunities = computed(() => 8)
const sharedResources = computed(() => 12)
const referralPartners = computed(() => 15)
const knowledgeGroups = computed(() => 6)
const nearbyBusinesses = computed(() => 42)

// Utility functions
const formatCurrency = (amount) => {
  return new Intl.NumberFormat('en-ZA').format(amount)
}
</script>

<style scoped>
.network-page {
  min-height: 100vh;
  background: linear-gradient(135deg, #4f46e5 0%, #7c3aed 100%);
  color: white;
  padding: 2rem;
}

.network-header {
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

.network-stats {
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

.quick-actions,
.network-connections,
.local-businesses,
.success-stories {
  margin-bottom: 3rem;
}

.quick-actions h2,
.network-connections h2,
.local-businesses h2,
.success-stories h2 {
  font-size: 1.875rem;
  font-weight: 700;
  margin-bottom: 1.5rem;
}

.actions-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 1.5rem;
}

.action-card {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 15px;
  padding: 1.5rem;
  border: 1px solid rgba(255, 255, 255, 0.2);
  text-align: center;
}

.action-icon {
  font-size: 2.5rem;
  margin-bottom: 1rem;
}

.action-card h3 {
  font-size: 1.25rem;
  font-weight: 600;
  margin-bottom: 0.75rem;
}

.action-card p {
  opacity: 0.9;
  margin-bottom: 1rem;
  line-height: 1.5;
}

.action-stats {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
  margin-bottom: 1rem;
}

.action-stats .stat {
  font-size: 0.875rem;
  opacity: 0.8;
}

.action-stats .savings {
  font-weight: 600;
  color: #10b981;
}

.btn-action {
  background: rgba(255, 255, 255, 0.2);
  border: 1px solid rgba(255, 255, 255, 0.3);
  color: white;
  padding: 0.75rem 1.5rem;
  border-radius: 25px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-action:hover {
  background: rgba(255, 255, 255, 0.3);
  transform: translateY(-2px);
}

.connections-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(350px, 1fr));
  gap: 1.5rem;
}

.connection-card {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 15px;
  padding: 1.5rem;
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.connection-header {
  display: flex;
  align-items: flex-start;
  gap: 1rem;
  margin-bottom: 1rem;
}

.business-avatar {
  font-size: 2rem;
  width: 60px;
  height: 60px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(255, 255, 255, 0.2);
  border-radius: 50%;
}

.business-info {
  flex: 1;
}

.business-info h3 {
  font-size: 1.125rem;
  font-weight: 600;
  margin-bottom: 0.25rem;
}

.business-type {
  font-size: 0.875rem;
  opacity: 0.8;
  margin-bottom: 0.25rem;
}

.location {
  font-size: 0.875rem;
  opacity: 0.7;
}

.status-badge {
  padding: 0.25rem 0.75rem;
  border-radius: 15px;
  font-size: 0.75rem;
  font-weight: 500;
}

.status-badge.active {
  background: #10b981;
  color: white;
}

.status-badge.pending {
  background: #f59e0b;
  color: white;
}

.collaboration-summary {
  margin-bottom: 1rem;
}

.collaboration-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 0.5rem;
  padding: 0.5rem;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 8px;
}

.collab-icon {
  font-size: 1rem;
}

.collab-text {
  flex: 1;
  font-size: 0.875rem;
}

.collab-value {
  font-size: 0.875rem;
  font-weight: 600;
  color: #10b981;
}

.connection-actions {
  display: flex;
  gap: 0.75rem;
}

.btn-message,
.btn-collaborate {
  flex: 1;
  padding: 0.5rem 1rem;
  border-radius: 8px;
  border: 1px solid rgba(255, 255, 255, 0.3);
  background: rgba(255, 255, 255, 0.1);
  color: white;
  font-size: 0.875rem;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.btn-message:hover,
.btn-collaborate:hover {
  background: rgba(255, 255, 255, 0.2);
}

.map-placeholder {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 15px;
  padding: 3rem;
  text-align: center;
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.map-icon {
  font-size: 3rem;
  margin-bottom: 1rem;
}

.map-content h3 {
  font-size: 1.5rem;
  font-weight: 600;
  margin-bottom: 0.75rem;
}

.map-content p {
  opacity: 0.9;
  margin-bottom: 1rem;
}

.nearby-count {
  font-size: 1.125rem;
  font-weight: 600;
  color: #10b981;
  margin-bottom: 1.5rem;
}

.btn-explore {
  background: rgba(255, 255, 255, 0.2);
  border: 1px solid rgba(255, 255, 255, 0.3);
  color: white;
  padding: 0.75rem 2rem;
  border-radius: 25px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-explore:hover {
  background: rgba(255, 255, 255, 0.3);
  transform: translateY(-2px);
}

.stories-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1.5rem;
}

.story-card {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 15px;
  padding: 1.5rem;
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.story-header {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 1rem;
}

.story-icon {
  font-size: 1.5rem;
}

.story-header h3 {
  font-size: 1.125rem;
  font-weight: 600;
}

.story-description {
  opacity: 0.9;
  line-height: 1.5;
  margin-bottom: 1rem;
}

.story-impact {
  text-align: center;
}

.impact-value {
  display: block;
  font-size: 1.5rem;
  font-weight: 700;
  color: #10b981;
  margin-bottom: 0.25rem;
}

.impact-label {
  font-size: 0.875rem;
  opacity: 0.8;
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
  .network-page {
    padding: 1rem;
  }

  .header-content h1 {
    font-size: 2rem;
  }

  .network-stats {
    grid-template-columns: 1fr;
  }

  .actions-grid {
    grid-template-columns: 1fr;
  }

  .connections-grid {
    grid-template-columns: 1fr;
  }

  .connection-header {
    flex-direction: column;
    text-align: center;
  }

  .business-info {
    order: 2;
  }

  .connection-actions {
    flex-direction: column;
  }
}
</style>
