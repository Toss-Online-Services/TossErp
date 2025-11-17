# 🚀 TOSS ERP Sales Modules - Quick Start Guide

**Last Updated:** January 10, 2025  
**For:** Developers continuing the sales modules implementation

---

## ⚡ 60-Second Overview

You're working on **TOSS ERP III** - an ERP platform for South African township SMMEs (spaza shops, chisa nyamas, etc.). The sales modules are based on **ERPNext patterns** with TOSS-specific adaptations for township businesses.

**Current Status:** 15% complete (infrastructure done, quotations 75%, delivery notes 50%)

---

## 📁 What You Have

### ✅ Fully Configured
- **28 Nuxt modules** installed and configured
- **5 language support** (EN, ZU, XH, AF, ST) - i18n ready
- **FormKit** for forms with Tailwind theme
- **Icons** (@nuxt/icon - 200k+ icons available)
- **PWA** (@vite-pwa/nuxt for offline)
- **Security** headers and rate limiting
- **SEO** modules (sitemap, robots, schema.org)

### ✅ Pages Built
1. `/sales/quotations` - List all quotations (search, filter, stats)
2. `/sales/quotations/create` - Create new quotation (full form with line items)
3. `/sales/quotations/[id]` - View quotation details (professional layout)
4. `/sales/delivery-notes` - List all deliveries (track status, drivers)

### 📝 Documentation
- `NUXT_MODULES_GUIDE.md` - How to use all 28 modules (350 lines)
- `SALES_MODULES_IMPLEMENTATION.md` - Complete roadmap (400 lines)
- `SALES_MODULES_TODO.md` - Detailed tasks (600 lines)
- `SESSION_SUMMARY.md` - What was done (this session)

---

## 🏃 Getting Started (5 Minutes)

### 1. Install Dependencies
```bash
cd toss-web
pnpm install
```

### 2. Environment Variables
Copy `.env.example` to `.env` and fill in:
```env
# Optional - for development you can skip these
NUXT_PUBLIC_SITE_URL=http://localhost:3000
NUXT_PUBLIC_GTAG_ID=
NUXT_PUBLIC_FORMKIT_PRO_KEY=
```

### 3. Start Dev Server
```bash
pnpm dev
```

Visit: `http://localhost:3000/sales/quotations`

### 4. Expected Warnings (Ignore These)
- TypeScript errors in Vue files → Will resolve when dev server starts
- Peer dependency warnings for @pinia-plugin-persistedstate and @vitejs/plugin-vue → Non-critical
- @apply CSS warnings → IDE-only, works fine in browser

---

## 🎯 Your First Task: Complete Quotations Module

**Time Estimate:** 2-3 hours  
**Priority:** HIGH

### What's Missing:
1. **Edit Page** (`/sales/quotations/[id]/edit.vue`)
2. **PDF Generation** (Download quotation as PDF)
3. **Email Sending** (Send quotation to customer)
4. **Convert to Order** (Create sales order from quotation)

### Step-by-Step:

#### Task 1: Create Edit Page (45 min)
```bash
# Copy create page as starting point
cp pages/sales/quotations/create.vue pages/sales/quotations/[id]/edit.vue
```

**Changes needed:**
1. Fetch quotation data in `onMounted`:
   ```typescript
   const quotation = ref(null)
   
   onMounted(async () => {
     const id = route.params.id
     // API call: const { data } = await useFetch(`/api/quotations/${id}`)
     quotation.value = mockQuotation // For now, use mock data
     
     // Populate form
     formData.value = { ...quotation.value }
     lineItems.value = quotation.value.items
   })
   ```

2. Change submit button text to "Update Quotation"
3. Change API call from POST to PUT
4. Navigate back to detail page after save

**Test:** Can edit existing quotation and see changes

---

#### Task 2: PDF Generation (60 min)

**Install jsPDF:**
```bash
pnpm add jspdf jspdf-autotable
```

**Create PDF utility** (`/utils/quotationPDF.ts`):
```typescript
import jsPDF from 'jspdf'
import 'jspdf-autotable'

export function generateQuotationPDF(quotation: any) {
  const doc = new jsPDF()
  
  // Add company logo
  // doc.addImage(logoBase64, 'PNG', 15, 10, 40, 15)
  
  // Company details
  doc.setFontSize(10)
  doc.text('TOSS Online Services', 15, 35)
  doc.text('123 Township Street', 15, 40)
  doc.text('Johannesburg, Gauteng 2000', 15, 45)
  
  // Title
  doc.setFontSize(20)
  doc.text('QUOTATION', 105, 35, { align: 'center' })
  
  // Quotation details
  doc.setFontSize(10)
  doc.text(`Quotation #: ${quotation.quotationNo}`, 120, 50)
  doc.text(`Date: ${formatDate(quotation.quotationDate)}`, 120, 55)
  doc.text(`Valid Until: ${formatDate(quotation.validUntil)}`, 120, 60)
  
  // Customer details
  doc.text('Bill To:', 15, 60)
  doc.text(quotation.customerName, 15, 65)
  doc.text(quotation.customer.address, 15, 70)
  doc.text(quotation.customer.phone, 15, 75)
  
  // Line items table
  const tableData = quotation.items.map((item, index) => [
    index + 1,
    item.productName,
    item.quantity,
    `R${item.rate.toFixed(2)}`,
    `${item.discountPercent}%`,
    `R${item.amount.toFixed(2)}`
  ])
  
  doc.autoTable({
    startY: 90,
    head: [['#', 'Product', 'Qty', 'Rate', 'Disc', 'Amount']],
    body: tableData,
    foot: [
      ['', '', '', '', 'Subtotal:', `R${quotation.subtotal.toFixed(2)}`],
      ['', '', '', '', 'VAT (15%):', `R${quotation.taxAmount.toFixed(2)}`],
      ['', '', '', '', 'Total:', `R${quotation.grandTotal.toFixed(2)}`]
    ],
    theme: 'grid'
  })
  
  // Terms and conditions
  const finalY = doc.lastAutoTable.finalY + 10
  doc.text('Terms and Conditions:', 15, finalY)
  doc.setFontSize(9)
  const terms = doc.splitTextToSize(quotation.termsAndConditions || '', 180)
  doc.text(terms, 15, finalY + 5)
  
  return doc
}
```

**Use in detail page:**
```typescript
const downloadPDF = () => {
  const doc = generateQuotationPDF(quotation.value)
  doc.save(`quotation-${quotation.value.quotationNo}.pdf`)
}
```

**Test:** Click "View PDF" button, download should start

---

#### Task 3: Email Sending (30 min)

**In detail page** (`/sales/quotations/[id].vue`):
```typescript
const sendEmail = async () => {
  try {
    // Generate PDF
    const doc = generateQuotationPDF(quotation.value)
    const pdfBlob = doc.output('blob')
    
    // Create FormData
    const formData = new FormData()
    formData.append('pdf', pdfBlob, `quotation-${quotation.value.quotationNo}.pdf`)
    formData.append('to', quotation.value.customer.email)
    formData.append('subject', `Quotation ${quotation.value.quotationNo}`)
    formData.append('message', `Please find attached quotation ${quotation.value.quotationNo}`)
    
    // Send via API
    await $fetch('/api/quotations/send-email', {
      method: 'POST',
      body: formData
    })
    
    // Update status
    quotation.value.status = 'sent'
    alert(t('sales.quotations.emailSent'))
  } catch (error) {
    alert(t('sales.quotations.errorSending'))
  }
}
```

**Backend needed** (create `/server/api/quotations/send-email.post.ts`):
```typescript
import nodemailer from 'nodemailer'

export default defineEventHandler(async (event) => {
  const formData = await readMultipartFormData(event)
  
  // Extract data
  const to = formData.find(f => f.name === 'to')?.data.toString()
  const subject = formData.find(f => f.name === 'subject')?.data.toString()
  const message = formData.find(f => f.name === 'message')?.data.toString()
  const pdf = formData.find(f => f.name === 'pdf')
  
  // Configure email
  const transporter = nodemailer.createTransport({
    host: process.env.SMTP_HOST,
    port: 587,
    auth: {
      user: process.env.SMTP_USER,
      pass: process.env.SMTP_PASS
    }
  })
  
  // Send email
  await transporter.sendMail({
    from: 'sales@toss.co.za',
    to,
    subject,
    text: message,
    attachments: [{
      filename: 'quotation.pdf',
      content: pdf.data
    }]
  })
  
  return { success: true }
})
```

**Test:** Send email (will fail until SMTP configured, but logic works)

---

#### Task 4: Convert to Order (30 min)

**In detail page:**
```typescript
const convertToOrder = async () => {
  if (!confirm(t('sales.quotations.confirmConvertToOrder'))) {
    return
  }
  
  try {
    // Create order from quotation
    const order = await $fetch('/api/sales-orders', {
      method: 'POST',
      body: {
        quotationId: quotation.value.id,
        customerId: quotation.value.customer.id,
        orderDate: new Date().toISOString().split('T')[0],
        items: quotation.value.items,
        subtotal: quotation.value.subtotal,
        taxAmount: quotation.value.taxAmount,
        grandTotal: quotation.value.grandTotal,
        status: 'pending'
      }
    })
    
    // Update quotation status
    quotation.value.status = 'converted'
    
    // Navigate to new order
    navigateTo(`/sales/orders/${order.id}`)
  } catch (error) {
    alert(t('sales.quotations.errorConverting'))
  }
}
```

**Test:** Convert quotation → Check order created

---

## 📋 Next Tasks After Quotations

### 1. Complete Delivery Notes (3 hours)
- Create delivery note creation page
- Implement packing slip generation
- Build proof of delivery component (signature + photo)

**Files to create:**
- `/pages/sales/delivery-notes/create.vue`
- `/pages/sales/delivery-notes/[id].vue`
- `/components/delivery/ProofOfDelivery.vue`
- `/components/delivery/PackingSlip.vue`
- `/utils/packingSlipPDF.ts`

### 2. Enhance Sales Orders (6 hours)
- Build order creation from quotation
- Add order amendments (add/remove items after creation)
- Implement partial fulfillment tracking
- Add bulk order processing

**Files to modify/create:**
- `/pages/sales/orders/index.vue` (enhance existing)
- `/pages/sales/orders/create.vue`
- `/pages/sales/orders/[id].vue`
- `/pages/sales/orders/[id]/amend.vue`

### 3. POS Enhancements (8 hours)
- Create POS profiles management
- Build session opening/closing pages
- Integrate loyalty programs
- Add multiple payment modes

**Files to create:**
- `/pages/sales/pos/profiles/index.vue`
- `/pages/sales/pos/session-opening.vue`
- `/pages/sales/pos/session-closing.vue`
- `/pages/sales/pos/loyalty/index.vue`

---

## 🛠️ Common Patterns You'll Use

### 1. Fetching Data with Mock Fallback
```typescript
const { data: items } = await useFetch('/api/items', {
  default: () => mockItems // Falls back to mock if API fails
})
```

### 2. i18n Translation
```vue
<script setup>
const { t, locale } = useI18n()
</script>

<template>
  <h1>{{ t('sales.quotations.title') }}</h1>
  <button @click="locale = 'zu'">Switch to Zulu</button>
</template>
```

### 3. Status Badge Component Pattern
```vue
<span :class="getStatusClass(status)">
  {{ t(`sales.quotations.${status}`) }}
</span>

<script>
const getStatusClass = (status: string) => {
  const classes = {
    'draft': 'bg-gray-100 text-gray-800',
    'sent': 'bg-blue-100 text-blue-800',
    'accepted': 'bg-green-100 text-green-800',
    // ...
  }
  return classes[status] || 'bg-gray-100 text-gray-800'
}
</script>
```

### 4. Currency Formatting
```typescript
const formatCurrency = (amount: number) => {
  return amount.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',')
}

// Usage: R{{ formatCurrency(1234.56) }} → R1,234.56
```

### 5. Date Formatting (South African)
```typescript
const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('en-ZA', {
    day: 'numeric',
    month: 'long',
    year: 'numeric'
  })
}

// Usage: {{ formatDate('2025-01-10') }} → 10 January 2025
```

### 6. FormKit Form
```vue
<FormKit
  type="form"
  @submit="handleSubmit"
  :actions="false"
  v-model="formData"
>
  <FormKit
    type="text"
    name="name"
    label="Customer Name"
    validation="required|length:3"
  />
  
  <FormKit
    type="repeater"
    name="items"
    label="Line Items"
  >
    <FormKit type="select" name="product" :options="products" />
    <FormKit type="number" name="quantity" />
  </FormKit>
</FormKit>
```

### 7. Icon Usage
```vue
<!-- Using icon alias -->
<Icon name="cart" size="24" />

<!-- Using full icon name -->
<Icon name="mdi:cash-register" size="32" class="text-blue-600" />

<!-- Animated -->
<Icon name="mdi:loading" class="animate-spin" />
```

### 8. Responsive Device Detection
```vue
<script setup>
const { isMobile, isTablet, isDesktop } = useDevice()
</script>

<template>
  <div v-if="isMobile" class="mobile-layout">
    <!-- Simplified UI for mobile -->
  </div>
  
  <div v-else class="desktop-layout">
    <!-- Full featured UI -->
  </div>
</template>
```

---

## 🔌 API Integration

### Current State: Mock Data
All pages use mock data defined in `ref()`:
```typescript
const quotations = ref([
  { id: 1, quotationNo: 'QTN-001', ... },
  { id: 2, quotationNo: 'QTN-002', ... }
])
```

### Migration to Real API
Replace with `useFetch`:
```typescript
const { data: quotations, pending, error } = await useFetch('/api/quotations')
```

### API Endpoints Needed
```
# Quotations
GET    /api/quotations          - List all
POST   /api/quotations          - Create
GET    /api/quotations/:id      - Get one
PUT    /api/quotations/:id      - Update
DELETE /api/quotations/:id      - Delete
POST   /api/quotations/:id/send-email
POST   /api/quotations/:id/convert-to-order

# Sales Orders
GET    /api/sales-orders
POST   /api/sales-orders
GET    /api/sales-orders/:id
PUT    /api/sales-orders/:id

# Delivery Notes
GET    /api/delivery-notes
POST   /api/delivery-notes
GET    /api/delivery-notes/:id
PUT    /api/delivery-notes/:id
POST   /api/delivery-notes/:id/proof-of-delivery

# Customers & Products
GET    /api/customers
GET    /api/products
```

### Creating Server Endpoints
Create in `/server/api/`:
```typescript
// server/api/quotations/index.get.ts
export default defineEventHandler(async (event) => {
  // Get query params
  const query = getQuery(event)
  
  // Fetch from database
  const quotations = await db.quotations.findMany({
    where: {
      status: query.status || undefined
    }
  })
  
  return quotations
})
```

---

## 🌍 Multi-Language Support

### Adding Translations

**1. Add keys to `locales/en.json`:**
```json
{
  "sales": {
    "newFeature": {
      "title": "New Feature",
      "description": "Feature description"
    }
  }
}
```

**2. Translate to other languages:**
- `locales/zu.json` (isiZulu)
- `locales/xh.json` (isiXhosa)
- `locales/af.json` (Afrikaans)
- `locales/st.json` (Sesotho)

**3. Use in component:**
```vue
<h1>{{ t('sales.newFeature.title') }}</h1>
```

### Current Translation Status
- English (en): 100% ✅
- Afrikaans (af): 20% 🔄
- isiZulu (zu): 0% ❌
- isiXhosa (xh): 0% ❌
- Sesotho (st): 0% ❌

**Priority:** Complete translations after core features are done

---

## 🧪 Testing

### Manual Testing Checklist

**Quotations:**
- [ ] Can list all quotations
- [ ] Can search by quotation # or customer
- [ ] Can filter by status
- [ ] Can filter by date range
- [ ] Stats cards show correct counts
- [ ] Can create new quotation
- [ ] Line items calculate correctly
- [ ] Can add/remove line items
- [ ] Can save as draft
- [ ] Can save and send
- [ ] Can view quotation details
- [ ] Can edit quotation
- [ ] Can download PDF
- [ ] Can send email
- [ ] Can convert to order
- [ ] Can delete quotation

**Delivery Notes:**
- [ ] Can list all deliveries
- [ ] Can search by delivery # or order #
- [ ] Can filter by status and date
- [ ] Stats cards show correct counts
- [ ] Can create new delivery
- [ ] Can view delivery details
- [ ] Can print packing slip
- [ ] Can start delivery (status change)
- [ ] Can capture proof of delivery
- [ ] Can view proof of delivery

### Automated Testing (Future)

**Unit Tests** (Vitest):
```bash
pnpm add -D vitest @vue/test-utils
```

**E2E Tests** (Playwright):
```bash
pnpm add -D @playwright/test
```

---

## 📱 Mobile Testing

### Test On:
1. **Chrome DevTools** - Mobile device emulation
2. **Real device** - Test on actual Android phone (primary TOSS user device)
3. **Slow 3G** - Test performance on slow connection
4. **Offline** - Test PWA offline capabilities

### Key Mobile Considerations:
- Touch targets must be ≥ 44px
- Bottom navigation for easy thumb access
- Swipe gestures where appropriate
- Large, readable text (16px minimum)
- Avoid hover states (no hover on mobile)

---

## 🚨 Common Issues & Solutions

### Issue 1: TypeScript Errors in Vue Files
**Error:** `Cannot find name 'useI18n'`, `Cannot find name 'ref'`

**Solution:** These are auto-imports from Nuxt. Errors will disappear when you run `pnpm dev`. Ignore them in IDE.

---

### Issue 2: @apply Not Working
**Error:** `Unknown at rule @apply`

**Solution:** This is an IDE warning only. Tailwind's @apply works fine when running. You can suppress the warning in VS Code:
```json
// .vscode/settings.json
{
  "css.lint.unknownAtRules": "ignore"
}
```

---

### Issue 3: Icons Not Showing
**Error:** Icons appear as text "mdi:cart"

**Solution:** Make sure dev server is running. Icons are loaded dynamically. Check nuxt.config.ts has `@nuxt/icon` in modules.

---

### Issue 4: Translations Missing
**Error:** Translation keys showing as "sales.quotations.title" instead of "Quotations"

**Solution:**
1. Check key exists in `locales/en.json`
2. Check i18n module is configured in nuxt.config.ts
3. Restart dev server

---

### Issue 5: FormKit Not Styled
**Error:** FormKit inputs have no styling

**Solution:**
1. Verify `formkit.config.ts` exists
2. Verify `nuxt.config.ts` has `formkit: { configFile: './formkit.config.ts' }`
3. Check `formkit.theme.ts` has Tailwind classes
4. Restart dev server

---

## 📚 Helpful Resources

### Documentation:
- **Nuxt:** https://nuxt.com
- **FormKit:** https://formkit.com
- **Tailwind:** https://tailwindcss.com
- **Vue i18n:** https://vue-i18n.intlify.dev
- **ERPNext:** https://docs.frappe.io/erpnext

### TOSS-Specific:
- `NUXT_MODULES_GUIDE.md` - How to use each module
- `SALES_MODULES_IMPLEMENTATION.md` - Complete roadmap
- `SALES_MODULES_TODO.md` - Detailed task list
- `SESSION_SUMMARY.md` - What's been done

### External:
- **Icon Search:** https://icon-sets.iconify.design
- **Color Palette:** Tailwind default colors
- **Date Formatting:** MDN Intl.DateTimeFormat

---

## 💬 Need Help?

### Check First:
1. Is dev server running? (`pnpm dev`)
2. Did you restart after config changes?
3. Is the issue in the TODO list?
4. Did you read the relevant documentation file?

### Debugging:
```typescript
// Add console logs
console.log('Quotation data:', quotation.value)

// Check Vue DevTools
// Vue DevTools browser extension

// Check Network tab
// See if API calls are failing
```

### Ask Questions:
- Document what you tried
- Include error messages
- Show relevant code
- Mention which file

---

## 🎯 Success Criteria

You'll know you're done when:

**Quotations Module:**
- ✅ Can create quotation in < 2 minutes
- ✅ Can edit and update quotation
- ✅ Can download professional PDF
- ✅ Can email to customer
- ✅ Can convert to sales order
- ✅ All actions work on mobile

**Delivery Notes Module:**
- ✅ Can create delivery from order
- ✅ Can assign driver and schedule
- ✅ Can print packing slip
- ✅ Can capture proof of delivery
- ✅ Can track delivery status

**Performance:**
- ✅ Page loads in < 2 seconds on 3G
- ✅ Forms submit successfully
- ✅ No console errors
- ✅ Works offline (PWA)

---

## 🎉 You're Ready!

### Quick Commands:
```bash
# Start development
pnpm dev

# Build for production
pnpm build

# Preview production build
pnpm preview

# Run linter
pnpm lint

# Format code
pnpm format
```

### Your First Steps:
1. ✅ Read this guide (you're here!)
2. ⏳ Complete quotation edit page
3. ⏳ Implement PDF generation
4. ⏳ Add email sending
5. ⏳ Test everything works

### Remember:
- **Code quality > speed** - Write clean, maintainable code
- **Mobile first** - Test on mobile constantly
- **i18n always** - All text must be translatable
- **Document decisions** - Update docs when you change things
- **Ask questions** - Better to ask than assume

---

**Good luck! You've got this! 🚀**

**Questions?** Check the documentation files or leave comments in code.

**Stuck?** Look at existing pages for patterns (quotations/index.vue is a good reference).

**Making progress?** Update SALES_MODULES_TODO.md to track completion.

---

Last updated: January 10, 2025  
Next review: After quotations module complete
