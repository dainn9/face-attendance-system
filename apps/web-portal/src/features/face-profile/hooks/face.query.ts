import { useQuery } from "@tanstack/react-query";
import { faceApi } from "../services/face.api";

export const faceQueryKeys = {
    all: ["face"],
    status: () => [...faceQueryKeys.all, "status"],
};

export const useFaceStatus = () =>
    useQuery({
        queryKey: faceQueryKeys.status(),
        queryFn: () => faceApi.getStatus(),
        placeholderData: (previousData) => previousData,
    });
