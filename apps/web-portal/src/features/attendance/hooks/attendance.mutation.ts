import { useMutation, useQueryClient } from "@tanstack/react-query";
import { attendanceApi } from "../services/attendance.api";
import { attendanceQueryKeys } from "./attendance.query";
import { useNavigate } from "react-router-dom";

export const useCloseAttendanceSession = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (attendanceSessionId: string) =>
            attendanceApi.closeAttendanceSession(attendanceSessionId),

        onSuccess: (_, attendanceSessionId) => {
            queryClient.invalidateQueries({
                queryKey: attendanceQueryKeys.detail(attendanceSessionId),
            });
            queryClient.invalidateQueries({
                queryKey: ["attendance", "sessions", attendanceSessionId, "students"],
            });
            queryClient.invalidateQueries({
                queryKey: ["attendance", "sessions", "history"],
            });
        }
    });
}

export const useStartAttendanceSession = () => {
    const navigate = useNavigate();
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (courseSectionId: string) =>
            attendanceApi.createAttendanceSession({ courseSectionId }),

        onSuccess: (attendanceSessionId, courseSectionId) => {
            queryClient.invalidateQueries({
                queryKey: ["attendance", "sessions", "history", courseSectionId],
            });
            navigate(
                `/lecturer/courses/${courseSectionId}/attendance-sessions/${attendanceSessionId}`,
            );
        }
    });
}
