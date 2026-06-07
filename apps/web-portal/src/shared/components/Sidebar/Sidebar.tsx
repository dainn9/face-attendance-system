import { FaChartBar, FaUser } from "react-icons/fa"
import { NavLink} from "react-router-dom"
import LogoutButton from "../LogoutButton/LogoutButton";

const Sidebar = () => {
    const baseClass = "flex items-center gap-3 px-4 py-3 rounded-xl mt-1 font-medium";

    return (
        <aside className="w-64 bg-white shadow-xl flex flex-col">
            <div className="p-6 border-b">
                <h1 className="text-2xl font-bold text-blue-600">LabGuard</h1>
                <p className="text-sm text-gray-500">Giám sát thi phòng lab</p>
            </div>

            <nav className="p-4 flex-1">
                <NavLink
                to="/profile"
                className={({isActive}) => `${baseClass} ${isActive ? "bg-blue-50 text-blue-600" : "hover:bg-gray-100"}`}
                >
                    <FaUser /> Hồ sơ cá nhân
                </NavLink>
            
                <NavLink
                to="/results"
                className={({isActive}) => `${baseClass} ${isActive ? "bg-blue-50 text-blue-600" : "hover:bg-gray-100"}`}
                >
                    <FaChartBar />Kết quả
                </NavLink>
            </nav>

            <div className="p-4 border-t mt-auto">
                <LogoutButton/>
            </div>
        </aside>
    )
}

export default Sidebar