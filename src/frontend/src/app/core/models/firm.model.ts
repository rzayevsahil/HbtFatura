export interface FirmDto {
    id: string;
    name: string;
    createdAt: string;
}

export interface CreateFirmRequest {
    name: string;
    adminEmail: string;
    adminPassword: string;
    adminFullName: string;
}

export interface UpdateFirmRequest {
    name: string;
}

export interface FirmUserDto {
    id: string;
    email: string;
    fullName: string;
    role: string;
    createdAt: string;
}
