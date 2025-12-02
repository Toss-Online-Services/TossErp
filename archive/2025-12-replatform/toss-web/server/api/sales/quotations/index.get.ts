import { defineEventHandler, getQuery } from 'h3'
import { listQuotations } from '~/server/utils/quotations'

export default defineEventHandler(async (event) => {
  const query = getQuery(event)
  const status = typeof query.status === 'string' ? query.status : undefined
  const search = typeof query.search === 'string' ? query.search : undefined
  const dateFrom = typeof query.dateFrom === 'string' ? query.dateFrom : undefined
  const dateTo = typeof query.dateTo === 'string' ? query.dateTo : undefined

  const data = await listQuotations({ status, search, dateFrom, dateTo })

  return {
    data: data.records,
    stats: data.stats
  }
})
