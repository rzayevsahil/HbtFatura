export interface MainAccountCodeDto {
    id: string;
    firmId?: string | null;
    code: string;
    name: string;
    sortOrder: number;
    createdAt: string;
    isSystem?: boolean;
}

export interface CreateMainAccountCodeRequest {
    code: string;
    name: string;
    sortOrder?: number;
    firmId?: string;
}

export interface UpdateMainAccountCodeRequest {
    code: string;
    name: string;
    sortOrder?: number;
}
