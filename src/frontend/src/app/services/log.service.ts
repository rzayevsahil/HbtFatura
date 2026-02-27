import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService, PagedResult } from '../core/services/api.service';

export interface LogEntry {
    id: string;
    timestamp: string;
    level: string;
    message: string;
    action?: string;
    module?: string;
    userId?: string;
    userFullName?: string;
    ipAddress?: string;
    details?: string;
}

@Injectable({ providedIn: 'root' })
export class LogService {
    private base = '/api/logs';

    constructor(private api: ApiService) { }

    getPaged(params: {
        page: number;
        pageSize: number;
        level?: string;
        module?: string;
        dateFrom?: string;
        dateTo?: string;
    }): Observable<PagedResult<LogEntry>> {
        const p: any = { page: params.page, pageSize: params.pageSize };
        if (params.level) p.level = params.level;
        if (params.module) p.module = params.module;
        if (params.dateFrom) p.dateFrom = params.dateFrom;
        if (params.dateTo) p.dateTo = params.dateTo;

        return this.api.get<PagedResult<LogEntry>>(this.base, p);
    }
}
