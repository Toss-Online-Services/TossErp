import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { useStockStore } from './stock'

export interface Supplier {
  id: string
  code: string
  name: string
  contactPerson?: string
  email?: string
  phone?: string
  address?: string
  city?: string
  province?: string
  postalCode?: string
  paymentTerms: string // e.g., "Net 30", "Cash on Delivery"
  currency: string
  creditLimit?: number
  currentBalance: number
  isActive: boolean
  notes?: string
  createdAt: Date
  updatedAt: Date
}

export interface PurchaseOrderItem {
  id: string
  itemId: string
  itemCode: string
  itemName: string
  quantity: number
  unit: string
  rate: number
  amount: number
  receivedQuantity: number
  rejectedQuantity: number
}

export type PurchaseOrderStatus = 'draft' | 'submitted' | 'approved' | 'partially_received' | 'completed' | 'cancelled'

export interface PurchaseOrder {
  id: string
  poNumber: string
  supplierId: string
  supplierName: string
  orderDate: Date
  expectedDeliveryDate?: Date
  requiredDate?: Date
  status: PurchaseOrderStatus
  subtotal: number
  taxAmount: number
  shippingCost: number
  total: number
  notes?: string
  approvedBy?: string
  approvedDate?: Date
  items: PurchaseOrderItem[]
  createdAt: Date
  updatedAt: Date
}

export interface GoodsReceiptItem {
  id: string
  purchaseOrderId: string
  purchaseOrderItemId: string
  itemId: string
  itemCode: string
  itemName: string
  orderedQuantity: number
  receivedQuantity: number
  rejectedQuantity: number
  rate: number
  amount: number
  batchNumber?: string
  expiryDate?: Date
  notes?: string
}

export type GoodsReceiptStatus = 'draft' | 'submitted' | 'cancelled'

export interface GoodsReceipt {
  id: string
  receiptNumber: string
  purchaseOrderId?: string
  purchaseOrderNumber?: string
  supplierId: string
  supplierName: string
  receiptDate: Date
  warehouse: string
  status: GoodsReceiptStatus
  subtotal: number
  taxAmount: number
  total: number
  notes?: string
  items: GoodsReceiptItem[]
  createdAt: Date
  updatedAt: Date
}

export const useBuyingStore = defineStore('buying', () => {
  const stockStore = useStockStore()

  // State
  const suppliers = ref<Supplier[]>([])
  const purchaseOrders = ref<PurchaseOrder[]>([])
  const goodsReceipts = ref<GoodsReceipt[]>([])
  const loading = ref(false)

  // Computed
  const activeSuppliers = computed(() => {
    return suppliers.value.filter(s => s.isActive)
  })

  const pendingPurchaseOrders = computed(() => {
    return purchaseOrders.value.filter(po => 
      po.status === 'submitted' || po.status === 'approved' || po.status === 'partially_received'
    )
  })

  const draftPurchaseOrders = computed(() => {
    return purchaseOrders.value.filter(po => po.status === 'draft')
  })

  const totalOutstanding = computed(() => {
    return suppliers.value.reduce((sum, s) => sum + s.currentBalance, 0)
  })

  // Actions
  async function fetchSuppliers() {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      // Mock data
      suppliers.value = [
        {
          id: '1',
          code: 'SUP-001',
          name: 'PPC Cement',
          contactPerson: 'John Smith',
          email: 'john@ppc.co.za',
          phone: '+27 11 123 4567',
          address: '123 Industrial Road',
          city: 'Johannesburg',
          province: 'Gauteng',
          postalCode: '2000',
          paymentTerms: 'Net 30',
          currency: 'ZAR',
          creditLimit: 50000,
          currentBalance: 0,
          isActive: true,
          createdAt: new Date(),
          updatedAt: new Date()
        },
        {
          id: '2',
          code: 'SUP-002',
          name: 'Tongaat Hulett',
          contactPerson: 'Sarah Johnson',
          email: 'sarah@tongaat.co.za',
          phone: '+27 31 234 5678',
          address: '456 Sugar Street',
          city: 'Durban',
          province: 'KwaZulu-Natal',
          postalCode: '4000',
          paymentTerms: 'Net 15',
          currency: 'ZAR',
          creditLimit: 30000,
          currentBalance: 0,
          isActive: true,
          createdAt: new Date(),
          updatedAt: new Date()
        },
        {
          id: '3',
          code: 'SUP-003',
          name: 'Willowton Oil',
          contactPerson: 'Mike Williams',
          email: 'mike@willowton.co.za',
          phone: '+27 12 345 6789',
          address: '789 Oil Avenue',
          city: 'Pretoria',
          province: 'Gauteng',
          postalCode: '0001',
          paymentTerms: 'Cash on Delivery',
          currency: 'ZAR',
          creditLimit: 20000,
          currentBalance: 0,
          isActive: true,
          createdAt: new Date(),
          updatedAt: new Date()
        }
      ]
    } catch (error) {
      console.error('Failed to fetch suppliers:', error)
    } finally {
      loading.value = false
    }
  }

  async function fetchPurchaseOrders() {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      // Mock data
      purchaseOrders.value = [
        {
          id: '1',
          poNumber: 'PO-2024-001',
          supplierId: '1',
          supplierName: 'PPC Cement',
          orderDate: new Date('2024-01-15'),
          expectedDeliveryDate: new Date('2024-01-25'),
          status: 'submitted',
          subtotal: 8500,
          taxAmount: 1275,
          shippingCost: 200,
          total: 9975,
          items: [
            {
              id: '1',
              itemId: '1',
              itemCode: 'CEM-001',
              itemName: 'Cement 50kg',
              quantity: 100,
              unit: 'bag',
              rate: 85,
              amount: 8500,
              receivedQuantity: 0,
              rejectedQuantity: 0
            }
          ],
          createdAt: new Date('2024-01-15'),
          updatedAt: new Date('2024-01-15')
        },
        {
          id: '2',
          poNumber: 'PO-2024-002',
          supplierId: '2',
          supplierName: 'Tongaat Hulett',
          orderDate: new Date('2024-01-20'),
          expectedDeliveryDate: new Date('2024-01-30'),
          status: 'draft',
          subtotal: 2800,
          taxAmount: 420,
          shippingCost: 100,
          total: 3320,
          items: [
            {
              id: '2',
              itemId: '2',
              itemCode: 'SUG-001',
              itemName: 'Sugar 2.5kg',
              quantity: 100,
              unit: 'pack',
              rate: 28,
              amount: 2800,
              receivedQuantity: 0,
              rejectedQuantity: 0
            }
          ],
          createdAt: new Date('2024-01-20'),
          updatedAt: new Date('2024-01-20')
        }
      ]
    } catch (error) {
      console.error('Failed to fetch purchase orders:', error)
    } finally {
      loading.value = false
    }
  }

  async function fetchGoodsReceipts() {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      // Mock data
      goodsReceipts.value = [
        {
          id: '1',
          receiptNumber: 'GR-2024-001',
          purchaseOrderId: '1',
          purchaseOrderNumber: 'PO-2024-001',
          supplierId: '1',
          supplierName: 'PPC Cement',
          receiptDate: new Date('2024-01-24'),
          warehouse: 'main',
          status: 'submitted',
          subtotal: 8500,
          taxAmount: 1275,
          total: 9775,
          items: [
            {
              id: '1',
              purchaseOrderId: '1',
              purchaseOrderItemId: '1',
              itemId: '1',
              itemCode: 'CEM-001',
              itemName: 'Cement 50kg',
              orderedQuantity: 100,
              receivedQuantity: 100,
              rejectedQuantity: 0,
              rate: 85,
              amount: 8500
            }
          ],
          createdAt: new Date('2024-01-24'),
          updatedAt: new Date('2024-01-24')
        }
      ]
    } catch (error) {
      console.error('Failed to fetch goods receipts:', error)
    } finally {
      loading.value = false
    }
  }

  function getSupplierById(id: string): Supplier | undefined {
    return suppliers.value.find(s => s.id === id)
  }

  function getPurchaseOrderById(id: string): PurchaseOrder | undefined {
    return purchaseOrders.value.find(po => po.id === id)
  }

  function getGoodsReceiptById(id: string): GoodsReceipt | undefined {
    return goodsReceipts.value.find(gr => gr.id === id)
  }

  async function createPurchaseOrder(po: Omit<PurchaseOrder, 'id' | 'poNumber' | 'createdAt' | 'updatedAt'>): Promise<PurchaseOrder> {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const newPO: PurchaseOrder = {
        ...po,
        id: `po_${Date.now()}`,
        poNumber: `PO-${new Date().getFullYear()}-${String(purchaseOrders.value.length + 1).padStart(3, '0')}`,
        createdAt: new Date(),
        updatedAt: new Date()
      }
      
      purchaseOrders.value.push(newPO)
      return newPO
    } catch (error) {
      console.error('Failed to create purchase order:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  async function updatePurchaseOrder(id: string, updates: Partial<PurchaseOrder>): Promise<void> {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const index = purchaseOrders.value.findIndex(po => po.id === id)
      if (index !== -1) {
        purchaseOrders.value[index] = {
          ...purchaseOrders.value[index],
          ...updates,
          updatedAt: new Date()
        }
      }
    } catch (error) {
      console.error('Failed to update purchase order:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  async function submitPurchaseOrder(id: string): Promise<void> {
    await updatePurchaseOrder(id, { status: 'submitted' })
  }

  async function createGoodsReceipt(gr: Omit<GoodsReceipt, 'id' | 'receiptNumber' | 'createdAt' | 'updatedAt'>): Promise<GoodsReceipt> {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const newGR: GoodsReceipt = {
        ...gr,
        id: `gr_${Date.now()}`,
        receiptNumber: `GR-${new Date().getFullYear()}-${String(goodsReceipts.value.length + 1).padStart(3, '0')}`,
        createdAt: new Date(),
        updatedAt: new Date()
      }
      
      goodsReceipts.value.push(newGR)
      
      // Update stock for received items
      if (gr.status === 'submitted') {
        for (const item of gr.items) {
          const stockItem = stockStore.items.find(i => i.id === item.itemId)
          if (stockItem) {
            stockItem.currentStock += item.receivedQuantity
            // Create stock movement
            stockStore.movements.push({
              id: `mov_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`,
              itemId: item.itemId,
              itemName: item.itemName,
              type: 'purchase',
              quantity: item.receivedQuantity,
              warehouse: gr.warehouse,
              referenceType: 'GoodsReceipt',
              referenceId: newGR.id,
              notes: `Received from ${gr.supplierName}`,
              createdBy: 'current_user', // TODO: Get from auth
              createdAt: new Date()
            })
          }
        }
      }
      
      return newGR
    } catch (error) {
      console.error('Failed to create goods receipt:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  async function createSupplier(supplier: Omit<Supplier, 'id' | 'code' | 'currentBalance' | 'createdAt' | 'updatedAt'>): Promise<Supplier> {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const newSupplier: Supplier = {
        ...supplier,
        id: `sup_${Date.now()}`,
        code: `SUP-${String(suppliers.value.length + 1).padStart(3, '0')}`,
        currentBalance: 0,
        createdAt: new Date(),
        updatedAt: new Date()
      }
      
      suppliers.value.push(newSupplier)
      return newSupplier
    } catch (error) {
      console.error('Failed to create supplier:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  async function updateSupplier(id: string, updates: Partial<Supplier>): Promise<void> {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const index = suppliers.value.findIndex(s => s.id === id)
      if (index !== -1) {
        suppliers.value[index] = {
          ...suppliers.value[index],
          ...updates,
          updatedAt: new Date()
        }
      }
    } catch (error) {
      console.error('Failed to update supplier:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  return {
    // State
    suppliers,
    purchaseOrders,
    goodsReceipts,
    loading,
    
    // Computed
    activeSuppliers,
    pendingPurchaseOrders,
    draftPurchaseOrders,
    totalOutstanding,
    
    // Actions
    fetchSuppliers,
    fetchPurchaseOrders,
    fetchGoodsReceipts,
    getSupplierById,
    getPurchaseOrderById,
    getGoodsReceiptById,
    createPurchaseOrder,
    updatePurchaseOrder,
    submitPurchaseOrder,
    createGoodsReceipt,
    createSupplier,
    updateSupplier
  }
})

