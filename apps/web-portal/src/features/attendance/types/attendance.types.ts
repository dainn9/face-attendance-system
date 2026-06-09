export interface AttendanceSessionHistoryDto {
    id: string;
    date: string;
    startTime: string;
    endTime: string;
    status: number;
    presentCount: number;
    absentCount: number;
    attendanceRate: number;
}

export interface AttendanceSessionHistoryRequest {
    page?: number;
    pageSize?: number;
}
