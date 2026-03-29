import { Component, OnInit, HostListener, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { taxNumberValidator } from '../../core/validators/tax-number.validator';
import { taxNumberUniqueAsyncValidator } from '../../core/validators/tax-number-unique.async-validator';
import { TaxNumberValidationService } from '../../core/services/tax-number-validation.service';
import { CustomerService } from '../../services/customer.service';
import { MainAccountCodeService } from '../../services/main-account-code.service';
import { TaxOfficeService } from '../../services/tax-office.service';
import { MainAccountCodeDto, CityResponse, DistrictResponse, TaxOfficeDto, CustomerDto } from '../../core/models';
import { ToastrService } from 'ngx-toastr';
import { PhoneFormatter } from '../../core/utils/phone-formatter';

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

  cities: CityResponse[] = [];
  districts: DistrictResponse[] = [];

  selectedCityName = '';
  selectedDistrictName = '';

  citySearchText = '';
  cityDropdownOpen = false;

  districtSearchText = '';
  districtDropdownOpen = false;

  taxOffices: TaxOfficeDto[] = [];
  selectedTaxOfficeName = '';
  taxOfficeSearchText = '';
  taxOfficeDropdownOpen = false;

  get filteredCities(): CityResponse[] {
    const t = this.citySearchText?.trim().toLocaleLowerCase('tr');
    if (!t) return this.cities;
    return this.cities.filter(c => c.name.toLocaleLowerCase('tr').includes(t));
  }

  get filteredDistricts(): DistrictResponse[] {
    const t = this.districtSearchText?.trim().toLocaleLowerCase('tr');
    if (!t) return this.districts;
    return this.districts.filter(d => d.name.toLocaleLowerCase('tr').includes(t));
  }

  get filteredTaxOffices(): TaxOfficeDto[] {
    const t = this.taxOfficeSearchText?.trim().toLocaleLowerCase('tr');
    if (!t) return this.taxOffices;
    return this.taxOffices.filter(o => o.name.toLocaleLowerCase('tr').includes(t));
  }

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
    title: ['', Validators.required],
    taxPayerType: [2 as number],
    cardType: [1 as number],
    taxNumber: ['', [taxNumberValidator({ validateTcknChecksum: true })]],
    address: [''],
    cityId: [null as string | null],
    districtId: [null as string | null],
    taxOfficeId: [null as string | null],
    postalCode: [''],
    country: [''],
    phone: ['', [Validators.minLength(14), Validators.maxLength(14)]],
    email: [''],
    website: ['']
  });
  id: string | null = null;
  /** Düzenlemede cari API’den gelene kadar */
  formInitialLoading = false;
  error = '';
  saving = false;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private api: CustomerService,
    private mainAccountCodeApi: MainAccountCodeService,
    private taxOfficeApi: TaxOfficeService,
    private toastr: ToastrService,
    private el: ElementRef,
    private taxNumberValidation: TaxNumberValidationService
  ) { }

  @HostListener('document:click', ['$event'])
  onDocumentClick(e: MouseEvent): void {
    if (!this.el.nativeElement.contains(e.target)) {
      this.cityDropdownOpen = false;
      this.districtDropdownOpen = false;
      this.taxOfficeDropdownOpen = false;
    }
  }

  toggleCityDropdown(): void {
    this.cityDropdownOpen = !this.cityDropdownOpen;
    this.districtDropdownOpen = false;
    if (this.cityDropdownOpen) this.citySearchText = '';
  }

  toggleDistrictDropdown(): void {
    if (!this.form.get('cityId')?.value) return;
    this.districtDropdownOpen = !this.districtDropdownOpen;
    this.cityDropdownOpen = false;
    this.taxOfficeDropdownOpen = false;
    if (this.districtDropdownOpen) this.districtSearchText = '';
  }

  toggleTaxOfficeDropdown(): void {
    if (!this.form.get('districtId')?.value) return;
    this.taxOfficeDropdownOpen = !this.taxOfficeDropdownOpen;
    this.cityDropdownOpen = false;
    this.districtDropdownOpen = false;
    if (this.taxOfficeDropdownOpen) this.taxOfficeSearchText = '';
  }

  selectCity(city: CityResponse | null): void {
    this.onCityChange(city?.id || null, city?.name || '');
    this.cityDropdownOpen = false;
  }

  selectDistrict(district: DistrictResponse | null): void {
    this.onDistrictChange(district?.id || null, district?.name || '');
    this.districtDropdownOpen = false;
  }

  selectTaxOffice(office: TaxOfficeDto | null): void {
    this.selectedTaxOfficeName = office?.name || '';
    this.form.patchValue({ taxOfficeId: office?.id || null });
    this.taxOfficeDropdownOpen = false;
  }

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id');
    if (this.id) {
      this.formInitialLoading = true;
    }
    const taxCtrl = this.form.get('taxNumber');
    taxCtrl?.addAsyncValidators(
      taxNumberUniqueAsyncValidator(this.taxNumberValidation, 'customer', {
        excludeCustomerId: () => this.id
      })
    );
    taxCtrl?.updateValueAndValidity({ emitEvent: false });

    this.mainAccountCodeApi.getByFirm().subscribe(list => {
      this.mainAccountCodes = list;
      this.onMainAccountCodesLoaded();
    });
    this.taxOfficeApi.getCities().subscribe(res => this.cities = res);
    if (this.id) {
      this.api.getById(this.id).subscribe({
        next: (c) => {
          this.form.patchValue({
            mainAccountCode: c.mainAccountCode ?? '',
            code: c.code ?? '',
            title: c.title,
            taxPayerType: c.taxPayerType ?? 2,
            cardType: c.cardType ?? 1,
            taxNumber: c.taxNumber ?? '',
            address: c.address ?? '',
            cityId: c.cityId,
            districtId: c.districtId,
            taxOfficeId: c.taxOfficeId,
            postalCode: c.postalCode ?? '',
            country: c.country ?? '',
            phone: c.phone ? PhoneFormatter.format(c.phone) : '',
            email: c.email ?? '',
            website: c.website ?? ''
          });

          if (c.cityId) {
            this.selectedCityName = c.cityName ?? '';
            this.selectedDistrictName = c.districtName ?? '';
            this.taxOfficeApi.getDistricts(c.cityId).subscribe(res => {
              this.districts = res;
              this.form.patchValue({ districtId: c.districtId });

              if (c.cityId && c.districtId) {
                this.selectedTaxOfficeName = c.taxOfficeName ?? '';
                this.taxOfficeApi.getOffices(c.cityId, c.districtId).subscribe(offices => {
                  this.taxOffices = offices;
                  this.form.patchValue({ taxOfficeId: c.taxOfficeId });
                });
              }
            });
          }
          this.onMainAccountCodesLoaded();
          this.formInitialLoading = false;
        },
        error: () => {
          this.formInitialLoading = false;
        }
      });
    }
  }

  onCityChange(id: string | null, name: string): void {
    this.selectedCityName = name;
    this.selectedDistrictName = '';
    this.selectedTaxOfficeName = '';
    this.districts = [];
    this.taxOffices = [];
    this.districtSearchText = '';
    this.taxOfficeSearchText = '';
    this.form.patchValue({ cityId: id, districtId: null, taxOfficeId: null });
    if (id) {
      this.taxOfficeApi.getDistricts(id).subscribe(res => this.districts = res);
    }
  }

  onDistrictChange(id: string | null, name: string): void {
    this.selectedDistrictName = name;
    this.selectedTaxOfficeName = '';
    this.taxOffices = [];
    this.taxOfficeSearchText = '';
    this.form.patchValue({ districtId: id, taxOfficeId: null });
    const cityId = this.form.get('cityId')?.value;
    if (cityId && id) {
      this.taxOfficeApi.getOffices(cityId, id).subscribe(res => this.taxOffices = res);
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

  onPhoneInput(event: Event): void {
    const formatted = PhoneFormatter.handleInput(event);
    this.form.patchValue({ phone: formatted });
  }

  onSubmit(): void {
    this.error = '';
    if (this.form.pending) {
      this.error = 'Vergi no / TC kontrolü tamamlanıyor, lütfen kısa süre bekleyin.';
      return;
    }
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.error = 'Lütfen zorunlu alanları doldurun (Cari adı).';
      return;
    }
    this.saving = true;
    const v = this.form.getRawValue();
    const payload = {
      mainAccountCode: v.mainAccountCode?.trim() ?? '',
      code: v.code?.trim() ?? '',
      title: v.title?.trim() ?? '',
      taxPayerType: typeof v.taxPayerType === 'number' ? v.taxPayerType : 2,
      cardType: typeof v.cardType === 'number' ? v.cardType : 1,
      taxNumber: (v.taxNumber ?? '').replace(/\D/g, '').slice(0, 11),
      address: v.address?.trim() ?? '',
      cityId: v.cityId,
      districtId: v.districtId,
      taxOfficeId: v.taxOfficeId,
      postalCode: v.postalCode?.trim() ?? '',
      country: v.country?.trim() ?? '',
      phone: v.phone?.trim() ?? '',
      email: v.email?.trim() ?? '',
      website: v.website?.trim() ?? ''
    };
    if (this.id) {
      this.api.update(this.id, payload).subscribe({
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
      this.api.create(payload).subscribe({
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
