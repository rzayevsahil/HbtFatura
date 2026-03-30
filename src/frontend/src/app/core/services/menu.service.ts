import { Injectable, signal, effect } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthService } from './auth.service';
import { Observable, of, tap } from 'rxjs';

import { MenuItem } from '../models';

@Injectable({ providedIn: 'root' })
export class MenuService {
    private menuItems = signal<MenuItem[]>([]);
    menu = this.menuItems.asReadonly();

    constructor(private http: HttpClient, private auth: AuthService) {
        // Re-fetch menu when user logs in
        effect(() => {
            if (this.auth.isAuthenticated()) {
                this.fetchMenu().subscribe();
            } else {
                this.menuItems.set([]);
            }
        }, { allowSignalWrites: true });
    }

    fetchMenu(): Observable<MenuItem[]> {
        return this.http.get<MenuItem[]>('/api/menu').pipe(
            tap(items => this.menuItems.set(this.filterSidebarForRole(items)))
        );
    }

    /** SuperAdmin: Personel menüsü gizli; çalışan yönetimi yalnızca firma detayından. */
    private filterSidebarForRole(items: MenuItem[]): MenuItem[] {
        const role = this.auth.user()?.role;
        return items
            .map(item => ({
                ...item,
                children: item.children?.length ? this.filterSidebarForRole(item.children) : item.children
            }))
            .filter(item => !(role === 'SuperAdmin' && item.routerLink === '/employees'));
    }

    // Management APIs
    getAllMenus(): Observable<MenuItem[]> {
        return this.http.get<MenuItem[]>('/api/menu/all');
    }

    createMenu(menu: Partial<MenuItem>): Observable<MenuItem> {
        return this.http.post<MenuItem>('/api/menu', menu);
    }

    updateMenu(id: string, menu: Partial<MenuItem>): Observable<void> {
        return this.http.put<void>(`/api/menu/${id}`, menu);
    }

    deleteMenu(id: string): Observable<void> {
        return this.http.delete<void>(`/api/menu/${id}`);
    }

    reorderMenus(items: { id: string; parentId?: string; sortOrder: number }[]): Observable<void> {
        return this.http.put<void>('/api/menu/reorder', items);
    }
}
