import { openDB, DBSchema, IDBPDatabase } from 'idb'

interface PosCartItem {
  productId: number
  productName: string
  quantity: number
  unitPrice: number
  total: number
}

interface QueuedSale {
  id: string // idempotency key
  shopId: number
  customerId?: number
  paymentMethod: 'Cash' | 'Card' | 'Mobile'
  paymentReference?: string
  notes?: string
  items: Array<{
    productId: number
    quantity: number
    unitPrice: number
  }>
  total: number
  createdAt: number
  synced: boolean
  saleId?: number // Set after successful sync
}

interface TossDB extends DBSchema {
  cart: {
    key: string
    value: PosCartItem
    indexes: { 'by-product': number }
  }
  queuedSales: {
    key: string
    value: QueuedSale
    indexes: { 'by-synced': boolean; 'by-created': number }
  }
  products: {
    key: number
    value: {
      id: number
      name: string
      sku: string
      price: number
      costPrice?: number
      barcode?: string
    }
    indexes: { 'by-sku': string; 'by-barcode': string }
  }
}

let db: IDBPDatabase<TossDB> | null = null

export const useIndexedDB = () => {
  const initDB = async () => {
    if (db) return db

    db = await openDB<TossDB>('toss-pos-db', 1, {
      upgrade(db) {
        // Cart store
        if (!db.objectStoreNames.contains('cart')) {
          const cartStore = db.createObjectStore('cart', { keyPath: 'productId' })
          cartStore.createIndex('by-product', 'productId')
        }

        // Queued sales store
        if (!db.objectStoreNames.contains('queuedSales')) {
          const salesStore = db.createObjectStore('queuedSales', { keyPath: 'id' })
          salesStore.createIndex('by-synced', 'synced')
          salesStore.createIndex('by-created', 'createdAt')
        }

        // Products cache store
        if (!db.objectStoreNames.contains('products')) {
          const productsStore = db.createObjectStore('products', { keyPath: 'id' })
          productsStore.createIndex('by-sku', 'sku')
          productsStore.createIndex('by-barcode', 'barcode')
        }
      }
    })

    return db
  }

  // Cart operations
  const getCart = async (): Promise<PosCartItem[]> => {
    const database = await initDB()
    return await database.getAll('cart')
  }

  const addToCart = async (item: PosCartItem) => {
    const database = await initDB()
    const existing = await database.get('cart', item.productId)
    
    if (existing) {
      const updated = {
        ...existing,
        quantity: existing.quantity + item.quantity,
        total: (existing.quantity + item.quantity) * item.unitPrice
      }
      await database.put('cart', updated)
    } else {
      await database.add('cart', item)
    }
  }

  const updateCartItem = async (productId: number, quantity: number) => {
    const database = await initDB()
    const item = await database.get('cart', productId)
    
    if (!item) return
    
    if (quantity <= 0) {
      await database.delete('cart', productId)
    } else {
      const updated = {
        ...item,
        quantity,
        total: quantity * item.unitPrice
      }
      await database.put('cart', updated)
    }
  }

  const clearCart = async () => {
    const database = await initDB()
    await database.clear('cart')
  }

  // Queued sales operations
  const queueSale = async (sale: Omit<QueuedSale, 'synced' | 'createdAt'>) => {
    const database = await initDB()
    const queuedSale: QueuedSale = {
      ...sale,
      synced: false,
      createdAt: Date.now()
    }
    await database.add('queuedSales', queuedSale)
    return queuedSale.id
  }

  const getQueuedSales = async (synced?: boolean): Promise<QueuedSale[]> => {
    const database = await initDB()
    
    if (synced !== undefined) {
      return await database.getAllFromIndex('queuedSales', 'by-synced', synced)
    }
    
    return await database.getAll('queuedSales')
  }

  const markSaleSynced = async (id: string, saleId: number) => {
    const database = await initDB()
    const sale = await database.get('queuedSales', id)
    
    if (sale) {
      sale.synced = true
      sale.saleId = saleId
      await database.put('queuedSales', sale)
    }
  }

  const removeQueuedSale = async (id: string) => {
    const database = await initDB()
    await database.delete('queuedSales', id)
  }

  // Products cache operations
  const cacheProduct = async (product: TossDB['products']['value']) => {
    const database = await initDB()
    await database.put('products', product)
  }

  const getCachedProduct = async (id: number) => {
    const database = await initDB()
    return await database.get('products', id)
  }

  const searchCachedProducts = async (query: string) => {
    const database = await initDB()
    const all = await database.getAll('products')
    
    const lowerQuery = query.toLowerCase()
    return all.filter(p => 
      p.name.toLowerCase().includes(lowerQuery) ||
      p.sku.toLowerCase().includes(lowerQuery) ||
      (p.barcode && p.barcode.toLowerCase().includes(lowerQuery))
    )
  }

  const clearProductCache = async () => {
    const database = await initDB()
    await database.clear('products')
  }

  return {
    initDB,
    getCart,
    addToCart,
    updateCartItem,
    clearCart,
    queueSale,
    getQueuedSales,
    markSaleSynced,
    removeQueuedSale,
    cacheProduct,
    getCachedProduct,
    searchCachedProducts,
    clearProductCache
  }
}

