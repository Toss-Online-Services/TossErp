import { APIRequestContext } from '@playwright/test';

/**
 * API Helper functions for E2E tests
 * These functions make direct API calls to the backend for setup and verification
 */

const API_BASE_URL = process.env.API_BASE_URL || 'http://localhost:5000/api';

// Store management
export async function createStoreAPI(request: APIRequestContext, storeData: any, token: string) {
  const response = await request.post(`${API_BASE_URL}/stores`, {
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
    data: storeData,
  });
  
  if (!response.ok()) {
    throw new Error(`Failed to create store: ${response.status()} ${await response.text()}`);
  }
  
  return await response.json();
}

export async function getStoreAPI(request: APIRequestContext, storeId: number, token: string) {
  const response = await request.get(`${API_BASE_URL}/stores/${storeId}`, {
    headers: {
      'Authorization': `Bearer ${token}`,
    },
  });
  
  if (!response.ok()) {
    throw new Error(`Failed to get store: ${response.status()} ${await response.text()}`);
  }
  
  return await response.json();
}

// User management
export async function registerUserAPI(request: APIRequestContext, userData: any) {
  const response = await request.post(`${API_BASE_URL}/users/register`, {
    headers: {
      'Content-Type': 'application/json',
    },
    data: userData,
  });
  
  if (!response.ok()) {
    throw new Error(`Failed to register user: ${response.status()} ${await response.text()}`);
  }
  
  return await response.json();
}

export async function loginUserAPI(request: APIRequestContext, email: string, password: string) {
  const response = await request.post(`${API_BASE_URL}/users/login`, {
    headers: {
      'Content-Type': 'application/json',
    },
    data: { email, password },
  });
  
  if (!response.ok()) {
    throw new Error(`Failed to login: ${response.status()} ${await response.text()}`);
  }
  
  return await response.json();
}

export async function createUserAPI(request: APIRequestContext, userData: any, token: string) {
  const response = await request.post(`${API_BASE_URL}/users`, {
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
    data: userData,
  });
  
  if (!response.ok()) {
    throw new Error(`Failed to create user: ${response.status()} ${await response.text()}`);
  }
  
  return await response.json();
}

// Customer management
export async function createCustomerAPI(request: APIRequestContext, customerData: any, token: string) {
  const response = await request.post(`${API_BASE_URL}/crm/customers`, {
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
    data: customerData,
  });
  
  if (!response.ok()) {
    throw new Error(`Failed to create customer: ${response.status()} ${await response.text()}`);
  }
  
  return await response.json();
}

export async function getCustomerAPI(request: APIRequestContext, customerId: number, token: string) {
  const response = await request.get(`${API_BASE_URL}/crm/customers/${customerId}`, {
    headers: {
      'Authorization': `Bearer ${token}`,
    },
  });
  
  if (!response.ok()) {
    throw new Error(`Failed to get customer: ${response.status()} ${await response.text()}`);
  }
  
  return await response.json();
}

// Product management
export async function createProductAPI(request: APIRequestContext, productData: any, token: string) {
  const response = await request.post(`${API_BASE_URL}/inventory/products`, {
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
    data: productData,
  });
  
  if (!response.ok()) {
    throw new Error(`Failed to create product: ${response.status()} ${await response.text()}`);
  }
  
  return await response.json();
}

export async function adjustStockAPI(request: APIRequestContext, adjustmentData: any, token: string) {
  const response = await request.post(`${API_BASE_URL}/inventory/stock/adjust`, {
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
    data: adjustmentData,
  });
  
  if (!response.ok()) {
    throw new Error(`Failed to adjust stock: ${response.status()} ${await response.text()}`);
  }
  
  return await response.json();
}

// Sales management
export async function createSaleAPI(request: APIRequestContext, saleData: any, token: string) {
  const response = await request.post(`${API_BASE_URL}/sales`, {
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
    data: saleData,
  });
  
  if (!response.ok()) {
    throw new Error(`Failed to create sale: ${response.status()} ${await response.text()}`);
  }
  
  return await response.json();
}

export async function getSaleAPI(request: APIRequestContext, saleId: number, token: string) {
  const response = await request.get(`${API_BASE_URL}/sales/${saleId}`, {
    headers: {
      'Authorization': `Bearer ${token}`,
    },
  });
  
  if (!response.ok()) {
    throw new Error(`Failed to get sale: ${response.status()} ${await response.text()}`);
  }
  
  return await response.json();
}

// Vendor management
export async function createVendorAPI(request: APIRequestContext, vendorData: any, token: string) {
  const response = await request.post(`${API_BASE_URL}/vendors`, {
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
    data: vendorData,
  });
  
  if (!response.ok()) {
    throw new Error(`Failed to create vendor: ${response.status()} ${await response.text()}`);
  }
  
  return await response.json();
}

// Purchase Order management
export async function createPurchaseOrderAPI(request: APIRequestContext, poData: any, token: string) {
  const response = await request.post(`${API_BASE_URL}/buying/purchase-orders`, {
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
    data: poData,
  });
  
  if (!response.ok()) {
    throw new Error(`Failed to create purchase order: ${response.status()} ${await response.text()}`);
  }
  
  return await response.json();
}

// Driver and Logistics management
export async function createDriverAPI(request: APIRequestContext, driverData: any, token: string) {
  const response = await request.post(`${API_BASE_URL}/logistics/drivers`, {
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
    data: driverData,
  });
  
  if (!response.ok()) {
    throw new Error(`Failed to create driver: ${response.status()} ${await response.text()}`);
  }
  
  return await response.json();
}

export async function createDeliveryRunAPI(request: APIRequestContext, runData: any, token: string) {
  const response = await request.post(`${API_BASE_URL}/logistics/delivery-runs`, {
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
    data: runData,
  });
  
  if (!response.ok()) {
    throw new Error(`Failed to create delivery run: ${response.status()} ${await response.text()}`);
  }
  
  return await response.json();
}

// Helper to wait for API to be ready
export async function waitForAPIReady(request: APIRequestContext, maxAttempts = 30, delayMs = 1000): Promise<boolean> {
  for (let i = 0; i < maxAttempts; i++) {
    try {
      const response = await request.get(`${API_BASE_URL}/health`);
      if (response.ok()) {
        console.log('✅ API is ready');
        return true;
      }
    } catch (error) {
      console.log(`⏳ Waiting for API... (attempt ${i + 1}/${maxAttempts})`);
    }
    
    await new Promise(resolve => setTimeout(resolve, delayMs));
  }
  
  throw new Error('API did not become ready in time');
}

