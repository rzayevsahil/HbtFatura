import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * Prevents SuperAdmin from accessing firm-specific business pages (Invoices, Customers, etc.)
 */
export const firmBusinessGuard: CanActivateFn = () => {
    const auth = inject(AuthService);
    const router = inject(Router);

    if (!auth.isAuthenticated()) {
        router.navigate(['/login']);
        return false;
    }

    if (auth.user()?.role !== 'SuperAdmin') {
        return true;
    }

    // If it's SuperAdmin, redirect to their home page
    router.navigate(['/firms']);
    return false;
};
