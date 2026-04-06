import { ChangeDetectorRef, Component, DestroyRef, HostListener, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
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
import { SearchableSelectComponent, SearchableSelectOption } from '../../shared/searchable-select/searchable-select.component';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-order-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, RouterLink, SearchableSelectComponent, TranslateModule],
  templateUrl: './order-form.component.html',
  styleUrls: ['./order-form.component.scss']
})
export class OrderFormComponent implements OnInit {
  private readonly destroyRef = inject(DestroyRef);

  form: FormGroup;
  id: string | null = null;
  /** Düzenleme sayfasında sipariş API’den gelene kadar */
  editLoading = false;
  orderNumber: string | null = null;
  /** Düzenlemede API’den gelen özet para birimi; yoksa satırlardaki ürünlerden türetilir. */
  loadedOrderCurrency: string | null = null;
  customers: CustomerDto[] = [];
  products: ProductDto[] = [];
  productFilterText = '';
  stockLevels: StockLevelsReportDto | null = null;
  error = '';
  saving = false;
  activeItemIndex: number | null = null;
  activeItemField: 'code' | 'description' | null = null;

  get items(): FormArray {
    return this.form.get('items') as FormArray;
  }

  get customerSearchableOptions(): SearchableSelectOption[] {
    return this.customers.map(c => ({
      id: c.id,
      primary: c.title ?? '',
      secondary: [c.code, c.taxNumber].filter(Boolean).join(' · ')
    }));
  }

  get orderTypeSearchableOptions(): SearchableSelectOption[] {
    return this.lookups.getGroup('OrderType')().map(l => ({
      id: String(l.code),
      primary: l.name,
    }));
  }

  /** Kalem satırı `unit` değeri lookup’ta `name || code` ile tutulur (ürün formu / eski UnitFieldSelect ile uyumlu). */
  unitSearchableOptionsForLine(currentUnit: unknown): SearchableSelectOption[] {
    const list = this.lookups.getGroup('ProductUnit')();
    const opts: SearchableSelectOption[] = list.map(l => {
      const id = ((l.name || l.code || '').trim() || 'Adet');
      return {
        id,
        primary: this.lookups.displayProductUnitLabel(l) || id,
      };
    });
    const current = (currentUnit ?? '').toString().trim();
    if (current && !opts.some(o => o.id === current)) {
      opts.unshift({
        id: current,
        primary: this.lookups.getName('ProductUnit', current) || current,
      });
    }
    return opts;
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
    public lookups: LookupService,
    private translate: TranslateService,
    private cdr: ChangeDetectorRef
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

  get orderCurrency(): string {
    if (this.loadedOrderCurrency) return this.loadedOrderCurrency;
    for (let i = 0; i < this.items.length; i++) {
      const pid = this.items.at(i).get('productId')?.value as string | null | undefined;
      if (pid) {
        const p = this.products.find(x => x.id === pid);
        if (p?.currency) return String(p.currency).toUpperCase();
      }
    }
    return 'TRY';
  }

  ngOnInit(): void {
    this.translate.onLangChange.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.cdr.markForCheck());
    this.id = this.route.snapshot.paramMap.get('id');
    if (!this.id) this.loadedOrderCurrency = null;
    this.lookups.loadLookupsAndDefaultVat().subscribe(({ defaultVat }) => {
      if (!this.id) {
        this.items.controls.forEach(c => c.get('vatRate')?.patchValue(defaultVat, { emitEvent: false }));
      }
    });

    this.customerApi.getDropdown().subscribe(list => this.customers = list);
    this.productApi.getDropdown().subscribe(list => this.products = list);
    this.reportApi.getStockLevels().subscribe(data => this.stockLevels = data);
    if (this.id) {
      this.editLoading = true;
      this.form.get('customerId')?.setValidators(Validators.required);
      this.form.get('customerId')?.updateValueAndValidity();
      this.orderApi.getById(this.id).subscribe({
        next: (o) => {
          this.orderNumber = o.orderNumber ?? null;
          const c = o.currency?.trim();
          this.loadedOrderCurrency = c ? c.toUpperCase() : null;
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
            this.items.push(this.fb.nonNullable.group({
              productId: [it.productId ?? null],
              productCode: [it.productCode || p?.code || ''],
              description: [it.description],
              unit: [it.unit || 'Adet'],
              quantity: [it.quantity],
              unitPrice: [it.unitPrice],
              vatRate: [it.vatRate],
              sortOrder: [idx]
            }));
          });
          this.editLoading = false;
        },
        error: () => {
          this.editLoading = false;
        }
      });
    }
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(e: MouseEvent): void {
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
      unit: ['Adet'],
      quantity: [1],
      unitPrice: [0],
      vatRate: [this.lookups.defaultVatRatePercent()],
      sortOrder: [0]
    });
  }

  onProductSelect(i: number, p: ProductDto): void {
    const g = this.items.at(i);
    g.patchValue({
      productId: p.id,
      productCode: p.code,
      description: p.name,
      unit: (p.unit || 'Adet').trim() || 'Adet',
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
    const t = e.target as HTMLElement;
    if (e.key === 'F9' && !this.saving && !this.editLoading && this.form.valid && !['INPUT', 'TEXTAREA', 'SELECT'].includes(t?.tagName) && !t?.closest('app-searchable-select')) {
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

    // Alınan sipariş (0 = Satis): yetersiz stok uyarısı. Verilen sipariş (1 = Alis): stok düşmez/artmaz; kontrol yok.
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
        unit: String(it.unit ?? '').trim() || 'Adet',
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
          unit: String(it.unit ?? '').trim() || 'Adet',
          quantity: Number(it.quantity) || 0,
          unitPrice: Number(it.unitPrice) || 0,
          vatRate: Number(it.vatRate) || 0,
          sortOrder: idx
        }))
      };
      this.orderApi.update(this.id, updateReq).subscribe({
        next: () => { this.toastr.success(this.translate.instant('orders.toastrUpdated')); this.router.navigate(['/orders']); },
        error: e => { this.error = e.error?.message ?? 'Kaydedilemedi.'; this.saving = false; }
      });
    } else {
      this.orderApi.create(req).subscribe({
        next: () => { this.toastr.success(this.translate.instant('orders.toastrCreated')); this.router.navigate(['/orders']); },
        error: e => { this.error = e.error?.message ?? 'Kaydedilemedi.'; this.saving = false; }
      });
    }
  }
}
