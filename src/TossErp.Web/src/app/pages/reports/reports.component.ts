import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { NgxChartsModule } from '@swimlane/ngx-charts';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    MatFormFieldModule,
    MatDatepickerModule,
    MatNativeDateModule,
    NgxChartsModule
  ],
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.scss']
})
export class ReportsComponent {
  selectedReport = 'sales';
  selectedPeriod = 'month';

  reportTypes = [
    { value: 'sales', label: 'Sales Report', icon: 'trending_up' },
    { value: 'inventory', label: 'Inventory Report', icon: 'inventory' },
    { value: 'customers', label: 'Customer Report', icon: 'people' },
    { value: 'revenue', label: 'Revenue Report', icon: 'account_balance_wallet' }
  ];

  periods = [
    { value: 'week', label: 'This Week' },
    { value: 'month', label: 'This Month' },
    { value: 'quarter', label: 'This Quarter' },
    { value: 'year', label: 'This Year' }
  ];

  revenueData = [
    { name: 'Jan', value: 45000 },
    { name: 'Feb', value: 52000 },
    { name: 'Mar', value: 48000 },
    { name: 'Apr', value: 61000 },
    { name: 'May', value: 58000 },
    { name: 'Jun', value: 72000 }
  ];

  customerData = [
    { name: 'New', value: 45 },
    { name: 'Returning', value: 35 },
    { name: 'Inactive', value: 20 }
  ];

  revenueChartScheme = 'vivid';
  customerChartScheme = 'cool';

  generateReport() {
    console.log('Generating report:', this.selectedReport, 'for period:', this.selectedPeriod);
  }

  exportReport() {
    console.log('Exporting report as PDF/Excel');
  }
}
