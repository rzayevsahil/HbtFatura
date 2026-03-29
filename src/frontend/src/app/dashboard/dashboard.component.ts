import { ChangeDetectorRef, Component, OnDestroy, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { Subscription } from 'rxjs';
import { AuthService } from '../core/services/auth.service';
import { DashboardService } from '../services/dashboard.service';
import { DashboardData, DashboardStat } from '../core/models';

@Component({
    selector: 'app-dashboard',
    standalone: true,
    imports: [CommonModule, RouterLink, TranslateModule],
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit, OnDestroy {
    data = signal<DashboardData | null>(null);
    loading = signal(true);
    readonly today = new Date();
    private langSub?: Subscription;

    constructor(
        public auth: AuthService,
        public translate: TranslateService,
        private dashboardService: DashboardService,
        private cdr: ChangeDetectorRef
    ) { }

    get dateLocaleId(): string {
        return this.translate.currentLang === 'en' ? 'en-US' : 'tr';
    }

    ngOnInit(): void {
        this.refresh();
        this.langSub = this.translate.onLangChange.subscribe(() => this.cdr.markForCheck());
    }

    ngOnDestroy(): void {
        this.langSub?.unsubscribe();
    }

    statLabel(stat: DashboardStat): string {
        const k = stat.key?.trim();
        if (k) {
            const t = this.translate.instant(`dashboard.stats.${k}.label`);
            if (t !== `dashboard.stats.${k}.label`) {
                return t;
            }
        }
        return stat.label;
    }

    statValueDisplay(stat: DashboardStat): string {
        const k = stat.key;
        const locale = this.translate.currentLang === 'en' ? 'en-US' : 'tr-TR';
        if (k === 'monthly_sales' || k === 'pending_collection') {
            if (stat.amount != null) {
                return new Intl.NumberFormat(locale, { style: 'currency', currency: 'TRY' }).format(stat.amount);
            }
        }
        if (k === 'total_firms' || k === 'total_users' || k === 'new_firms' || k === 'system_errors') {
            if (stat.count != null) {
                return String(stat.count);
            }
        }
        if (k === 'new_orders' && stat.count != null) {
            return this.translate.instant('dashboard.stats.value.units', { count: stat.count });
        }
        if (k === 'total_customers' && stat.count != null) {
            return this.translate.instant('dashboard.stats.value.records', { count: stat.count });
        }
        return stat.value;
    }

    statTrendDisplay(stat: DashboardStat): string | null {
        const kind = stat.trendKind;
        if (!kind) {
            return stat.trend ?? null;
        }
        if (kind === 'invoice_count' && stat.trendCount != null) {
            return this.translate.instant('dashboard.stats.trend.invoiceCount', { count: stat.trendCount });
        }
        const map: Record<string, string> = {
            this_month: 'dashboard.stats.trend.thisMonth',
            pending: 'dashboard.stats.trend.pending',
            registered: 'dashboard.stats.trend.registered',
            active: 'dashboard.stats.trend.active',
            critical: 'dashboard.stats.trend.critical'
        };
        const tk = map[kind];
        if (!tk) {
            return stat.trend ?? null;
        }
        const t = this.translate.instant(tk);
        return t !== tk ? t : (stat.trend ?? null);
    }

    trendPositive(stat: DashboardStat): boolean {
        const k = stat.trendKind;
        return k === 'this_month' || k === 'registered' || k === 'active';
    }

    trendNegative(stat: DashboardStat): boolean {
        const k = stat.trendKind;
        return k === 'invoice_count' || k === 'pending' || k === 'critical';
    }

    refresh(): void {
        this.loading.set(true);
        this.dashboardService.getSummary().subscribe({
            next: (res) => {
                this.data.set(res);
                this.loading.set(false);
            },
            error: () => this.loading.set(false)
        });
    }
}
