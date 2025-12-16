import { stockItems } from '../../mock-data'

export default defineEventHandler(() => {
  return stockItems.filter(item => item.qty < item.minQty)
})

