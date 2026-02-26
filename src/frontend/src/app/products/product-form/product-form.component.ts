import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProductService } from '../../services/product.service';
import { AuthService } from '../../core/services/auth.service';
import { FirmService } from '../../services/firm.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.scss']
})
export class ProductFormComponent implements OnInit {
  form = this.fb.nonNullable.group({
    code: ['', Validators.required],
    name: ['', Validators.required],
    barcode: [''],
    unit: ['Adet'],
    stockQuantity: [0],
    firmId: [null as string | null]
  });
  id: string | null = null;
  firms: { id: string; name: string }[] = [];
  error = '';
  saving = false;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private api: ProductService,
    public auth: AuthService,
    private firmApi: FirmService,
    private toastr: ToastrService
  ) { }

  ngOnInit(): void {
    if (this.auth.user()?.role === 'SuperAdmin') {
      this.firmApi.getAll().subscribe(f => (this.firms = f));
    }
    this.id = this.route.snapshot.paramMap.get('id');
    if (this.id) {
      this.api.getById(this.id).subscribe(p => this.form.patchValue({
        code: p.code,
        name: p.name,
        barcode: p.barcode ?? '',
        unit: p.unit ?? 'Adet',
        stockQuantity: p.stockQuantity ?? 0
      }));
    }
  }

  onSubmit(): void {
    this.error = '';
    this.saving = true;
    const v = this.form.getRawValue();
    const payload = {
      code: v.code,
      name: v.name,
      barcode: v.barcode || undefined,
      unit: v.unit || 'Adet',
      stockQuantity: v.stockQuantity ?? 0,
      firmId: (this.auth.user()?.role === 'SuperAdmin' && v.firmId) ? v.firmId : undefined
    };
    if (this.id) {
      this.api.update(this.id, payload).subscribe({
        next: () => {
          this.toastr.success('Ürün güncellendi.');
          this.router.navigate(['/products']);
        },
        error: e => {
          this.error = e.error?.message ?? 'Hata';
          this.saving = false;
          this.toastr.error(e.error?.message ?? 'Güncelleme sırasında hata oluştu.');
        },
        complete: () => { this.saving = false; }
      });
    } else {
      this.api.create(payload).subscribe({
        next: () => {
          this.toastr.success('Ürün eklendi.');
          this.router.navigate(['/products']);
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
