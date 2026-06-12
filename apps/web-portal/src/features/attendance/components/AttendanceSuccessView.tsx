import { FaCheck, FaHome } from "react-icons/fa";
import { Link } from "react-router-dom";
import AttendanceInfoCard, {
    type AttendanceInfoItem,
} from "./AttendanceInfoCard";

type AttendanceSuccessViewProps = {
    homePath: string;
    items: AttendanceInfoItem[];
};

const AttendanceSuccessView = ({
    homePath,
    items,
}: AttendanceSuccessViewProps) => (
    <div className="flex min-h-[calc(100vh-88px)]">
        <section className="flex w-full items-center justify-center bg-white p-6 text-center shadow-sm ring-1 ring-emerald-100 sm:p-8">
            <div className="w-full max-w-3xl">
                <span className="mx-auto flex size-20 items-center justify-center rounded-full bg-emerald-100 text-3xl text-emerald-700 ring-8 ring-emerald-50">
                    <FaCheck />
                </span>
                <h1 className="mt-6 text-2xl font-semibold text-gray-900">
                    Điểm danh thành công!
                </h1>
                <AttendanceInfoCard items={items} variant="success" />
                <Link
                    to={homePath}
                    className="mt-7 inline-flex min-h-11 items-center justify-center gap-2 rounded-md bg-blue-600 px-5 py-2 text-sm font-semibold text-white transition hover:bg-blue-700"
                >
                    <FaHome />
                    Về trang chủ
                </Link>
            </div>
        </section>
    </div>
);

export default AttendanceSuccessView;
