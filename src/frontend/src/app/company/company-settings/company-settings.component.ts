import { Component, OnInit, computed } from '@angular/core';
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

  constructor(
    private fb: FormBuilder,
    private api: CompanyService,
    private taxService: TaxOfficeService,
    public auth: AuthService,
    private route: ActivatedRoute,
    private toastr: ToastrService,
    private location: Location
  ) { }

  ngOnInit(): void {
    this.firmId = this.route.snapshot.queryParamMap.get('firmId') ?? undefined;
    this.loadCities();
    this.api.get(this.firmId).subscribe({
      next: c => {
        this.form.patchValue({
          companyName: c.companyName,
          taxOffice: c.taxOffice ?? '',
          taxNumber: c.taxNumber ?? '',
          address: c.address ?? '',
          phone: c.phone ?? '',
          email: c.email ?? '',
          iban: c.iban ?? '',
          bankName: c.bankName ?? ''
        });
        this.loading = false;
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
    this.form.patchValue({ taxOffice: '' });

    if (city) {
      this.taxService.getDistricts(city).subscribe(districts => {
        this.districts = districts;
      });
    }
  }

  onDistrictChange(district: string): void {
    this.selectedDistrict = district;
    this.offices = [];
    this.form.patchValue({ taxOffice: '' });

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
