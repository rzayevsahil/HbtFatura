import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../core/services/api.service';

export interface UserNotificationDto {
  id: string;
  type: string;
  title: string;
  body: string;
  referenceType?: string | null;
  referenceId?: string | null;
  readAt?: string | null;
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private base = '/api/Notifications';
  constructor(private api: ApiService) { }

  list(take = 30): Observable<UserNotificationDto[]> {
    return this.api.get<UserNotificationDto[]>(this.base, { take });
  }

  unreadCount(): Observable<{ count: number }> {
    return this.api.get<{ count: number }>(`${this.base}/unread-count`);
  }

  markRead(id: string): Observable<void> {
    return this.api.patch<void>(`${this.base}/${id}/read`, {});
  }

  markAllRead(): Observable<void> {
    return this.api.patch<void>(`${this.base}/read-all`, {});
  }
}
