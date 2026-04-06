import { ChangeDetectorRef, Component, DestroyRef, OnInit, HostListener, ViewChild, ElementRef, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { taxNumberValidator } from '../../core/validators/tax-number.validator';
import { taxNumberUniqueAsyncValidator } from '../../core/validators/tax-number-unique.async-validator';
import { TaxNumberValidationService } from '../../core/services/tax-number-validation.service';
import { InvoiceService } from '../../services/invoice.service';
import { CustomerService } from '../../services/customer.service';
import { ProductService } from '../../services/product.service';
import { DeliveryNoteService } from '../../services/delivery-note.service';
import { CreateInvoiceRequest, InvoiceItemInputDto, CustomerDto, ProductDto, DeliveryNoteListDto } from '../../core/models';
import { ToastrService } from 'ngx-toastr';
import { LookupService } from '../../core/services/lookup.service';
import { SearchableSelectComponent, SearchableSelectOption } from '../../shared/searchable-select/searchable-select.component';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-invoice-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, RouterLink, SearchableSelectComponent, TranslateModule],
  templateUrl: './invoice-form.component.html',
  styleUrls: ['./invoice-form.component.scss']
})
export class InvoiceFormComponent implements OnInit {
  private readonly destroyRef = inject(DestroyRef);

  @ViewChild('dnDropdownWrap') dnDropdownWrap?: ElementRef<HTMLElement>;

  form: FormGroup;
  id: string | null = null;
  editLoading = false;
  /** İrsaliyeden oluşturulmuş faturada tip değiştirilemez. */
  invoiceSourceId: string | null = null;
  customers: CustomerDto[] = [];
  products: ProductDto[] = [];
  productFilterText = '';
  error = '';
  saving = false;
  activeItemIndex: number | null = null;
  activeItemField: 'code' | 'description' | null = null;
  confirmedDeliveryNotes: DeliveryNoteListDto[] = [];
  dnSearchText = '';
  dnDropdownOpen = false;

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

  get filteredDeliveryNotes(): DeliveryNoteListDto[] {
    const q = (this.dnSearchText || '').trim().toLowerCase();
    if (!q) return this.confirmedDeliveryNotes;
    return this.confirmedDeliveryNotes.filter(dn =>
      dn.deliveryNumber.toLowerCase().includes(q) ||
      (dn.customerTitle || '').toLowerCase().includes(q)
    );
  }

  get invoiceTypeSearchableOptions(): SearchableSelectOption[] {
    return this.lookups.getGroup('InvoiceType')().map(l => ({
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

  get selectedDeliveryNoteNumber(): string {
    const dnid = this.form.get('deliveryNoteId')?.value;
    if (!dnid) return this.translate.instant('common.selectPlease');
    const dn = this.confirmedDeliveryNotes.find(x => x.id === dnid);
    return dn ? `${dn.deliveryNumber} (${dn.customerTitle})` : this.translate.instant('common.selectPlease');
  }

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private invoiceApi: InvoiceService,
    private customerApi: CustomerService,
    private productApi: ProductService,
    private deliveryNoteApi: DeliveryNoteService,
    private toastr: ToastrService,
    public lookups: LookupService,
    private taxNumberValidation: TaxNumberValidationService,
    private translate: TranslateService,
    private cdr: ChangeDetectorRef
  ) {
    this.form = this.fb.nonNullable.group({
      invoiceType: [0 as number, Validators.required], // 0=Satış, 1=Alış
      customerId: this.fb.control<string | null>(null),
      deliveryNoteId: [null as string | null],
      customerTitle: ['', Validators.required],
      customerTaxNumber: ['', [Validators.required, Validators.maxLength(11), taxNumberValidator({ validateTcknChecksum: true })]],
      customerAddress: ['', Validators.required],
      customerPhone: [''],
      customerEmail: [''],
      customerWebsite: [''],
      customerTaxOffice: [''],
      invoiceDate: [this.formatDateForInput(new Date()), Validators.required],
      currency: ['TRY'],
      exchangeRate: [1],
      items: this.fb.array([this.createItemGroup()])
    });
  }

  get currency(): string {
    return this.form.get('currency')?.value || 'TRY';
  }

  get totalNet(): number {
    const items = this.items.controls;
    return items.reduce((sum, c) => {
      const qty = Number(c.get('quantity')?.value || 0);
      const price = Number(c.get('unitPrice')?.value || 0);
      const discount = Number(c.get('discountPercent')?.value || 0);
      const lineNet = qty * price * (1 - discount / 100);
      return sum + lineNet;
    }, 0);
  }

  get totalVat(): number {
    const items = this.items.controls;
    return items.reduce((sum, c) => {
      const qty = Number(c.get('quantity')?.value || 0);
      const price = Number(c.get('unitPrice')?.value || 0);
      const discount = Number(c.get('discountPercent')?.value || 0);
      const vatRate = Number(c.get('vatRate')?.value || 0);
      const lineNet = qty * price * (1 - discount / 100);
      return sum + lineNet * vatRate / 100;
    }, 0);
  }

  get totalGross(): number {
    return this.totalNet + this.totalVat;
  }

  ngOnInit(): void {
    this.translate.onLangChange.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.cdr.markForCheck());
    this.id = this.route.snapshot.paramMap.get('id');
    this.lookups.load().subscribe();

    this.customerApi.getDropdown().subscribe(list => this.customers = list);
    this.productApi.getDropdown().subscribe(list => this.products = list);

    this.form.get('customerId')?.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(cid => {
      if (cid) this.onCustomerSelect();
    });

    // Onaylanan (faturaya hazır) irsaliyeleri getir
    this.deliveryNoteApi.getPaged({ page: 1, pageSize: 100, status: 1 }).subscribe(res => {
      this.confirmedDeliveryNotes = res.items;
    });

    if (this.id) {
      this.editLoading = true;
      this.invoiceApi.getById(this.id).subscribe({
        next: (inv) => {
          this.invoiceSourceId = inv.sourceId ?? null;
          this.form.patchValue({
            invoiceType: inv.invoiceType ?? 0,
            customerId: inv.customerId ?? null,
            customerTitle: inv.customerTitle,
            customerTaxNumber: inv.customerTaxNumber ?? '',
            customerAddress: inv.customerAddress ?? '',
            customerPhone: inv.customerPhone ?? '',
            customerEmail: inv.customerEmail ?? '',
            customerWebsite: inv.customerWebsite ?? '',
            customerTaxOffice: inv.customerTaxOffice ?? '',
            invoiceDate: inv.invoiceDate ? this.formatDateForInput(new Date(inv.invoiceDate)) : '',
            currency: inv.currency,
            exchangeRate: inv.exchangeRate
          });
          this.items.clear();
          inv.items.forEach(it => {
            const p = this.products.find(x => x.id === it.productId);
            this.items.push(this.fb.nonNullable.group({
              productId: [it.productId ?? null],
              productCode: [p?.code || ''],
              description: [it.description],
              unit: [it.unit || 'Adet'],
              quantity: [it.quantity],
              unitPrice: [it.unitPrice],
              vatRate: [it.vatRate],
              discountPercent: [it.discountPercent ?? 0],
              sortOrder: [it.sortOrder]
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

  createItemGroup(): FormGroup {
    return this.fb.nonNullable.group({
      productId: [null as string | null],
      productCode: [''],
      description: [''],
      unit: ['Adet'],
      quantity: [1],
      unitPrice: [0],
      vatRate: [this.lookups.defaultVatRatePercent()],
      discountPercent: [0],
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

  toggleDnDropdown(): void {
    this.dnDropdownOpen = !this.dnDropdownOpen;
    if (this.dnDropdownOpen) this.dnSearchText = '';
  }

  selectDeliveryNote(dnId: string | null): void {
    this.dnDropdownOpen = false;
    if (!dnId) {
      this.form.patchValue({ deliveryNoteId: null });
      return;
    }

    this.deliveryNoteApi.getById(dnId).subscribe({
      next: dn => {
        this.form.patchValue({
          customerId: dn.customerId ?? null,
          deliveryNoteId: dn.id,
          invoiceType: dn.deliveryType ?? 0,
        });

        // Müşteri detaylarını çek
        if (dn.customerId) {
          const c = this.customers.find(x => x.id === dn.customerId);
          if (c) {
            this.form.patchValue({
              customerTitle: c.title,
              customerTaxNumber: c.taxNumber ?? '',
              customerAddress: c.address ?? '',
              customerPhone: c.phone ?? '',
              customerEmail: c.email ?? '',
              customerWebsite: c.website ?? '',
              customerTaxOffice: c.taxOfficeName ?? ''
            });
          }
        }

        this.items.clear();
        dn.items.forEach(it => {
          this.items.push(this.fb.nonNullable.group({
            productId: [it.productId ?? null],
            productCode: [it.productCode || ''],
            description: [it.description],
            unit: [it.unit || 'Adet'],
            quantity: [it.quantity],
            unitPrice: [it.unitPrice],
            vatRate: [it.vatRate],
            discountPercent: [0],
            sortOrder: [it.sortOrder]
          }));
        });
        if (this.items.length === 0) {
          this.addItem();
        }
        this.toastr.info(`${dn.deliveryNumber} nolu irsaliye bilgileri aktarıldı.`);
      },
      error: () => this.toastr.error('İrsaliye bilgileri yüklenemedi.')
    });
  }

  @HostListener('document:keydown', ['$event'])
  onKeyDown(e: KeyboardEvent): void {
    const t = e.target as HTMLElement;
    if (e.key === 'F9' && !this.saving && !this.editLoading && !this.form.pending && this.form.valid && !['INPUT', 'TEXTAREA', 'SELECT'].includes(t?.tagName) && !t?.closest('app-searchable-select')) {
      e.preventDefault();
      this.onSubmit();
    }
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(e: MouseEvent): void {
    if (this.dnDropdownOpen && this.dnDropdownWrap?.nativeElement && !this.dnDropdownWrap.nativeElement.contains(e.target as Node)) {
      this.dnDropdownOpen = false;
    }
    const target = e.target as HTMLElement;
    if (!target.closest('.autocomplete-container')) {
      this.activeItemIndex = null;
      this.activeItemField = null;
    }
  }

  onCustomerSelect(): void {
    const cid = this.form.get('customerId')?.value;
    const c = this.customers.find(x => x.id === cid);
    if (c) {
      this.form.patchValue({
        customerTitle: c.title,
        customerTaxNumber: c.taxNumber ?? '',
        customerAddress: c.address ?? '',
        customerPhone: c.phone ?? '',
        customerEmail: c.email ?? '',
        customerWebsite: c.website ?? '',
        customerTaxOffice: c.taxOfficeName ?? ''
      });
    }
  }

  onSubmit(): void {
    this.error = '';
    if (this.form.pending) {
      this.error = this.translate.instant('invoices.taxPendingWait');
      return;
    }
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving = true;
    const v = this.form.getRawValue();
    const req: CreateInvoiceRequest = {
      invoiceDate: v.invoiceDate,
      invoiceType: v.invoiceType ?? 0,
      customerId: v.customerId ?? undefined,
      customerTitle: v.customerTitle,
      customerTaxNumber: v.customerTaxNumber || undefined,
      customerAddress: v.customerAddress || undefined,
      customerPhone: v.customerPhone || undefined,
      customerEmail: v.customerEmail || undefined,
      customerWebsite: v.customerWebsite || undefined,
      customerTaxOffice: v.customerTaxOffice || undefined,
      currency: v.currency,
      exchangeRate: v.exchangeRate,
      deliveryNoteId: v.deliveryNoteId ?? undefined,
      items: v.items.map((it: any, i: number) => ({
        productId: it.productId ?? undefined,
        description: it.description,
        unit: String(it.unit ?? '').trim() || 'Adet',
        quantity: Number(it.quantity),
        unitPrice: Number(it.unitPrice),
        vatRate: Number(it.vatRate),
        discountPercent: Number(it.discountPercent) || 0,
        sortOrder: i
      } as InvoiceItemInputDto))
    };
    if (this.id) {
      this.invoiceApi.update(this.id, req).subscribe({
        next: () => {
          this.toastr.success(this.translate.instant('invoices.toastrUpdated'));
          this.router.navigate(['/invoices']);
        },
        error: e => {
          this.error = e.error?.message ?? 'Hata';
          this.saving = false;
          this.toastr.error(e.error?.message ?? this.translate.instant('invoices.toastrUpdateError'));
        },
        complete: () => { this.saving = false; }
      });
    } else {
      this.invoiceApi.create(req).subscribe({
        next: () => {
          this.toastr.success(this.translate.instant('invoices.toastrCreated'));
          this.router.navigate(['/invoices']);
        },
        error: e => {
          this.error = e.error?.message ?? 'Hata';
          this.saving = false;
          this.toastr.error(e.error?.message ?? this.translate.instant('invoices.toastrSaveError'));
        },
        complete: () => { this.saving = false; }
      });
    }
  }

  private formatDateForInput(date: Date): string {
    const d = new Date(date);
    d.setMinutes(d.getMinutes() - d.getTimezoneOffset());
    return d.toISOString().slice(0, 16);
  }
}
