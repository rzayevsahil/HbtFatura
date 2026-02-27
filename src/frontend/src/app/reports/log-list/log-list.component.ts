import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LogService, LogEntry } from '../../services/log.service';

@Component({
    selector: 'app-log-list',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './log-list.component.html',
    styleUrls: ['./log-list.component.scss']
})
export class LogListComponent implements OnInit {
    items: LogEntry[] = [];
    totalCount = 0;
    page = 1;
    pageSize = 50;
    loading = false;

    // Filters
    level = '';
    module = '';
    dateFrom = '';
    dateTo = '';

    constructor(private api: LogService) { }

    ngOnInit(): void {
        this.load();
    }

    load(): void {
        this.loading = true;
        this.api.getPaged({
            page: this.page,
            pageSize: this.pageSize,
            level: this.level || undefined,
            module: this.module || undefined,
            dateFrom: this.dateFrom || undefined,
            dateTo: this.dateTo || undefined
        }).subscribe({
            next: (res) => {
                this.items = res.items;
                this.totalCount = res.totalCount;
                this.loading = false;
            },
            error: () => {
                this.loading = false;
            }
        });
    }

    getLevelClass(level: string): string {
        switch (level?.toLowerCase()) {
            case 'info': return 'log-info';
            case 'warning': return 'log-warning';
            case 'error': return 'log-error';
            default: return '';
        }
    }

    nextPage(): void {
        this.page++;
        this.load();
    }

    prevPage(): void {
        if (this.page > 1) {
            this.page--;
            this.load();
        }
    }
}
