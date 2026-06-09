export const Gender = {
    Male: 1,
    Female: 2,
} as const;

export type Gender = (typeof Gender)[keyof typeof Gender];

export const Role = {
    Admin: 1,
    Lecturer: 2,
    Student: 3,
} as const;

export type Role = (typeof Role)[keyof typeof Role];

export interface UserDto {
    userId: string;
    userCode: string;
    fullName: string;
    gender: Gender;
    dateOfBirth: string;
    email: string;
    role: Role;
}