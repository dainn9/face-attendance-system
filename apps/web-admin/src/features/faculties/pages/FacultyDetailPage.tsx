import { useParams } from "react-router-dom";
import { useFacultyDetail } from "../hooks/faculty.query";
import Skeleton from "react-loading-skeleton";
import FacultyDetailView from "../components/FacultyDetailView";
import { useState } from "react";
import {
    useCreateMajor,
    useUpdateFaculty,
    useUpdateMajor,
} from "../hooks/faculty.mutation";
import FacultyFormModal from "../components/FacultyFormModal";
import type {
    FacultyRequest,
    Major,
    MajorRequest,
} from "../types/faculty.types";
import MajorFormModal from "../components/MajorFormModal";

const FacultyDetailPage = () => {
    const { id } = useParams();
    const { data: facultyDetail, isLoading } = useFacultyDetail(id!);
    const [isCreateOpenMajor, setIsCreateOpenMajor] = useState(false);
    const [isUpdateFacultyOpen, setIsUpdateFacultyOpen] = useState(false);
    const [isUpdateMajorOpen, setIsUpdateMajorOpen] = useState(false);
    const [selectedMajor, setSelectedMajor] = useState<Major | null>(null);

    const {
        mutate: updateFaculty,
        isPending: isUpdatingFaculty,
        error: updateFacultyError,
        reset: resetUpdateFaculty,
    } = useUpdateFaculty(id!);

    const {
        mutate: createMajor,
        isPending: isCreatingMajor,
        error: createMajorError,
        reset: resetCreateMajor,
    } = useCreateMajor(id!);

    const {
        mutate: updateMajor,
        isPending: isUpdatingMajor,
        error: updateMajorError,
        reset: resetUpdateMajor,
    } = useUpdateMajor(id!, selectedMajor?.id!);

    const handleOpenUpdateFaculty = () => setIsUpdateFacultyOpen(true);
    const handleCloseUpdateFaculty = () => {
        resetUpdateFaculty();
        setIsUpdateFacultyOpen(false);
    };

    const handleSubmitUpdateFaculty = (data: FacultyRequest) => {
        updateFaculty(data, {
            onSuccess: () => {
                handleCloseUpdateFaculty();
            },
        });
    };

    const handleOpenCreateMajor = () => setIsCreateOpenMajor(true);
    const handleCloseCreateMajor = () => {
        resetCreateMajor();
        setIsCreateOpenMajor(false);
    };

    const handleSubmitCreateMajor = (data: MajorRequest) => {
        createMajor(data, {
            onSuccess: () => {
                handleCloseCreateMajor();
            },
        });
    };

    const handleOpenUpdateMajor = (major: Major) => {
        setSelectedMajor(major);
        setIsUpdateMajorOpen(true);
    };

    const handleCloseUpdateMajor = () => {
        resetUpdateMajor();
        setIsUpdateMajorOpen(false);
    };

    const handleSubmitUpdateMajor = (data: MajorRequest) => {
        updateMajor(data, {
            onSuccess: () => {
                handleCloseUpdateMajor();
            },
        });
    };

    if (isLoading) return <FacultyDetailSkeleton />;

    if (!facultyDetail) {
        return <div className="p-6 text-gray-500">Không tìm thấy khoa.</div>;
    }

    return (
        <>
            <FacultyDetailView
                data={facultyDetail}
                onCreateMajor={handleOpenCreateMajor}
                onEditFaculty={handleOpenUpdateFaculty}
                onEditMajor={handleOpenUpdateMajor}
            />

            <MajorFormModal
                facultyId={facultyDetail.id}
                name={facultyDetail.name}
                code={facultyDetail.code}
                mode="create"
                isPending={isCreatingMajor}
                error={createMajorError}
                isOpen={isCreateOpenMajor}
                onClose={handleCloseCreateMajor}
                onSubmit={handleSubmitCreateMajor}
            />

            <MajorFormModal
                facultyId={facultyDetail.id}
                name={facultyDetail.name}
                code={facultyDetail.code}
                mode="edit"
                initialData={{
                    name: selectedMajor?.name || "",
                    code: selectedMajor?.code || "",
                }}
                isPending={isUpdatingMajor}
                error={updateMajorError}
                isOpen={isUpdateMajorOpen}
                onClose={handleCloseUpdateMajor}
                onSubmit={handleSubmitUpdateMajor}
            />

            <FacultyFormModal
                isOpen={isUpdateFacultyOpen}
                mode="edit"
                initialData={{
                    name: facultyDetail.name,
                    code: facultyDetail.code,
                }}
                isPending={isUpdatingFaculty}
                error={updateFacultyError}
                onClose={handleCloseUpdateFaculty}
                onSubmit={handleSubmitUpdateFaculty}
            />
        </>
    );
};

export default FacultyDetailPage;

const FacultyDetailSkeleton = () => {
    return (
        <div className="p-6">
            {/* Breadcrumb */}
            <div className="mb-4 flex items-center gap-2">
                <Skeleton width={80} height={16} />
                <Skeleton width={16} height={16} />
                <Skeleton width={120} height={16} />
            </div>

            {/* Header card */}
            <div className="mb-6 flex items-center justify-between rounded-2xl border bg-white p-5 shadow-sm">
                <div className="flex items-center gap-4">
                    <Skeleton width={56} height={56} borderRadius={16} />
                    <div>
                        <Skeleton width={200} height={28} className="mb-1" />
                        <Skeleton width={120} height={16} />
                    </div>
                </div>
                <Skeleton width={80} height={36} borderRadius={12} />
            </div>

            {/* Stat cards */}
            <div className="mb-6 grid gap-4 md:grid-cols-3">
                {[1, 2, 3].map((i) => (
                    <Skeleton key={i} height={80} borderRadius={16} />
                ))}
            </div>

            {/* Majors list */}
            <div className="mb-6 rounded-2xl border bg-white p-5 shadow-sm">
                <div className="mb-4 flex items-center justify-between">
                    <Skeleton width={140} height={24} />
                    <Skeleton width={100} height={36} borderRadius={12} />
                </div>
                <div className="divide-y">
                    {[1, 2, 3].map((i) => (
                        <div
                            key={i}
                            className="flex items-center justify-between py-4"
                        >
                            <div>
                                <Skeleton
                                    width={160}
                                    height={18}
                                    className="mb-1"
                                />
                                <Skeleton width={80} height={14} />
                            </div>
                            <Skeleton width={100} height={16} />
                        </div>
                    ))}
                </div>
            </div>

            {/* Lecturers list */}
            <div className="rounded-2xl border bg-white p-5 shadow-sm">
                <Skeleton width={120} height={24} className="mb-4" />
                <div className="divide-y">
                    {[1, 2, 3].map((i) => (
                        <div key={i} className="flex items-center gap-3 py-4">
                            <Skeleton circle width={40} height={40} />
                            <div>
                                <Skeleton
                                    width={140}
                                    height={18}
                                    className="mb-1"
                                />
                                <Skeleton width={80} height={14} />
                            </div>
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
};
