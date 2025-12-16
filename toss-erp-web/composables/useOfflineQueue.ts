import { useOfflineStatus } from './useOfflineStatus'

interface QueuedOperation<T = any> {
  id: string
  type: string
  payload: T
  createdAt: number
  status: 'pending' | 'syncing' | 'failed'
  retries: number
}

const STORAGE_KEY = 'toss_offline_queue'

function loadQueue(): QueuedOperation[] {
  if (typeof window === 'undefined') return []
  const raw = localStorage.getItem(STORAGE_KEY)
  return raw ? (JSON.parse(raw) as QueuedOperation[]) : []
}

function saveQueue(queue: QueuedOperation[]) {
  if (typeof window === 'undefined') return
  localStorage.setItem(STORAGE_KEY, JSON.stringify(queue))
}

export function useOfflineQueue() {
  const { isOnline } = useOfflineStatus()
  const queue = ref<QueuedOperation[]>(loadQueue())
  const isSyncing = ref(false)

  const enqueue = (type: string, payload: any) => {
    const op: QueuedOperation = {
      id: `${type}-${Date.now()}`,
      type,
      payload,
      createdAt: Date.now(),
      status: 'pending',
      retries: 0
    }
    queue.value.push(op)
    saveQueue(queue.value)
  }

  const remove = (id: string) => {
    queue.value = queue.value.filter(q => q.id !== id)
    saveQueue(queue.value)
  }

  const sync = async (handler: (op: QueuedOperation) => Promise<void>) => {
    if (!isOnline.value || isSyncing.value) return
    isSyncing.value = true
    try {
      for (const op of queue.value) {
        try {
          op.status = 'syncing'
          await handler(op)
          remove(op.id)
        } catch (err) {
          op.status = 'failed'
          op.retries += 1
          saveQueue(queue.value)
        }
      }
    } finally {
      isSyncing.value = false
    }
  }

  watch(
    isOnline,
    online => {
      if (online) {
        // consumer should call sync with handler
      }
    },
    { immediate: true }
  )

  return {
    queue,
    isSyncing,
    enqueue,
    remove,
    sync
  }
}

