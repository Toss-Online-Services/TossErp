import { defineEventHandler, getQuery } from 'h3'
import { MockSalesService } from '~/services/mock'
import type { PosSession } from '~/types/sales'

export default defineEventHandler((event) => {
  const query = getQuery(event)
  const statusParam = query.status as PosSession['status'] | 'all' | undefined
  const profileId = query.profileId as string | undefined
  const cashierId = query.cashierId as string | undefined

  const data = MockSalesService.listPosSessions({
    status: statusParam,
    profileId,
    cashierId
  })

  return { data }
})
