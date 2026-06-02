import { FaHome, FaRegBuilding, FaUserFriends } from "react-icons/fa";
import { NavLink } from "react-router-dom";
import LogoutButton from "../LogoutButton/LogoutButton";

const Sidebar = () => {
    const baseClass =
        "flex items-center gap-3 px-4 py-3 rounded-xl mt-1 font-medium transition-colors";

    return (
        <aside className="w-64 bg-white shadow-xl flex flex-col">
            {/* Header */}
            <div className="p-6 border-b">
                <h1 className="text-2xl font-bold text-blue-600">FaceAttend</h1>
                <p className="text-sm text-gray-500">Hệ thống điểm danh</p>
            </div>

            {/* Menu */}
            <nav className="p-4 flex-1">
                {/* Tổng quan */}
                <div className="text-[10px] text-gray-400 px-4 pb-1 uppercase tracking-wide">
                    Tổng quan
                </div>

                <NavLink
                    to="/"
                    end
                    className={({ isActive }) =>
                        `${baseClass} ${
                            isActive
                                ? "bg-blue-50 text-blue-600"
                                : "hover:bg-gray-100 text-gray-700"
                        }`
                    }
                >
                    <FaHome />
                    Dashboard
                </NavLink>

                {/* Quản lý */}
                <div className="text-[10px] text-gray-400 px-4 pt-6 pb-1 uppercase tracking-wide">
                    Quản lý
                </div>

                <NavLink
                    to="/users"
                    className={({ isActive }) =>
                        `${baseClass} ${
                            isActive
                                ? "bg-blue-50 text-blue-600"
                                : "hover:bg-gray-100 text-gray-700"
                        }`
                    }
                >
                    <FaUserFriends />
                    Người dùng
                </NavLink>

                <NavLink
                    to="/faculties"
                    className={({ isActive }) =>
                        `${baseClass} ${
                            isActive
                                ? "bg-blue-50 text-blue-600"
                                : "hover:bg-gray-100 text-gray-700"
                        }`
                    }
                >
                    <FaRegBuilding />
                    Khoa / Ngành
                </NavLink>
            </nav>

            {/* Footer */}
            <div className="p-4 border-t">
                <LogoutButton />
            </div>
        </aside>
    );
};

export default Sidebar;
