export interface UserProfileDto {
    id: string;
    email: string;
    fullName: string;
    phoneNumber?: string;
    role: string;
    roleDisplayName?: string;
}

export interface UpdateProfileRequest {
    fullName: string;
    phoneNumber?: string;
    currentPassword?: string;
    newPassword?: string;
}
