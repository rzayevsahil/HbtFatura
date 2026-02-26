import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { DeliveryNoteService, DeliveryNoteDto, DeliveryNoteStatus } from '../../services/delivery-note.service';
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
  ) {}

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

  statusLabel(s: DeliveryNoteStatus): string {
    const map: Record<DeliveryNoteStatus, string> = { 0: 'Taslak', 1: 'Onaylandı', 2: 'İptal' };
    return map[s] ?? '';
  }

  typeLabel(t: number): string {
    return t === 1 ? 'Alış' : 'Satış';
  }

  confirmDeliveryNote(): void {
    if (!this.deliveryNote || this.deliveryNote.status !== 0) return;
    this.deliveryNoteApi.setStatus(this.deliveryNote.id, 1).subscribe({
      next: () => { this.toastr.success('İrsaliye onaylandı.'); this.ngOnInit(); },
      error: e => this.toastr.error(e.error?.message ?? 'Onaylanamadı.')
    });
  }

  createInvoice(): void {
    if (!this.deliveryNote || this.deliveryNote.status !== 1) return;
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
}
