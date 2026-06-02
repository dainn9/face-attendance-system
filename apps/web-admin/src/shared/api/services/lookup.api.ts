import type { LookupDto } from "../../../features/users/types/user.types";
import { api } from "../client";
import { API_ENDPOINTS } from "../endpoints";
import type { ApiResponse } from "../types";

export const lookupApi = {
    faculty: async () : Promise<LookupDto[]> => {
        const res = await api.get<unknown, ApiResponse<LookupDto[]>>(API_ENDPOINTS.FACULTIES.FACULTY_LOOKUP);
        return res.data;
    },

    major: async (facultyId: string) : Promise<LookupDto[]> => {
        const res = await api.get<unknown, ApiResponse<LookupDto[]>>(API_ENDPOINTS.FACULTIES.MAJOR_LOOKUP(facultyId));
        return res.data;
    }
}