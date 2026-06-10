import { useQuery } from "@tanstack/react-query";
import { courseApi } from "../../../shared/api/services/course.api";
import type { GetCourseSectionPagedRequest, GetEnrolledStudentsRequest } from "../types/course.types";
import { lookupApi } from "../../../shared/api/services/lookup.api";

export const courseQueryKeys = {
    all: ["courses"] as const,
    lists: () => [...courseQueryKeys.all, "list"] as const,
    list: (params: GetCourseSectionPagedRequest) => [...courseQueryKeys.lists(), params] as const,
    details: () => [...courseQueryKeys.all, "detail"] as const,
    detail: (id: string) => [...courseQueryKeys.details(), id] as const,
    students: (
        courseSectionId: string,
        params: GetEnrolledStudentsRequest
    ) => [...courseQueryKeys.detail(courseSectionId), "students", params] as const,

    studentLookup: (keyWord?: string) => [...courseQueryKeys.all, "student-lookup", keyWord] as const,
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

export const useEnrolledStudents = (
    courseSectionId: string,
    params: GetEnrolledStudentsRequest
) =>
    useQuery({
        queryKey: courseQueryKeys.students(
            courseSectionId,
            params
        ),
        queryFn: () =>
            courseApi.getEnrolledStudents(
                courseSectionId,
                params
            ),
        enabled: !!courseSectionId,
    });

export const useStudentLookup = (keyWord?: string) =>
    useQuery({
        queryKey: courseQueryKeys.studentLookup(keyWord),
        queryFn: () => lookupApi.student(keyWord),
        placeholderData: (previousData) => previousData,
        enabled: keyWord ? keyWord.trim().length >= 3 : false
    });

