import { useProfileDetail } from "../hooks/profile.query";
import { Gender, Role, type UserDto } from "../types/profile.types";
import { getInitials } from "../../../shared/utils/getInitials";

const formatGender = (gender: UserDto["gender"]) => {
    if (gender === Gender.Male) return "Nam";
    if (gender === Gender.Female) return "Nữ";
    return "Chưa cập nhật";
};

const formatRole = (role: UserDto["role"]) => {
    if (role === Role.Lecturer) return "Giảng viên";
    if (role === Role.Student) return "Sinh viên";
    if (role === Role.Admin) return "Quản trị viên";
    return "Chưa xác định";
};

const formatDate = (value: string) => {
    if (!value) return "Chưa cập nhật";

    const date = new Date(value);
    if (Number.isNaN(date.getTime())) return value;

    return date.toLocaleDateString("vi-VN");
};

const ProfilePage = () => {
    const profileQuery = useProfileDetail();
    const profile = profileQuery.data;

    if (profileQuery.isLoading) {
        return (
            <div className="rounded-lg bg-white p-5 text-sm text-gray-500 shadow-sm">
                Đang tải hồ sơ...
            </div>
        );
    }

    if (!profile) {
        return (
            <div className="rounded-lg bg-white p-5 text-sm text-gray-500 shadow-sm">
                Không tìm thấy thông tin hồ sơ.
            </div>
        );
    }

    const profileItems = [
        ["Mã người dùng", profile.userCode],
        ["Email", profile.email],
        ["Giới tính", formatGender(profile.gender)],
        ["Ngày sinh", formatDate(profile.dateOfBirth)],
        ["Vai trò", formatRole(profile.role)],
    ];

    return (
        <div className="space-y-5">
            <div>
                <h1 className="text-xl font-semibold text-gray-900">
                    Thông tin cá nhân
                </h1>
                <p className="mt-1 text-sm text-gray-500">
                    Quản lý thông tin cá nhân của bạn
                </p>
            </div>

            <section className="rounded-lg bg-white p-5 shadow-sm">
                <div className="flex flex-col gap-4 sm:flex-row sm:items-center">
                    <span className="flex size-16 items-center justify-center rounded-full bg-indigo-100 text-lg font-semibold text-indigo-700">
                        {getInitials(profile.fullName)}
                    </span>
                    <div>
                        <h2 className="text-lg font-semibold text-gray-900">
                            {profile.fullName}
                        </h2>
                        <p className="mt-1 text-sm text-gray-500">
                            {formatRole(profile.role)} - {profile.userCode}
                        </p>
                    </div>
                </div>
            </section>

            <section className="rounded-lg bg-white p-5 shadow-sm">
                <h2 className="text-sm font-semibold text-gray-900">
                    Thông tin chi tiết
                </h2>
                <div className="mt-3 divide-y divide-gray-100 text-sm">
                    {profileItems.map(([label, value]) => (
                        <div
                            key={label}
                            className="flex flex-col gap-1 py-3 sm:flex-row sm:items-center sm:justify-between"
                        >
                            <span className="text-gray-500">{label}</span>
                            <span className="font-medium text-gray-900">
                                {value || "Chưa cập nhật"}
                            </span>
                        </div>
                    ))}
                </div>
            </section>
        </div>
    );
};

export default ProfilePage;
