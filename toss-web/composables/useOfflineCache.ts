import Dexie, { type Table } from 'dexie'

export interface CachedEntity {
  id: string
  type: string
  data: any
  timestamp: number
  version: number
}

export interface OutboxCommand {
  id: string
  type: string
  payload: any
  timestamp: number
  retries: number
}

class TossOfflineDB extends Dexie {
  cache_entities!: Table<CachedEntity>
  outbox_commands!: Table<OutboxCommand>

  constructor() {
    super('TossOfflineDB')
    this.version(1).stores({
      cache_entities: 'id, type, timestamp',
      outbox_commands: 'id, type, timestamp'
    })
  }
}

let dbInstance: TossOfflineDB | null = null

export const getDB = () => {
  if (!dbInstance) {
    dbInstance = new TossOfflineDB()
  }
  return dbInstance
}

export const useOfflineCache = () => {
  const db = getDB()

  const cacheEntity = async (type: string, id: string, data: any) => {
    await db.cache_entities.put({
      id: `${type}_${id}`,
      type,
      data,
      timestamp: Date.now(),
      version: 1
    })
  }

  const getCachedEntity = async (type: string, id: string) => {
    return await db.cache_entities.get(`${type}_${id}`)
  }

  const clearCache = async (type?: string) => {
    if (type) {
      await db.cache_entities.where('type').equals(type).delete()
    } else {
      await db.cache_entities.clear()
    }
  }

  return {
    cacheEntity,
    getCachedEntity,
    clearCache
  }
}


