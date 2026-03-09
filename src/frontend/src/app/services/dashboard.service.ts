import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { DashboardData } from '../core/models';

@Injectable({ providedIn: 'root' })
export class DashboardService {
    constructor(private http: HttpClient) { }

    getSummary(): Observable<DashboardData> {
        return this.http.get<DashboardData>('/api/dashboard');
    }
}
