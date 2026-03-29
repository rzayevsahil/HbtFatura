import { Component, effect } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Subscription, interval } from 'rxjs';
import { AuthService } from './core/services/auth.service';
import { LookupService } from './core/services/lookup.service';
import { MenuService } from './core/services/menu.service';
import { NotificationService, UserNotificationDto } from './services/notification.service';
import { ThemeService } from './core/services/theme.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  headerMenuOpen = false;
  notifOpen = false;
  notifItems: UserNotificationDto[] = [];
  notifUnread = 0;
  private pollSub?: Subscription;

  constructor(
    public auth: AuthService,
    public theme: ThemeService,
    private lookup: LookupService,
    public menu: MenuService,
    private notifApi: NotificationService,
    private router: Router
  ) {
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
}
