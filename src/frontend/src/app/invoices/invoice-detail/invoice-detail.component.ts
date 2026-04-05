import { Component, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { InvoiceService } from '../../services/invoice.service';
import { InvoiceDto, InvoiceScenario, InvoiceStatus } from '../../core/models';
import { ToastrService } from 'ngx-toastr';
import { SearchableSelectComponent, SearchableSelectOption } from '../../shared/searchable-select/searchable-select.component';

@Component({
  selector: 'app-invoice-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, TranslateModule, SearchableSelectComponent],
  templateUrl: './invoice-detail.component.html',
  styleUrls: ['./invoice-detail.component.scss']
})
export class InvoiceDetailComponent implements OnInit {
  invoice: InvoiceDto | null = null;
  loading = true;
  selectedScenario: InvoiceScenario = 0;

  get gibScenarioSearchableOptions(): SearchableSelectOption[] {
    return [
      { id: '0', primary: this.translate.instant('invoices.basicInvoice') },
      { id: '1', primary: this.translate.instant('invoices.commercialInvoice') }
    ];
  }

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private api: InvoiceService,
    private toastr: ToastrService,
    private translate: TranslateService
  ) { }

  @HostListener('document:keydown', ['$event'])
  onKeyDown(e: KeyboardEvent): void {
    const t = e.target as HTMLElement;
    if (e.key === 'F2' && this.invoice?.status === 0 && !['INPUT', 'TEXTAREA', 'SELECT'].includes(t?.tagName) && !t?.closest('app-searchable-select')) {
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
    const map: Record<string, string> = {
      '0': 'invoices.statusDraft', Draft: 'invoices.statusDraft',
      '1': 'invoices.statusIssued', Issued: 'invoices.statusIssued',
      '2': 'invoices.statusPaid', Paid: 'invoices.statusPaid',
      '3': 'invoices.statusCancelled', Cancelled: 'invoices.statusCancelled',
      '4': 'invoices.statusPendingGib', PendingGibAcceptance: 'invoices.statusPendingGib'
    };
    const key = map[String(s)];
    if (key) {
      return this.translate.instant(key);
    }
    return s !== null && s !== undefined ? String(s) : '';
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
          this.toastr.success(this.translate.instant('invoices.toastrPurchaseOk'));
        });
      },
      error: () => {
        this.approvingPurchase = false;
        this.toastr.error(this.translate.instant('invoices.toastrApproveError'));
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
          this.toastr.success(this.translate.instant('invoices.toastrGibQueued'));
        });
      },
      error: e => {
        this.sendingGib = false;
        this.toastr.error(e.error?.message ?? this.translate.instant('invoices.toastrGibError'));
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
        this.toastr.success(this.translate.instant('invoices.toastrPdfOk'));
      },
      error: () => this.toastr.error(this.translate.instant('invoices.toastrPdfFail'))
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
