export const useSettingsStore = defineStore('settings', {
  state: () => ({
    darkMode: false,
    language: 'en',
    notifications: {
      email: true,
      push: true,
      sms: false
    },
    sidebar: {
      collapsed: false
    }
  }),
  
  actions: {
    async loadSettings() {
      // Load from localStorage or API
      const saved = localStorage.getItem('toss-settings')
      if (saved) {
        this.$patch(JSON.parse(saved))
      }
    },
    
    async saveSettings() {
      localStorage.setItem('toss-settings', JSON.stringify(this.$state))
    },
    
    toggleDarkMode() {
      this.darkMode = !this.darkMode
      this.saveSettings()
    },
    
    setLanguage(lang: string) {
      this.language = lang
      this.saveSettings()
    },
    
    toggleSidebar() {
      this.sidebar.collapsed = !this.sidebar.collapsed
      this.saveSettings()
    }
  }
})
