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

    activityTitle(title: string | null | undefined): string {
        const raw = (title ?? '').trim();
        if (!raw) return '';

        const base = raw.replace(/Async$/i, '');
        const candidates = [
            `dashboard.activityTitles.${raw}`,
            `dashboard.activityTitles.${base}`
        ];

        for (const key of candidates) {
            const t = this.translate.instant(key);
            if (t && t !== key) return t;
        }

        return raw;
    }

    activityDescription(description: string | null | undefined, title: string | null | undefined): string {
        const desc = description ?? '';
        const rawTitle = (title ?? '').trim();
        if (!desc || !rawTitle) return desc;

        // TR tarafında içerikler backend'den Türkçe geldiği için olduğu gibi bırakıyoruz.
        if (this.translate.currentLang !== 'en') return desc;

        // EN tarafında, backend'in Türkçe log message kalıplarını çeviriyoruz.
        // (SuperAdmin'da description formatı: "System/Anonymous: {Message}" şeklinde geliyor.)
        const stripPrefix = () => desc.replace(/^[^:]+:\s*/u, '');

        const msg = stripPrefix();
        const prefixMatch = desc.match(/^([^:]+):\s*/u);
        const prefix = prefixMatch ? prefixMatch[1] : null;

        const withPrefix = (translatedMsg: string): string =>
            prefix ? `${prefix}: ${translatedMsg}` : translatedMsg;

        const m = (re: RegExp) => {
            const r = msg.match(re);
            return r && r.length >= 2 ? r : null;
        };

        // Auth (super admin sistem hareketleri)
        {
            const rLoginOk = m(/^Kullanıcı giriş yaptı:\s*(.+)$/i);
            if (rLoginOk) {
                const email = (rLoginOk[1] ?? '').trim();
                return withPrefix(this.translate.instant('dashboard.activityDescriptions.LoginSignedIn', { email }));
            }

            const rLoginFail = m(/^Hatalı giriş denemesi:\s*(.+)$/i);
            if (rLoginFail) {
                const email = (rLoginFail[1] ?? '').trim();
                return withPrefix(this.translate.instant('dashboard.activityDescriptions.LoginFailed', { email }));
            }

            const rRegister = m(/^Yeni kullanıcı kaydedildi:\s*(.+?)\s*\((.+)\)\s*$/i);
            if (rRegister) {
                const email = (rRegister[1] ?? '').trim();
                const role = (rRegister[2] ?? '').trim();
                return withPrefix(this.translate.instant('dashboard.activityDescriptions.RegisterNewUser', { email, role }));
            }

            if (/^Profil bilgileri güncellendi$/i.test(msg)) {
                return withPrefix(this.translate.instant('dashboard.activityDescriptions.UpdateProfile'));
            }
        }

        // Invoice/DeliveryNote/Order actions
        {
            const rSendToGib = m(/^Fatura GİB simülasyon kuyruğuna alındı:\s*(.+)$/i);
            if (rSendToGib) {
                const invoiceNo = (rSendToGib[1] ?? '').trim();
                const t = this.translate.instant('dashboard.activityDescriptions.SendToGib', { invoiceNo });
                return withPrefix(t && t !== 'dashboard.activityDescriptions.SendToGib' ? t : msg);
            }

            const rFromDn = m(/^İrsaliyeden fatura oluşturuldu:\s*([^\(]+?)\s*\(İrsaliye:\s*([^)]+?)\)\s*$/i);
            if (rFromDn) {
                const invoiceNo = (rFromDn[1] ?? '').trim();
                const deliveryNoteNo = (rFromDn[2] ?? '').trim();
                const t = this.translate.instant('dashboard.activityDescriptions.CreateFromDeliveryNote', { invoiceNo, deliveryNoteNo });
                return withPrefix(t && t !== 'dashboard.activityDescriptions.CreateFromDeliveryNote' ? t : msg);
            }

            const rFromOrder = m(/^Siparişten irsaliye oluşturuldu:\s*([^\(]+?)\s*\(Sipariş:\s*([^)]+?)\)\s*$/i);
            if (rFromOrder) {
                const deliveryNoteNo = (rFromOrder[1] ?? '').trim();
                const orderNo = (rFromOrder[2] ?? '').trim();
                const t = this.translate.instant('dashboard.activityDescriptions.CreateFromOrder', { deliveryNoteNo, orderNo });
                return withPrefix(t && t !== 'dashboard.activityDescriptions.CreateFromOrder' ? t : msg);
            }

            const rSetInv = m(/^Fatura durumu değişti:\s*(.+?)\s*->\s*(.+)$/i);
            if (rSetInv) {
                const invoiceNo = (rSetInv[1] ?? '').trim();
                const status = (rSetInv[2] ?? '').trim();
                const t = this.translate.instant('dashboard.activityDescriptions.SetStatusInvoice', { invoiceNo, status });
                return withPrefix(t && t !== 'dashboard.activityDescriptions.SetStatusInvoice' ? t : msg);
            }

            const rSetDn = m(/^İrsaliye durumu değişti:\s*(.+?)\s*->\s*(.+)$/i);
            if (rSetDn) {
                const deliveryNoteNo = (rSetDn[1] ?? '').trim();
                const status = (rSetDn[2] ?? '').trim();
                const t = this.translate.instant('dashboard.activityDescriptions.SetStatusDeliveryNote', { deliveryNoteNo, status });
                return withPrefix(t && t !== 'dashboard.activityDescriptions.SetStatusDeliveryNote' ? t : msg);
            }

            const rSetOrder = m(/^Sipariş durumu değişti:\s*(.+?)\s*->\s*(.+)$/i);
            if (rSetOrder) {
                const orderNo = (rSetOrder[1] ?? '').trim();
                const status = (rSetOrder[2] ?? '').trim();
                const t = this.translate.instant('dashboard.activityDescriptions.SetStatusOrder', { orderNo, status });
                return withPrefix(t && t !== 'dashboard.activityDescriptions.SetStatusOrder' ? t : msg);
            }
        }

        // Fallback: eğer backend İngilizce/friendly bir metin bastıysa title içindeki bilinen kalıpları değiştir.
        const replaceMap: Array<{ from: string; toKey: string }> = [
            { from: 'Sent to GİB', toKey: 'dashboard.activityTitles.Sent to GİB' },
            { from: 'Invoice created from delivery note', toKey: 'dashboard.activityTitles.Invoice created from delivery note' },
            { from: 'Status updated', toKey: 'dashboard.activityTitles.Status updated' }
        ];
        let out = desc;
        for (const r of replaceMap) {
            if (!out.includes(r.from)) continue;
            const t = this.translate.instant(r.toKey);
            if (t && t !== r.toKey) out = out.replace(r.from, t);
        }
        return out;
    }

    activityTimeAgo(time: string | null | undefined): string {
        const raw = (time ?? '').trim();
        if (!raw) return '';
        if (this.translate.currentLang !== 'en') return raw;

        if (/^Az\s*önce$/i.test(raw) || /^Az\s+önce$/i.test(raw)) return 'Just now';

        const mMin = raw.match(/^(\d+)\s+dak\.\s+önce$/i);
        if (mMin) {
            const n = Number(mMin[1]);
            return `${n} minute${n === 1 ? '' : 's'} ago`;
        }

        const mHour = raw.match(/^(\d+)\s+saat\s+önce$/i);
        if (mHour) {
            const n = Number(mHour[1]);
            return `${n} hour${n === 1 ? '' : 's'} ago`;
        }

        const mDay = raw.match(/^(\d+)\s+gün\s+önce$/i);
        if (mDay) {
            const n = Number(mDay[1]);
            return `${n} day${n === 1 ? '' : 's'} ago`;
        }

        return raw;
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
