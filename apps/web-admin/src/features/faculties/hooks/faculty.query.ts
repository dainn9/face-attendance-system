import { useQuery } from "@tanstack/react-query";
import { facultyApi } from "../services/faculty.api";

export const facultyQueryKeys = {
    all: ["faculties"] as const,
    lists: () => [...facultyQueryKeys.all, "list"] as const,
    details: () => [...facultyQueryKeys.all, "detail"] as const,
    detail: (id: string) => [...facultyQueryKeys.details(), id] as const,
}

export const useFaculties = () =>
  useQuery({
    queryKey: facultyQueryKeys.lists(),
    queryFn: facultyApi.list,
  });

export const useFacultyDetail = (id: string) =>
  useQuery({
    queryKey: facultyQueryKeys.detail(id),
    queryFn: () => facultyApi.detail(id!),
    enabled: !!id,
  });
