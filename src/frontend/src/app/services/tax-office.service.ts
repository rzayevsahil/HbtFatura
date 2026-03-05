import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../core/services/api.service';

export interface CityResponse {
    id: string;
    name: string;
}

export interface DistrictResponse {
    id: string;
    name: string;
}

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

    getCities(): Observable<CityResponse[]> {
        return this.api.get<CityResponse[]>(`${this.base}/cities`);
    }

    getDistricts(cityId: string): Observable<DistrictResponse[]> {
        return this.api.get<DistrictResponse[]>(`${this.base}/districts/${cityId}`);
    }

    getOffices(cityId: string, districtId: string): Observable<TaxOfficeDto[]> {
        return this.api.get<TaxOfficeDto[]>(`${this.base}/offices/${cityId}/${districtId}`);
    }
}
