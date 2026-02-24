import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { companyGuard } from './core/guards/company.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'invoices', pathMatch: 'full' },
  { path: 'login', loadComponent: () => import('./auth/login/login.component').then(m => m.LoginComponent) },
  { path: 'invoices', loadComponent: () => import('./invoices/invoice-list/invoice-list.component').then(m => m.InvoiceListComponent), canActivate: [authGuard] },
  { path: 'invoices/new', loadComponent: () => import('./invoices/invoice-form/invoice-form.component').then(m => m.InvoiceFormComponent), canActivate: [authGuard] },
  { path: 'invoices/:id', loadComponent: () => import('./invoices/invoice-detail/invoice-detail.component').then(m => m.InvoiceDetailComponent), canActivate: [authGuard] },
  { path: 'invoices/:id/edit', loadComponent: () => import('./invoices/invoice-form/invoice-form.component').then(m => m.InvoiceFormComponent), canActivate: [authGuard] },
  { path: 'customers', loadComponent: () => import('./customers/customer-list/customer-list.component').then(m => m.CustomerListComponent), canActivate: [authGuard] },
  { path: 'customers/new', loadComponent: () => import('./customers/customer-form/customer-form.component').then(m => m.CustomerFormComponent), canActivate: [authGuard] },
  { path: 'customers/:id/edit', loadComponent: () => import('./customers/customer-form/customer-form.component').then(m => m.CustomerFormComponent), canActivate: [authGuard] },
  { path: 'company', loadComponent: () => import('./company/company-settings/company-settings.component').then(m => m.CompanySettingsComponent), canActivate: [authGuard, companyGuard] },
  { path: 'firms', loadComponent: () => import('./firms/firm-list/firm-list.component').then(m => m.FirmListComponent), canActivate: [authGuard] },
  { path: 'firms/new', loadComponent: () => import('./firms/firm-form/firm-form.component').then(m => m.FirmFormComponent), canActivate: [authGuard] },
  { path: 'firms/:id', loadComponent: () => import('./firms/firm-detail/firm-detail.component').then(m => m.FirmDetailComponent), canActivate: [authGuard] },
  { path: 'employees', loadComponent: () => import('./employees/employee-list/employee-list.component').then(m => m.EmployeeListComponent), canActivate: [authGuard] },
  { path: 'employees/new', loadComponent: () => import('./employees/employee-form/employee-form.component').then(m => m.EmployeeFormComponent), canActivate: [authGuard] },
  { path: '**', redirectTo: 'invoices' }
];
