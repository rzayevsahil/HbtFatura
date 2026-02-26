import { Component, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { OrderService, OrderDto, OrderStatus } from '../../services/order.service';
import { DeliveryNoteService } from '../../services/delivery-note.service';
import { ToastrService } from 'ngx-toastr';

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
    private toastr: ToastrService
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
    if (s === undefined || s === null) return '';
    const mapByNumber: Record<number, string> = { 0: 'Bekliyor', 1: 'Tamamı Teslim', 2: 'İptal', 3: 'Onaylandı', 4: 'Kısmi Teslim' };
    const mapByString: Record<string, string> = { Bekliyor: 'Bekliyor', TamamiTeslim: 'Tamamı Teslim', Iptal: 'İptal', Onaylandi: 'Onaylandı', KismiTeslim: 'Kısmi Teslim' };
    if (typeof s === 'number') return mapByNumber[s] ?? '';
    return mapByString[String(s)] ?? '';
  }

  isEditableOrder(): boolean {
    return this.order != null && (this.order.status === 0 || this.order.status === 'Bekliyor');
  }

  /** Onaylandı / Kısmi Teslim iken irsaliyeye aktarılabilir. */
  canSendToDeliveryNote(): boolean {
    if (!this.order) return false;
    const s = this.order.status;
    return s === 3 || s === 4 || s === 'Onaylandi' || s === 'KismiTeslim';
  }

  typeLabel(t: number): string {
    return t === 1 ? 'Alış' : 'Satış';
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
    const deliveryDate = new Date().toISOString().slice(0, 10);
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
}
