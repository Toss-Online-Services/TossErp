import { APIRequestContext } from '@playwright/test';

/**
 * Sales-specific API helper functions for E2E tests
 */

const API_BASE_URL = process.env.API_BASE_URL || 'https://localhost:5001/api';

/**
 * Create a sale via API
 */
export async function createSaleAPI(
  request: APIRequestContext,
  saleData: {
    shopId: number;
    customerId?: number;
    items: Array<{
      productId: number;
      quantity: number;
      unitPrice: number;
    }>;
    paymentType: 'Cash' | 'Card' | 'MobileMoney' | 'BankTransfer' | 'PayLink';
    totalAmount: number;
  },
  token?: string
) {
  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
  };

  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }

  const response = await request.post(`${API_BASE_URL}/Sales`, {
    headers,
    data: saleData,
    ignoreHTTPSErrors: true,
  });

  if (!response.ok()) {
    const errorText = await response.text();
    throw new Error(`Failed to create sale: ${response.status()} ${errorText}`);
  }

  return await response.json();
}

/**
 * Create an invoice from a sale via API
 */
export async function createInvoiceAPI(
  request: APIRequestContext,
  invoiceData: {
    saleId: number;
    invoiceNumber?: string;
    dueDate?: string;
    notes?: string;
  },
  token?: string
) {
  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
  };

  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }

  const response = await request.post(`${API_BASE_URL}/Sales/invoices`, {
    headers,
    data: invoiceData,
    ignoreHTTPSErrors: true,
  });

  if (!response.ok()) {
    const errorText = await response.text();
    throw new Error(`Failed to create invoice: ${response.status()} ${errorText}`);
  }

  return await response.json();
}

/**
 * Get sales documents (invoices/receipts) via API
 */
export async function getSalesDocumentsAPI(
  request: APIRequestContext,
  params: {
    shopId: number;
    type?: number; // 0 = Receipt, 1 = Invoice
    pageNumber?: number;
    pageSize?: number;
  },
  token?: string
) {
  const queryParams = new URLSearchParams({
    shopId: params.shopId.toString(),
    pageNumber: (params.pageNumber || 1).toString(),
    pageSize: (params.pageSize || 100).toString(),
  });

  if (params.type !== undefined) {
    queryParams.append('type', params.type.toString());
  }

  const headers: Record<string, string> = {};
  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }

  const response = await request.get(`${API_BASE_URL}/Sales/documents?${queryParams.toString()}`, {
    headers,
    ignoreHTTPSErrors: true,
  });

  if (!response.ok()) {
    const errorText = await response.text();
    throw new Error(`Failed to get sales documents: ${response.status()} ${errorText}`);
  }

  return await response.json();
}

/**
 * Get a specific sale by ID
 */
export async function getSaleAPI(
  request: APIRequestContext,
  saleId: number,
  token?: string
) {
  const headers: Record<string, string> = {};
  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }

  const response = await request.get(`${API_BASE_URL}/Sales/${saleId}`, {
    headers,
    ignoreHTTPSErrors: true,
  });

  if (!response.ok()) {
    const errorText = await response.text();
    throw new Error(`Failed to get sale: ${response.status()} ${errorText}`);
  }

  return await response.json();
}

/**
 * Generate test sale data
 */
export function generateSaleData(shopId: number, productIds: number[] = [1, 2]) {
  const items = productIds.map((productId, index) => ({
    productId,
    quantity: (index + 1) * 2,
    unitPrice: 25.00 + (index * 10),
  }));

  const subtotal = items.reduce((sum, item) => sum + (item.quantity * item.unitPrice), 0);

  return {
    shopId,
    customerId: 1,
    items,
    paymentType: 'Cash' as const,
    totalAmount: subtotal,
  };
}

/**
 * Generate invoice data from a sale
 */
export function generateInvoiceData(saleId: number, invoiceNumber?: string) {
  const now = new Date();
  const dueDate = new Date(now);
  dueDate.setDate(dueDate.getDate() + 30);

  return {
    saleId,
    invoiceNumber: invoiceNumber || `INV-${now.getFullYear()}-${String(saleId).padStart(3, '0')}`,
    dueDate: dueDate.toISOString().split('T')[0],
    notes: 'Payment due within 30 days. Thank you for your business!',
  };
}

