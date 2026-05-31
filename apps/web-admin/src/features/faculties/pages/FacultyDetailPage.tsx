import { useParams } from "react-router-dom";
import { useFacultyDetail } from "../hooks/faculty.query";
import Skeleton from "react-loading-skeleton";
import FacultyDetailView from "../components/FacultyDetailView";
import { useState } from "react";
import CreateMajorModal from "../components/CreateMajorModal";

const FacultyDetailPage = () => {
    const { id } = useParams();
    const { data: facultyDetail, isLoading } = useFacultyDetail(id!);
    const [isCreateOpen, setIsCreateOpen] = useState(false);

    if (isLoading) {
        return (
            <>
                <Skeleton height={40} width={250} />
                <Skeleton count={5} height={100} />
            </>
        );
    }

    const handleOpenCreateMajor = () => setIsCreateOpen(true);
    const handleCloseCreateMajor = () => setIsCreateOpen(false);

    if (!facultyDetail) {
        return <div className="p-6 text-gray-500">Không tìm thấy khoa.</div>;
    }

    return (
        <>
            <FacultyDetailView
                data={facultyDetail}
                onCreateMajor={handleOpenCreateMajor}
            />

            <CreateMajorModal
                facultyId={facultyDetail.id}
                name={facultyDetail.name}
                code={facultyDetail.code}
                isOpen={isCreateOpen}
                onClose={handleCloseCreateMajor}
            />
        </>
    );
};

export default FacultyDetailPage;
