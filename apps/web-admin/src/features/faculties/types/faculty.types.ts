export interface Major {
    id: string;
    name: string;
    code: string;
    studentCount: number;
};

export interface Faculty {
    id: string;
    name: string;
    code: string;
    majorCount: number;
    studentCount: number;
    lecturerCount: number;
    majors: Major[];
};

export interface Lecturer {
    id: string;
    fullName: string;
    userCode: string;
}

export interface FacultyDetail {
    id: string;
    name: string;
    code: string;
    majorCount: number;
    studentCount: number;
    lecturerCount: number;
    majors: Major[];
    lecturers: Lecturer[];
};

// Request
export interface FacultyRequest {
    name: string;
    code: string;
};

export interface MajorRequest {
    name: string;
    code: string;
};
