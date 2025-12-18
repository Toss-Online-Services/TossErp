import { defineEventHandler } from 'h3'

interface KPISet {
	totalRevenue: number
	revenueDelta: number
	totalOrders: number
	ordersDelta: number
	newCustomers: number
	customersDelta: number
	inventoryValue: number
	inventoryDelta: number
}

interface TimeSeries {
	labels: string[]
	data: number[]
}

interface OrdersSeries {
	labels: string[]
	data: number[]
	period: '7D' | '30D' | '90D'
}

const buildSequentialData = (length: number, base: number, variance: number) => {
	return Array.from({ length }, (_, index) => {
		const delta = Math.sin(index / 3) * variance * 0.6 + Math.cos(index / 2) * variance * 0.4
		return Math.max(Math.round(base + delta + (index % 5) * variance * 0.15), 5)
	})
}

export default defineEventHandler(() => {
	const kpis: KPISet = {
		totalRevenue: 1250000,
		revenueDelta: 12.5,
		totalOrders: 2457,
		ordersDelta: 8.2,
		newCustomers: 356,
		customersDelta: 15.3,
		inventoryValue: 287500,
		inventoryDelta: -2.1
	}

	const revenueTrend: TimeSeries = {
		labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
		data: [185000, 198500, 205000, 218000, 224000, 231500, 240000, 248500, 256000, 263500, 271000, 280500]
	}

	const salesByCategory: TimeSeries = {
		labels: ['Groceries', 'Prepared Food', 'Airtime & Data', 'Household', 'Toiletries', 'Other'],
		data: [485000, 312000, 205000, 164000, 132500, 78000]
	}

	const ordersOverview: Record<OrdersSeries['period'], OrdersSeries> = {
		'7D': {
			period: '7D',
			labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
			data: buildSequentialData(7, 180, 22)
		},
		'30D': {
			period: '30D',
			labels: Array.from({ length: 30 }, (_, index) => `Day ${index + 1}`),
			data: buildSequentialData(30, 165, 28)
		},
		'90D': {
			period: '90D',
			labels: Array.from({ length: 90 }, (_, index) => `Day ${index + 1}`),
			data: buildSequentialData(90, 150, 35)
		}
	}

	const salesByRegion = [
		{ name: 'South Africa', flag: 'za', sales: 465000, growth: 12.4, orders: 1280 },
		{ name: 'Nigeria', flag: 'ng', sales: 348500, growth: 9.8, orders: 1034 },
		{ name: 'Kenya', flag: 'ke', sales: 218750, growth: 7.5, orders: 724 },
		{ name: 'Ghana', flag: 'gh', sales: 164500, growth: 3.2, orders: 518 },
		{ name: 'Tanzania', flag: 'tz', sales: 142750, growth: 5.9, orders: 442 }
	]

	return {
		kpis,
		revenueTrend,
		salesByCategory,
		ordersOverview,
		salesByRegion,
		updatedAt: new Date().toISOString()
	}
})
