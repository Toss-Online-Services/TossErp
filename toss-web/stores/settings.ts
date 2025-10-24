import { defineStore } from 'pinia'

export const useSettingsStore = defineStore('settings', () => {
  // State
  const darkMode = ref(false)
  const language = ref('en')
  const notifications = ref({
    email: true,
    push: true,
    sms: false
  })
  const sidebar = ref({
    collapsed: false
  })

  // Actions
  const loadSettings = () => {
    if (process.client) {
      const saved = localStorage.getItem('toss-settings')
      if (saved) {
        const parsed = JSON.parse(saved)
        darkMode.value = parsed.darkMode ?? false
        language.value = parsed.language ?? 'en'
        notifications.value = parsed.notifications ?? { email: true, push: true, sms: false }
        sidebar.value = parsed.sidebar ?? { collapsed: false }
      }
    }
  }

  const saveSettings = () => {
    if (process.client) {
      localStorage.setItem('toss-settings', JSON.stringify({
        darkMode: darkMode.value,
        language: language.value,
        notifications: notifications.value,
        sidebar: sidebar.value
      }))
    }
  }

  const toggleDarkMode = () => {
    darkMode.value = !darkMode.value
    saveSettings()
  }

  const setLanguage = (lang: string) => {
    language.value = lang
    saveSettings()
  }

  const toggleSidebar = () => {
    sidebar.value.collapsed = !sidebar.value.collapsed
    saveSettings()
  }

  return {
    // State
    darkMode,
    language,
    notifications,
    sidebar,
    
    // Actions
    loadSettings,
    saveSettings,
    toggleDarkMode,
    setLanguage,
    toggleSidebar
  }
})
