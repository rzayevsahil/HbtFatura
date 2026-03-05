import { Component, OnInit, computed, HostListener, ViewChild, ElementRef } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CompanyService } from '../../services/company.service';
import { TaxOfficeService, TaxOfficeDto } from '../../services/tax-office.service';
import { AuthService } from '../../core/services/auth.service';
import { ToastrService } from 'ngx-toastr';

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
    companyName: ['', Validators.required],
    taxOffice: [''],
    taxOfficeCity: [''],
    taxOfficeDistrict: [''],
    taxNumber: [''],
    address: [''],
    phone: [''],
    email: [''],
    iban: [''],
    bankName: ['']
  });
  error = '';
  saving = false;
  loading = true;

  cities: string[] = [];
  districts: string[] = [];
  offices: TaxOfficeDto[] = [];

  selectedCity = '';
  selectedDistrict = '';

  citySearchText = '';
  cityDropdownOpen = false;

  districtSearchText = '';
  districtDropdownOpen = false;

  officeSearchText = '';
  officeDropdownOpen = false;

  get filteredCities(): string[] {
    const t = this.citySearchText?.trim().toLocaleLowerCase('tr');
    if (!t) return this.cities;
    return this.cities.filter(c => c.toLocaleLowerCase('tr').includes(t));
  }

  get filteredDistricts(): string[] {
    const t = this.districtSearchText?.trim().toLocaleLowerCase('tr');
    if (!t) return this.districts;
    return this.districts.filter(d => d.toLocaleLowerCase('tr').includes(t));
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
    if (this.isReadOnly() || !this.selectedCity) return;
    this.districtDropdownOpen = !this.districtDropdownOpen;
    this.cityDropdownOpen = false;
    this.officeDropdownOpen = false;
    if (this.districtDropdownOpen) this.districtSearchText = '';
  }

  toggleOfficeDropdown(): void {
    if (this.isReadOnly() || !this.selectedDistrict) return;
    this.officeDropdownOpen = !this.officeDropdownOpen;
    this.cityDropdownOpen = false;
    this.districtDropdownOpen = false;
    if (this.officeDropdownOpen) this.officeSearchText = '';
  }

  selectCity(city: string | null): void {
    this.onCityChange(city || '');
    this.cityDropdownOpen = false;
  }

  selectDistrict(district: string | null): void {
    this.onDistrictChange(district || '');
    this.districtDropdownOpen = false;
  }

  selectOffice(officeName: string | null): void {
    this.form.patchValue({ taxOffice: officeName || '' });
    this.officeDropdownOpen = false;
  }

  ngOnInit(): void {
    this.firmId = this.route.snapshot.queryParamMap.get('firmId') ?? undefined;
    this.loadCities();
    this.api.get(this.firmId).subscribe({
      next: c => {
        this.form.patchValue({
          companyName: c.companyName,
          taxOffice: c.taxOffice ?? '',
          taxOfficeCity: c.taxOfficeCity ?? '',
          taxOfficeDistrict: c.taxOfficeDistrict ?? '',
          taxNumber: c.taxNumber ?? '',
          address: c.address ?? '',
          phone: c.phone ?? '',
          email: c.email ?? '',
          iban: c.iban ?? '',
          bankName: c.bankName ?? ''
        });

        // Restore cascading dropdowns
        if (c.taxOfficeCity) {
          this.selectedCity = c.taxOfficeCity;
          this.taxService.getDistricts(c.taxOfficeCity).subscribe(districts => {
            this.districts = districts;
            if (c.taxOfficeDistrict) {
              this.selectedDistrict = c.taxOfficeDistrict;
              this.taxService.getOffices(c.taxOfficeCity!, c.taxOfficeDistrict).subscribe(offices => {
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

  onCityChange(city: string): void {
    this.selectedCity = city;
    this.selectedDistrict = '';
    this.districts = [];
    this.offices = [];
    this.districtSearchText = '';
    this.officeSearchText = '';
    this.form.patchValue({
      taxOffice: '',
      taxOfficeCity: city,
      taxOfficeDistrict: ''
    });

    if (city) {
      this.taxService.getDistricts(city).subscribe(districts => {
        this.districts = districts;
      });
    }
  }

  onDistrictChange(district: string): void {
    this.selectedDistrict = district;
    this.offices = [];
    this.officeSearchText = '';
    this.form.patchValue({
      taxOffice: '',
      taxOfficeDistrict: district
    });

    if (this.selectedCity && district) {
      this.taxService.getOffices(this.selectedCity, district).subscribe(offices => {
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

  goBack(): void {
    this.location.back();
  }
}
