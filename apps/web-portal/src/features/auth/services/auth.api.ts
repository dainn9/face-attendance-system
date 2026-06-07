import { api } from "../../../shared/api/client";
import { API_ENDPOINTS } from "../../../shared/api/endpoints";

export const authApi = {
    login: (data: { email: string; password: string }) =>
        api.post(API_ENDPOINTS.AUTH.LOGIN, data),

    logout: () => api.post(API_ENDPOINTS.AUTH.LOGOUT),

    getMe: () => api.get(API_ENDPOINTS.AUTH.ME),
}