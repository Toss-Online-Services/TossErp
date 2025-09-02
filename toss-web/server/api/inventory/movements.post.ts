import { defineEventHandler, readValidatedBody } from 'h3'
import { sendProblem } from '../../utils/problem'
import { applyMovement, getById } from '../../utils/inventoryData'
import { stockMovementSchema } from '../../utils/schemas'
import { getTenantId } from '../../utils/tenant'

export default defineEventHandler(async (event) => {
  const parsed = await readValidatedBody(event, stockMovementSchema.safeParse)
  if (!(parsed as any).success) {
    return sendProblem(event, 400, 'Invalid input', 'itemId and quantity are required', {
      type: 'https://toss.example.com/problems/validation-error'
    })
  }
  const body = (parsed as any).data as import('../../utils/schemas').StockMovement

  const item = getById(body.itemId)
  if (!item) return sendProblem(event, 404, 'Not found', `SKU with id ${body.itemId} not found`)
  const tenantId = getTenantId(event)
  if (item.tenantId !== tenantId) return sendProblem(event, 404, 'Not found', `SKU with id ${body.itemId} not found`)

  const updated = applyMovement({ itemId: body.itemId, quantity: body.quantity, reason: body.reason })
  if (!updated) return sendProblem(event, 500, 'Movement failed', 'Could not apply movement')
  return updated
})
