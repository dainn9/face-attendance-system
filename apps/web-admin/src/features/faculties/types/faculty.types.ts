export type Major = {
    id: string;
    name: string;
    code: string;
    studentCount: number;
};

export type Faculty = {
    id: string;
    name: string;
    code: string;
    majorCount: number;
    studentCount: number;
    lecturerCount: number;
    majors: Major[];
};

export type CreateFacultyRequest = {
    name: string;
    code: string;
};