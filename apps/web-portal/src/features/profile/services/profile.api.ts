import { api } from "../../../shared/api/client";
import { API_ENDPOINTS } from "../../../shared/api/endpoints";
import type { ApiResponse } from "../../../shared/api/types";
import type { UserDto } from "../types/profile.types";

export const userApi = {
    getProfile: async () => {
        const res = await api.get<unknown, ApiResponse<UserDto>>(API_ENDPOINTS.USER.PROFILE);
        return res.data;
    },
}