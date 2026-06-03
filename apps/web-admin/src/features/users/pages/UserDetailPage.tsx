import {
    FiChevronRight,
    FiEdit2,
    FiMail,
    FiTrash2,
    FiUser,
} from "react-icons/fi";
import { Link } from "react-router-dom";
import { getInitials } from "../../../shared/utils/getInitials";
import RoleBadge from "../components/RoleBadge";
import StatusBadge from "../components/StatusBadge";
import type { UserRole } from "../types/user.types";

const userDetailData = {
    userId: "1",
    fullName: "Nguyễn Văn A",
    email: "a.nguyen@school.edu",
    role: "Student" as UserRole,
    isActive: true,
    personalInfo: {
        gender: "Nam",
        dateOfBirth: "01/01/2002",
        userCode: "SV2021001",
        majorName: "Khoa học máy tính",
        facultyName: "Công nghệ thông tin",
    },
    classes: [
        {
            id: "WEB301-01",
            name: "Lập trình Web",
            code: "WEB301-01",
            semester: "HK2 2024-2025",
            status: "Đang học",
            isActive: true,
        },
        {
            id: "DB201-02",
            name: "Cơ sở dữ liệu",
            code: "DB201-02",
            semester: "HK2 2024-2025",
            status: "Đang học",
            isActive: true,
        },
        {
            id: "NET201-01",
            name: "Mạng máy tính",
            code: "NET201-01",
            semester: "HK1 2024-2025",
            status: "Kết thúc",
            isActive: false,
        },
    ],
};

const personalFields = [
    { label: "Họ và tên", value: userDetailData.fullName },
    { label: "Giới tính", value: userDetailData.personalInfo.gender },
    { label: "Ngày sinh", value: userDetailData.personalInfo.dateOfBirth },
    { label: "Mã số", value: userDetailData.personalInfo.userCode },
    { label: "Ngành", value: userDetailData.personalInfo.majorName },
    { label: "Khoa", value: userDetailData.personalInfo.facultyName },
];

const UserDetailPage = () => {
    const user = userDetailData;

    return (
        <div className="p-6">
            <div className="mb-4 flex items-center gap-2 text-sm text-gray-500">
                <Link to="/users" className="hover:text-blue-600">
                    Người dùng
                </Link>
                <FiChevronRight size={14} />
                <span className="font-medium text-gray-900">
                    {user.fullName}
                </span>
            </div>

            <div className="mb-4 flex items-center gap-4 rounded-2xl border border-gray-200 bg-white p-5 shadow-sm">
                <div className="flex size-16 shrink-0 items-center justify-center rounded-full bg-blue-50 text-lg font-bold text-blue-600">
                    {getInitials(user.fullName)}
                </div>

                <div className="min-w-0 flex-1">
                    <h1 className="truncate text-xl font-bold text-gray-900">
                        {user.fullName}
                    </h1>
                    <div className="mt-2 flex flex-wrap items-center gap-2">
                        <RoleBadge role={user.role} />
                        <StatusBadge isActive={user.isActive} />
                    </div>
                    <div className="mt-2 flex items-center gap-2 text-sm text-gray-500">
                        <FiMail size={14} />
                        <span className="truncate">{user.email}</span>
                    </div>
                </div>

                <div className="flex shrink-0 gap-2">
                    <button
                        type="button"
                        className="flex items-center gap-2 rounded-xl border border-gray-200 px-4 py-2 text-sm font-semibold text-gray-700 hover:bg-gray-50"
                    >
                        <FiEdit2 size={16} />
                        Sửa
                    </button>
                    <button
                        type="button"
                        className="flex items-center gap-2 rounded-xl border border-red-200 px-4 py-2 text-sm font-semibold text-red-600 hover:bg-red-50"
                    >
                        <FiTrash2 size={16} />
                        Xóa
                    </button>
                </div>
            </div>

            <div className="space-y-4">
                <section className="overflow-hidden rounded-2xl border border-gray-200 bg-white shadow-sm">
                    <div className="border-b border-gray-200 px-5 py-3">
                        <h2 className="text-sm font-bold text-gray-900">
                            Thông tin cá nhân
                        </h2>
                    </div>

                    <div className="grid gap-4 p-5 sm:grid-cols-2 lg:grid-cols-3">
                        {personalFields.map((field) => (
                            <div key={field.label} className="space-y-1">
                                <div className="text-xs font-medium text-gray-500">
                                    {field.label}
                                </div>
                                <div className="text-sm font-semibold text-gray-900">
                                    {field.value || "Chưa cập nhật"}
                                </div>
                            </div>
                        ))}
                    </div>
                </section>

                <section className="overflow-hidden rounded-2xl border border-gray-200 bg-white shadow-sm">
                    <div className="border-b border-gray-200 px-5 py-3">
                        <h2 className="text-sm font-bold text-gray-900">
                            Lớp đang tham gia
                        </h2>
                    </div>

                    <div className="divide-y divide-gray-200">
                        {user.classes.map((classItem) => (
                            <div
                                key={classItem.id}
                                className="flex items-center justify-between gap-4 px-5 py-4"
                            >
                                <div className="flex min-w-0 items-center gap-3">
                                    <div className="flex size-10 shrink-0 items-center justify-center rounded-xl bg-gray-100 text-gray-500">
                                        <FiUser size={16} />
                                    </div>
                                    <div className="min-w-0">
                                        <div className="truncate text-sm font-semibold text-gray-900">
                                            {classItem.name}
                                        </div>
                                        <div className="mt-1 text-xs text-gray-500">
                                            {classItem.code} ·{" "}
                                            {classItem.semester}
                                        </div>
                                    </div>
                                </div>

                                <span
                                    className={`shrink-0 rounded-full px-2.5 py-1 text-xs font-semibold ${
                                        classItem.isActive
                                            ? "bg-emerald-50 text-emerald-700"
                                            : "bg-gray-100 text-gray-600"
                                    }`}
                                >
                                    {classItem.status}
                                </span>
                            </div>
                        ))}
                    </div>
                </section>
            </div>
        </div>
    );
};

export default UserDetailPage;
