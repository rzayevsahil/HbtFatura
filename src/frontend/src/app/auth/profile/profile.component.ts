import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../../services/account.service';
import { UserProfileDto, UpdateProfileRequest } from '../../core/models';
import { AuthService } from '../../core/services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { PhoneFormatter } from '../../core/utils/phone-formatter';

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
                this.model.phoneNumber = res.phoneNumber ? PhoneFormatter.format(res.phoneNumber) : '';
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
        this.model.phoneNumber = PhoneFormatter.handleInput(event);
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
