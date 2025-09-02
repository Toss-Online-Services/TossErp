import { defineEventHandler, readValidatedBody } from 'h3'
import { sendProblem } from '../../utils/problem'
import type { SKUItem } from '../../../types/inventory'
import { addItem } from '../../utils/inventoryData'
import { createSkuSchema } from '../../utils/schemas'
import { getTenantId } from '../../utils/tenant'

// naive in-memory id counter
let nextId = 1000
const nowIso = () => new Date().toISOString()

export default defineEventHandler(async (event) => {
	const validated = await readValidatedBody(event, createSkuSchema.safeParse)
	if (!(validated as any).success) {
		return sendProblem(event, 400, 'Invalid input', 'Request body validation failed', {
			type: 'https://toss.example.com/problems/validation-error'
		})
	}
	const body = (validated as any).data as import('../../utils/schemas').CreateSku

	// In real app, infer tenant from auth/session
	const tenantId = getTenantId(event)
	const item: SKUItem = {
		id: String(nextId++),
		tenantId,
		sku: body.sku,
		name: body.name,
		description: body.description,
		category: body.category,
		unit: body.unit,
		sellingPrice: body.sellingPrice,
		costPrice: body.costPrice,
		reorderLevel: body.reorderLevel,
		reorderQty: body.reorderQty,
		isActive: true,
		createdAt: nowIso(),
		updatedAt: nowIso(),
		quantityOnHand: 0
	}
		// TODO: persist to DB
		addItem(item)
		return item
})
