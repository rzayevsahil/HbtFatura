import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FirmService } from '../../services/firm.service';
import { CompanyService } from '../../services/company.service';
import { FirmDto, FirmUserDto, CompanySettingsDto } from '../../core/models';
import { environment } from '../../../environments/environment';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-firm-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './firm-detail.component.html',
  styleUrls: ['./firm-detail.component.scss']
})
export class FirmDetailComponent implements OnInit {
  firm: FirmDto | null = null;
  company: CompanySettingsDto | null = null;
  firmUsers: FirmUserDto[] = [];

  constructor(
    private route: ActivatedRoute,
    private firmApi: FirmService,
    private companyApi: CompanyService,
    public auth: AuthService
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
          // If firm not found or unauthorized, go back
          window.history.back();
        }
      });
    } else {
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
    const map: Record<string, string> = { FirmAdmin: 'Firma Admin', Employee: 'Çalışan' };
    return map[role] ?? role;
  }
}
