import { Component, OnInit, HostListener, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { InvoiceService, CreateInvoiceRequest, InvoiceItemInputDto } from '../../services/invoice.service';
import { CustomerService, CustomerDto } from '../../services/customer.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-invoice-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './invoice-form.component.html',
  styleUrls: ['./invoice-form.component.scss']
})
export class InvoiceFormComponent implements OnInit {
  @ViewChild('customerDropdownWrap') customerDropdownWrap?: ElementRef<HTMLElement>;

  form: FormGroup;
  id: string | null = null;
  customers: CustomerDto[] = [];
  customerSearchText = '';
  customerDropdownOpen = false;
  error = '';
  saving = false;

  get items(): FormArray {
    return this.form.get('items') as FormArray;
  }

  get selectedCustomerTitle(): string {
    const cid = this.form.get('customerId')?.value;
    if (!cid) return 'Manuel gir';
    const c = this.customers.find(x => x.id === cid);
    return c?.title ?? 'Manuel gir';
  }

  get filteredCustomers(): CustomerDto[] {
    const q = (this.customerSearchText || '').trim().toLowerCase();
    if (!q) return this.customers;
    return this.customers.filter(c => c.title.toLowerCase().includes(q));
  }

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private invoiceApi: InvoiceService,
    private customerApi: CustomerService,
    private toastr: ToastrService
  ) {
    this.form = this.fb.nonNullable.group({
      customerId: this.fb.control<string | null>(null),
      customerTitle: ['', Validators.required],
      customerTaxNumber: [''],
      customerAddress: [''],
      customerPhone: [''],
      customerEmail: [''],
      invoiceDate: [new Date().toISOString().slice(0, 10), Validators.required],
      currency: ['TRY'],
      exchangeRate: [1],
      items: this.fb.array([this.createItemGroup()])
    });
  }

  ngOnInit(): void {
    this.customerApi.getDropdown().subscribe(list => this.customers = list);
    this.id = this.route.snapshot.paramMap.get('id');
    if (this.id) {
      this.invoiceApi.getById(this.id).subscribe(inv => {
        this.form.patchValue({
          customerId: inv.customerId ?? null,
          customerTitle: inv.customerTitle,
          customerTaxNumber: inv.customerTaxNumber ?? '',
          customerAddress: inv.customerAddress ?? '',
          customerPhone: inv.customerPhone ?? '',
          customerEmail: inv.customerEmail ?? '',
          invoiceDate: inv.invoiceDate.slice(0, 10),
          currency: inv.currency,
          exchangeRate: inv.exchangeRate
        });
        this.items.clear();
        inv.items.forEach(it => this.items.push(this.fb.nonNullable.group({
          description: [it.description],
          quantity: [it.quantity],
          unitPrice: [it.unitPrice],
          vatRate: [it.vatRate],
          sortOrder: [it.sortOrder]
        })));
      });
    }
  }

  createItemGroup(): FormGroup {
    return this.fb.nonNullable.group({
      description: [''],
      quantity: [1],
      unitPrice: [0],
      vatRate: [18],
      sortOrder: [0]
    });
  }

  addItem(): void {
    this.items.push(this.createItemGroup());
  }

  removeItem(i: number): void {
    this.items.removeAt(i);
  }

  toggleCustomerDropdown(): void {
    this.customerDropdownOpen = !this.customerDropdownOpen;
    if (this.customerDropdownOpen) this.customerSearchText = '';
  }

  selectCustomer(c: CustomerDto | null): void {
    this.form.patchValue({ customerId: c?.id ?? null });
    if (c) this.onCustomerSelect();
    this.customerDropdownOpen = false;
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(e: MouseEvent): void {
    if (this.customerDropdownOpen && this.customerDropdownWrap?.nativeElement && !this.customerDropdownWrap.nativeElement.contains(e.target as Node)) {
      this.customerDropdownOpen = false;
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
        customerEmail: c.email ?? ''
      });
    }
  }

  onSubmit(): void {
    this.error = '';
    this.saving = true;
    const v = this.form.getRawValue();
    const req: CreateInvoiceRequest = {
      invoiceDate: v.invoiceDate,
      customerId: v.customerId ?? undefined,
      customerTitle: v.customerTitle,
      customerTaxNumber: v.customerTaxNumber || undefined,
      customerAddress: v.customerAddress || undefined,
      customerPhone: v.customerPhone || undefined,
      customerEmail: v.customerEmail || undefined,
      currency: v.currency,
      exchangeRate: v.exchangeRate,
      items: v.items.map((it: any, i: number) => ({
        description: it.description,
        quantity: Number(it.quantity),
        unitPrice: Number(it.unitPrice),
        vatRate: Number(it.vatRate),
        sortOrder: i
      } as InvoiceItemInputDto))
    };
    if (this.id) {
      this.invoiceApi.update(this.id, req).subscribe({
        next: () => {
          this.toastr.success('Fatura güncellendi.');
          this.router.navigate(['/invoices']);
        },
        error: e => {
          this.error = e.error?.message ?? 'Hata';
          this.saving = false;
          this.toastr.error(e.error?.message ?? 'Güncelleme sırasında hata oluştu.');
        },
        complete: () => { this.saving = false; }
      });
    } else {
      this.invoiceApi.create(req).subscribe({
        next: () => {
          this.toastr.success('Fatura oluşturuldu.');
          this.router.navigate(['/invoices']);
        },
        error: e => {
          this.error = e.error?.message ?? 'Hata';
          this.saving = false;
          this.toastr.error(e.error?.message ?? 'Kayıt sırasında hata oluştu.');
        },
        complete: () => { this.saving = false; }
      });
    }
  }
}
