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

export type Lecturer = {
    id: string;
    name: string;
    userCode: string;
}

export type FacultyDetail = {
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
export type FacultyRequest = {
    name: string;
    code: string;
};

export type MajorRequest = {
    name: string;
    code: string;
};
