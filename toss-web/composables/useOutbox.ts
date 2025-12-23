import { ref, computed, onMounted } from 'vue'
import { getDB, type OutboxCommand } from './useOfflineCache'

const outboxCommands = ref<OutboxCommand[]>([])

export const useOutbox = () => {
  const db = getDB()

  const loadCommands = async () => {
    const commands = await db.outbox_commands.toArray()
    outboxCommands.value = commands
  }

  const queueCommand = async (type: string, payload: any) => {
    const command: OutboxCommand = {
      id: `${Date.now()}-${Math.random().toString(36).substr(2, 9)}`,
      type,
      payload,
      timestamp: Date.now(),
      retries: 0
    }
    await db.outbox_commands.add(command)
    await loadCommands()
    return command.id
  }

  const removeCommand = async (id: string) => {
    await db.outbox_commands.delete(id)
    await loadCommands()
  }

  const incrementRetries = async (id: string) => {
    const command = await db.outbox_commands.get(id)
    if (command) {
      command.retries++
      await db.outbox_commands.put(command)
      await loadCommands()
    }
  }

  const pendingCount = computed(() => outboxCommands.value.length)

  const flush = async () => {
    const commands = await db.outbox_commands.toArray()
    for (const command of commands) {
      try {
        // In a real implementation, this would call the API
        // const config = useRuntimeConfig()
        // await $fetch(`${config.public.apiBase}/api/commands`, {
        //   method: 'POST',
        //   body: command
        // })
        await removeCommand(command.id)
      } catch (error) {
        await incrementRetries(command.id)
      }
    }
  }

  onMounted(() => {
    loadCommands()
  })

  return {
    queueCommand,
    removeCommand,
    pendingCount,
    flush,
    commands: outboxCommands
  }
}

