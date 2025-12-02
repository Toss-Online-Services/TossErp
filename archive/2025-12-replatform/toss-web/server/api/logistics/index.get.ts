import { defineEventHandler } from 'h3'

export default defineEventHandler(async () => {
  return [
    {
      id: 'ship-001',
      type: 'Inbound',
      status: 'In Transit',
      carrier: 'Community Courier',
      eta: new Date(Date.now() + 1000 * 60 * 90).toISOString(),
      route: ['Supplier Hub', 'Township Central Warehouse'],
    },
    {
      id: 'ship-002',
      type: 'Outbound',
      status: 'Scheduled',
      carrier: 'Local Driver Thami',
      eta: new Date(Date.now() + 1000 * 60 * 240).toISOString(),
      route: ["Thabo's Spaza Shop", 'Customer: Naledi'],
    },
  ]
})

