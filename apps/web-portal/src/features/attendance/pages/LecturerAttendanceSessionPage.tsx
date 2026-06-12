import { useState } from "react";
import { Link, useParams } from "react-router-dom";
import { useCourseDetail } from "../../courses/hooks/course.query";
import { formatTimeToMinute } from "../../courses/types/course.types";
import { getInitials } from "../../../shared/utils/getInitials";
import { toastEmitter } from "../../../shared/utils/toastEmitter";
import {
    openSessionRefetchIntervalMs,
    useAttendanceSessionDetail,
    useAttendanceSessionStudents,
} from "../hooks/attendance.query";
import { useCloseAttendanceSession } from "../hooks/attendance.mutation";

const studentPageSize = 10;

const formatDate = (value?: string) => {
    if (!value) return "-";

    const date = new Date(value);

    if (Number.isNaN(date.getTime())) {
        return value;
    }

    return date.toLocaleDateString("vi-VN");
};

const getAttendanceSessionStatusLabel = (status?: number) => {
    if (status === 1) return "Đang diễn ra";
    if (status === 2) return "Đã đóng";
    return "Không xác định";
};

const getStudentAttendanceStatus = (status?: number) => {
    if (status === 1) {
        return {
            label: "Đã điểm danh",
            className: "bg-green-100 text-green-700",
        };
    }

    if (status === 2) {
        return {
            label: "Vắng mặt",
            className: "bg-red-100 text-red-700",
        };
    }

    return {
        label: "Chưa điểm danh",
        className: "bg-gray-100 text-gray-600",
    };
};

const formatConfidence = (similarity?: number) => {
    if (similarity == null) return "--";

    return `${(similarity * 100).toFixed(1)}%`;
};

const formatCheckedInAt = (checkedInAt?: string) => {
    if (!checkedInAt) return "-";

    const date = new Date(checkedInAt);

    if (Number.isNaN(date.getTime())) {
        return formatTimeToMinute(checkedInAt);
    }

    return date.toLocaleTimeString("vi-VN", {
        hour: "2-digit",
        minute: "2-digit",
    });
};

const LecturerAttendanceSessionPage = () => {
    const { courseId, sessionId } = useParams();
    const [studentPage, setStudentPage] = useState(1);
    const [isCloseConfirmOpen, setIsCloseConfirmOpen] = useState(false);
    const hasSessionId = Boolean(sessionId);
    const courseDetailQuery = useCourseDetail(courseId);
    const attendanceSessionDetailQuery = useAttendanceSessionDetail(sessionId);
    const attendanceSession = attendanceSessionDetailQuery.data;
    const isSessionOpen = attendanceSession?.status === 1;
    const closeAttendanceSessionMutation = useCloseAttendanceSession();
    const attendanceSessionStudentsQuery = useAttendanceSessionStudents(
        courseId,
        sessionId,
        {
            page: studentPage,
            pageSize: studentPageSize,
        },
        isSessionOpen ? openSessionRefetchIntervalMs : false,
    );
    const course = courseDetailQuery.data;
    const isReviewMode = attendanceSession?.status === 2;
    const studentPageData = attendanceSessionStudentsQuery.data;
    const students = studentPageData?.items ?? [];
    const presentCount = attendanceSession?.presentCount ?? 0;
    const absentCount = attendanceSession?.absentCount ?? 0;
    const total = studentPageData?.totalCount ?? course?.studentCount ?? 0;
    const percentage = Math.round(attendanceSession?.attendanceRate ?? 0);
    const pendingOrAbsentLabel = isSessionOpen ? "Chưa điểm danh" : "Vắng mặt";
    const pageTitle = isReviewMode
        ? "Xem lại phiên điểm danh"
        : "Buổi điểm danh đang diễn ra";
    const statusText = isReviewMode
        ? `Trạng thái phiên: ${getAttendanceSessionStatusLabel(attendanceSession?.status)}.`
        : "Phiên điểm danh đang mở, sinh viên có thể check in.";
    const statusClass = isReviewMode
        ? "border-blue-200 bg-blue-50 text-blue-800"
        : "border-emerald-200 bg-emerald-50 text-emerald-800";
    const statusDotClass = isReviewMode ? "bg-blue-600" : "bg-emerald-600";
    const schedule = course?.schedules?.[0];
    const sessionDate = attendanceSession?.date;
    const sessionStartTime =
        attendanceSession?.startTime ?? schedule?.startTime;
    const sessionEndTime = attendanceSession?.endTime;
    const displayEndTime =
        sessionEndTime ?? (isReviewMode ? schedule?.endTime : undefined);

    if (courseDetailQuery.isLoading || attendanceSessionDetailQuery.isLoading) {
        return (
            <div className="rounded-lg bg-white p-5 text-sm text-gray-500 shadow-sm">
                Đang tải thông tin phiên điểm danh...
            </div>
        );
    }

    const handleCloseAttendanceSession = () => {
        if (!sessionId || !isSessionOpen) return;

        closeAttendanceSessionMutation.mutate(sessionId, {
            onSuccess: () => {
                setIsCloseConfirmOpen(false);
                toastEmitter.success("Đã đóng phiên điểm danh.");
            },
            onError: () => {
                toastEmitter.error(
                    "Không thể đóng phiên điểm danh. Vui lòng thử lại.",
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
                <Link
                    to={`/lecturer/courses/${courseId}`}
                    className="hover:text-gray-900"
                >
                    {course?.subjectName ?? "Chi tiet lop hoc"}
                </Link>
                {hasSessionId && (
                    <>
                        <span className="mx-2">/</span>
                        <Link
                            to={`/lecturer/courses/${courseId}/attendance-history`}
                            className="hover:text-gray-900"
                        >
                            Lịch sử điểm danh
                        </Link>
                    </>
                )}
                <span className="mx-2">/</span>
                <span className="text-gray-900">
                    {isReviewMode ? "Xem lại" : "Điểm danh"}
                </span>
            </div>

            <div>
                <h1 className="text-xl font-semibold text-gray-900">
                    {pageTitle}
                </h1>
                <p className="mt-1 text-sm text-gray-500">
                    {course?.subjectName ?? "Dang tai lop hoc"} -{" "}
                    {course?.courseSectionCode ?? "-"} -{" "}
                    {formatDate(sessionDate)} -{" "}
                    {formatTimeToMinute(sessionStartTime)} -{" "}
                    {displayEndTime
                        ? formatTimeToMinute(displayEndTime)
                        : "Đang mở"}
                </p>
            </div>

            {attendanceSessionDetailQuery.isError && (
                <div className="rounded-lg border border-red-100 bg-red-50 px-4 py-3 text-sm text-red-700">
                    Khong the tai thong tin phien diem danh. Vui long thu lai.
                </div>
            )}

            <div className="grid gap-3 sm:grid-cols-2 xl:grid-cols-4">
                {[
                    [
                        "Đã điểm danh",
                        presentCount,
                        `/ ${total} sinh viên`,
                        "text-emerald-700",
                    ],
                    [
                        pendingOrAbsentLabel,
                        absentCount,
                        "chưa điểm danh",
                        "text-gray-900",
                    ],
                    [
                        "Tỷ lệ",
                        `${percentage}%`,
                        "đã điểm danh",
                        "text-gray-900",
                    ],
                    [
                        "Trạng thái",
                        getAttendanceSessionStatusLabel(
                            attendanceSession?.status ?? 1,
                        ),
                        isReviewMode
                            ? "phiên điểm danh đã đóng"
                            : "phiên điểm danh đang mở",
                        "text-gray-900",
                    ],
                ].map(([label, value, sub, color]) => (
                    <div
                        key={label}
                        className="rounded-lg bg-white p-4 shadow-sm"
                    >
                        <p className="text-xs text-gray-500">{label}</p>
                        <p className={`mt-1 text-2xl font-semibold ${color}`}>
                            {value}
                        </p>
                        <p className="text-xs text-gray-500">{sub}</p>
                    </div>
                ))}
            </div>

            <div className="grid gap-4 xl:grid-cols-[340px_1fr]">
                <div className="space-y-4">
                    <section className="rounded-lg bg-white p-5 shadow-sm">
                        <div
                            className={`rounded-md border px-4 py-3 text-sm ${statusClass}`}
                        >
                            <span
                                className={`mr-2 inline-block size-2 rounded-full ${statusDotClass}`}
                            />
                            {statusText}
                        </div>
                        <h2 className="mt-5 text-sm font-semibold text-gray-900">
                            Tiến độ điểm danh
                        </h2>
                        <p className="mt-3 text-center text-sm text-gray-500">
                            <span className="text-2xl font-semibold text-emerald-700">
                                {presentCount}
                            </span>{" "}
                            / {total} sinh viên
                        </p>
                        <div className="mt-3 h-2 rounded-full bg-gray-100">
                            <div
                                className="h-2 rounded-full bg-emerald-600"
                                style={{ width: `${percentage}%` }}
                            />
                        </div>
                        <div className="mt-5 divide-y divide-gray-100 text-sm">
                            {[
                                [
                                    "Bắt đầu lúc",
                                    formatTimeToMinute(sessionStartTime),
                                ],
                                ...(displayEndTime
                                    ? [
                                          [
                                              "Kết thúc lúc",
                                              formatTimeToMinute(
                                                  displayEndTime,
                                              ),
                                          ],
                                      ]
                                    : []),
                                ["Ngày", formatDate(sessionDate)],
                                ...(hasSessionId
                                    ? [
                                          [
                                              "Trạng thái",
                                              getAttendanceSessionStatusLabel(
                                                  attendanceSession?.status,
                                              ),
                                          ],
                                          ["Mã phiên", sessionId ?? "-"],
                                      ]
                                    : []),
                            ].map(([label, value]) => (
                                <div
                                    key={label}
                                    className="flex justify-between py-2"
                                >
                                    <span className="text-gray-500">
                                        {label}
                                    </span>
                                    <span className="font-medium text-gray-900">
                                        {value}
                                    </span>
                                </div>
                            ))}
                        </div>
                        <button
                            type="button"
                            onClick={() => setIsCloseConfirmOpen(true)}
                            className={`mt-5 w-full rounded-md border px-4 py-2 text-sm font-medium ${
                                isReviewMode
                                    ? "border-gray-200 bg-gray-50 text-gray-600"
                                    : "border-red-200 bg-red-50 text-red-700"
                            }`}
                            disabled={
                                !isSessionOpen ||
                                closeAttendanceSessionMutation.isPending
                            }
                        >
                            {closeAttendanceSessionMutation.isPending
                                ? "Đang đóng phiên..."
                                : isReviewMode
                                  ? "Phiên đã đóng"
                                  : "Kết thúc điểm danh"}
                        </button>
                    </section>
                </div>

                <section className="overflow-hidden rounded-lg bg-white shadow-sm">
                    <div className="flex flex-col gap-3 border-b border-gray-100 px-5 py-3 sm:flex-row sm:items-center sm:justify-between">
                        <h2 className="text-sm font-semibold text-gray-900">
                            {isReviewMode
                                ? "Danh sách điểm danh"
                                : "Danh sách điểm danh real-time"}
                        </h2>
                        {/* <div className="flex gap-2">
                            <select className="rounded-md border border-gray-300 px-2 py-1.5 text-xs">
                                <option>Tất cả</option>
                                <option>Đã điểm danh</option>
                                <option>Chưa điểm danh</option>
                            </select>
                            <input
                                className="w-40 rounded-md border border-gray-300 px-2 py-1.5 text-xs"
                                placeholder="Tìm sinh viên..."
                            />
                        </div> */}
                    </div>
                    <table className="w-full text-left text-sm">
                        <thead className="bg-gray-50 text-xs text-gray-500">
                            <tr>
                                <th className="px-5 py-3 font-medium">
                                    Sinh viên
                                </th>
                                <th className="px-5 py-3 font-medium">MSSV</th>
                                <th className="px-5 py-3 font-medium">
                                    Trạng thái
                                </th>
                                <th className="px-5 py-3 font-medium">
                                    Độ tương đồng
                                </th>
                                <th className="px-5 py-3 font-medium">
                                    Thời gian check-in
                                </th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-gray-100">
                            {attendanceSessionStudentsQuery.isLoading && (
                                <tr>
                                    <td
                                        className="px-5 py-8 text-center text-gray-500"
                                        colSpan={5}
                                    >
                                        Đang tải danh sách sinh viên...
                                    </td>
                                </tr>
                            )}
                            {attendanceSessionStudentsQuery.isError && (
                                <tr>
                                    <td
                                        className="px-5 py-8 text-center text-red-600"
                                        colSpan={5}
                                    >
                                        Không thể tải danh sách sinh viên.
                                    </td>
                                </tr>
                            )}
                            {!attendanceSessionStudentsQuery.isLoading &&
                                !attendanceSessionStudentsQuery.isError &&
                                students.length === 0 && (
                                    <tr>
                                        <td
                                            className="px-5 py-8 text-center text-gray-500"
                                            colSpan={5}
                                        >
                                            Chưa có sinh viên trong phiên điểm
                                            danh này.
                                        </td>
                                    </tr>
                                )}
                            {students.map((item) => {
                                const status = getStudentAttendanceStatus(
                                    item.attendanceStatus,
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
                                            <span
                                                className={`rounded-md px-2 py-1 text-xs ${status.className}`}
                                            >
                                                {status.label}
                                            </span>
                                        </td>
                                        <td className="px-5 py-3 text-emerald-700">
                                            {formatConfidence(item.confidence)}
                                        </td>
                                        <td className="px-5 py-3 text-gray-500">
                                            {formatCheckedInAt(
                                                item.checkedInAt,
                                            )}
                                        </td>
                                    </tr>
                                );
                            })}
                        </tbody>
                    </table>
                    <div className="flex items-center justify-between border-t border-gray-100 px-5 py-3 text-sm text-gray-500">
                        <span>
                            Hiển thị{" "}
                            {students.length === 0
                                ? 0
                                : (studentPage - 1) * studentPageSize + 1}
                            -
                            {(studentPage - 1) * studentPageSize +
                                students.length}{" "}
                            / {studentPageData?.totalCount ?? total} sinh viên
                            {!isReviewMode && " - Tự động cập nhật mỗi 5 giây"}
                        </span>
                        <div className="flex gap-1">
                            {Array.from(
                                { length: studentPageData?.totalPages ?? 1 },
                                (_, index) => index + 1,
                            ).map((pageNumber) => (
                                <button
                                    key={pageNumber}
                                    type="button"
                                    onClick={() => setStudentPage(pageNumber)}
                                    className={`size-8 rounded-md border text-sm ${
                                        pageNumber === studentPage
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

            {isCloseConfirmOpen && (
                <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 px-4">
                    <div className="w-full max-w-md rounded-lg bg-white p-5 shadow-lg">
                        <h2 className="text-base font-semibold text-gray-900">
                            Đóng phiên điểm danh?
                        </h2>
                        <p className="mt-2 text-sm text-gray-500">
                            Sinh viên sẽ không thể check in sau khi phiên điểm
                            danh được đóng.
                        </p>
                        <div className="mt-5 flex justify-end gap-2">
                            <button
                                type="button"
                                onClick={() => setIsCloseConfirmOpen(false)}
                                className="rounded-md border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700"
                                disabled={
                                    closeAttendanceSessionMutation.isPending
                                }
                            >
                                Hủy
                            </button>
                            <button
                                type="button"
                                onClick={handleCloseAttendanceSession}
                                className="rounded-md border border-red-200 bg-red-50 px-4 py-2 text-sm font-medium text-red-700"
                                disabled={
                                    closeAttendanceSessionMutation.isPending
                                }
                            >
                                {closeAttendanceSessionMutation.isPending
                                    ? "Đang đóng..."
                                    : "Đóng phiên"}
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default LecturerAttendanceSessionPage;
