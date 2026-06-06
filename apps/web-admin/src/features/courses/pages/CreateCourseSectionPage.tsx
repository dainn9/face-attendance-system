import { useState, type FocusEvent, type FormEvent } from "react";
import {
    FiChevronDown,
    FiChevronRight,
    FiPlus,
    FiTrash2,
} from "react-icons/fi";
import { Link } from "react-router-dom";
import {
    useLecturerLookup,
    useSubjectLookup,
} from "../../users/hooks/lookup.query";
import { ValidationError } from "../../../shared/api/errors";
import { useCreateCourseSection } from "../hooks/course.mutation";
import {
    Semester,
    type CreateCourseSectionRequest,
} from "../types/course.types";

type ScheduleForm = {
    id: number;
    dayOfWeek: number;
    startTime: string;
    endTime: string;
    room: string;
};

const dayOptions = [
    { label: "Thứ 2", value: 1 },
    { label: "Thứ 3", value: 2 },
    { label: "Thứ 4", value: 3 },
    { label: "Thứ 5", value: 4 },
    { label: "Thứ 6", value: 5 },
    { label: "Thứ 7", value: 6 },
];

const formatTimeForPayload = (time: string) =>
    time.length === 5 ? `${time}:00` : time;

const formatAcademicYear = (startYear: string) => {
    const normalizedStartYear = startYear.trim();
    const parsedStartYear = Number(normalizedStartYear);

    if (!/^\d{4}$/.test(normalizedStartYear) || Number.isNaN(parsedStartYear)) {
        return "";
    }

    return `${normalizedStartYear}-${parsedStartYear + 1}`;
};

const getScheduleConflictIndexes = (schedules: ScheduleForm[]) => {
    const conflictIndexes = new Set<number>();

    schedules.forEach((schedule, index) => {
        schedules.forEach((otherSchedule, otherIndex) => {
            if (otherIndex <= index) {
                return;
            }

            if (
                schedule.dayOfWeek !== otherSchedule.dayOfWeek ||
                !schedule.startTime ||
                !schedule.endTime ||
                !otherSchedule.startTime ||
                !otherSchedule.endTime
            ) {
                return;
            }

            const hasOverlap =
                schedule.startTime < otherSchedule.endTime &&
                otherSchedule.startTime < schedule.endTime;

            if (hasOverlap) {
                conflictIndexes.add(index);
                conflictIndexes.add(otherIndex);
            }
        });
    });

    return conflictIndexes;
};

const CreateCourseSectionPage = () => {
    const [subjectId, setSubjectId] = useState("");
    const [selectedSubjectName, setSelectedSubjectName] = useState("");
    const [selectedSubjectFacultyId, setSelectedSubjectFacultyId] =
        useState("");
    const [courseSectionCode, setCourseSectionCode] = useState("WEB301-01");
    const [semester, setSemester] = useState<Semester>(Semester.HK1);
    const [academicYearStart, setAcademicYearStart] = useState("2024");
    const [maxCapacity, setMaxCapacity] = useState(40);
    const [subjectKeyword, setSubjectKeyword] = useState("");
    const [isSubjectDropdownOpen, setIsSubjectDropdownOpen] = useState(false);
    const { data: subjects = [], isFetching: isLoadingSubjects } =
        useSubjectLookup(subjectKeyword.trim() || undefined);
    const [lecturerId, setLecturerId] = useState("");
    const [selectedLecturerName, setSelectedLecturerName] = useState("");
    const [lecturerKeyword, setLecturerKeyword] = useState("");
    const [isLecturerDropdownOpen, setIsLecturerDropdownOpen] = useState(false);
    const { data: lecturers = [], isFetching: isLoadingLecturers } =
        useLecturerLookup(
            lecturerKeyword.trim() || undefined,
            selectedSubjectFacultyId || undefined,
        );
    const {
        mutate: createCourseSection,
        error,
        isPending,
    } = useCreateCourseSection();
    const [schedules, setSchedules] = useState<ScheduleForm[]>([
        {
            id: 1,
            dayOfWeek: 2,
            startTime: "07:00",
            endTime: "09:30",
            room: "A101",
        },
        {
            id: 2,
            dayOfWeek: 4,
            startTime: "13:00",
            endTime: "15:30",
            room: "B203",
        },
    ]);

    const updateSchedule = (
        id: number,
        field: keyof Omit<ScheduleForm, "id">,
        value: ScheduleForm[keyof Omit<ScheduleForm, "id">],
    ) => {
        setSchedules((items) =>
            items.map((item) =>
                item.id === id ? { ...item, [field]: value } : item,
            ),
        );
    };

    const addSchedule = () => {
        setSchedules((items) => [
            ...items,
            {
                id: Date.now(),
                dayOfWeek: 1,
                startTime: "07:00",
                endTime: "09:30",
                room: "",
            },
        ]);
    };

    const removeSchedule = (id: number) => {
        setSchedules((items) =>
            items.length > 1 ? items.filter((item) => item.id !== id) : items,
        );
    };

    const closeSubjectDropdown = (event: FocusEvent<HTMLDivElement>) => {
        if (!event.currentTarget.contains(event.relatedTarget)) {
            setIsSubjectDropdownOpen(false);
        }
    };

    const closeLecturerDropdown = (event: FocusEvent<HTMLDivElement>) => {
        if (!event.currentTarget.contains(event.relatedTarget)) {
            setIsLecturerDropdownOpen(false);
        }
    };

    const getValidationError = (field: string) =>
        error instanceof ValidationError ? error.get(field) : undefined;

    const getScheduleValidationError = (
        index: number,
        field: "dayOfWeek" | "startTime" | "endTime" | "room",
    ) =>
        getValidationError(`schedules[${index}].${field}`) ??
        getValidationError(
            `schedules[${index}].${field.charAt(0).toUpperCase()}${field.slice(1)}`,
        );

    const scheduleConflictIndexes = getScheduleConflictIndexes(schedules);
    const academicYear = formatAcademicYear(academicYearStart);

    const isFormComplete =
        Boolean(subjectId) &&
        Boolean(courseSectionCode.trim()) &&
        Boolean(semester) &&
        Boolean(academicYear) &&
        Boolean(lecturerId) &&
        maxCapacity > 0 &&
        schedules.length > 0 &&
        schedules.every(
            (schedule) =>
                Boolean(schedule.dayOfWeek) &&
                Boolean(schedule.startTime) &&
                Boolean(schedule.endTime) &&
                Boolean(schedule.room.trim()),
        ) &&
        scheduleConflictIndexes.size === 0;

    const handleSubmit = (event: FormEvent) => {
        event.preventDefault();

        if (!isFormComplete) {
            return;
        }

        const payload: CreateCourseSectionRequest = {
            subjectId,
            courseSectionCode: courseSectionCode.trim(),
            semester,
            academicYear,
            lecturerId,
            maxCapacity,
            schedules: schedules.map((schedule) => ({
                dayOfWeek: schedule.dayOfWeek,
                startTime: formatTimeForPayload(schedule.startTime),
                endTime: formatTimeForPayload(schedule.endTime),
                room: schedule.room.trim(),
            })),
        };

        createCourseSection(payload);
    };

    return (
        <div className="p-6">
            <div className="mb-4 flex items-center gap-2 text-sm text-gray-500">
                <Link to="/courses" className="hover:text-blue-600">
                    Lớp học
                </Link>
                <FiChevronRight size={14} />
                <span className="font-medium text-gray-900">
                    Tạo lớp học phần
                </span>
            </div>

            <form onSubmit={handleSubmit} className="space-y-4">
                {error && !(error instanceof ValidationError) && (
                    <div className="rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-600">
                        {error.message}
                    </div>
                )}

                <section className="rounded-2xl border border-gray-200 bg-white p-5 shadow-sm">
                    <div className="mb-4 border-b border-gray-200 pb-2 text-sm font-semibold text-gray-900">
                        Học phần
                    </div>

                    <div className="grid gap-4 md:grid-cols-2">
                        <div
                            className="relative block text-sm font-medium text-gray-600"
                            onBlur={closeSubjectDropdown}
                        >
                            Học phần <span className="text-red-500">*</span>
                            <input
                                name="subjectId"
                                type="hidden"
                                value={subjectId}
                                readOnly
                            />
                            <button
                                type="button"
                                onClick={() =>
                                    setIsSubjectDropdownOpen(
                                        (isOpen) => !isOpen,
                                    )
                                }
                                className="mt-1 flex w-full items-center justify-between gap-2 rounded-xl border border-gray-200 bg-white px-3 py-2 text-left text-sm text-gray-700 outline-none focus:border-blue-500"
                            >
                                <span
                                    className={
                                        selectedSubjectName
                                            ? "truncate"
                                            : "truncate text-gray-400"
                                    }
                                >
                                    {selectedSubjectName || "Chọn học phần"}
                                </span>
                                <FiChevronDown size={16} />
                            </button>
                            {isSubjectDropdownOpen && (
                                <div className="absolute left-0 right-0 z-20 mt-2 overflow-hidden rounded-xl border border-gray-200 bg-white shadow-lg">
                                    <div className="border-b border-gray-100 p-2">
                                        <input
                                            autoFocus
                                            value={subjectKeyword}
                                            onChange={(event) => {
                                                setSubjectKeyword(
                                                    event.target.value,
                                                );
                                                setSubjectId("");
                                                setSelectedSubjectName("");
                                                setSelectedSubjectFacultyId("");
                                                setLecturerId("");
                                                setSelectedLecturerName("");
                                                setLecturerKeyword("");
                                            }}
                                            placeholder="Tìm học phần..."
                                            className="w-full rounded-lg border border-gray-200 bg-white px-3 py-2 text-sm text-gray-700 outline-none focus:border-blue-500"
                                        />
                                    </div>

                                    <div className="max-h-56 overflow-y-auto py-1">
                                        {isLoadingSubjects && (
                                            <div className="px-3 py-2 text-sm text-gray-500">
                                                Đang tải học phần...
                                            </div>
                                        )}

                                        {!isLoadingSubjects &&
                                            subjects.map((subject) => (
                                                <button
                                                    key={subject.id}
                                                    type="button"
                                                    onMouseDown={(event) => {
                                                        event.preventDefault();
                                                        setSubjectId(
                                                            subject.id,
                                                        );
                                                        setSelectedSubjectName(
                                                            subject.name,
                                                        );
                                                        setSelectedSubjectFacultyId(
                                                            subject.facultyId ??
                                                                "",
                                                        );
                                                        setLecturerId("");
                                                        setSelectedLecturerName(
                                                            "",
                                                        );
                                                        setLecturerKeyword("");
                                                        setSubjectKeyword("");
                                                        setIsSubjectDropdownOpen(
                                                            false,
                                                        );
                                                    }}
                                                    className="block w-full px-3 py-2 text-left text-sm text-gray-700 hover:bg-blue-50 hover:text-blue-700"
                                                >
                                                    {subject.name}
                                                </button>
                                            ))}

                                        {!isLoadingSubjects &&
                                            !subjects.length && (
                                                <div className="px-3 py-2 text-sm text-gray-500">
                                                    Không tìm thấy học phần.
                                                </div>
                                            )}
                                    </div>
                                </div>
                            )}
                            {getValidationError("subjectId") && (
                                <p className="mt-1 text-xs text-red-500">
                                    {getValidationError("subjectId")}
                                </p>
                            )}
                        </div>

                        <label className="block text-sm font-medium text-gray-600">
                            Mã lớp học phần{" "}
                            <span className="text-red-500">*</span>
                            <input
                                value={courseSectionCode}
                                onChange={(event) =>
                                    setCourseSectionCode(event.target.value)
                                }
                                placeholder="VD: WEB301-01"
                                className="mt-1 w-full rounded-xl border border-gray-200 bg-white px-3 py-2 text-sm text-gray-700 outline-none focus:border-blue-500"
                            />
                            {getValidationError("courseSectionCode") && (
                                <p className="mt-1 text-xs text-red-500">
                                    {getValidationError("courseSectionCode")}
                                </p>
                            )}
                        </label>
                    </div>

                    <div className="mt-4 grid gap-4 md:grid-cols-2">
                        <label className="block text-sm font-medium text-gray-600">
                            Học kỳ <span className="text-red-500">*</span>
                            <select
                                value={semester}
                                onChange={(event) =>
                                    setSemester(
                                        Number(event.target.value) as Semester,
                                    )
                                }
                                className="mt-1 w-full rounded-xl border border-gray-200 bg-white px-3 py-2 text-sm text-gray-700 outline-none focus:border-blue-500"
                            >
                                <option value={Semester.HK1}>HK1</option>
                                <option value={Semester.HK2}>HK2</option>
                                <option value={Semester.HK3}>HK3</option>
                            </select>
                            {getValidationError("semester") && (
                                <p className="mt-1 text-xs text-red-500">
                                    {getValidationError("semester")}
                                </p>
                            )}
                        </label>

                        <label className="block text-sm font-medium text-gray-600">
                            Năm bắt đầu <span className="text-red-500">*</span>
                            <input
                                type="number"
                                min={2000}
                                max={2100}
                                value={academicYearStart}
                                onChange={(event) =>
                                    setAcademicYearStart(event.target.value)
                                }
                                placeholder="VD: 2024"
                                className="mt-1 w-full rounded-xl border border-gray-200 bg-white px-3 py-2 text-sm text-gray-700 outline-none focus:border-blue-500"
                            />
                            <div className="mt-1 text-xs text-gray-500">
                                {academicYear || "yyyy-yyyy"}
                            </div>
                            {getValidationError("academicYear") && (
                                <p className="mt-1 text-xs text-red-500">
                                    {getValidationError("academicYear")}
                                </p>
                            )}
                        </label>
                    </div>
                </section>

                <section className="rounded-2xl border border-gray-200 bg-white p-5 shadow-sm">
                    <div className="mb-4 border-b border-gray-200 pb-2 text-sm font-semibold text-gray-900">
                        Giảng viên phụ trách
                    </div>

                    <div className="grid gap-4 md:grid-cols-2">
                        <div
                            className="relative block text-sm font-medium text-gray-600"
                            onBlur={closeLecturerDropdown}
                        >
                            Giảng viên <span className="text-red-500">*</span>
                            <input
                                name="lecturerId"
                                type="hidden"
                                value={lecturerId}
                                readOnly
                            />
                            <button
                                type="button"
                                onClick={() =>
                                    setIsLecturerDropdownOpen(
                                        (isOpen) => !isOpen,
                                    )
                                }
                                className="mt-1 flex w-full items-center justify-between gap-2 rounded-xl border border-gray-200 bg-white px-3 py-2 text-left text-sm text-gray-700 outline-none focus:border-blue-500"
                            >
                                <span
                                    className={
                                        selectedLecturerName
                                            ? "truncate"
                                            : "truncate text-gray-400"
                                    }
                                >
                                    {selectedLecturerName || "Chọn giảng viên"}
                                </span>
                                <FiChevronDown size={16} />
                            </button>
                            {isLecturerDropdownOpen && (
                                <div className="absolute left-0 right-0 z-20 mt-2 overflow-hidden rounded-xl border border-gray-200 bg-white shadow-lg">
                                    <div className="border-b border-gray-100 p-2">
                                        <input
                                            autoFocus
                                            value={lecturerKeyword}
                                            onChange={(event) => {
                                                setLecturerKeyword(
                                                    event.target.value,
                                                );
                                                setLecturerId("");
                                                setSelectedLecturerName("");
                                            }}
                                            placeholder="Tìm giảng viên..."
                                            className="w-full rounded-lg border border-gray-200 bg-white px-3 py-2 text-sm text-gray-700 outline-none focus:border-blue-500"
                                        />
                                    </div>

                                    <div className="max-h-56 overflow-y-auto py-1">
                                        {isLoadingLecturers && (
                                            <div className="px-3 py-2 text-sm text-gray-500">
                                                Đang tải giảng viên...
                                            </div>
                                        )}

                                        {!isLoadingLecturers &&
                                            lecturers.map((lecturer) => (
                                                <button
                                                    key={lecturer.userId}
                                                    type="button"
                                                    onMouseDown={(event) => {
                                                        event.preventDefault();
                                                        setLecturerId(
                                                            lecturer.userId,
                                                        );
                                                        setSelectedLecturerName(
                                                            lecturer.fullName,
                                                        );
                                                        setLecturerKeyword("");
                                                        setIsLecturerDropdownOpen(
                                                            false,
                                                        );
                                                    }}
                                                    className="block w-full px-3 py-2 text-left text-sm text-gray-700 hover:bg-blue-50 hover:text-blue-700"
                                                >
                                                    {lecturer.fullName}
                                                </button>
                                            ))}

                                        {!isLoadingLecturers &&
                                            !lecturers.length && (
                                                <div className="px-3 py-2 text-sm text-gray-500">
                                                    Không tìm thấy giảng viên.
                                                </div>
                                            )}
                                    </div>
                                </div>
                            )}
                            {getValidationError("lecturerId") && (
                                <p className="mt-1 text-xs text-red-500">
                                    {getValidationError("lecturerId")}
                                </p>
                            )}
                        </div>

                        <label className="block text-sm font-medium text-gray-600">
                            Sĩ số tối đa
                            <input
                                type="number"
                                value={maxCapacity}
                                onChange={(event) =>
                                    setMaxCapacity(Number(event.target.value))
                                }
                                placeholder="VD: 40"
                                className="mt-1 w-full rounded-xl border border-gray-200 bg-white px-3 py-2 text-sm text-gray-700 outline-none focus:border-blue-500"
                            />
                            {getValidationError("maxCapacity") && (
                                <p className="mt-1 text-xs text-red-500">
                                    {getValidationError("maxCapacity")}
                                </p>
                            )}
                        </label>
                    </div>
                </section>

                <section className="rounded-2xl border border-gray-200 bg-white p-5 shadow-sm">
                    <div className="mb-4 border-b border-gray-200 pb-2 text-sm font-semibold text-gray-900">
                        Lịch học
                    </div>

                    <div className="mb-2 hidden grid-cols-[1.2fr_1fr_1fr_1fr_40px] gap-3 text-xs font-medium text-gray-500 md:grid">
                        <span>Thứ</span>
                        <span>Giờ bắt đầu</span>
                        <span>Giờ kết thúc</span>
                        <span>Phòng học</span>
                        <span />
                    </div>

                    <div className="space-y-3">
                        {schedules.map((schedule, index) => (
                            <div
                                key={schedule.id}
                                className="grid gap-3 md:grid-cols-[1.2fr_1fr_1fr_1fr_40px]"
                            >
                                <div>
                                    <select
                                        value={schedule.dayOfWeek}
                                        onChange={(event) =>
                                            updateSchedule(
                                                schedule.id,
                                                "dayOfWeek",
                                                Number(event.target.value),
                                            )
                                        }
                                        className="w-full rounded-xl border border-gray-200 bg-white px-3 py-2 text-sm text-gray-700 outline-none focus:border-blue-500"
                                    >
                                        {dayOptions.map((day) => (
                                            <option
                                                key={day.value}
                                                value={day.value}
                                            >
                                                {day.label}
                                            </option>
                                        ))}
                                    </select>
                                    {getScheduleValidationError(
                                        index,
                                        "dayOfWeek",
                                    ) && (
                                        <p className="mt-1 text-xs text-red-500">
                                            {getScheduleValidationError(
                                                index,
                                                "dayOfWeek",
                                            )}
                                        </p>
                                    )}
                                </div>
                                <div>
                                    <input
                                        type="time"
                                        value={schedule.startTime}
                                        onChange={(event) =>
                                            updateSchedule(
                                                schedule.id,
                                                "startTime",
                                                event.target.value,
                                            )
                                        }
                                        className="w-full rounded-xl border border-gray-200 bg-white px-3 py-2 text-sm text-gray-700 outline-none focus:border-blue-500"
                                    />
                                    {getScheduleValidationError(
                                        index,
                                        "startTime",
                                    ) && (
                                        <p className="mt-1 text-xs text-red-500">
                                            {getScheduleValidationError(
                                                index,
                                                "startTime",
                                            )}
                                        </p>
                                    )}
                                </div>
                                <div>
                                    <input
                                        type="time"
                                        value={schedule.endTime}
                                        onChange={(event) =>
                                            updateSchedule(
                                                schedule.id,
                                                "endTime",
                                                event.target.value,
                                            )
                                        }
                                        className="w-full rounded-xl border border-gray-200 bg-white px-3 py-2 text-sm text-gray-700 outline-none focus:border-blue-500"
                                    />
                                    {getScheduleValidationError(
                                        index,
                                        "endTime",
                                    ) && (
                                        <p className="mt-1 text-xs text-red-500">
                                            {getScheduleValidationError(
                                                index,
                                                "endTime",
                                            )}
                                        </p>
                                    )}
                                </div>
                                <div>
                                    <input
                                        value={schedule.room}
                                        onChange={(event) =>
                                            updateSchedule(
                                                schedule.id,
                                                "room",
                                                event.target.value,
                                            )
                                        }
                                        placeholder="VD: A101"
                                        className="w-full rounded-xl border border-gray-200 bg-white px-3 py-2 text-sm text-gray-700 outline-none focus:border-blue-500"
                                    />
                                    {getScheduleValidationError(
                                        index,
                                        "room",
                                    ) && (
                                        <p className="mt-1 text-xs text-red-500">
                                            {getScheduleValidationError(
                                                index,
                                                "room",
                                            )}
                                        </p>
                                    )}
                                </div>
                                <button
                                    type="button"
                                    onClick={() => removeSchedule(schedule.id)}
                                    className="flex size-10 items-center justify-center rounded-xl border border-gray-200 text-red-500 hover:bg-red-50"
                                    aria-label="Xóa buổi học"
                                >
                                    <FiTrash2 size={16} />
                                </button>
                                {scheduleConflictIndexes.has(index) && (
                                    <p className="text-xs text-red-500 md:col-span-5">
                                        Lich hoc bi trung voi mot dong khac.
                                    </p>
                                )}
                            </div>
                        ))}
                    </div>
                    {getValidationError("schedules") && (
                        <p className="mt-2 text-xs text-red-500">
                            {getValidationError("schedules")}
                        </p>
                    )}

                    <button
                        type="button"
                        onClick={addSchedule}
                        className="mt-3 flex items-center gap-2 text-sm font-semibold text-blue-600 hover:text-blue-700"
                    >
                        <FiPlus size={15} />
                        Thêm buổi học
                    </button>
                </section>

                <div className="flex justify-end gap-3 border-t border-gray-200 pt-5">
                    <Link
                        to="/courses"
                        className="rounded-xl border border-gray-200 px-5 py-2.5 text-sm font-semibold text-gray-600 hover:bg-gray-50"
                    >
                        Hủy
                    </Link>
                    <button
                        type="submit"
                        disabled={isPending || !isFormComplete}
                        className="rounded-xl bg-blue-600 px-5 py-2.5 text-sm font-semibold text-white hover:bg-blue-700 disabled:cursor-not-allowed disabled:opacity-60"
                    >
                        Tạo lớp học phần
                    </button>
                </div>
            </form>
        </div>
    );
};

export default CreateCourseSectionPage;
