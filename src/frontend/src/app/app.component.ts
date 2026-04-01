import { ApplicationRef, Component, computed, effect, signal } from '@angular/core';
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
import { getMenuDisplayLabel } from './core/i18n/menu-display-label';
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
    private router: Router,
    private appRef: ApplicationRef
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
      this.appRef.tick();
    });
  }

  isLang(lang: 'tr' | 'en'): boolean {
    return this.translate.currentLang === lang;
  }

  menuDisplayLabel(item: MenuItem): string {
    return getMenuDisplayLabel(this.translate, item);
  }
}
