import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { InvoiceService, InvoiceDto } from '../../services/invoice.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-invoice-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './invoice-detail.component.html',
  styleUrls: ['./invoice-detail.component.scss']
})
export class InvoiceDetailComponent implements OnInit {
  invoice: InvoiceDto | null = null;

  constructor(private route: ActivatedRoute, private api: InvoiceService, private toastr: ToastrService) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) this.api.getById(id).subscribe(inv => this.invoice = inv);
  }

  statusLabel(s: number): string {
    const map: Record<number, string> = { 0: 'Taslak', 1: 'Kesildi', 2: 'Ödendi', 3: 'İptal' };
    return map[s] ?? '';
  }

  statusClass(s: number): string {
    const map: Record<number, string> = { 0: 'draft', 1: 'issued', 2: 'paid', 3: 'cancelled' };
    return map[s] ?? '';
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
}
