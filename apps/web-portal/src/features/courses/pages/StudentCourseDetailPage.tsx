import { Link, useParams, useSearchParams } from "react-router-dom";
import {
    useCourseDetail,
    useStudentAttendanceRecords,
} from "../hooks/course.query";
import {
    formatDayOfWeek,
    formatSemester,
    formatTimeToMinute,
} from "../types/course.types";

const formatDate = (date?: string | null) => {
    if (!date) return "--";

    const parsedDate = new Date(date);
    if (Number.isNaN(parsedDate.getTime())) return date;

    return new Intl.DateTimeFormat("vi-VN", {
        day: "2-digit",
        month: "2-digit",
        year: "numeric",
    }).format(parsedDate);
};

const getAttendanceStatus = (status: number) => {
    if (status === 1) {
        return {
            label: "Co mat",
            className: "bg-emerald-100 text-emerald-700",
        };
    }

    if (status === 3) {
        return {
            label: "Di tre",
            className: "bg-amber-100 text-amber-700",
        };
    }

    return {
        label: "Vang",
        className: "bg-red-100 text-red-700",
    };
};

const formatConfidence = (similarity?: number) => {
    if (similarity == null) return "--";

    return `${(similarity * 100).toFixed(1)}%`;
};

const formatCheckInTime = (time?: string | null) => {
    if (!time) return "--";
    if (/^\d{2}:\d{2}/.test(time)) return time.slice(0, 5);

    const parsedDate = new Date(time);
    if (Number.isNaN(parsedDate.getTime())) return time;

    return new Intl.DateTimeFormat("vi-VN", {
        hour: "2-digit",
        minute: "2-digit",
    }).format(parsedDate);
};

const StudentCourseDetailPage = () => {
    const { courseId } = useParams();
    const [searchParams] = useSearchParams();
    const sessionId = searchParams.get("sessionId");
    const courseDetailQuery = useCourseDetail(courseId);
    const attendanceRecordsQuery = useStudentAttendanceRecords(courseId);
    const course = courseDetailQuery.data;

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

    const semesterLabel = `${formatSemester(course.semester)} ${course.academicYear}`;
    const checkInPath =
        courseId && sessionId
            ? `/student/checkin?courseId=${courseId}&sessionId=${sessionId}`
            : "";

    return (
        <div className="space-y-4 sm:space-y-5">
            <div className="flex flex-wrap items-center gap-y-1 text-sm text-gray-500">
                <Link to="/student" className="hover:text-gray-900">
                    Lớp học của tôi
                </Link>
                <span className="mx-2">/</span>
                <span className="line-clamp-1 min-w-0 text-gray-900">
                    {course.subjectName}
                </span>
            </div>

            <div className="flex flex-col gap-3 lg:flex-row lg:items-start lg:justify-between">
                <div className="min-w-0">
                    <h1 className="text-lg font-semibold leading-6 text-gray-900 sm:text-xl">
                        {course.subjectName}
                    </h1>
                    <p className="mt-1 text-sm text-gray-500">
                        {course.courseSectionCode} - {semesterLabel}
                    </p>
                </div>
                {sessionId ? (
                    <Link
                        to={checkInPath}
                        className="inline-flex min-h-11 shrink-0 items-center justify-center rounded-md bg-emerald-600 px-4 py-2 text-sm font-semibold text-white transition hover:bg-emerald-700 sm:w-auto"
                    >
                        Điểm danh ngay
                    </Link>
                ) : (
                    <button
                        type="button"
                        disabled
                        className="inline-flex min-h-11 shrink-0 cursor-not-allowed items-center justify-center rounded-md bg-gray-200 px-4 py-2 text-sm font-semibold text-gray-500 sm:w-auto"
                    >
                        Chưa đến phiên điểm danh
                    </button>
                )}
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
                            ["Tín chỉ", `${course.credits} tín chỉ`],
                        ].map(([label, value]) => (
                            <div
                                key={label}
                                className="flex flex-col gap-1 py-2 sm:flex-row sm:items-center sm:justify-between"
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
                    {course.schedules.length > 0 ? (
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
                    ) : (
                        <p className="mt-3 text-sm text-gray-500">
                            Chua co lich hoc.
                        </p>
                    )}
                </section>
            </div>

            <section className="overflow-hidden rounded-lg bg-white shadow-sm">
                <div className="border-b border-gray-100 px-5 py-3">
                    <h2 className="text-sm font-semibold text-gray-900">
                        Lịch sử điểm danh
                    </h2>
                </div>

                {attendanceRecordsQuery.isLoading && (
                    <div className="px-5 py-5 text-sm text-gray-500">
                        Dang tai lich su diem danh...
                    </div>
                )}

                {!attendanceRecordsQuery.isLoading &&
                    (attendanceRecordsQuery.data?.length ?? 0) === 0 && (
                        <div className="px-5 py-5 text-sm text-gray-500">
                            Chưa có lich sử điểm danh nào.
                        </div>
                    )}

                {!attendanceRecordsQuery.isLoading &&
                    (attendanceRecordsQuery.data?.length ?? 0) > 0 && (
                        <div className="divide-y divide-gray-100">
                            {attendanceRecordsQuery.data?.map((record) => {
                                const status = getAttendanceStatus(
                                    record.status,
                                );

                                return (
                                    <div
                                        key={record.attendanceSessionId}
                                        className="grid gap-3 px-5 py-4 text-sm md:grid-cols-[1.2fr_1fr_1fr_auto]"
                                    >
                                        <div>
                                            <p className="font-medium text-gray-900">
                                                {formatDate(record.date)}
                                            </p>
                                            <p className="mt-1 text-xs text-gray-500">
                                                Bắt đầu{" "}
                                                {formatTimeToMinute(
                                                    record.startTime,
                                                )}
                                            </p>
                                        </div>
                                        <div>
                                            <p className="text-xs text-gray-500">
                                                Giờ điểm danh
                                            </p>
                                            <p className="mt-1 font-medium text-gray-900">
                                                {formatCheckInTime(
                                                    record.checkedInAt,
                                                )}
                                            </p>
                                        </div>
                                        <div>
                                            <p className="text-xs text-gray-500">
                                                Độ tương đồng
                                            </p>
                                            <p className="mt-1 font-medium text-gray-900">
                                                {formatConfidence(
                                                    record.confidenceScore,
                                                )}
                                            </p>
                                        </div>
                                        <div className="md:text-right">
                                            <span
                                                className={`inline-flex rounded-md px-2 py-1 text-xs font-medium ${status.className}`}
                                            >
                                                {status.label}
                                            </span>
                                        </div>
                                    </div>
                                );
                            })}
                        </div>
                    )}
            </section>
        </div>
    );
};

export default StudentCourseDetailPage;
