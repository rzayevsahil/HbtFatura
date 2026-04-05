import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { DeliveryNoteService } from '../../services/delivery-note.service';
import { DeliveryNoteDto, DeliveryNoteItemDto, DeliveryNoteStatus } from '../../core/models';
import { InvoiceService } from '../../services/invoice.service';
import { ToastrService } from 'ngx-toastr';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { LookupService } from '../../core/services/lookup.service';

@Component({
  selector: 'app-delivery-note-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslateModule],
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
    private toastr: ToastrService,
    private translate: TranslateService,
    public lookups: LookupService
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

  statusLabel(s: DeliveryNoteStatus | string | undefined): string {
    return this.lookups.getName('DeliveryNoteStatus', s);
  }

  typeLabel(t: number): string {
    return this.lookups.getName('DeliveryNoteType', t);
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

  get dnCurrencyCode(): string {
    const c = this.deliveryNote?.currency?.trim();
    return c ? c.toUpperCase() : 'TRY';
  }

  lineItemCurrency(it: DeliveryNoteItemDto): string {
    const line = it.currency?.trim();
    if (line) return line.toUpperCase();
    return this.dnCurrencyCode;
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
      next: () => { this.toastr.success(this.translate.instant('deliveryNotes.toastrApproved')); this.ngOnInit(); },
      error: e => this.toastr.error(e.error?.message ?? this.translate.instant('deliveryNotes.approveFailed'))
    });
  }

  createInvoice(): void {
    if (!this.deliveryNote || !this.isOnaylandi(this.deliveryNote.status) || this.deliveryNote.invoiceId) return;
    this.creatingInvoice = true;
    this.invoiceApi.createFromDeliveryNote(this.deliveryNote.id).subscribe({
      next: inv => {
        this.toastr.success(this.translate.instant('deliveryNotes.toastrInvoiceCreated'));
        this.router.navigate(['/invoices', inv.id]);
      },
      error: e => {
        this.toastr.error(e.error?.message ?? this.translate.instant('deliveryNotes.toastrInvoiceFailed'));
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
