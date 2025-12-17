import type { NitroFetchOptions } from 'nitropack'

export function useApiClient() {
  const config = useRuntimeConfig()

  const apiFetch = async <T>(
    path: string,
    opts: Omit<NitroFetchOptions<string>, 'baseURL'> = {}
  ) => {
    const baseURL = config.public.apiBase || ''
    return $fetch<T>(path, {
      baseURL,
      ...opts
    })
  }

  return {
    apiFetch
  }
}
