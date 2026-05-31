const AUTH_PREFIX = import.meta.env.VITE_PRIFIX_API_AUTH;
const FACULTIES_PREFIX = import.meta.env.VITE_PRIFIX_API_USERS;


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

        CREATE_MAJOR: (facultyId: string) => `${FACULTIES_PREFIX}/faculties/${facultyId}/majors`,
    }
}