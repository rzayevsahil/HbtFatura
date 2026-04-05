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
    private readonly activityDescCache = new Map<string, string>();

    /** API'de Türkçe varsayılan başlıklar (Log.Action boş/legacy). */
    private static readonly activityTitleSpecialKeys: Record<string, string> = {
        'Sistem Olayı': 'dashboard.activityActions.fallbackSystem',
        'İşlem': 'dashboard.activityActions.fallbackGeneric'
    };

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
        this.langSub = this.translate.onLangChange.subscribe(() => {
            this.activityDescCache.clear();
            this.cdr.markForCheck();
        });
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

        const special = DashboardComponent.activityTitleSpecialKeys[raw];
        if (special) {
            const ts = this.translate.instant(special);
            if (ts !== special) return ts;
        }

        const base = raw.replace(/Async$/i, '');
        const actionKeys = [`dashboard.activityActions.${raw}`, `dashboard.activityActions.${base}`];
        for (const key of actionKeys) {
            const t = this.translate.instant(key);
            if (t && t !== key) return t;
        }

        const legacy = [`dashboard.activityTitles.${raw}`, `dashboard.activityTitles.${base}`];
        for (const key of legacy) {
            const t = this.translate.instant(key);
            if (t && t !== key) return t;
        }

        return raw;
    }

    /** Backend Türkçe log satırlarını i18n anahtarlarına çevirir (tüm diller). */
    private activityDescTx(key: string, params?: Record<string, unknown>): string | null {
        const t = this.translate.instant(key, params);
        return t !== key ? t : null;
    }

    activityDescription(description: string | null | undefined, title: string | null | undefined): string {
        const desc = description ?? '';
        const rawTitle = (title ?? '').trim();
        if (!desc || !rawTitle) return desc;
        const ck = `${this.translate.currentLang}\x1e${rawTitle}\x1e${desc}`;
        const cached = this.activityDescCache.get(ck);
        if (cached !== undefined) return cached;
        const out = this.computeActivityDescription(desc, rawTitle);
        this.activityDescCache.set(ck, out);
        return out;
    }

    private computeActivityDescription(desc: string, rawTitle: string): string {
        // SuperAdmin: "Kullanıcı adı: {log mesajı}". Firma: doğrudan log metni — içinde ':' vardır; asla ilk ':'a kadar kesme.
        const isSuper = this.auth.user()?.role === 'SuperAdmin';
        const prefixMatch = isSuper ? desc.match(/^([^:]+):\s*/u) : null;
        const prefix = prefixMatch?.[1]?.trim() ?? null;
        const rawMsg = isSuper ? desc.replace(/^[^:]+:\s*/u, '') : desc;
        const msg = rawMsg
            .replace(/\u00a0/g, ' ')
            .replace(/\r\n|\r|\n/g, ' ')
            .replace(/[ \t\f\v]+/g, ' ')
            .trim();

        const withPrefix = (translatedMsg: string): string =>
            prefix ? `${prefix}: ${translatedMsg}` : translatedMsg;

        const m = (re: RegExp) => {
            const r = msg.match(re);
            return r && r.length >= 2 ? r : null;
        };

        const tryKey = (i18nKey: string, params?: Record<string, unknown>): string | null => {
            const localized = this.activityDescTx(i18nKey, params);
            return localized ? withPrefix(localized) : null;
        };

        // Auth
        {
            const rLoginOk = m(/^Kullanıcı giriş yaptı:\s*(.+)$/i);
            if (rLoginOk) {
                const email = (rLoginOk[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.LoginSignedIn', { email });
                if (out) return out;
            }

            const rLoginFail = m(/^(?:Hatalı|Geçersiz)\s+giriş\s+denemesi:\s*(.+)$/i);
            if (rLoginFail) {
                const email = (rLoginFail[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.LoginFailed', { email });
                if (out) return out;
            }

            const rRegister = m(/^Yeni kullanıcı kaydedildi:\s*(.+?)\s*\((.+)\)\s*$/i);
            if (rRegister) {
                const email = (rRegister[1] ?? '').trim();
                const role = (rRegister[2] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.RegisterNewUser', { email, role });
                if (out) return out;
            }

            if (/^Profil bilgileri güncellendi$/i.test(msg)) {
                const out = tryKey('dashboard.activityDescriptions.UpdateProfile');
                if (out) return out;
            }

            if (/^Kullanıcı şifresi değiştirildi$/i.test(msg)) {
                const out = tryKey('dashboard.activityDescriptions.ChangePassword');
                if (out) return out;
            }
        }

        // Fatura / irsaliye / sipariş (özel kalıplar önce)
        {
            const rSendToGib = m(/^Fatura GİB simülasyon kuyruğuna alındı:\s*(.+)$/i);
            if (rSendToGib) {
                const invoiceNo = (rSendToGib[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.SendToGib', { invoiceNo });
                if (out) return out;
            }

            const rFromDn = m(/^İrsaliyeden fatura oluşturuldu:\s*([^\(]+?)\s*\(İrsaliye:\s*([^)]+?)\)\s*$/i);
            if (rFromDn) {
                const invoiceNo = (rFromDn[1] ?? '').trim();
                const deliveryNoteNo = (rFromDn[2] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.CreateFromDeliveryNote', { invoiceNo, deliveryNoteNo });
                if (out) return out;
            }

            const rFromOrder = m(/^Siparişten irsaliye oluşturuldu:\s*([^\(]+?)\s*\(Sipariş:\s*([^)]+?)\)\s*$/i);
            if (rFromOrder) {
                const deliveryNoteNo = (rFromOrder[1] ?? '').trim();
                const orderNo = (rFromOrder[2] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.CreateFromOrder', { deliveryNoteNo, orderNo });
                if (out) return out;
            }

            const rSetInv = m(/^Fatura durumu değişti:\s*(.+?)\s*->\s*(.+)$/i);
            if (rSetInv) {
                const invoiceNo = (rSetInv[1] ?? '').trim();
                const status = (rSetInv[2] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.SetStatusInvoice', { invoiceNo, status });
                if (out) return out;
            }

            const rSetDn = m(/^İrsaliye durumu değişti:\s*(.+?)\s*->\s*(.+)$/i);
            if (rSetDn) {
                const deliveryNoteNo = (rSetDn[1] ?? '').trim();
                const status = (rSetDn[2] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.SetStatusDeliveryNote', { deliveryNoteNo, status });
                if (out) return out;
            }

            const rSetOrder = m(/^Sipariş durumu değişti:\s*(.+?)\s*->\s*(.+)$/i);
            if (rSetOrder) {
                const orderNo = (rSetOrder[1] ?? '').trim();
                const status = (rSetOrder[2] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.SetStatusOrder', { orderNo, status });
                if (out) return out;
            }

            const rInvCreate = m(/^Fatura oluşturuldu:\s*(.+)$/i);
            if (rInvCreate) {
                const invoiceNo = (rInvCreate[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.InvoiceCreated', { invoiceNo });
                if (out) return out;
            }

            const rInvUpd = m(/^Fatura güncellendi:\s*(.+)$/i);
            if (rInvUpd) {
                const invoiceNo = (rInvUpd[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.InvoiceUpdated', { invoiceNo });
                if (out) return out;
            }

            const rDnCreate = m(/^İrsaliye oluşturuldu:\s*(.+)$/i);
            if (rDnCreate) {
                const deliveryNoteNo = (rDnCreate[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.DeliveryNoteCreated', { deliveryNoteNo });
                if (out) return out;
            }

            const rDnUpd = m(/^İrsaliye güncellendi:\s*(.+)$/i);
            if (rDnUpd) {
                const deliveryNoteNo = (rDnUpd[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.DeliveryNoteUpdated', { deliveryNoteNo });
                if (out) return out;
            }

            const rOrdCreate = m(/^Sipariş oluşturuldu:\s*(.+)$/i);
            if (rOrdCreate) {
                const orderNo = (rOrdCreate[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.OrderCreated', { orderNo });
                if (out) return out;
            }

            const rOrdUpd = m(/^Sipariş güncellendi:\s*(.+)$/i);
            if (rOrdUpd) {
                const orderNo = (rOrdUpd[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.OrderUpdated', { orderNo });
                if (out) return out;
            }
        }

        // GİB simülasyon
        {
            const rGibOk = m(/^GİB simülasyonu onayı:\s*(.+)$/i);
            if (rGibOk) {
                const invoiceNo = (rGibOk[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.GibSimulationAccept', { invoiceNo });
                if (out) return out;
            }

            const rGibRej = m(/^GİB simülasyonu ret:\s*(.+)$/i);
            if (rGibRej) {
                const invoiceNo = (rGibRej[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.GibSimulationReject', { invoiceNo });
                if (out) return out;
            }
        }

        // Ana hesap kodu (em/en tire veya düz tire; DB/font farklarına tolerans)
        {
            const splitMainAccountRest = (rest: string): { code: string; name: string } | null => {
                const trimmed = rest.trim();
                const seps = [' — ', ' – ', ' - ', '\u2014', '\u2013', '—', '–', '-'];
                for (const s of seps) {
                    const i = trimmed.indexOf(s);
                    if (i > 0) {
                        const code = trimmed.slice(0, i).trim();
                        const name = trimmed.slice(i + s.length).trim();
                        if (code && name) return { code, name };
                    }
                }
                return null;
            };

            const tryMain = (prefix: string, i18nKey: string): string | null => {
                if (!msg.toLowerCase().startsWith(prefix.toLowerCase())) return null;
                const rest = msg.slice(prefix.length).trim();
                const parts = splitMainAccountRest(rest);
                if (!parts) return null;
                return tryKey(i18nKey, { code: parts.code, name: parts.name });
            };

            const outMacC =
                tryMain('Ana hesap kodu oluşturuldu:', 'dashboard.activityDescriptions.MainAccountCodeCreated') ??
                (() => {
                    const rMacC = m(/^Ana hesap kodu oluşturuldu:\s*(.+?)\s*[—–-]\s*(.+)$/i);
                    if (!rMacC) return null;
                    return tryKey('dashboard.activityDescriptions.MainAccountCodeCreated', {
                        code: (rMacC[1] ?? '').trim(),
                        name: (rMacC[2] ?? '').trim()
                    });
                })();
            if (outMacC) return outMacC;

            const outMacU =
                tryMain('Ana hesap kodu güncellendi:', 'dashboard.activityDescriptions.MainAccountCodeUpdated') ??
                (() => {
                    const rMacU = m(/^Ana hesap kodu güncellendi:\s*(.+?)\s*[—–-]\s*(.+)$/i);
                    if (!rMacU) return null;
                    return tryKey('dashboard.activityDescriptions.MainAccountCodeUpdated', {
                        code: (rMacU[1] ?? '').trim(),
                        name: (rMacU[2] ?? '').trim()
                    });
                })();
            if (outMacU) return outMacU;

            const outMacD =
                tryMain('Ana hesap kodu silindi:', 'dashboard.activityDescriptions.MainAccountCodeDeleted') ??
                (() => {
                    const rMacD = m(/^Ana hesap kodu silindi:\s*(.+?)\s*[—–-]\s*(.+)$/i);
                    if (!rMacD) return null;
                    return tryKey('dashboard.activityDescriptions.MainAccountCodeDeleted', {
                        code: (rMacD[1] ?? '').trim(),
                        name: (rMacD[2] ?? '').trim()
                    });
                })();
            if (outMacD) return outMacD;
        }

        // Çek / senet
        {
            const rChk = m(/^Çek\s+portföye\s+eklendi:\s*(.+)$/i);
            if (rChk) {
                const portfolioNo = (rChk[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.ChequePortfolioAdded', { portfolioNo });
                if (out) return out;
            }

            const rSen = m(/^Senet\s+portföye\s+eklendi:\s*(.+)$/i);
            if (rSen) {
                const portfolioNo = (rSen[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.PromissoryPortfolioAdded', { portfolioNo });
                if (out) return out;
            }

            const rChkSt = m(/^Çek\/senet\s+durumu\s+güncellendi:\s*(.+?)\s*[→>]\s*(.+)$/i);
            if (rChkSt) {
                const portfolio = (rChkSt[1] ?? '').trim();
                const status = (rChkSt[2] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.ChequePromissoryStatusUpdated', { portfolio, status });
                if (out) return out;
            }

            const rChkDel = m(/^Çek\/senet\s+silindi:\s*(.+)$/i);
            if (rChkDel) {
                const portfolio = (rChkDel[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.ChequePromissoryDeleted', { portfolio });
                if (out) return out;
            }
        }

        // Kasa
        {
            const rCashC = m(/^Kasa tanımlandı:\s*(.+)$/i);
            if (rCashC) {
                const name = (rCashC[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.CashRegisterCreated', { name });
                if (out) return out;
            }

            const rCashU = m(/^Kasa güncellendi:\s*(.+)$/i);
            if (rCashU) {
                const name = (rCashU[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.CashRegisterUpdated', { name });
                if (out) return out;
            }

            const rCashD = m(/^Kasa silindi:\s*(.+)$/i);
            if (rCashD) {
                const name = (rCashD[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.CashRegisterDeleted', { name });
                if (out) return out;
            }

            const rCashTx = m(/^Kasaya manuel işlem eklendi:\s*(.+?)\s+TL\s*\((\d+)\)\s*$/i);
            if (rCashTx) {
                const amount = (rCashTx[1] ?? '').trim();
                const typeNum = (rCashTx[2] ?? '').trim();
                const typeKey =
                    typeNum === '1'
                        ? 'dashboard.activityDescriptions.CashBankTxnTypeIn'
                        : typeNum === '2'
                          ? 'dashboard.activityDescriptions.CashBankTxnTypeOut'
                          : '';
                const typeLabel = typeKey ? this.activityDescTx(typeKey) ?? typeNum : typeNum;
                const out = tryKey('dashboard.activityDescriptions.CashManualTransaction', { amount, typeLabel });
                if (out) return out;
            }
        }

        // Tahsilat / ödeme (cari)
        {
            const rTah = m(/^Tahsilat\s+işlemi\s+yapıldı:\s*(.+)$/i);
            if (rTah) {
                const amount = (rTah[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.AccountPaymentTahsilat', { amount });
                if (out) return out;
            }

            const rOde = m(/^Odeme\s+işlemi\s+yapıldı:\s*(.+)$/i);
            if (rOde) {
                const amount = (rOde[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.AccountPaymentOdeme', { amount });
                if (out) return out;
            }

            const rOdeTr = m(/^Ödeme\s+işlemi\s+yapıldı:\s*(.+)$/i);
            if (rOdeTr) {
                const amount = (rOdeTr[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.AccountPaymentOdeme', { amount });
                if (out) return out;
            }
        }

        // Cari
        {
            const rCustC = m(/^Cari kart oluşturuldu:\s*(.+)$/i);
            if (rCustC) {
                const title = (rCustC[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.CustomerCreated', { title });
                if (out) return out;
            }

            const rCustU = m(/^Cari kart güncellendi:\s*(.+)$/i);
            if (rCustU) {
                const title = (rCustU[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.CustomerUpdated', { title });
                if (out) return out;
            }

            const rCustD = m(/^Cari kart silindi:\s*(.+)$/i);
            if (rCustD) {
                const title = (rCustD[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.CustomerDeleted', { title });
                if (out) return out;
            }
        }

        // Ürün
        {
            const rPrC = m(/^Ürün kartı oluşturuldu:\s*(.+?)\s*\(([^)]+)\)\s*$/i);
            if (rPrC) {
                const name = (rPrC[1] ?? '').trim();
                const code = (rPrC[2] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.ProductCreated', { name, code });
                if (out) return out;
            }

            const rPrU = m(/^Ürün kartı güncellendi:\s*(.+?)\s*\(([^)]+)\)\s*$/i);
            if (rPrU) {
                const name = (rPrU[1] ?? '').trim();
                const code = (rPrU[2] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.ProductUpdated', { name, code });
                if (out) return out;
            }

            const rPrD = m(/^Ürün kartı silindi:\s*(.+?)\s*\(([^)]+)\)\s*$/i);
            if (rPrD) {
                const name = (rPrD[1] ?? '').trim();
                const code = (rPrD[2] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.ProductDeleted', { name, code });
                if (out) return out;
            }

            const rStk = m(/^Manuel stok hareketi:\s*(.+?)\s*[—–-]\s*(Giriş|Çıkış)\s+(.+)$/i);
            if (rStk) {
                const code = (rStk[1] ?? '').trim();
                const dirTr = (rStk[2] ?? '').trim();
                const quantity = (rStk[3] ?? '').trim();
                const dirKey =
                    dirTr === 'Giriş'
                        ? 'dashboard.activityDescriptions.StockDirIn'
                        : dirTr === 'Çıkış'
                          ? 'dashboard.activityDescriptions.StockDirOut'
                          : '';
                const direction = dirKey ? this.activityDescTx(dirKey) ?? dirTr : dirTr;
                const out = tryKey('dashboard.activityDescriptions.ManualStockMovement', { code, direction, quantity });
                if (out) return out;
            }
        }

        // Lookup / yetki / menü / firma / personel / banka
        {
            const rLkC = m(/^Lookup oluşturuldu:\s*(.+?)\s*[—–-]\s*(.+)$/i);
            if (rLkC) {
                const code = (rLkC[1] ?? '').trim();
                const name = (rLkC[2] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.LookupCreated', { code, name });
                if (out) return out;
            }

            const rLkV = m(/^KDV oranı \(lookup\) güncellendi:\s*(.+)$/i);
            if (rLkV) {
                const code = (rLkV[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.LookupVatUpdated', { code });
                if (out) return out;
            }

            const rLkU = m(/^Lookup güncellendi:\s*(.+?)\s*[—–-]\s*(.+)$/i);
            if (rLkU) {
                const code = (rLkU[1] ?? '').trim();
                const name = (rLkU[2] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.LookupUpdated', { code, name });
                if (out) return out;
            }

            const rLkD = m(/^Lookup silindi:\s*(.+?)\s*[—–-]\s*(.+)$/i);
            if (rLkD) {
                const code = (rLkD[1] ?? '').trim();
                const name = (rLkD[2] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.LookupDeleted', { code, name });
                if (out) return out;
            }

            const rPermC = m(/^Yetki oluşturuldu:\s*(.+?)\s*[—–-]\s*(.+)$/i);
            if (rPermC) {
                const code = (rPermC[1] ?? '').trim();
                const name = (rPermC[2] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.PermissionCreated', { code, name });
                if (out) return out;
            }

            const rPermU = m(/^Yetki güncellendi:\s*(.+?)\s*[—–-]\s*(.+)$/i);
            if (rPermU) {
                const code = (rPermU[1] ?? '').trim();
                const name = (rPermU[2] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.PermissionUpdated', { code, name });
                if (out) return out;
            }

            const rPermD = m(/^Yetki silindi:\s*(.+?)\s*[—–-]\s*(.+)$/i);
            if (rPermD) {
                const code = (rPermD[1] ?? '').trim();
                const name = (rPermD[2] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.PermissionDeleted', { code, name });
                if (out) return out;
            }

            const rRole = m(/^Rol yetkileri güncellendi:\s*(.+)$/i);
            if (rRole) {
                const roleName = (rRole[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.RolePermissionsUpdated', { roleName });
                if (out) return out;
            }

            const rMenuC = m(/^Menü öğesi oluşturuldu:\s*(.+)$/i);
            if (rMenuC) {
                const label = (rMenuC[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.MenuItemCreated', { label });
                if (out) return out;
            }

            const rMenuR = m(/^Menü sıralaması güncellendi\s*\((\d+)\s+kayıt\)\s*$/i);
            if (rMenuR) {
                const count = (rMenuR[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.MenuReorder', { count });
                if (out) return out;
            }

            const rMenuU = m(/^Menü öğesi güncellendi:\s*(.+)$/i);
            if (rMenuU) {
                const label = (rMenuU[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.MenuItemUpdated', { label });
                if (out) return out;
            }

            const rMenuD = m(/^Menü öğesi silindi:\s*(.+)$/i);
            if (rMenuD) {
                const label = (rMenuD[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.MenuItemDeleted', { label });
                if (out) return out;
            }

            const rFirmC = m(/^Firma oluşturuldu:\s*(.+)$/i);
            if (rFirmC) {
                const name = (rFirmC[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.FirmCreated', { name });
                if (out) return out;
            }

            const rFirmU = m(/^Firma güncellendi:\s*(.+)$/i);
            if (rFirmU) {
                const name = (rFirmU[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.FirmUpdated', { name });
                if (out) return out;
            }

            const rEmpC = m(/^Personel oluşturuldu:\s*(.+?)\s*\(([^)]+)\)\s*$/i);
            if (rEmpC) {
                const fullName = (rEmpC[1] ?? '').trim();
                const email = (rEmpC[2] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.EmployeeCreated', { fullName, email });
                if (out) return out;
            }

            const rEmpU = m(/^Personel güncellendi:\s*(.+?)\s*\(([^)]+)\)\s*$/i);
            if (rEmpU) {
                const fullName = (rEmpU[1] ?? '').trim();
                const email = (rEmpU[2] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.EmployeeUpdated', { fullName, email });
                if (out) return out;
            }

            const rEmpD = m(/^Personel silindi:\s*(.+?)\s*\(([^)]+)\)\s*$/i);
            if (rEmpD) {
                const fullName = (rEmpD[1] ?? '').trim();
                const email = (rEmpD[2] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.EmployeeDeleted', { fullName, email });
                if (out) return out;
            }

            const rBankC = m(/^Banka hesabı oluşturuldu:\s*(.+)$/i);
            if (rBankC) {
                const name = (rBankC[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.BankAccountCreated', { name });
                if (out) return out;
            }

            const rBankU = m(/^Banka hesabı güncellendi:\s*(.+)$/i);
            if (rBankU) {
                const name = (rBankU[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.BankAccountUpdated', { name });
                if (out) return out;
            }

            const rBankD = m(/^Banka hesabı silindi:\s*(.+)$/i);
            if (rBankD) {
                const name = (rBankD[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.BankAccountDeleted', { name });
                if (out) return out;
            }

            const rBankTx = m(/^Bankaya manuel işlem eklendi:\s*(.+?)\s+TL\s*\((\d+)\)\s*$/i);
            if (rBankTx) {
                const amount = (rBankTx[1] ?? '').trim();
                const typeNum = (rBankTx[2] ?? '').trim();
                const typeKey =
                    typeNum === '1'
                        ? 'dashboard.activityDescriptions.CashBankTxnTypeIn'
                        : typeNum === '2'
                          ? 'dashboard.activityDescriptions.CashBankTxnTypeOut'
                          : '';
                const typeLabel = typeKey ? this.activityDescTx(typeKey) ?? typeNum : typeNum;
                const out = tryKey('dashboard.activityDescriptions.BankManualTransaction', { amount, typeLabel });
                if (out) return out;
            }

            const rCompC = m(/^Şirket ayarları oluşturuldu:\s*(.+)$/i);
            if (rCompC) {
                const title = (rCompC[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.CompanySettingsCreated', { title });
                if (out) return out;
            }

            const rCompU = m(/^Şirket ayarları güncellendi:\s*(.+)$/i);
            if (rCompU) {
                const title = (rCompU[1] ?? '').trim();
                const out = tryKey('dashboard.activityDescriptions.CompanySettingsUpdated', { title });
                if (out) return out;
            }
        }

        // İngilizce / legacy parça değişimi
        const replaceMap: Array<{ from: string; toKey: string }> = [
            { from: 'Sent to GİB', toKey: 'dashboard.activityActions.SendToGib' },
            { from: 'Invoice created from delivery note', toKey: 'dashboard.activityActions.CreateFromDeliveryNote' },
            { from: 'Status updated', toKey: 'dashboard.activityActions.SetStatus' }
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
                this.activityDescCache.clear();
                this.data.set(res);
                this.loading.set(false);
            },
            error: () => this.loading.set(false)
        });
    }
}
