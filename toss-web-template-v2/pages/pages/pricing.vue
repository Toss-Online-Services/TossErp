<template>
  <div class="container-fluid py-4">
    <div class="row">
      <div class="col-12 text-center mb-4">
        <MDTypography variant="h2" font-weight="bold" class="mb-2">
          Choose Your Plan
        </MDTypography>
        <MDTypography variant="body1" color="text">
          Simple, transparent pricing that grows with you. Try any plan free for 30 days.
        </MDTypography>
      </div>
    </div>
    
    <div class="row mt-5">
      <div v-for="plan in plans" :key="plan.name" class="col-lg-4 col-md-6 mb-4">
        <div class="card" :class="{ 'border-info': plan.featured }">
          <div class="card-header text-center pt-4 pb-3" :class="{ 'bg-gradient-info': plan.featured }">
            <MDBadge 
              v-if="plan.featured" 
              color="white" 
              variant="contained" 
              size="sm"
              class="mb-2"
            >
              Most Popular
            </MDBadge>
            <MDTypography 
              variant="h5" 
              font-weight="bold"
              :color="plan.featured ? 'white' : 'dark'"
            >
              {{ plan.name }}
            </MDTypography>
            <div class="mt-3">
              <MDTypography 
                variant="h1" 
                font-weight="bold"
                :color="plan.featured ? 'white' : 'dark'"
              >
                ${{ plan.price }}
              </MDTypography>
              <MDTypography 
                variant="caption" 
                :color="plan.featured ? 'white' : 'text'"
              >
                per month
              </MDTypography>
            </div>
          </div>
          <div class="card-body text-center">
            <ul class="list-unstyled my-4">
              <li v-for="(feature, index) in plan.features" :key="index" class="mb-3">
                <Icon 
                  :name="feature.included ? 'mdi:check-circle' : 'mdi:close-circle'" 
                  :class="feature.included ? 'text-success' : 'text-secondary'"
                  size="20"
                  class="me-2"
                />
                <MDTypography 
                  variant="button" 
                  :color="feature.included ? 'dark' : 'text'"
                >
                  {{ feature.text }}
                </MDTypography>
              </li>
            </ul>
            <MDButton 
              :color="plan.featured ? 'info' : 'light'" 
              :variant="plan.featured ? 'gradient' : 'outlined'"
              size="large"
              full-width
            >
              Get Started
            </MDButton>
          </div>
        </div>
      </div>
    </div>
    
    <div class="row mt-5">
      <div class="col-12">
        <div class="card">
          <div class="card-header">
            <MDTypography variant="h5" font-weight="bold">
              Frequently Asked Questions
            </MDTypography>
          </div>
          <div class="card-body">
            <div v-for="(faq, index) in faqs" :key="index" class="mb-4">
              <MDTypography variant="button" font-weight="bold" class="mb-2">
                {{ faq.question }}
              </MDTypography>
              <MDTypography variant="body2" color="text">
                {{ faq.answer }}
              </MDTypography>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

definePageMeta({
  layout: 'default'
})

const plans = ref([
  {
    name: 'Starter',
    price: 29,
    featured: false,
    features: [
      { text: '5 Team Members', included: true },
      { text: '10 GB Storage', included: true },
      { text: 'Basic Support', included: true },
      { text: 'Advanced Analytics', included: false },
      { text: 'Custom Domain', included: false }
    ]
  },
  {
    name: 'Professional',
    price: 59,
    featured: true,
    features: [
      { text: '15 Team Members', included: true },
      { text: '50 GB Storage', included: true },
      { text: 'Priority Support', included: true },
      { text: 'Advanced Analytics', included: true },
      { text: 'Custom Domain', included: false }
    ]
  },
  {
    name: 'Enterprise',
    price: 99,
    featured: false,
    features: [
      { text: 'Unlimited Team Members', included: true },
      { text: '500 GB Storage', included: true },
      { text: '24/7 Premium Support', included: true },
      { text: 'Advanced Analytics', included: true },
      { text: 'Custom Domain', included: true }
    ]
  }
])

const faqs = ref([
  {
    question: 'Can I change my plan later?',
    answer: 'Yes, you can upgrade or downgrade your plan at any time. Changes will be reflected in your next billing cycle.'
  },
  {
    question: 'What payment methods do you accept?',
    answer: 'We accept all major credit cards, PayPal, and bank transfers for annual subscriptions.'
  },
  {
    question: 'Is there a free trial?',
    answer: 'Yes! All plans come with a 30-day free trial. No credit card required.'
  },
  {
    question: 'What happens when I cancel?',
    answer: 'You can cancel anytime. Your account will remain active until the end of your billing period.'
  }
])
</script>

<style scoped>
.list-unstyled {
  padding-left: 0;
  list-style: none;
}

.border-info {
  border: 2px solid var(--md-info, #1a73e8) !important;
}

.text-success {
  color: var(--md-success, #4caf50);
}

.text-secondary {
  color: var(--md-secondary, #7b809a);
}
</style>
