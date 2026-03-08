import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { PermissionService } from '../services/permission.service';
import { ToastrService } from 'ngx-toastr';

@Injectable({ providedIn: 'root' })
export class PermissionGuard implements CanActivate {
    constructor(
        private permService: PermissionService,
        private router: Router,
        private toastr: ToastrService
    ) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
        const requiredPermission = route.data['permission'] as string;

        if (!requiredPermission || this.permService.hasPermission(requiredPermission)) {
            return true;
        }

        this.toastr.error('Bu sayfaya erişim yetkiniz bulunmamaktadır.', 'Yetkisiz Erişim');
        this.router.navigate(['/']);
        return false;
    }
}
