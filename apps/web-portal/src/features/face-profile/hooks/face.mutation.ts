import { useMutation, useQueryClient } from "@tanstack/react-query";
import { faceApi } from "../services/face.api";
import { faceQueryKeys } from "./face.query";
import { toastEmitter } from "../../../shared/utils/toastEmitter";

type RegisterFaceFiles = {
    left: File;
    center: File;
    right: File;
};

export const useRegisterFace = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({ left, center, right }: RegisterFaceFiles) =>
            faceApi.registerFace(left, center, right),

        onSuccess: () => {
            toastEmitter.success("Cập nhật khuôn mặt thành công");
            queryClient.invalidateQueries({
                queryKey: faceQueryKeys.status(),
            });
        },
    });
};
