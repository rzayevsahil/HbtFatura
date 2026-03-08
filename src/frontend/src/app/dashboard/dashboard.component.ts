import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../core/services/auth.service';
import { DashboardService, DashboardData } from '../services/dashboard.service';

@Component({
    selector: 'app-dashboard',
    standalone: true,
    imports: [CommonModule, RouterLink],
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
    data = signal<DashboardData | null>(null);
    loading = signal(true);

    constructor(
        public auth: AuthService,
        private dashboardService: DashboardService
    ) { }

    ngOnInit(): void {
        this.refresh();
    }

    refresh(): void {
        this.loading.set(true);
        this.dashboardService.getSummary().subscribe({
            next: (res) => {
                this.data.set(res);
                this.loading.set(false);
            },
            error: () => this.loading.set(false)
        });
    }
}
