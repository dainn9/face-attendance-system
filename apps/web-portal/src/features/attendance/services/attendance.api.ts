import { api } from "../../../shared/api/client";
import { API_ENDPOINTS } from "../../../shared/api/endpoints";
import type { ApiResponse, PagedResult } from "../../../shared/api/types";
import type {
    AttendanceSessionHistoryDto,
    AttendanceSessionHistoryRequest,
    AttendanceSessionDetailDto,
    AttendanceSessionStudentDto,
    AttendanceSessionStudentsRequest,
    CreateAttendanceSessionRequest,
    AttendanceCheckInInfoDto,
} from "../types/attendance.types";

export const attendanceApi = {
    getAttendanceSessionHistory: async (
        courseSectionId: string,
        params: AttendanceSessionHistoryRequest
    ) => {
        const res = await api.get<
            unknown,
            ApiResponse<PagedResult<AttendanceSessionHistoryDto>>
        >(API_ENDPOINTS.ATTENDANCE.SESSION_HISTORY(courseSectionId), {
            params,
        });

        return res.data;
    },
    getAttendanceSessionDetail: async (attendanceSessionId: string) => {
        const res = await api.get<
            unknown,
            ApiResponse<AttendanceSessionDetailDto>
        >(API_ENDPOINTS.ATTENDANCE.SESSION_DETAIL(attendanceSessionId));

        return res.data;
    },

    getAttendanceSessionStudents: async (
        courseSectionId: string,
        attendanceSessionId: string,
        params: AttendanceSessionStudentsRequest
    ) => {
        const res = await api.get<
            unknown,
            ApiResponse<PagedResult<AttendanceSessionStudentDto>>
        >(API_ENDPOINTS.ATTENDANCE.SESSION_STUDENTS(courseSectionId, attendanceSessionId), {
            params,
        });

        return res.data;
    },

    createAttendanceSession: async (data: CreateAttendanceSessionRequest) => {
        const res = await api.post<unknown,ApiResponse<string>>(API_ENDPOINTS.ATTENDANCE.CREATE_SESSION, data);

        return res.data;
    },

    closeAttendanceSession: async (attendanceSessionId: string) => {
        await api.post(API_ENDPOINTS.ATTENDANCE.CLOSE_SESSION(attendanceSessionId));
    },

    getAttendanceCheckInInfo: async (attendanceSessionId: string) => {
        const res = await api.get<unknown,ApiResponse<AttendanceCheckInInfoDto>>(
            API_ENDPOINTS.ATTENDANCE.CHECK_IN_INFO(attendanceSessionId)
        );
        return res.data;
    }
};
