import { Routes } from '@angular/router';
import { AdminComponent } from './layouts/admin/admin.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';

export const routes: Routes = [
    {
        path: '',
        component: AdminComponent,
        children: [
          { path: 'dashboard', component: DashboardComponent },
          { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
        ]
      },
      // Fallback when no prior route is matched
      { path: '**', redirectTo: '', pathMatch: 'full' }
];
