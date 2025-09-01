import { defineEventHandler, getRouterParam, setHeader, sendStream } from 'h3'

export default defineEventHandler(async (event) => {
  const id = getRouterParam(event, 'id') || 'unknown'
  setHeader(event, 'Content-Type', 'text/event-stream')
  setHeader(event, 'Cache-Control', 'no-cache')
  setHeader(event, 'Connection', 'keep-alive')

  const controller = new ReadableStream({
    start(controller) {
      const push = (data: any) => controller.enqueue(`data: ${JSON.stringify(data)}\n\n`)
      push({ type: 'connected', id })
      const interval = setInterval(() => {
        push({ type: 'location', id, lat: -26.205 + Math.random() * 0.002, lng: 28.049 + Math.random() * 0.002, ts: Date.now() })
      }, 2000)
      ;(event.node.res as any).on('close', () => { clearInterval(interval); controller.close() })
    }
  })

  return sendStream(event, controller as any)
})


