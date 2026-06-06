import { useMutation} from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { courseApi } from "../../../shared/api/services/course.api";
import type { CreateCourseSectionRequest } from "../types/course.types";
import { toastEmitter } from "../../../shared/utils/toastEmitter";

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
