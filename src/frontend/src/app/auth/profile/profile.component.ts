import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AccountService, UserProfileDto, UpdateProfileRequest } from '../../services/account.service';
import { AuthService } from '../../core/services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
    selector: 'app-profile',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './profile.component.html',
    styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
    profile: UserProfileDto | null = null;
    model: UpdateProfileRequest = {
        fullName: '',
        phoneNumber: '',
        currentPassword: '',
        newPassword: ''
    };
    loading = false;
    saving = false;
    userNameAfterUpdate = '';

    constructor(
        private accountService: AccountService,
        private auth: AuthService,
        private toastr: ToastrService
    ) { }

    ngOnInit(): void {
        this.loadProfile();
    }

    loadProfile(): void {
        this.loading = true;
        this.accountService.getProfile().subscribe({
            next: (res) => {
                this.profile = res;
                this.model.fullName = res.fullName;
                this.model.phoneNumber = res.phoneNumber ? this.applyPhoneFormat(res.phoneNumber) : '';
                this.loading = false;
            },
            error: () => {
                if (!this.auth.loggingOut() && this.auth.isAuthenticated()) {
                    this.toastr.error('Profil bilgileri yüklenemedi.');
                }
                this.loading = false;
            }
        });
    }

    onPhoneInput(event: Event): void {
        const input = event.target as HTMLInputElement;
        let value = input.value.replace(/[^0-9]/g, '');

        if (value.length > 0 && !value.startsWith('0')) {
            value = '0' + value;
        } else if (value.length === 0) {
            value = '0';
        }

        if (value.length > 11) value = value.substring(0, 11);

        let formatted = '';
        for (let i = 0; i < value.length; i++) {
            if (i === 1 || i === 4 || i === 7 || i === 9) formatted += ' ';
            formatted += value[i];
        }

        this.model.phoneNumber = formatted.trim();
        input.value = formatted.trim();
    }

    private applyPhoneFormat(value: string): string {
        let cleaned = value.replace(/[^0-9]/g, '');
        if (cleaned.length > 11) cleaned = cleaned.substring(0, 11);
        if (cleaned.length > 0 && !cleaned.startsWith('0')) cleaned = '0' + cleaned;

        let formatted = '';
        for (let i = 0; i < cleaned.length; i++) {
            if (i === 1 || i === 4 || i === 7 || i === 9) formatted += ' ';
            formatted += cleaned[i];
        }
        return formatted.trim();
    }

    save(): void {
        this.saving = true;
        this.accountService.updateProfile(this.model).subscribe({
            next: () => {
                this.toastr.success('Profil başarıyla güncellendi.');

                // AuthService'deki kullanıcı adını da güncelle
                this.auth.updateUser({ fullName: this.model.fullName });

                // Sayfadaki (profile-header) ismini de güncelle
                if (this.profile) {
                    this.profile.fullName = this.model.fullName;
                }

                this.model.currentPassword = '';
                this.model.newPassword = '';
                this.saving = false;
            },
            error: (err) => {
                this.toastr.error(err.error?.message || 'Profil güncellenirken bir hata oluştu.');
                this.saving = false;
            }
        });
    }
}
