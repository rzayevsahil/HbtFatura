import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { companyGuard } from './core/guards/company.guard';
import { superAdminGuard } from './core/guards/super-admin.guard';
import { firmBusinessGuard } from './core/guards/firm-business.guard';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'invoices'
  },
  { path: 'login', loadComponent: () => import('./auth/login/login.component').then(m => m.LoginComponent) },
  { path: 'profile', loadComponent: () => import('./auth/profile/profile.component').then(m => m.ProfileComponent), canActivate: [authGuard] },

  // Business Routes (Protected by firmBusinessGuard to exclude SuperAdmin)
  { path: 'invoices', loadComponent: () => import('./invoices/invoice-list/invoice-list.component').then(m => m.InvoiceListComponent), canActivate: [firmBusinessGuard] },
  { path: 'invoices/new', loadComponent: () => import('./invoices/invoice-form/invoice-form.component').then(m => m.InvoiceFormComponent), canActivate: [firmBusinessGuard] },
  { path: 'invoices/:id', loadComponent: () => import('./invoices/invoice-detail/invoice-detail.component').then(m => m.InvoiceDetailComponent), canActivate: [firmBusinessGuard] },
  { path: 'invoices/:id/edit', loadComponent: () => import('./invoices/invoice-form/invoice-form.component').then(m => m.InvoiceFormComponent), canActivate: [firmBusinessGuard] },

  { path: 'orders', loadComponent: () => import('./orders/order-list/order-list.component').then(m => m.OrderListComponent), canActivate: [firmBusinessGuard] },
  { path: 'orders/new', loadComponent: () => import('./orders/order-form/order-form.component').then(m => m.OrderFormComponent), canActivate: [firmBusinessGuard] },
  { path: 'orders/:id', loadComponent: () => import('./orders/order-detail/order-detail.component').then(m => m.OrderDetailComponent), canActivate: [firmBusinessGuard] },
  { path: 'orders/:id/edit', loadComponent: () => import('./orders/order-form/order-form.component').then(m => m.OrderFormComponent), canActivate: [firmBusinessGuard] },

  { path: 'delivery-notes', loadComponent: () => import('./delivery-notes/delivery-note-list/delivery-note-list.component').then(m => m.DeliveryNoteListComponent), canActivate: [firmBusinessGuard] },
  { path: 'delivery-notes/new', loadComponent: () => import('./delivery-notes/delivery-note-form/delivery-note-form.component').then(m => m.DeliveryNoteFormComponent), canActivate: [firmBusinessGuard] },
  { path: 'delivery-notes/:id', loadComponent: () => import('./delivery-notes/delivery-note-detail/delivery-note-detail.component').then(m => m.DeliveryNoteDetailComponent), canActivate: [firmBusinessGuard] },
  { path: 'delivery-notes/:id/edit', loadComponent: () => import('./delivery-notes/delivery-note-form/delivery-note-form.component').then(m => m.DeliveryNoteFormComponent), canActivate: [firmBusinessGuard] },

  { path: 'customers', loadComponent: () => import('./customers/customer-list/customer-list.component').then(m => m.CustomerListComponent), canActivate: [firmBusinessGuard] },
  { path: 'customers/new', loadComponent: () => import('./customers/customer-form/customer-form.component').then(m => m.CustomerFormComponent), canActivate: [firmBusinessGuard] },
  { path: 'customers/:id', loadComponent: () => import('./customers/customer-detail/customer-detail.component').then(m => m.CustomerDetailComponent), canActivate: [firmBusinessGuard] },
  { path: 'customers/:id/edit', loadComponent: () => import('./customers/customer-form/customer-form.component').then(m => m.CustomerFormComponent), canActivate: [firmBusinessGuard] },

  { path: 'main-account-codes', loadComponent: () => import('./main-account-codes/main-account-code-list/main-account-code-list.component').then(m => m.MainAccountCodeListComponent), canActivate: [firmBusinessGuard] },
  { path: 'main-account-codes/new', loadComponent: () => import('./main-account-codes/main-account-code-form/main-account-code-form.component').then(m => m.MainAccountCodeFormComponent), canActivate: [firmBusinessGuard] },
  { path: 'main-account-codes/:id/edit', loadComponent: () => import('./main-account-codes/main-account-code-form/main-account-code-form.component').then(m => m.MainAccountCodeFormComponent), canActivate: [firmBusinessGuard] },

  { path: 'company', loadComponent: () => import('./company/company-settings/company-settings.component').then(m => m.CompanySettingsComponent), canActivate: [firmBusinessGuard, companyGuard] },

  // Admin Routes
  { path: 'firms', loadComponent: () => import('./firms/firm-list/firm-list.component').then(m => m.FirmListComponent), canActivate: [authGuard, superAdminGuard] },
  { path: 'firms/new', loadComponent: () => import('./firms/firm-form/firm-form.component').then(m => m.FirmFormComponent), canActivate: [authGuard, superAdminGuard] },
  { path: 'firms/:id', loadComponent: () => import('./firms/firm-detail/firm-detail.component').then(m => m.FirmDetailComponent), canActivate: [authGuard] },

  { path: 'employees', loadComponent: () => import('./employees/employee-list/employee-list.component').then(m => m.EmployeeListComponent), canActivate: [firmBusinessGuard] },
  { path: 'employees/new', loadComponent: () => import('./employees/employee-form/employee-form.component').then(m => m.EmployeeFormComponent), canActivate: [firmBusinessGuard] },
  { path: 'employees/:id/edit', loadComponent: () => import('./employees/employee-form/employee-form.component').then(m => m.EmployeeFormComponent), canActivate: [firmBusinessGuard] },

  { path: 'cash-registers', loadComponent: () => import('./cash-registers/cash-register-list/cash-register-list.component').then(m => m.CashRegisterListComponent), canActivate: [firmBusinessGuard] },
  { path: 'cash-registers/new', loadComponent: () => import('./cash-registers/cash-register-form/cash-register-form.component').then(m => m.CashRegisterFormComponent), canActivate: [firmBusinessGuard] },
  { path: 'cash-registers/:id', loadComponent: () => import('./cash-registers/cash-register-detail/cash-register-detail.component').then(m => m.CashRegisterDetailComponent), canActivate: [firmBusinessGuard] },
  { path: 'cash-registers/:id/edit', loadComponent: () => import('./cash-registers/cash-register-form/cash-register-form.component').then(m => m.CashRegisterFormComponent), canActivate: [firmBusinessGuard] },

  { path: 'bank-accounts', loadComponent: () => import('./bank-accounts/bank-account-list/bank-account-list.component').then(m => m.BankAccountListComponent), canActivate: [firmBusinessGuard] },
  { path: 'bank-accounts/new', loadComponent: () => import('./bank-accounts/bank-account-form/bank-account-form.component').then(m => m.BankAccountFormComponent), canActivate: [firmBusinessGuard] },
  { path: 'bank-accounts/:id', loadComponent: () => import('./bank-accounts/bank-account-detail/bank-account-detail.component').then(m => m.BankAccountDetailComponent), canActivate: [firmBusinessGuard] },
  { path: 'bank-accounts/:id/edit', loadComponent: () => import('./bank-accounts/bank-account-form/bank-account-form.component').then(m => m.BankAccountFormComponent), canActivate: [firmBusinessGuard] },

  { path: 'account-payments', loadComponent: () => import('./account-payments/account-payment-form/account-payment-form.component').then(m => m.AccountPaymentFormComponent), canActivate: [firmBusinessGuard] },

  { path: 'products', loadComponent: () => import('./products/product-list/product-list.component').then(m => m.ProductListComponent), canActivate: [firmBusinessGuard] },
  { path: 'products/new', loadComponent: () => import('./products/product-form/product-form.component').then(m => m.ProductFormComponent), canActivate: [firmBusinessGuard] },
  { path: 'products/:id', loadComponent: () => import('./products/product-detail/product-detail.component').then(m => m.ProductDetailComponent), canActivate: [firmBusinessGuard] },
  { path: 'products/:id/edit', loadComponent: () => import('./products/product-form/product-form.component').then(m => m.ProductFormComponent), canActivate: [firmBusinessGuard] },

  { path: 'logs', loadComponent: () => import('./reports/log-list/log-list.component').then(m => m.LogListComponent), canActivate: [authGuard, superAdminGuard] },
  { path: 'reports', loadComponent: () => import('./reports/reports.component').then(m => m.ReportsComponent), canActivate: [firmBusinessGuard] },

  { path: 'cheques', loadComponent: () => import('./cheques/cheque-list/cheque-list.component').then(m => m.ChequeListComponent), canActivate: [firmBusinessGuard] },
  { path: 'cheques/new', loadComponent: () => import('./cheques/cheque-form/cheque-form.component').then(m => m.ChequeFormComponent), canActivate: [firmBusinessGuard] },
  { path: 'cheques/:id', loadComponent: () => import('./cheques/cheque-detail/cheque-detail.component').then(m => m.ChequeDetailComponent), canActivate: [firmBusinessGuard] },
  { path: 'cheques/:id/edit', loadComponent: () => import('./cheques/cheque-form/cheque-form.component').then(m => m.ChequeFormComponent), canActivate: [firmBusinessGuard] },

  { path: '**', redirectTo: 'invoices' }
];
