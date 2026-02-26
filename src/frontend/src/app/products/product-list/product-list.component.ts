import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProductService, ProductListDto } from '../../services/product.service';
import { AuthService } from '../../core/services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { PagedResult } from '../../core/services/api.service';

import { ConfirmModalComponent } from '../../shared/confirm-modal/confirm-modal.component';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, ConfirmModalComponent],
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss']
})
export class ProductListComponent implements OnInit {
  items: ProductListDto[] = [];
  totalCount = 0;
  page = 1;
  pageSize = 20;
  search = '';
  firmId: string | undefined;
  loading = false;
  showDeleteModal = false;
  deletingProduct: { id: string, name: string } | null = null;

  constructor(
    private api: ProductService,
    public auth: AuthService,
    private toastr: ToastrService
  ) { }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.api.getPaged(this.page, this.pageSize, this.search || undefined, this.firmId).subscribe({
      next: (res: PagedResult<ProductListDto>) => {
        this.items = res.items;
        this.totalCount = res.totalCount;
        this.loading = false;
      },
      error: e => {
        this.toastr.error(e.error?.message ?? 'Liste yüklenemedi.');
        this.loading = false;
      }
    });
  }

  applySearch(): void {
    this.page = 1;
    this.load();
  }

  onDeleteClick(id: string, name: string): void {
    this.deletingProduct = { id, name };
    this.showDeleteModal = true;
  }

  confirmDelete(): void {
    if (!this.deletingProduct) return;
    this.api.delete(this.deletingProduct.id).subscribe({
      next: () => {
        this.toastr.success('Ürün silindi.');
        this.showDeleteModal = false;
        this.deletingProduct = null;
        this.load();
      },
      error: e => {
        this.toastr.error(e.error?.message ?? 'Silme sırasında hata oluştu.');
        this.showDeleteModal = false;
        this.deletingProduct = null;
      }
    });
  }

}
