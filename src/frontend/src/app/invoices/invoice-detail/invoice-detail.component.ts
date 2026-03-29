import { Component, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { InvoiceService } from '../../services/invoice.service';
import { InvoiceDto, InvoiceScenario, InvoiceStatus } from '../../core/models';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-invoice-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './invoice-detail.component.html',
  styleUrls: ['./invoice-detail.component.scss']
})
export class InvoiceDetailComponent implements OnInit {
  invoice: InvoiceDto | null = null;
  loading = true;
  selectedScenario: InvoiceScenario = 0;

  constructor(private route: ActivatedRoute, private router: Router, private api: InvoiceService, private toastr: ToastrService) { }

  @HostListener('document:keydown', ['$event'])
  onKeyDown(e: KeyboardEvent): void {
    if (e.key === 'F2' && this.invoice?.status === 0 && !['INPUT', 'TEXTAREA', 'SELECT'].includes((e.target as HTMLElement)?.tagName)) {
      e.preventDefault();
      this.router.navigate(['/invoices', this.invoice.id, 'edit']);
    }
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.api.getById(id).subscribe({
        next: (inv) => {
          this.invoice = inv;
          this.loading = false;
        },
        error: () => {
          this.loading = false;
        }
      });
    } else {
      this.loading = false;
    }
  }

  statusLabel(s: any, sourceType?: string | null): string {
    const map: any = {
      0: 'Fatura Oluşturuldu', 'Draft': 'Fatura Oluşturuldu',
      1: 'Onaylandı', 'Issued': 'Onaylandı',
      2: 'Ödendi', 'Paid': 'Ödendi',
      3: 'İptal', 'Cancelled': 'İptal',
      4: 'GİB onayı bekliyor', 'PendingGibAcceptance': 'GİB onayı bekliyor'
    };
    return map[s] ?? (s !== null && s !== undefined ? s.toString() : '');
  }

  statusClass(s: any): string {
    const map: any = {
      0: 'draft', 'Draft': 'draft',
      1: 'issued', 'Issued': 'issued',
      2: 'paid', 'Paid': 'paid',
      3: 'cancelled', 'Cancelled': 'cancelled',
      4: 'pending-gib', 'PendingGibAcceptance': 'pending-gib'
    };
    return map[s] ?? '';
  }

  sendingGib = false;
  approvingPurchase = false;

  /** Alış faturası: GİB yok; durum Onaylandı yapılır, PDF açılır. */
  approvePurchaseInvoice(): void {
    if (!this.invoice || this.invoice.invoiceType !== 1 || this.invoice.status !== 0) return;
    this.approvingPurchase = true;
    this.api.setStatus(this.invoice.id, 1 as InvoiceStatus).subscribe({
      next: () => {
        this.api.getById(this.invoice!.id).subscribe(inv => {
          this.invoice = inv;
          this.approvingPurchase = false;
          this.toastr.success('Alış faturası onaylandı; PDF indirebilirsiniz.');
        });
      },
      error: () => {
        this.approvingPurchase = false;
        this.toastr.error('Onay sırasında bir hata oluştu.');
      }
    });
  }

  sendToGib(): void {
    if (!this.invoice || this.invoice.isGibSent || this.invoice.status === 4) return;

    this.sendingGib = true;
    this.api.sendToGib(this.invoice.id, this.selectedScenario).subscribe({
      next: () => {
        this.sendingGib = false;
        this.api.getById(this.invoice!.id).subscribe(inv => {
          this.invoice = inv;
          this.toastr.success('Fatura GİB simülasyon kuyruğuna alındı; karşı firma onayından sonra kesinleşir.');
        });
      },
      error: e => {
        this.sendingGib = false;
        this.toastr.error(e.error?.message ?? "GİB simülasyon gönderiminde hata oluştu.");
      }
    });
  }

  downloadPdf(): void {
    if (!this.invoice) return;
    this.api.getPdf(this.invoice.id).subscribe({
      next: blob => {
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `fatura-${this.invoice!.invoiceNumber}.pdf`;
        a.click();
        URL.revokeObjectURL(url);
        this.toastr.success('Fatura PDF indirildi.');
      },
      error: () => this.toastr.error('PDF indirilemedi.')
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
