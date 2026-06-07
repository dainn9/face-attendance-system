const AUTH_PREFIX = import.meta.env.VITE_PREFIX_API_AUTH;

export const API_ENDPOINTS = {
    AUTH: {
        REFRESH: `${AUTH_PREFIX}/auth/refresh`,
        LOGIN: `${AUTH_PREFIX}/auth/login`,
        LOGOUT: `${AUTH_PREFIX}/auth/logout`,
        CHANGE_PASSWORD: `${AUTH_PREFIX}/auth/change-password`,
        ME: `${AUTH_PREFIX}/auth/me`,
    },
}