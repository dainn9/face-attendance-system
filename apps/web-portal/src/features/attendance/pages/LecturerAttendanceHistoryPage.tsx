import { useState } from "react";
import { FaEye } from "react-icons/fa";
import { Link, useParams } from "react-router-dom";
import { useCourseDetail } from "../../courses/hooks/course.query";
import {
    formatSemester,
    formatTimeToMinute,
} from "../../courses/types/course.types";
import { useAttendanceSessionHistory } from "../hooks/attendance.query";

const pageSize = 10;

const formatDate = (value: string) => {
    const date = new Date(value);

    if (Number.isNaN(date.getTime())) {
        return value;
    }

    return date.toLocaleDateString("vi-VN");
};

const getAttendanceSessionStatusLabel = (status: number) => {
    if (status === 1) return "Đang diễn ra";
    if (status === 2) return "Đã đóng";
    return "Không xác định";
};

const LecturerAttendanceHistoryPage = () => {
    const { courseId } = useParams();
    const [historyPage, setHistoryPage] = useState(1);
    const courseDetailQuery = useCourseDetail(courseId);
    const attendanceHistoryQuery = useAttendanceSessionHistory(courseId, {
        page: historyPage,
        pageSize,
    });
    const course = courseDetailQuery.data;
    const historyPageData = attendanceHistoryQuery.data;
    const historyItems = historyPageData?.items ?? [];

    const totalSessions = historyPageData?.totalCount ?? 0;
    const subtitle = `${course?.courseSectionCode ?? "Đang tải lớp"} - ${
        course
            ? `${formatSemester(course.semester)} ${course.academicYear}`
            : "Đang tải học kỳ"
    }`;

    if (courseDetailQuery.isLoading) {
        return (
            <div className="rounded-lg bg-white p-5 text-sm text-gray-500 shadow-sm">
                Đang tải lịch sử điểm danh của lớp...
            </div>
        );
    }

    return (
        <div className="space-y-5">
            <div className="text-sm text-gray-500">
                <Link to="/lecturer" className="hover:text-gray-900">
                    Lớp học của tôi
                </Link>
                <span className="mx-2">/</span>
                <Link
                    to={`/lecturer/courses/${courseId}`}
                    className="hover:text-gray-900"
                >
                    {course?.subjectName ?? "Chi tiết lớp học"}
                </Link>
                <span className="mx-2">/</span>
                <span className="text-gray-900">Lịch sử điểm danh</span>
            </div>

            <div className="rounded-lg bg-white p-5 shadow-sm">
                <div className="flex flex-col gap-4 lg:flex-row lg:items-start lg:justify-between">
                    <div>
                        <h1 className="mt-1 text-xl font-semibold text-gray-900">
                            Lịch sử điểm danh -{" "}
                            {course?.subjectName ?? "Lop hoc"}
                        </h1>
                        <p className="mt-1 text-sm text-gray-500">{subtitle}</p>
                    </div>
                    <Link
                        to={`/lecturer/courses/${courseId}`}
                        className="rounded-md border border-gray-300 px-4 py-2 text-sm text-gray-700"
                    >
                        Quay lại
                    </Link>
                </div>

                <div className="mt-4 grid gap-3 text-sm md:grid-cols-3">
                    <div className="rounded-md bg-gray-50 p-3">
                        <p className="text-xs text-gray-500">Mã lớp</p>
                        <p className="mt-1 font-semibold text-gray-900">
                            {course?.courseSectionCode ?? "-"}
                        </p>
                    </div>
                    <div className="rounded-md bg-gray-50 p-3">
                        <p className="text-xs text-gray-500">Môn học</p>
                        <p className="mt-1 font-semibold text-gray-900">
                            {course?.subjectName ?? "-"}
                        </p>
                    </div>
                    <div className="rounded-md bg-gray-50 p-3">
                        <p className="text-xs text-gray-500">Sinh viên</p>
                        <p className="mt-1 font-semibold text-gray-900">
                            {course?.studentCount ?? 0} sinh viên
                        </p>
                    </div>
                </div>
            </div>

            <div className="grid gap-3 sm:grid-cols-2 xl:grid-cols-4">
                {[
                    ["Tổng phiên", totalSessions, "đã diễn ra"],
                    ["Tỉ lệ trung bình", "Sắp ra mắt", "lớp này"],
                    ["Lượt có mặt", "Sắp ra mắt", "tổng sinh viên"],
                    ["Lượt vắng", "Sắp ra mắt", "tổng sinh viên"],
                ].map(([label, value, sub]) => (
                    <div
                        key={label}
                        className="rounded-lg bg-white p-4 shadow-sm"
                    >
                        <p className="text-xs text-gray-500">{label}</p>
                        <p className="mt-1 text-2xl font-semibold text-gray-900">
                            {value}
                        </p>
                        <p className="text-xs text-gray-500">{sub}</p>
                    </div>
                ))}
            </div>

            <section className="overflow-hidden rounded-lg bg-white shadow-sm">
                <div className="border-b border-gray-100 px-5 py-3">
                    <h2 className="text-sm font-semibold text-gray-900">
                        Các phiên điểm danh
                    </h2>
                </div>
                <div className="overflow-x-auto">
                    <table className="w-full text-left text-sm">
                        <thead className="bg-gray-50 text-xs text-gray-500">
                            <tr>
                                <th className="px-5 py-3 font-medium">Ngày</th>
                                <th className="px-5 py-3 font-medium">
                                    Bắt đầu
                                </th>
                                <th className="px-5 py-3 font-medium">
                                    Kết thúc
                                </th>
                                <th className="px-5 py-3 font-medium">
                                    Có mặt
                                </th>
                                <th className="px-5 py-3 font-medium">Vắng</th>
                                <th className="px-5 py-3 font-medium">Tỷ lệ</th>
                                <th className="px-5 py-3 font-medium">
                                    Trạng thái
                                </th>
                                <th className="px-5 py-3 font-medium">
                                    Thao tác
                                </th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-gray-100">
                            {attendanceHistoryQuery.isLoading && (
                                <tr>
                                    <td
                                        className="px-5 py-8 text-center text-gray-500"
                                        colSpan={8}
                                    >
                                        Dang tai lich su diem danh...
                                    </td>
                                </tr>
                            )}
                            {!attendanceHistoryQuery.isLoading &&
                                historyItems.length === 0 && (
                                    <tr>
                                        <td
                                            className="px-5 py-8 text-center text-gray-500"
                                            colSpan={8}
                                        >
                                            Chua co phien diem danh nao cho lop
                                            nay.
                                        </td>
                                    </tr>
                                )}
                            {historyItems.map((item) => (
                                <tr key={item.id}>
                                    <td className="px-5 py-3 text-gray-700">
                                        {formatDate(item.date)}
                                    </td>
                                    <td className="px-5 py-3 text-gray-500">
                                        {formatTimeToMinute(item.startTime)}
                                    </td>
                                    <td className="px-5 py-3 text-gray-500">
                                        {formatTimeToMinute(item.endTime)}
                                    </td>
                                    <td className="px-5 py-3 text-emerald-700">
                                        {item.presentCount}
                                    </td>
                                    <td className="px-5 py-3 text-red-700">
                                        {item.absentCount}
                                    </td>
                                    <td className="px-5 py-3">
                                        <div className="flex items-center gap-2">
                                            <span className="w-10 font-medium text-gray-900">
                                                {Math.round(
                                                    item.attendanceRate,
                                                )}
                                                %
                                            </span>
                                            <div className="h-1.5 w-20 rounded-full bg-gray-100">
                                                <div
                                                    className="h-1.5 rounded-full bg-emerald-600"
                                                    style={{
                                                        width: `${item.attendanceRate}%`,
                                                    }}
                                                />
                                            </div>
                                        </div>
                                    </td>
                                    <td className="px-5 py-3">
                                        <span className="rounded-md bg-gray-100 px-2 py-1 text-xs text-gray-600">
                                            {getAttendanceSessionStatusLabel(
                                                item.status,
                                            )}
                                        </span>
                                    </td>
                                    <td className="px-5 py-3">
                                        <Link
                                            to={`/lecturer/courses/${courseId}/attendance-sessions/${item.id}`}
                                            className="inline-flex size-8 items-center justify-center rounded-md border border-gray-200 text-gray-600 hover:bg-gray-50 hover:text-gray-900"
                                            title="Xem lại"
                                        >
                                            <FaEye />
                                        </Link>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
                <div className="flex items-center justify-between border-t border-gray-100 px-5 py-3 text-sm text-gray-500">
                    <span>
                        Hien thi{" "}
                        {historyItems.length === 0
                            ? 0
                            : (historyPage - 1) * pageSize + 1}
                        -{(historyPage - 1) * pageSize + historyItems.length} /{" "}
                        {totalSessions} phien diem danh
                    </span>
                    <div className="flex gap-1">
                        {Array.from(
                            { length: historyPageData?.totalPages ?? 1 },
                            (_, index) => index + 1,
                        ).map((pageNumber) => (
                            <button
                                key={pageNumber}
                                type="button"
                                onClick={() => setHistoryPage(pageNumber)}
                                className={`size-8 rounded-md border text-sm ${
                                    pageNumber === historyPage
                                        ? "bg-gray-100 text-gray-900"
                                        : "border-gray-200 text-gray-500"
                                }`}
                            >
                                {pageNumber}
                            </button>
                        ))}
                    </div>
                </div>
            </section>
        </div>
    );
};

export default LecturerAttendanceHistoryPage;
