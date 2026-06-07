import { useMutation, useQueryClient } from "@tanstack/react-query"
import { authApi } from "../services/auth.api";
import { useNavigate } from "react-router-dom";
import type { LoginRequest } from "../types/auth.types";
import { useAuthStore } from "../store/auth.store";

export const useLogin = () => {
    const queryClient = useQueryClient();
    const navigate = useNavigate();
    const setAuth = useAuthStore((s) => s.setAuth);

    return useMutation({
        mutationFn: (data: LoginRequest) =>
            authApi.login(data),

        onSuccess: () => {
            setAuth(true);
            queryClient.invalidateQueries({
                queryKey: ["me"]
            });
            navigate("/");
        }
    })
}

export const useLogout = () => {
    const queryClient = useQueryClient();
    const navigate = useNavigate();
    const setAuth = useAuthStore((s) => s.setAuth);

    return useMutation({
        mutationFn: () => authApi.logout(),

        onSuccess: () => {
            setAuth(false);
            queryClient.removeQueries({
                queryKey: ["me"]
            });
            navigate("/login");
        }
    })
}