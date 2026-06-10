import { useMutation, useQueryClient } from "@tanstack/react-query"
import { authApi } from "../services/auth.api";
import { useNavigate } from "react-router-dom";
import type { LoginRequest } from "../types/auth.types";
import { useAuthStore } from "../store/auth.store";
import { getHomePathByRole, getMeFromPayload } from "../utils/roleRedirect";

export const useLogin = () => {
    const queryClient = useQueryClient();
    const navigate = useNavigate();
    const setUser = useAuthStore((s) => s.setUser);

    return useMutation({
        mutationFn: (data: LoginRequest) =>
            authApi.login(data),

        onSuccess: async () => {
            const payload = await queryClient.fetchQuery({
                queryKey: ["me"],
                queryFn: authApi.getMe,
                retry: false,
            });

            const user = getMeFromPayload(payload);
            setUser(user);
            navigate(getHomePathByRole(user.role));
        }
    })
}

export const useLogout = () => {
    const queryClient = useQueryClient();
    const navigate = useNavigate();
    const setUser = useAuthStore((s) => s.setUser);

    return useMutation({
        mutationFn: () => authApi.logout(),

        onSuccess: () => {
            setUser(null);
            queryClient.removeQueries({
                queryKey: ["me"]
            });
            navigate("/login");
        }
    })
}
