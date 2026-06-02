import {
    FiChevronLeft,
    FiChevronRight,
    FiEdit2,
    FiEye,
    FiPlus,
    FiTrash2,
} from "react-icons/fi";
import { getInitials } from "../../../shared/utils/getInitials";
import StatusBadge from "../components/StatusBadge";
import RoleBadge from "../components/RoleBadge";
import PagerButton from "../components/PagerButton";
import FilterSelect from "../components/FilterSelect";
import { useUsers } from "../hooks/user.query";
import Spinner from "../../../shared/components/Spinner/Spinner";
import type { UserRole } from "../types/user.types";
import { useState } from "react";
import { useFacultyLookup } from "../hooks/lookup.query";

const defaultPagedResult = {
    items: [],
    totalCount: 0,
    page: 1,
    pageSize: 10,
    totalPages: 0,
    hasNextPage: false,
    hasPreviousPage: false,
};

const roleOptions = ["Admin", "Lecturer", "Student"].map((role) => ({
    label: role,
    value: role,
}));

const UserPage = () => {
    const [page, setPage] = useState(1);
    const [searchQuery, setSearchQuery] = useState("");
    const [role, setRole] = useState<UserRole>();
    const [facultyId, setFacultyId] = useState<string | undefined>();

    const { data = defaultPagedResult, isLoading } = useUsers({
        page,
        pageSize: 10,
        searchQuery: searchQuery || undefined,
        role: role || undefined,
        facultyId: facultyId || undefined,
    });

    const { data: faculties = [] } = useFacultyLookup();

    if (isLoading && !data.items.length) {
        return (
            <div className="flex min-h-[60vh] items-center justify-center">
                <Spinner className="size-10 text-blue-600" />
            </div>
        );
    }
    return (
        <div className="p-6">
            <div className="mb-5 flex items-center justify-between">
                <h1 className="text-2xl font-bold text-gray-900">Người dùng</h1>

                <button className="flex items-center gap-2 rounded-xl bg-blue-600 px-4 py-2 text-sm font-semibold text-white hover:bg-blue-700">
                    <FiPlus size={16} />
                    Tạo người dùng
                </button>
            </div>

            <div className="mb-4 flex flex-wrap gap-3">
                <input
                    placeholder="Tìm tên, email, mã số..."
                    value={searchQuery}
                    onChange={(e) => {
                        setSearchQuery(e.target.value);
                        setPage(1);
                    }}
                    className="w-64 rounded-xl border border-gray-200 bg-white px-3 py-2 text-sm outline-none focus:border-blue-500"
                />

                <FilterSelect
                    placeholder="Tất cả role"
                    onSelect={(value) => {
                        setRole(value as UserRole);
                        setPage(1);
                    }}
                    options={roleOptions}
                />

                {/* <FilterSelect
                    placeholder="Tất cả trạng thái"
                    value={facultyId ?? ""}
                    onSelect={(value) => {
                        setFacultyId(value);
                        setPage(1);
                    }}
                    options={["Active", "Inactive"]}
                /> */}

                <FilterSelect
                    placeholder="Tất cả khoa"
                    value={facultyId ?? ""}
                    onSelect={(value) => {
                        setFacultyId(value || undefined);

                        setPage(1);
                    }}
                    options={faculties.map((f) => ({
                        label: f.name,
                        value: f.id,
                    }))}
                />
            </div>

            <div className="overflow-hidden rounded-2xl border border-gray-200 bg-white shadow-sm">
                <table className="w-full table-fixed text-sm">
                    <thead>
                        <tr className="border-b border-gray-200 bg-gray-50 text-left text-xs font-semibold text-gray-500">
                            <th className="w-[28%] px-4 py-3">Người dùng</th>
                            <th className="w-[24%] px-4 py-3">Email</th>
                            <th className="w-[12%] px-4 py-3">Role</th>
                            <th className="w-[14%] px-4 py-3">Mã số</th>
                            <th className="w-[12%] px-4 py-3">Trạng thái</th>
                            <th className="w-[10%] px-4 py-3"></th>
                        </tr>
                    </thead>

                    <tbody className="divide-y divide-gray-200">
                        {data.items.map((user) => (
                            <tr key={user.userId} className="hover:bg-gray-50">
                                <td className="px-4 py-3">
                                    <div className="flex items-center gap-3">
                                        <div className="flex size-9 items-center justify-center rounded-full bg-blue-50 text-xs font-bold text-blue-600">
                                            {getInitials(user.fullName)}
                                        </div>

                                        <div>
                                            <div className="font-medium text-gray-900">
                                                {user.fullName}
                                            </div>
                                            <div className="text-xs text-gray-500">
                                                {user.facultyName}
                                            </div>
                                        </div>
                                    </div>
                                </td>

                                <td className="px-4 py-3 text-gray-500">
                                    {user.email}
                                </td>

                                <td className="px-4 py-3">
                                    <RoleBadge
                                        role={user.roleName as UserRole}
                                    />
                                </td>

                                <td className="px-4 py-3 text-xs text-gray-500">
                                    {user.userCode ?? "—"}
                                </td>

                                <td className="px-4 py-3">
                                    <StatusBadge isActive={user.isActive} />
                                </td>

                                <td className="px-4 py-3">
                                    <div className="flex items-center gap-3 text-gray-400">
                                        <button className="hover:text-blue-600">
                                            <FiEye size={16} />
                                        </button>
                                        <button className="hover:text-blue-600">
                                            <FiEdit2 size={16} />
                                        </button>
                                        <button className="hover:text-red-600">
                                            <FiTrash2 size={16} />
                                        </button>
                                    </div>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>

            <div className="mt-4 flex items-center justify-between">
                <div className="text-sm text-gray-500">
                    Hiển thị {(data.page - 1) * data.pageSize + 1}–
                    {Math.min(data.page * data.pageSize, data.totalCount)} /{" "}
                    {data.totalCount} người dùng
                </div>

                <div className="flex items-center gap-1">
                    <PagerButton
                        disabled={!data.hasPreviousPage}
                        onClick={() => setPage(data.page - 1)}
                    >
                        <FiChevronLeft size={14} />
                    </PagerButton>

                    {Array.from(
                        { length: data.totalPages },
                        (_, i) => i + 1,
                    ).map((pageNumber) => (
                        <PagerButton
                            key={pageNumber}
                            active={pageNumber === data.page}
                            onClick={() => setPage(pageNumber)}
                        >
                            {pageNumber}
                        </PagerButton>
                    ))}

                    <PagerButton
                        disabled={!data.hasNextPage}
                        onClick={() => setPage(data.page + 1)}
                    >
                        <FiChevronRight size={14} />
                    </PagerButton>
                </div>
            </div>
        </div>
    );
};

export default UserPage;
