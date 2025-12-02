import { defineEventHandler, getRouterParam } from 'h3'

export default defineEventHandler(async (event) => {
  const trackingNumber = getRouterParam(event, 'trackingNumber') || 'unknown'
  return {
    trackingNumber,
    status: 'In Transit',
    lastUpdate: new Date().toISOString(),
    checkpoints: [
      { ts: new Date(Date.now() - 1000 * 60 * 60 * 6).toISOString(), note: 'Picked up from supplier' },
      { ts: new Date(Date.now() - 1000 * 60 * 60 * 2).toISOString(), note: 'Arrived at regional hub' },
      { ts: new Date(Date.now() - 1000 * 60 * 30).toISOString(), note: 'Departed hub towards destination' },
    ],
    eta: new Date(Date.now() + 1000 * 60 * 90).toISOString(),
  }
})

