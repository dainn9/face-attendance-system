import { useQuery } from "@tanstack/react-query";
import type {
    GetLecturerCourseSectionsRequest,
    LecturerCourseStudentsRequest,
} from "../types/course.types";
import { courseApi } from "../services/course.api";

export const courseQueryKeys = {
    all: ["courses"],

    lecturerCourseSections: (
        params: GetLecturerCourseSectionsRequest
    ) => [...courseQueryKeys.all, "lecturer", "sections", params],

    lecturerCourseSectionLookup: () =>
        ["courses", "lecturer", "lookup"],

    detail: (id?: string) => [...courseQueryKeys.all, "detail", id],

    lecturerCourseStudents: (
        courseSectionId?: string,
        params?: LecturerCourseStudentsRequest
    ) => [...courseQueryKeys.all, "lecturer", "detail", courseSectionId, "students", params],

    studentCourseSections: () =>
        [...courseQueryKeys.all, "student", "sections"],

    studentAttendanceRecords: (courseSectionId?: string) =>
        [...courseQueryKeys.all, "student", "attendance-records", courseSectionId],

};

export const useLecturerCourseSections = (
    params: GetLecturerCourseSectionsRequest
) =>
    useQuery({
        queryKey: courseQueryKeys.lecturerCourseSections(params),
        queryFn: () => courseApi.getLecturerCourseSections(params),
        placeholderData: (previousData) => previousData
    });

export const useLecturerCourseSectionLookup = () =>
    useQuery({
        queryKey: courseQueryKeys.lecturerCourseSectionLookup(),
        queryFn: () => courseApi.getLecturerCourseSectionLookup(),
        placeholderData: (previousData) => previousData
    });

export const useCourseDetail = (id?: string) =>
    useQuery({
        queryKey: courseQueryKeys.detail(id),
        queryFn: () => courseApi.getCourseDetail(id as string),
        enabled: !!id,
    });

export const useLecturerCourseStudents = (
    courseSectionId?: string,
    params: LecturerCourseStudentsRequest = {}
) =>
    useQuery({
        queryKey: courseQueryKeys.lecturerCourseStudents(courseSectionId, params),
        queryFn: () => courseApi.getLecturerCourseStudents(courseSectionId as string, params),
        enabled: !!courseSectionId,
        placeholderData: (previousData) => previousData
    });

export const useStudentCourseSections = () =>
    useQuery({
        queryKey: courseQueryKeys.studentCourseSections(),
        queryFn: () => courseApi.getStudentCourseSections(),
        placeholderData: (previousData) => previousData
    });

export const useStudentAttendanceRecords = (courseSectionId?: string) =>
    useQuery({
        queryKey: courseQueryKeys.studentAttendanceRecords(courseSectionId),
        queryFn: () => courseApi.getStudentAttendanceRecords(courseSectionId as string),
        enabled: !!courseSectionId,
        placeholderData: (previousData) => previousData
    });
