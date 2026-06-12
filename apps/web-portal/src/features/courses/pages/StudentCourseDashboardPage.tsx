import { Link } from "react-router-dom";
import { useProfileDetail } from "../../profile/hooks/profile.query";
import { getInitials } from "../../../shared/utils/getInitials";
import { useStudentCourseSections } from "../hooks/course.query";
import {
    formatDayOfWeek,
    formatSemester,
    formatTimeToMinute,
    type StudentCourseSectionDto,
} from "../types/course.types";

const formatSchedules = (course: StudentCourseSectionDto) =>
    course.schedules.map(
        (item) =>
            `${formatDayOfWeek(item.dayOfWeek)} ${formatTimeToMinute(item.startTime)} - ${formatTimeToMinute(item.endTime)} - P.${item.room}`,
    );

const canCheckInCourse = (course: StudentCourseSectionDto) =>
    course.canCheckIn ?? !!course.openAttendanceSessionId;

const getCourseDetailPath = (course: StudentCourseSectionDto) =>
    course.openAttendanceSessionId
        ? `/student/courses/${course.id}?sessionId=${course.openAttendanceSessionId}`
        : `/student/courses/${course.id}`;

const StudentCourseDashboardPage = () => {
    const profileQuery = useProfileDetail();
    const courseSectionsQuery = useStudentCourseSections();
    const profile = profileQuery.data;
    const courses = courseSectionsQuery.data ?? [];
    const firstCourse = courses[0];
    const semesterLabel = firstCourse
        ? `${formatSemester(firstCourse.semester)} ${firstCourse.academicYear}`
        : "Chua co hoc ky";
    const studentName = profile?.fullName ?? "Sinh vien";
    const studentCode = profile?.userCode ?? "-";
    const activeCourse = courses.find(canCheckInCourse);

    return (
        <div className="space-y-4 sm:space-y-5">
            <div className="flex items-start justify-between gap-3">
                <div className="min-w-0">
                    <h1 className="truncate text-lg font-semibold text-gray-900 sm:text-xl">
                        Xin chao, {studentName}
                    </h1>
                    <p className="mt-1 text-sm text-gray-500">
                        MSSV: {studentCode}
                    </p>
                    <p className="text-xs text-gray-500 sm:text-sm">
                        {semesterLabel}
                    </p>
                </div>
                <div className="shrink-0">
                    <span className="flex size-11 items-center justify-center rounded-full bg-indigo-100 text-sm font-semibold text-indigo-700">
                        {getInitials(studentName)}
                    </span>
                </div>
            </div>

            {activeCourse && (
                <section className="flex flex-col gap-4 rounded-lg border border-emerald-200 bg-emerald-50 p-4 sm:p-5 lg:flex-row lg:items-center lg:justify-between">
                    <div className="min-w-0">
                        <p className="text-sm font-semibold leading-5 text-emerald-900">
                            <span className="mr-2 inline-block size-2 rounded-full bg-emerald-600" />
                            Dang co buoi diem danh - {activeCourse.subjectName}
                        </p>
                    </div>
                    <Link
                        to={getCourseDetailPath(activeCourse)}
                        className="inline-flex min-h-11 items-center justify-center rounded-md bg-emerald-600 px-4 py-2 text-sm font-semibold text-white sm:w-auto"
                    >
                        Xem lop diem danh
                    </Link>
                </section>
            )}

            <p className="text-xs font-semibold uppercase tracking-wide text-gray-500">
                {courses.length} lop hoc - {semesterLabel}
            </p>

            {courseSectionsQuery.isLoading && (
                <div className="rounded-lg bg-white p-5 text-sm text-gray-500 shadow-sm">
                    Dang tai danh sach lop...
                </div>
            )}

            {!courseSectionsQuery.isLoading && courses.length === 0 && (
                <div className="rounded-lg bg-white p-5 text-sm text-gray-500 shadow-sm">
                    Chua co lop hoc nao.
                </div>
            )}

            <div className="grid gap-3 sm:grid-cols-2 xl:grid-cols-4">
                {courses.map((course) => {
                    const schedules = formatSchedules(course);
                    const canCheckIn = canCheckInCourse(course);

                    return (
                        <Link
                            key={course.id}
                            to={getCourseDetailPath(course)}
                            className={`rounded-lg border bg-white p-4 shadow-sm transition hover:border-gray-400 sm:p-5 ${canCheckIn ? "border-emerald-300" : "border-gray-200"}`}
                        >
                            <div className="flex items-start justify-between gap-3">
                                <div className="min-w-0">
                                    <h2 className="line-clamp-2 text-base font-semibold leading-5 text-gray-900">
                                        {course.subjectName}
                                    </h2>
                                    <p className="mt-1 text-xs text-gray-500">
                                        {course.courseSectionCode} -{" "}
                                        {course.subjectCode}
                                    </p>
                                </div>
                                <span
                                    className={`shrink-0 rounded-md px-2 py-1 text-xs font-medium ${canCheckIn ? "bg-emerald-100 text-emerald-700" : "bg-gray-100 text-gray-600"}`}
                                >
                                    {canCheckIn ? "Dang mo" : "Dang hoc"}
                                </span>
                            </div>
                            <p className="mt-4 truncate text-sm text-gray-500">
                                GV {course.lecturerName}
                            </p>
                            <div className="mt-2 space-y-1 text-sm leading-5 text-gray-500">
                                {schedules.length > 0 ? (
                                    schedules.map((schedule) => (
                                        <p key={schedule}>{schedule}</p>
                                    ))
                                ) : (
                                    <p>Chua co lich hoc</p>
                                )}
                            </div>
                        </Link>
                    );
                })}
            </div>
        </div>
    );
};

export default StudentCourseDashboardPage;
