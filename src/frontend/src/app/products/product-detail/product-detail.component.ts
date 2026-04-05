import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../services/product.service';
import { ProductDto, StockMovementDto, PagedResult, StockMovementType } from '../../core/models';
import { ToastrService } from 'ngx-toastr';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { SearchableSelectComponent, SearchableSelectOption } from '../../shared/searchable-select/searchable-select.component';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, TranslateModule, SearchableSelectComponent],
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.scss']
})
export class ProductDetailComponent implements OnInit {
  product: ProductDto | null = null;
  loading = true;
  movements: StockMovementDto[] = [];
  totalCount = 0;
  page = 1;
  pageSize = 20;
  dateFrom = '';
  dateTo = '';
  showAddModal = false;
  newMovement: { date: string, type: StockMovementType, quantity: number, description: string } = {
    date: '',
    type: 1,
    quantity: 0,
    description: ''
  };

  constructor(
    private route: ActivatedRoute,
    private api: ProductService,
    private toastr: ToastrService,
    private translate: TranslateService
  ) { }

  get movementTypeSearchableOptions(): SearchableSelectOption[] {
    return [
      { id: '1', primary: this.translate.instant('common.txnIn') },
      { id: '2', primary: this.translate.instant('common.txnOut') }
    ];
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.api.getById(id).subscribe({
        next: (p) => {
          this.product = p;
          this.loading = false;
        },
        error: () => {
          this.loading = false;
        }
      });
      this.loadMovements();
    } else {
      this.loading = false;
    }
  }

  loadMovements(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) return;
    const dateFrom = this.dateFrom || undefined;
    const dateTo = this.dateTo || undefined;
    this.api.getMovements(id, this.page, this.pageSize, dateFrom, dateTo).subscribe((res: PagedResult<StockMovementDto>) => {
      this.movements = res.items;
      this.totalCount = res.totalCount;
    });
  }

  applyFilter(): void {
    this.page = 1;
    this.loadMovements();
  }

  prevPage(): void {
    if (this.page > 1) { this.page--; this.loadMovements(); }
  }

  nextPage(): void {
    this.page++; this.loadMovements();
  }

  openAddModal(): void {
    const pad = (n: number) => n.toString().padStart(2, '0');
    const d = new Date();
    const dateVal = `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
    this.newMovement = {
      date: dateVal,
      type: 1,
      quantity: 0,
      description: this.translate.instant('products.manualMovementDesc')
    };
    this.showAddModal = true;
  }

  closeAddModal(): void {
    this.showAddModal = false;
  }

  submitMovement(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id || this.newMovement.quantity <= 0) return;

    if (this.newMovement.type === 2 && this.newMovement.quantity > (this.product?.stockQuantity ?? 0)) {
      this.toastr.warning(this.translate.instant('products.toastInsufficientStock'));
      return;
    }

    this.api.addMovement(id, {
      date: this.newMovement.date,
      type: this.newMovement.type,
      quantity: this.newMovement.quantity,
      description: this.newMovement.description || this.translate.instant('products.manualMovementDesc')
    }).subscribe({
      next: () => {
        this.toastr.success(this.translate.instant('products.toastStockAdded'));
        this.closeAddModal();
        this.api.getById(id).subscribe(p => this.product = p);
        this.loadMovements();
      },
      error: e => this.toastr.error(e.error?.message ?? this.translate.instant('products.toastMovementError'))
    });
  }

  typeLabel(type: number): string {
    if (type === 1) return this.translate.instant('common.txnIn');
    if (type === 2) return this.translate.instant('common.txnOut');
    return this.translate.instant('products.movementTypeTransfer');
  }
}
