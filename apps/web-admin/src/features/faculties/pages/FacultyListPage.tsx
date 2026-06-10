import CreateFacultyCard from "../components/CreateFacultyCard";
import FacultyCard from "../components/FacultyCard";
import { useFaculties } from "../hooks/faculty.query";
import Skeleton from "react-loading-skeleton";
import "react-loading-skeleton/dist/skeleton.css";
import { useState } from "react";
import FacultyFormModal from "../components/FacultyFormModal";
import { useCreateFaculty } from "../hooks/faculty.mutation";
import type { FacultyRequest } from "../types/faculty.types";

const FacultyListPage = () => {
    const { data: faculties = [], isLoading } = useFaculties();
    const {
        mutate: createFaculty,
        error,
        reset,
        isPending,
    } = useCreateFaculty();
    const [isCreateOpen, setIsCreateOpen] = useState(false);

    const handleClose = () => {
        reset();
        setIsCreateOpen(false);
    };

    const handleSubmit = (data: FacultyRequest) => {
        createFaculty(data, {
            onSuccess: () => {
                handleClose();
            },
        });
    };

    if (isLoading) {
        return (
            <>
                <Skeleton height={40} width={250} />
                <Skeleton count={5} height={100} />
            </>
        );
    }
    return (
        <div className="p-6">
            <div className="mb-6 flex items-center justify-between">
                <h1 className="text-2xl font-bold text-gray-900">
                    Khoa / Ngành
                </h1>

                <button
                    onClick={() => setIsCreateOpen(true)}
                    className="
                    rounded-xl
                    bg-blue-600
                    px-4
                    py-2
                    text-sm
                    font-semibold
                    text-white
                    hover:bg-blue-700
                "
                >
                    Thêm khoa
                </button>
            </div>

            <div
                className="
                    grid
                    grid-cols-[repeat(auto-fill,minmax(360px,1fr))]
                    gap-6
                "
            >
                {faculties.map((faculty) => (
                    <FacultyCard key={faculty.id} faculty={faculty} />
                ))}

                <CreateFacultyCard onClick={() => setIsCreateOpen(true)} />
            </div>

            <FacultyFormModal
                isOpen={isCreateOpen}
                mode="create"
                isPending={isPending}
                error={error}
                onClose={handleClose}
                onSubmit={handleSubmit}
            />
        </div>
    );
};

export default FacultyListPage;
