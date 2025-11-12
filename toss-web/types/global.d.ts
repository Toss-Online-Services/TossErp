import { vi } from 'vitest'

declare global {
  var useHead: typeof vi.fn
  var useRouter: typeof vi.fn
  var useRoute: typeof vi.fn
  var navigateTo: typeof vi.fn
  var useState: typeof vi.fn
  var useRuntimeConfig: typeof vi.fn
  var useApi: typeof vi.fn
}
