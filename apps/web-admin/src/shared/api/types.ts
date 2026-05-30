export type ApiResponse<T> = {
    success: boolean;
    message?: string;
    errors?: unknown;
    data: T;
};