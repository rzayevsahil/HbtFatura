import { Component, OnInit, HostListener, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { DeliveryNoteService, CreateDeliveryNoteRequest, UpdateDeliveryNoteRequest, DeliveryNoteItemInputDto } from '../../services/delivery-note.service';
import { CustomerService, CustomerDto } from '../../services/customer.service';
import { ProductService, ProductDto } from '../../services/product.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-delivery-note-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, RouterLink],
  templateUrl: './delivery-note-form.component.html',
  styleUrls: ['./delivery-note-form.component.scss']
})
export class DeliveryNoteFormComponent implements OnInit {
  @ViewChild('customerDropdownWrap') customerDropdownWrap?: ElementRef<HTMLElement>;

  form: FormGroup;
  id: string | null = null;
  customers: CustomerDto[] = [];
  products: ProductDto[] = [];
  productFilterText = '';
  customerSearchText = '';
  customerDropdownOpen = false;
  error = '';
  saving = false;

  get items(): FormArray {
    return this.form.get('items') as FormArray;
  }

  get selectedCustomerTitle(): string {
    const cid = this.form.get('customerId')?.value;
    if (!cid) return 'Seçiniz';
    const c = this.customers.find(x => x.id === cid);
    return c?.title ?? 'Seçiniz';
  }

  get filteredCustomers(): CustomerDto[] {
    const q = (this.customerSearchText || '').trim().toLowerCase();
    if (!q) return this.customers;
    return this.customers.filter(c => c.title.toLowerCase().includes(q));
  }

  get filteredProducts(): ProductDto[] {
    const t = (this.productFilterText || '').trim().toLowerCase();
    if (!t) return this.products;
    return this.products.filter(p =>
      (p.code?.toLowerCase().includes(t)) || (p.name?.toLowerCase().includes(t))
    );
  }

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private deliveryNoteApi: DeliveryNoteService,
    private customerApi: CustomerService,
    private productApi: ProductService,
    private toastr: ToastrService
  ) {
    this.form = this.fb.nonNullable.group({
      customerId: this.fb.control<string | null>(null),
      deliveryDate: [new Date().toISOString().slice(0, 10), Validators.required],
      deliveryType: [0 as number, Validators.required],
      items: this.fb.array([this.createItemGroup()])
    });
  }

  ngOnInit(): void {
    this.customerApi.getDropdown().subscribe(list => this.customers = list);
    this.productApi.getDropdown().subscribe(list => this.products = list);
    this.id = this.route.snapshot.paramMap.get('id');
    if (this.id) {
      this.deliveryNoteApi.getById(this.id).subscribe({
        next: dn => {
          this.form.patchValue({
            customerId: dn.customerId ?? null,
            deliveryDate: dn.deliveryDate.slice(0, 10),
            deliveryType: dn.deliveryType ?? 0
          });
          this.items.clear();
          dn.items.forEach(it => this.items.push(this.fb.nonNullable.group({
            productId: [it.productId ?? null],
            description: [it.description],
            quantity: [it.quantity],
            unitPrice: [it.unitPrice],
            vatRate: [it.vatRate],
            sortOrder: [it.sortOrder]
          })));
        },
        error: () => this.router.navigate(['/delivery-notes'])
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
    const p = this.products.find((x: ProductDto) => x.id === productId);
    if (p) g.patchValue({ description: p.name });
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
    this.customerDropdownOpen = false;
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(e: MouseEvent): void {
    if (this.customerDropdownOpen && this.customerDropdownWrap?.nativeElement && !this.customerDropdownWrap.nativeElement.contains(e.target as Node)) {
      this.customerDropdownOpen = false;
    }
  }

  onSubmit(): void {
    this.error = '';
    this.saving = true;
    const v = this.form.getRawValue();
    const itemDtos: DeliveryNoteItemInputDto[] = v.items.map((it: any, i: number) => ({
      productId: it.productId ?? undefined,
      orderItemId: undefined,
      description: it.description ?? '',
      quantity: Number(it.quantity),
      unitPrice: Number(it.unitPrice),
      vatRate: Number(it.vatRate),
      sortOrder: i
    }));
    if (this.id) {
      const req: UpdateDeliveryNoteRequest = {
        customerId: v.customerId ?? undefined,
        deliveryDate: v.deliveryDate,
        items: itemDtos
      };
      this.deliveryNoteApi.update(this.id, req).subscribe({
        next: () => {
          this.toastr.success('İrsaliye güncellendi.');
          this.router.navigate(['/delivery-notes', this.id]);
        },
        error: e => {
          this.error = e.error?.message ?? 'Hata';
          this.saving = false;
          this.toastr.error(e.error?.message ?? 'Güncelleme sırasında hata oluştu.');
        },
        complete: () => { this.saving = false; }
      });
    } else {
      const req: CreateDeliveryNoteRequest = {
        customerId: v.customerId ?? undefined,
        orderId: undefined,
        deliveryDate: v.deliveryDate,
        deliveryType: v.deliveryType ?? 0,
        items: itemDtos
      };
      this.deliveryNoteApi.create(req).subscribe({
        next: (dto) => {
          this.toastr.success('İrsaliye oluşturuldu.');
          this.router.navigate(['/delivery-notes', dto.id]);
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
