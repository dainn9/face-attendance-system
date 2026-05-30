import { api } from "../../../shared/api/client";
import { API_ENDPOINTS } from "../../../shared/api/endpoints";
import type { ApiResponse } from "../../../shared/api/types";
import type { Faculty } from "../types/faculty.types";

export const facultyApi = {
    create: async (data: { name: string; code: string }) =>
    {
        const res = await api.post(API_ENDPOINTS.FACULTIES.CREATE, data);
        return res.data;
    },

    list: async (): Promise<Faculty[]> => {
        const res = await api.get<unknown, ApiResponse<Faculty[]>>(API_ENDPOINTS.FACULTIES.LIST);
        
        return res.data ?? [];
    },
}