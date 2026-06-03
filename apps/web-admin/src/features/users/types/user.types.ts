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
    userCode: string | null;
    email: string;
    password: string;
    userRole: UserRoleNumber;
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

export const UserRoleValue = {
    Admin: 1,
    Lecturer: 2,
    Student: 3,
} as const;

export type UserRoleNumber =
    (typeof UserRoleValue)[keyof typeof UserRoleValue];

export type UserRole = "Admin" | "Lecturer" | "Student";
