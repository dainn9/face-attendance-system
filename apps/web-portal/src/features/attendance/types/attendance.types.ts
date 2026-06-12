export interface AttendanceSessionHistoryDto {
    id: string;
    date: string;
    startTime: string;
    endTime?: string | null;
    status: number;
    presentCount: number;
    absentCount: number;
    attendanceRate: number;
}

export interface AttendanceSessionHistoryRequest {
    page?: number;
    pageSize?: number;
}

export interface AttendanceSessionDetailDto {
    id: string;
    date: string;
    startTime: string;
    endTime?: string | null;
    status: number; //1 open session, 2 closed session
    presentCount: number;
    absentCount: number;
    attendanceRate: number;
}

export interface AttendanceSessionStudentsRequest {
    page?: number;
    pageSize?: number;
}

export interface AttendanceSessionStudentDto {
    userId: string;
    studentCode: string;
    fullName: string;
    email: string;

    /**
     * Present = 1, Absent = 2.
     * Undefined means the student has not checked in yet while the session is still open.
    */
    attendanceStatus?: number;

    /**
     * Face recognition confidence score.
     * Undefined if the student has not checked in or was marked absent.
     */
    confidence?: number;

    /**
     * Student check-in time.
     * Undefined if the student has not checked in or was marked absent.
    */
    checkedInAt?: string;
}

export interface CreateAttendanceSessionRequest {
    courseSectionId: string;
}

export interface AttendanceCheckInInfoDto {
    id: string;
    subjectName: string;
    courseSectionCode?: string;
    CourseSectionCode?: string;
    date: string;
    startTime: string;
    status: number; //1 open session, 2 closed session
}
