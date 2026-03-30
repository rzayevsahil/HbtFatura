import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { companyGuard } from './core/guards/company.guard';
import { superAdminGuard, notSuperAdminGuard } from './core/guards/super-admin.guard';
import { firmBusinessGuard } from './core/guards/firm-business.guard';
import { PermissionGuard } from './core/guards/permission.guard';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'dashboard'
  },
  { path: 'login', loadComponent: () => import('./auth/login/login.component').then(m => m.LoginComponent) },
  { path: 'profile', loadComponent: () => import('./auth/profile/profile.component').then(m => m.ProfileComponent), canActivate: [authGuard] },

  // Business Routes
  {
    path: 'invoices',
    loadComponent: () => import('./invoices/invoice-list/invoice-list.component').then(m => m.InvoiceListComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Invoices.View' }
  },
  {
    path: 'invoices/new',
    loadComponent: () => import('./invoices/invoice-form/invoice-form.component').then(m => m.InvoiceFormComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Invoices.Create' }
  },
  {
    path: 'invoices/:id',
    loadComponent: () => import('./invoices/invoice-detail/invoice-detail.component').then(m => m.InvoiceDetailComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Invoices.View' }
  },
  {
    path: 'invoices/:id/edit',
    loadComponent: () => import('./invoices/invoice-form/invoice-form.component').then(m => m.InvoiceFormComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Invoices.Edit' }
  },

  {
    path: 'gib-simulation/inbox',
    loadComponent: () => import('./gib-simulation/gib-inbox/gib-inbox.component').then(m => m.GibInboxComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'GibSimulation.ViewInbox' }
  },

  {
    path: 'orders',
    loadComponent: () => import('./orders/order-list/order-list.component').then(m => m.OrderListComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Orders.View' }
  },
  {
    path: 'orders/new',
    loadComponent: () => import('./orders/order-form/order-form.component').then(m => m.OrderFormComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Orders.Create' }
  },
  {
    path: 'orders/:id',
    loadComponent: () => import('./orders/order-detail/order-detail.component').then(m => m.OrderDetailComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Orders.View' }
  },
  {
    path: 'orders/:id/edit',
    loadComponent: () => import('./orders/order-form/order-form.component').then(m => m.OrderFormComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Orders.Edit' }
  },

  {
    path: 'delivery-notes',
    loadComponent: () => import('./delivery-notes/delivery-note-list/delivery-note-list.component').then(m => m.DeliveryNoteListComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'DeliveryNotes.View' }
  },
  {
    path: 'delivery-notes/new',
    loadComponent: () => import('./delivery-notes/delivery-note-form/delivery-note-form.component').then(m => m.DeliveryNoteFormComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'DeliveryNotes.Create' }
  },
  {
    path: 'delivery-notes/:id',
    loadComponent: () => import('./delivery-notes/delivery-note-detail/delivery-note-detail.component').then(m => m.DeliveryNoteDetailComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'DeliveryNotes.View' }
  },
  {
    path: 'delivery-notes/:id/edit',
    loadComponent: () => import('./delivery-notes/delivery-note-form/delivery-note-form.component').then(m => m.DeliveryNoteFormComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'DeliveryNotes.Edit' }
  },

  {
    path: 'customers',
    loadComponent: () => import('./customers/customer-list/customer-list.component').then(m => m.CustomerListComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Customers.View' }
  },
  {
    path: 'customers/new',
    loadComponent: () => import('./customers/customer-form/customer-form.component').then(m => m.CustomerFormComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Customers.Edit' }
  },
  {
    path: 'customers/:id/edit',
    loadComponent: () => import('./customers/customer-form/customer-form.component').then(m => m.CustomerFormComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Customers.Edit' }
  },
  {
    path: 'customers/:id',
    loadComponent: () => import('./customers/customer-detail/customer-detail.component').then(m => m.CustomerDetailComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Customers.View' }
  },
  {
    path: 'main-account-codes',
    loadComponent: () => import('./main-account-codes/main-account-code-list/main-account-code-list.component').then(m => m.MainAccountCodeListComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'MainAccountCodes.View' }
  },
  {
    path: 'main-account-codes/new',
    loadComponent: () => import('./main-account-codes/main-account-code-form/main-account-code-form.component').then(m => m.MainAccountCodeFormComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'MainAccountCodes.Edit' }
  },
  {
    path: 'main-account-codes/:id/edit',
    loadComponent: () => import('./main-account-codes/main-account-code-form/main-account-code-form.component').then(m => m.MainAccountCodeFormComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'MainAccountCodes.Edit' }
  },
  {
    path: 'payments',
    loadComponent: () => import('./account-payments/account-payment-list/account-payment-list.component').then(m => m.AccountPaymentListComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Payments.View' }
  },
  {
    path: 'payments/new',
    loadComponent: () => import('./account-payments/account-payment-form/account-payment-form.component').then(m => m.AccountPaymentFormComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Payments.Create' }
  },
  {
    path: 'cash-registers',
    loadComponent: () => import('./cash-registers/cash-register-list/cash-register-list.component').then(m => m.CashRegisterListComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Cash.View' }
  },
  {
    path: 'cash-registers/new',
    loadComponent: () => import('./cash-registers/cash-register-form/cash-register-form.component').then(m => m.CashRegisterFormComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Cash.Edit' }
  },
  {
    path: 'cash-registers/:id/edit',
    loadComponent: () => import('./cash-registers/cash-register-form/cash-register-form.component').then(m => m.CashRegisterFormComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Cash.Edit' }
  },
  {
    path: 'cash-registers/:id',
    loadComponent: () => import('./cash-registers/cash-register-detail/cash-register-detail.component').then(m => m.CashRegisterDetailComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Cash.View' }
  },
  {
    path: 'bank-accounts',
    loadComponent: () => import('./bank-accounts/bank-account-list/bank-account-list.component').then(m => m.BankAccountListComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Banking.View' }
  },
  {
    path: 'bank-accounts/new',
    loadComponent: () => import('./bank-accounts/bank-account-form/bank-account-form.component').then(m => m.BankAccountFormComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Banking.Edit' }
  },
  {
    path: 'bank-accounts/:id/edit',
    loadComponent: () => import('./bank-accounts/bank-account-form/bank-account-form.component').then(m => m.BankAccountFormComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Banking.Edit' }
  },
  {
    path: 'bank-accounts/:id',
    loadComponent: () => import('./bank-accounts/bank-account-detail/bank-account-detail.component').then(m => m.BankAccountDetailComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Banking.View' }
  },
  {
    path: 'cheques',
    loadComponent: () => import('./cheques/cheque-list/cheque-list.component').then(m => m.ChequeListComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Cheques.View' }
  },
  {
    path: 'cheques/new',
    loadComponent: () => import('./cheques/cheque-form/cheque-form.component').then(m => m.ChequeFormComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Cheques.Edit' }
  },
  {
    path: 'cheques/:id',
    loadComponent: () => import('./cheques/cheque-detail/cheque-detail.component').then(m => m.ChequeDetailComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Cheques.View' }
  },
  {
    path: 'cheques/:id/edit',
    loadComponent: () => import('./cheques/cheque-form/cheque-form.component').then(m => m.ChequeFormComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Cheques.Edit' }
  },
  {
    path: 'reports',
    loadComponent: () => import('./reports/reports.component').then(m => m.ReportsComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Reports.View' }
  },

  {
    path: 'products',
    loadComponent: () => import('./products/product-list/product-list.component').then(m => m.ProductListComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Products.View' }
  },
  {
    path: 'products/new',
    loadComponent: () => import('./products/product-form/product-form.component').then(m => m.ProductFormComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Products.Edit' }
  },
  {
    path: 'products/:id/edit',
    loadComponent: () => import('./products/product-form/product-form.component').then(m => m.ProductFormComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Products.Edit' }
  },
  {
    path: 'products/:id',
    loadComponent: () => import('./products/product-detail/product-detail.component').then(m => m.ProductDetailComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Products.View' }
  },

  {
    path: 'lookups',
    loadComponent: () => import('./lookups/lookup-list/lookup-list.component').then(m => m.LookupListComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Lookups.View' }
  },
  {
    path: 'company/profile',
    loadComponent: () => import('./firms/firm-detail/firm-detail.component').then(m => m.FirmDetailComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'CompanyProfile.View' }
  },
  {
    path: 'company/profile/edit',
    loadComponent: () => import('./company/company-settings/company-settings.component').then(m => m.CompanySettingsComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'CompanyProfile.Edit' }
  },
  {
    path: 'logs',
    loadComponent: () => import('./reports/log-list/log-list.component').then(m => m.LogListComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Logs.View' }
  },
  {
    path: 'permissions',
    loadComponent: () => import('./auth/permission-management/permission-management.component').then(m => m.PermissionManagementComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Roles.View' }
  },
  {
    path: 'menus',
    loadComponent: () => import('./auth/menu-management/menu-management.component').then(m => m.MenuManagementComponent),
    canActivate: [authGuard, PermissionGuard], data: { permission: 'Menus.View' }
  },
  {
    path: 'material-icons',
    loadComponent: () => import('./auth/material-icons/material-icon-list.component').then(m => m.MaterialIconListComponent),
    canActivate: [authGuard, superAdminGuard]
  },

  { path: 'firms', loadComponent: () => import('./firms/firm-list/firm-list.component').then(m => m.FirmListComponent), canActivate: [authGuard, superAdminGuard] },
  { path: 'firms/new', loadComponent: () => import('./firms/firm-form/firm-form.component').then(m => m.FirmFormComponent), canActivate: [authGuard, superAdminGuard] },
  { path: 'firms/:id', loadComponent: () => import('./firms/firm-detail/firm-detail.component').then(m => m.FirmDetailComponent), canActivate: [authGuard] },

  { path: 'employees', loadComponent: () => import('./employees/employee-list/employee-list.component').then(m => m.EmployeeListComponent), canActivate: [authGuard, notSuperAdminGuard, PermissionGuard], data: { permission: 'Employees.View' } },
  { path: 'employees/new', loadComponent: () => import('./employees/employee-form/employee-form.component').then(m => m.EmployeeFormComponent), canActivate: [authGuard, notSuperAdminGuard, PermissionGuard], data: { permission: 'Employees.Edit' } },
  { path: 'employees/:id/edit', loadComponent: () => import('./employees/employee-form/employee-form.component').then(m => m.EmployeeFormComponent), canActivate: [authGuard, PermissionGuard], data: { permission: 'Employees.Edit' } },

  { path: 'dashboard', loadComponent: () => import('./dashboard/dashboard.component').then(m => m.DashboardComponent), canActivate: [authGuard] },
  {
    path: '**',
    loadComponent: () => import('./not-found/not-found.component').then(m => m.NotFoundComponent)
  }
];
