import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CustomerService } from '../../services/customer.service';
import { MainAccountCodeService, MainAccountCodeDto } from '../../services/main-account-code.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-customer-form',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterLink],
  templateUrl: './customer-form.component.html',
  styleUrls: ['./customer-form.component.scss']
})
export class CustomerFormComponent implements OnInit {
  mainAccountCodes: MainAccountCodeDto[] = [];
  mainAccountCodeSearch = '';
  selectedMainAccount: MainAccountCodeDto | null = null;
  showMainAccountDropdown = false;

  get filteredMainAccountCodes(): MainAccountCodeDto[] {
    const q = this.mainAccountCodeSearch.trim().toLowerCase();
    if (!q) return this.mainAccountCodes;
    return this.mainAccountCodes.filter(
      (x) =>
        (x.code ?? '').toLowerCase().includes(q) ||
        (x.name ?? '').toLowerCase().includes(q)
    );
  }

  form = this.fb.nonNullable.group({
    mainAccountCode: [''],
    code: [''],
    accountType: [1 as number],
    title: ['', Validators.required],
    taxPayerType: [2 as number],
    cardType: [1 as number],
    taxNumber: [''],
    address: [''],
    city: [''],
    district: [''],
    postalCode: [''],
    country: [''],
    phone: [''],
    email: ['']
  });
  id: string | null = null;
  error = '';
  saving = false;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private api: CustomerService,
    private mainAccountCodeApi: MainAccountCodeService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id');
    this.mainAccountCodeApi.getByFirm().subscribe(list => {
      this.mainAccountCodes = list;
      this.onMainAccountCodesLoaded();
    });
    if (this.id) {
      this.api.getById(this.id).subscribe(c => {
        this.form.patchValue({
          mainAccountCode: c.mainAccountCode ?? '',
          code: c.code ?? '',
          accountType: c.accountType ?? 1,
          title: c.title,
          taxPayerType: c.taxPayerType ?? 2,
          cardType: c.cardType ?? 1,
          taxNumber: c.taxNumber ?? '',
          address: c.address ?? '',
          city: c.city ?? '',
          district: c.district ?? '',
          postalCode: c.postalCode ?? '',
          country: c.country ?? '',
          phone: c.phone ?? '',
          email: c.email ?? ''
        });
        this.onMainAccountCodesLoaded();
      });
    }
  }

  onMainAccountCodesLoaded(): void {
    const code = this.form.getRawValue().mainAccountCode;
    if (code) {
      this.selectedMainAccount =
        this.mainAccountCodes.find((x) => x.code === code) ?? null;
    }
  }

  selectMainAccountCode(item: MainAccountCodeDto): void {
    this.selectedMainAccount = item;
    this.form.patchValue({ mainAccountCode: item.code ?? '' });
    this.mainAccountCodeSearch = '';
    this.showMainAccountDropdown = false;
  }

  clearMainAccountCode(): void {
    this.selectedMainAccount = null;
    this.form.patchValue({ mainAccountCode: '' });
    this.mainAccountCodeSearch = '';
    this.showMainAccountDropdown = false;
  }

  onSubmit(): void {
    this.error = '';
    this.saving = true;
    const v = this.form.getRawValue();
    if (this.id) {
      this.api.update(this.id, v).subscribe({
        next: () => {
          this.toastr.success('Cari güncellendi.');
          this.router.navigate(['/customers']);
        },
        error: e => {
          this.error = e.error?.message ?? 'Hata';
          this.saving = false;
          this.toastr.error(e.error?.message ?? 'Güncelleme sırasında hata oluştu.');
        },
        complete: () => { this.saving = false; }
      });
    } else {
      this.api.create(v).subscribe({
        next: () => {
          this.toastr.success('Cari eklendi.');
          this.router.navigate(['/customers']);
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
