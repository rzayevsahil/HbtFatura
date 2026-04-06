import { Injectable, signal, computed } from '@angular/core';
import { ApiService } from './api.service';
import { Observable, forkJoin, map, tap } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';

import { LookupGroupDto, LookupDto } from '../models';
import { FALLBACK_DEFAULT_VAT_RATE_PERCENT } from '../constants/vat-defaults';

@Injectable({ providedIn: 'root' })
export class LookupService {
    private lookups = signal<LookupDto[]>([]);
    private groups = signal<LookupGroupDto[]>([]);

    /** Dil değişince abone olan görünümlerin güncellenmesi için (getName / display* ile okunur). */
    private readonly langRev = signal(0);

    /** Sunucudaki sistem varsayılanı (VatRate lookup / App:DefaultVatRate). */
    readonly defaultVatRatePercent = signal(FALLBACK_DEFAULT_VAT_RATE_PERCENT);

    constructor(
        private api: ApiService,
        private translate: TranslateService
    ) {
        this.translate.onLangChange.subscribe(() => this.langRev.update((n) => n + 1));
    }

    private preferEnglish(): boolean {
        this.langRev();
        const l = (this.translate.currentLang ?? 'tr').toLowerCase();
        return l === 'en' || l.startsWith('en-');
    }

    /** Grup başlığı: EN dilinde DisplayNameEn, yoksa Türkçe DisplayName. */
    displayGroupLabel(group: LookupGroupDto | undefined | null): string {
        if (!group) return '';
        if (this.preferEnglish()) {
            const en = group.displayNameEn?.trim();
            if (en) return en;
        }
        return group.displayName || group.name;
    }

    /** Tanım satırı adı: EN dilinde NameEn, yoksa Name (TR). */
    displayLookupLabel(item: LookupDto | undefined | null): string {
        if (!item) return '';
        if (this.preferEnglish()) {
            const en = item.nameEn?.trim();
            if (en) return en;
        }
        return item.name ?? '';
    }

    /**
     * Ürün birimi (ProductUnit): `products.units.<code>` çevirisi varsa kullanılır;
     * yoksa tanımın Name/NameEn değeri (displayLookupLabel).
     */
    displayProductUnitLabel(item: LookupDto | undefined | null): string {
        this.langRev();
        if (!item) return '';
        const code = (item.code ?? '').trim();
        if (!code) return this.displayLookupLabel(item);
        const key = 'products.units.' + code;
        const t = this.translate.instant(key);
        if (t && t !== key) return t;
        return this.displayLookupLabel(item);
    }

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
        this.langRev();
        if (code === undefined || code === null) return '';
        const c = String(code).trim();
        const item = this.lookups().find(x => x.group?.name === groupName && x.code === c);
        if (groupName === 'ProductUnit') {
            if (item) return this.displayProductUnitLabel(item);
            const key = 'products.units.' + c;
            const t = this.translate.instant(key);
            if (t && t !== key) return t;
            return c;
        }
        if (!item) return c;
        return this.displayLookupLabel(item);
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
