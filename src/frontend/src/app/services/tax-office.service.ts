import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../core/services/api.service';

export interface TaxOfficeDto {
    id: string;
    city: string;
    district: string;
    name: string;
}

@Injectable({ providedIn: 'root' })
export class TaxOfficeService {
    private base = '/api/taxoffices';

    constructor(private api: ApiService) { }

    getCities(): Observable<string[]> {
        return this.api.get<string[]>(`${this.base}/cities`);
    }

    getDistricts(city: string): Observable<string[]> {
        return this.api.get<string[]>(`${this.base}/districts/${encodeURIComponent(city)}`);
    }

    getOffices(city: string, district: string): Observable<TaxOfficeDto[]> {
        return this.api.get<TaxOfficeDto[]>(`${this.base}/offices/${encodeURIComponent(city)}/${encodeURIComponent(district)}`);
    }
}
