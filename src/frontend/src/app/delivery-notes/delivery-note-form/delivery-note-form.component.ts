import { Component, OnInit, HostListener, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { DeliveryNoteService } from '../../services/delivery-note.service';
import { CustomerService } from '../../services/customer.service';
import { ProductService } from '../../services/product.service';
import { OrderService } from '../../services/order.service';
import {
  CreateDeliveryNoteRequest, UpdateDeliveryNoteRequest,
  DeliveryNoteItemInputDto, CustomerDto, ProductDto, OrderListDto
} from '../../core/models';
import { ToastrService } from 'ngx-toastr';
import { LookupService } from '../../core/services/lookup.service';
import { SearchableSelectComponent, SearchableSelectOption } from '../../shared/searchable-select/searchable-select.component';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-delivery-note-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, RouterLink, SearchableSelectComponent, TranslateModule],
  templateUrl: './delivery-note-form.component.html',
  styleUrls: ['./delivery-note-form.component.scss']
})
export class DeliveryNoteFormComponent implements OnInit {
  @ViewChild('orderDropdownWrap') orderDropdownWrap?: ElementRef<HTMLElement>;

  form: FormGroup;
  id: string | null = null;
  editLoading = false;
  deliveryNumber: string | null = null;
  /** Düzenlemede API’den gelen özet para birimi; yoksa satırlardaki ürünlerden türetilir. */
  loadedDnCurrency: string | null = null;
  deliveryStatus: string | number | undefined; // düzenlemede salt okunur göstermek için
  customers: CustomerDto[] = [];
  products: ProductDto[] = [];
  productFilterText = '';
  error = '';
  saving = false;
  activeItemIndex: number | null = null;
  activeItemField: 'code' | 'description' | null = null;
  confirmedOrders: OrderListDto[] = [];
  orderSearchText = '';
  orderDropdownOpen = false;

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

  get filteredProducts(): ProductDto[] {
    const t = (this.productFilterText || '').trim().toLowerCase();
    if (!t) return this.products;
    return this.products.filter(p =>
      (p.code?.toLowerCase().includes(t)) || (p.name?.toLowerCase().includes(t))
    );
  }

  get filteredOrders(): OrderListDto[] {
    const q = (this.orderSearchText || '').trim().toLowerCase();
    if (!q) return this.confirmedOrders;
    return this.confirmedOrders.filter(o =>
      o.orderNumber.toLowerCase().includes(q) ||
      (o.customerTitle || '').toLowerCase().includes(q)
    );
  }

  get selectedOrderNumber(): string {
    const oid = this.form.get('orderId')?.value;
    if (!oid) return 'Seçiniz';
    const o = this.confirmedOrders.find(x => x.id === oid);
    return o ? `${o.orderNumber} (${o.customerTitle})` : 'Seçiniz';
  }

  get deliveryTypeSearchableOptions(): SearchableSelectOption[] {
    return this.lookups.getGroup('DeliveryNoteType')().map(l => ({
      id: String(l.code),
      primary: l.name
    }));
  }

  unitSearchableOptionsForLine(currentUnit: unknown): SearchableSelectOption[] {
    const list = this.lookups.getGroup('ProductUnit')();
    const opts: SearchableSelectOption[] = list.map(l => {
      const id = ((l.name || l.code || '').trim() || 'Adet');
      return {
        id,
        primary: this.lookups.displayLookupLabel(l) || id
      };
    });
    const current = (currentUnit ?? '').toString().trim();
    if (current && !opts.some(o => o.id === current)) {
      opts.unshift({ id: current, primary: current });
    }
    return opts;
  }

  /** Düzenlemede salt okunur durum etiketi (API sayı veya string dönebilir). */
  statusLabel(s: string | number | undefined): string {
    return this.lookups.getName('DeliveryNoteStatus', s);
  }

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private deliveryNoteApi: DeliveryNoteService,
    private customerApi: CustomerService,
    private productApi: ProductService,
    private orderApi: OrderService,
    private toastr: ToastrService,
    public lookups: LookupService,
    private translate: TranslateService
  ) {
    this.form = this.fb.nonNullable.group({
      customerId: this.fb.control<string | null>(null, Validators.required),
      orderId: [null as string | null],
      deliveryDate: [this.toDatetimeLocalValue(new Date()), Validators.required],
      deliveryType: [0 as number, Validators.required],
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

  get dnCurrency(): string {
    if (this.loadedDnCurrency) return this.loadedDnCurrency;
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
    this.id = this.route.snapshot.paramMap.get('id');
    if (!this.id) this.loadedDnCurrency = null;
    this.lookups.loadLookupsAndDefaultVat().subscribe(({ defaultVat }) => {
      if (!this.id) {
        this.items.controls.forEach(c => c.get('vatRate')?.patchValue(defaultVat, { emitEvent: false }));
      }
    });

    this.customerApi.getDropdown().subscribe(list => this.customers = list);
    this.productApi.getDropdown().subscribe(list => this.products = list);

    // Onaylanan veya kısmi teslim edilen siparişleri getir
    this.orderApi.getPaged({ page: 1, pageSize: 100, status: 3 }).subscribe(res => {
      this.confirmedOrders = [...this.confirmedOrders, ...res.items];
    });
    this.orderApi.getPaged({ page: 1, pageSize: 100, status: 4 }).subscribe(res => {
      this.confirmedOrders = [...this.confirmedOrders, ...res.items];
    });

    if (this.id) {
      this.editLoading = true;
      this.deliveryNoteApi.getById(this.id).subscribe({
        next: dn => {
          this.deliveryNumber = dn.deliveryNumber ?? null;
          this.deliveryStatus = dn.status;
          const cur = dn.currency?.trim();
          this.loadedDnCurrency = cur ? cur.toUpperCase() : null;
          this.form.patchValue({
            customerId: dn.customerId ?? null,
            orderId: dn.orderId ?? null,
            deliveryDate: this.toDatetimeLocalValue(dn.deliveryDate),
            deliveryType: dn.deliveryType ?? 0
          });
          this.items.clear();
          dn.items.forEach(it => {
            const p = this.products.find(x => x.id === it.productId);
            this.items.push(this.fb.nonNullable.group({
              productId: [it.productId ?? null],
              productCode: [it.productCode || p?.code || ''],
              orderItemId: [it.orderItemId ?? null],
              description: [it.description],
              unit: [it.unit || 'Adet'],
              quantity: [it.quantity],
              unitPrice: [it.unitPrice],
              vatRate: [it.vatRate],
              sortOrder: [it.sortOrder]
            }));
          });
          this.editLoading = false;
        },
        error: () => {
          this.editLoading = false;
          this.router.navigate(['/delivery-notes']);
        }
      });
    }
  }

  createItemGroup(): FormGroup {
    return this.fb.nonNullable.group({
      productId: [null as string | null],
      productCode: [''],
      orderItemId: [null as string | null],
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

  addItem(): void {
    this.items.push(this.createItemGroup());
  }

  removeItem(i: number): void {
    this.items.removeAt(i);
  }

  toggleOrderDropdown(): void {
    this.orderDropdownOpen = !this.orderDropdownOpen;
    if (this.orderDropdownOpen) this.orderSearchText = '';
  }

  selectOrder(orderId: string | null): void {
    this.orderDropdownOpen = false;
    if (!orderId) {
      this.form.patchValue({ orderId: null });
      return;
    }

    this.orderApi.getById(orderId).subscribe({
      next: o => {
        this.form.patchValue({
          customerId: o.customerId ?? null,
          orderId: o.id,
          deliveryType: o.orderType ?? 0
        });
        this.items.clear();
        o.items.forEach(it => {
          this.items.push(this.fb.nonNullable.group({
            productId: [it.productId ?? null],
            productCode: [it.productCode || ''],
            orderItemId: [it.id],
            description: [it.description],
            unit: [it.unit || 'Adet'],
            quantity: [it.quantity],
            unitPrice: [it.unitPrice],
            vatRate: [it.vatRate],
            sortOrder: [it.sortOrder]
          }));
        });
        if (this.items.length === 0) {
          this.addItem();
        }
        this.toastr.info(this.translate.instant('deliveryNotes.toastrOrderInfo', { no: o.orderNumber }));
      },
      error: () => this.toastr.error('Sipariş bilgileri yüklenemedi.')
    });
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

  /** API'ye gönderilecek tarih: ISO 8601 (yyyy-MM-ddTHH:mm:ss). */
  toDeliveryDateApiValue(value: string): string {
    if (!value || typeof value !== 'string') return new Date().toISOString().slice(0, 19);
    const s = value.trim();
    if (s.length === 10) return s + 'T00:00:00';
    if (s.length === 16) return s + ':00';
    return s.slice(0, 19);
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(e: MouseEvent): void {
    if (this.orderDropdownOpen && this.orderDropdownWrap?.nativeElement && !this.orderDropdownWrap.nativeElement.contains(e.target as Node)) {
      this.orderDropdownOpen = false;
    }
    const target = e.target as HTMLElement;
    if (!target.closest('.autocomplete-container')) {
      this.activeItemIndex = null;
      this.activeItemField = null;
    }
  }

  onSubmit(): void {
    this.error = '';
    this.saving = true;
    const v = this.form.getRawValue();
    const itemDtos: DeliveryNoteItemInputDto[] = v.items.map((it: any, i: number) => ({
      productId: it.productId ?? undefined,
      orderItemId: it.orderItemId ?? undefined,
      description: it.description ?? '',
      unit: String(it.unit ?? '').trim() || 'Adet',
      quantity: Number(it.quantity),
      unitPrice: Number(it.unitPrice),
      vatRate: Number(it.vatRate),
      sortOrder: i
    }));
    if (this.id) {
      const req: UpdateDeliveryNoteRequest = {
        customerId: v.customerId ?? undefined,
        deliveryDate: this.toDeliveryDateApiValue(v.deliveryDate),
        items: itemDtos
      };
      this.deliveryNoteApi.update(this.id, req).subscribe({
        next: () => {
          this.toastr.success(this.translate.instant('deliveryNotes.toastrUpdated'));
          this.router.navigate(['/delivery-notes', this.id]);
        },
        error: e => {
          this.error = e.error?.message ?? 'Hata';
          this.saving = false;
          this.toastr.error(e.error?.message ?? this.translate.instant('deliveryNotes.updateError'));
        },
        complete: () => { this.saving = false; }
      });
    } else {
      const req: CreateDeliveryNoteRequest = {
        customerId: v.customerId ?? undefined,
        orderId: v.orderId ?? undefined,
        deliveryDate: this.toDeliveryDateApiValue(v.deliveryDate),
        deliveryType: v.deliveryType ?? 0,
        items: itemDtos
      };
      this.deliveryNoteApi.create(req).subscribe({
        next: (dto) => {
          this.toastr.success(this.translate.instant('deliveryNotes.toastrCreated'));
          this.router.navigate(['/delivery-notes', dto.id]);
        },
        error: e => {
          this.error = e.error?.message ?? 'Hata';
          this.saving = false;
          this.toastr.error(e.error?.message ?? this.translate.instant('deliveryNotes.saveErrorKayit'));
        },
        complete: () => { this.saving = false; }
      });
    }
  }
}
