import { defineStore } from 'pinia'
import { computed, ref, watch } from 'vue'

export type UiDirection = 'ltr' | 'rtl'
export type UiSidenavColor = 'primary' | 'dark' | 'info' | 'success' | 'warning' | 'error'

type UiControllerPersistedState = {
  miniSidenav: boolean
  transparentSidenav: boolean
  whiteSidenav: boolean
  sidenavColor: UiSidenavColor
  transparentNavbar: boolean
  fixedNavbar: boolean
  openConfigurator: boolean
  direction: UiDirection
}

const STORAGE_KEY = 'toss-ui-controller'

const defaultState = (): UiControllerPersistedState => ({
  miniSidenav: false,
  transparentSidenav: true,
  whiteSidenav: false,
  sidenavColor: 'info',
  transparentNavbar: true,
  fixedNavbar: true,
  openConfigurator: false,
  direction: 'ltr',
})

export const useUiControllerStore = defineStore('uiController', () => {
  const colorMode = useColorMode()

  const miniSidenav = ref(false)
  const transparentSidenav = ref(true)
  const whiteSidenav = ref(false)
  const sidenavColor = ref<UiSidenavColor>('info')

  const transparentNavbar = ref(true)
  const fixedNavbar = ref(true)
  const openConfigurator = ref(false)

  const direction = ref<UiDirection>('ltr')

  const isDarkMode = computed(() => {
    // colorMode.preference can be 'system'
    return colorMode.value === 'dark'
  })

  const setDarkMode = (enabled: boolean) => {
    colorMode.preference = enabled ? 'dark' : 'light'
  }

  const toggleDarkMode = () => setDarkMode(!isDarkMode.value)

  const setDirection = (value: UiDirection) => {
    direction.value = value
  }

  const toggleDirection = () => {
    direction.value = direction.value === 'rtl' ? 'ltr' : 'rtl'
  }

  const setMiniSidenav = (value: boolean) => {
    miniSidenav.value = value
  }

  const toggleMiniSidenav = () => {
    miniSidenav.value = !miniSidenav.value
  }

  const setTransparentSidenav = (value: boolean) => {
    transparentSidenav.value = value
    if (value) {
      whiteSidenav.value = false
    }
  }

  const setWhiteSidenav = (value: boolean) => {
    whiteSidenav.value = value
    if (value) {
      transparentSidenav.value = false
    }
  }

  const setSidenavColor = (value: UiSidenavColor) => {
    sidenavColor.value = value
  }

  const setTransparentNavbar = (value: boolean) => {
    transparentNavbar.value = value
  }

  const setFixedNavbar = (value: boolean) => {
    fixedNavbar.value = value
  }

  const setOpenConfigurator = (value: boolean) => {
    openConfigurator.value = value
  }

  const toggleConfigurator = () => {
    openConfigurator.value = !openConfigurator.value
  }

  const loadPersistedState = () => {
    if (!process.client) return

    const raw = localStorage.getItem(STORAGE_KEY)
    if (!raw) return

    try {
      const parsed = JSON.parse(raw) as Partial<UiControllerPersistedState>
      const merged = { ...defaultState(), ...parsed }

      miniSidenav.value = merged.miniSidenav
      transparentSidenav.value = merged.transparentSidenav
      whiteSidenav.value = merged.whiteSidenav
      sidenavColor.value = merged.sidenavColor
      transparentNavbar.value = merged.transparentNavbar
      fixedNavbar.value = merged.fixedNavbar
      openConfigurator.value = merged.openConfigurator
      direction.value = merged.direction
    } catch {
      // ignore corrupted state
    }
  }

  const persistState = () => {
    if (!process.client) return

    const payload: UiControllerPersistedState = {
      miniSidenav: miniSidenav.value,
      transparentSidenav: transparentSidenav.value,
      whiteSidenav: whiteSidenav.value,
      sidenavColor: sidenavColor.value,
      transparentNavbar: transparentNavbar.value,
      fixedNavbar: fixedNavbar.value,
      openConfigurator: openConfigurator.value,
      direction: direction.value,
    }

    localStorage.setItem(STORAGE_KEY, JSON.stringify(payload))
  }

  watch(
    [
      miniSidenav,
      transparentSidenav,
      whiteSidenav,
      sidenavColor,
      transparentNavbar,
      fixedNavbar,
      openConfigurator,
      direction,
    ],
    () => {
      persistState()
    },
    { flush: 'post' }
  )

  // Eagerly hydrate persisted state on the client.
  // Keeps behavior consistent even before components mount.
  if (process.client) {
    loadPersistedState()
  }

  return {
    // state
    miniSidenav,
    transparentSidenav,
    whiteSidenav,
    sidenavColor,
    transparentNavbar,
    fixedNavbar,
    openConfigurator,
    direction,

    // derived
    isDarkMode,

    // actions
    loadPersistedState,
    setDarkMode,
    toggleDarkMode,
    setDirection,
    toggleDirection,
    setMiniSidenav,
    toggleMiniSidenav,
    setTransparentSidenav,
    setWhiteSidenav,
    setSidenavColor,
    setTransparentNavbar,
    setFixedNavbar,
    setOpenConfigurator,
    toggleConfigurator,
  }
})
