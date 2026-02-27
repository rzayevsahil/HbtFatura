import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AccountService, UserProfileDto, UpdateProfileRequest } from '../../services/account.service';
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

    constructor(private accountService: AccountService, private toastr: ToastrService) { }

    ngOnInit(): void {
        this.loadProfile();
    }

    loadProfile(): void {
        this.loading = true;
        this.accountService.getProfile().subscribe({
            next: (res) => {
                this.profile = res;
                this.model.fullName = res.fullName;
                this.model.phoneNumber = res.phoneNumber || '';
                this.loading = false;
            },
            error: () => {
                this.toastr.error('Profil bilgileri yüklenemedi.');
                this.loading = false;
            }
        });
    }

    save(): void {
        this.saving = true;
        this.accountService.updateProfile(this.model).subscribe({
            next: () => {
                this.toastr.success('Profil başarıyla güncellendi.');
                this.model.currentPassword = '';
                this.model.newPassword = '';
                this.saving = false;
                // Opsiyonel: AuthService'deki kullanıcı adını da güncellemek gerekebilir eğer signal kullanılıyorsa.
                // Ama sayfa yenilenince veya AuthService API ile eşleşince zaten güncellenir.
            },
            error: (err) => {
                this.toastr.error(err.error?.message || 'Profil güncellenirken bir hata oluştu.');
                this.saving = false;
            }
        });
    }
}
