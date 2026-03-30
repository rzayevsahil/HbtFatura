import { Component, computed, effect, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { filter } from 'rxjs/operators';
import { CommonModule } from '@angular/common';
import { Subscription, interval } from 'rxjs';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { AuthService } from './core/services/auth.service';
import { LookupService } from './core/services/lookup.service';
import { MenuService } from './core/services/menu.service';
import { NotificationService, UserNotificationDto } from './services/notification.service';
import { ThemeService } from './core/services/theme.service';
import { LANG_STORAGE_KEY } from './core/i18n/app-shell.init';
import { MenuItem } from './core/models';

/** Düzleştirilmiş sol menü satırı (API ağaç yapısı → sıralı liste). */
interface SidebarNavEntry {
  kind: 'link' | 'group';
  item: MenuItem;
  depth: number;
}

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive, TranslateModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  private static readonly SIDEBAR_STORAGE_KEY = 'hbt-sidebar-collapsed';

  /** API menü `routerLink` → i18n anahtarı */
  private static readonly MENU_LINK_KEYS: Record<string, string> = {
    '/dashboard': 'menu.dashboard',
    '/invoices': 'menu.invoices',
    '/gib-simulation/inbox': 'menu.gibInbox',
    '/orders': 'menu.orders',
    '/delivery-notes': 'menu.deliveryNotes',
    '/products': 'menu.products',
    '/customers': 'menu.customers',
    '/main-account-codes': 'menu.mainAccountCodes',
    '/payments': 'menu.payments',
    '/cash-registers': 'menu.cashRegisters',
    '/bank-accounts': 'menu.bankAccounts',
    '/cheques': 'menu.cheques',
    '/reports': 'menu.reports',
    '/lookups': 'menu.lookups',
    '/permissions': 'menu.permissions',
    '/menus': 'menu.menus',
    '/material-icons': 'menu.materialIcons',
    '/firms': 'menu.firms',
    '/employees': 'menu.employees',
    '/logs': 'menu.logs',
    '/company/profile': 'menu.companyProfile'
  };

  private static readonly MENU_LABEL_FALLBACK: Record<string, string> = {
    'Dashboard': 'menu.dashboard',
    'Faturalar': 'menu.invoices',
    'GİB Kutusu (simülasyon)': 'menu.gibInbox',
    'Siparişler': 'menu.orders',
    'İrsaliyeler': 'menu.deliveryNotes',
    'Ürünler': 'menu.products',
    'Cari Kartlar': 'menu.customers',
    'Hesap Kodları': 'menu.mainAccountCodes',
    'Tahsilat / Ödeme': 'menu.payments',
    'Kasa Yönetimi': 'menu.cashRegisters',
    'Banka Yönetimi': 'menu.bankAccounts',
    'Çek / Senet': 'menu.cheques',
    'Raporlar': 'menu.reports',
    'Sistem Tanımları': 'menu.lookups',
    'Rol ve Yetki Yönetimi': 'menu.permissions',
    'Menü Yönetimi': 'menu.menus',
    'Material İkonları': 'menu.materialIcons',
    'Firma Yönetimi': 'menu.firms',
    'Personel Yönetimi': 'menu.employees',
    'Sistem Logları': 'menu.logs',
    'Şirket Profili': 'menu.companyProfile'
  };

  private readonly urlPath = signal<string>('/');

  /** Header + sidebar: oturum açık olsa bile giriş rotasında chrome gösterme (giriş sırasında titreme önlemi). */
  readonly showAppChrome = computed(
    () => this.auth.isAuthenticated() && this.urlPath() !== '/login'
  );

  /** Sol menü daraltılmış: yalnızca ikonlar */
  sidebarCollapsed = signal(false);

  /** Menü API’sinden gelen ağaç → gruplar ve linkler (alt menüler dahil). */
  readonly sidebarNavEntries = computed(() =>
    AppComponent.flattenSidebarMenu(this.menu.menu(), 0)
  );

  headerMenuOpen = false;
  notifOpen = false;
  notifItems: UserNotificationDto[] = [];
  notifUnread = 0;
  private pollSub?: Subscription;

  constructor(
    public auth: AuthService,
    public theme: ThemeService,
    public translate: TranslateService,
    private lookup: LookupService,
    public menu: MenuService,
    private notifApi: NotificationService,
    private router: Router
  ) {
    this.urlPath.set(AppComponent.pathOnly(this.router.url));
    this.router.events
      .pipe(
        filter((e): e is NavigationEnd => e instanceof NavigationEnd),
        takeUntilDestroyed()
      )
      .subscribe(e => this.urlPath.set(AppComponent.pathOnly(e.urlAfterRedirects)));

    this.lookup.load().subscribe();
    effect(() => {
      if (!this.auth.isAuthenticated()) {
        this.stopNotifPolling();
        this.notifUnread = 0;
        this.notifItems = [];
        this.notifOpen = false;
        return;
      }
      this.startNotifPolling();
    });
  }

  private startNotifPolling(): void {
    this.pollSub?.unsubscribe();
    this.refreshUnreadBadge();
    this.pollSub = interval(45000).subscribe(() => this.refreshUnreadBadge());
  }

  private stopNotifPolling(): void {
    this.pollSub?.unsubscribe();
    this.pollSub = undefined;
  }

  private refreshUnreadBadge(): void {
    this.notifApi.unreadCount().subscribe({
      next: r => this.notifUnread = r.count,
      error: () => { }
    });
  }

  closeHeaderMenu(): void {
    this.headerMenuOpen = false;
  }

  toggleSidebar(): void {
    const next = !this.sidebarCollapsed();
    this.sidebarCollapsed.set(next);
    try {
      localStorage.setItem(AppComponent.SIDEBAR_STORAGE_KEY, next ? '1' : '0');
    } catch {
      /* */
    }
  }

  toggleNotifPanel(): void {
    this.notifOpen = !this.notifOpen;
    if (this.notifOpen) {
      this.notifApi.list(25).subscribe({
        next: list => this.notifItems = list,
        error: () => { }
      });
      this.refreshUnreadBadge();
    }
  }

  closeNotifPanel(): void {
    this.notifOpen = false;
  }

  /** API çoğunlukla timezone’suz ISO döndürür (UTC anı); Z ekleyerek parse edilir. */
  notificationInstant(value: string | Date | undefined | null): Date {
    if (value == null) return new Date(0);
    if (value instanceof Date) return value;
    const s = String(value).trim();
    if (!s) return new Date(0);
    const hasTz = /Z$/i.test(s) || /[+-]\d{2}:\d{2}$/.test(s) || /[+-]\d{4}$/.test(s);
    if (!hasTz && /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(\.\d+)?$/.test(s)) {
      return new Date(s + 'Z');
    }
    return new Date(s);
  }

  notificationTargetUrl(n: UserNotificationDto): string | null {
    switch (n.type) {
      case 'GibInvoiceReceived':
        return this.canViewGibInbox() ? '/gib-simulation/inbox' : null;
      case 'GibInvoiceAccepted':
      case 'GibInvoiceRejected': {
        const rt = (n.referenceType || '').toLowerCase();
        if (n.referenceId && (rt === 'fatura' || rt === 'invoice')) {
          return `/invoices/${n.referenceId}`;
        }
        return null;
      }
      default:
        return null;
    }
  }

  onNotificationClick(n: UserNotificationDto): void {
    if (!n.readAt) {
      this.notifApi.markRead(n.id).subscribe({
        next: () => {
          n.readAt = new Date().toISOString();
          this.refreshUnreadBadge();
        }
      });
    }
    const url = this.notificationTargetUrl(n);
    if (url) {
      this.closeNotifPanel();
      void this.router.navigateByUrl(url);
    }
  }

  markAllNotifsRead(): void {
    this.notifApi.markAllRead().subscribe(() => {
      this.notifItems.forEach(x => { x.readAt = x.readAt ?? new Date().toISOString(); });
      this.notifUnread = 0;
    });
  }

  canViewGibInbox(): boolean {
    return !!this.auth.user()?.permissions?.includes('GibSimulation.ViewInbox');
  }

  private static pathOnly(url: string): string {
    const q = url.split(/[?#]/)[0] ?? '/';
    return q || '/';
  }

  private static flattenSidebarMenu(items: MenuItem[], depth: number): SidebarNavEntry[] {
    const sorted = [...items].filter(i => i.isActive !== false).sort((a, b) => a.sortOrder - b.sortOrder);
    const out: SidebarNavEntry[] = [];
    for (const item of sorted) {
      const children = [...(item.children ?? [])]
        .filter(c => c.isActive !== false)
        .sort((a, b) => a.sortOrder - b.sortOrder);
      if (item.routerLink) {
        out.push({ kind: 'link', item, depth });
      }
      if (children.length > 0) {
        if (!item.routerLink) {
          out.push({ kind: 'group', item, depth });
        }
        out.push(...AppComponent.flattenSidebarMenu(children, depth + 1));
      } else if (!item.routerLink) {
        out.push({ kind: 'group', item, depth });
      }
    }
    return out;
  }

  /** Geniş menüde sol girinti (rem); dar menüde kullanılmaz. */
  sidebarIndentRem(depth: number): number {
    return 1.5 + depth * 0.75;
  }

  /** Angular `DatePipe` yerel ayarı (dil değişince güncellenir) */
  get dateLocaleId(): string {
    return this.translate.currentLang === 'en' ? 'en-US' : 'tr';
  }

  setLang(lang: 'tr' | 'en'): void {
    this.translate.use(lang).subscribe(() => {
      try {
        localStorage.setItem(LANG_STORAGE_KEY, lang);
      } catch {
        /* */
      }
      document.documentElement.lang = lang === 'en' ? 'en' : 'tr';
      this.closeHeaderMenu();
    });
  }

  isLang(lang: 'tr' | 'en'): boolean {
    return this.translate.currentLang === lang;
  }

  menuDisplayLabel(item: MenuItem): string {
    const link = (item.routerLink || '').trim();
    const linkKey = link ? AppComponent.MENU_LINK_KEYS[link] : undefined;
    if (linkKey) {
      const t = this.translate.instant(linkKey);
      if (t !== linkKey) {
        return t;
      }
    }
    const fb = item.label ? AppComponent.MENU_LABEL_FALLBACK[item.label] : undefined;
    if (fb) {
      const t = this.translate.instant(fb);
      if (t !== fb) {
        return t;
      }
    }
    return item.label;
  }
}
