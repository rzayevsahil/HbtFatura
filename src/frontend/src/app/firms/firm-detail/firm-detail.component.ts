import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FirmService } from '../../services/firm.service';
import { CompanyService } from '../../services/company.service';
import { FirmDto, FirmUserDto, CompanySettingsDto } from '../../core/models';
import { environment } from '../../../environments/environment';
import { AuthService } from '../../core/services/auth.service';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-firm-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslateModule],
  templateUrl: './firm-detail.component.html',
  styleUrls: ['./firm-detail.component.scss']
})
export class FirmDetailComponent implements OnInit {
  firm: FirmDto | null = null;
  company: CompanySettingsDto | null = null;
  firmUsers: FirmUserDto[] = [];
  loading = true;

  constructor(
    private route: ActivatedRoute,
    private firmApi: FirmService,
    private companyApi: CompanyService,
    public auth: AuthService,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    let id = this.route.snapshot.paramMap.get('id');
    if (!id && this.auth.user()?.firmId) {
      id = this.auth.user()?.firmId!;
    }

    if (id) {
      this.firmApi.getById(id).subscribe({
        next: f => {
          this.firm = f;
          this.loading = false;
          this.companyApi.get(f.id).subscribe({
            next: c => (this.company = c),
            error: () => (this.company = null)
          });
          this.firmApi.getFirmUsers(f.id).subscribe({
            next: list => (this.firmUsers = list),
            error: () => (this.firmUsers = [])
          });
        },
        error: () => {
          this.loading = false;
          window.history.back();
        }
      });
    } else {
      this.loading = false;
      window.history.back();
    }
  }

  getLogoUrl(): string | null {
    if (!this.company?.logoUrl) return null;
    const url = this.company.logoUrl;
    if (url.startsWith('data:') || url.startsWith('http')) return url;
    // Prefix relative paths with API URL (remove /api if present at end)
    const baseUrl = environment.apiUrl.replace(/\/api\/?$/, '');
    return `${baseUrl}${url}`;
  }

  roleLabel(role: string): string {
    if (role === 'FirmAdmin') return this.translate.instant('firmProfile.roleFirmAdmin');
    if (role === 'Employee') return this.translate.instant('firmProfile.roleEmployee');
    return role;
  }
}
