import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ChequeService, ChequeOrPromissoryDto } from '../../services/cheque.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-cheque-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './cheque-detail.component.html',
  styleUrls: ['./cheque-detail.component.scss']
})
export class ChequeDetailComponent implements OnInit {
  item: ChequeOrPromissoryDto | null = null;
  newStatus: number = 0;

  constructor(
    private route: ActivatedRoute,
    private api: ChequeService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.api.getById(id).subscribe(c => {
        this.item = c;
        this.newStatus = c.status;
      });
    }
  }

  updateStatus(): void {
    if (!this.item) return;
    this.api.setStatus(this.item.id, this.newStatus).subscribe({
      next: () => {
        this.toastr.success('Durum güncellendi.');
        this.item = { ...this.item!, status: this.newStatus };
      },
      error: e => this.toastr.error(e.error?.message ?? 'Güncellenemedi.')
    });
  }

  typeLabel(type: number): string {
    return type === 1 ? 'Çek' : type === 2 ? 'Senet' : '';
  }

  statusLabel(status: number): string {
    const map: Record<number, string> = { 0: 'Portföyde', 1: 'Tahsil edildi', 2: 'Ödendi', 3: 'Reddedildi' };
    return map[status] ?? '';
  }
}
