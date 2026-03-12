export interface EmployeeListDto {
    id: string;
    firmId?: string;
    email: string;
    fullName: string;
    createdAt: string;
}

export interface CreateEmployeeRequest {
    email: string;
    password: string;
    fullName: string;
}
