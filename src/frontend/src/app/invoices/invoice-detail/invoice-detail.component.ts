import { Component, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { InvoiceService, InvoiceDto, InvoiceScenario } from '../../services/invoice.service';
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
    if (id) this.api.getById(id).subscribe(inv => this.invoice = inv);
  }

  statusLabel(s: any, sourceType?: string | null): string {
    const map: any = {
      0: 'Fatura Oluşturuldu', 'Draft': 'Fatura Oluşturuldu',
      1: 'Onaylandı', 'Issued': 'Onaylandı',
      2: 'Ödendi', 'Paid': 'Ödendi',
      3: 'İptal', 'Cancelled': 'İptal'
    };
    return map[s] ?? (s !== null && s !== undefined ? s.toString() : '');
  }

  statusClass(s: any): string {
    const map: any = {
      0: 'draft', 'Draft': 'draft',
      1: 'issued', 'Issued': 'issued',
      2: 'paid', 'Paid': 'paid',
      3: 'cancelled', 'Cancelled': 'cancelled'
    };
    return map[s] ?? '';
  }

  sendingGib = false;

  sendToGib(): void {
    if (!this.invoice || this.invoice.isGibSent) return;

    this.sendingGib = true;
    this.api.sendToGib(this.invoice.id, this.selectedScenario).subscribe({
      next: () => {
        this.sendingGib = false;
        if (this.invoice) {
          this.invoice.isGibSent = true;
          this.invoice.status = 1; // Onaylandı
          this.invoice.scenario = this.selectedScenario;
        }
        this.toastr.success("Fatura başarıyla GİB'e gönderildi.");
      },
      error: () => {
        this.sendingGib = false;
        this.toastr.error("GİB'e gönderim sırasında bir hata oluştu.");
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
}
