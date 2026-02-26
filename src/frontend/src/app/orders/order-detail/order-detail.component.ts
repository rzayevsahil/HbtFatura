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
  ) {}

  @HostListener('document:keydown', ['$event'])
  onKeyDown(e: KeyboardEvent): void {
    if (e.key === 'F2' && this.order && !['INPUT', 'TEXTAREA', 'SELECT'].includes((e.target as HTMLElement)?.tagName)) {
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

  statusLabel(s: OrderStatus): string {
    const map: Record<OrderStatus, string> = { 0: 'Bekliyor', 1: 'Tamamı Teslim', 2: 'İptal' };
    return map[s] ?? '';
  }

  typeLabel(t: number): string {
    return t === 1 ? 'Alış' : 'Satış';
  }

  createDeliveryNote(): void {
    if (!this.order || this.order.status !== 0) return;
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
