import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../core/services/api.service';

export interface UserProfileDto {
    id: string;
    email: string;
    fullName: string;
    phoneNumber?: string;
}

export interface UpdateProfileRequest {
    fullName: string;
    phoneNumber?: string;
    currentPassword?: string;
    newPassword?: string;
}

@Injectable({ providedIn: 'root' })
export class AccountService {
    private base = '/api/users';

    constructor(private api: ApiService) { }

    getProfile(): Observable<UserProfileDto> {
        return this.api.get<UserProfileDto>(`${this.base}/profile`);
    }

    updateProfile(req: UpdateProfileRequest): Observable<void> {
        return this.api.put<void>(`${this.base}/profile`, req);
    }
}
