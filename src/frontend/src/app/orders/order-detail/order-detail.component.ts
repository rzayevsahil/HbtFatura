import { Component, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { OrderService } from '../../services/order.service';
import { OrderDto, OrderStatus } from '../../core/models';
import { DeliveryNoteService } from '../../services/delivery-note.service';
import { ToastrService } from 'ngx-toastr';
import { LookupService } from '../../core/services/lookup.service';

@Component({
  selector: 'app-order-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
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
    public lookups: LookupService
  ) { }

  @HostListener('document:keydown', ['$event'])
  onKeyDown(e: KeyboardEvent): void {
    if (e.key === 'F2' && this.order && this.isEditableOrder() && !['INPUT', 'TEXTAREA', 'SELECT'].includes((e.target as HTMLElement)?.tagName)) {
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

  setStatusOnayla(): void {
    if (!this.order || !this.isEditableOrder()) return;
    this.orderApi.setStatus(this.order.id, 3).subscribe({
      next: () => {
        this.toastr.success('Sipariş onaylandı.');
        this.ngOnInit();
      },
      error: e => this.toastr.error(e.error?.message ?? 'Durum güncellenemedi.')
    });
  }

  setStatusIptal(): void {
    if (!this.order || !this.isEditableOrder()) return;
    if (!confirm('Siparişi iptal etmek istediğinize emin misiniz?')) return;
    this.orderApi.setStatus(this.order.id, 2).subscribe({
      next: () => {
        this.toastr.success('Sipariş iptal edildi.');
        this.ngOnInit();
      },
      error: e => this.toastr.error(e.error?.message ?? 'Durum güncellenemedi.')
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
        this.toastr.success('İrsaliye oluşturuldu.');
        this.router.navigate(['/delivery-notes', dn.id]);
      },
      error: e => {
        this.toastr.error(e.error?.message ?? 'İrsaliye oluşturulamadı.');
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
}
