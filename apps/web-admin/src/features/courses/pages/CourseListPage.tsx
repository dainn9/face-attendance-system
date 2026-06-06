import { useState } from "react";
import { useQueryClient } from "@tanstack/react-query";
import { FiPlus } from "react-icons/fi";
import { Link } from "react-router-dom";
import Spinner from "../../../shared/components/Spinner/Spinner";
import FilterSelect from "../../users/components/FilterSelect";
import { useFacultyLookup } from "../../users/hooks/lookup.query";
import { courseApi } from "../../../shared/api/services/course.api";
import CourseSectionCard from "../components/CourseSectionCard";
import { courseQueryKeys, useCourses } from "../hooks/course.query";
import { Semester, type CourseSectionDto } from "../types/course.types";

const defaultPagedResult = {
    items: [],
    totalCount: 0,
    page: 1,
    pageSize: 12,
    totalPages: 0,
    hasNextPage: false,
    hasPreviousPage: false,
};

const statusOptions = [
    { label: "Đang mở", value: "active" },
    { label: "Đã kết thúc", value: "inactive" },
];

const semesterOptions = [
    { label: "HK1", value: String(Semester.HK1) },
    { label: "HK2", value: String(Semester.HK2) },
    { label: "HK3", value: String(Semester.HK3) },
];

const CourseListPage = () => {
    const queryClient = useQueryClient();
    const [searchQuery, setSearchQuery] = useState("");
    const [facultyId, setFacultyId] = useState<string | undefined>();
    const [status, setStatus] = useState("");
    const [semester, setSemester] = useState("");
    const [schoolYearMode, setSchoolYearMode] = useState("");
    const [schoolYearStart, setSchoolYearStart] = useState("");
    const [schoolYearEnd, setSchoolYearEnd] = useState("");
    const [loadedPage, setLoadedPage] = useState(1);
    const [additionalCourses, setAdditionalCourses] = useState<
        CourseSectionDto[]
    >([]);
    const [isFetchingMore, setIsFetchingMore] = useState(false);

    const academicYear =
        schoolYearMode === "custom" && schoolYearStart && schoolYearEnd
            ? `${schoolYearStart}-${schoolYearEnd}`
            : undefined;

    const firstPageParams = {
        page: 1,
        pageSize: 12,
        searchQuery: searchQuery.trim() || undefined,
        facultyId: facultyId || undefined,
        semester: semester ? (Number(semester) as Semester) : undefined,
        academicYear,
        isActive: status ? status === "active" : undefined,
    };
    const { data = defaultPagedResult, isLoading } =
        useCourses(firstPageParams);
    const { data: faculties = [] } = useFacultyLookup();
    const courses = [...data.items, ...additionalCourses];
    const hasMore = loadedPage < data.totalPages;

    const resetLoadedCourses = () => {
        setLoadedPage(1);
        setAdditionalCourses([]);
    };

    const handleLoadMore = async () => {
        const nextPage = loadedPage + 1;
        const nextPageParams = {
            ...firstPageParams,
            page: nextPage,
        };

        setIsFetchingMore(true);
        try {
            const nextData = await queryClient.fetchQuery({
                queryKey: courseQueryKeys.list(nextPageParams),
                queryFn: () => courseApi.list(nextPageParams),
            });

            setAdditionalCourses((items) => [...items, ...nextData.items]);
            setLoadedPage(nextPage);
        } finally {
            setIsFetchingMore(false);
        }
    };

    if (isLoading && !courses.length) {
        return (
            <div className="flex min-h-[60vh] items-center justify-center">
                <Spinner className="size-10 text-blue-600" />
            </div>
        );
    }

    return (
        <div className="p-6">
            <div className="mb-5 flex items-center justify-between gap-4">
                <h1 className="text-2xl font-bold text-gray-900">Lớp học</h1>

                <Link
                    to="/courses/create"
                    className="flex items-center gap-2 rounded-xl bg-blue-600 px-4 py-2 text-sm font-semibold text-white hover:bg-blue-700"
                >
                    <FiPlus size={16} />
                    Tạo lớp học
                </Link>
            </div>

            <div className="mb-4 flex flex-wrap gap-3">
                <input
                    placeholder="Tìm tên lớp, mã lớp, mã môn..."
                    value={searchQuery}
                    onChange={(event) => {
                        setSearchQuery(event.target.value);
                        resetLoadedCourses();
                    }}
                    className="w-64 rounded-xl border border-gray-200 bg-white px-3 py-2 text-sm outline-none focus:border-blue-500"
                />

                <FilterSelect
                    placeholder="Tất cả khoa"
                    value={facultyId ?? ""}
                    onSelect={(value) => {
                        setFacultyId(value || undefined);
                        resetLoadedCourses();
                    }}
                    options={faculties.map((faculty) => ({
                        label: faculty.name,
                        value: faculty.id,
                    }))}
                />

                <FilterSelect
                    placeholder="Tất cả trạng thái"
                    value={status}
                    onSelect={(value) => {
                        setStatus(value);
                        resetLoadedCourses();
                    }}
                    options={statusOptions}
                />

                <FilterSelect
                    placeholder="Tất cả học kỳ"
                    value={semester}
                    onSelect={(value) => {
                        setSemester(value);
                        resetLoadedCourses();
                    }}
                    options={semesterOptions}
                />

                <FilterSelect
                    placeholder="Tất cả năm học"
                    value={schoolYearMode}
                    onSelect={(value) => {
                        setSchoolYearMode(value);
                        if (!value) {
                            setSchoolYearStart("");
                            setSchoolYearEnd("");
                        }
                        resetLoadedCourses();
                    }}
                    options={[{ label: "Tùy chọn", value: "custom" }]}
                />

                {schoolYearMode === "custom" && (
                    <div className="flex items-center gap-2">
                        <input
                            type="number"
                            min={2000}
                            max={2100}
                            placeholder="2024"
                            value={schoolYearStart}
                            onChange={(event) => {
                                setSchoolYearStart(event.target.value);
                                resetLoadedCourses();
                            }}
                            className="w-24 rounded-xl border border-gray-200 bg-white px-3 py-2 text-sm text-gray-600 outline-none focus:border-blue-500"
                        />
                        <span className="text-sm text-gray-400">-</span>
                        <input
                            type="number"
                            min={2000}
                            max={2100}
                            placeholder="2025"
                            value={schoolYearEnd}
                            onChange={(event) => {
                                setSchoolYearEnd(event.target.value);
                                resetLoadedCourses();
                            }}
                            className="w-24 rounded-xl border border-gray-200 bg-white px-3 py-2 text-sm text-gray-600 outline-none focus:border-blue-500"
                        />
                    </div>
                )}
            </div>

            <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
                {courses.map((course) => (
                    <CourseSectionCard key={course.id} course={course} />
                ))}
            </div>

            {!courses.length && (
                <div className="rounded-2xl border border-dashed border-gray-200 bg-white p-8 text-center text-sm text-gray-500">
                    Không có lớp học phần phù hợp.
                </div>
            )}

            <div className="mt-4 flex items-center justify-between">
                <div className="text-sm text-gray-500">
                    Hiển thị {courses.length} / {data.totalCount} lớp học phần
                </div>

                {hasMore && (
                    <button
                        type="button"
                        disabled={isFetchingMore}
                        onClick={handleLoadMore}
                        className="rounded-xl border border-gray-200 bg-white px-4 py-2 text-sm font-semibold text-gray-700 hover:bg-gray-50 disabled:cursor-not-allowed disabled:opacity-50"
                    >
                        {isFetchingMore ? "Đang tải..." : "Xem thêm"}
                    </button>
                )}
            </div>
        </div>
    );
};

export default CourseListPage;
