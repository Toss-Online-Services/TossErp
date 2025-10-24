# 🎯 TOSS MVP - Frontend Integration Plan (Phase 5)

**Target:** Wire up Nuxt 4 frontend to ASP.NET Core backend  
**Duration:** 2-3 days  
**Current Status:** Backend 100% complete, Frontend 0% integrated

---

## 📊 **Current State Analysis**

### **Frontend Structure (toss-web/)**
```
✅ Components: 60+ Vue components
✅ Pages: 30+ route pages
✅ Composables: 27 composables
✅ Stores: 8 Pinia stores
✅ Server Routes: 98 .ts API files
✅ Types: 10 TypeScript type files
✅ Tests: 11+ test files
```

### **Backend API (backend/Toss)**
```
✅ Endpoints: 53 REST methods
✅ Modules: 11 endpoint groups
✅ Auth: Identity + JWT ready
✅ Swagger: OpenAPI documentation
✅ Base URL: http://localhost:5001 (development)
```

---

## 🚀 **Integration Strategy**

### **Approach: Hybrid Integration**
1. **Direct API Calls** - Composables call backend directly
2. **Server Middleware** - For SSR/auth handling
3. **Type Generation** - From OpenAPI spec
4. **Incremental Migration** - Module by module

---

## 📋 **Phase 5 Task Breakdown**

### **Task 5.1: Configuration Setup** (30 minutes)

#### 5.1.1 Update nuxt.config.ts
```typescript
export default defineNuxtConfig({
  runtimeConfig: {
    public: {
      apiBase: process.env.NUXT_PUBLIC_API_BASE || 'http://localhost:5001',
      apiTimeout: 30000
    }
  },
  // ... rest of config
})
```

#### 5.1.2 Create .env file
```bash
# toss-web/.env
NUXT_PUBLIC_API_BASE=http://localhost:5001
```

---

### **Task 5.2: Generate TypeScript Types** (1 hour)

#### 5.2.1 Install OpenAPI Generator
```bash
cd toss-web
npm install -D openapi-typescript
```

#### 5.2.2 Generate Types from Backend
```bash
# Start backend first
cd ../backend/Toss/src/Web
dotnet run

# In another terminal, generate types
cd ../../../../toss-web
npx openapi-typescript http://localhost:5001/swagger/v1/swagger.json --output types/api-generated.ts
```

#### 5.2.3 Create Type Mappings
Create `types/api.ts` to export generated types with proper names

---

### **Task 5.3: Update Core Composables** (4 hours)

#### 5.3.1 Update useApi.ts (Base API Client)
**File:** `toss-web/composables/useApi.ts`

```typescript
export const useApi = () => {
  const config = useRuntimeConfig()
  const { token } = useAuth()

  const baseURL = config.public.apiBase

  const api = $fetch.create({
    baseURL,
    headers: {
      'Content-Type': 'application/json'
    },
    onRequest({ options }) {
      if (token.value) {
        options.headers = {
          ...options.headers,
          Authorization: `Bearer ${token.value}`
        }
      }
    },
    onResponseError({ response }) {
      // Handle errors
      console.error('API Error:', response._data)
    }
  })

  return { api, baseURL }
}
```

#### 5.3.2 Update useSalesAPI.ts
**File:** `toss-web/composables/useSalesAPI.ts`

Map to backend endpoints:
```typescript
export const useSalesAPI = () => {
  const { api } = useApi()

  const createSale = async (saleData: CreateSaleDto) => {
    return await api('/api/sales', {
      method: 'POST',
      body: saleData
    })
  }

  const getSales = async (params?: SalesQueryParams) => {
    return await api('/api/sales', {
      method: 'GET',
      query: params
    })
  }

  const getDailySummary = async (shopId: number) => {
    return await api('/api/sales/daily-summary', {
      method: 'GET',
      query: { shopId }
    })
  }

  const voidSale = async (saleId: number) => {
    return await api(`/api/sales/${saleId}/void`, {
      method: 'POST'
    })
  }

  const generateReceipt = async (saleId: number) => {
    return await api(`/api/sales/${saleId}/receipt`, {
      method: 'POST'
    })
  }

  return {
    createSale,
    getSales,
    getDailySummary,
    voidSale,
    generateReceipt
  }
}
```

#### 5.3.3 Update useStock.ts (Inventory)
**File:** `toss-web/composables/useStock.ts`

```typescript
export const useStock = () => {
  const { api } = useApi()

  const getProducts = async (params?: ProductQueryParams) => {
    return await api('/api/inventory/products', {
      method: 'GET',
      query: params
    })
  }

  const createProduct = async (productData: CreateProductDto) => {
    return await api('/api/inventory/products', {
      method: 'POST',
      body: productData
    })
  }

  const getStockLevels = async (shopId: number) => {
    return await api('/api/inventory/stock-levels', {
      method: 'GET',
      query: { shopId }
    })
  }

  const adjustStock = async (adjustmentData: AdjustStockDto) => {
    return await api('/api/inventory/stock/adjust', {
      method: 'POST',
      body: adjustmentData
    })
  }

  const getLowStockAlerts = async (shopId: number) => {
    return await api('/api/inventory/low-stock-alerts', {
      method: 'GET',
      query: { shopId }
    })
  }

  return {
    getProducts,
    createProduct,
    getStockLevels,
    adjustStock,
    getLowStockAlerts
  }
}
```

#### 5.3.4 Update useGroupBuying.ts
**File:** `toss-web/composables/useGroupBuying.ts`

```typescript
export const useGroupBuying = () => {
  const { api } = useApi()

  const createPool = async (poolData: CreatePoolDto) => {
    return await api('/api/group-buying/pools', {
      method: 'POST',
      body: poolData
    })
  }

  const getActivePools = async (params?: PoolQueryParams) => {
    return await api('/api/group-buying/pools/active', {
      method: 'GET',
      query: params
    })
  }

  const joinPool = async (poolId: number, participationData: JoinPoolDto) => {
    return await api(`/api/group-buying/pools/${poolId}/join`, {
      method: 'POST',
      body: participationData
    })
  }

  const confirmPool = async (poolId: number) => {
    return await api(`/api/group-buying/pools/${poolId}/confirm`, {
      method: 'POST'
    })
  }

  const getMyParticipations = async (shopId: number) => {
    return await api('/api/group-buying/participations', {
      method: 'GET',
      query: { shopId }
    })
  }

  const getNearbyOpportunities = async (shopId: number, maxDistance?: number) => {
    return await api('/api/group-buying/opportunities', {
      method: 'GET',
      query: { shopId, maxDistanceKm: maxDistance }
    })
  }

  return {
    createPool,
    getActivePools,
    joinPool,
    confirmPool,
    getMyParticipations,
    getNearbyOpportunities
  }
}
```

#### 5.3.5 Update useSharedDelivery.ts
**File:** `toss-web/composables/useSharedDelivery.ts`

```typescript
export const useSharedDelivery = () => {
  const { api } = useApi()

  const createDeliveryRun = async (runData: CreateSharedDeliveryRunDto) => {
    return await api('/api/logistics/delivery-runs', {
      method: 'POST',
      body: runData
    })
  }

  const getDeliveryRuns = async (params?: DeliveryRunQueryParams) => {
    return await api('/api/logistics/delivery-runs', {
      method: 'GET',
      query: params
    })
  }

  const getDriverRunView = async (runId: number, driverId: number) => {
    return await api(`/api/logistics/delivery-runs/${runId}/driver-view`, {
      method: 'GET',
      query: { driverId }
    })
  }

  const updateDeliveryStatus = async (runId: number, statusData: UpdateDeliveryStatusDto) => {
    return await api(`/api/logistics/delivery-runs/${runId}/status`, {
      method: 'POST',
      body: statusData
    })
  }

  const captureProofOfDelivery = async (stopId: number, podData: CaptureProofOfDeliveryDto) => {
    return await api(`/api/logistics/delivery-stops/${stopId}/proof`, {
      method: 'POST',
      body: podData
    })
  }

  return {
    createDeliveryRun,
    getDeliveryRuns,
    getDriverRunView,
    updateDeliveryStatus,
    captureProofOfDelivery
  }
}
```

#### 5.3.6 Update useBuyingAPI.ts
**File:** `toss-web/composables/useBuyingAPI.ts`

```typescript
export const useBuyingAPI = () => {
  const { api } = useApi()

  const createPurchaseOrder = async (poData: CreatePurchaseOrderDto) => {
    return await api('/api/buying/purchase-orders', {
      method: 'POST',
      body: poData
    })
  }

  const getPurchaseOrderById = async (id: number) => {
    return await api(`/api/buying/purchase-orders/${id}`, {
      method: 'GET'
    })
  }

  const approvePurchaseOrder = async (id: number) => {
    return await api(`/api/buying/purchase-orders/${id}/approve`, {
      method: 'POST'
    })
  }

  return {
    createPurchaseOrder,
    getPurchaseOrderById,
    approvePurchaseOrder
  }
}
```

#### 5.3.7 Update useDashboard.ts
**File:** `toss-web/composables/useDashboard.ts`

```typescript
export const useDashboard = () => {
  const { api } = useApi()

  const getDashboardSummary = async (shopId: number) => {
    return await api('/api/dashboard/summary', {
      method: 'GET',
      query: { shopId }
    })
  }

  const getSalesTrends = async (shopId: number, days?: number) => {
    return await api('/api/dashboard/sales-trends', {
      method: 'GET',
      query: { shopId, days }
    })
  }

  const getTopProducts = async (shopId: number, limit?: number) => {
    return await api('/api/dashboard/top-products', {
      method: 'GET',
      query: { shopId, limit }
    })
  }

  const getCashFlowSummary = async (shopId: number) => {
    return await api('/api/dashboard/cash-flow', {
      method: 'GET',
      query: { shopId }
    })
  }

  return {
    getDashboardSummary,
    getSalesTrends,
    getTopProducts,
    getCashFlowSummary
  }
}
```

#### 5.3.8 Create useSuppliers.ts (New)
**File:** `toss-web/composables/useSuppliers.ts`

```typescript
export const useSuppliers = () => {
  const { api } = useApi()

  const getSuppliers = async (params?: SupplierQueryParams) => {
    return await api('/api/suppliers', {
      method: 'GET',
      query: params
    })
  }

  const getSupplierById = async (id: number) => {
    return await api(`/api/suppliers/${id}`, {
      method: 'GET'
    })
  }

  const createSupplier = async (supplierData: CreateSupplierDto) => {
    return await api('/api/suppliers', {
      method: 'POST',
      body: supplierData
    })
  }

  const getSupplierProducts = async (supplierId: number) => {
    return await api(`/api/suppliers/${supplierId}/products`, {
      method: 'GET'
    })
  }

  const linkSupplierProduct = async (supplierId: number, productData: LinkSupplierProductDto) => {
    return await api(`/api/suppliers/${supplierId}/products`, {
      method: 'POST',
      body: productData
    })
  }

  return {
    getSuppliers,
    getSupplierById,
    createSupplier,
    getSupplierProducts,
    linkSupplierProduct
  }
}
```

#### 5.3.9 Create useCustomers.ts (New)
**File:** `toss-web/composables/useCustomers.ts`

```typescript
export const useCustomers = () => {
  const { api } = useApi()

  const getCustomers = async (params?: CustomerQueryParams) => {
    return await api('/api/crm/customers', {
      method: 'GET',
      query: params
    })
  }

  const getCustomerProfile = async (id: number) => {
    return await api(`/api/crm/customers/${id}`, {
      method: 'GET'
    })
  }

  const createCustomer = async (customerData: CreateCustomerDto) => {
    return await api('/api/crm/customers', {
      method: 'POST',
      body: customerData
    })
  }

  return {
    getCustomers,
    getCustomerProfile,
    createCustomer
  }
}
```

#### 5.3.10 Create usePayments.ts (New)
**File:** `toss-web/composables/usePayments.ts`

```typescript
export const usePayments = () => {
  const { api } = useApi()

  const generatePayLink = async (payLinkData: GeneratePayLinkDto) => {
    return await api('/api/payments/pay-links', {
      method: 'POST',
      body: payLinkData
    })
  }

  const recordPayment = async (paymentData: RecordPaymentDto) => {
    return await api('/api/payments/record', {
      method: 'POST',
      body: paymentData
    })
  }

  const getPayments = async (params?: PaymentQueryParams) => {
    return await api('/api/payments', {
      method: 'GET',
      query: params
    })
  }

  return {
    generatePayLink,
    recordPayment,
    getPayments
  }
}
```

---

### **Task 5.4: Update Authentication** (2 hours)

#### 5.4.1 Update useAuth.ts
**File:** `toss-web/composables/useAuth.ts`

```typescript
export const useAuth = () => {
  const { api } = useApi()
  const token = useCookie('auth-token')
  const user = useState('user', () => null)

  const login = async (email: string, password: string) => {
    const response = await api('/api/auth/login', {
      method: 'POST',
      body: { email, password }
    })

    if (response.accessToken) {
      token.value = response.accessToken
      user.value = response.user
    }

    return response
  }

  const logout = async () => {
    await api('/api/auth/logout', {
      method: 'POST'
    })

    token.value = null
    user.value = null

    navigateTo('/login')
  }

  const refreshToken = async () => {
    const response = await api('/api/auth/refresh', {
      method: 'POST',
      body: { refreshToken: token.value }
    })

    if (response.accessToken) {
      token.value = response.accessToken
    }

    return response
  }

  const verifyToken = async () => {
    const response = await api('/api/auth/verify', {
      method: 'POST'
    })

    if (response.user) {
      user.value = response.user
    }

    return response
  }

  return {
    token,
    user,
    login,
    logout,
    refreshToken,
    verifyToken
  }
}
```

---

### **Task 5.5: Update Pinia Stores** (2 hours)

#### 5.5.1 Update inventory.ts Store
**File:** `toss-web/stores/inventory.ts`

```typescript
export const useInventoryStore = defineStore('inventory', () => {
  const products = ref([])
  const stockLevels = ref([])
  const lowStockAlerts = ref([])
  const loading = ref(false)

  const { getProducts, getStockLevels, getLowStockAlerts, createProduct, adjustStock } = useStock()

  const fetchProducts = async (params) => {
    loading.value = true
    try {
      products.value = await getProducts(params)
    } finally {
      loading.value = false
    }
  }

  const fetchStockLevels = async (shopId) => {
    loading.value = true
    try {
      stockLevels.value = await getStockLevels(shopId)
    } finally {
      loading.value = false
    }
  }

  const fetchLowStockAlerts = async (shopId) => {
    lowStockAlerts.value = await getLowStockAlerts(shopId)
  }

  return {
    products,
    stockLevels,
    lowStockAlerts,
    loading,
    fetchProducts,
    fetchStockLevels,
    fetchLowStockAlerts
  }
})
```

#### 5.5.2 Update groupBuying.ts Store
**File:** `toss-web/stores/groupBuying.ts`

```typescript
export const useGroupBuyingStore = defineStore('groupBuying', () => {
  const activePools = ref([])
  const myParticipations = ref([])
  const opportunities = ref([])
  const loading = ref(false)

  const {
    getActivePools,
    createPool,
    joinPool,
    confirmPool,
    getMyParticipations,
    getNearbyOpportunities
  } = useGroupBuying()

  const fetchActivePools = async (params) => {
    loading.value = true
    try {
      activePools.value = await getActivePools(params)
    } finally {
      loading.value = false
    }
  }

  const fetchMyParticipations = async (shopId) => {
    myParticipations.value = await getMyParticipations(shopId)
  }

  const fetchOpportunities = async (shopId, maxDistance) => {
    opportunities.value = await getNearbyOpportunities(shopId, maxDistance)
  }

  return {
    activePools,
    myParticipations,
    opportunities,
    loading,
    fetchActivePools,
    fetchMyParticipations,
    fetchOpportunities
  }
})
```

#### 5.5.3 Update sharedLogistics.ts Store
**File:** `toss-web/stores/sharedLogistics.ts`

```typescript
export const useSharedLogisticsStore = defineStore('sharedLogistics', () => {
  const deliveryRuns = ref([])
  const currentRun = ref(null)
  const loading = ref(false)

  const {
    getDeliveryRuns,
    createDeliveryRun,
    getDriverRunView,
    updateDeliveryStatus,
    captureProofOfDelivery
  } = useSharedDelivery()

  const fetchDeliveryRuns = async (params) => {
    loading.value = true
    try {
      deliveryRuns.value = await getDeliveryRuns(params)
    } finally {
      loading.value = false
    }
  }

  const fetchDriverRunView = async (runId, driverId) => {
    currentRun.value = await getDriverRunView(runId, driverId)
  }

  return {
    deliveryRuns,
    currentRun,
    loading,
    fetchDeliveryRuns,
    fetchDriverRunView
  }
})
```

---

### **Task 5.6: Update Server Routes** (3 hours)

**Strategy:** Keep existing server routes as a compatibility layer but have them proxy to backend.

#### Example: sales/index.post.ts
**File:** `toss-web/server/api/sales/index.post.ts`

```typescript
export default defineEventHandler(async (event) => {
  const config = useRuntimeConfig()
  const body = await readBody(event)
  const token = getCookie(event, 'auth-token')

  const response = await $fetch(`${config.public.apiBase}/api/sales`, {
    method: 'POST',
    body,
    headers: {
      Authorization: token ? `Bearer ${token}` : ''
    }
  })

  return response
})
```

---

## 📝 **Integration Checklist**

### Configuration ✅
- [ ] Update nuxt.config.ts with backend URL
- [ ] Create .env file
- [ ] Configure CORS on backend

### Types ✅
- [ ] Install openapi-typescript
- [ ] Generate types from OpenAPI spec
- [ ] Create type mappings

### Composables ✅
- [ ] Update useApi.ts (base client)
- [ ] Update useAuth.ts
- [ ] Update useSalesAPI.ts
- [ ] Update useStock.ts
- [ ] Update useGroupBuying.ts
- [ ] Update useSharedDelivery.ts
- [ ] Update useBuyingAPI.ts
- [ ] Update useDashboard.ts
- [ ] Create useSuppliers.ts
- [ ] Create useCustomers.ts
- [ ] Create usePayments.ts

### Stores ✅
- [ ] Update inventory.ts
- [ ] Update groupBuying.ts
- [ ] Update sharedLogistics.ts
- [ ] Update customers.ts
- [ ] Update settings.ts

### Server Routes ✅
- [ ] Update/create proxy routes for each module
- [ ] Add error handling
- [ ] Add request logging

### Pages ✅
- [ ] Test dashboard page
- [ ] Test sales pages
- [ ] Test inventory pages
- [ ] Test group buying pages
- [ ] Test logistics pages

### Authentication ✅
- [ ] Login flow
- [ ] Token refresh
- [ ] Logout
- [ ] Route guards

---

## 🧪 **Testing Strategy**

### Unit Tests
- Test composables with mocked API
- Test stores with mocked composables

### Integration Tests
- Test full flow from page → store → composable → API

### E2E Tests
- Test critical user journeys
- POS transaction
- Group buying pool creation
- Delivery tracking

---

## ⏱️ **Estimated Timeline**

| Task | Duration | Status |
|------|----------|--------|
| 5.1 Configuration | 30 min | ⏸️ Pending |
| 5.2 Type Generation | 1 hour | ⏸️ Pending |
| 5.3 Update Composables | 4 hours | ⏸️ Pending |
| 5.4 Update Auth | 2 hours | ⏸️ Pending |
| 5.5 Update Stores | 2 hours | ⏸️ Pending |
| 5.6 Update Server Routes | 3 hours | ⏸️ Pending |
| 5.7 Testing | 4 hours | ⏸️ Pending |
| 5.8 Bug Fixes | 2 hours | ⏸️ Pending |

**Total:** ~18 hours (2-3 days)

---

## 🚀 **Next Steps After Integration**

1. **Phase 6:** Testing & validation
2. **Phase 7:** External service integration
3. **Phase 8:** Deployment configuration

---

**Status:** Ready to begin Task 5.1  
**Date:** 2025-10-24

