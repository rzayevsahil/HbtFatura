import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../services/product.service';
import { ProductDto, StockMovementDto, PagedResult, StockMovementType } from '../../core/models';
import { ToastrService } from 'ngx-toastr';

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
  newMovement: { date: string, type: StockMovementType, quantity: number, description: string } = {
    date: '',
    type: 1,
    quantity: 0,
    description: ''
  };

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
    this.newMovement = { date: dateVal, type: 1, quantity: 0, description: 'Manuel giriş/çıkış' };
    this.showAddModal = true;
  }

  closeAddModal(): void {
    this.showAddModal = false;
  }

  submitMovement(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id || this.newMovement.quantity <= 0) return;

    if (this.newMovement.type === 2 && this.newMovement.quantity > (this.product?.stockQuantity ?? 0)) {
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
    return type === 1 ? 'Giriş' : type === 2 ? 'Çıkış' : 'Transfer';
  }
}
