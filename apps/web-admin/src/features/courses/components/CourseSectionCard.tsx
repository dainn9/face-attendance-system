import {
    FiCalendar,
    FiClock,
    FiEdit2,
    FiEye,
    FiMapPin,
    FiTrash2,
    FiUser,
} from "react-icons/fi";
import { Link } from "react-router-dom";
import {
    formatDayOfWeek,
    formatSemester,
    formatTimeToMinute,
    type CourseSectionDto,
} from "../types/course.types";

type CourseSectionCardProps = {
    course: CourseSectionDto;
};

const CourseSectionCard = ({ course }: CourseSectionCardProps) => {
    const firstSchedule = course.firstSchedule;

    return (
        <article className="rounded-2xl border border-gray-200 bg-white p-4 shadow-sm transition hover:border-blue-300 hover:shadow-md">
            <div className="mb-3 flex items-start justify-between gap-3">
                <div className="min-w-0">
                    <h2 className="truncate text-base font-semibold text-gray-900">
                        {course.subjectName}
                    </h2>
                    <div className="mt-1 text-xs text-gray-500">
                        {course.courseSectionCode} - {course.subjectCode}
                    </div>
                </div>

                <span
                    className={`shrink-0 rounded-full px-2.5 py-1 text-xs font-semibold ${
                        course.isActive
                            ? "bg-emerald-50 text-emerald-700"
                            : "bg-gray-100 text-gray-600"
                    }`}
                >
                    {course.isActive ? "Đang mở" : "Đã kết thúc"}
                </span>
            </div>

            <div className="space-y-2 text-sm text-gray-500">
                <div className="flex items-center gap-2">
                    <FiUser size={15} />
                    <span>GV: {course.lecturerName}</span>
                </div>
                {firstSchedule && (
                    <>
                        <div className="flex items-center gap-2">
                            <FiCalendar size={15} />
                            <span>
                                {formatDayOfWeek(firstSchedule.dayOfWeek)} -{" "}
                                {formatTimeToMinute(firstSchedule.startTime)}-
                                {formatTimeToMinute(firstSchedule.endTime)}
                            </span>
                        </div>
                        <div className="flex items-center gap-2">
                            <FiMapPin size={15} />
                            <span>Phòng {firstSchedule.room}</span>
                        </div>
                    </>
                )}
                <div className="flex items-center gap-2">
                    <FiClock size={15} />
                    <span>
                        {formatSemester(course.semester)} -{" "}
                        {course.academicYear}
                    </span>
                </div>
            </div>

            <div className="mt-4 flex items-center justify-between border-t border-gray-200 pt-3">
                <div className="text-sm text-gray-500">
                    <strong className="font-semibold text-gray-900">
                        {course.studentCount}
                    </strong>{" "}
                    Sinh viên
                </div>

                <div className="flex items-center gap-3 text-gray-400">
                    <Link
                        to={`/courses/${course.id}`}
                        className="hover:text-blue-600"
                        aria-label="Xem lop hoc"
                    >
                        <FiEye size={16} />
                    </Link>
                    <button
                        type="button"
                        className="hover:text-blue-600"
                        aria-label="Sua lop hoc"
                    >
                        <FiEdit2 size={16} />
                    </button>
                    <button
                        type="button"
                        className="hover:text-red-600"
                        aria-label="Xoa lop hoc"
                    >
                        <FiTrash2 size={16} />
                    </button>
                </div>
            </div>
        </article>
    );
};

export default CourseSectionCard;
