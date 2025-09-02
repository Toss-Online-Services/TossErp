import { defineEventHandler } from 'h3'
import { sendProblem } from '../../utils/problem'
import { getById } from '../../utils/inventoryData'
import { getTenantId } from '../../utils/tenant'

export default defineEventHandler((event) => {
  const id = event.context.params?.id as string | undefined
  if (!id) return sendProblem(event, 400, 'Invalid id', 'Path parameter id is required')
  const item = getById(id)
  const tenantId = getTenantId(event)
  if (!item || item.tenantId !== tenantId) return sendProblem(event, 404, 'Not found', `SKU with id ${id} not found`, {
    type: 'https://toss.example.com/problems/not-found',
    extensions: { id }
  })
  return item
})
