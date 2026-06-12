export interface ApiResponse<T> {
    success: boolean;
    message?: string;
    errorCode?: string;
    errors?: unknown;
    data: T;
};

export interface PagedResult<T> {
    items: T[];
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
    hasNextPage: boolean;
    hasPreviousPage: boolean;
}

export interface Me {
    id: string;
    email: string;
    role: string;
}
