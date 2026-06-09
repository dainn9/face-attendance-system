import { useQuery } from "@tanstack/react-query";
import { userApi } from "../services/profile.api";

export const profileQueryKeys = {
    all: ["profile"],
    profile: () => [...profileQueryKeys.all, "profile"],
};

export const useProfileDetail = () =>
    useQuery({
        queryKey: profileQueryKeys.profile(),
        queryFn: () => userApi.getProfile(),
        placeholderData: (previousData) => previousData
    });