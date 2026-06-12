import { useQuery } from "@tanstack/react-query";
import { attendanceApi } from "../services/attendance.api";
import type { AttendanceSessionHistoryRequest, AttendanceSessionStudentsRequest } from "../types/attendance.types";

export const openSessionRefetchIntervalMs = 5000;

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

    detail: (attendanceSessionId?: string) => [
        ...attendanceQueryKeys.all,
        "sessions",
        "detail",
        attendanceSessionId,
    ],

    checkInInfo: (attendanceSessionId?: string) => [
        ...attendanceQueryKeys.all,
        "sessions",
        "check-in-info",
        attendanceSessionId,
    ],

    getStudents: (
        courseSectionId?: string,
        attendanceSessionId?: string,
        params?: AttendanceSessionStudentsRequest
    ) => [
        ...attendanceQueryKeys.all,
        "sessions",
        attendanceSessionId,
        "students",
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

export const useAttendanceSessionDetail = (attendanceSessionId?: string) =>
    useQuery({
        queryKey: attendanceQueryKeys.detail(attendanceSessionId),
        queryFn: () =>
            attendanceApi.getAttendanceSessionDetail(
                attendanceSessionId as string
            ),
        enabled: !!attendanceSessionId,
        placeholderData: (previousData) => previousData,
        refetchInterval: (query) =>
            query.state.data?.status === 1 ? openSessionRefetchIntervalMs : false,
    });

export const useAttendanceCheckInInfo = (attendanceSessionId?: string) =>
    useQuery({
        queryKey: attendanceQueryKeys.checkInInfo(attendanceSessionId),
        queryFn: () =>
            attendanceApi.getAttendanceCheckInInfo(
                attendanceSessionId as string
            ),
        enabled: !!attendanceSessionId,
        placeholderData: (previousData) => previousData,
        refetchInterval: (query) =>
            query.state.data?.status === 1 ? openSessionRefetchIntervalMs : false,
    });

export const useAttendanceSessionStudents = (
    courseSectionId?: string,
    attendanceSessionId?: string,
    params: AttendanceSessionStudentsRequest = {},
    refetchInterval: number | false = false
) =>
    useQuery({
        queryKey: attendanceQueryKeys.getStudents(
            courseSectionId,
            attendanceSessionId,
            params
        ),
        queryFn: () =>
            attendanceApi.getAttendanceSessionStudents(
                courseSectionId as string,
                attendanceSessionId as string,
                params
        ),
        enabled: !!courseSectionId && !!attendanceSessionId,
        placeholderData: (previousData) => previousData,
        refetchInterval,
    });
