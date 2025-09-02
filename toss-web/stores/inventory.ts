import { defineStore } from 'pinia'
import { $fetch } from 'ofetch'
import type { InventoryListResponse, SKUItem, CreateSKUInput, StockMovementInput } from '../types/inventory'

export const useInventoryStore = defineStore('inventory', {
  state: () => ({
    items: [] as SKUItem[],
    totalCount: 0,
    loading: false,
    error: '' as string | null,
    current: null as SKUItem | null
  }),
  actions: {
    async list(params?: { page?: number; pageSize?: number; searchTerm?: string; category?: string }) {
      this.loading = true
      this.error = null
      try {
        const query = new URLSearchParams()
        if (params?.page) query.set('page', String(params.page))
        if (params?.pageSize) query.set('pageSize', String(params.pageSize))
        if (params?.searchTerm) query.set('searchTerm', params.searchTerm)
        if (params?.category) query.set('category', params.category)
        const res = await $fetch<InventoryListResponse>(`/api/inventory?${query.toString()}`)
        this.items = res.items
        this.totalCount = res.totalCount
      } catch (e: any) {
        this.error = e?.data?.detail || e?.message || 'Failed to load inventory'
      } finally {
        this.loading = false
      }
    },
    async getById(id: string) {
      this.loading = true
      this.error = null
      try {
        this.current = await $fetch<SKUItem>(`/api/inventory/${id}`)
      } catch (e: any) {
        this.error = e?.data?.detail || e?.message || 'Failed to load item'
      } finally {
        this.loading = false
      }
    },
    async create(input: CreateSKUInput) {
      this.loading = true
      this.error = null
      try {
        const created = await $fetch<SKUItem>(`/api/inventory`, { method: 'POST', body: input })
        this.items.unshift(created)
        this.totalCount += 1
        return created
      } catch (e: any) {
        this.error = e?.data?.detail || e?.message || 'Failed to create item'
        throw e
      } finally {
        this.loading = false
      }
    },
    async move(input: StockMovementInput) {
      this.loading = true
      this.error = null
      try {
        const updated = await $fetch<SKUItem>(`/api/inventory/movements`, { method: 'POST', body: input })
        const idx = this.items.findIndex(i => i.id === updated.id)
        if (idx >= 0) this.items[idx] = updated
        if (this.current?.id === updated.id) this.current = updated
        return updated
      } catch (e: any) {
        this.error = e?.data?.detail || e?.message || 'Failed to apply movement'
        throw e
      } finally {
        this.loading = false
      }
    }
  }
})
