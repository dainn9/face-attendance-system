import { FaSignOutAlt } from "react-icons/fa"
import { useLogout } from "../../../features/auth/hooks/auth.mutation";

const LogoutButton = () => {
    const { mutate: logout } = useLogout();

    const handleLogout = () => {
        logout();
    }

    return (
        <button onClick={handleLogout}
        className="w-full flex items-center justify-center gap-3 px-4 py-3 text-red-600 hover:bg-red-50 rounded-xl font-medium transition cursor-pointer">
            <FaSignOutAlt />
            <span>Đăng xuất</span>
        </button>
    );
}

export default LogoutButton