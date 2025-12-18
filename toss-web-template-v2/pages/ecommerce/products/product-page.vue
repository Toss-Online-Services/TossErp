<template>
  <div class="container-fluid py-4">
    <div class="row">
      <div class="col-lg-8 mb-4">
        <!-- Product Images -->
        <div class="card">
          <div class="card-body">
            <div class="row">
              <div class="col-12 mb-3">
                <img :src="selectedImage" class="img-fluid border-radius-lg w-100" alt="product" style="max-height: 500px; object-fit: cover;">
              </div>
              <div class="col-12">
                <div class="d-flex gap-2">
                  <div 
                    v-for="(image, index) in product.images" 
                    :key="index"
                    class="thumbnail-image"
                    :class="{ active: selectedImage === image }"
                    @click="selectedImage = image"
                  >
                    <img :src="image" class="img-fluid border-radius-lg" alt="thumbnail">
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Product Details -->
        <div class="card mt-4">
          <div class="card-body">
            <MDTypography variant="h6" font-weight="medium" class="mb-3">
              Product Details
            </MDTypography>
            <MDTypography variant="button" color="text" class="d-block mb-3">
              {{ product.description }}
            </MDTypography>

            <MDTypography variant="h6" font-weight="medium" class="mt-4 mb-3">
              Key Features
            </MDTypography>
            <ul class="list-unstyled">
              <li v-for="(feature, index) in product.features" :key="index" class="mb-2">
                <Icon name="mdi:check-circle" class="text-success me-2" />
                <MDTypography variant="button" color="text">
                  {{ feature }}
                </MDTypography>
              </li>
            </ul>

            <MDTypography variant="h6" font-weight="medium" class="mt-4 mb-3">
              Specifications
            </MDTypography>
            <div class="table-responsive">
              <table class="table table-sm">
                <tbody>
                  <tr v-for="(value, key) in product.specifications" :key="key">
                    <td class="w-50">
                      <MDTypography variant="button" font-weight="bold">
                        {{ key }}
                      </MDTypography>
                    </td>
                    <td>
                      <MDTypography variant="button" color="text">
                        {{ value }}
                      </MDTypography>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>

        <!-- Reviews -->
        <div class="card mt-4">
          <div class="card-body">
            <div class="d-flex justify-content-between align-items-center mb-4">
              <MDTypography variant="h6" font-weight="medium">
                Customer Reviews ({{ product.reviews.length }})
              </MDTypography>
              <MDButton color="info" size="sm">
                Write a Review
              </MDButton>
            </div>

            <div v-for="review in product.reviews" :key="review.id" class="mb-4">
              <div class="d-flex mb-2">
                <MDAvatar :src="review.avatar" alt="reviewer" size="md" class="me-3" />
                <div class="flex-grow-1">
                  <div class="d-flex justify-content-between align-items-center">
                    <MDTypography variant="button" font-weight="bold">
                      {{ review.name }}
                    </MDTypography>
                    <MDTypography variant="caption" color="text">
                      {{ review.date }}
                    </MDTypography>
                  </div>
                  <div class="d-flex mb-2">
                    <Icon 
                      v-for="i in 5" 
                      :key="i" 
                      name="mdi:star" 
                      :class="i <= review.rating ? 'text-warning' : 'text-secondary'"
                      size="16"
                    />
                  </div>
                  <MDTypography variant="button" color="text">
                    {{ review.comment }}
                  </MDTypography>
                </div>
              </div>
              <hr class="horizontal dark">
            </div>
          </div>
        </div>
      </div>

      <div class="col-lg-4">
        <!-- Purchase Card -->
        <div class="card position-sticky top-1">
          <div class="card-body">
            <MDBadge v-if="product.badge" :color="product.badge.color" variant="gradient" class="mb-3">
              {{ product.badge.text }}
            </MDBadge>
            
            <MDTypography variant="h4" font-weight="bold" class="mb-2">
              {{ product.name }}
            </MDTypography>
            
            <div class="d-flex align-items-center mb-3">
              <div class="d-flex me-2">
                <Icon 
                  v-for="i in 5" 
                  :key="i" 
                  name="mdi:star" 
                  :class="i <= product.rating ? 'text-warning' : 'text-secondary'"
                  size="18"
                />
              </div>
              <MDTypography variant="caption" color="text">
                {{ product.rating }} ({{ product.reviewCount }} reviews)
              </MDTypography>
            </div>

            <div class="mb-3">
              <span class="text-h3 font-weight-bold text-gradient text-primary">
                ${{ product.price.toFixed(2) }}
              </span>
              <span v-if="product.comparePrice" class="text-sm text-decoration-line-through text-secondary ms-2">
                ${{ product.comparePrice.toFixed(2) }}
              </span>
            </div>

            <MDTypography variant="button" color="text" class="d-block mb-3">
              {{ product.shortDescription }}
            </MDTypography>

            <MDTypography variant="button" font-weight="bold" class="d-block mb-2">
              SKU: {{ product.sku }}
            </MDTypography>

            <div class="mb-3">
              <MDBadge 
                :color="product.stock > 0 ? 'success' : 'error'" 
                variant="gradient"
              >
                {{ product.stock > 0 ? `${product.stock} in stock` : 'Out of Stock' }}
              </MDBadge>
            </div>

            <div class="mb-3">
              <label class="form-label">Quantity</label>
              <div class="input-group">
                <button 
                  class="btn btn-outline-secondary btn-sm" 
                  type="button"
                  @click="decreaseQuantity"
                >
                  <Icon name="mdi:minus" />
                </button>
                <input 
                  v-model.number="quantity" 
                  type="number" 
                  class="form-control text-center" 
                  min="1"
                  :max="product.stock"
                >
                <button 
                  class="btn btn-outline-secondary btn-sm" 
                  type="button"
                  @click="increaseQuantity"
                >
                  <Icon name="mdi:plus" />
                </button>
              </div>
            </div>

            <div class="d-grid gap-2 mb-3">
              <MDButton 
                color="primary" 
                size="lg" 
                :disabled="product.stock === 0"
                @click="addToCart"
              >
                <Icon name="mdi:cart" class="me-2" />
                Add to Cart
              </MDButton>
              <MDButton 
                color="info" 
                size="lg"
                variant="outlined"
                @click="buyNow"
              >
                <Icon name="mdi:lightning-bolt" class="me-2" />
                Buy Now
              </MDButton>
            </div>

            <div class="d-flex gap-2">
              <MDButton color="light" class="flex-grow-1">
                <Icon name="mdi:heart-outline" />
              </MDButton>
              <MDButton color="light" class="flex-grow-1">
                <Icon name="mdi:share-variant" />
              </MDButton>
            </div>

            <hr class="horizontal dark my-4">

            <MDTypography variant="button" font-weight="bold" class="d-block mb-2">
              Shipping & Returns
            </MDTypography>
            <MDTypography variant="caption" color="text" class="d-block mb-2">
              <Icon name="mdi:truck-fast" size="16" class="me-1" />
              Free shipping on orders over $50
            </MDTypography>
            <MDTypography variant="caption" color="text" class="d-block mb-2">
              <Icon name="mdi:package-variant" size="16" class="me-1" />
              30-day return policy
            </MDTypography>
            <MDTypography variant="caption" color="text" class="d-block">
              <Icon name="mdi:shield-check" size="16" class="me-1" />
              2-year warranty included
            </MDTypography>
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

const quantity = ref(1)
const selectedImage = ref('https://images.unsplash.com/photo-1505740420928-5e560c06d30e?auto=format&fit=crop&w=800')

const product = ref({
  name: 'Premium Wireless Headphones',
  sku: 'WH-1000XM4',
  price: 349.99,
  comparePrice: 399.99,
  rating: 4.5,
  reviewCount: 128,
  stock: 45,
  badge: {
    text: 'Best Seller',
    color: 'success'
  },
  shortDescription: 'Industry-leading noise canceling with Dual Noise Sensor technology',
  description: 'Experience premium sound quality with our industry-leading wireless headphones. Featuring advanced noise cancellation technology, superior comfort, and up to 30 hours of battery life, these headphones are perfect for music lovers, travelers, and professionals alike.',
  images: [
    'https://images.unsplash.com/photo-1505740420928-5e560c06d30e?auto=format&fit=crop&w=800',
    'https://images.unsplash.com/photo-1484704849700-f032a568e944?auto=format&fit=crop&w=800',
    'https://images.unsplash.com/photo-1487215078519-e21cc028cb29?auto=format&fit=crop&w=800',
    'https://images.unsplash.com/photo-1546435770-a3e426bf472b?auto=format&fit=crop&w=800'
  ],
  features: [
    'Industry-leading noise canceling with Dual Noise Sensor technology',
    'Next-level music with Edge-AI technology',
    'Up to 30-hour battery life with quick charging (10 min charge = 5 hours)',
    'Touch sensor controls for easy playback control',
    'Speak-to-chat technology automatically reduces volume',
    'Superior call quality with precise voice pickup',
    'Multipoint connection allows connecting to two devices',
    'Adaptive Sound Control adjusts settings to your activity'
  ],
  specifications: {
    'Brand': 'Premium Audio',
    'Model': 'WH-1000XM4',
    'Color': 'Silver',
    'Connectivity': 'Bluetooth 5.0, NFC, Wired',
    'Battery Life': 'Up to 30 hours',
    'Charging Time': '3 hours (full charge)',
    'Driver Unit': '40mm',
    'Frequency Response': '4 Hz - 40,000 Hz',
    'Weight': '254g'
  },
  reviews: [
    {
      id: 1,
      name: 'Sarah Johnson',
      avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-2.jpg',
      rating: 5,
      date: '2 days ago',
      comment: 'Absolutely amazing headphones! The noise cancellation is incredible and the sound quality is top-notch. Worth every penny.'
    },
    {
      id: 2,
      name: 'Michael Chen',
      avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-3.jpg',
      rating: 4,
      date: '1 week ago',
      comment: 'Great headphones overall. The battery life is impressive and they are very comfortable to wear for long periods. Only minor complaint is the touch controls can be a bit sensitive.'
    },
    {
      id: 3,
      name: 'Emma Wilson',
      avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-4.jpg',
      rating: 5,
      date: '2 weeks ago',
      comment: 'Best purchase I made this year! Perfect for my daily commute and working from home. The noise cancellation blocks out everything.'
    }
  ]
})

const increaseQuantity = () => {
  if (quantity.value < product.value.stock) {
    quantity.value++
  }
}

const decreaseQuantity = () => {
  if (quantity.value > 1) {
    quantity.value--
  }
}

const addToCart = () => {
  alert(`Added ${quantity.value} item(s) to cart!`)
}

const buyNow = () => {
  alert('Proceeding to checkout...')
}
</script>

<style scoped>
.thumbnail-image {
  width: 100px;
  height: 100px;
  cursor: pointer;
  border: 2px solid transparent;
  border-radius: 0.5rem;
  overflow: hidden;
  transition: border-color 0.2s;
}

.thumbnail-image:hover,
.thumbnail-image.active {
  border-color: #5e72e4;
}

.thumbnail-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.gap-2 {
  gap: 0.5rem;
}

.d-grid {
  display: grid;
}

.flex-grow-1 {
  flex-grow: 1;
}

.position-sticky {
  position: sticky;
}

.top-1 {
  top: 1rem;
}

.text-h3 {
  font-size: 2rem;
}

.text-gradient {
  background: linear-gradient(195deg, #5e72e4 0%, #825ee4 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.text-decoration-line-through {
  text-decoration: line-through;
}

.input-group {
  display: flex;
}

.input-group input {
  max-width: 80px;
  text-align: center;
  border-left: 0;
  border-right: 0;
}

.input-group button {
  background: white;
  border: 1px solid #d2d6da;
}

.input-group button:hover {
  background: #f8f9fa;
}

.horizontal.dark {
  background-color: rgba(0, 0, 0, 0.1);
}

.list-unstyled {
  padding-left: 0;
  list-style: none;
}
</style>
