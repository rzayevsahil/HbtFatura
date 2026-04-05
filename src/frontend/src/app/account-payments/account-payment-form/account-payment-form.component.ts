import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators, FormsModule } from '@angular/forms';
import { CustomerService } from '../../services/customer.service';
import { CashRegisterService } from '../../services/cash-register.service';
import { BankAccountService } from '../../services/bank-account.service';
import { AccountPaymentService } from '../../services/account-payment.service';
import { InvoiceService } from '../../services/invoice.service';
import { CustomerDto, CashRegisterDto, BankAccountDto, InvoiceListDto, AccountPaymentMethod, AccountPaymentType } from '../../core/models';
import { ToastrService } from 'ngx-toastr';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { SearchableSelectComponent, SearchableSelectOption } from '../../shared/searchable-select/searchable-select.component';

@Component({
  selector: 'app-account-payment-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, RouterLink, TranslateModule, SearchableSelectComponent],
  templateUrl: './account-payment-form.component.html',
  styleUrls: ['./account-payment-form.component.scss']
})
export class AccountPaymentFormComponent implements OnInit {
  form = this.fb.nonNullable.group({
    type: ['Tahsilat' as string, Validators.required],
    customerId: ['', Validators.required],
    amount: [0, [Validators.required, Validators.min(0.01)]],
    dateTime: [new Date().toISOString().slice(0, 16), Validators.required],
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

  get paymentTypeSearchableOptions(): SearchableSelectOption[] {
    return [
      { id: 'Tahsilat', primary: this.translate.instant('payments.typeCollection') },
      { id: 'Odeme', primary: this.translate.instant('payments.typePayment') }
    ];
  }

  get customerSearchableOptions(): SearchableSelectOption[] {
    return this.customers.map(c => ({
      id: c.id,
      primary: c.title ?? '',
      secondary: c.code ?? ''
    }));
  }

  get paymentMethodSearchableOptions(): SearchableSelectOption[] {
    return [
      { id: 'Kasa', primary: this.translate.instant('payments.methodCash') },
      { id: 'Banka', primary: this.translate.instant('payments.methodBank') }
    ];
  }

  get cashRegisterSearchableOptions(): SearchableSelectOption[] {
    return this.cashRegisters.map(r => ({
      id: r.id,
      primary: r.name,
      secondary: `${this.formatAmount(r.balance)} ${r.currency ?? ''}`.trim()
    }));
  }

  /** Liste satırı alt metni (para birimi + bakiye); fatura satırındaki tutar satırıyla aynı fikir. */
  private formatAmount(value: number): string {
    return new Intl.NumberFormat('tr-TR', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    }).format(Number(value) || 0);
  }

  get bankAccountSearchableOptions(): SearchableSelectOption[] {
    return this.bankAccounts.map(b => ({
      id: b.id,
      primary: `${b.name}`,
      secondary: b.bankName ?? ''
    }));
  }

  get invoiceLinkSearchableOptions(): SearchableSelectOption[] {
    return this.customerInvoices.map(inv => ({
      id: inv.id,
      primary: `${inv.invoiceNumber}`,
      secondary: `${inv.grandTotal} ${inv.currency}`
    }));
  }

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private customerApi: CustomerService,
    private cashApi: CashRegisterService,
    private bankApi: BankAccountService,
    private paymentApi: AccountPaymentService,
    private invoiceApi: InvoiceService,
    private toastr: ToastrService,
    private translate: TranslateService
  ) { }

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
      this.error = this.translate.instant('payments.selectCashRequired');
      return;
    }
    if (v.paymentMethod === 'Banka' && !v.bankAccountId) {
      this.error = this.translate.instant('payments.selectBankRequired');
      return;
    }
    this.saving = true;
    const dateStr = (v.dateTime.length === 16 ? v.dateTime + ':00' : v.dateTime);
    this.paymentApi.create({
      customerId: v.customerId,
      amount: v.amount,
      date: dateStr,
      paymentMethod: v.paymentMethod as AccountPaymentMethod,
      cashRegisterId: v.paymentMethod === 'Kasa' ? v.cashRegisterId || undefined : undefined,
      bankAccountId: v.paymentMethod === 'Banka' ? v.bankAccountId || undefined : undefined,
      description: v.description || (v.type === 'Tahsilat'
        ? this.translate.instant('payments.defaultDescCollection')
        : this.translate.instant('payments.defaultDescPayment')),
      type: v.type as AccountPaymentType,
      invoiceId: v.invoiceId || undefined
    }).subscribe({
      next: () => {
        this.toastr.success(
          v.type === 'Tahsilat'
            ? this.translate.instant('payments.toastCollectionSaved')
            : this.translate.instant('payments.toastPaymentSaved')
        );
        this.router.navigate(['/payments']);
      },
      error: e => {
        this.error = e.error?.message ?? 'Hata';
        this.toastr.error(e.error?.message ?? this.translate.instant('payments.saveError'));
        this.saving = false;
      },
      complete: () => { this.saving = false; }
    });
  }
}
