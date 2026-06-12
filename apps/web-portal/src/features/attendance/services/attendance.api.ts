import { api } from "../../../shared/api/client";
import { API_ENDPOINTS } from "../../../shared/api/endpoints";
import { ApiError } from "../../../shared/api/errors";
import type { ApiResponse, PagedResult } from "../../../shared/api/types";
import type {
    AttendanceSessionHistoryDto,
    AttendanceSessionHistoryRequest,
    AttendanceSessionDetailDto,
    AttendanceSessionStudentDto,
    AttendanceSessionStudentsRequest,
    CreateAttendanceSessionRequest,
    AttendanceCheckInInfoDto,
    ChallengeDto,
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
    },

    getChallengeCode: async () => {
        const res = await api.get<unknown, ApiResponse<ChallengeDto>>(API_ENDPOINTS.ATTENDANCE.CHALLENGE_CODE);
        return res.data;
    },

    checkInAttendance: async (
        attendanceSessionId: string,
        challenge: string,
        center_image: File,
        challenge_image: File
    ) => {
        const formData = new FormData();
        formData.append("challenge", challenge);
        formData.append("center", center_image);
        formData.append("challengeImage", challenge_image);

        const res = await api.post<unknown, ApiResponse<void> | undefined | "">(
            API_ENDPOINTS.ATTENDANCE.CHECK_IN(attendanceSessionId),
            formData,
        );
        
        if (!res) {
            return;
        }

        if (!res.success) {
            throw new ApiError(
                0,
                res.message || "Không thể cập nhật khuôn mặt",
                res.errorCode,
            );
        }
    }
};
