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
    FiTrash2,
    FiX,
} from "react-icons/fi";
import { Link, useParams } from "react-router-dom";
import { ApiError, ValidationError } from "../../../shared/api/errors";
import Spinner from "../../../shared/components/Spinner/Spinner";
import { getInitials } from "../../../shared/utils/getInitials";
import {
    useEnrollCourseSection,
    useRemoveEnrollCourseSection,
} from "../hooks/course.mutation";
import {
    useCourse,
    useEnrolledStudents,
    useStudentLookup,
} from "../hooks/course.query";
import {
    formatDayOfWeek,
    formatSemester,
    formatTimeToMinute,
    type StudentLookupDto,
    type StudentSummary,
} from "../types/course.types";

const defaultStudentsPagedResult = {
    items: [],
    totalCount: 0,
    page: 1,
    pageSize: 20,
    totalPages: 0,
    hasNextPage: false,
    hasPreviousPage: false,
};

const getApiErrorMessage = (
    error: unknown,
    fallbackMessage: string,
    validationFallbackMessage: string,
) => {
    if (!error) return undefined;

    if (error instanceof ValidationError) {
        const messages = Object.values(error.errors).flat();
        return messages.length ? messages.join(" ") : validationFallbackMessage;
    }

    if (error instanceof ApiError || error instanceof Error) {
        return error.message;
    }

    return fallbackMessage;
};

const CourseSectionDetailPage = () => {
    const { id = "" } = useParams();
    const { data: course, isError, isLoading } = useCourse(id);
    const {
        data: enrolledStudents = defaultStudentsPagedResult,
        isError: isStudentsError,
        isLoading: isStudentsLoading,
    } = useEnrolledStudents(id, {
        page: 1,
        pageSize: 20,
    });
    const enrollCourseSection = useEnrollCourseSection(id);
    const removeEnrollCourseSection = useRemoveEnrollCourseSection(id);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [studentToRemove, setStudentToRemove] =
        useState<StudentSummary | null>(null);
    const [searchQuery, setSearchQuery] = useState("");
    const [selectedStudentIds, setSelectedStudentIds] = useState<string[]>([]);
    const [selectedStudents, setSelectedStudents] = useState<
        StudentLookupDto[]
    >([]);
    const trimmedSearchQuery = searchQuery.trim();
    const {
        data: studentLookup = [],
        isError: isStudentLookupError,
        isFetching: isStudentLookupLoading,
    } = useStudentLookup(trimmedSearchQuery);

    const enrolledStudentIds = useMemo(
        () => new Set(enrolledStudents.items.map((student) => student.userId)),
        [enrolledStudents.items],
    );

    const availableLookupStudents = useMemo(
        () =>
            studentLookup.filter(
                (student) =>
                    !selectedStudentIds.includes(student.userId) &&
                    !enrolledStudentIds.has(student.userId),
            ),
        [enrolledStudentIds, selectedStudentIds, studentLookup],
    );

    const enrollmentErrorMessage = useMemo(() => {
        return getApiErrorMessage(
            enrollCourseSection.error,
            "Không thể thêm sinh viên vào lớp học phần.",
            "Dữ liệu thêm sinh viên chưa hợp lệ.",
        );
    }, [enrollCourseSection.error]);

    const removeEnrollmentErrorMessage = useMemo(() => {
        return getApiErrorMessage(
            removeEnrollCourseSection.error,
            "Không thể xóa sinh viên khỏi lớp học phần.",
            "Dữ liệu xóa sinh viên chưa hợp lệ.",
        );
    }, [removeEnrollCourseSection.error]);

    const closeEnrollmentModal = () => {
        setIsModalOpen(false);
        enrollCourseSection.reset();
    };

    const removeSelectedStudent = (studentId: string) => {
        enrollCourseSection.reset();
        setSelectedStudentIds((ids) => ids.filter((id) => id !== studentId));
        setSelectedStudents((students) =>
            students.filter((student) => student.userId !== studentId),
        );
    };

    const selectStudent = (student: StudentLookupDto) => {
        enrollCourseSection.reset();
        setSelectedStudentIds((ids) =>
            ids.includes(student.userId) ? ids : [...ids, student.userId],
        );
        setSelectedStudents((students) =>
            students.some((item) => item.userId === student.userId)
                ? students
                : [...students, student],
        );
    };

    const submitSelectedStudents = () => {
        if (!selectedStudentIds.length) return;

        enrollCourseSection.mutate(
            { studentIds: selectedStudentIds },
            {
                onSuccess: () => {
                    setSelectedStudentIds([]);
                    setSelectedStudents([]);
                    setSearchQuery("");
                    setIsModalOpen(false);
                },
            },
        );
    };

    const openRemoveStudentModal = (student: StudentSummary) => {
        removeEnrollCourseSection.reset();
        setStudentToRemove(student);
    };

    const closeRemoveStudentModal = () => {
        removeEnrollCourseSection.reset();
        setStudentToRemove(null);
    };

    const submitRemoveStudent = () => {
        if (!studentToRemove) return;

        removeEnrollCourseSection.mutate(studentToRemove.userId, {
            onSuccess: () => {
                setStudentToRemove(null);
            },
        });
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

                <div className="p-6 text-sm text-gray-500">
                    {isStudentsLoading && (
                        <div className="flex justify-center">
                            <Spinner className="size-8 text-blue-600" />
                        </div>
                    )}

                    {isStudentsError && !isStudentsLoading && (
                        <div className="text-center">
                            Không thể tải danh sách sinh viên của lớp học phần.
                        </div>
                    )}

                    {!isStudentsLoading &&
                        !isStudentsError &&
                        !enrolledStudents.items.length && (
                            <div className="text-center">
                                Chưa có sinh viên nào đăng ký lớp học phần này.
                            </div>
                        )}

                    {!isStudentsLoading &&
                        !isStudentsError &&
                        enrolledStudents.items.length > 0 && (
                            <div className="overflow-hidden rounded-xl border border-gray-200 text-left">
                                <div className="grid grid-cols-[minmax(0,1.3fr)_minmax(110px,0.7fr)_minmax(0,1.2fr)_minmax(0,1fr)_48px] gap-3 border-b border-gray-200 bg-gray-50 px-4 py-3 text-xs font-semibold uppercase text-gray-500">
                                    <div>Sinh viên</div>
                                    <div>Mã SV</div>
                                    <div>Email</div>
                                    <div>Khoa</div>
                                    <div />
                                </div>
                                <div className="divide-y divide-gray-100">
                                    {enrolledStudents.items.map((student) => (
                                        <div
                                            key={student.userId}
                                            className="grid grid-cols-[minmax(0,1.3fr)_minmax(110px,0.7fr)_minmax(0,1.2fr)_minmax(0,1fr)_48px] items-center gap-3 px-4 py-3 text-sm"
                                        >
                                            <div className="flex min-w-0 items-center gap-3">
                                                <div className="flex size-9 shrink-0 items-center justify-center rounded-full bg-blue-50 text-xs font-bold text-blue-600">
                                                    {getInitials(
                                                        student.fullName,
                                                    )}
                                                </div>
                                                <div className="min-w-0">
                                                    <div className="truncate font-medium text-gray-900">
                                                        {student.fullName}
                                                    </div>
                                                </div>
                                            </div>
                                            <div className="truncate font-medium text-gray-700">
                                                {student.userCode}
                                            </div>
                                            <div className="truncate text-gray-500">
                                                {student.email}
                                            </div>
                                            <div className="truncate text-gray-500">
                                                {student.facultyName}
                                            </div>
                                            <button
                                                type="button"
                                                onClick={() =>
                                                    openRemoveStudentModal(
                                                        student,
                                                    )
                                                }
                                                className="flex size-8 items-center justify-center rounded-lg text-gray-400 hover:bg-red-50 hover:text-red-600"
                                                aria-label={`Xóa ${student.fullName} khỏi lớp`}
                                            >
                                                <FiTrash2 size={16} />
                                            </button>
                                        </div>
                                    ))}
                                </div>
                            </div>
                        )}
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
                                onClick={closeEnrollmentModal}
                                className="flex size-8 items-center justify-center rounded-xl bg-gray-100 text-gray-500 hover:text-gray-700"
                                aria-label="Dong"
                            >
                                <FiX size={18} />
                            </button>
                        </div>

                        <div className="space-y-3 border-b border-gray-200 px-5 py-3">
                            {selectedStudents.length > 0 && (
                                <div className="flex flex-wrap gap-2">
                                    {selectedStudents.map((student) => (
                                        <span
                                            key={student.userId}
                                            className="inline-flex max-w-full items-center gap-2 rounded-full bg-blue-50 px-3 py-1.5 text-xs font-medium text-blue-700"
                                        >
                                            <span className="truncate">
                                                {student.fullName} (
                                                {student.userCode})
                                            </span>
                                            <button
                                                type="button"
                                                onClick={() =>
                                                    removeSelectedStudent(
                                                        student.userId,
                                                    )
                                                }
                                                className="flex size-4 shrink-0 items-center justify-center rounded-full text-blue-500 hover:bg-blue-100 hover:text-blue-700"
                                                aria-label={`Xóa ${student.fullName} khỏi danh sách đã chọn`}
                                            >
                                                <FiX size={12} />
                                            </button>
                                        </span>
                                    ))}
                                </div>
                            )}
                            <input
                                value={searchQuery}
                                onChange={(event) =>
                                    setSearchQuery(event.target.value)
                                }
                                placeholder="Tìm tên, mã SV, email..."
                                className="w-full rounded-xl border border-gray-200 bg-white px-3 py-2 text-sm outline-none focus:border-blue-500"
                            />
                        </div>

                        <div className="flex-1 overflow-y-auto">
                            {trimmedSearchQuery.length < 3 && (
                                <div className="px-5 py-6 text-center text-sm text-gray-500">
                                    Nhập ít nhất 3 ký tự để tìm sinh viên.
                                </div>
                            )}

                            {trimmedSearchQuery.length >= 3 &&
                                isStudentLookupLoading && (
                                    <div className="flex justify-center px-5 py-6">
                                        <Spinner className="size-7 text-blue-600" />
                                    </div>
                                )}

                            {trimmedSearchQuery.length >= 3 &&
                                isStudentLookupError &&
                                !isStudentLookupLoading && (
                                    <div className="px-5 py-6 text-center text-sm text-gray-500">
                                        Không thể tìm sinh viên.
                                    </div>
                                )}

                            {trimmedSearchQuery.length >= 3 &&
                                !isStudentLookupLoading &&
                                !isStudentLookupError &&
                                !availableLookupStudents.length && (
                                <div className="px-5 py-6 text-center text-sm text-gray-500">
                                    Không có sinh viên phù hợp.
                                </div>
                            )}

                            {trimmedSearchQuery.length >= 3 &&
                                !isStudentLookupError &&
                                !isStudentLookupLoading &&
                                availableLookupStudents.map((student) => (
                                    <button
                                        key={student.userId}
                                        type="button"
                                        onClick={() =>
                                            selectStudent(student)
                                        }
                                        className="group flex w-full items-center justify-between gap-4 border-b border-gray-200 px-5 py-3 text-left hover:bg-gray-50"
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
                                                    {student.userCode} -{" "}
                                                    {student.email}
                                                </div>
                                            </div>
                                        </div>
                                        <span className="flex size-5 shrink-0 items-center justify-center rounded-md border border-gray-300 bg-white text-xs text-white group-hover:border-blue-600 group-hover:bg-blue-600">
                                            <FiCheck size={13} />
                                        </span>
                                    </button>
                                ))}
                        </div>

                        {enrollmentErrorMessage && (
                            <div className="border-t border-red-100 bg-red-50 px-5 py-3 text-sm text-red-700">
                                {enrollmentErrorMessage}
                            </div>
                        )}

                        <div className="flex items-center justify-between gap-3 border-t border-gray-200 px-5 py-4">
                            <div className="text-sm text-gray-500">
                                Đã chọn {selectedStudentIds.length} sinh viên
                            </div>
                            <div className="flex gap-2">
                                <button
                                    type="button"
                                    onClick={closeEnrollmentModal}
                                    className="rounded-xl border border-gray-200 px-4 py-2 text-sm font-semibold text-gray-600 hover:bg-gray-50"
                                >
                                    Hủy
                                </button>
                                <button
                                    type="button"
                                    onClick={submitSelectedStudents}
                                    disabled={
                                        !selectedStudentIds.length ||
                                        enrollCourseSection.isPending
                                    }
                                    className="rounded-xl bg-blue-600 px-4 py-2 text-sm font-semibold text-white hover:bg-blue-700 disabled:cursor-not-allowed disabled:bg-gray-300"
                                >
                                    {enrollCourseSection.isPending
                                        ? "Đang thêm..."
                                        : "Thêm vào lớp học phần"}
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            )}

            {studentToRemove && (
                <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
                    <div className="w-full max-w-md overflow-hidden rounded-2xl bg-white shadow-2xl">
                        <div className="flex items-center justify-between border-b border-gray-200 px-5 py-4">
                            <h2 className="text-base font-semibold text-gray-900">
                                Xóa sinh viên khỏi lớp
                            </h2>
                            <button
                                type="button"
                                onClick={closeRemoveStudentModal}
                                className="flex size-8 items-center justify-center rounded-xl bg-gray-100 text-gray-500 hover:text-gray-700"
                                aria-label="Đóng"
                            >
                                <FiX size={18} />
                            </button>
                        </div>

                        <div className="px-5 py-4 text-sm text-gray-600">
                            Bạn có muốn xóa{" "}
                            <span className="font-semibold text-gray-900">
                                {studentToRemove.fullName}
                            </span>{" "}
                            ({studentToRemove.userCode}) khỏi lớp học phần này
                            không?
                        </div>

                        {removeEnrollmentErrorMessage && (
                            <div className="border-t border-red-100 bg-red-50 px-5 py-3 text-sm text-red-700">
                                {removeEnrollmentErrorMessage}
                            </div>
                        )}

                        <div className="flex justify-end gap-2 border-t border-gray-200 px-5 py-4">
                            <button
                                type="button"
                                onClick={closeRemoveStudentModal}
                                className="rounded-xl border border-gray-200 px-4 py-2 text-sm font-semibold text-gray-600 hover:bg-gray-50"
                            >
                                Hủy
                            </button>
                            <button
                                type="button"
                                onClick={submitRemoveStudent}
                                disabled={removeEnrollCourseSection.isPending}
                                className="rounded-xl bg-red-600 px-4 py-2 text-sm font-semibold text-white hover:bg-red-700 disabled:cursor-not-allowed disabled:bg-gray-300"
                            >
                                {removeEnrollCourseSection.isPending
                                    ? "Đang xóa..."
                                    : "Xóa khỏi lớp"}
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default CourseSectionDetailPage;
