import { useMutation } from "@tanstack/react-query"
import { facultyApi } from "../services/faculty.api"
import type { CreateFacultyRequest } from "../types/faculty.types"
import { useNavigate } from "react-router-dom";
import { toastEmitter } from "../../../shared/utils/toastEmitter";

export const useCreateFaculty = () => {
    const navigate = useNavigate();
    
    return useMutation({
        mutationFn: async (data: CreateFacultyRequest) =>
            {
                const response = await facultyApi.create(data);
                return response.data;
            },

        onSuccess: (result) => {
            toastEmitter.success("Tạo khoa thành công!");
            navigate(`/faculties/${result.data}`);
        }
    })
}