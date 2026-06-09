import { useQuery } from "@tanstack/react-query";
import { attendanceApi } from "../services/attendance.api";
import type { AttendanceSessionHistoryRequest } from "../types/attendance.types";

export const attendanceQueryKeys = {
    all: ["attendance"],

    sessionHistory: (
        courseSectionId?: string,
        params?: AttendanceSessionHistoryRequest
    ) => [
        ...attendanceQueryKeys.all,
        "sessions",
        "history",
        courseSectionId,
        params,
    ],
};

export const useAttendanceSessionHistory = (
    courseSectionId?: string,
    params: AttendanceSessionHistoryRequest = {}
) =>
    useQuery({
        queryKey: attendanceQueryKeys.sessionHistory(courseSectionId, params),
        queryFn: () =>
            attendanceApi.getAttendanceSessionHistory(
                courseSectionId as string,
                params
            ),
        enabled: !!courseSectionId,
        placeholderData: (previousData) => previousData,
    });
