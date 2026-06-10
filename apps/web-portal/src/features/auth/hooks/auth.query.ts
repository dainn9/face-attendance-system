import { useQuery } from "@tanstack/react-query"
import { authApi } from "../services/auth.api"

export const useMe = (enabled = true) => {
    return useQuery({
        queryKey: ["me"],
        queryFn: () => authApi.getMe(),
        retry: false,
        enabled,
    })
}