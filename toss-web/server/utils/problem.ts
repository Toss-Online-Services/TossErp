import { H3Event, setResponseHeader, setResponseStatus } from 'h3'

// RFC 9457 Problem Details interface (JSON variant)
export interface ProblemDetails {
  type?: string
  title?: string
  status?: number
  detail?: string
  instance?: string
  // Allow extensions
  [key: string]: unknown
}

/**
 * Send an RFC 9457 Problem Details response.
 * Sets status and content-type=application/problem+json, then returns the object.
 */
export function sendProblem(
  event: H3Event,
  status: number,
  title: string,
  detail?: string,
  options?: { type?: string; instance?: string; extensions?: Record<string, unknown> }
) {
  const body: ProblemDetails = {
    type: options?.type || 'about:blank',
    title,
    status,
    detail,
    instance: options?.instance,
    ...(options?.extensions || {}),
  }
  setResponseStatus(event, status)
  setResponseHeader(event, 'content-type', 'application/problem+json; charset=utf-8')
  return body
}
