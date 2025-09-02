import type { H3Event } from 'h3'

// Simple tenant extraction from header for MVP. Replace with auth session later.
export function getTenantId(event: H3Event): string {
	const header = getHeader(event, 'x-tenant-id') || ''
	return header || 'tenant1'
}

// Local import to avoid circulars
import { getHeader } from 'h3'

