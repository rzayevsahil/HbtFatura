import { Component, OnInit, computed, HostListener, ViewChild, ElementRef } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CompanyService } from '../../services/company.service';
import { TaxOfficeService, TaxOfficeDto, CityResponse, DistrictResponse } from '../../services/tax-office.service';
import { AuthService } from '../../core/services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-company-settings',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterLink],
  templateUrl: './company-settings.component.html',
  styleUrls: ['./company-settings.component.scss']
})
export class CompanySettingsComponent implements OnInit {
  isReadOnly = computed(() => this.auth.user()?.role === 'Employee');
  firmId: string | undefined;

  form = this.fb.nonNullable.group({
    companyName: ['', [Validators.required]],
    cityId: [null as string | null, [Validators.required]],
    districtId: [null as string | null, [Validators.required]],
    taxOfficeId: [null as string | null, [Validators.required]],
    taxNumber: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(11)]],
    address: ['', [Validators.required]],
    phone: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    website: [''],
    iban: ['', [Validators.required, Validators.minLength(32), Validators.maxLength(32)]],
    bankName: ['', [Validators.required]],
    logoUrl: [null as string | null]
  });
  error = '';
  saving = false;
  loading = true;

  cities: CityResponse[] = [];
  districts: DistrictResponse[] = [];
  offices: TaxOfficeDto[] = [];

  selectedCityName = '';
  selectedDistrictName = '';
  selectedOfficeName = '';

  citySearchText = '';
  cityDropdownOpen = false;

  districtSearchText = '';
  districtDropdownOpen = false;

  officeSearchText = '';
  officeDropdownOpen = false;

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

  get filteredOffices(): TaxOfficeDto[] {
    const t = this.officeSearchText?.trim().toLocaleLowerCase('tr');
    if (!t) return this.offices;
    return this.offices.filter(o => o.name.toLocaleLowerCase('tr').includes(t));
  }

  constructor(
    private fb: FormBuilder,
    private api: CompanyService,
    private taxService: TaxOfficeService,
    public auth: AuthService,
    private route: ActivatedRoute,
    private toastr: ToastrService,
    private location: Location,
    private el: ElementRef
  ) { }

  @HostListener('document:click', ['$event'])
  onDocumentClick(e: MouseEvent): void {
    if (!this.el.nativeElement.contains(e.target)) {
      this.cityDropdownOpen = false;
      this.districtDropdownOpen = false;
      this.officeDropdownOpen = false;
    }
  }

  toggleCityDropdown(): void {
    if (this.isReadOnly()) return;
    this.cityDropdownOpen = !this.cityDropdownOpen;
    this.districtDropdownOpen = false;
    this.officeDropdownOpen = false;
    if (this.cityDropdownOpen) this.citySearchText = '';
  }

  toggleDistrictDropdown(): void {
    if (this.isReadOnly() || !this.form.get('cityId')?.value) return;
    this.districtDropdownOpen = !this.districtDropdownOpen;
    this.cityDropdownOpen = false;
    this.officeDropdownOpen = false;
    if (this.districtDropdownOpen) this.districtSearchText = '';
  }

  toggleOfficeDropdown(): void {
    if (this.isReadOnly() || !this.form.get('districtId')?.value) return;
    this.officeDropdownOpen = !this.officeDropdownOpen;
    this.cityDropdownOpen = false;
    this.districtDropdownOpen = false;
    if (this.officeDropdownOpen) this.officeSearchText = '';
  }

  selectCity(city: CityResponse | null): void {
    this.onCityChange(city?.id || null, city?.name || '');
    this.cityDropdownOpen = false;
  }

  selectDistrict(district: DistrictResponse | null): void {
    this.onDistrictChange(district?.id || null, district?.name || '');
    this.districtDropdownOpen = false;
  }

  selectOffice(office: TaxOfficeDto | null): void {
    this.form.patchValue({ taxOfficeId: office?.id || null });
    this.selectedOfficeName = office?.name || '';
    this.officeDropdownOpen = false;
  }

  ngOnInit(): void {
    this.firmId = this.route.snapshot.queryParamMap.get('firmId') ?? undefined;
    this.loadCities();
    this.api.get(this.firmId).subscribe({
      next: c => {
        this.form.patchValue({
          companyName: c.companyName,
          cityId: c.cityId ?? null,
          districtId: c.districtId ?? null,
          taxOfficeId: c.taxOfficeId ?? null,
          taxNumber: c.taxNumber ?? '',
          address: c.address ?? '',
          phone: c.phone ?? '',
          email: c.email ?? '',
          website: c.website ?? '',
          iban: c.iban ?? '',
          bankName: c.bankName ?? '',
          logoUrl: c.logoUrl ?? null
        });

        this.selectedCityName = c.cityName ?? '';
        this.selectedDistrictName = c.districtName ?? '';
        this.selectedOfficeName = c.taxOfficeName ?? '';

        // Restore cascading dropdowns
        if (c.cityId) {
          this.taxService.getDistricts(c.cityId).subscribe(districts => {
            this.districts = districts;
            if (c.districtId) {
              this.taxService.getOffices(c.cityId!, c.districtId).subscribe(offices => {
                this.offices = offices;
                this.loading = false;
              });
            } else {
              this.loading = false;
            }
          });
        } else {
          this.loading = false;
        }
      },
      error: () => {
        this.loading = false; // 404 = no settings yet
      }
    });
  }

  loadCities(): void {
    this.taxService.getCities().subscribe(cities => {
      this.cities = cities;
    });
  }

  onCityChange(cityId: string | null, cityName: string): void {
    this.selectedCityName = cityName;
    this.selectedDistrictName = '';
    this.selectedOfficeName = '';
    this.districts = [];
    this.offices = [];
    this.districtSearchText = '';
    this.officeSearchText = '';
    this.form.patchValue({
      cityId: cityId,
      districtId: null,
      taxOfficeId: null
    });

    if (cityId) {
      this.taxService.getDistricts(cityId).subscribe(districts => {
        this.districts = districts;
      });
    }
  }

  onDistrictChange(districtId: string | null, districtName: string): void {
    this.selectedDistrictName = districtName;
    this.selectedOfficeName = '';
    this.offices = [];
    this.officeSearchText = '';
    this.form.patchValue({
      districtId: districtId,
      taxOfficeId: null
    });

    const cityId = this.form.get('cityId')?.value;
    if (cityId && districtId) {
      this.taxService.getOffices(cityId, districtId).subscribe(offices => {
        this.offices = offices;
      });
    }
  }

  onSubmit(): void {
    this.error = '';
    this.saving = true;
    this.api.save(this.form.getRawValue(), this.firmId).subscribe({
      next: () => {
        this.saving = false;
        this.toastr.success('Firma bilgileri kaydedildi.');
        this.location.back();
      },
      error: e => {
        this.error = e.error?.message ?? 'Hata';
        this.saving = false;
        this.toastr.error(e.error?.message ?? 'Kayıt sırasında hata oluştu.');
      },
      complete: () => { this.saving = false; }
    });
  }

  onLogoChange(event: any): void {
    const file = event.target.files[0];
    if (file) {
      if (file.size > 5 * 1024 * 1024) {
        this.toastr.warning('Logo boyutu 5MB\'dan küçük olmalıdır.');
        return;
      }
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.form.patchValue({ logoUrl: e.target.result });
      };
      reader.readAsDataURL(file);
    }
  }

  onIbanInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    let value = input.value.toUpperCase().replace(/[^A-Z0-9]/g, '');

    // Always start with TR
    if (value.length > 0 && !value.startsWith('TR')) {
      if (value.startsWith('T')) value = 'TR' + value.substring(1);
      else value = 'TR' + value;
    } else if (value.length === 0) {
      value = 'TR';
    }

    // Keep only TR and numbers
    const prefix = value.substring(0, 2);
    const rest = value.substring(2).replace(/[^0-9]/g, '');
    value = prefix + rest;

    // Limit to 26 characters (TR + 24 digits)
    if (value.length > 26) value = value.substring(0, 26);

    // Add spaces every 4 characters
    let formatted = '';
    for (let i = 0; i < value.length; i++) {
      if (i > 0 && i % 4 === 0) formatted += ' ';
      formatted += value[i];
    }

    this.form.patchValue({ iban: formatted }, { emitEvent: false });
    input.value = formatted;
  }

  getLogoUrl(): string | null {
    const url = this.form.get('logoUrl')?.value;
    if (!url) return null;
    if (url.startsWith('data:') || url.startsWith('http')) return url;
    // Prefix relative paths with API URL
    const baseUrl = environment.apiUrl.replace(/\/api\/?$/, '');
    return `${baseUrl}${url}`;
  }

  clearLogo(): void {
    this.form.patchValue({ logoUrl: null });
  }

  goBack(): void {
    this.location.back();
  }
}
