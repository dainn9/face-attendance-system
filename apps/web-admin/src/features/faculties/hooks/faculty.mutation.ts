import { useMutation, useQueryClient } from "@tanstack/react-query"
import { facultyApi } from "../services/faculty.api"
import type { CreateFacultyRequest } from "../types/faculty.types"
import { useNavigate } from "react-router-dom";
import { toastEmitter } from "../../../shared/utils/toastEmitter";
import { facultyQueryKeys } from "./faculty.query";

export const useCreateFaculty = () => {
    const navigate = useNavigate();
    
    return useMutation({
        mutationFn: async (data: CreateFacultyRequest) =>
            {
                const response = await facultyApi.create(data);
                return response;
            },

        onSuccess: (result) => {
            toastEmitter.success("Tạo khoa thành công!");
            navigate(`/faculties/${result}`);
        }
    })
}

export const useCreateMajor = (facultyId: string) => {
    const queryClient = useQueryClient();
       
    return useMutation({
        mutationFn: (data: { name: string; code: string }) => 
            facultyApi.createMajor(facultyId, data),

        onSuccess: () => {
            toastEmitter.success("Tạo ngành thành công!");

            queryClient.invalidateQueries({
                queryKey: facultyQueryKeys.detail(facultyId),
            });
        }
    })
}