export const useDeliveriesAPI = () => {
  const config = useRuntimeConfig()
  const apiBase = config.public.apiBase
  const baseURL = apiBase + '/api'

  return {
    /**
     * Get deliveries assigned to a driver
     * For MVP, we'll filter purchase orders by driver assignment
     */
    async getDeliveries(driverId: number, status?: string) {
      // TODO: When backend has dedicated delivery endpoints, use those
      // For now, get purchase orders that are assigned to this driver
      return await $fetch<any[]>(`${baseURL}/Buying/purchase-orders`, {
        method: 'GET',
        params: {
          driverId,
          status: status || undefined
        }
      })
    },

    /**
     * Get delivery by ID
     */
    async getDeliveryById(id: number) {
      return await $fetch<any>(`${baseURL}/Buying/purchase-orders/${id}`, {
        method: 'GET'
      })
    },

    /**
     * Update delivery status
     */
    async updateDeliveryStatus(id: number, status: string, notes?: string) {
      return await $fetch<{ success: boolean }>(`${baseURL}/Buying/purchase-orders/${id}/status`, {
        method: 'POST',
        body: { status, notes }
      })
    },

    /**
     * Mark delivery as picked up
     */
    async markPickedUp(id: number, notes?: string) {
      return await this.updateDeliveryStatus(id, 'PickedUp', notes)
    },

    /**
     * Mark delivery as delivered
     */
    async markDelivered(id: number, notes?: string, confirmed: boolean = true) {
      return await $fetch<{ success: boolean }>(`${baseURL}/Buying/purchase-orders/${id}/status`, {
        method: 'POST',
        body: {
          status: 'Delivered',
          notes,
          deliveryConfirmed: confirmed
        }
      })
    }
  }
}

