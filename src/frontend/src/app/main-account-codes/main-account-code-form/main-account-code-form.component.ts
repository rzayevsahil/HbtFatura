import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MainAccountCodeService } from '../../services/main-account-code.service';
import { FirmService, FirmDto } from '../../services/firm.service';
import { AuthService } from '../../core/services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-main-account-code-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './main-account-code-form.component.html',
  styleUrls: ['./main-account-code-form.component.scss']
})
export class MainAccountCodeFormComponent implements OnInit {
  @Input() isModal = false;
  @Input() editId: string | null = null;
  @Output() saved = new EventEmitter<void>();
  @Output() cancel = new EventEmitter<void>();

  form = this.fb.nonNullable.group({
    code: ['', Validators.required],
    name: ['', Validators.required],
    sortOrder: [0],
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
    private api: MainAccountCodeService,
    private firmApi: FirmService,
    public auth: AuthService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.id = this.isModal ? this.editId : this.route.snapshot.paramMap.get('id');
    if (this.auth.user()?.role === 'SuperAdmin') {
      this.firmApi.getAll().subscribe(list => (this.firms = list));
    } else {
      (this.form as FormGroup).removeControl('firmId');
    }
    if (this.id) {
      this.api.getById(this.id).subscribe({
        next: item => {
          if (item.isSystem) {
            this.toastr.warning('Sistem kodları düzenlenemez.');
            if (this.isModal) this.cancel.emit(); else this.router.navigate(['/main-account-codes']);
            return;
          }
          this.form.patchValue({
            code: item.code,
            name: item.name,
            sortOrder: item.sortOrder ?? 0
          });
        },
        error: () => this.isModal ? this.cancel.emit() : this.router.navigate(['/main-account-codes'])
      });
    } else {
      this.api.getByFirm(undefined).subscribe(list => {
        const max = list.length ? Math.max(...list.map(x => x.sortOrder ?? 0)) : 0;
        this.form.patchValue({ sortOrder: max + 1 });
      });
    }
  }

  onSubmit(): void {
    this.error = '';
    this.saving = true;
    const v = this.form.getRawValue();
    const req = {
      code: v.code.trim(),
      name: v.name.trim(),
      sortOrder: v.sortOrder ?? 0,
      firmId: 'firmId' in v && v.firmId ? v.firmId : undefined
    };

    if (this.id) {
      this.api.update(this.id, { code: req.code, name: req.name, sortOrder: req.sortOrder }).subscribe({
        next: () => {
          this.toastr.success('Ana cari kodu güncellendi.');
          if (this.isModal) this.saved.emit(); else this.router.navigate(['/main-account-codes']);
        },
        error: e => {
          this.error = e.error?.message ?? 'Hata';
          this.saving = false;
          this.toastr.error(e.error?.message ?? 'Güncelleme sırasında hata oluştu.');
        },
        complete: () => {
          this.saving = false;
        }
      });
    } else {
      this.api.create(req).subscribe({
        next: () => {
          this.toastr.success('Ana cari kodu oluşturuldu.');
          if (this.isModal) this.saved.emit(); else this.router.navigate(['/main-account-codes']);
        },
        error: e => {
          this.error = e.error?.message ?? 'Hata';
          this.saving = false;
          this.toastr.error(e.error?.message ?? 'Kayıt sırasında hata oluştu.');
        },
        complete: () => {
          this.saving = false;
        }
      });
    }
  }
}
