import { useMutation, useQueryClient } from "@tanstack/react-query"
import { facultyApi } from "../services/faculty.api"
import type { FacultyRequest, MajorRequest } from "../types/faculty.types"
import { useNavigate } from "react-router-dom";
import { toastEmitter } from "../../../shared/utils/toastEmitter";
import { facultyQueryKeys } from "./faculty.query";

export const useCreateFaculty = () => {
    const navigate = useNavigate();
    
    return useMutation({
        mutationFn: async (data: FacultyRequest) =>
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

export const useUpdateFaculty = (facultyId: string) => {
    const queryClient = useQueryClient();
       
    return useMutation({
        mutationFn: (data: FacultyRequest) => 
            facultyApi.update(facultyId, data),

        onSuccess: () => {
            toastEmitter.success("Cập nhật khoa thành công!");
            queryClient.invalidateQueries({
                queryKey: facultyQueryKeys.detail(facultyId),
            });
        }
    })
}

export const useCreateMajor = (facultyId: string) => {
    const queryClient = useQueryClient();
       
    return useMutation({
        mutationFn: (data: MajorRequest) => 
            facultyApi.createMajor(facultyId, data),

        onSuccess: () => {
            toastEmitter.success("Tạo ngành thành công!");

            queryClient.invalidateQueries({
                queryKey: facultyQueryKeys.detail(facultyId),
            });
        }
    })
}

export const useUpdateMajor = (facultyId: string, majorId: string) => {
    const queryClient = useQueryClient();
       
    return useMutation({
        mutationFn: (data: MajorRequest) => 
            facultyApi.updateMajor(facultyId, majorId, data),

        onSuccess: () => {
            toastEmitter.success("Cập nhật ngành thành công!");

            queryClient.invalidateQueries({
                queryKey: facultyQueryKeys.detail(facultyId),
            });
        }
    })
}

