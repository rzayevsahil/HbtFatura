import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { GibSimulationService, GibInboxItemDto } from '../../services/gib-simulation.service';
import { InvoiceService } from '../../services/invoice.service';
import { ToastrService } from 'ngx-toastr';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-gib-inbox',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule],
  templateUrl: './gib-inbox.component.html',
  styleUrls: ['./gib-inbox.component.scss']
})
export class GibInboxComponent implements OnInit {
  items: GibInboxItemDto[] = [];
  search = '';
  loading = true;
  busyId: string | null = null;
  pdfBusyId: string | null = null;

  constructor(
    private gib: GibSimulationService,
    private invoices: InvoiceService,
    private toastr: ToastrService,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.gib.inbox().subscribe({
      next: rows => {
        this.items = rows;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.toastr.error('GİB kutusu yüklenemedi.');
      }
    });
  }

  accept(row: GibInboxItemDto): void {
    this.busyId = row.submissionId;
    this.gib.accept(row.submissionId).subscribe({
      next: () => {
        this.toastr.success(this.translate.instant('gibInbox.toastAccepted'));
        this.load();
        this.busyId = null;
      },
      error: e => {
        this.busyId = null;
        this.toastr.error(e.error?.message ?? this.translate.instant('gibInbox.toastAcceptFailed'));
      }
    });
  }

  downloadPdf(row: GibInboxItemDto): void {
    this.pdfBusyId = row.invoiceId;
    this.invoices.getPdf(row.invoiceId).subscribe({
      next: blob => {
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `fatura-${row.invoiceNumber}.pdf`;
        a.click();
        URL.revokeObjectURL(url);
        this.pdfBusyId = null;
        this.toastr.success(this.translate.instant('gibInbox.toastPdfOk'));
      },
      error: () => {
        this.pdfBusyId = null;
        this.toastr.error(this.translate.instant('gibInbox.toastPdfFail'));
      }
    });
  }

  reject(row: GibInboxItemDto): void {
    this.busyId = row.submissionId;
    this.gib.reject(row.submissionId).subscribe({
      next: () => {
        this.toastr.info(this.translate.instant('gibInbox.toastRejected'));
        this.load();
        this.busyId = null;
      },
      error: e => {
        this.busyId = null;
        this.toastr.error(e.error?.message ?? this.translate.instant('gibInbox.toastRejectFailed'));
      }
    });
  }

  get filteredItems(): GibInboxItemDto[] {
    const q = this.search?.trim().toLowerCase() || '';
    if (!q) return this.items;
    return this.items.filter(
      (r) =>
        r.invoiceNumber?.toLowerCase().includes(q) ||
        r.senderFirmName?.toLowerCase().includes(q) ||
        r.customerTitle?.toLowerCase().includes(q) ||
        r.recipientTaxNumber?.toLowerCase().includes(q)
    );
  }
}
