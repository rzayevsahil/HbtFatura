import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CustomerService } from '../../services/customer.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-customer-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './customer-form.component.html',
  styleUrls: ['./customer-form.component.scss']
})
export class CustomerFormComponent implements OnInit {
  form = this.fb.nonNullable.group({
    title: ['', Validators.required],
    taxNumber: [''],
    address: [''],
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
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id');
    if (this.id) {
      this.api.getById(this.id).subscribe(c => this.form.patchValue({
        title: c.title,
        taxNumber: c.taxNumber ?? '',
        address: c.address ?? '',
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
          this.toastr.success('Müşteri güncellendi.');
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
          this.toastr.success('Müşteri eklendi.');
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
