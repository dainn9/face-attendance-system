import {
    FaCamera,
    FaChartBar,
    FaClipboardCheck,
    FaSchool,
    FaUser,
} from "react-icons/fa";
import { NavLink } from "react-router-dom";
import { useAuthStore } from "../../../features/auth/store/auth.store";
import { PATHS } from "../../constants/path";
import LogoutButton from "../LogoutButton/LogoutButton";

const Sidebar = () => {
    const baseClass =
        "flex items-center gap-3 px-4 py-3 rounded-xl mt-1 font-medium";
    const role = useAuthStore((s) => s.user?.role?.toLowerCase());
    const isStudent = role === "student";
    const isLecturer = role === "lecturer";

    return (
        <aside className="w-64 bg-white shadow-xl flex flex-col">
            <div className="p-6 border-b">
                <h1 className="text-2xl font-bold text-blue-600">FaceAttend</h1>
                <p className="text-sm text-gray-500">
                    Hệ thống điểm danh bằng nhận diện khuôn mặt
                </p>
            </div>

            <nav className="p-4 flex-1">
                {isLecturer && (
                    <>
                        <NavLink
                            to={PATHS.LECTURER}
                            end
                            className={({ isActive }) =>
                                `${baseClass} ${isActive ? "bg-blue-50 text-blue-600" : "hover:bg-gray-100"}`
                            }
                        >
                            <FaSchool /> Lớp học của tôi
                        </NavLink>
                    </>
                )}

                {isStudent && (
                    <>
                        <NavLink
                            to={PATHS.STUDENT}
                            end
                            className={({ isActive }) =>
                                `${baseClass} ${isActive ? "bg-blue-50 text-blue-600" : "hover:bg-gray-100"}`
                            }
                        >
                            <FaSchool /> Lớp học của tôi
                        </NavLink>
                        <NavLink
                            to={PATHS.STUDENT_CHECKIN}
                            className={({ isActive }) =>
                                `${baseClass} ${isActive ? "bg-blue-50 text-blue-600" : "hover:bg-gray-100"}`
                            }
                        >
                            <FaClipboardCheck /> Điểm danh
                        </NavLink>
                        <NavLink
                            to={PATHS.UPDATE_FACE}
                            className={({ isActive }) =>
                                `${baseClass} ${isActive ? "bg-blue-50 text-blue-600" : "hover:bg-gray-100"}`
                            }
                        >
                            <FaCamera /> Cập nhật khuôn mặt
                        </NavLink>
                    </>
                )}

                <NavLink
                    to={PATHS.PROFILE}
                    className={({ isActive }) =>
                        `${baseClass} ${isActive ? "bg-blue-50 text-blue-600" : "hover:bg-gray-100"}`
                    }
                >
                    <FaUser /> Thông tin cá nhân
                </NavLink>

                {!isLecturer && !isStudent && (
                    <NavLink
                        to={PATHS.RESULTS}
                        className={({ isActive }) =>
                            `${baseClass} ${isActive ? "bg-blue-50 text-blue-600" : "hover:bg-gray-100"}`
                        }
                    >
                        <FaChartBar /> Kết quả điểm danh
                    </NavLink>
                )}
            </nav>

            <div className="p-4 border-t mt-auto">
                <LogoutButton />
            </div>
        </aside>
    );
};

export default Sidebar;
