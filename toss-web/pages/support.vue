<template>
  <div class="support-page">
    <ClientOnly>
      <template #default>
        <!-- Header Section -->
        <div class="support-header">
          <div class="header-content">
            <h1>🛟 Support Center</h1>
            <p class="header-subtitle">Get help with your Service as Software platform</p>
          </div>
        </div>

        <!-- Quick Help Options -->
        <div class="quick-help">
          <h2>How can we help you today?</h2>
          <div class="help-grid">
            <div class="help-card getting-started">
              <div class="help-icon">🚀</div>
              <h3>Getting Started</h3>
              <p>New to the platform? Learn the basics and set up your business</p>
              <button class="btn-help">Quick Start Guide</button>
            </div>
            
            <div class="help-card technical">
              <div class="help-icon">🔧</div>
              <h3>Technical Support</h3>
              <p>Having technical issues? Get help with bugs and system problems</p>
              <button class="btn-help">Report Issue</button>
            </div>
            
            <div class="help-card billing">
              <div class="help-icon">💳</div>
              <h3>Billing & Pricing</h3>
              <p>Questions about your subscription, payments, or pricing plans</p>
              <button class="btn-help">Billing Help</button>
            </div>
            
            <div class="help-card features">
              <div class="help-icon">✨</div>
              <h3>Feature Requests</h3>
              <p>Suggest new features or improvements to the platform</p>
              <button class="btn-help">Submit Request</button>
            </div>
          </div>
        </div>

        <!-- Contact Options -->
        <div class="contact-section">
          <h2>Contact Support</h2>
          <div class="contact-grid">
            <div class="contact-card chat">
              <div class="contact-icon">💬</div>
              <h3>Live Chat</h3>
              <p>Get instant help from our support team</p>
              <div class="availability">
                <span class="status online">● Online</span>
                <span class="hours">Available 9 AM - 6 PM SAST</span>
              </div>
              <button class="btn-contact chat-btn">Start Chat</button>
            </div>
            
            <div class="contact-card email">
              <div class="contact-icon">📧</div>
              <h3>Email Support</h3>
              <p>Send us a detailed message and we'll respond within 24 hours</p>
              <div class="response-time">
                <span class="time">Average response: 4 hours</span>
              </div>
              <button class="btn-contact email-btn">Send Email</button>
            </div>
            
            <div class="contact-card phone">
              <div class="contact-icon">📞</div>
              <h3>Phone Support</h3>
              <p>Speak directly with our support team for urgent issues</p>
              <div class="phone-info">
                <span class="number">+27 11 123 4567</span>
                <span class="hours">Mon-Fri, 9 AM - 6 PM SAST</span>
              </div>
              <button class="btn-contact phone-btn">Call Now</button>
            </div>
          </div>
        </div>

        <!-- Knowledge Base -->
        <div class="knowledge-base">
          <h2>Knowledge Base</h2>
          <div class="search-section">
            <div class="search-bar">
              <input 
                type="text" 
                v-model="searchQuery"
                placeholder="Search for help articles..."
                class="search-input"
              >
              <button class="search-btn">🔍</button>
            </div>
          </div>
          
          <div class="kb-categories">
            <div v-for="category in knowledgeBase" :key="category.id" class="kb-category">
              <div class="category-header">
                <span class="category-icon">{{ category.icon }}</span>
                <h3>{{ category.title }}</h3>
                <span class="article-count">{{ category.articles.length }} articles</span>
              </div>
              <div class="articles-list">
                <div v-for="article in category.articles" :key="article.id" class="article-item">
                  <span class="article-title">{{ article.title }}</span>
                  <span class="article-views">{{ article.views }} views</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- FAQ Section -->
        <div class="faq-section">
          <h2>Frequently Asked Questions</h2>
          <div class="faq-list">
            <div v-for="faq in faqs" :key="faq.id" class="faq-item">
              <div class="faq-question" @click="toggleFaq(faq.id)">
                <span>{{ faq.question }}</span>
                <span class="faq-toggle">{{ openFaqs.includes(faq.id) ? '−' : '+' }}</span>
              </div>
              <div v-if="openFaqs.includes(faq.id)" class="faq-answer">
                {{ faq.answer }}
              </div>
            </div>
          </div>
        </div>

        <!-- Status & Updates -->
        <div class="status-section">
          <h2>System Status</h2>
          <div class="status-grid">
            <div class="status-card">
              <div class="status-indicator operational">●</div>
              <div class="status-info">
                <h3>Platform Status</h3>
                <p>All systems operational</p>
              </div>
            </div>
            <div class="status-card">
              <div class="status-indicator operational">●</div>
              <div class="status-info">
                <h3>API Services</h3>
                <p>Running smoothly</p>
              </div>
            </div>
            <div class="status-card">
              <div class="status-indicator operational">●</div>
              <div class="status-info">
                <h3>Data Backup</h3>
                <p>Last backup: 2 hours ago</p>
              </div>
            </div>
          </div>
          <button class="btn-status">View Detailed Status</button>
        </div>

        <!-- Recent Updates -->
        <div class="updates-section">
          <h2>Recent Updates</h2>
          <div class="updates-list">
            <div v-for="update in recentUpdates" :key="update.id" class="update-item">
              <div class="update-date">{{ update.date }}</div>
              <div class="update-content">
                <h3>{{ update.title }}</h3>
                <p>{{ update.description }}</p>
              </div>
              <div class="update-type">{{ update.type }}</div>
            </div>
          </div>
        </div>
      </template>
      
      <template #fallback>
        <div class="loading-state">
          <div class="loading-spinner"></div>
          <p>Loading support center...</p>
        </div>
      </template>
    </ClientOnly>
  </div>
</template>

<script setup>
// Page meta
definePageMeta({
  title: 'Support Center'
})

// Reactive data
const searchQuery = ref('')
const openFaqs = ref([])

const knowledgeBase = ref([
  {
    id: 1,
    icon: '🏁',
    title: 'Getting Started',
    articles: [
      { id: 1, title: 'Setting up your business profile', views: 1247 },
      { id: 2, title: 'Understanding the dashboard', views: 987 },
      { id: 3, title: 'Creating your first service', views: 654 },
      { id: 4, title: 'Configuring payment methods', views: 543 }
    ]
  },
  {
    id: 2,
    icon: '💼',
    title: 'Business Management',
    articles: [
      { id: 5, title: 'Managing customer relationships', views: 892 },
      { id: 6, title: 'Tracking business metrics', views: 734 },
      { id: 7, title: 'Setting up automated workflows', views: 621 },
      { id: 8, title: 'Integrating with third-party tools', views: 456 }
    ]
  },
  {
    id: 3,
    icon: '🤝',
    title: 'Network & Collaboration',
    articles: [
      { id: 9, title: 'Finding business partners', views: 567 },
      { id: 10, title: 'Joining group buying initiatives', views: 432 },
      { id: 11, title: 'Setting up resource sharing', views: 321 },
      { id: 12, title: 'Managing referral networks', views: 289 }
    ]
  },
  {
    id: 4,
    icon: '🔧',
    title: 'Technical',
    articles: [
      { id: 13, title: 'API documentation', views: 789 },
      { id: 14, title: 'Troubleshooting login issues', views: 678 },
      { id: 15, title: 'Data export and backup', views: 543 },
      { id: 16, title: 'Security best practices', views: 432 }
    ]
  }
])

const faqs = ref([
  {
    id: 1,
    question: 'How do I get started with the platform?',
    answer: 'Start by completing your business profile, then explore the dashboard to understand the available features. You can access the quick start guide for step-by-step instructions.'
  },
  {
    id: 2,
    question: 'What payment methods are accepted?',
    answer: 'We accept all major credit cards, debit cards, and bank transfers. For South African businesses, we also support EFT and instant payments through major banks.'
  },
  {
    id: 3,
    question: 'How do I join group buying initiatives?',
    answer: 'Navigate to the Business Network section and click on "Group Buying". You can browse active opportunities and join those that match your business needs.'
  },
  {
    id: 4,
    question: 'Is my business data secure?',
    answer: 'Yes, we use enterprise-grade security measures including encryption, regular security audits, and compliance with international data protection standards.'
  },
  {
    id: 5,
    question: 'Can I export my data?',
    answer: 'Absolutely! You can export your data at any time through the settings page. We support various formats including CSV, Excel, and JSON.'
  }
])

const recentUpdates = ref([
  {
    id: 1,
    date: '2024-01-15',
    title: 'Enhanced Business Network Features',
    description: 'Added new collaboration tools and improved group buying interface',
    type: 'Feature'
  },
  {
    id: 2,
    date: '2024-01-10',
    title: 'Performance Improvements',
    description: 'Optimized dashboard loading times and enhanced mobile experience',
    type: 'Improvement'
  },
  {
    id: 3,
    date: '2024-01-05',
    title: 'Security Update',
    description: 'Enhanced authentication security and updated encryption protocols',
    type: 'Security'
  }
])

// Methods
const toggleFaq = (faqId) => {
  const index = openFaqs.value.indexOf(faqId)
  if (index > -1) {
    openFaqs.value.splice(index, 1)
  } else {
    openFaqs.value.push(faqId)
  }
}
</script>

<style scoped>
.support-page {
  min-height: 100vh;
  background: linear-gradient(135deg, #1e40af 0%, #3730a3 100%);
  color: white;
  padding: 2rem;
}

.support-header {
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
}

.quick-help,
.contact-section,
.knowledge-base,
.faq-section,
.status-section,
.updates-section {
  margin-bottom: 3rem;
}

.quick-help h2,
.contact-section h2,
.knowledge-base h2,
.faq-section h2,
.status-section h2,
.updates-section h2 {
  font-size: 1.875rem;
  font-weight: 700;
  margin-bottom: 1.5rem;
}

.help-grid,
.contact-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 1.5rem;
}

.help-card,
.contact-card {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 15px;
  padding: 1.5rem;
  border: 1px solid rgba(255, 255, 255, 0.2);
  text-align: center;
}

.help-icon,
.contact-icon {
  font-size: 2.5rem;
  margin-bottom: 1rem;
}

.help-card h3,
.contact-card h3 {
  font-size: 1.25rem;
  font-weight: 600;
  margin-bottom: 0.75rem;
}

.help-card p,
.contact-card p {
  opacity: 0.9;
  margin-bottom: 1rem;
  line-height: 1.5;
}

.btn-help,
.btn-contact {
  background: rgba(255, 255, 255, 0.2);
  border: 1px solid rgba(255, 255, 255, 0.3);
  color: white;
  padding: 0.75rem 1.5rem;
  border-radius: 25px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
  width: 100%;
}

.btn-help:hover,
.btn-contact:hover {
  background: rgba(255, 255, 255, 0.3);
  transform: translateY(-2px);
}

.availability,
.response-time,
.phone-info {
  margin-bottom: 1rem;
}

.status.online {
  color: #10b981;
}

.hours,
.time,
.number {
  display: block;
  font-size: 0.875rem;
  opacity: 0.8;
}

.search-section {
  margin-bottom: 2rem;
}

.search-bar {
  display: flex;
  max-width: 500px;
  margin: 0 auto;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 25px;
  padding: 0.5rem;
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.search-input {
  flex: 1;
  background: transparent;
  border: none;
  color: white;
  padding: 0.5rem 1rem;
  font-size: 1rem;
}

.search-input::placeholder {
  color: rgba(255, 255, 255, 0.6);
}

.search-btn {
  background: rgba(255, 255, 255, 0.2);
  border: none;
  color: white;
  padding: 0.5rem 1rem;
  border-radius: 20px;
  cursor: pointer;
}

.kb-categories {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 1.5rem;
}

.kb-category {
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
  padding-bottom: 0.75rem;
  border-bottom: 1px solid rgba(255, 255, 255, 0.2);
}

.category-icon {
  font-size: 1.5rem;
}

.category-header h3 {
  flex: 1;
  font-size: 1.125rem;
  font-weight: 600;
}

.article-count {
  font-size: 0.75rem;
  opacity: 0.7;
  background: rgba(255, 255, 255, 0.2);
  padding: 0.25rem 0.5rem;
  border-radius: 10px;
}

.article-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem 0;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.article-item:last-child {
  border-bottom: none;
}

.article-title {
  font-size: 0.875rem;
  cursor: pointer;
  transition: color 0.3s ease;
}

.article-title:hover {
  color: #60a5fa;
}

.article-views {
  font-size: 0.75rem;
  opacity: 0.6;
}


.faq-item {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 15px;
  border: 1px solid rgba(255, 255, 255, 0.2);
  margin-bottom: 1rem;
}

.faq-question {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.25rem;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.faq-question:hover {
  background: rgba(255, 255, 255, 0.1);
}

.faq-toggle {
  font-size: 1.5rem;
  font-weight: 300;
}

.faq-answer {
  padding: 0 1.25rem 1.25rem;
  opacity: 0.9;
  line-height: 1.6;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
  margin-top: -1px;
}

.status-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1.5rem;
  margin-bottom: 1.5rem;
}

.status-card {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 15px;
  padding: 1.5rem;
  border: 1px solid rgba(255, 255, 255, 0.2);
  display: flex;
  align-items: center;
  gap: 1rem;
}

.status-indicator {
  font-size: 1.5rem;
}

.status-indicator.operational {
  color: #10b981;
}

.status-info h3 {
  font-size: 1rem;
  font-weight: 600;
  margin-bottom: 0.25rem;
}

.status-info p {
  font-size: 0.875rem;
  opacity: 0.8;
}

.btn-status {
  background: rgba(255, 255, 255, 0.2);
  border: 1px solid rgba(255, 255, 255, 0.3);
  color: white;
  padding: 0.75rem 2rem;
  border-radius: 25px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-status:hover {
  background: rgba(255, 255, 255, 0.3);
}



.update-item {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 15px;
  padding: 1.5rem;
  border: 1px solid rgba(255, 255, 255, 0.2);
  display: flex;
  gap: 1.5rem;
  align-items: flex-start;
  margin-bottom: 1rem;
}

.update-date {
  font-size: 0.875rem;
  opacity: 0.7;
  min-width: 100px;
}

.update-content {
  flex: 1;
}

.update-content h3 {
  font-size: 1.125rem;
  font-weight: 600;
  margin-bottom: 0.5rem;
}

.update-content p {
  opacity: 0.9;
  line-height: 1.5;
}

.update-type {
  background: rgba(255, 255, 255, 0.2);
  padding: 0.25rem 0.75rem;
  border-radius: 15px;
  font-size: 0.75rem;
  font-weight: 500;
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
  .support-page {
    padding: 1rem;
  }

  .header-content h1 {
    font-size: 2rem;
  }

  .help-grid,
  .contact-grid {
    grid-template-columns: 1fr;
  }

  .kb-categories {
    grid-template-columns: 1fr;
  }

  .status-grid {
    grid-template-columns: 1fr;
  }

  .update-item {
    flex-direction: column;
    gap: 1rem;
  }

  .update-date {
    min-width: auto;
  }
}
</style>
