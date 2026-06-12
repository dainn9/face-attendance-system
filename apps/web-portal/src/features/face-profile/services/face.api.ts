import { api } from "../../../shared/api/client";
import { ApiError } from "../../../shared/api/errors";
import type { ApiResponse } from "../../../shared/api/types";
import { API_ENDPOINTS } from "../../../shared/api/endpoints";

export const faceApi = {
    getStatus: async () => {
        const res = await api.get<unknown, ApiResponse<boolean>>(API_ENDPOINTS.FACE.STATUS);
        return res.data;
    },

    registerFace: async (
        left: File,
        center: File,
        right: File
    ) => {
        const formData = new FormData();

        formData.append("left", left);
        formData.append("center", center);
        formData.append("right", right);

        const res = await api.post<unknown, ApiResponse<void> | undefined | "">(
            API_ENDPOINTS.FACE.REGISTER,
            formData,
        );

        if (!res) {
            return;
        }

        if (!res.success) {
            throw new ApiError(
                0,
                res.message || "Không thể cập nhật khuôn mặt",
                res.errorCode,
            );
        }

        return res.data;
    }
};
