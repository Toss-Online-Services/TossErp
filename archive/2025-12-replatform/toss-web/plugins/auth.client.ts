export default defineNuxtPlugin(() => {
  const { restoreAuth } = useAuth()
  
  // Restore authentication from localStorage on app initialization
  restoreAuth()
})

