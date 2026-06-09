import { api } from "../../../shared/api/client";
import { API_ENDPOINTS } from "../../../shared/api/endpoints";
import type { ApiResponse, PagedResult } from "../../../shared/api/types";
import type {
    AttendanceSessionHistoryDto,
    AttendanceSessionHistoryRequest,
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
};
