import { api } from "../../../shared/api/client";
import { API_ENDPOINTS } from "../../../shared/api/endpoints";
import type { ApiResponse, Me } from "../../../shared/api/types";
import type { ChangePasswordRequest } from "../types/auth.types";

export const authApi = {
    login: (data: { email: string; password: string }) =>
        api.post(API_ENDPOINTS.AUTH.LOGIN, data),

    logout: () => api.post(API_ENDPOINTS.AUTH.LOGOUT),

    getMe: () => api.get<unknown, ApiResponse<Me>>(API_ENDPOINTS.AUTH.ME),

    changePassword: (data: ChangePasswordRequest) =>
        api.post(API_ENDPOINTS.AUTH.CHANGE_PASSWORD, data),
}
