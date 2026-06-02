export interface UserDto {
    userId: string;
    userCode: string;
    fullName: string;
    email: string;
    roleName: string;
    facultyName: string;
    isActive: boolean;
};

export interface CreateUserRequest {
    userCode: string;
    email: string;
    password: string;
    userRole: string;
    fullName: string;
    gender: number;
    dateOfBirth: string;
    facultyId?: string;
    majorId?: string;
};

export interface GetUserPagedRequest {
    page?: number;
    pageSize?: number;
    searchQuery?: string;
    role?: UserRole;
    facultyId?: string;
}

export interface LookupDto {
    id: string;
    name: string;
}

export type UserRole = "Admin" | "Lecturer" | "Student";