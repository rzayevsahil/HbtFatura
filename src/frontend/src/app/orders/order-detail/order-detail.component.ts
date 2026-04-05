import { Component, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { OrderService } from '../../services/order.service';
import { OrderDto, OrderItemDto, OrderStatus } from '../../core/models';
import { DeliveryNoteService } from '../../services/delivery-note.service';
import { ToastrService } from 'ngx-toastr';
import { LookupService } from '../../core/services/lookup.service';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-order-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslateModule],
  templateUrl: './order-detail.component.html',
  styleUrls: ['./order-detail.component.scss']
})
export class OrderDetailComponent implements OnInit {
  order: OrderDto | null = null;
  loading = true;
  creatingDeliveryNote = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private orderApi: OrderService,
    private deliveryNoteApi: DeliveryNoteService,
    private toastr: ToastrService,
    public lookups: LookupService,
    private translate: TranslateService
  ) { }

  @HostListener('document:keydown', ['$event'])
  onKeyDown(e: KeyboardEvent): void {
    const t = e.target as HTMLElement;
    if (e.key === 'F2' && this.order && this.isEditableOrder() && !['INPUT', 'TEXTAREA', 'SELECT'].includes(t?.tagName) && !t?.closest('app-searchable-select')) {
      e.preventDefault();
      this.router.navigate(['/orders', this.order.id, 'edit']);
    }
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.orderApi.getById(id).subscribe({
        next: o => { this.order = o; this.loading = false; },
        error: () => { this.loading = false; }
      });
    } else {
      this.loading = false;
    }
  }

  /** Backend bazen enum'ı sayı bazen string (örn. "Bekliyor") döner. */
  statusLabel(s: OrderStatus | string | undefined): string {
    return this.lookups.getName('OrderStatus', s);
  }

  isEditableOrder(): boolean {
    return this.order != null && (this.order.status === 0 || this.order.status === 'Bekliyor');
  }

  /** Onaylandı / Kısmi Teslim iken irsaliyeye aktarılabilir. */
  canSendToDeliveryNote(): boolean {
    if (!this.order) return false;
    if (this.order.deliveryNoteId) return false;
    const s = this.order.status;
    return s === 3 || s === 4 || s === 'Onaylandi' || s === 'KismiTeslim';
  }

  typeLabel(t: number): string {
    return this.lookups.getName('OrderType', t);
  }

  get totalNet(): number {
    if (!this.order) return 0;
    return this.order.items.reduce((sum, it) => sum + (it.quantity || 0) * (it.unitPrice || 0), 0);
  }

  get totalVat(): number {
    if (!this.order) return 0;
    return this.order.items.reduce((sum, it) => {
      const net = (it.quantity || 0) * (it.unitPrice || 0);
      const vatRate = it.vatRate || 0;
      return sum + net * vatRate / 100;
    }, 0);
  }

  get totalGross(): number {
    return this.totalNet + this.totalVat;
  }

  /** strictTemplates: @for içinde order daraltması güvenilir olmadığı için şablonda doğrudan kullanılmaz. */
  get orderCurrencyCode(): string {
    const c = this.order?.currency?.trim();
    return c ? c.toUpperCase() : 'TRY';
  }

  lineItemCurrency(it: OrderItemDto): string {
    const line = it.currency?.trim();
    if (line) return line.toUpperCase();
    const doc = this.order?.currency?.trim();
    if (doc) return doc.toUpperCase();
    return 'TRY';
  }

  setStatusOnayla(): void {
    if (!this.order || !this.isEditableOrder()) return;
    this.orderApi.setStatus(this.order.id, 3).subscribe({
      next: () => {
        this.toastr.success(this.translate.instant('orders.toastrApproved'));
        this.ngOnInit();
      },
      error: e => this.toastr.error(e.error?.message ?? this.translate.instant('orders.statusError'))
    });
  }

  setStatusIptal(): void {
    if (!this.order || !this.isEditableOrder()) return;
    if (!confirm(this.translate.instant('common.confirmCancelOrder'))) return;
    this.orderApi.setStatus(this.order.id, 2).subscribe({
      next: () => {
        this.toastr.success(this.translate.instant('orders.toastrCancelled'));
        this.ngOnInit();
      },
      error: e => this.toastr.error(e.error?.message ?? this.translate.instant('orders.statusError'))
    });
  }

  createDeliveryNote(): void {
    if (!this.order || !this.canSendToDeliveryNote()) return;
    this.creatingDeliveryNote = true;
    const d = new Date();
    const pad = (n: number) => n.toString().padStart(2, '0');
    const deliveryDate = `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}:${pad(d.getSeconds())}`;
    this.deliveryNoteApi.createFromOrder({ orderId: this.order.id, deliveryDate }).subscribe({
      next: dn => {
        this.toastr.success(this.translate.instant('orders.toastrDnCreated'));
        this.router.navigate(['/delivery-notes', dn.id]);
      },
      error: e => {
        this.toastr.error(e.error?.message ?? this.translate.instant('orders.toastrDnFailed'));
        this.creatingDeliveryNote = false;
      }
    });
  }

  downloadPdf(): void {
    if (!this.order) return;
    this.orderApi.downloadPdf(this.order.id).subscribe({
      next: blob => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        const number = this.order!.orderNumber || this.order!.id;
        a.download = `siparis-${number}.pdf`;
        a.click();
        window.URL.revokeObjectURL(url);
      }
    });
  }

  goToCustomer(customerId: number | string | undefined): void {
    if (customerId) {
      this.router.navigate(['/customers', customerId]);
    }
  }

  goToDeliveryNote(deliveryNoteId: number | string | undefined): void {
    if (deliveryNoteId) {
      this.router.navigate(['/delivery-notes', deliveryNoteId]);
    }
  }
}
