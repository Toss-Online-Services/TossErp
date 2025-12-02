import { defineEventHandler, getRouterParam } from 'h3'

export default defineEventHandler(async (event) => {
  const id = getRouterParam(event, 'id') || 'unknown'
  return {
    id,
    driverLocation: { lat: -26.205, lng: 28.049 },
    lastUpdate: new Date().toISOString(),
    path: [
      { lat: -26.204, lng: 28.047 },
      { lat: -26.2045, lng: 28.048 },
      { lat: -26.205, lng: 28.049 },
    ],
  }
})


