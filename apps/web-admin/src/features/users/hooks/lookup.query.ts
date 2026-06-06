import { useQuery } from "@tanstack/react-query";
import { lookupApi } from "../../../shared/api/services/lookup.api";

export const lookupQueryKeys = {
    all: ["lookup"] as const,

    faculties: () =>
        [...lookupQueryKeys.all, "faculties"] as const,

    majorsByFaculty: (facultyId: string) =>
        [...lookupQueryKeys.all, "majors", facultyId] as const,

    subjects: (keyWord: string | undefined) =>
        [...lookupQueryKeys.all, "subjects", keyWord] as const,

    lecturers: (keyWord: string | undefined, facultyId: string | undefined) =>
        [...lookupQueryKeys.all, "lecturers", keyWord, facultyId] as const
};

export const useFacultyLookup = () =>
    useQuery({
        queryKey: lookupQueryKeys.faculties(),
        queryFn: lookupApi.faculty,
        placeholderData: (previousData) => previousData,
    });

export const useMajorLookup = (facultyId?: string) =>
    useQuery({
        queryKey: lookupQueryKeys.majorsByFaculty(facultyId ?? ""),
        queryFn: () => lookupApi.major(facultyId!),
        enabled: !!facultyId,
        placeholderData: (previousData) => previousData,
    });

export const useSubjectLookup = (keyWord?: string) =>
    useQuery({
        queryKey: lookupQueryKeys.subjects(keyWord),
        queryFn: () => lookupApi.subject(keyWord),
        placeholderData: (previousData) => previousData,
    });

export const useLecturerLookup = (keyWord?: string, facultyId?: string) =>
    useQuery({
        queryKey: lookupQueryKeys.lecturers(keyWord, facultyId),
        queryFn: () => lookupApi.lecturer(keyWord, facultyId),
        placeholderData: (previousData) => previousData,
    });
