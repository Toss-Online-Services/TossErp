import { z } from 'zod'

export const paginationSchema = z.object({
  page: z.coerce.number().int().positive().default(1),
  pageSize: z.coerce.number().int().positive().max(200).default(50),
  searchTerm: z.string().optional().default(''),
  category: z.string().optional().default('')
})

export const createSkuSchema = z.object({
  sku: z.string().min(1),
  name: z.string().min(1),
  description: z.string().optional(),
  category: z.string().optional(),
  unit: z.string().optional(),
  sellingPrice: z.number().optional(),
  costPrice: z.number().optional(),
  reorderLevel: z.number().int().nonnegative().optional(),
  reorderQty: z.number().int().nonnegative().optional()
})

export const stockMovementSchema = z.object({
  itemId: z.string().min(1),
  quantity: z.number(),
  reason: z.string().optional()
})

export type Pagination = z.infer<typeof paginationSchema>
export type CreateSku = z.infer<typeof createSkuSchema>
export type StockMovement = z.infer<typeof stockMovementSchema>
