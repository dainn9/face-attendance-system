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

        STUDENT_SECTIONS: `${API_PREFIX}/student/my-course-sections`,
        STUDENT_ATTENDANCE_RECORDS: (courseSectionId: string) => `${ATTENDANCE_PREFIX}/student/course-section/${courseSectionId}/attendance-records`,
    },

    ATTENDANCE: {
        SESSION_HISTORY: (courseSectionId: string) => `${ATTENDANCE_PREFIX}/lecturer/course-sections/${courseSectionId}/attendance-sessions`,
        SESSION_DETAIL: (attendanceSessionId: string) => `${ATTENDANCE_PREFIX}/attendance-sessions/${attendanceSessionId}`,
        SESSION_STUDENTS: (
            courseSectionId: string
            ,attendanceSessionId: string
        ) => `${API_PREFIX}/lecturer/course-sections/${courseSectionId}/attendance-sessions/${attendanceSessionId}/students`,
        CLOSE_SESSION: (attendanceSessionId: string) => `${ATTENDANCE_PREFIX}/attendance-sessions/${attendanceSessionId}/close`,
        CREATE_SESSION: `${ATTENDANCE_PREFIX}/attendance-sessions`,
        CHECK_IN_INFO: (attendanceSessionId: string) => `${ATTENDANCE_PREFIX}/student/attendance-session/${attendanceSessionId}/check-in-info`,

        CHALLENGE_CODE: `${API_PREFIX}/student/faces/challenge`,
            CHECK_IN: (attendanceSessionId: string) => `${API_PREFIX}/student/attendance-session/${attendanceSessionId}/check-in`,
    },

    USER: {
        PROFILE: `${USER_PREFIX}/users/profile`,
    },

    FACE: {
        STATUS: `${API_PREFIX}/student/faces/status`,
        REGISTER: `${API_PREFIX}/student/faces/register`,
    }
}