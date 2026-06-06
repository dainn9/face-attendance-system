import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toastEmitter } from "../../../shared/utils/toastEmitter";
import { userApi } from "../../../shared/api/services/user.api";
import type { CreateUserRequest } from "../types/user.types";
import { facultyQueryKeys } from "../../faculties/hooks/faculty.query";
import { useNavigate } from "react-router-dom";

export const useCreateUser = () => {
    const queryClient = useQueryClient();
    const navigate = useNavigate();

    return useMutation({
        mutationFn: (data: CreateUserRequest) => 
            userApi.create(data),

        onSuccess: () => {
            toastEmitter.success("Tạo người dùng thành công!");
            queryClient.invalidateQueries({
                queryKey: facultyQueryKeys.lists(),
            });
            navigate("/users");
        }
    })
}