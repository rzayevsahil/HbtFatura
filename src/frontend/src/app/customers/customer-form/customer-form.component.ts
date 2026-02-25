import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CustomerService } from '../../services/customer.service';
import { MainAccountCodeService, MainAccountCodeDto } from '../../services/main-account-code.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-customer-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './customer-form.component.html',
  styleUrls: ['./customer-form.component.scss']
})
export class CustomerFormComponent implements OnInit {
  mainAccountCodes: MainAccountCodeDto[] = [];
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
    this.mainAccountCodeApi.getByFirm().subscribe(list => (this.mainAccountCodes = list));
    if (this.id) {
      this.api.getById(this.id).subscribe(c => this.form.patchValue({
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
      }));
    }
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
