import { create } from "zustand";

type AuthState = {
    isAuthenticated: boolean;
    setAuth: (v: boolean) => void;
};

export const useAuthStore = create<AuthState>((set) => ({
    isAuthenticated: false,
    setAuth: (v) => set({ isAuthenticated: v }),
}));