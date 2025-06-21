import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { NgxChartsModule } from '@swimlane/ngx-charts';

@Component({
  selector: 'app-sales',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatCardModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    NgxChartsModule
  ],
  templateUrl: './sales.component.html',
  styleUrls: ['./sales.component.scss']
})
export class SalesComponent {
  displayedColumns: string[] = ['id', 'date', 'customer', 'product', 'amount', 'status'];
  dateRange: FormGroup;

  sales = [
    { id: 1, date: '2024-01-15', customer: 'John Doe', product: 'Product A', amount: 150.00, status: 'Completed' },
    { id: 2, date: '2024-01-14', customer: 'Jane Smith', product: 'Product B', amount: 275.50, status: 'Pending' },
    { id: 3, date: '2024-01-13', customer: 'Bob Johnson', product: 'Product C', amount: 89.99, status: 'Completed' },
    { id: 4, date: '2024-01-12', customer: 'Alice Brown', product: 'Product A', amount: 150.00, status: 'Completed' },
    { id: 5, date: '2024-01-11', customer: 'Charlie Wilson', product: 'Product D', amount: 320.00, status: 'Cancelled' }
  ];

  salesChartData = [
    { name: 'Jan', value: 4500 },
    { name: 'Feb', value: 5200 },
    { name: 'Mar', value: 4800 },
    { name: 'Apr', value: 6100 },
    { name: 'May', value: 5800 },
    { name: 'Jun', value: 7200 }
  ];

  productChartData = [
    { name: 'Product A', value: 35 },
    { name: 'Product B', value: 25 },
    { name: 'Product C', value: 20 },
    { name: 'Product D', value: 20 }
  ];

  salesChartScheme = 'vivid';
  productChartScheme = 'cool';

  constructor(private fb: FormBuilder) {
    this.dateRange = this.fb.group({
      start: [''],
      end: ['']
    });
  }
}
