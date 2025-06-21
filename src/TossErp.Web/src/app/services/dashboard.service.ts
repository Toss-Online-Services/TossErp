import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';

export interface DashboardStats {
  totalSales: number;
  totalOrders: number;
  totalCustomers: number;
  totalProducts: number;
  salesGrowth: number;
  ordersGrowth: number;
  customersGrowth: number;
}

export interface SalesChartData {
  name: string;
  value: number;
}

export interface TopProduct {
  id: number;
  name: string;
  sales: number;
  revenue: number;
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  constructor(private apiService: ApiService) { }

  getDashboardStats(): Observable<DashboardStats> {
    return this.apiService.get<DashboardStats>('/dashboard/stats');
  }

  getSalesChartData(period: string = 'month'): Observable<SalesChartData[]> {
    return this.apiService.get<SalesChartData[]>('/dashboard/sales-chart', { period });
  }

  getTopProducts(limit: number = 5): Observable<TopProduct[]> {
    return this.apiService.get<TopProduct[]>('/dashboard/top-products', { limit });
  }

  getRecentSales(limit: number = 10): Observable<any[]> {
    return this.apiService.get<any[]>('/dashboard/recent-sales', { limit });
  }
}
