import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface DashboardStat {
    label: string;
    value: string;
    icon: string;
    color: string;
    trend?: string;
}

export interface RecentInvoice {
    id: string;
    customer: string;
    amount: string;
    date: string;
    status: string;
}

export interface RecentActivity {
    id: string;
    type: string;
    title: string;
    description: string;
    time: string;
}

export interface DashboardData {
    stats: DashboardStat[];
    recentInvoices: RecentInvoice[];
    activities: RecentActivity[];
}

@Injectable({ providedIn: 'root' })
export class DashboardService {
    constructor(private http: HttpClient) { }

    getSummary(): Observable<DashboardData> {
        return this.http.get<DashboardData>('/api/dashboard');
    }
}
