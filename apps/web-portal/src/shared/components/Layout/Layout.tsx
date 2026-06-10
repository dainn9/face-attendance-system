import { Outlet } from "react-router-dom";
import Sidebar from "../Sidebar/Sidebar";

const Layout = () => {
    return (
        <div className="flex h-screen">
            <Sidebar />
            <main className="flex-1 overflow-y-auto bg-gray-100 p-6">
                <Outlet />
            </main>
        </div>
    );
};

export default Layout;
