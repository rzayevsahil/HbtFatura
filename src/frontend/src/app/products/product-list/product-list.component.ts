import { ChangeDetectorRef, Component, DestroyRef, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../services/product.service';
import { ProductListDto, PagedResult } from '../../core/models';
import { AuthService } from '../../core/services/auth.service';
import { LookupService } from '../../core/services/lookup.service';
import { ToastrService } from 'ngx-toastr';

import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ConfirmModalComponent } from '../../shared/confirm-modal/confirm-modal.component';
import { currencyDisplaySuffix } from '../../core/utils/currency-display';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, ConfirmModalComponent, TranslateModule],
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss']
})
export class ProductListComponent implements OnInit {
  private readonly destroyRef = inject(DestroyRef);

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
    private toastr: ToastrService,
    private translate: TranslateService,
    public lookups: LookupService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.translate.onLangChange.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.cdr.markForCheck());
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
        this.toastr.error(e.error?.message ?? this.translate.instant('products.loadError'));
        this.loading = false;
      }
    });
  }

  formatCurrencyLabel(code: string | undefined | null): string {
    return currencyDisplaySuffix(code);
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
        this.toastr.success(this.translate.instant('products.deleted'));
        this.showDeleteModal = false;
        this.deletingProduct = null;
        this.load();
      },
      error: e => {
        this.toastr.error(e.error?.message ?? this.translate.instant('products.deleteError'));
        this.showDeleteModal = false;
        this.deletingProduct = null;
      }
    });
  }

}
