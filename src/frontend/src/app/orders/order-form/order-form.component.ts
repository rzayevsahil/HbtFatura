import { Component, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators, FormsModule } from '@angular/forms';
import { OrderService, CreateOrderRequest, UpdateOrderRequest, OrderStatus } from '../../services/order.service';
import { CustomerService, CustomerDto } from '../../services/customer.service';
import { ProductService, ProductDto } from '../../services/product.service';
import { ReportService, StockLevelsReportDto } from '../../services/report.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-order-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, RouterLink],
  templateUrl: './order-form.component.html',
  styleUrls: ['./order-form.component.scss']
})
export class OrderFormComponent implements OnInit {
  form: FormGroup;
  id: string | null = null;
  orderNumber: string | null = null;
  customers: CustomerDto[] = [];
  products: ProductDto[] = [];
  productFilterText = '';
  stockLevels: StockLevelsReportDto | null = null;
  error = '';
  saving = false;

  get items(): FormArray {
    return this.form.get('items') as FormArray;
  }

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private orderApi: OrderService,
    private customerApi: CustomerService,
    private productApi: ProductService,
    private reportApi: ReportService,
    private toastr: ToastrService
  ) {
    this.form = this.fb.nonNullable.group({
      customerId: this.fb.control<string | null>(null),
      orderDate: [this.toDatetimeLocalValue(new Date()), Validators.required],
      orderType: [0 as number, Validators.required],
      status: [0 as number],
      items: this.fb.array([this.createItemGroup()])
    });
  }

  ngOnInit(): void {
    this.customerApi.getDropdown().subscribe(list => this.customers = list);
    this.productApi.getDropdown().subscribe(list => this.products = list);
    this.reportApi.getStockLevels().subscribe(data => this.stockLevels = data);
    this.id = this.route.snapshot.paramMap.get('id');
    if (this.id) {
      this.orderApi.getById(this.id).subscribe(o => {
        this.orderNumber = o.orderNumber ?? null;
        const orderTypeNum = this.normalizeOrderType(o.orderType);
        this.form.patchValue({
          customerId: o.customerId ?? null,
          orderDate: this.toDatetimeLocalValue(o.orderDate),
          orderType: orderTypeNum,
          status: 0
        });
        this.items.clear();
        o.items.forEach((it, idx) => this.items.push(this.fb.nonNullable.group({
          productId: [it.productId ?? null],
          description: [it.description],
          quantity: [it.quantity],
          unitPrice: [it.unitPrice],
          vatRate: [it.vatRate],
          sortOrder: [idx]
        })));
      });
    }
  }

  createItemGroup(): FormGroup {
    return this.fb.nonNullable.group({
      productId: [null as string | null],
      description: [''],
      quantity: [1],
      unitPrice: [0],
      vatRate: [18],
      sortOrder: [0]
    });
  }

  onProductSelect(i: number): void {
    const g = this.items.at(i);
    const productId = g.get('productId')?.value as string | null;
    if (!productId) return;
    const p = this.products.find(x => x.id === productId);
    if (p) g.patchValue({ description: p.name });
  }

  /** datetime-local input için değer: "yyyy-MM-ddTHH:mm". */
  toDatetimeLocalValue(date: string | Date): string {
    if (!date) return '';
    if (typeof date === 'string' && date.length === 10) return date + 'T00:00';
    const d = typeof date === 'string' ? new Date(date) : date;
    if (isNaN(d.getTime())) return '';
    const pad = (n: number) => n.toString().padStart(2, '0');
    return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
  }

  /** API bazen Tip'i string ("Satis", "Alis") döner; select sayı bekliyor. */
  normalizeOrderType(v: number | string | undefined): number {
    if (v === undefined || v === null) return 0;
    if (typeof v === 'number') return v;
    const s = String(v);
    if (s === 'Alis' || s === '1') return 1;
    return 0;
  }

  isLowStock(productId: string | null): boolean {
    if (!productId || !this.stockLevels) return false;
    const row = this.stockLevels.items.find(x => x.productId === productId);
    return row?.lowStock ?? false;
  }

  get filteredProducts(): ProductDto[] {
    const t = this.productFilterText?.trim().toLowerCase();
    if (!t) return this.products;
    return this.products.filter(p =>
      (p.code?.toLowerCase().includes(t)) || (p.name?.toLowerCase().includes(t))
    );
  }

  @HostListener('document:keydown', ['$event'])
  onKeyDown(e: KeyboardEvent): void {
    if (e.key === 'F9' && !this.saving && this.form.valid && !['INPUT', 'TEXTAREA', 'SELECT'].includes((e.target as HTMLElement)?.tagName)) {
      e.preventDefault();
      this.onSubmit();
    }
  }

  addItem(): void {
    this.items.push(this.createItemGroup());
  }

  removeItem(i: number): void {
    if (this.items.length > 1) this.items.removeAt(i);
  }

  /** API'nin kabul ettiği tarih formatı: ISO 8601 saniyeli (yyyy-MM-ddTHH:mm:ss). */
  toOrderDateApiValue(value: string): string {
    if (!value || typeof value !== 'string') return new Date().toISOString().slice(0, 19);
    const s = value.trim();
    if (s.length === 10) return s + 'T00:00:00';
    if (s.length === 16) return s + ':00';
    return s.slice(0, 19);
  }

  onSubmit(): void {
    this.error = '';
    this.form.markAllAsTouched();
    if (this.form.invalid) return;
    this.saving = true;
    const raw = this.form.getRawValue();
    const orderDateStr = this.toOrderDateApiValue(raw.orderDate);
    const req: CreateOrderRequest = {
      customerId: raw.customerId ?? undefined,
      orderDate: orderDateStr,
      orderType: raw.orderType,
      items: raw.items.map((it: any, idx: number) => ({
        productId: it.productId ?? undefined,
        description: it.description || '',
        quantity: Number(it.quantity) || 0,
        unitPrice: Number(it.unitPrice) || 0,
        vatRate: Number(it.vatRate) || 0,
        sortOrder: idx
      }))
    };
    if (this.id) {
      const updateReq: UpdateOrderRequest = {
        customerId: raw.customerId ?? undefined,
        orderDate: orderDateStr,
        status: raw.status as OrderStatus,
        items: raw.items.map((it: any, idx: number) => ({
          productId: it.productId ?? undefined,
          description: it.description || '',
          quantity: Number(it.quantity) || 0,
          unitPrice: Number(it.unitPrice) || 0,
          vatRate: Number(it.vatRate) || 0,
          sortOrder: idx
        }))
      };
      this.orderApi.update(this.id, updateReq).subscribe({
        next: () => { this.toastr.success('Sipariş güncellendi.'); this.router.navigate(['/orders']); },
        error: e => { this.error = e.error?.message ?? 'Kaydedilemedi.'; this.saving = false; }
      });
    } else {
      this.orderApi.create(req).subscribe({
        next: () => { this.toastr.success('Sipariş oluşturuldu.'); this.router.navigate(['/orders']); },
        error: e => { this.error = e.error?.message ?? 'Kaydedilemedi.'; this.saving = false; }
      });
    }
  }
}
