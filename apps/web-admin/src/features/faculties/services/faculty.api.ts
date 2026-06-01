import { api } from "../../../shared/api/client";
import { API_ENDPOINTS } from "../../../shared/api/endpoints";
import type { ApiResponse } from "../../../shared/api/types";
import type { Faculty, FacultyDetail, FacultyRequest, MajorRequest } from "../types/faculty.types";

export const facultyApi = {
    create: async (data: FacultyRequest) : Promise<string> =>
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

    update: async (id: string, data: FacultyRequest) => {
        await api.put(API_ENDPOINTS.FACULTIES.UPDATE(id), data);
    },

    createMajor: async (facultyId: string, data: MajorRequest) => {
        const res = await api.post(API_ENDPOINTS.FACULTIES.CREATE_MAJOR(facultyId), data);
        return res.data;
    },

    updateMajor: async (facultyId: string, majorId: string, data: MajorRequest) => {
        await api.put(API_ENDPOINTS.FACULTIES.UPDATE_MAJOR(facultyId, majorId), data);
    }

}