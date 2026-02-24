import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FirmService, FirmDto } from '../../services/firm.service';
import { CompanyService, CompanySettingsDto } from '../../services/company.service';

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

  constructor(
    private route: ActivatedRoute,
    private firmApi: FirmService,
    private companyApi: CompanyService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.firmApi.getById(id).subscribe(f => {
        this.firm = f;
        this.companyApi.get(f.id).subscribe({
          next: c => (this.company = c),
          error: () => (this.company = null)
        });
      });
    }
  }
}
