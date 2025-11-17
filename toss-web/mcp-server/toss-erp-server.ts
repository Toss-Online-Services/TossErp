#!/usr/bin/env node

/**
 * TOSS ERP MCP Server
 * Provides AI-powered business operations tools for South African SMMEs
 */

import { Server } from '@modelcontextprotocol/sdk/server/index.js';
import { StdioServerTransport } from '@modelcontextprotocol/sdk/server/stdio.js';
import {
  CallToolRequestSchema,
  ListToolsRequestSchema,
  CallToolRequest,
  ListToolsRequest,
  Tool,
  TextContent,
} from '@modelcontextprotocol/sdk/types.js';

// TOSS ERP Business Data Types
interface Customer {
  id: string;
  name: string;
  email: string;
  phone: string;
  address: string;
  balance: number;
  creditLimit: number;
  township: string;
}

interface Product {
  id: string;
  name: string;
  price: number;
  stock: number;
  category: string;
  supplier: string;
  reorderLevel: number;
}

interface SalesOrder {
  id: string;
  customerName: string;
  date: string;
  items: Array<{
    productName: string;
    quantity: number;
    price: number;
    total: number;
  }>;
  total: number;
  status: string;
  paymentMethod: string;
}

interface BusinessInsight {
  type: 'sales' | 'inventory' | 'customer' | 'financial';
  title: string;
  description: string;
  metric: number;
  trend: 'up' | 'down' | 'stable';
  recommendation: string;
}

// Mock data for demonstration
const customers: Customer[] = [
  {
    id: '1',
    name: 'Mthunzi\'s Spaza Shop',
    email: 'mthunzi@spaza.co.za',
    phone: '+27 71 123 4567',
    address: 'Main Road, Khayelitsha, Cape Town',
    balance: 1250.00,
    creditLimit: 5000.00,
    township: 'Khayelitsha'
  },
  {
    id: '2',
    name: 'Nomsa\'s Chisa Nyama',
    email: 'nomsa@chisanyama.co.za',
    phone: '+27 82 987 6543',
    address: 'Church Street, Soweto, Johannesburg',
    balance: 750.00,
    creditLimit: 3000.00,
    township: 'Soweto'
  },
  {
    id: '3',
    name: 'Bheki\'s Hardware',
    email: 'bheki@hardware.co.za',
    phone: '+27 76 555 7890',
    address: 'Industrial Road, Umlazi, Durban',
    balance: 2100.00,
    creditLimit: 8000.00,
    township: 'Umlazi'
  }
];

const products: Product[] = [
  {
    id: '1',
    name: 'White Bread Loaf',
    price: 14.50,
    stock: 45,
    category: 'Bakery',
    supplier: 'Albany Bakeries',
    reorderLevel: 10
  },
  {
    id: '2',
    name: 'Maize Meal (2.5kg)',
    price: 32.99,
    stock: 28,
    category: 'Staples',
    supplier: 'White Star',
    reorderLevel: 5
  },
  {
    id: '3',
    name: 'Cooking Oil (750ml)',
    price: 28.95,
    stock: 15,
    category: 'Cooking',
    supplier: 'Sunfoil',
    reorderLevel: 8
  },
  {
    id: '4',
    name: 'Coca-Cola (2L)',
    price: 22.50,
    stock: 67,
    category: 'Beverages',
    supplier: 'Coca-Cola SA',
    reorderLevel: 20
  },
  {
    id: '5',
    name: 'Beef Steak (per kg)',
    price: 189.99,
    stock: 12,
    category: 'Meat',
    supplier: 'Local Butchery',
    reorderLevel: 5
  }
];

const salesOrders: SalesOrder[] = [
  {
    id: 'SO-001',
    customerName: 'Mthunzi\'s Spaza Shop',
    date: '2024-11-12',
    items: [
      { productName: 'White Bread Loaf', quantity: 10, price: 14.50, total: 145.00 },
      { productName: 'Maize Meal (2.5kg)', quantity: 5, price: 32.99, total: 164.95 }
    ],
    total: 309.95,
    status: 'Confirmed',
    paymentMethod: 'Credit'
  },
  {
    id: 'SO-002',
    customerName: 'Nomsa\'s Chisa Nyama',
    date: '2024-11-11',
    items: [
      { productName: 'Beef Steak (per kg)', quantity: 8, price: 189.99, total: 1519.92 },
      { productName: 'Cooking Oil (750ml)', quantity: 3, price: 28.95, total: 86.85 }
    ],
    total: 1606.77,
    status: 'Delivered',
    paymentMethod: 'Cash'
  }
];

class TossErpServer {
  private server: Server;

  constructor() {
    this.server = new Server(
      {
        name: 'toss-erp-server',
        version: '1.0.0',
      },
      {
        capabilities: {
          tools: {},
        },
      }
    );

    this.setupHandlers();
    this.setupErrorHandling();
  }

  private setupErrorHandling(): void {
    this.server.onerror = (error) => {
      console.error('[MCP Error]', error);
    };

    process.on('SIGINT', async () => {
      await this.server.close();
      process.exit(0);
    });
  }

  private setupHandlers(): void {
    this.server.setRequestHandler(ListToolsRequestSchema, async () => {
      return {
        tools: [
          {
            name: 'get_customer_info',
            description: 'Get detailed information about a customer including balance, credit limit, and location',
            inputSchema: {
              type: 'object',
              properties: {
                customerName: {
                  type: 'string',
                  description: 'The name of the customer to search for',
                },
              },
              required: ['customerName'],
            },
          },
          {
            name: 'get_product_info',
            description: 'Get product information including price, stock levels, and supplier details',
            inputSchema: {
              type: 'object',
              properties: {
                productName: {
                  type: 'string',
                  description: 'The name of the product to search for',
                },
              },
              required: ['productName'],
            },
          },
          {
            name: 'get_low_stock_alerts',
            description: 'Get alerts for products that are running low on stock and need reordering',
            inputSchema: {
              type: 'object',
              properties: {},
            },
          },
          {
            name: 'create_sales_order',
            description: 'Create a new sales order for a customer',
            inputSchema: {
              type: 'object',
              properties: {
                customerName: {
                  type: 'string',
                  description: 'The customer name for the order',
                },
                items: {
                  type: 'array',
                  items: {
                    type: 'object',
                    properties: {
                      productName: { type: 'string' },
                      quantity: { type: 'number' },
                    },
                    required: ['productName', 'quantity'],
                  },
                  description: 'List of items to include in the order',
                },
                paymentMethod: {
                  type: 'string',
                  enum: ['Cash', 'Credit', 'Mobile Money', 'Bank Transfer'],
                  description: 'Payment method for the order',
                },
              },
              required: ['customerName', 'items', 'paymentMethod'],
            },
          },
          {
            name: 'get_business_insights',
            description: 'Get AI-powered business insights and recommendations based on sales data and trends',
            inputSchema: {
              type: 'object',
              properties: {
                insightType: {
                  type: 'string',
                  enum: ['sales', 'inventory', 'customer', 'financial', 'all'],
                  description: 'Type of business insight to generate',
                },
              },
              required: ['insightType'],
            },
          },
          {
            name: 'get_township_suppliers',
            description: 'Find suppliers and group buying opportunities in specific townships',
            inputSchema: {
              type: 'object',
              properties: {
                township: {
                  type: 'string',
                  description: 'The township name to search for suppliers',
                },
                productCategory: {
                  type: 'string',
                  description: 'Product category to find suppliers for',
                },
              },
              required: ['township'],
            },
          },
          {
            name: 'calculate_delivery_cost',
            description: 'Calculate delivery costs for orders within township networks',
            inputSchema: {
              type: 'object',
              properties: {
                fromTownship: {
                  type: 'string',
                  description: 'Source township or supplier location',
                },
                toTownship: {
                  type: 'string',
                  description: 'Destination township',
                },
                weight: {
                  type: 'number',
                  description: 'Order weight in kilograms',
                },
                orderValue: {
                  type: 'number',
                  description: 'Total order value in ZAR',
                },
              },
              required: ['fromTownship', 'toTownship', 'weight', 'orderValue'],
            },
          },
        ],
      };
    });

    this.server.setRequestHandler(CallToolRequestSchema, async (request: CallToolRequest) => {
      try {
        const { name, arguments: args } = request.params;

        if (!args) {
          throw new Error('No arguments provided');
        }

        switch (name) {
          case 'get_customer_info':
            return await this.getCustomerInfo(args.customerName as string);
          case 'get_product_info':
            return await this.getProductInfo(args.productName as string);
          case 'get_low_stock_alerts':
            return await this.getLowStockAlerts();
          case 'create_sales_order':
            return await this.createSalesOrder(
              args.customerName as string,
              args.items as any[],
              args.paymentMethod as string
            );
          case 'get_business_insights':
            return await this.getBusinessInsights(args.insightType as string);
          case 'get_township_suppliers':
            return await this.getTownshipSuppliers(
              args.township as string,
              args.productCategory as string
            );
          case 'calculate_delivery_cost':
            return await this.calculateDeliveryCost(
              args.fromTownship as string,
              args.toTownship as string,
              args.weight as number,
              args.orderValue as number
            );
          default:
            throw new Error(`Unknown tool: ${name}`);
        }
      } catch (error) {
        return {
          content: [
            {
              type: 'text',
              text: `Error: ${error instanceof Error ? error.message : String(error)}`,
            },
          ],
          isError: true,
        };
      }
    });
  }

  private async getCustomerInfo(customerName: string) {
    const customer = customers.find(c => 
      c.name.toLowerCase().includes(customerName.toLowerCase())
    );

    if (!customer) {
      return {
        content: [
          {
            type: 'text',
            text: `Customer "${customerName}" not found. Available customers: ${customers.map(c => c.name).join(', ')}`,
          },
        ],
      };
    }

    const availableCredit = customer.creditLimit - customer.balance;
    const creditUtilization = (customer.balance / customer.creditLimit) * 100;

    return {
      content: [
        {
          type: 'text',
          text: `**Customer Information: ${customer.name}**
          
📍 **Location:** ${customer.address}
🏘️ **Township:** ${customer.township}
📞 **Contact:** ${customer.phone}
📧 **Email:** ${customer.email}

💰 **Financial Status:**
- Current Balance: R${customer.balance.toFixed(2)}
- Credit Limit: R${customer.creditLimit.toFixed(2)}
- Available Credit: R${availableCredit.toFixed(2)}
- Credit Utilization: ${creditUtilization.toFixed(1)}%

⚠️ **Credit Status:** ${creditUtilization > 80 ? 'HIGH RISK' : creditUtilization > 60 ? 'MODERATE RISK' : 'GOOD STANDING'}

💡 **Recommendations:**
${creditUtilization > 80 ? '- Consider payment collection before extending more credit\n- Discuss payment plan options' :
  creditUtilization > 60 ? '- Monitor closely and encourage payments\n- Consider shorter payment terms' :
  '- Customer in good standing\n- Safe to extend normal credit terms'}`,
        },
      ],
    };
  }

  private async getProductInfo(productName: string) {
    const product = products.find(p => 
      p.name.toLowerCase().includes(productName.toLowerCase())
    );

    if (!product) {
      return {
        content: [
          {
            type: 'text',
            text: `Product "${productName}" not found. Available products: ${products.map(p => p.name).join(', ')}`,
          },
        ],
      };
    }

    const stockStatus = product.stock <= product.reorderLevel ? 'LOW STOCK' : 
                       product.stock <= product.reorderLevel * 2 ? 'MODERATE' : 'GOOD';
    const stockValue = product.stock * product.price;
    const daysOfStock = Math.floor(product.stock / (product.reorderLevel / 7)); // Assuming reorder level is weekly

    return {
      content: [
        {
          type: 'text',
          text: `**Product Information: ${product.name}**

💰 **Pricing:**
- Unit Price: R${product.price.toFixed(2)}
- Category: ${product.category}

📦 **Stock Information:**
- Current Stock: ${product.stock} units
- Stock Value: R${stockValue.toFixed(2)}
- Reorder Level: ${product.reorderLevel} units
- Stock Status: ${stockStatus}
- Estimated Days of Stock: ${daysOfStock} days

🏭 **Supplier Information:**
- Supplier: ${product.supplier}

⚠️ **Alerts:**
${product.stock <= product.reorderLevel ? 
  `🔴 URGENT: Stock is at or below reorder level! Order ${product.reorderLevel * 3} units soon.` :
  product.stock <= product.reorderLevel * 2 ?
  `🟡 WARNING: Stock is running low. Consider reordering ${product.reorderLevel * 2} units.` :
  `🟢 Stock levels are healthy.`}

💡 **Recommendations:**
- Order Quantity: ${product.reorderLevel * 3} units
- Estimated Reorder Cost: R${(product.reorderLevel * 3 * product.price * 0.8).toFixed(2)} (assuming 20% wholesale discount)`,
        },
      ],
    };
  }

  private async getLowStockAlerts() {
    const lowStockProducts = products.filter(p => p.stock <= p.reorderLevel);
    const moderateStockProducts = products.filter(p => 
      p.stock > p.reorderLevel && p.stock <= p.reorderLevel * 2
    );

    const totalLowStockValue = lowStockProducts.reduce((sum, p) => sum + (p.stock * p.price), 0);
    const totalReorderCost = lowStockProducts.reduce((sum, p) => sum + (p.reorderLevel * 3 * p.price * 0.8), 0);

    return {
      content: [
        {
          type: 'text',
          text: `**📦 Stock Level Alert Report**

🔴 **URGENT - Low Stock (${lowStockProducts.length} items):**
${lowStockProducts.length === 0 ? 'No items urgently require reordering.' :
  lowStockProducts.map(p => 
    `- ${p.name}: ${p.stock} units (reorder at ${p.reorderLevel}) - Order ${p.reorderLevel * 3} units`
  ).join('\n')}

🟡 **WARNING - Moderate Stock (${moderateStockProducts.length} items):**
${moderateStockProducts.length === 0 ? 'No items in warning range.' :
  moderateStockProducts.map(p => 
    `- ${p.name}: ${p.stock} units (will need reordering soon)`
  ).join('\n')}

💰 **Financial Impact:**
- Current Low Stock Value: R${totalLowStockValue.toFixed(2)}
- Estimated Reorder Cost: R${totalReorderCost.toFixed(2)}

📋 **Next Actions:**
${lowStockProducts.length > 0 ? 
  '1. Contact suppliers for urgent restocking\n2. Consider group buying to reduce costs\n3. Update pricing if supply costs have changed' :
  '1. Monitor moderate stock items\n2. Plan ahead for bulk purchasing opportunities'}

💡 **Group Buying Opportunity:**
${lowStockProducts.length >= 2 ? 
  'Multiple items need restocking - perfect opportunity to organize group buying with other township businesses to reduce costs!' :
  'Consider coordinating with neighboring businesses for future bulk orders.'}`,
        },
      ],
    };
  }

  private async createSalesOrder(customerName: string, items: any[], paymentMethod: string) {
    const customer = customers.find(c => 
      c.name.toLowerCase().includes(customerName.toLowerCase())
    );

    if (!customer) {
      return {
        content: [
          {
            type: 'text',
            text: `Customer "${customerName}" not found. Please check the name and try again.`,
          },
        ],
      };
    }

    let orderTotal = 0;
    const orderItems = [];
    const unavailableItems = [];

    for (const item of items) {
      const product = products.find(p => 
        p.name.toLowerCase().includes(item.productName.toLowerCase())
      );
      
      if (!product) {
        unavailableItems.push(item.productName);
        continue;
      }

      if (product.stock < item.quantity) {
        unavailableItems.push(`${product.name} (only ${product.stock} available, ${item.quantity} requested)`);
        continue;
      }

      const lineTotal = product.price * item.quantity;
      orderItems.push({
        productName: product.name,
        quantity: item.quantity,
        price: product.price,
        total: lineTotal
      });
      orderTotal += lineTotal;
    }

    if (unavailableItems.length > 0) {
      return {
        content: [
          {
            type: 'text',
            text: `**⚠️ Cannot create order - Product availability issues:**
            
${unavailableItems.map(item => `- ${item}`).join('\n')}

Please check stock levels and adjust quantities, or contact suppliers for restocking.`,
          },
        ],
      };
    }

    // Check credit limit for credit payments
    if (paymentMethod === 'Credit') {
      const newBalance = customer.balance + orderTotal;
      if (newBalance > customer.creditLimit) {
        const availableCredit = customer.creditLimit - customer.balance;
        return {
          content: [
            {
              type: 'text',
              text: `**❌ Credit Limit Exceeded**
              
Order Total: R${orderTotal.toFixed(2)}
Customer Balance: R${customer.balance.toFixed(2)}
Available Credit: R${availableCredit.toFixed(2)}
Would Exceed Limit by: R${(newBalance - customer.creditLimit).toFixed(2)}

**Options:**
1. Reduce order amount to R${availableCredit.toFixed(2)}
2. Request partial payment upfront
3. Change payment method to Cash or Mobile Money`,
            },
          ],
        };
      }
    }

    // Generate order number
    const orderNumber = `SO-${String(salesOrders.length + 1).padStart(3, '0')}`;
    const newOrder: SalesOrder = {
      id: orderNumber,
      customerName: customer.name,
      date: new Date().toISOString().split('T')[0],
      items: orderItems,
      total: orderTotal,
      status: paymentMethod === 'Cash' ? 'Confirmed' : 'Pending Payment',
      paymentMethod
    };

    // Add to orders (in real implementation, this would save to database)
    salesOrders.push(newOrder);

    // Update stock levels (in real implementation, this would update database)
    orderItems.forEach(orderItem => {
      const product = products.find(p => p.name === orderItem.productName);
      if (product) {
        product.stock -= orderItem.quantity;
      }
    });

    // Update customer balance for credit sales
    if (paymentMethod === 'Credit') {
      customer.balance += orderTotal;
    }

    return {
      content: [
        {
          type: 'text',
          text: `**✅ Sales Order Created Successfully!**

**Order Details:**
- Order Number: ${orderNumber}
- Customer: ${customer.name}
- Date: ${newOrder.date}
- Payment Method: ${paymentMethod}

**Order Items:**
${orderItems.map(item => 
  `- ${item.productName}: ${item.quantity} × R${item.price.toFixed(2)} = R${item.total.toFixed(2)}`
).join('\n')}

**💰 Order Total: R${orderTotal.toFixed(2)}**

**📦 Stock Impact:**
${orderItems.map(item => {
  const product = products.find(p => p.name === item.productName);
  return `- ${item.productName}: ${product!.stock + item.quantity} → ${product!.stock} units remaining`;
}).join('\n')}

${paymentMethod === 'Credit' ? 
  `**💳 Credit Update:**
- Previous Balance: R${(customer.balance - orderTotal).toFixed(2)}
- New Balance: R${customer.balance.toFixed(2)}
- Available Credit: R${(customer.creditLimit - customer.balance).toFixed(2)}` : ''}

**📲 Next Steps:**
1. ${paymentMethod === 'Cash' ? 'Process payment and confirm order' : 'Await payment confirmation'}
2. Prepare items for ${customer.township} delivery
3. Update delivery schedule
4. ${orderItems.some(item => products.find(p => p.name === item.productName)!.stock <= products.find(p => p.name === item.productName)!.reorderLevel) ? 'Consider restocking low inventory items' : 'Monitor stock levels'}`,
        },
      ],
    };
  }

  private async getBusinessInsights(insightType: string) {
    const insights: BusinessInsight[] = [];

    if (insightType === 'sales' || insightType === 'all') {
      const totalSales = salesOrders.reduce((sum, order) => sum + order.total, 0);
      const avgOrderValue = totalSales / salesOrders.length;
      
      insights.push({
        type: 'sales',
        title: 'Sales Performance',
        description: `Total sales: R${totalSales.toFixed(2)} across ${salesOrders.length} orders`,
        metric: avgOrderValue,
        trend: 'up',
        recommendation: 'Focus on upselling complementary products to increase average order value'
      });
    }

    if (insightType === 'inventory' || insightType === 'all') {
      const lowStockCount = products.filter(p => p.stock <= p.reorderLevel).length;
      const totalInventoryValue = products.reduce((sum, p) => sum + (p.stock * p.price), 0);
      
      insights.push({
        type: 'inventory',
        title: 'Inventory Health',
        description: `${lowStockCount} items need restocking. Total inventory value: R${totalInventoryValue.toFixed(2)}`,
        metric: lowStockCount,
        trend: lowStockCount > 3 ? 'down' : 'stable',
        recommendation: lowStockCount > 0 ? 'Implement automated reordering for fast-moving items' : 'Consider reducing stock levels for slow-moving items'
      });
    }

    if (insightType === 'customer' || insightType === 'all') {
      const totalCreditUtilization = customers.reduce((sum, c) => sum + ((c.balance / c.creditLimit) * 100), 0) / customers.length;
      const highRiskCustomers = customers.filter(c => (c.balance / c.creditLimit) > 0.8).length;
      
      insights.push({
        type: 'customer',
        title: 'Customer Credit Health',
        description: `Average credit utilization: ${totalCreditUtilization.toFixed(1)}%. ${highRiskCustomers} high-risk customers`,
        metric: totalCreditUtilization,
        trend: totalCreditUtilization > 70 ? 'down' : 'stable',
        recommendation: 'Implement payment reminders and consider credit insurance for high-risk accounts'
      });
    }

    if (insightType === 'financial' || insightType === 'all') {
      const totalReceivables = customers.reduce((sum, c) => sum + c.balance, 0);
      const cashSales = salesOrders.filter(o => o.paymentMethod === 'Cash').reduce((sum, o) => sum + o.total, 0);
      const creditSales = salesOrders.filter(o => o.paymentMethod === 'Credit').reduce((sum, o) => sum + o.total, 0);
      
      insights.push({
        type: 'financial',
        title: 'Financial Overview',
        description: `Outstanding receivables: R${totalReceivables.toFixed(2)}. Cash vs Credit ratio: ${(cashSales / (cashSales + creditSales) * 100).toFixed(1)}% cash`,
        metric: totalReceivables,
        trend: 'stable',
        recommendation: 'Consider offering cash discounts to improve cash flow and reduce credit risk'
      });
    }

    return {
      content: [
        {
          type: 'text',
          text: `**🔍 Business Insights Report**

${insights.map(insight => `
**${insight.title}** (${insight.type.toUpperCase()})
${insight.description}
📊 Metric: ${insight.type === 'financial' || insight.type === 'sales' ? `R${insight.metric.toFixed(2)}` : insight.metric}
📈 Trend: ${insight.trend === 'up' ? '📈 Improving' : insight.trend === 'down' ? '📉 Needs Attention' : '➡️ Stable'}

💡 **Recommendation:** ${insight.recommendation}
`).join('\n---\n')}

**� Key Action Items:**
1. ${insights.find(i => i.type === 'inventory')?.metric ?? 0 > 2 ? 'Urgent: Contact suppliers for restocking' : 'Monitor inventory levels weekly'}
2. ${insights.find(i => i.type === 'customer')?.metric ?? 0 > 70 ? 'Implement stricter credit policies' : 'Continue current credit management'}
3. ${insights.find(i => i.type === 'sales')?.trend === 'up' ? 'Capitalize on growth with marketing campaigns' : 'Focus on customer retention strategies'}

**📱 TOSS ERP Recommendations:**
- Set up automated low-stock alerts
- Enable mobile payments to reduce cash handling
- Join group buying networks in ${customers[0].township} for better supplier prices
- Use AI sales forecasting for better inventory planning`,
        },
      ],
    };
  }

  private async getTownshipSuppliers(township: string, productCategory?: string) {
    // Mock supplier data for townships
    const supplierData = {
      'Khayelitsha': [
        { name: 'Khayelitsha Fresh Produce Co-op', category: 'Fresh Produce', distance: '2km', rating: 4.5, groupBuying: true },
        { name: 'Township Wholesale Distributors', category: 'General Goods', distance: '5km', rating: 4.2, groupBuying: true },
        { name: 'Mama Afrika Bakery Supply', category: 'Bakery', distance: '3km', rating: 4.7, groupBuying: false }
      ],
      'Soweto': [
        { name: 'Soweto Wholesale Hub', category: 'General Goods', distance: '1km', rating: 4.6, groupBuying: true },
        { name: 'Meat Traders Collective', category: 'Meat', distance: '4km', rating: 4.3, groupBuying: true },
        { name: 'Beverages & More SA', category: 'Beverages', distance: '6km', rating: 4.1, groupBuying: false }
      ],
      'Umlazi': [
        { name: 'Umlazi Business Co-operative', category: 'General Goods', distance: '2km', rating: 4.4, groupBuying: true },
        { name: 'KZN Hardware Suppliers', category: 'Hardware', distance: '7km', rating: 4.5, groupBuying: true },
        { name: 'Durban Spice & Staples', category: 'Staples', distance: '3km', rating: 4.8, groupBuying: false }
      ]
    };

    const suppliers = supplierData[township as keyof typeof supplierData] || [];
    const filteredSuppliers = productCategory ? 
      suppliers.filter(s => s.category.toLowerCase().includes(productCategory.toLowerCase())) : 
      suppliers;

    if (filteredSuppliers.length === 0) {
      return {
        content: [
          {
            type: 'text',
            text: `**No suppliers found for ${productCategory ? `"${productCategory}" in ` : ''}${township}**

We don't have supplier information for this area${productCategory ? ' and category' : ''}. 

**💡 Suggestions:**
1. Check neighboring townships: ${Object.keys(supplierData).filter(t => t !== township).join(', ')}
2. Contact TOSS support to add suppliers in your area
3. Start a supplier co-operative with other ${township} businesses

**📞 Contact TOSS Support:** help@toss-erp.co.za`,
          },
        ],
      };
    }

    const groupBuyingSuppliers = filteredSuppliers.filter(s => s.groupBuying);

    return {
      content: [
        {
          type: 'text',
          text: `**🏭 Suppliers in ${township}${productCategory ? ` - ${productCategory}` : ''}**

${filteredSuppliers.map(supplier => `
**${supplier.name}**
📂 Category: ${supplier.category}
📍 Distance: ${supplier.distance}
⭐ Rating: ${supplier.rating}/5.0
🤝 Group Buying: ${supplier.groupBuying ? '✅ Available' : '❌ Not available'}
`).join('')}

**🤝 Group Buying Opportunities:**
${groupBuyingSuppliers.length === 0 ? 
  'No group buying available for filtered suppliers.' :
  `${groupBuyingSuppliers.length} supplier(s) offer group buying discounts:
${groupBuyingSuppliers.map(s => `- ${s.name} (${s.category})`).join('\n')}`}

**💰 Potential Savings:**
- Individual Purchase Markup: 15-25%
- Group Buying Discount: 10-20%
- **Net Savings: 5-15% on group orders**

**📋 Next Steps:**
1. Contact suppliers for current pricing
2. Coordinate with other ${township} businesses
3. Set up recurring group orders
4. Join the TOSS Township Network for automatic group buying opportunities

**📱 TOSS Features:**
- Automated group buying matching
- Shared delivery cost calculation
- Township business network
- Mobile payment integration`,
        },
      ],
    };
  }

  private async calculateDeliveryCost(fromTownship: string, toTownship: string, weight: number, orderValue: number) {
    // Distance matrix for common South African townships
    const distanceMatrix: { [key: string]: { [key: string]: number } } = {
      'Khayelitsha': { 'Khayelitsha': 0, 'Soweto': 1250, 'Umlazi': 1450, 'Mitchells Plain': 15, 'Gugulethu': 12 },
      'Soweto': { 'Khayelitsha': 1250, 'Soweto': 0, 'Umlazi': 550, 'Alexandra': 25, 'Orange Farm': 45 },
      'Umlazi': { 'Khayelitsha': 1450, 'Soweto': 550, 'Umlazi': 0, 'Chatsworth': 20, 'Phoenix': 35 },
      'Mitchells Plain': { 'Khayelitsha': 15, 'Mitchells Plain': 0, 'Athlone': 18 },
      'Gugulethu': { 'Khayelitsha': 12, 'Gugulethu': 0, 'Langa': 8 }
    };

    const distance = distanceMatrix[fromTownship]?.[toTownship] ?? 
                    distanceMatrix[toTownship]?.[fromTownship] ?? null;

    if (distance === null) {
      return {
        content: [
          {
            type: 'text',
            text: `**❌ Route Not Found**
            
Unable to calculate delivery cost from ${fromTownship} to ${toTownship}.

**Available Routes:**
${Object.keys(distanceMatrix).map(from => 
  `${from}: ${Object.keys(distanceMatrix[from]).filter(to => to !== from).join(', ')}`
).join('\n')}

**💡 Custom Route Request:**
Contact TOSS support to add this route to our delivery network.
📞 Email: logistics@toss-erp.co.za`,
          },
        ],
      };
    }

    // Delivery cost calculation
    const baseCost = distance <= 50 ? 25 : distance <= 200 ? 45 : distance <= 500 ? 85 : 150;
    const weightCost = weight * 2.5; // R2.50 per kg
    const valueSurcharge = orderValue > 5000 ? orderValue * 0.005 : 0; // 0.5% for high-value orders
    
    const totalCost = baseCost + weightCost + valueSurcharge;
    
    // Discounts
    const groupDeliveryDiscount = orderValue > 2000 ? totalCost * 0.2 : 0; // 20% for group orders
    const sameLocationDiscount = distance === 0 ? totalCost * 0.5 : 0; // 50% for same township
    const bulkDiscount = weight > 50 ? totalCost * 0.15 : 0; // 15% for bulk orders
    
    const finalCost = totalCost - Math.max(groupDeliveryDiscount, sameLocationDiscount, bulkDiscount);
    
    // Delivery time estimation
    const estimatedHours = distance <= 50 ? '2-4 hours' : 
                          distance <= 200 ? '4-8 hours' : 
                          distance <= 500 ? '1-2 days' : '2-3 days';

    return {
      content: [
        {
          type: 'text',
          text: `**🚛 Delivery Cost Calculation**

**Route:** ${fromTownship} → ${toTownship}
**Distance:** ${distance}km
**Order Weight:** ${weight}kg
**Order Value:** R${orderValue.toFixed(2)}

**💰 Cost Breakdown:**
- Base Delivery Cost: R${baseCost.toFixed(2)}
- Weight Charge (R2.50/kg): R${weightCost.toFixed(2)}
- Value Surcharge (0.5% for >R5,000): R${valueSurcharge.toFixed(2)}
- **Subtotal:** R${totalCost.toFixed(2)}

**💸 Discounts Applied:**
${groupDeliveryDiscount > 0 ? `- Group Order Discount (20%): -R${groupDeliveryDiscount.toFixed(2)}` : ''}
${sameLocationDiscount > 0 ? `- Same Township Delivery (50%): -R${sameLocationDiscount.toFixed(2)}` : ''}
${bulkDiscount > 0 ? `- Bulk Order Discount (15%): -R${bulkDiscount.toFixed(2)}` : ''}
${Math.max(groupDeliveryDiscount, sameLocationDiscount, bulkDiscount) === 0 ? '- No discounts applicable' : ''}

**🎯 Final Delivery Cost: R${finalCost.toFixed(2)}**
**📅 Estimated Delivery Time:** ${estimatedHours}

**💡 Cost Optimization Tips:**
${orderValue < 2000 ? '- Add R' + (2000 - orderValue).toFixed(2) + ' more for group buying discount (20% off delivery)' : ''}
${weight < 50 ? '- Combine with other orders for bulk discount (15% off at 50kg+)' : ''}
${distance > 0 ? '- Consider local township suppliers to reduce delivery costs' : ''}

**🤝 Shared Delivery Options:**
- Join existing delivery runs in your area
- Coordinate with nearby businesses
- Use TOSS Township Delivery Network
- Split costs with neighboring shops

**📱 Book Delivery:**
Use TOSS mobile app to schedule delivery and track in real-time.`,
        },
      ],
    };
  }

  async run(): Promise<void> {
    const transport = new StdioServerTransport();
    await this.server.connect(transport);
    console.error('TOSS ERP MCP Server running on stdio');
  }
}

// Run the server
const server = new TossErpServer();
server.run().catch(console.error);