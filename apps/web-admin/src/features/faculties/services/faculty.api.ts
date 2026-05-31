import { api } from "../../../shared/api/client";
import { API_ENDPOINTS } from "../../../shared/api/endpoints";
import type { ApiResponse } from "../../../shared/api/types";
import type { Faculty, FacultyDetail } from "../types/faculty.types";

export const facultyApi = {
    create: async (data: { name: string; code: string }) : Promise<string> =>
    {
        const res = await api.post<string>(API_ENDPOINTS.FACULTIES.CREATE, data);
        return res.data;
    },

    list: async (): Promise<Faculty[]> => {
        const res = await api.get<unknown, ApiResponse<Faculty[]>>(API_ENDPOINTS.FACULTIES.LIST);
        
        return res.data ?? [];
    },

    detail: async (id: string): Promise<FacultyDetail> => {
        const res = await api.get<unknown, ApiResponse<FacultyDetail>>(API_ENDPOINTS.FACULTIES.DETAIL(id));
        return res.data;
    },

    createMajor: async (facultyId: string, data: { name: string; code: string }) => {
        const res = await api.post(API_ENDPOINTS.FACULTIES.CREATE_MAJOR(facultyId), data);
        return res.data;
    }
}