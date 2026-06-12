import { useState } from "react";
import { FaBars } from "react-icons/fa";
import { Outlet } from "react-router-dom";
import Sidebar from "../Sidebar/Sidebar";

const Layout = () => {
    const [isMobileSidebarOpen, setIsMobileSidebarOpen] = useState(false);

    return (
        <div className="flex h-screen bg-gray-100">
            <Sidebar className="hidden lg:flex" />

            {isMobileSidebarOpen && (
                <div className="fixed inset-0 z-50 lg:hidden">
                    <button
                        type="button"
                        className="absolute inset-0 bg-black/40"
                        aria-label="Dong menu"
                        onClick={() => setIsMobileSidebarOpen(false)}
                    />
                    <div className="relative h-full w-64">
                        <Sidebar
                            className="h-full"
                            onNavigate={() => setIsMobileSidebarOpen(false)}
                        />
                    </div>
                </div>
            )}

            <main className="flex-1 overflow-y-auto bg-gray-100">
                <div className="sticky top-0 z-30 flex h-14 items-center justify-between border-b border-gray-200 bg-white px-4 shadow-sm lg:hidden">
                    <button
                        type="button"
                        className="inline-flex size-10 items-center justify-center rounded-md border border-gray-200 text-gray-700"
                        aria-label="Mo menu"
                        onClick={() => setIsMobileSidebarOpen(true)}
                    >
                        <FaBars />
                    </button>
                    <span className="text-base font-semibold text-blue-600">
                        FaceAttend
                    </span>
                    <span className="size-10" aria-hidden="true" />
                </div>

                <div className="p-4 sm:p-5 lg:p-6">
                    <Outlet />
                </div>
            </main>
        </div>
    );
};

export default Layout;
