export interface User {
    id: string;
    email: string;
    fullName: string;
    role: string;
    roleDisplayName?: string;
    firmId: string | null;
    firmName: string | null;
    permissions: string[];
}

export interface AuthResponse {
    accessToken: string;
    refreshToken: string;
    expiresAt: string;
    user: User;
}

export interface LoginRequest {
    email: string;
    password: string;
}

export interface RefreshTokenRequest {
    refreshToken: string;
}

export interface RegisterRequest {
    email: string;
    password: string;
    fullName: string;
    role: string;
    firmId?: string;
}
