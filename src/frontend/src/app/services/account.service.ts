import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../core/services/api.service';

import { UserProfileDto, UpdateProfileRequest } from '../core/models';

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
