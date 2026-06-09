import { useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { useProfileDetail } from "../../profile/hooks/profile.query";
import { getInitials } from "../../../shared/utils/getInitials";
import { lecturer } from "../data/mockCourses";
import {
    useLecturerCourseSectionLookup,
    useLecturerCourseSections,
} from "../hooks/course.query";
import {
    formatDayOfWeek,
    formatTimeToMinute,
    type LecturerCourseSectionLookup,
    type Semester,
} from "../types/course.types";

const getLookupKey = (item: LecturerCourseSectionLookup) =>
    `${item.semester}-${getLookupAcademicYear(item)}`;

const getLookupAcademicYear = (item: LecturerCourseSectionLookup) => {
    if ("academic" in item) {
        return String(item.academic);
    }

    return item.academicYear;
};

const emptyLookupOptions: LecturerCourseSectionLookup[] = [];

const LecturerCourseDashboardPage = () => {
    const [page, setPage] = useState(1);
    const [selectedLookupKey, setSelectedLookupKey] = useState("");
    const [statusFilter, setStatusFilter] = useState("");
    const [searchQuery, setSearchQuery] = useState("");

    const profileQuery = useProfileDetail();
    const profile = profileQuery.data;
    const lookupQuery = useLecturerCourseSectionLookup();
    const lookupOptions = lookupQuery.data ?? emptyLookupOptions;
    const selectedLookup = useMemo(
        () =>
            lookupOptions.find(
                (item) => getLookupKey(item) === selectedLookupKey,
            ) ?? lookupOptions[0],
        [lookupOptions, selectedLookupKey],
    );

    const courseSectionsQuery = useLecturerCourseSections({
        page,
        pageSize: 20,
        searchQuery: searchQuery || undefined,
        semester: selectedLookup
            ? (Number(selectedLookup.semester) as Semester)
            : undefined,
        academicYear: selectedLookup
            ? getLookupAcademicYear(selectedLookup)
            : undefined,
        isActive: statusFilter === "" ? undefined : statusFilter === "active",
    });
    const coursePage = courseSectionsQuery.data;
    const courseSections = coursePage?.items ?? [];
    const activeCount = courseSections.filter(
        (course) => course.isActive,
    ).length;
    const totalStudents = courseSections.reduce(
        (sum, course) => sum + course.studentCount,
        0,
    );

    return (
        <div className="space-y-5">
            <div className="flex flex-col gap-3 lg:flex-row lg:items-center lg:justify-between">
                <div>
                    <h1 className="text-xl font-semibold text-gray-900">
                        Lớp học của tôi -{" "}
                        {selectedLookup?.label ?? lecturer.semester}
                    </h1>
                    <p className="mt-1 text-sm text-gray-500">
                        {lecturer.semester}
                    </p>
                </div>
                <div className="flex items-center gap-3">
                    <span className="text-sm text-gray-500">
                        {profile?.fullName ?? "Giang vien"}
                    </span>
                    <span className="flex size-10 items-center justify-center rounded-full bg-indigo-100 text-sm font-semibold text-indigo-700">
                        {getInitials(profile?.fullName)}
                    </span>
                </div>
            </div>

            <div className="flex flex-wrap gap-2">
                <select
                    className="rounded-md border border-gray-300 bg-white px-3 py-2 text-sm text-gray-700"
                    value={selectedLookup ? getLookupKey(selectedLookup) : ""}
                    onChange={(event) => {
                        setSelectedLookupKey(event.target.value);
                        setPage(1);
                    }}
                >
                    {lookupOptions.length === 0 && (
                        <option value="">Đang tải học kì...</option>
                    )}
                    {lookupOptions.map((item) => (
                        <option
                            key={getLookupKey(item)}
                            value={getLookupKey(item)}
                        >
                            {item.label}
                        </option>
                    ))}
                </select>
                <select
                    className="rounded-md border border-gray-300 bg-white px-3 py-2 text-sm text-gray-700"
                    value={statusFilter}
                    onChange={(event) => {
                        setStatusFilter(event.target.value);
                        setPage(1);
                    }}
                >
                    <option value="">Tất cả trạng thái</option>
                    <option value="active">Hoạt động</option>
                    <option value="inactive">Không hoạt động</option>
                </select>
                <input
                    className="w-52 rounded-md border border-gray-300 bg-white px-3 py-2 text-sm"
                    placeholder="Tim kiem lop..."
                    value={searchQuery}
                    onChange={(event) => {
                        setSearchQuery(event.target.value);
                        setPage(1);
                    }}
                />
            </div>

            <div className="grid gap-3 sm:grid-cols-3">
                {[
                    ["Lớp đang dạy", activeCount, "trong học kỳ này"],
                    ["Tổng sinh viên", totalStudents, "trong học kỳ này"],
                    [
                        "Tổng lớp học",
                        coursePage?.totalCount ?? 0,
                        selectedLookup?.label ?? lecturer.semester,
                    ],
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

            <p className="text-xs font-semibold uppercase tracking-wide text-gray-500">
                {coursePage?.totalCount ?? 0} lop hoc -{" "}
                {selectedLookup?.label ?? lecturer.semester}
            </p>

            <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
                {courseSectionsQuery.isLoading && (
                    <div className="rounded-lg border border-gray-200 bg-white p-5 text-sm text-gray-500 shadow-sm">
                        Đang tải danh sách lớp...
                    </div>
                )}

                {!courseSectionsQuery.isLoading &&
                    courseSections.length === 0 && (
                        <div className="rounded-lg border border-gray-200 bg-white p-5 text-sm text-gray-500 shadow-sm">
                            Không tìm thấy lớp học nào phù hợp
                        </div>
                    )}

                {courseSections.map((course) => (
                    <Link
                        key={course.id}
                        to={`/lecturer/courses/${course.id}`}
                        className="rounded-lg border border-gray-200 bg-white p-5 shadow-sm transition hover:border-gray-400"
                    >
                        <div className="flex items-start justify-between gap-3">
                            <div>
                                <h2 className="text-base font-semibold text-gray-900">
                                    {course.subjectName}
                                </h2>
                                <p className="mt-1 text-xs text-gray-500">
                                    {course.courseSectionCode} -{" "}
                                    {course.subjectCode}
                                </p>
                            </div>
                            <span
                                className={`shrink-0 rounded-md px-2 py-1 text-xs font-medium ${course.isActive ? "bg-green-100 text-green-700" : "bg-gray-100 text-gray-600"}`}
                            >
                                {course.isActive ? "Hoạt động" : "Đã kết thúc"}
                            </span>
                        </div>

                        <div className="mt-4 space-y-2">
                            {course.schedules.map((item) => (
                                <p
                                    key={`${item.dayOfWeek}-${item.startTime}-${item.room}`}
                                    className="text-sm text-gray-500"
                                >
                                    {formatDayOfWeek(item.dayOfWeek)}{" "}
                                    {formatTimeToMinute(item.startTime)} -{" "}
                                    {formatTimeToMinute(item.endTime)} - P.
                                    {item.room}
                                </p>
                            ))}
                        </div>

                        <div className="mt-4 border-t border-gray-100 pt-4">
                            <div className="flex items-center justify-between gap-3">
                                <p className="text-xs text-gray-500">
                                    Sinh viên:{" "}
                                    <span className="font-semibold text-gray-900">
                                        {course.studentCount}
                                    </span>
                                </p>
                                <span className="rounded-md border border-gray-300 px-3 py-1.5 text-xs text-gray-700">
                                    Xem chi tiet
                                </span>
                            </div>
                        </div>
                    </Link>
                ))}
            </div>

            {coursePage && coursePage.totalPages > 1 && (
                <div className="flex items-center justify-between rounded-lg bg-white px-4 py-3 text-sm shadow-sm">
                    <span className="text-gray-500">
                        Trang {coursePage.page} / {coursePage.totalPages} - Tổng{" "}
                        {coursePage.totalCount} lớp học
                    </span>
                    <div className="flex gap-2">
                        <button
                            type="button"
                            className="rounded-md border border-gray-300 px-3 py-1.5 text-gray-700 disabled:cursor-not-allowed disabled:opacity-50"
                            disabled={!coursePage.hasPreviousPage}
                            onClick={() =>
                                setPage((current) => Math.max(1, current - 1))
                            }
                        >
                            Truoc
                        </button>
                        <button
                            type="button"
                            className="rounded-md border border-gray-300 px-3 py-1.5 text-gray-700 disabled:cursor-not-allowed disabled:opacity-50"
                            disabled={!coursePage.hasNextPage}
                            onClick={() => setPage((current) => current + 1)}
                        >
                            Sau
                        </button>
                    </div>
                </div>
            )}
        </div>
    );
};

export default LecturerCourseDashboardPage;
