import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { DashboardService, DashboardStats, SalesChartData, TopProduct } from '../../services/dashboard.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatIconModule,
    MatProgressSpinnerModule,
    NgxChartsModule
  ],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  stats: DashboardStats | null = null;
  salesChartData: SalesChartData[] = [];
  topProducts: TopProduct[] = [];
  loading = true;

  salesChartScheme = 'vivid';
  productChartScheme = 'cool';

  constructor(
    private dashboardService: DashboardService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadDashboardData();
  }

  navigateToUsers(): void {
    this.router.navigate(['/users']);
  }

  navigateToSales(): void {
    this.router.navigate(['/sales']);
  }

  private loadDashboardData(): void {
    this.loading = true;

    // Load dashboard stats
    this.dashboardService.getDashboardStats().subscribe({
      next: (stats) => {
        this.stats = stats;
      },
      error: (error) => {
        console.error('Error loading dashboard stats:', error);
        // Fallback to mock data
        this.stats = {
          totalSales: 45230,
          totalOrders: 1234,
          totalCustomers: 567,
          totalProducts: 89,
          salesGrowth: 12.5,
          ordersGrowth: 8.2,
          customersGrowth: 15.3
        };
      }
    });

    // Load sales chart data
    this.dashboardService.getSalesChartData().subscribe({
      next: (data) => {
        this.salesChartData = data;
      },
      error: (error) => {
        console.error('Error loading sales chart data:', error);
        // Fallback to mock data
        this.salesChartData = [
          { name: 'Jan', value: 4500 },
          { name: 'Feb', value: 5200 },
          { name: 'Mar', value: 4800 },
          { name: 'Apr', value: 6100 },
          { name: 'May', value: 5800 },
          { name: 'Jun', value: 7200 }
        ];
      }
    });

    // Load top products
    this.dashboardService.getTopProducts().subscribe({
      next: (products) => {
        this.topProducts = products;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading top products:', error);
        // Fallback to mock data
        this.topProducts = [
          { id: 1, name: 'Product A', sales: 150, revenue: 22500 },
          { id: 2, name: 'Product B', sales: 120, revenue: 18000 },
          { id: 3, name: 'Product C', sales: 100, revenue: 15000 },
          { id: 4, name: 'Product D', sales: 80, revenue: 12000 },
          { id: 5, name: 'Product E', sales: 60, revenue: 9000 }
        ];
        this.loading = false;
      }
    });
  }
}
