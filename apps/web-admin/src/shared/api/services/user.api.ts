import { api } from "../client";
import { API_ENDPOINTS } from "../endpoints";
import type { ApiResponse, PagedResult } from "../types";
import type { CreateUserRequest, GetUserPagedRequest, LookupDto, UserDto } from "../../../features/users/types/user.types";

export const userApi = {
    create: async (data: CreateUserRequest)=> {
        await api.post<string>(API_ENDPOINTS.USERS.CREATE, data);
    },

    list: async (params: GetUserPagedRequest) : Promise<PagedResult<UserDto>> => {
        const res = await api.get<unknown, ApiResponse<PagedResult<UserDto>>>(API_ENDPOINTS.USERS.LIST, { params });

        return res.data;
    },
}