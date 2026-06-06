import { useMemo, useState } from "react";
import {
    FiBookOpen,
    FiCalendar,
    FiCheck,
    FiChevronRight,
    FiClock,
    FiEdit2,
    FiMapPin,
    FiPlus,
    FiX,
} from "react-icons/fi";
import { Link, useParams } from "react-router-dom";
import Spinner from "../../../shared/components/Spinner/Spinner";
import { getInitials } from "../../../shared/utils/getInitials";
import { availableStudents } from "../data/mockCourses";
import { useCourse } from "../hooks/course.query";
import {
    formatDayOfWeek,
    formatSemester,
    formatTimeToMinute,
} from "../types/course.types";

const CourseSectionDetailPage = () => {
    const { id = "" } = useParams();
    const { data: course, isError, isLoading } = useCourse(id);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [searchQuery, setSearchQuery] = useState("");
    const [selectedStudentIds, setSelectedStudentIds] = useState<string[]>([
        availableStudents[0].id,
        availableStudents[1].id,
    ]);

    const filteredStudents = useMemo(() => {
        const normalizedSearch = searchQuery.trim().toLowerCase();

        return availableStudents.filter(
            (student) =>
                !normalizedSearch ||
                student.fullName.toLowerCase().includes(normalizedSearch) ||
                student.code.toLowerCase().includes(normalizedSearch) ||
                student.major.toLowerCase().includes(normalizedSearch),
        );
    }, [searchQuery]);

    const toggleStudent = (studentId: string) => {
        setSelectedStudentIds((ids) =>
            ids.includes(studentId)
                ? ids.filter((id) => id !== studentId)
                : [...ids, studentId],
        );
    };

    if (isLoading) {
        return (
            <div className="flex min-h-[60vh] items-center justify-center">
                <Spinner className="size-10 text-blue-600" />
            </div>
        );
    }

    if (isError || !course) {
        return (
            <div className="p-6">
                <div className="rounded-2xl border border-dashed border-gray-200 bg-white p-8 text-center text-sm text-gray-500">
                    Không tìm thấy lớp học phần.
                </div>
            </div>
        );
    }

    return (
        <div className="p-6">
            <div className="mb-4 flex items-center gap-2 text-sm text-gray-500">
                <Link to="/courses" className="hover:text-blue-600">
                    Lớp học phần
                </Link>
                <FiChevronRight size={14} />
                <span className="font-medium text-gray-900">
                    {course.courseSectionCode}
                </span>
            </div>

            <div className="mb-4 flex items-center justify-between gap-4">
                <div className="flex min-w-0 items-center gap-3">
                    <div className="flex size-12 shrink-0 items-center justify-center rounded-xl bg-blue-50 text-blue-600">
                        <FiBookOpen size={22} />
                    </div>
                    <div className="min-w-0">
                        <h1 className="truncate text-xl font-bold text-gray-900">
                            {course.subjectName} - {course.courseSectionCode}
                        </h1>
                        <div className="mt-1 text-sm text-gray-500">
                            {formatSemester(course.semester)} -{" "}
                            {course.academicYear} - GV:{" "}
                            {course.lecturer.fullName} (
                            {course.lecturer.userCode})
                        </div>
                    </div>
                </div>

                <button
                    type="button"
                    className="flex items-center gap-2 rounded-xl border border-gray-200 bg-white px-4 py-2 text-sm font-semibold text-gray-700 hover:bg-gray-50"
                >
                    <FiEdit2 size={16} />
                    Sửa
                </button>
            </div>

            <div className="mb-4 grid gap-3 md:grid-cols-4">
                {[
                    ["Học phần", course.subjectName],
                    ["Tín chỉ", `${course.credits} TC`],
                    [
                        "Số lượng",
                        `${course.studentCount}/${course.maxCapacity ?? "-"}`,
                    ],
                    ["Trạng thái", course.isActive ? "Đang mở" : "Đã đóng"],
                ].map(([label, value]) => (
                    <div
                        key={label}
                        className="rounded-xl border border-gray-200 bg-white p-4 shadow-sm"
                    >
                        <div className="text-xs font-medium text-gray-500">
                            {label}
                        </div>
                        <div
                            className={`mt-1 text-sm font-semibold ${
                                label === "Trạng thái" && course.isActive
                                    ? "text-emerald-600"
                                    : "text-gray-900"
                            }`}
                        >
                            {value}
                        </div>
                    </div>
                ))}
            </div>

            <section className="mb-4 overflow-hidden rounded-2xl border border-gray-200 bg-white shadow-sm">
                <div className="flex items-center gap-2 border-b border-gray-200 px-5 py-3 text-sm font-semibold text-gray-900">
                    <FiCalendar size={16} className="text-blue-600" />
                    <span>Lịch học</span>
                </div>
                <div className="divide-y divide-gray-100">
                    {course.schedules.map((schedule) => (
                        <div
                            key={schedule.id}
                            className="grid items-center gap-3 px-5 py-3 text-sm md:grid-cols-[minmax(120px,1fr)_minmax(170px,1fr)_minmax(160px,1fr)]"
                        >
                            <div className="flex min-w-0 items-center gap-2">
                                <span className="flex size-8 shrink-0 items-center justify-center rounded-lg bg-blue-50 text-blue-600">
                                    <FiCalendar size={15} />
                                </span>
                                <div className="min-w-0">
                                    <div className="text-xs text-gray-500">
                                        Thứ
                                    </div>
                                    <div className="truncate font-semibold text-gray-900">
                                        {formatDayOfWeek(schedule.dayOfWeek)}
                                    </div>
                                </div>
                            </div>
                            <div className="flex min-w-0 items-center gap-2">
                                <span className="flex size-8 shrink-0 items-center justify-center rounded-lg bg-gray-100 text-gray-600">
                                    <FiClock size={15} />
                                </span>
                                <div className="min-w-0">
                                    <div className="text-xs text-gray-500">
                                        Thời gian
                                    </div>
                                    <div className="truncate font-medium text-gray-900">
                                        {formatTimeToMinute(schedule.startTime)}{" "}
                                        - {formatTimeToMinute(schedule.endTime)}
                                    </div>
                                </div>
                            </div>
                            <div className="flex min-w-0 items-center gap-2">
                                <span className="flex size-8 shrink-0 items-center justify-center rounded-lg bg-gray-100 text-gray-600">
                                    <FiMapPin size={15} />
                                </span>
                                <div className="min-w-0">
                                    <div className="text-xs text-gray-500">
                                        Phòng học
                                    </div>
                                    <div className="truncate font-medium text-gray-900">
                                        {schedule.room}
                                    </div>
                                </div>
                            </div>
                        </div>
                    ))}
                    {!course.schedules.length && (
                        <div className="px-5 py-4 text-sm text-gray-500">
                            Chưa có lịch học cho lớp học phần này.
                        </div>
                    )}
                </div>
            </section>

            <section className="overflow-hidden rounded-2xl border border-gray-200 bg-white shadow-sm">
                <div className="flex items-center justify-between gap-3 border-b border-gray-200 px-5 py-3">
                    <h2 className="text-sm font-semibold text-gray-900">
                        Danh sách sinh viên ({course.studentCount}/
                        {course.maxCapacity ?? "-"})
                    </h2>
                    <button
                        type="button"
                        onClick={() => setIsModalOpen(true)}
                        className="flex items-center gap-2 rounded-xl bg-blue-600 px-3 py-2 text-sm font-semibold text-white hover:bg-blue-700"
                    >
                        <FiPlus size={15} />
                        Thêm sinh viên
                    </button>
                </div>

                <div className="p-6 text-center text-sm text-gray-500">
                    Chưa có sinh viên nào đăng ký lớp học phần này.
                </div>
            </section>

            {isModalOpen && (
                <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
                    <div className="flex max-h-[80vh] w-full max-w-xl flex-col overflow-hidden rounded-2xl bg-white shadow-2xl">
                        <div className="flex items-center justify-between border-b border-gray-200 px-5 py-4">
                            <h2 className="text-base font-semibold text-gray-900">
                                Chọn sinh viên để thêm vào lớp học phần
                            </h2>
                            <button
                                type="button"
                                onClick={() => setIsModalOpen(false)}
                                className="flex size-8 items-center justify-center rounded-xl bg-gray-100 text-gray-500 hover:text-gray-700"
                                aria-label="Dong"
                            >
                                <FiX size={18} />
                            </button>
                        </div>

                        <div className="border-b border-gray-200 px-5 py-3">
                            <input
                                value={searchQuery}
                                onChange={(event) =>
                                    setSearchQuery(event.target.value)
                                }
                                placeholder="Tìm tên, mã SV, ngành..."
                                className="w-full rounded-xl border border-gray-200 bg-white px-3 py-2 text-sm outline-none focus:border-blue-500"
                            />
                        </div>

                        <div className="flex-1 overflow-y-auto">
                            {filteredStudents.map((student) => {
                                const isSelected = selectedStudentIds.includes(
                                    student.id,
                                );

                                return (
                                    <button
                                        key={student.id}
                                        type="button"
                                        onClick={() =>
                                            toggleStudent(student.id)
                                        }
                                        className="flex w-full items-center justify-between gap-4 border-b border-gray-200 px-5 py-3 text-left hover:bg-gray-50"
                                    >
                                        <div className="flex min-w-0 items-center gap-3">
                                            <div className="flex size-9 shrink-0 items-center justify-center rounded-full bg-blue-50 text-xs font-bold text-blue-600">
                                                {getInitials(student.fullName)}
                                            </div>
                                            <div className="min-w-0">
                                                <div className="truncate text-sm font-medium text-gray-900">
                                                    {student.fullName}
                                                </div>
                                                <div className="truncate text-xs text-gray-500">
                                                    {student.code} -{" "}
                                                    {student.major}
                                                </div>
                                            </div>
                                        </div>
                                        <span
                                            className={`flex size-5 shrink-0 items-center justify-center rounded-md border text-xs ${
                                                isSelected
                                                    ? "border-blue-600 bg-blue-600 text-white"
                                                    : "border-gray-300 bg-white"
                                            }`}
                                        >
                                            {isSelected ? (
                                                <FiCheck size={13} />
                                            ) : null}
                                        </span>
                                    </button>
                                );
                            })}
                        </div>

                        <div className="flex items-center justify-between gap-3 border-t border-gray-200 px-5 py-4">
                            <div className="text-sm text-gray-500">
                                Đã chọn {selectedStudentIds.length} sinh viên
                            </div>
                            <div className="flex gap-2">
                                <button
                                    type="button"
                                    onClick={() => setIsModalOpen(false)}
                                    className="rounded-xl border border-gray-200 px-4 py-2 text-sm font-semibold text-gray-600 hover:bg-gray-50"
                                >
                                    Hủy
                                </button>
                                <button
                                    type="button"
                                    className="rounded-xl bg-blue-600 px-4 py-2 text-sm font-semibold text-white hover:bg-blue-700"
                                >
                                    Thêm vào lớp học phần
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default CourseSectionDetailPage;
