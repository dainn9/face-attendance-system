const AUTH_PREFIX = import.meta.env.VITE_PREFIX_API_AUTH;
const ATTENDANCE_PREFIX = import.meta.env.VITE_PREFIX_API_ATTENDANCE;
const API_PREFIX = import.meta.env.VITE_PREFIX_API;
const USER_PREFIX = import.meta.env.VITE_PREFIX_API_USER;   


export const API_ENDPOINTS = {
    AUTH: {
        REFRESH: `${AUTH_PREFIX}/auth/refresh`,
        LOGIN: `${AUTH_PREFIX}/auth/login`,
        LOGOUT: `${AUTH_PREFIX}/auth/logout`,
        CHANGE_PASSWORD: `${AUTH_PREFIX}/auth/change-password`,
        ME: `${AUTH_PREFIX}/auth/me`,
    },

    COURSES: {
        LECTURER_SECTIONS: `${ATTENDANCE_PREFIX}/lecturer/course-sections/my`,
        LECTURER_COURSE_SECTION_LOOKUP: `${ATTENDANCE_PREFIX}/lecturer/course-sections/lookup`,
        DETAIL: (id: string) => `${API_PREFIX}/course-sections/${id}`,
        LECTURER_COURSE_STUDENTS: (courseSectionId: string) => `${API_PREFIX}/lecturer/course-sections/${courseSectionId}/students`,
    },

    ATTENDANCE: {
        SESSION_HISTORY: (courseSectionId: string) => `${ATTENDANCE_PREFIX}/lecturer/course-sections/${courseSectionId}/attendance-sessions`,
    },

    USER: {
        PROFILE: `${USER_PREFIX}/users/profile`,
    },
}
