import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CustomerService, CustomerDto } from '../../services/customer.service';
import { CashRegisterService, CashRegisterDto } from '../../services/cash-register.service';
import { BankAccountService, BankAccountDto } from '../../services/bank-account.service';
import { AccountPaymentService } from '../../services/account-payment.service';
import { InvoiceService, InvoiceListDto } from '../../services/invoice.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-account-payment-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './account-payment-form.component.html',
  styleUrls: ['./account-payment-form.component.scss']
})
export class AccountPaymentFormComponent implements OnInit {
  form = this.fb.nonNullable.group({
    type: ['Tahsilat' as string, Validators.required],
    customerId: ['', Validators.required],
    amount: [0, [Validators.required, Validators.min(0.01)]],
    date: [new Date().toISOString().slice(0, 10), Validators.required],
    paymentMethod: ['Kasa' as string, Validators.required],
    cashRegisterId: [''],
    bankAccountId: [''],
    description: [''],
    invoiceId: ['']
  });
  customers: CustomerDto[] = [];
  cashRegisters: CashRegisterDto[] = [];
  bankAccounts: BankAccountDto[] = [];
  customerInvoices: InvoiceListDto[] = [];
  error = '';
  saving = false;

  constructor(
    private fb: FormBuilder,
    private customerApi: CustomerService,
    private cashApi: CashRegisterService,
    private bankApi: BankAccountService,
    private paymentApi: AccountPaymentService,
    private invoiceApi: InvoiceService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.customerApi.getDropdown().subscribe(c => this.customers = c);
    this.cashApi.getAll().subscribe(c => this.cashRegisters = c.filter(x => x.isActive));
    this.bankApi.getAll().subscribe(b => this.bankAccounts = b.filter(x => x.isActive));
    this.form.patchValue({
      cashRegisterId: '',
      bankAccountId: '',
      invoiceId: ''
    });
    this.form.get('customerId')?.valueChanges.subscribe(customerId => {
      this.form.patchValue({ invoiceId: '' });
      this.customerInvoices = [];
      if (this.form.get('type')?.value === 'Tahsilat' && customerId) {
        this.invoiceApi.getPaged({ page: 1, pageSize: 200, customerId, status: 1 }).subscribe(res => {
          this.customerInvoices = res.items;
        });
      }
    });
    this.form.get('type')?.valueChanges.subscribe(t => {
      this.form.patchValue({ invoiceId: '' });
      const customerId = this.form.get('customerId')?.value;
      if (t === 'Tahsilat' && customerId) {
        this.invoiceApi.getPaged({ page: 1, pageSize: 200, customerId, status: 1 }).subscribe(res => {
          this.customerInvoices = res.items;
        });
      } else {
        this.customerInvoices = [];
      }
    });
  }

  get paymentMethod(): string {
    return this.form.get('paymentMethod')?.value ?? 'Kasa';
  }

  onSubmit(): void {
    this.error = '';
    const v = this.form.getRawValue();
    if (v.paymentMethod === 'Kasa' && !v.cashRegisterId) {
      this.error = 'Kasa seçiniz.';
      return;
    }
    if (v.paymentMethod === 'Banka' && !v.bankAccountId) {
      this.error = 'Banka hesabı seçiniz.';
      return;
    }
    this.saving = true;
    this.paymentApi.create({
      customerId: v.customerId,
      amount: v.amount,
      date: v.date,
      paymentMethod: v.paymentMethod,
      cashRegisterId: v.paymentMethod === 'Kasa' ? v.cashRegisterId || undefined : undefined,
      bankAccountId: v.paymentMethod === 'Banka' ? v.bankAccountId || undefined : undefined,
      description: v.description || (v.type === 'Tahsilat' ? 'Tahsilat' : 'Ödeme'),
      type: v.type,
      invoiceId: v.invoiceId || undefined
    }).subscribe({
      next: () => {
        this.toastr.success(v.type === 'Tahsilat' ? 'Tahsilat kaydedildi.' : 'Ödeme kaydedildi.');
        this.form.patchValue({ amount: 0, description: '' });
      },
      error: e => {
        this.error = e.error?.message ?? 'Hata';
        this.toastr.error(e.error?.message ?? 'Kayıt sırasında hata oluştu.');
        this.saving = false;
      },
      complete: () => { this.saving = false; }
    });
  }
}
