import { Injectable, signal, computed } from '@angular/core';
import { ApiService } from './api.service';
import { Observable, forkJoin, map, tap } from 'rxjs';

import { LookupGroupDto, LookupDto } from '../models';
import { FALLBACK_DEFAULT_VAT_RATE_PERCENT } from '../constants/vat-defaults';

@Injectable({ providedIn: 'root' })
export class LookupService {
    private lookups = signal<LookupDto[]>([]);
    private groups = signal<LookupGroupDto[]>([]);

    /** Sunucudaki sistem varsayılanı (VatRate lookup / App:DefaultVatRate). */
    readonly defaultVatRatePercent = signal(FALLBACK_DEFAULT_VAT_RATE_PERCENT);

    constructor(private api: ApiService) { }

    load(): Observable<LookupDto[]> {
        return this.api.get<LookupDto[]>('/api/lookups').pipe(
            tap(list => this.lookups.set(list))
        );
    }

    loadGroups(): Observable<LookupGroupDto[]> {
        return this.api.get<LookupGroupDto[]>('/api/lookups/groups').pipe(
            tap(list => this.groups.set(list))
        );
    }

    getDefaultVatRate(): Observable<number> {
        return this.api.get<{ defaultVatRate: number }>('/api/lookups/default-vat-rate').pipe(
            map(r => {
                const v = Number(r?.defaultVatRate);
                return Number.isFinite(v) && v >= 0 && v <= 100 ? v : FALLBACK_DEFAULT_VAT_RATE_PERCENT;
            })
        );
    }

    loadLookupsAndDefaultVat(): Observable<{ defaultVat: number }> {
        return forkJoin({
            _lookups: this.load(),
            defaultVat: this.getDefaultVatRate()
        }).pipe(
            tap(({ defaultVat }) => this.defaultVatRatePercent.set(defaultVat))
        );
    }

    getGroups() {
        return this.groups;
    }

    getGroup(groupName: string) {
        return computed(() => this.lookups().filter(x => x.group?.name === groupName && x.isActive));
    }

    getName(groupName: string, code: string | number | undefined): string {
        if (code === undefined || code === null) return '';
        const c = String(code);
        const item = this.lookups().find(x => x.group?.name === groupName && x.code === c);
        return item?.name ?? c;
    }

    getColor(groupName: string, code: string | number | undefined): string {
        if (code === undefined || code === null) return '';
        const c = String(code);
        const item = this.lookups().find(x => x.group?.name === groupName && x.code === c);
        return item?.color ?? '';
    }

    create(lookup: Partial<LookupDto>): Observable<LookupDto> {
        return this.api.post<LookupDto>('/api/lookups', lookup).pipe(
            tap(() => this.load().subscribe())
        );
    }

    update(id: string, lookup: Partial<LookupDto>): Observable<LookupDto> {
        return this.api.put<LookupDto>(`/api/lookups/${id}`, lookup).pipe(
            tap(() => this.load().subscribe())
        );
    }

    delete(id: string): Observable<void> {
        return this.api.delete<void>(`/api/lookups/${id}`).pipe(
            tap(() => this.load().subscribe())
        );
    }
}
