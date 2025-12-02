import type { ChartData } from 'chart.js'

export interface ChartDataset {
  label: string
  data: number[]
  backgroundColor?: string | string[]
  borderColor?: string | string[]
  tension?: number
}

export interface TimeSeriesData {
  date: string
  value: number
}

export const useCharts = () => {
  // Generate mock time series data for demonstrations
  const generateTimeSeriesData = (days: number = 7, baseValue: number = 100, variance: number = 20): TimeSeriesData[] => {
    const data: TimeSeriesData[] = []
    const today = new Date()
    
    for (let i = days - 1; i >= 0; i--) {
      const date = new Date(today)
      date.setDate(date.getDate() - i)
      
      const randomVariance = (Math.random() - 0.5) * variance
      const value = Math.max(0, baseValue + randomVariance)
      
      data.push({
        date: date.toISOString().split('T')[0],
        value: Math.round(value)
      })
    }
    
    return data
  }

  // Generate production trend data
  const getProductionTrendData = (): ChartData<'line'> => {
    const data = generateTimeSeriesData(7, 1200, 300)
    
    return {
      labels: data.map(d => new Date(d.date).toLocaleDateString('en-ZA', { month: 'short', day: 'numeric' })),
      datasets: [{
        label: 'Units Produced',
        data: data.map(d => d.value),
        borderColor: 'rgba(59, 130, 246, 1)',
        backgroundColor: 'rgba(59, 130, 246, 0.1)',
        tension: 0.4,
        fill: true,
      }]
    }
  }

  // Generate capacity utilization data
  const getCapacityUtilizationData = (): ChartData<'doughnut'> => {
    return {
      labels: ['Utilized', 'Available'],
      datasets: [{
        data: [75, 25],
        backgroundColor: [
          'rgba(16, 185, 129, 0.8)',
          'rgba(229, 231, 235, 0.8)'
        ],
        borderColor: [
          'rgba(16, 185, 129, 1)',
          'rgba(229, 231, 235, 1)'
        ],
        borderWidth: 2,
      }]
    }
  }

  // Generate sales analytics data
  const getSalesAnalyticsData = (): ChartData<'bar'> => {
    const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun']
    const salesData = months.map(() => Math.floor(Math.random() * 50000) + 20000)
    
    return {
      labels: months,
      datasets: [{
        label: 'Sales (R)',
        data: salesData,
        backgroundColor: 'rgba(245, 158, 11, 0.8)',
        borderColor: 'rgba(245, 158, 11, 1)',
        borderWidth: 1,
      }]
    }
  }

  // Generate inventory levels data
  const getInventoryLevelsData = (): ChartData<'line'> => {
    const products = ['Widget A', 'Gadget B', 'Component C', 'Part D', 'Assembly E']
    const data = generateTimeSeriesData(30, 500, 150)
    
    return {
      labels: data.map(d => new Date(d.date).toLocaleDateString('en-ZA', { month: 'short', day: 'numeric' })),
      datasets: [{
        label: 'Total Inventory',
        data: data.map(d => d.value),
        borderColor: 'rgba(139, 92, 246, 1)',
        backgroundColor: 'rgba(139, 92, 246, 0.1)',
        tension: 0.4,
        fill: true,
      }]
    }
  }

  // Generate financial performance data
  const getFinancialPerformanceData = (): ChartData<'bar'> => {
    const quarters = ['Q1 2024', 'Q2 2024', 'Q3 2024', 'Q4 2024']
    
    return {
      labels: quarters,
      datasets: [
        {
          label: 'Revenue',
          data: [450000, 520000, 480000, 580000],
          backgroundColor: 'rgba(16, 185, 129, 0.8)',
          borderColor: 'rgba(16, 185, 129, 1)',
        },
        {
          label: 'Expenses',
          data: [320000, 380000, 350000, 420000],
          backgroundColor: 'rgba(239, 68, 68, 0.8)',
          borderColor: 'rgba(239, 68, 68, 1)',
        }
      ]
    }
  }

  // Generate HR attendance data
  const getHRAttendanceData = (): ChartData<'line'> => {
    const data = generateTimeSeriesData(30, 95, 5)
    
    return {
      labels: data.map(d => new Date(d.date).toLocaleDateString('en-ZA', { month: 'short', day: 'numeric' })),
      datasets: [{
        label: 'Attendance Rate (%)',
        data: data.map(d => Math.min(100, d.value)),
        borderColor: 'rgba(34, 197, 94, 1)',
        backgroundColor: 'rgba(34, 197, 94, 0.1)',
        tension: 0.4,
        fill: true,
      }]
    }
  }

  // Generate work center performance data
  const getWorkCenterPerformanceData = (): ChartData<'bar'> => {
    const workCenters = ['Assembly Line 1', 'Assembly Line 2', 'Packaging', 'Quality Control']
    
    return {
      labels: workCenters,
      datasets: [
        {
          label: 'Efficiency (%)',
          data: [92, 88, 95, 98],
          backgroundColor: 'rgba(59, 130, 246, 0.8)',
          borderColor: 'rgba(59, 130, 246, 1)',
        },
        {
          label: 'Utilization (%)',
          data: [85, 78, 90, 65],
          backgroundColor: 'rgba(245, 158, 11, 0.8)',
          borderColor: 'rgba(245, 158, 11, 1)',
        }
      ]
    }
  }

  // Generate quality metrics data
  const getQualityMetricsData = (): ChartData<'pie'> => {
    return {
      labels: ['Passed', 'Minor Defects', 'Major Defects', 'Critical Defects'],
      datasets: [{
        data: [920, 45, 25, 10],
        backgroundColor: [
          'rgba(34, 197, 94, 0.8)',
          'rgba(245, 158, 11, 0.8)',
          'rgba(239, 68, 68, 0.8)',
          'rgba(127, 29, 29, 0.8)'
        ],
        borderColor: [
          'rgba(34, 197, 94, 1)',
          'rgba(245, 158, 11, 1)',
          'rgba(239, 68, 68, 1)',
          'rgba(127, 29, 29, 1)'
        ],
        borderWidth: 2,
      }]
    }
  }

  return {
    generateTimeSeriesData,
    getProductionTrendData,
    getCapacityUtilizationData,
    getSalesAnalyticsData,
    getInventoryLevelsData,
    getFinancialPerformanceData,
    getHRAttendanceData,
    getWorkCenterPerformanceData,
    getQualityMetricsData,
  }
}
