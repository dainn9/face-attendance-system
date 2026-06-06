import type { CourseSectionDetail, CourseSectionDto, CreateCourseSectionRequest, GetCourseSectionPagedRequest } from "../../../features/courses/types/course.types";
import { api } from "../client";
import { API_ENDPOINTS } from "../endpoints";
import type { ApiResponse, PagedResult } from "../types";

export const courseApi = {
    create: async (data: CreateCourseSectionRequest)=> {
        const res = await api.post<string>(API_ENDPOINTS.COURSES.CREATE, data);
        return res.data;
    },

    list: async (params: GetCourseSectionPagedRequest) : Promise<PagedResult<CourseSectionDto>> => {
        const res = await api.get<unknown, ApiResponse<PagedResult<CourseSectionDto>>>(API_ENDPOINTS.COURSES.LIST, { params });

        return res.data;
    },

    detail: async (id: string) : Promise<CourseSectionDetail> => {
        const res = await api.get<unknown, ApiResponse<CourseSectionDetail>>(API_ENDPOINTS.COURSES.DETAIL(id));
        return res.data;
    }
}
