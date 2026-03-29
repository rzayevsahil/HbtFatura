import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ChequeService } from '../../services/cheque.service';
import { LookupService } from '../../core/services/lookup.service';
import { ChequeOrPromissoryDto } from '../../core/models';
import { ToastrService } from 'ngx-toastr';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-cheque-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, TranslateModule],
  templateUrl: './cheque-detail.component.html',
  styleUrls: ['./cheque-detail.component.scss']
})
export class ChequeDetailComponent implements OnInit {
  item: ChequeOrPromissoryDto | null = null;
  loading = true;
  newStatus: number = 0;

  constructor(
    private route: ActivatedRoute,
    private api: ChequeService,
    public lookups: LookupService,
    private toastr: ToastrService,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.api.getById(id).subscribe({
        next: (c) => {
          this.item = c;
          this.newStatus = c.status;
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
    if (type === 1) return this.translate.instant('chequesPage.typeCheque');
    if (type === 2) return this.translate.instant('chequesPage.typeNote');
    return '';
  }

  statusLabel(status: number): string {
    return this.lookups.getName('ChequeStatus', status);
  }
}
