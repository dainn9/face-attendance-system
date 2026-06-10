const AUTH_PREFIX = import.meta.env.VITE_PREFIX_API_AUTH;
const USERS_PREFIX = import.meta.env.VITE_PREFIX_API_USERS;
const ADMIN_PREFIX = import.meta.env.VITE_PREFIX_API_ADMIN;
const ATTENDANCE_PREFIX = import.meta.env.VITE_PREFIX_API_ATTENDANCE;
const API_PREFIX = import.meta.env.VITE_PREFIX_API;

export const API_ENDPOINTS = {
    AUTH: {
        REFRESH: `${AUTH_PREFIX}/auth/refresh`,
        LOGIN: `${AUTH_PREFIX}/auth/login`,
        LOGOUT: `${AUTH_PREFIX}/auth/logout`,
        CHANGE_PASSWORD: `${AUTH_PREFIX}/auth/change-password`,
        ME: `${AUTH_PREFIX}/auth/me`,
    },
    FACULTIES: {
        CREATE: `${USERS_PREFIX}/faculties`,
        LIST: `${USERS_PREFIX}/faculties`,
        DETAIL: (id: string) => `${USERS_PREFIX}/faculties/${id}`,
        UPDATE: (id: string) => `${USERS_PREFIX}/faculties/${id}`,

        CREATE_MAJOR: (facultyId: string) => `${USERS_PREFIX}/faculties/${facultyId}/majors`,
        UPDATE_MAJOR: (facultyId: string, majorId: string) => `${USERS_PREFIX}/faculties/${facultyId}/majors/${majorId}`,

        FACULTY_LOOKUP: `${USERS_PREFIX}/faculties/lookup`,
        MAJOR_LOOKUP: (facultyId: string) => `${USERS_PREFIX}/faculties/${facultyId}/majors/lookup`
    },

    USERS: {
        CREATE: `${ADMIN_PREFIX}/users`,
        LIST: `${ADMIN_PREFIX}/users`,
        LECTURER_LOOKUP: (keyWord?: string, facultyId?: string) => {
            const params = new URLSearchParams();

            if (keyWord) {
                params.append("keyword", keyWord);
            }

            if (facultyId) {
                params.append("facultyId", facultyId);
            }

            return `${USERS_PREFIX}/users/lookup/lecturers${
                params.toString() ? `?${params.toString()}` : ""
            }`;
        },
        STUDENT_LOOKUP: (keyWord?: string) => {
            const params = new URLSearchParams();

            if (keyWord) {
                params.append("keyword", keyWord);
            }

            return `${USERS_PREFIX}/users/lookup/students${
                params.toString() ? `?${params.toString()}` : ""
            }`;
        }
    },

    COURSES: {
        CREATE: `${ADMIN_PREFIX}/course-sections`,
        LIST: `${ADMIN_PREFIX}/course-sections`,
        DETAIL: (id: string) => `${API_PREFIX}/course-sections/${id}`,
        ENROLLED_STUDENTS: (courseSectionId: string) => `${ADMIN_PREFIX}/course-sections/${courseSectionId}/students`,
        ENROLLMENTS: (courseSectionId: string) => `${ADMIN_PREFIX}/course-sections/${courseSectionId}/enrollments`,
        REMOVE_ENROLLMENT: (courseSectionId: string, studentId: string) => `${ATTENDANCE_PREFIX}/course-sections/${courseSectionId}/enrollments/${studentId}`
    },
    
    SUBJECTS: {
        SUBJECT_LOOKUP: (keyWord?: string) => `${ATTENDANCE_PREFIX}/subjects/lookup${keyWord ? `?keyword=${encodeURIComponent(keyWord)}` : ""}`,
    }
}