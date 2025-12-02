import { ref, watch, type Ref } from 'vue'

/**
 * Debounce composable for optimizing search inputs and API calls
 * 
 * Usage:
 * const searchTerm = ref('')
 * const debouncedSearch = useDebounce(searchTerm, 500)
 * 
 * // debouncedSearch.value will only update 500ms after searchTerm stops changing
 */
export function useDebounce<T>(value: Ref<T>, delay: number = 300): Ref<T> {
  const debouncedValue = ref(value.value) as Ref<T>

  let timeoutId: ReturnType<typeof setTimeout> | null = null

  watch(
    value,
    (newValue) => {
      if (timeoutId) {
        clearTimeout(timeoutId)
      }

      timeoutId = setTimeout(() => {
        debouncedValue.value = newValue
      }, delay)
    },
    { immediate: true }
  )

  return debouncedValue
}

/**
 * Throttle composable for limiting function calls
 * 
 * Usage:
 * const throttledFn = useThrottle(() => {
 *   // Expensive operation
 * }, 1000)
 */
export function useThrottle<T extends (...args: any[]) => any>(
  fn: T,
  delay: number = 300
): T {
  let lastCall = 0
  let timeoutId: ReturnType<typeof setTimeout> | null = null

  return ((...args: Parameters<T>) => {
    const now = Date.now()

    if (now - lastCall >= delay) {
      lastCall = now
      return fn(...args)
    } else {
      if (timeoutId) {
        clearTimeout(timeoutId)
      }
      timeoutId = setTimeout(() => {
        lastCall = Date.now()
        fn(...args)
      }, delay - (now - lastCall))
    }
  }) as T
}


