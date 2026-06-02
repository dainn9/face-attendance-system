import { useQuery } from "@tanstack/react-query";
import { lookupApi } from "../../../shared/api/services/lookup.api";

export const lookupQueryKeys = {
    all: ["lookup"] as const,

    faculties: () =>
        [...lookupQueryKeys.all, "faculties"] as const,

    majorsByFaculty: (facultyId: string) =>
        [...lookupQueryKeys.all, "majors", facultyId] as const,
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