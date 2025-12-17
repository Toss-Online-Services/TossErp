import { defineEventHandler, getQuery } from 'h3'

export default defineEventHandler(async (event) => {
  const q = getQuery(event)
  const category = (q.category as string) || 'all'
  const all = [
    { id: 'prov-1', category: 'driver', name: 'Sipho D.', vehicleType: 'bakkie', rating: 4.9, completed: 124 },
    { id: 'prov-2', category: 'plumber', name: 'Bongani P.', rating: 4.8, completed: 89 },
    { id: 'prov-3', category: 'electrician', name: 'Nomsa E.', rating: 4.7, completed: 76 },
    { id: 'prov-4', category: 'cleaner', name: 'Thandi C.', rating: 4.6, completed: 54 },
  ]
  return category === 'all' ? all : all.filter(p => p.category === category)
})


