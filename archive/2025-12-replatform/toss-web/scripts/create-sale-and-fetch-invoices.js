/**
 * Script to create a sale and fetch invoices/receipts
 * Usage: node scripts/create-sale-and-fetch-invoices.js
 */

import https from 'https';
import http from 'http';

// API Configuration
const API_BASE = process.env.API_BASE_URL || 'https://localhost:5001';
const API_URL = `${API_BASE}/api`;

// Helper to make API requests (handles self-signed certificates)
function makeRequest(url, options = {}) {
  return new Promise((resolve, reject) => {
    const urlObj = new URL(url);
    const isHttps = urlObj.protocol === 'https:';
    const client = isHttps ? https : http;
    
    const requestOptions = {
      hostname: urlObj.hostname,
      port: urlObj.port || (isHttps ? 443 : 80),
      path: urlObj.pathname + urlObj.search,
      method: options.method || 'GET',
      headers: {
        'Content-Type': 'application/json',
        ...options.headers
      },
      // Allow self-signed certificates in development
      rejectUnauthorized: false
    };

    const req = client.request(requestOptions, (res) => {
      let data = '';
      
      res.on('data', (chunk) => {
        data += chunk;
      });
      
      res.on('end', () => {
        try {
          const parsed = data ? JSON.parse(data) : null;
          resolve({
            status: res.statusCode,
            headers: res.headers,
            data: parsed
          });
        } catch (e) {
          resolve({
            status: res.statusCode,
            headers: res.headers,
            data: data
          });
        }
      });
    });

    req.on('error', (error) => {
      reject(error);
    });

    if (options.body) {
      req.write(JSON.stringify(options.body));
    }

    req.end();
  });
}

async function createSale() {
  console.log('📦 Creating a new sale...\n');
  
  const saleData = {
    shopId: 1,
    customerId: 1, // Optional
    items: [
      {
        productId: 1,
        quantity: 2,
        unitPrice: 50.00
      },
      {
        productId: 2,
        quantity: 1,
        unitPrice: 25.00
      }
    ],
    paymentType: 'Cash',
    totalAmount: 125.00
  };

  try {
    const response = await makeRequest(`${API_URL}/Sales`, {
      method: 'POST',
      body: saleData
    });

    if (response.status === 200 || response.status === 201) {
      console.log('✅ Sale created successfully!');
      console.log('Sale ID:', response.data?.id);
      console.log('Response:', JSON.stringify(response.data, null, 2));
      return response.data;
    } else {
      console.error('❌ Failed to create sale');
      console.error('Status:', response.status);
      console.error('Response:', response.data);
      return null;
    }
  } catch (error) {
    console.error('❌ Error creating sale:', error.message);
    return null;
  }
}

async function createInvoiceFromSale(saleId) {
  console.log(`\n📝 Creating invoice from sale ${saleId}...\n`);
  
  try {
    const invoiceData = {
      saleId: saleId,
      invoiceNumber: `INV-${new Date().getFullYear()}-${String(saleId).padStart(3, '0')}`,
      dueDate: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0],
      notes: 'Payment due within 30 days. Thank you for your business!'
    };

    const response = await makeRequest(`${API_URL}/Sales/invoices`, {
      method: 'POST',
      body: invoiceData
    });

    if (response.status === 200 || response.status === 201) {
      console.log('✅ Invoice created successfully!');
      console.log('Response:', JSON.stringify(response.data, null, 2));
      return response.data;
    } else {
      console.error('❌ Failed to create invoice');
      console.error('Status:', response.status);
      console.error('Response:', response.data);
      return null;
    }
  } catch (error) {
    console.error('❌ Error creating invoice:', error.message);
    return null;
  }
}

async function fetchInvoices() {
  console.log('\n📋 Fetching invoices/receipts...\n');
  
  try {
    // Try the unified documents endpoint first (type=1 for invoices)
    console.log('🔍 Fetching invoices (type=1)...');
    const documentsResponse = await makeRequest(
      `${API_URL}/Sales/documents?shopId=1&type=1&pageNumber=1&pageSize=100`
    );

    if (documentsResponse.status === 200) {
      console.log('✅ Successfully fetched documents');
      console.log('Full response:', JSON.stringify(documentsResponse.data, null, 2));
      
      // Handle different response structures
      let documents = [];
      if (documentsResponse.data) {
        if (Array.isArray(documentsResponse.data)) {
          documents = documentsResponse.data;
        } else if (documentsResponse.data.Items) {
          documents = documentsResponse.data.Items;
        } else if (documentsResponse.data.items) {
          documents = documentsResponse.data.items;
        } else if (documentsResponse.data.data) {
          documents = documentsResponse.data.data;
        }
      }
      
      console.log(`\nFound ${documents.length} document(s)`);
      
      if (documents.length > 0) {
        console.log('\n📄 Documents:');
        documents.forEach((doc, index) => {
          // Handle both camelCase and PascalCase property names
          const id = doc.id || doc.Id;
          const docNumber = doc.documentNumber || doc.DocumentNumber || doc.invoiceNumber || doc.InvoiceNumber;
          const docDate = doc.documentDate || doc.DocumentDate || doc.invoiceDate || doc.InvoiceDate;
          const customer = doc.customer || doc.Customer || doc.customerName || doc.CustomerName;
          const total = doc.totalAmount || doc.TotalAmount || doc.total || doc.Total;
          const isPaid = doc.isPaid || doc.IsPaid || false;
          const docType = doc.documentType || doc.DocumentType || 'Invoice';
          const saleNumber = doc.saleNumber || doc.SaleNumber;
          const subtotal = doc.subtotal || doc.Subtotal;
          const taxAmount = doc.taxAmount || doc.TaxAmount;
          const dueDate = doc.dueDate || doc.DueDate;
          
          console.log(`\n${index + 1}. Document #${id || 'N/A'}`);
          console.log(`   Number: ${docNumber || 'N/A'}`);
          console.log(`   Sale Number: ${saleNumber || 'N/A'}`);
          console.log(`   Date: ${docDate ? new Date(docDate).toLocaleDateString() : 'N/A'}`);
          console.log(`   Due Date: ${dueDate ? new Date(dueDate).toLocaleDateString() : 'N/A'}`);
          console.log(`   Customer: ${customer || 'Walk-in Customer'}`);
          console.log(`   Subtotal: R${subtotal || 0}`);
          console.log(`   Tax: R${taxAmount || 0}`);
          console.log(`   Total: R${total || 0}`);
          console.log(`   Status: ${isPaid ? 'Paid' : 'Unpaid'}`);
          console.log(`   Type: ${docType}`);
        });
      } else {
        console.log('No documents found in response');
      }
      
      return documents;
    } else {
      console.error('❌ Failed to fetch documents');
      console.error('Status:', documentsResponse.status);
      console.error('Response:', documentsResponse.data);
    }
  } catch (error) {
    console.error('❌ Error fetching invoices:', error.message);
    
    // Try alternative endpoint
    try {
      console.log('\n🔄 Trying alternative invoices endpoint...');
      const invoicesResponse = await makeRequest(`${API_URL}/Sales/invoices?shopId=1`);
      
      if (invoicesResponse.status === 200) {
        console.log('✅ Successfully fetched invoices');
        const invoices = invoicesResponse.data || [];
        console.log(`Found ${invoices.length} invoice(s)`);
        console.log('Invoices:', JSON.stringify(invoices, null, 2));
        return invoices;
      }
    } catch (altError) {
      console.error('❌ Alternative endpoint also failed:', altError.message);
    }
  }
  
  // Try fetching receipts (type=0 or type=2)
  try {
    console.log('\n🔍 Fetching receipts (type=0)...');
    const receiptsResponse = await makeRequest(
      `${API_URL}/Sales/documents?shopId=1&type=0&pageNumber=1&pageSize=100`
    );
    
    if (receiptsResponse.status === 200 && receiptsResponse.data?.Items) {
      const receipts = receiptsResponse.data.Items;
      console.log(`Found ${receipts.length} receipt(s)`);
      if (receipts.length > 0) {
        receipts.forEach((rec, index) => {
          console.log(`\n${index + 1}. Receipt #${rec.id || rec.documentNumber || 'N/A'}`);
          console.log(`   Number: ${rec.documentNumber || 'N/A'}`);
          console.log(`   Date: ${rec.documentDate || 'N/A'}`);
          console.log(`   Total: R${rec.totalAmount || 0}`);
        });
      }
    }
  } catch (error) {
    console.error('Error fetching receipts:', error.message);
  }
  
  return [];
}

async function main() {
  console.log('🚀 Starting sale creation and invoice fetch process\n');
  console.log(`API Base URL: ${API_BASE}\n`);
  
  // Create a sale
  const sale = await createSale();
  
  // Wait a moment for the sale to be processed
  if (sale) {
    console.log('\n⏳ Waiting 2 seconds for sale to be processed...');
    await new Promise(resolve => setTimeout(resolve, 2000));
    
    // Create an invoice from the sale
    await createInvoiceFromSale(sale.id);
    
    // Wait a bit more for invoice to be created
    console.log('\n⏳ Waiting 2 seconds for invoice to be created...');
    await new Promise(resolve => setTimeout(resolve, 2000));
  }
  
  // Fetch invoices/receipts
  await fetchInvoices();
  
  console.log('\n✨ Process complete!');
}

// Run the script
main().catch(console.error);

