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
