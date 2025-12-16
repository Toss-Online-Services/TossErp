import { dashboardKpis, salesTrend, tasksTrend, countrySales } from '../mock-data'

export default defineEventHandler(() => {
  return {
    kpis: dashboardKpis,
    salesTrend,
    tasksTrend,
    countrySales
  }
})

