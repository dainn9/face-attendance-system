import { useQuery } from "@tanstack/react-query";
import { courseApi } from "../../../shared/api/services/course.api";
import type { GetCourseSectionPagedRequest } from "../types/course.types";

export const courseQueryKeys = {
    all: ["courses"] as const,
    lists: () => [...courseQueryKeys.all, "list"] as const,
    list: (params: GetCourseSectionPagedRequest) => [...courseQueryKeys.lists(), params] as const,
    details: () => [...courseQueryKeys.all, "detail"] as const,
    detail: (id: string) => [...courseQueryKeys.details(), id] as const,
}

export const useCourses = (
    params: GetCourseSectionPagedRequest
) =>
    useQuery({
        queryKey: courseQueryKeys.list(params),
        queryFn: () => courseApi.list(params),
        placeholderData: (previousData) => previousData
    });

export const useCourse = (id: string) =>
    useQuery({
        queryKey: courseQueryKeys.detail(id),
        queryFn: () => courseApi.detail(id!),
        enabled: !!id,
    });