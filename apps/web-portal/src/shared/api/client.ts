import axios from "axios";
import type { AxiosRequestConfig } from "axios";
import { ApiError, NotFoundError, UnauthorizedError, ValidationError } from "./errors";
import { API_ENDPOINTS } from "./endpoints";
import { ERROR_CODE } from "../constants/error-code";
import { emitAuthSessionExpired } from "../utils/authSessionEvents";

const API_URL = import.meta.env.VITE_API_URL;

export const api = axios.create({
    baseURL: API_URL,
    withCredentials: true,
});

// ─── REFRESH TOKEN QUEUE ─────────────────────────────────

let isRefreshing = false
let failedQueue: { resolve: (v: unknown) => void; reject: (e: unknown) => void }[] = []

const processQueue = (error: unknown) => {
    failedQueue.forEach((p) => {
        if (error) {
            p.reject(error)
        } else {
            p.resolve(true)
        }
    })
    failedQueue = []
}

const rejectUnauthorizedSession = (message?: string) => {
    emitAuthSessionExpired();
    return Promise.reject(new UnauthorizedError(message));
}

// ─── RESPONSE ───────────────────────────────────────────

api.interceptors.response.use(
    // Nếu response thành công thì trả về data
    (response) => response.data,

    // Nếu response lỗi thì xử lý lỗi
    async (error) => {
        if (!axios.isAxiosError(error)) {
            throw new ApiError(0, "Unknown error")
        }

        const originalRequest = error.config as (AxiosRequestConfig & { _retry?: boolean }) | undefined
        const status = error.response?.status
        const data = error.response?.data

        // Tránh loop refresh
        if (originalRequest?._retry) {
            if (status === 401) {
                return rejectUnauthorizedSession(data?.message);
            }

            return Promise.reject(error);
        }

        if (originalRequest?.url?.includes(API_ENDPOINTS.AUTH.REFRESH)) {
            processQueue(new UnauthorizedError());
            isRefreshing = false;
            return rejectUnauthorizedSession();
        }
        // HANDLE 401 + REFRESH TOKEN
        if (status === 401 && data?.errorCode === ERROR_CODE.Token_Expired) {
            if (!originalRequest) {
                return rejectUnauthorizedSession();
            }

            if (isRefreshing) {
                return new Promise((resolve, reject) => {
                    failedQueue.push({ resolve, reject });
                }).then(() => {
                    originalRequest._retry = true;
                    return api(originalRequest);
                });
            }

            originalRequest._retry = true
            isRefreshing = true

            try {
                await axios.post(
                    `${API_URL}/${API_ENDPOINTS.AUTH.REFRESH}`,
                    {},
                    { withCredentials: true }
                );

                processQueue(null)
                return api(originalRequest)
            } catch (err) {
                processQueue(err);
                return rejectUnauthorizedSession();
            } finally {
                isRefreshing = false
            }
        }

        // MAP STATUS CODE

        if (data?.errorCode === ERROR_CODE.Validation_Error) {
            throw new ValidationError(data?.errors || {}, data?.message)
        }

        switch (status) {
            case 401:
                return rejectUnauthorizedSession(data?.message)
            case 404:
                throw new NotFoundError(data?.message || "Not Found")
            case 500:
                throw new ApiError(500, data?.message || "Internal Server Error", data?.errorCode)
        }

        throw new ApiError(status || 0, data?.message || "API Error", data?.errorCode)
    }
);
