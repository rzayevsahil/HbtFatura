import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProductService, ProductDto, StockMovementDto } from '../../services/product.service';
import { PagedResult } from '../../core/services/api.service';
import { ToastrService } from 'ngx-toastr';

const STOCK_GIRIS = 1;
const STOCK_CIKIS = 2;

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.scss']
})
export class ProductDetailComponent implements OnInit {
  product: ProductDto | null = null;
  movements: StockMovementDto[] = [];
  totalCount = 0;
  page = 1;
  pageSize = 20;
  dateFrom = '';
  dateTo = '';
  showAddModal = false;
  newMovement = { date: '', type: STOCK_GIRIS, quantity: 0, description: '' };

  constructor(
    private route: ActivatedRoute,
    private api: ProductService,
    private toastr: ToastrService
  ) { }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.api.getById(id).subscribe(p => this.product = p);
      this.loadMovements();
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

  openAddModal(): void {
    this.newMovement = { date: new Date().toISOString().slice(0, 10), type: STOCK_GIRIS, quantity: 0, description: 'Manuel giriş/çıkış' };
    this.showAddModal = true;
  }

  closeAddModal(): void {
    this.showAddModal = false;
  }

  submitMovement(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id || this.newMovement.quantity <= 0) return;

    if (this.newMovement.type === STOCK_CIKIS && this.newMovement.quantity > (this.product?.stockQuantity ?? 0)) {
      this.toastr.warning('Yetersiz stok! Mevcut stoktan fazla çıkış yapamazsınız.');
      return;
    }

    this.api.addMovement(id, {
      date: this.newMovement.date,
      type: this.newMovement.type,
      quantity: this.newMovement.quantity,
      description: this.newMovement.description || 'Manuel'
    }).subscribe({
      next: () => {
        this.toastr.success('Stok hareketi eklendi.');
        this.closeAddModal();
        this.api.getById(id).subscribe(p => this.product = p);
        this.loadMovements();
      },
      error: e => this.toastr.error(e.error?.message ?? 'Eklenemedi.')
    });
  }

  typeLabel(type: number): string {
    return type === STOCK_GIRIS ? 'Giriş' : type === STOCK_CIKIS ? 'Çıkış' : 'Transfer';
  }
}
