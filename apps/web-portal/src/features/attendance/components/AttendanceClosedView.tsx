import { FaExclamationTriangle } from "react-icons/fa";
import { Link } from "react-router-dom";
import AttendanceInfoCard, {
    type AttendanceInfoItem,
} from "./AttendanceInfoCard";

type AttendanceClosedViewProps = {
    items: AttendanceInfoItem[];
    studentPath: string;
};

const AttendanceClosedView = ({
    items,
    studentPath,
}: AttendanceClosedViewProps) => (
    <div className="flex min-h-[calc(100vh-88px)] items-center justify-center">
        <section className="w-full max-w-3xl rounded-lg border border-gray-200 bg-white p-6 text-center shadow-sm sm:p-8">
            <span className="mx-auto flex size-16 items-center justify-center rounded-full bg-gray-100 text-2xl text-gray-500 ring-8 ring-gray-50">
                <FaExclamationTriangle />
            </span>
            <h1 className="mt-5 text-2xl font-semibold text-gray-900">
                Phiên điểm danh đã đóng
            </h1>
            <p className="mx-auto mt-2 max-w-xl text-sm leading-6 text-gray-600">
                Bạn không thể thực hiện điểm danh vì giảng viên đã đóng phiên
                này.
            </p>

            <AttendanceInfoCard items={items} variant="closed" />

            <Link
                to={studentPath}
                className="mt-7 inline-flex min-h-11 items-center justify-center rounded-md bg-blue-600 px-5 py-2 text-sm font-semibold text-white transition hover:bg-blue-700"
            >
                Quay lại
            </Link>
        </section>
    </div>
);

export default AttendanceClosedView;
