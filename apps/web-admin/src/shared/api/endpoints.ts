const AUTH_PREFIX = import.meta.env.VITE_PREFIX_API_AUTH;
const FACULTIES_PREFIX = import.meta.env.VITE_PREFIX_API_USERS;
const ADMIN_PREFIX = import.meta.env.VITE_PREFIX_API_ADMIN;


export const API_ENDPOINTS = {
    AUTH: {
        REFRESH: `${AUTH_PREFIX}/auth/refresh`,
        LOGIN: `${AUTH_PREFIX}/auth/login`,
        LOGOUT: `${AUTH_PREFIX}/auth/logout`,
        CHANGE_PASSWORD: `${AUTH_PREFIX}/auth/change-password`,
        ME: `${AUTH_PREFIX}/auth/me`,
    },
    FACULTIES: {
        CREATE: `${FACULTIES_PREFIX}/faculties`,
        LIST: `${FACULTIES_PREFIX}/faculties`,
        DETAIL: (id: string) => `${FACULTIES_PREFIX}/faculties/${id}`,
        UPDATE: (id: string) => `${FACULTIES_PREFIX}/faculties/${id}`,

        CREATE_MAJOR: (facultyId: string) => `${FACULTIES_PREFIX}/faculties/${facultyId}/majors`,
        UPDATE_MAJOR: (facultyId: string, majorId: string) => `${FACULTIES_PREFIX}/faculties/${facultyId}/majors/${majorId}`,

        FACULTY_LOOKUP: `${FACULTIES_PREFIX}/faculties/lookup`,
        MAJOR_LOOKUP: (facultyId: string) => `${FACULTIES_PREFIX}/faculties/${facultyId}/majors/lookup`
    },

    USERS: {
        CREATE: `${ADMIN_PREFIX}/users`,
        LIST: `${ADMIN_PREFIX}/users`,
    }
}