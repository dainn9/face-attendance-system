export type CourseSectionStatus = "Open" | "Closed";

export const Semester = {
    None: 0,
    HK1: 1,
    HK2: 2,
    HK3: 3,
} as const;

export type Semester = (typeof Semester)[keyof typeof Semester];

export type ScheduleDto = {
    dayOfWeek: number;
    startTime: string;
    endTime: string;
    room: string;
};

export type CourseStudent = {
    id: string;
    code: string;
    fullName: string;
    major: string;
};

export type CourseSectionSchedule = {
    id: string;
    day: string;
    startTime: string;
    endTime: string;
    room: string;
};

export interface CourseSection {
    id: string;
    courseName: string;
    courseCode: string;
    classCode: string;
    faculty: string;
    lecturerName: string;
    semester: string;
    schoolYear: string;
    credits: number;
    maxStudents: number;
    status: CourseSectionStatus;
    schedules: CourseSectionSchedule[];
    students: CourseStudent[];
}

export interface CourseSectionDto {
    id: string;
    subjectName: string;
    subjectCode: string;
    courseSectionCode: string;
    lecturerId: string;
    facultyId?: string;
    isActive: boolean;
    semester: Semester;
    academicYear: string;
    studentCount: number;
    maxCapacity?: number;
    firstSchedule?: ScheduleDto | null;
    lecturerName: string;
};

export interface GetCourseSectionPagedRequest {
    page?: number;
    pageSize?: number;
    searchQuery?: string;
    facultyId?: string;
    semester?: Semester;
    academicYear?: string;
    isActive?: boolean;
}

export const formatSemester = (semester: Semester) => {
    if (semester === Semester.HK1) return "HK1";
    if (semester === Semester.HK2) return "HK2";
    if (semester === Semester.HK3) return "HK3";
    return "Chua xac dinh";
};

export const formatDayOfWeek = (dayOfWeek: number) => {
    const dayLabels: Record<number, string> = {
        0: "Chủ nhật",
        1: "Thứ 2",
        2: "Thứ 3",
        3: "Thứ 4",
        4: "Thứ 5",
        5: "Thứ 6",
        6: "Thứ 7",
    };

    return dayLabels[dayOfWeek] ?? `Thứ ${dayOfWeek}`;
};

export const formatTimeToMinute = (time: string) => time.slice(0, 5);

export interface CreateCourseSectionRequest {
    subjectId: string;
    courseSectionCode: string;
    semester: Semester;
    academicYear: string;
    lecturerId: string;
    maxCapacity: number;
    schedules: {
        dayOfWeek: number;
        startTime: string;
        endTime: string;
        room: string;
    }[];
}

type LecturerDto = {
    userId: string;
    userCode: string;
    fullName: string;
};

type ScheduleDetailDto = {
    id: string;
    dayOfWeek: number;
    startTime: string;
    endTime: string;
    room: string;
};

export interface CourseSectionDetail {
    id: string;
    subjectName: string;
    credits: number;
    courseSectionCode: string;
    isActive: boolean;
    semester: Semester;
    academicYear: string;
    studentCount: number;
    maxCapacity: number;
    lecturer: LecturerDto;
    schedules: ScheduleDetailDto[];
}
