import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { DeliveryNoteService } from '../../services/delivery-note.service';
import { DeliveryNoteDto, DeliveryNoteStatus } from '../../core/models';
import { InvoiceService } from '../../services/invoice.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-delivery-note-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './delivery-note-detail.component.html',
  styleUrls: ['./delivery-note-detail.component.scss']
})
export class DeliveryNoteDetailComponent implements OnInit {
  deliveryNote: DeliveryNoteDto | null = null;
  loading = true;
  creatingInvoice = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private deliveryNoteApi: DeliveryNoteService,
    private invoiceApi: InvoiceService,
    private toastr: ToastrService
  ) { }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.deliveryNoteApi.getById(id).subscribe({
        next: dn => { this.deliveryNote = dn; this.loading = false; },
        error: () => { this.loading = false; }
      });
    } else {
      this.loading = false;
    }
  }

  /** Backend bazen enum'ı sayı bazen string (örn. "Taslak") gönderebilir. */
  statusLabel(s: DeliveryNoteStatus | string | undefined): string {
    if (s === undefined || s === null) return '';
    const byNumber: Record<number, string> = { 0: 'Taslak', 1: 'Onaylandı', 2: 'İptal', 3: 'Faturalandı' };
    const byString: Record<string, string> = { Taslak: 'Taslak', Onaylandi: 'Onaylandı', Iptal: 'İptal', Faturalandi: 'Faturalandı' };
    if (typeof s === 'number') return byNumber[s] ?? '';
    return byString[String(s)] ?? '';
  }

  typeLabel(t: number): string {
    return t === 1 ? 'Alış' : 'Satış';
  }

  get totalNet(): number {
    if (!this.deliveryNote) return 0;
    return this.deliveryNote.items.reduce((sum, it) => sum + (it.quantity || 0) * (it.unitPrice || 0), 0);
  }

  get totalVat(): number {
    if (!this.deliveryNote) return 0;
    return this.deliveryNote.items.reduce((sum, it) => {
      const net = (it.quantity || 0) * (it.unitPrice || 0);
      const vatRate = it.vatRate || 0;
      return sum + net * vatRate / 100;
    }, 0);
  }

  get totalGross(): number {
    return this.totalNet + this.totalVat;
  }

  isTaslak(s: DeliveryNoteStatus | string | undefined): boolean {
    return s === 0 || s === 'Taslak';
  }

  isOnaylandi(s: DeliveryNoteStatus | string | undefined): boolean {
    return s === 1 || s === 'Onaylandi' || s === 'Onaylandı';
  }

  confirmDeliveryNote(): void {
    if (!this.deliveryNote || !this.isTaslak(this.deliveryNote.status)) return;
    this.deliveryNoteApi.setStatus(this.deliveryNote.id, 1).subscribe({
      next: () => { this.toastr.success('İrsaliye onaylandı.'); this.ngOnInit(); },
      error: e => this.toastr.error(e.error?.message ?? 'Onaylanamadı.')
    });
  }

  createInvoice(): void {
    if (!this.deliveryNote || !this.isOnaylandi(this.deliveryNote.status) || this.deliveryNote.invoiceId) return;
    this.creatingInvoice = true;
    this.invoiceApi.createFromDeliveryNote(this.deliveryNote.id).subscribe({
      next: inv => {
        this.toastr.success('Fatura oluşturuldu.');
        this.router.navigate(['/invoices', inv.id]);
      },
      error: e => {
        this.toastr.error(e.error?.message ?? 'Fatura oluşturulamadı.');
        this.creatingInvoice = false;
      }
    });
  }

  downloadPdf(): void {
    if (!this.deliveryNote) return;
    this.deliveryNoteApi.downloadPdf(this.deliveryNote.id).subscribe({
      next: blob => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        const number = this.deliveryNote?.deliveryNumber || this.deliveryNote?.id;
        a.href = url;
        a.download = `irsaliye-${number}.pdf`;
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

  goToOrder(orderId: number | string | undefined): void {
    if (orderId) {
      this.router.navigate(['/orders', orderId]);
    }
  }
}
