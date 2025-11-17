import { defineEventHandler } from 'h3'
import { MockSalesService } from '~/services/mock'

export default defineEventHandler(() => {
  const data = MockSalesService.listPosProfiles()
  return { data }
})
