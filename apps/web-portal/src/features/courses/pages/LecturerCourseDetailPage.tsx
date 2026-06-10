import { useState } from "react";
import { Link, useParams } from "react-router-dom";
import { toastEmitter } from "../../../shared/utils/toastEmitter";
import { useStartAttendanceSession } from "../../attendance/hooks/attendance.mutation";
import { useAttendanceSessionHistory } from "../../attendance/hooks/attendance.query";
import {
    useCourseDetail,
    useLecturerCourseStudents,
} from "../hooks/course.query";
import {
    formatDayOfWeek,
    formatSemester,
    formatTimeToMinute,
} from "../types/course.types";

const pageSize = 10;
const recentAttendancePageSize = 3;

const getInitials = (name: string) =>
    name
        .trim()
        .split(/\s+/)
        .slice(-2)
        .map((part) => part[0])
        .join("")
        .toUpperCase();

const getErrorMessage = (error: unknown, fallback: string) => {
    if (error instanceof Error && error.message) {
        return error.message;
    }

    return fallback;
};

const LecturerCourseDetailPage = () => {
    const { courseId } = useParams();
    const [page, setPage] = useState(1);
    const [isStartConfirmOpen, setIsStartConfirmOpen] = useState(false);
    const startAttendanceSessionMutation = useStartAttendanceSession();
    const courseDetailQuery = useCourseDetail(courseId);
    const courseStudentsQuery = useLecturerCourseStudents(courseId, {
        page,
        pageSize,
    });
    const attendanceHistoryQuery = useAttendanceSessionHistory(courseId, {
        page: 1,
        pageSize: recentAttendancePageSize,
    });
    const course = courseDetailQuery.data;
    const studentPage = courseStudentsQuery.data;
    const students = studentPage?.items ?? [];
    const recentAttendanceHistory = attendanceHistoryQuery.data?.items ?? [];

    if (courseDetailQuery.isLoading) {
        return (
            <div className="rounded-lg bg-white p-5 text-sm text-gray-500 shadow-sm">
                Dang tai thong tin lop...
            </div>
        );
    }

    if (!course) {
        return (
            <div className="rounded-lg bg-white p-5 text-sm text-gray-500 shadow-sm">
                Khong tim thay lop hoc.
            </div>
        );
    }

    const statusLabel = course.isActive ? "Hoạt động" : "Đã kết thúc";
    const semesterLabel = `${formatSemester(course.semester)} ${course.academicYear}`;
    const attendanceHistoryPath = `/lecturer/courses/${course.id}/attendance-history`;
    const handleStartAttendanceSession = () => {
        startAttendanceSessionMutation.mutate(course.id, {
            onSuccess: () => {
                setIsStartConfirmOpen(false);
                toastEmitter.success("Đã tạo phiên điểm danh.");
            },
            onError: (error) => {
                toastEmitter.error(
                    getErrorMessage(
                        error,
                        "Không thể tạo phiên điểm danh. Vui lòng thử lại.",
                    ),
                );
            },
        });
    };

    return (
        <div className="space-y-5">
            <div className="text-sm text-gray-500">
                <Link to="/lecturer" className="hover:text-gray-900">
                    Lớp học của tôi
                </Link>
                <span className="mx-2">/</span>
                <span className="text-gray-900">{course.subjectName}</span>
            </div>

            <div className="flex flex-col gap-3 lg:flex-row lg:items-start lg:justify-between">
                <div>
                    <div className="flex items-center gap-3">
                        <h1 className="text-xl font-semibold text-gray-900">
                            {course.subjectName}
                        </h1>
                        <span
                            className={`rounded-md px-2 py-1 text-xs font-medium ${course.isActive ? "bg-green-100 text-green-700" : "bg-gray-100 text-gray-600"}`}
                        >
                            {statusLabel}
                        </span>
                    </div>
                    <p className="mt-1 text-sm text-gray-500">
                        {course.courseSectionCode} - {semesterLabel}
                    </p>
                </div>
                <div className="flex gap-2">
                    <Link
                        to={attendanceHistoryPath}
                        className="rounded-md border border-gray-300 px-4 py-2 text-sm text-gray-700"
                    >
                        Lịch sử điểm danh
                    </Link>
                    <button
                        type="button"
                        onClick={() => setIsStartConfirmOpen(true)}
                        className="rounded-md bg-emerald-600 px-4 py-2 text-sm font-medium text-white"
                        disabled={startAttendanceSessionMutation.isPending}
                    >
                        Tạo buổi điểm danh
                    </button>
                </div>
            </div>

            <div className="grid auto-rows-fr gap-3 sm:grid-cols-3">
                {[
                    [
                        "Số lượng sinh viên",
                        course.studentCount,
                        `/ ${course.maxCapacity} tối đa`,
                    ],
                    ["Tín chỉ", course.credits, "tín chỉ"],
                    ["Trạng thái", statusLabel, "tình trạng lớp học"],
                ].map(([label, value, sub]) => (
                    <div
                        key={label}
                        className="h-full rounded-lg bg-white p-4 shadow-sm"
                    >
                        <p className="text-xs text-gray-500">{label}</p>
                        <p className="mt-1 text-2xl font-semibold text-gray-900">
                            {value}
                        </p>
                        <p className="text-xs text-gray-500">{sub}</p>
                    </div>
                ))}
            </div>

            <div className="grid gap-4 xl:grid-cols-2">
                <section className="rounded-lg bg-white p-5 shadow-sm">
                    <h2 className="text-sm font-semibold text-gray-900">
                        Thông tin lớp học
                    </h2>
                    <div className="mt-3 divide-y divide-gray-100 text-sm">
                        {[
                            ["Môn học", course.subjectName],
                            ["Mã lớp", course.courseSectionCode],
                            ["Học kỳ", semesterLabel],
                            ["Giảng viên", course.lecturer.fullName],
                        ].map(([label, value]) => (
                            <div
                                key={label}
                                className="flex justify-between py-2"
                            >
                                <span className="text-gray-500">{label}</span>
                                <span className="font-medium text-gray-900">
                                    {value}
                                </span>
                            </div>
                        ))}
                    </div>
                </section>
                <section className="rounded-lg bg-white p-5 shadow-sm">
                    <h2 className="text-sm font-semibold text-gray-900">
                        Lịch học
                    </h2>
                    <div className="mt-3 space-y-2">
                        {course.schedules.map((item) => (
                            <div
                                key={item.id}
                                className="rounded-md bg-blue-50 px-3 py-2 text-sm text-blue-900"
                            >
                                {formatDayOfWeek(item.dayOfWeek)}{" "}
                                {formatTimeToMinute(item.startTime)} -{" "}
                                {formatTimeToMinute(item.endTime)} - P.
                                {item.room}
                            </div>
                        ))}
                    </div>
                </section>
            </div>

            <section className="overflow-hidden rounded-lg bg-white shadow-sm">
                <div className="flex items-center justify-between border-b border-gray-100 px-5 py-3">
                    <h2 className="text-sm font-semibold text-gray-900">
                        Danh sách sinh viên
                    </h2>
                </div>
                <table className="w-full text-left text-sm">
                    <thead className="bg-gray-50 text-xs text-gray-500">
                        <tr>
                            <th className="px-5 py-3 font-medium">Sinh viên</th>
                            <th className="px-5 py-3 font-medium">MSSV</th>
                            <th className="px-5 py-3 font-medium">Số buổi</th>
                            <th className="px-5 py-3 font-medium">Tỷ lệ</th>
                        </tr>
                    </thead>
                    <tbody className="divide-y divide-gray-100">
                        {courseStudentsQuery.isLoading && (
                            <tr>
                                <td
                                    className="px-5 py-6 text-center text-gray-500"
                                    colSpan={5}
                                >
                                    Danh sách sinh viên đang được tải...
                                </td>
                            </tr>
                        )}
                        {!courseStudentsQuery.isLoading &&
                            students.length === 0 && (
                                <tr>
                                    <td
                                        className="px-5 py-6 text-center text-gray-500"
                                        colSpan={5}
                                    >
                                        Không tìm thấy sinh viên nào.
                                    </td>
                                </tr>
                            )}
                        {students.map((item) => {
                            const attendanceRate = Math.round(
                                item.attendanceRate,
                            );
                            return (
                                <tr key={item.userId}>
                                    <td className="px-5 py-3">
                                        <div className="flex items-center gap-2">
                                            <span className="flex size-8 items-center justify-center rounded-full bg-indigo-100 text-xs font-semibold text-indigo-700">
                                                {getInitials(item.fullName)}
                                            </span>
                                            <div>
                                                <p className="font-medium text-gray-900">
                                                    {item.fullName}
                                                </p>
                                                <p className="text-xs text-gray-500">
                                                    {item.email}
                                                </p>
                                            </div>
                                        </div>
                                    </td>
                                    <td className="px-5 py-3 text-gray-500">
                                        {item.studentCode}
                                    </td>
                                    <td className="px-5 py-3">
                                        {item.presentSessions} /{" "}
                                        {item.totalSessions}
                                    </td>
                                    <td className="px-5 py-3">
                                        {attendanceRate}%
                                    </td>
                                </tr>
                            );
                        })}
                    </tbody>
                </table>
                <div className="flex items-center justify-between border-t border-gray-100 px-5 py-3 text-sm text-gray-500">
                    <span>
                        Hiển thị{" "}
                        {students.length === 0 ? 0 : (page - 1) * pageSize + 1}-
                        {(page - 1) * pageSize + students.length} /{" "}
                        {studentPage?.totalCount ?? course.studentCount} sinh
                        viên
                    </span>
                    <div className="flex gap-1">
                        {Array.from(
                            { length: studentPage?.totalPages ?? 1 },
                            (_, index) => index + 1,
                        ).map((pageNumber) => (
                            <button
                                key={pageNumber}
                                type="button"
                                onClick={() => setPage(pageNumber)}
                                className={`size-8 rounded-md border text-sm ${pageNumber === page ? "bg-gray-100 text-gray-900" : "border-gray-200 text-gray-500"}`}
                            >
                                {pageNumber}
                            </button>
                        ))}
                    </div>
                </div>
            </section>

            <section className="overflow-hidden rounded-lg bg-white shadow-sm">
                <div className="flex items-center justify-between border-b border-gray-100 px-5 py-3">
                    <div>
                        <h2 className="text-sm font-semibold text-gray-900">
                            Lịch sử điểm danh gần đây
                        </h2>
                    </div>
                    <Link
                        to={attendanceHistoryPath}
                        className="rounded-md border border-gray-300 px-3 py-1.5 text-xs text-gray-700"
                    >
                        Xem tất cả
                    </Link>
                </div>
                <div className="divide-y divide-gray-100">
                    {attendanceHistoryQuery.isLoading && (
                        <div className="px-5 py-6 text-sm text-gray-500">
                            Đang tải lịch sử điểm danh...
                        </div>
                    )}
                    {!attendanceHistoryQuery.isLoading &&
                        recentAttendanceHistory.length === 0 && (
                            <div className="px-5 py-6 text-sm text-gray-500">
                                Không có lịch sử điểm danh cho lớp này.
                            </div>
                        )}
                    {recentAttendanceHistory.map((item) => (
                        <div
                            key={item.id}
                            className="grid gap-3 px-5 py-4 text-sm sm:grid-cols-[1fr_120px_120px_120px] sm:items-center"
                        >
                            <div>
                                <p className="font-medium text-gray-900">
                                    {item.date}
                                </p>
                                <p className="text-xs text-gray-500">
                                    {formatTimeToMinute(item.startTime)} -{" "}
                                    {formatTimeToMinute(item.endTime)}
                                </p>
                            </div>
                            <p className="text-gray-600">
                                {item.presentCount} có mặt
                            </p>
                            <p className="text-gray-600">
                                {item.absentCount} vắng mặt
                            </p>
                            <div className="flex items-center gap-2">
                                <span className="font-medium text-emerald-700">
                                    {Math.round(item.attendanceRate)}%
                                </span>
                                <span className="rounded-md bg-gray-100 px-2 py-1 text-xs text-gray-600">
                                    {item.status === 1 ? "Đang mở" : "Đã đóng"}
                                </span>
                            </div>
                        </div>
                    ))}
                </div>
            </section>

            {isStartConfirmOpen && (
                <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 px-4">
                    <div className="w-full max-w-md rounded-lg bg-white p-5 shadow-lg">
                        <h2 className="text-base font-semibold text-gray-900">
                            Tạo phiên điểm danh?
                        </h2>
                        <p className="mt-2 text-sm text-gray-500">
                            Hệ thống sẽ tạo phiên điểm danh mới cho lớp{" "}
                            {course.courseSectionCode} và chuyển sang màn hình
                            theo dõi điểm danh.
                        </p>
                        <div className="mt-5 flex justify-end gap-2">
                            <button
                                type="button"
                                onClick={() => setIsStartConfirmOpen(false)}
                                className="rounded-md border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700"
                                disabled={
                                    startAttendanceSessionMutation.isPending
                                }
                            >
                                Hủy
                            </button>
                            <button
                                type="button"
                                onClick={handleStartAttendanceSession}
                                className="rounded-md bg-emerald-600 px-4 py-2 text-sm font-medium text-white"
                                disabled={
                                    startAttendanceSessionMutation.isPending
                                }
                            >
                                {startAttendanceSessionMutation.isPending
                                    ? "Đang tạo..."
                                    : "Tạo phiên"}
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default LecturerCourseDetailPage;
