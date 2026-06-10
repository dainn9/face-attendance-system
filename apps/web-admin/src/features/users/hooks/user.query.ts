import { useQuery } from "@tanstack/react-query";
import { userApi } from "../../../shared/api/services/user.api";
import type { GetUserPagedRequest } from "../types/user.types";

export const userQueryKeys = {
    all: ["users"] as const,
    lists: () => [...userQueryKeys.all, "list"] as const,
    list: (params: GetUserPagedRequest) => [...userQueryKeys.lists(), params] as const,
}

export const useUsers = (
    params: GetUserPagedRequest
) =>
    useQuery({
        queryKey: userQueryKeys.list(params),
        queryFn: () => userApi.list(params),
        placeholderData: (previousData) => previousData
    });