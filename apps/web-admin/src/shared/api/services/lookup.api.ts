import type { StudentLookupDto } from "../../../features/courses/types/course.types";
import type { SubjectLookupDto } from "../../../features/courses/types/subject.types";
import type { LookupDto, UserLookupDto } from "../../../features/users/types/user.types";
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
    },

    subject: async (keyWord?: string) : Promise<SubjectLookupDto[]> => {
        const res = await api.get<unknown, ApiResponse<SubjectLookupDto[]>>(API_ENDPOINTS.SUBJECTS.SUBJECT_LOOKUP(keyWord));
        return res.data;
    },

    lecturer: async (keyWord?: string, facultyId?: string) : Promise<UserLookupDto[]> => {
        const res = await api.get<unknown, ApiResponse<UserLookupDto[]>>(API_ENDPOINTS.USERS.LECTURER_LOOKUP(keyWord, facultyId));
        return res.data;
    },

    student: async (keyWord?: string) : Promise<StudentLookupDto[]> => {
        const res = await api.get<unknown, ApiResponse<StudentLookupDto[]>>(API_ENDPOINTS.USERS.STUDENT_LOOKUP(keyWord));
        return res.data;
    }
}