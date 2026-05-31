import CreateFacultyCard from "../components/CreateFacultyCard";
import FacultyCard from "../components/FacultyCard";
import { useFaculties } from "../hooks/faculty.query";
import Skeleton from "react-loading-skeleton";
import "react-loading-skeleton/dist/skeleton.css";
import { useState } from "react";
import CreateFacultyModal from "../components/CreateFacultyModal";

const FacultyListPage = () => {
    const { data: faculties = [], isLoading } = useFaculties();
    const [isCreateOpen, setIsCreateOpen] = useState(false);

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

            <CreateFacultyModal
                isOpen={isCreateOpen}
                onClose={() => setIsCreateOpen(false)}
            />
        </div>
    );
};

export default FacultyListPage;
