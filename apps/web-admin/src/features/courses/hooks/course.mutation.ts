import { useMutation, useQueryClient} from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { courseApi } from "../../../shared/api/services/course.api";
import type { CreateCourseSectionRequest } from "../types/course.types";
import { toastEmitter } from "../../../shared/utils/toastEmitter";
import { courseQueryKeys } from "./course.query";

export const useCreateCourseSection = () => {
    const navigate = useNavigate();

    return useMutation({
        mutationFn: (data: CreateCourseSectionRequest) => courseApi.create(data),

        onSuccess: (result) => {
            toastEmitter.success("Tạo lớp học phần thành công!");
            navigate(`/courses/${result}`);
        }
    })
}

export const useEnrollCourseSection = (courseSectionId: string) => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (data: { studentIds: string[] }) =>
            courseApi.enrollment(courseSectionId, data),

        onSuccess: () => {
            toastEmitter.success("Thêm sinh viên vào lớp học phần thành công!");
            queryClient.invalidateQueries({
                queryKey: courseQueryKeys.detail(courseSectionId),
            });
        }
    })
}

export const useRemoveEnrollCourseSection = (courseSectionId: string) => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (studentId: string) =>
            courseApi.removeEnrollment(courseSectionId, studentId),

        onSuccess: () => {
            toastEmitter.success("Xóa sinh viên khỏi lớp học phần thành công!");
            queryClient.invalidateQueries({
                queryKey: courseQueryKeys.detail(courseSectionId),
            });
        }
    })
}
