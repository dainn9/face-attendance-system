import { Link } from "react-router-dom";
import type { FacultyDetail } from "../types/faculty.types";
import {
    FiChevronRight,
    FiCpu,
    FiEdit,
    FiPlus,
    FiTrash2,
} from "react-icons/fi";
import StatCard from "./StatCard";
import { getInitials } from "../../../shared/utils/getInitials";

type Props = {
    data: FacultyDetail;
    onCreateMajor?: () => void;
};

const FacultyDetailView = ({ data, onCreateMajor }: Props) => {
    return (
        <div className="p-6">
            <div className="mb-4 flex items-center gap-2 text-sm text-gray-500">
                <Link to="/faculties" className="hover:text-blue-600">
                    Khoa / Ngành
                </Link>
                <FiChevronRight className="size-4" />
                <span className="font-medium text-gray-800">{data.name}</span>
            </div>

            <div className="mb-6 flex items-center justify-between rounded-2xl border bg-white p-5 shadow-sm">
                <div className="flex items-center gap-4">
                    <div className="flex size-14 items-center justify-center rounded-2xl bg-blue-50 text-blue-600">
                        <FiCpu className="size-7" />
                    </div>

                    <div>
                        <h1 className="text-2xl font-bold text-gray-900">
                            {data.name}
                        </h1>
                        <p className="text-sm text-gray-500">
                            Mã khoa: {data.code}
                        </p>
                    </div>
                </div>

                <button className="flex items-center gap-2 rounded-xl border px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50">
                    <FiEdit className="size-4" />
                    Sửa
                </button>
            </div>

            <div className="mb-6 grid gap-4 md:grid-cols-3">
                <StatCard label="Ngành" value={data.majorCount} />
                <StatCard label="Sinh viên" value={data.studentCount} />
                <StatCard label="Giảng viên" value={data.lecturerCount} />
            </div>

            <div className="mb-6 rounded-2xl border bg-white p-5 shadow-sm">
                <div className="mb-4 flex items-center justify-between">
                    <h2 className="text-lg font-semibold text-gray-900">
                        Danh sách ngành
                    </h2>

                    <button
                        // onClick={onCreateMajor}
                        className="flex items-center gap-2 rounded-xl bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
                    >
                        <FiPlus className="size-4" />
                        Thêm ngành
                    </button>
                </div>

                <div className="divide-y">
                    {data.majors.map((major) => (
                        <div
                            key={major.id}
                            className="flex items-center justify-between py-4"
                        >
                            <div>
                                <div className="font-medium text-gray-900">
                                    {major.name}
                                </div>
                                <div className="text-sm text-gray-500">
                                    {major.code}
                                </div>
                            </div>

                            <div className="flex items-center gap-4">
                                <div className="text-sm text-gray-500">
                                    {major.studentCount} sinh viên
                                </div>

                                <div className="flex items-center gap-2 text-gray-400">
                                    <button className="hover:text-blue-600">
                                        <FiEdit className="size-4" />
                                    </button>
                                    <button className="hover:text-red-600">
                                        <FiTrash2 className="size-4" />
                                    </button>
                                </div>
                            </div>
                        </div>
                    ))}

                    {data.majors.length === 0 && (
                        <div className="py-6 text-center text-sm text-gray-500">
                            Chưa có ngành nào.
                        </div>
                    )}
                </div>
            </div>

            <div className="rounded-2xl border bg-white p-5 shadow-sm">
                <h2 className="mb-4 text-lg font-semibold text-gray-900">
                    Giảng viên
                </h2>

                <div className="divide-y">
                    {data.lecturers.map((lecturer) => (
                        <div
                            key={lecturer.id}
                            className="flex items-center justify-between py-4"
                        >
                            <div className="flex items-center gap-3">
                                <div className="flex size-10 items-center justify-center rounded-full bg-blue-50 text-sm font-semibold text-blue-600">
                                    {getInitials(lecturer.name)}
                                </div>

                                <div>
                                    <div className="font-medium text-gray-900">
                                        {lecturer.name}
                                    </div>
                                    <div className="text-sm text-gray-500">
                                        {lecturer.userCode}
                                    </div>
                                </div>
                            </div>
                        </div>
                    ))}

                    {data.lecturers.length === 0 && (
                        <div className="py-6 text-center text-sm text-gray-500">
                            Chưa có giảng viên nào.
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
};

export default FacultyDetailView;
