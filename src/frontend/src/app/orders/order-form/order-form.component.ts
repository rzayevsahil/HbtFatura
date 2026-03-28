import { Component, OnInit, HostListener, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators, FormsModule } from '@angular/forms';
import { OrderService } from '../../services/order.service';
import { CustomerService } from '../../services/customer.service';
import { ProductService } from '../../services/product.service';
import { ReportService } from '../../services/report.service';
import { CustomerDto, ProductDto, StockLevelsReportDto, CreateOrderRequest, UpdateOrderRequest, OrderStatus, OrderItemInputDto } from '../../core/models';
import { ToastrService } from 'ngx-toastr';
import { LookupService } from '../../core/services/lookup.service';

@Component({
  selector: 'app-order-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, RouterLink],
  templateUrl: './order-form.component.html',
  styleUrls: ['./order-form.component.scss']
})
export class OrderFormComponent implements OnInit {
  @ViewChild('customerDropdownWrap') customerDropdownWrap?: ElementRef<HTMLElement>;

  form: FormGroup;
  id: string | null = null;
  orderNumber: string | null = null;
  customers: CustomerDto[] = [];
  products: ProductDto[] = [];
  productFilterText = '';
  stockLevels: StockLevelsReportDto | null = null;
  error = '';
  saving = false;
  /** Arama kutusu sadece dropdown içinde (fatura cari gibi) */
  customerSearchText = '';
  customerDropdownOpen = false;
  activeItemIndex: number | null = null;
  activeItemField: 'code' | 'description' | null = null;

  get items(): FormArray {
    return this.form.get('items') as FormArray;
  }

  get selectedCustomerTitle(): string {
    const cid = this.form.get('customerId')?.value;
    if (!cid) return 'Seçin';
    const c = this.customers.find(x => x.id === cid);
    return c?.title ?? 'Seçin';
  }

  /** Dropdown içindeki arama kutusuna göre filtrelenir */
  get filteredCustomers(): CustomerDto[] {
    const t = this.customerSearchText?.trim().toLowerCase();
    if (!t) return this.customers;
    return this.customers.filter(c =>
      (c.title?.toLowerCase().includes(t)) ||
      (c.code?.toLowerCase().includes(t)) ||
      (c.taxNumber?.toLowerCase().includes(t))
    );
  }

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private orderApi: OrderService,
    private customerApi: CustomerService,
    private productApi: ProductService,
    private reportApi: ReportService,
    private toastr: ToastrService,
    public lookups: LookupService
  ) {
    this.form = this.fb.nonNullable.group({
      customerId: this.fb.control<string | null>(null, Validators.required),
      orderDate: [this.toDatetimeLocalValue(new Date()), Validators.required],
      orderType: [0 as number, Validators.required],
      status: [0 as number],
      items: this.fb.array([this.createItemGroup()])
    });
  }

  get totalNet(): number {
    const items = this.items.controls;
    return items.reduce((sum, c) => {
      const qty = Number(c.get('quantity')?.value || 0);
      const price = Number(c.get('unitPrice')?.value || 0);
      return sum + qty * price;
    }, 0);
  }

  get totalVat(): number {
    const items = this.items.controls;
    return items.reduce((sum, c) => {
      const qty = Number(c.get('quantity')?.value || 0);
      const price = Number(c.get('unitPrice')?.value || 0);
      const vatRate = Number(c.get('vatRate')?.value || 0);
      const net = qty * price;
      return sum + net * vatRate / 100;
    }, 0);
  }

  get totalGross(): number {
    return this.totalNet + this.totalVat;
  }

  ngOnInit(): void {
    this.customerApi.getDropdown().subscribe(list => this.customers = list);
    this.productApi.getDropdown().subscribe(list => this.products = list);
    this.reportApi.getStockLevels().subscribe(data => this.stockLevels = data);
    this.id = this.route.snapshot.paramMap.get('id');
    if (this.id) {
      this.form.get('customerId')?.setValidators(Validators.required);
      this.form.get('customerId')?.updateValueAndValidity();
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
        o.items.forEach((it, idx) => {
          const p = this.products.find(x => x.id === it.productId);
          // API satırında productCode var; ürün listesi henüz yüklenmediyse yine dolu olsun (yarış koşulu).
          this.items.push(this.fb.nonNullable.group({
            productId: [it.productId ?? null],
            productCode: [it.productCode || p?.code || ''],
            description: [it.description],
            quantity: [it.quantity],
            unitPrice: [it.unitPrice],
            vatRate: [it.vatRate],
            sortOrder: [idx]
          }));
        });
      });
    }
  }

  toggleCustomerDropdown(): void {
    this.customerDropdownOpen = !this.customerDropdownOpen;
    if (this.customerDropdownOpen) this.customerSearchText = '';
  }

  selectCustomer(c: CustomerDto | null): void {
    this.form.patchValue({ customerId: c?.id ?? null });
    this.customerDropdownOpen = false;
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(e: MouseEvent): void {
    if (this.customerDropdownOpen && this.customerDropdownWrap?.nativeElement && !this.customerDropdownWrap.nativeElement.contains(e.target as Node)) {
      this.customerDropdownOpen = false;
    }
    const target = e.target as HTMLElement;
    if (!target.closest('.autocomplete-container')) {
      this.activeItemIndex = null;
      this.activeItemField = null;
    }
  }

  createItemGroup(): FormGroup {
    return this.fb.nonNullable.group({
      productId: [null as string | null],
      productCode: [''],
      description: [''],
      quantity: [1],
      unitPrice: [0],
      vatRate: [18],
      sortOrder: [0]
    });
  }

  onProductSelect(i: number, p: ProductDto): void {
    const g = this.items.at(i);
    g.patchValue({
      productId: p.id,
      productCode: p.code,
      description: p.name,
      unitPrice: p.unitPrice
    });
    this.activeItemIndex = null;
    this.activeItemField = null;
  }

  onItemInput(index: number, field: 'code' | 'description'): void {
    this.activeItemIndex = index;
    this.activeItemField = field;
  }

  getFilteredItems(searchText: string): ProductDto[] {
    const t = searchText?.trim().toLowerCase();
    if (!t) return [];
    return this.products.filter(p =>
      (p.code?.toLowerCase().includes(t)) || (p.name?.toLowerCase().includes(t))
    ).slice(0, 10);
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

    // Satış siparişi (0) ise stok kontrolü yap
    if (raw.orderType === 0) {
      for (const it of raw.items) {
        if (!it.productId) continue;
        const p = this.products.find(x => x.id === it.productId);
        if (p && Number(it.quantity) > p.stockQuantity) {
          this.toastr.warning(`"${p.name}" ürünü için yetersiz stok! Mevcut: ${p.stockQuantity}`, 'Stok Uyarısı');
          this.saving = false;
          return;
        }
      }
    }

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
