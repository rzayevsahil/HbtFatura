import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GibSimulationService, GibInboxItemDto } from '../../services/gib-simulation.service';
import { InvoiceService } from '../../services/invoice.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-gib-inbox',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './gib-inbox.component.html',
  styleUrls: ['./gib-inbox.component.scss']
})
export class GibInboxComponent implements OnInit {
  items: GibInboxItemDto[] = [];
  loading = true;
  busyId: string | null = null;
  pdfBusyId: string | null = null;

  constructor(
    private gib: GibSimulationService,
    private invoices: InvoiceService,
    private toastr: ToastrService
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
        this.toastr.success('Fatura onaylandı (simülasyon).');
        this.load();
        this.busyId = null;
      },
      error: e => {
        this.busyId = null;
        this.toastr.error(e.error?.message ?? 'Onay başarısız.');
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
        this.toastr.success('PDF indirildi.');
      },
      error: () => {
        this.pdfBusyId = null;
        this.toastr.error('PDF indirilemedi veya erişim yok.');
      }
    });
  }

  reject(row: GibInboxItemDto): void {
    this.busyId = row.submissionId;
    this.gib.reject(row.submissionId).subscribe({
      next: () => {
        this.toastr.info('Fatura reddedildi; gönderen taraf taslak olarak görecek.');
        this.load();
        this.busyId = null;
      },
      error: e => {
        this.busyId = null;
        this.toastr.error(e.error?.message ?? 'Red işlemi başarısız.');
      }
    });
  }
}
