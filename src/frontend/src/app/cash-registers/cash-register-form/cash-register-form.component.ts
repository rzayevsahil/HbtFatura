import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CashRegisterService } from '../../services/cash-register.service';
import { FirmService, FirmDto } from '../../services/firm.service';
import { AuthService } from '../../core/services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-cash-register-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './cash-register-form.component.html',
  styleUrls: ['./cash-register-form.component.scss']
})
export class CashRegisterFormComponent implements OnInit {
  form = this.fb.nonNullable.group({
    name: ['', Validators.required],
    currency: ['TRY'],
    isActive: [true],
    firmId: [null as string | null]
  });
  id: string | null = null;
  firms: FirmDto[] = [];
  error = '';
  saving = false;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private api: CashRegisterService,
    private firmApi: FirmService,
    public auth: AuthService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id');
    if (this.auth.user()?.role === 'SuperAdmin') {
      this.firmApi.getAll().subscribe(list => this.firms = list);
    } else {
      (this.form as FormGroup).removeControl('firmId');
    }
    if (this.id) {
      this.api.getById(this.id).subscribe(c => this.form.patchValue({
        name: c.name,
        currency: c.currency,
        isActive: c.isActive
      }));
    }
  }

  onSubmit(): void {
    this.error = '';
    this.saving = true;
    const v = this.form.getRawValue();
    const req: { name: string; currency: string; firmId?: string } = { name: v.name, currency: v.currency };
    if ('firmId' in v && v.firmId) req.firmId = v.firmId;

    if (this.id) {
      this.api.update(this.id, { name: v.name, isActive: v.isActive }).subscribe({
        next: () => {
          this.toastr.success('Kasa güncellendi.');
          this.router.navigate(['/cash-registers']);
        },
        error: e => {
          this.error = e.error?.message ?? 'Hata';
          this.saving = false;
          this.toastr.error(e.error?.message ?? 'Güncelleme sırasında hata oluştu.');
        },
        complete: () => { this.saving = false; }
      });
    } else {
      this.api.create(req).subscribe({
        next: () => {
          this.toastr.success('Kasa oluşturuldu.');
          this.router.navigate(['/cash-registers']);
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
