import { defineEventHandler, readBody, createError } from 'h3'

export default defineEventHandler(async (event) => {
  const body = await readBody<{ jobs?: Array<any>; drivers?: Array<any> }>(event)
  if (!Array.isArray(body?.jobs) || !Array.isArray(body?.drivers)) {
    throw createError({ statusCode: 400, statusMessage: 'jobs and drivers arrays required' })
  }

  // Mock: assign nearest (random) driver for each job
  const assignments = body.jobs.map((job, idx) => ({ jobId: job.id, driverId: body.drivers[idx % body.drivers.length]?.id }))
  return { assignments, strategy: 'nearest_available_v1' }
})


