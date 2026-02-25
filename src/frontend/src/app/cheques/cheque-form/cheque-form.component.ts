import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ChequeService } from '../../services/cheque.service';
import { CustomerService, CustomerDto } from '../../services/customer.service';
import { BankAccountService, BankAccountDto } from '../../services/bank-account.service';
import { AuthService } from '../../core/services/auth.service';
import { FirmService } from '../../services/firm.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-cheque-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './cheque-form.component.html',
  styleUrls: ['./cheque-form.component.scss']
})
export class ChequeFormComponent implements OnInit {
  form = this.fb.nonNullable.group({
    type: [1 as number, Validators.required],
    customerId: ['', Validators.required],
    amount: [0, [Validators.required, Validators.min(0.01)]],
    issueDate: [new Date().toISOString().slice(0, 10), Validators.required],
    dueDate: [new Date().toISOString().slice(0, 10), Validators.required],
    bankAccountId: [null as string | null],
    firmId: [null as string | null]
  });
  id: string | null = null;
  customers: CustomerDto[] = [];
  bankAccounts: BankAccountDto[] = [];
  firms: { id: string; name: string }[] = [];
  error = '';
  saving = false;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private api: ChequeService,
    public auth: AuthService,
    private customerApi: CustomerService,
    private bankApi: BankAccountService,
    private firmApi: FirmService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.customerApi.getDropdown().subscribe(c => this.customers = c);
    this.bankApi.getAll().subscribe(b => this.bankAccounts = b.filter(x => x.isActive));
    if (this.auth.user()?.role === 'SuperAdmin') this.firmApi.getAll().subscribe(f => this.firms = f);
    this.id = this.route.snapshot.paramMap.get('id');
    if (this.id) {
      this.api.getById(this.id).subscribe(c => this.form.patchValue({
        type: c.type,
        customerId: c.customerId,
        amount: c.amount,
        issueDate: c.issueDate.slice(0, 10),
        dueDate: c.dueDate.slice(0, 10),
        bankAccountId: c.bankAccountId ?? null
      }));
    }
  }

  onSubmit(): void {
    this.error = '';
    this.saving = true;
    const v = this.form.getRawValue();
    const payload = {
      type: v.type,
      customerId: v.customerId,
      amount: v.amount,
      issueDate: v.issueDate,
      dueDate: v.dueDate,
      bankAccountId: v.bankAccountId ?? undefined,
      firmId: (this.auth.user()?.role === 'SuperAdmin' && v.firmId) ? v.firmId : undefined
    };
    if (this.id) {
      this.api.update(this.id, payload).subscribe({
        next: () => {
          this.toastr.success('Kayıt güncellendi.');
          this.router.navigate(['/cheques']);
        },
        error: e => {
          this.error = e.error?.message ?? 'Hata';
          this.saving = false;
        },
        complete: () => { this.saving = false; }
      });
    } else {
      this.api.create(payload).subscribe({
        next: () => {
          this.toastr.success('Kayıt eklendi.');
          this.router.navigate(['/cheques']);
        },
        error: e => {
          this.error = e.error?.message ?? 'Hata';
          this.saving = false;
        },
        complete: () => { this.saving = false; }
      });
    }
  }
}
