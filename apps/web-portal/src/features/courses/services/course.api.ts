import { api } from "../../../shared/api/client";
import { API_ENDPOINTS } from "../../../shared/api/endpoints";
import type { ApiResponse, PagedResult } from "../../../shared/api/types";
import type {
    CourseSectionDetail,
    CourseSectionDto,
    GetLecturerCourseSectionsRequest,
    LecturerCourseSectionLookup,
    LecturerCourseStudentDto,
    LecturerCourseStudentsRequest,
} from "../types/course.types";

export const courseApi = {
    getLecturerCourseSections: async (params: GetLecturerCourseSectionsRequest) => {
        const res = await api.get<unknown, ApiResponse<PagedResult<CourseSectionDto>>>(API_ENDPOINTS.COURSES.LECTURER_SECTIONS, { params });
        return res.data;
    },

    getLecturerCourseSectionLookup: async () => {
        const res = await api.get<unknown, ApiResponse<LecturerCourseSectionLookup[]>>(API_ENDPOINTS.COURSES.LECTURER_COURSE_SECTION_LOOKUP);
        return res.data;
    },

    getCourseDetail: async (id: string) => {
        const res = await api.get<unknown, ApiResponse<CourseSectionDetail>>(API_ENDPOINTS.COURSES.DETAIL(id));
        return res.data;
    },

    getLecturerCourseStudents: async (
        courseSectionId: string,
        params: LecturerCourseStudentsRequest
    ) => {
        const res = await api.get<unknown, ApiResponse<PagedResult<LecturerCourseStudentDto>>>(API_ENDPOINTS.COURSES.LECTURER_COURSE_STUDENTS(courseSectionId), { params });
        return res.data;
    },

}
