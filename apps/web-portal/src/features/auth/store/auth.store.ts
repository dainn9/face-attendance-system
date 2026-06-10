import { create } from "zustand";
import type { Me } from "../../../shared/api/types";

type AuthState = {
    isAuthenticated: boolean;
    isAuthLoading: boolean;
    user: Me | null;
    setAuth: (v: boolean) => void;
    setUser: (user: Me | null) => void;
    finishAuthLoading: () => void;
};

export const useAuthStore = create<AuthState>((set) => ({
    isAuthenticated: false,
    isAuthLoading: true,
    user: null,
    setAuth: (v) => set({ isAuthenticated: v }),
    setUser: (user) => set({ user, isAuthenticated: Boolean(user) }),
    finishAuthLoading: () => set({ isAuthLoading: false }),
}));
