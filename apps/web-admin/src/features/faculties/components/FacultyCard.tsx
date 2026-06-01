import { FiCpu, FiEdit2, FiTrash2 } from "react-icons/fi";
import type { Faculty, Major } from "../types/faculty.types";
import { Link } from "react-router-dom";

type FacultyCardProps = {
    faculty: Faculty;
};

const MajorItem = ({ major }: { major: Major }) => {
    return (
        <div className="flex items-center justify-between rounded-xl border border-gray-100 bg-gray-50 px-4 py-3">
            <div>
                <div className="text-sm font-semibold text-gray-900">
                    {major.name}
                </div>
                <div className="text-xs text-gray-500">{major.code}</div>
            </div>

            <div className="flex items-center gap-3">
                <span className="text-sm font-semibold text-blue-600">
                    {major.studentCount} SV
                </span>
            </div>
        </div>
    );
};

const FacultyCard = ({ faculty }: FacultyCardProps) => {
    return (
        <div className="rounded-2xl border border-gray-200 bg-white p-5 shadow-sm flex h-full flex-col">
            <div className="flex items-start justify-between gap-4">
                <div className="flex items-center gap-3">
                    <div className="flex h-11 w-11 items-center justify-center rounded-xl bg-blue-50 text-blue-600">
                        <FiCpu size={22} />
                    </div>

                    <div>
                        <h3 className="text-base font-bold text-gray-900">
                            {faculty.name}
                        </h3>
                        <p className="text-sm text-gray-500">
                            Mã: {faculty.code}
                        </p>
                    </div>
                </div>

                <div className="flex items-center gap-3 text-gray-400">
                    <Link
                        to={`/faculties/${faculty.id}`}
                        className="hover:text-blue-600"
                    >
                        <FiEdit2 size={17} />
                    </Link>
                    <button className="hover:text-red-600">
                        <FiTrash2 size={17} />
                    </button>
                </div>
            </div>

            <div className="mt-5 grid grid-cols-3 gap-3">
                <div className="rounded-xl bg-gray-50 py-3 text-center">
                    <div className="text-xl font-bold text-gray-900">
                        {faculty.majorCount}
                    </div>
                    <div className="text-xs text-gray-500">Ngành</div>
                </div>

                <div className="rounded-xl bg-gray-50 py-3 text-center">
                    <div className="text-xl font-bold text-gray-900">
                        {faculty.studentCount}
                    </div>
                    <div className="text-xs text-gray-500">Sinh viên</div>
                </div>

                <div className="rounded-xl bg-gray-50 py-3 text-center">
                    <div className="text-xl font-bold text-gray-900">
                        {faculty.lecturerCount}
                    </div>
                    <div className="text-xs text-gray-500">Giảng viên</div>
                </div>
            </div>

            <div className="mt-5 flex flex-1 flex-col">
                <div className="mb-3 text-sm font-bold text-gray-900">
                    Danh sách ngành
                </div>

                <div className="flex-1 space-y-3">
                    {faculty.majors.map((major) => (
                        <MajorItem key={major.id} major={major} />
                    ))}
                </div>

                <Link
                    to={`/faculties/${faculty.id}`}
                    className="mt-4 flex w-full items-center justify-center rounded-xl bg-blue-600 py-3 text-sm font-semibold text-white hover:bg-blue-700"
                >
                    Xem chi tiết
                </Link>
            </div>
        </div>
    );
};

export default FacultyCard;
