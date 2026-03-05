import { Component, OnInit, HostListener, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { taxNumberValidator } from '../../core/validators/tax-number.validator';
import { CustomerService } from '../../services/customer.service';
import { MainAccountCodeService, MainAccountCodeDto } from '../../services/main-account-code.service';
import { TaxOfficeService, CityResponse, DistrictResponse } from '../../services/tax-office.service';
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

  cities: CityResponse[] = [];
  districts: DistrictResponse[] = [];

  selectedCityName = '';
  selectedDistrictName = '';

  citySearchText = '';
  cityDropdownOpen = false;

  districtSearchText = '';
  districtDropdownOpen = false;

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
    taxNumber: ['', [taxNumberValidator()]],
    address: [''],
    cityId: [null as string | null],
    districtId: [null as string | null],
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
    private taxOfficeApi: TaxOfficeService,
    private toastr: ToastrService,
    private el: ElementRef
  ) { }

  @HostListener('document:click', ['$event'])
  onDocumentClick(e: MouseEvent): void {
    if (!this.el.nativeElement.contains(e.target)) {
      this.cityDropdownOpen = false;
      this.districtDropdownOpen = false;
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
    if (this.districtDropdownOpen) this.districtSearchText = '';
  }

  selectCity(city: CityResponse | null): void {
    this.onCityChange(city?.id || null, city?.name || '');
    this.cityDropdownOpen = false;
  }

  selectDistrict(district: DistrictResponse | null): void {
    this.selectedDistrictName = district?.name || '';
    this.form.patchValue({ districtId: district?.id || null });
    this.districtDropdownOpen = false;
  }

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id');
    this.mainAccountCodeApi.getByFirm().subscribe(list => {
      this.mainAccountCodes = list;
      this.onMainAccountCodesLoaded();
    });
    this.taxOfficeApi.getCities().subscribe(res => this.cities = res);
    if (this.id) {
      this.api.getById(this.id).subscribe(c => {
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
          postalCode: c.postalCode ?? '',
          country: c.country ?? '',
          phone: c.phone ?? '',
          email: c.email ?? ''
        });

        if (c.cityId) {
          this.selectedCityName = c.cityName ?? '';
          this.selectedDistrictName = c.districtName ?? '';
          this.taxOfficeApi.getDistricts(c.cityId).subscribe(res => {
            this.districts = res;
            this.form.patchValue({ districtId: c.districtId });
          });
        }
        this.onMainAccountCodesLoaded();
      });
    }
  }

  onCityChange(id: string | null, name: string): void {
    this.selectedCityName = name;
    this.selectedDistrictName = '';
    this.districts = [];
    this.districtSearchText = '';
    this.form.patchValue({ cityId: id, districtId: null });
    if (id) {
      this.taxOfficeApi.getDistricts(id).subscribe(res => this.districts = res);
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
      taxNumber: v.taxNumber?.trim() ?? '',
      address: v.address?.trim() ?? '',
      cityId: v.cityId,
      districtId: v.districtId,
      postalCode: v.postalCode?.trim() ?? '',
      country: v.country?.trim() ?? '',
      phone: v.phone?.trim() ?? '',
      email: v.email?.trim() ?? ''
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
