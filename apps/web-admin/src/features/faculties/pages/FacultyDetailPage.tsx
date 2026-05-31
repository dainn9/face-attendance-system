import { useParams } from "react-router-dom";
import { useFacultyDetail } from "../hooks/faculty.query";
import Skeleton from "react-loading-skeleton";
import FacultyDetailView from "../components/FacultyDetailView";

const FacultyDetailPage = () => {
    const { id } = useParams();
    const { data: facultyDetail, isLoading } = useFacultyDetail(id!);

    if (isLoading) {
        return (
            <>
                <Skeleton height={40} width={250} />
                <Skeleton count={5} height={100} />
            </>
        );
    }

    if (!facultyDetail) {
        return <div className="p-6 text-gray-500">Không tìm thấy khoa.</div>;
    }

    return (
        <FacultyDetailView
            data={facultyDetail}
            onCreateMajor={() => {
                console.log("open create major modal");
            }}
        />
    );
};

export default FacultyDetailPage;
