export default defineEventHandler(async (event) => {
  return {
    status: 'ok',
    timestamp: new Date().toISOString(),
    service: 'TOSS Service as Software Platform',
    version: '1.0.0',
    uptime: process.uptime()
  }
})
