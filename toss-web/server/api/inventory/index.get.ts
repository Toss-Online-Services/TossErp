import { defineEventHandler, getValidatedQuery } from 'h3'
import type { InventoryListResponse } from '../../../types/inventory'
import { getAll } from '../../utils/inventoryData'
import { paginationSchema } from '../../utils/schemas'
import { getTenantId } from '../../utils/tenant'
import { sendProblem } from '../../utils/problem'

export default defineEventHandler(async (event) => {
	const parsed = await getValidatedQuery(event, paginationSchema.safeParse)
			if (!(parsed as any).success) {
				return sendProblem(event, 400, 'Invalid input', 'Invalid query parameters', {
					type: 'https://toss.example.com/problems/validation-error'
				})
			}
	const { page, pageSize, searchTerm, category } = (parsed as any).data
	const tenantId = getTenantId(event)

	let items = getAll().filter(i => i.tenantId === tenantId)
	if (searchTerm) items = items.filter(i => `${i.name} ${i.sku}`.toLowerCase().includes(searchTerm.toLowerCase()))
	if (category) items = items.filter(i => i.category === category)

	const start = (page - 1) * pageSize
	const paged = items.slice(start, start + pageSize)
	return { items: paged, totalCount: items.length } as InventoryListResponse
})
