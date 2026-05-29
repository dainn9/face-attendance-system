import { api } from "../../../shared/api/client";
import { API_ENDPOINTS } from "../../../shared/api/endpoints";

export const facultyApi = {
    create: (data: { name: string; code: string }) =>
        api.post(API_ENDPOINTS.FACULTIES.CREATE, data),
}