import { useState } from "react";
import {
    FiArrowLeft,
    FiBookOpen,
    FiBriefcase,
    FiCpu,
    FiGlobe,
    FiSave,
    FiSettings,
} from "react-icons/fi";
import { Link } from "react-router-dom";
import { useCreateFaculty } from "../hooks/faculty.mutation";
import { ValidationError } from "../../../shared/api/errors";

const iconOptions = [
    { value: "cpu", label: "Công nghệ", Icon: FiCpu },
    { value: "book", label: "Giáo dục", Icon: FiBookOpen },
    { value: "briefcase", label: "Kinh tế", Icon: FiBriefcase },
    { value: "globe", label: "Ngoại ngữ", Icon: FiGlobe },
    { value: "settings", label: "Kỹ thuật", Icon: FiSettings },
];

const FacultyCreatePage = () => {
    const { mutate, isPending, error } = useCreateFaculty();

    const [formData, setFormData] = useState({
        name: "",
        code: "",
        iconName: "cpu",
    });

    const updateField = (field: keyof typeof formData, value: string) => {
        setFormData((prev) => ({
            ...prev,
            [field]: value,
        }));
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        mutate(formData);
    };

    return (
        <div className="p-6">
            <Link
                to="/faculties"
                className="mb-5 flex items-center gap-2 text-sm font-semibold text-gray-500 hover:text-blue-600"
            >
                <FiArrowLeft />
                Quay lại
            </Link>

            <div className="mx-auto max-w-3xl rounded-2xl border border-gray-200 bg-white p-6 shadow-sm">
                <div className="mb-6">
                    <h1 className="text-2xl font-bold text-gray-900">
                        Thêm khoa mới
                    </h1>
                    <p className="mt-1 text-sm text-gray-500">
                        Tạo thông tin khoa để quản lý ngành học, giảng viên và
                        sinh viên.
                    </p>
                </div>

                {error && !(error instanceof ValidationError) && (
                    <div className="mb-4 rounded border border-red-400 bg-red-100 px-4 py-3 text-red-700">
                        {error.message}
                    </div>
                )}

                <form onSubmit={handleSubmit} className="space-y-6">
                    <label className="block space-y-2">
                        <span className="text-sm font-semibold text-gray-700">
                            Tên khoa
                        </span>
                        <input
                            value={formData.name}
                            onChange={(e) =>
                                updateField("name", e.target.value)
                            }
                            placeholder="Ví dụ: Công nghệ thông tin"
                            className={`
                                h-11
                                w-full
                                rounded-xl
                                border
                                px-4
                                text-sm
                                outline-none
                                ${
                                    error instanceof ValidationError &&
                                    error.get("code")
                                        ? `
                                            border-red-400
                                            bg-red-50
                                            focus:border-red-500
                                        `
                                        : `
                                            border-gray-200
                                            bg-gray-50
                                            focus:border-blue-400
                                            focus:bg-white
                                        `
                                }
                            `}
                        />
                        {error instanceof ValidationError &&
                            error.get("name") && (
                                <p className="mt-1 text-xs text-red-500">
                                    {error.get("name")}
                                </p>
                            )}
                    </label>

                    <label className="block space-y-2">
                        <span className="text-sm font-semibold text-gray-700">
                            Mã khoa
                        </span>
                        <input
                            value={formData.code}
                            onChange={(e) =>
                                updateField(
                                    "code",
                                    e.target.value.toUpperCase(),
                                )
                            }
                            placeholder="Ví dụ: CNTT"
                            className={`
                                h-11
                                w-full
                                rounded-xl
                                border
                                px-4
                                text-sm
                                uppercase
                                outline-none
                                ${
                                    error instanceof ValidationError &&
                                    error.get("code")
                                        ? `
                                            border-red-400
                                            bg-red-50
                                            focus:border-red-500
                                        `
                                        : `
                                            border-gray-200
                                            bg-gray-50
                                            focus:border-blue-400
                                            focus:bg-white
                                        `
                                }
                            `}
                        />
                        {error instanceof ValidationError &&
                            error.get("code") && (
                                <p className="mt-1 text-xs text-red-500">
                                    {error.get("code")}
                                </p>
                            )}
                    </label>

                    <div>
                        <div className="mb-3 text-sm font-semibold text-gray-700">
                            Icon hiển thị
                        </div>

                        <div className="grid grid-cols-2 gap-3 sm:grid-cols-3">
                            {iconOptions.map(({ value, label, Icon }) => {
                                const isActive = formData.iconName === value;

                                return (
                                    <button
                                        key={value}
                                        type="button"
                                        onClick={() =>
                                            updateField("iconName", value)
                                        }
                                        className={`flex items-center gap-3 rounded-xl border p-4 text-left transition ${
                                            isActive
                                                ? "border-blue-500 bg-blue-50 text-blue-600"
                                                : "border-gray-200 bg-gray-50 text-gray-500 hover:border-blue-300 hover:bg-white"
                                        }`}
                                    >
                                        <Icon size={22} />
                                        <span className="text-sm font-semibold">
                                            {label}
                                        </span>
                                    </button>
                                );
                            })}
                        </div>
                    </div>

                    <div className="flex justify-end gap-3 border-t border-gray-100 pt-5">
                        <Link
                            to="/faculties"
                            className="rounded-xl border border-gray-200 px-5 py-2.5 text-sm font-semibold text-gray-600 hover:bg-gray-50"
                        >
                            Hủy
                        </Link>

                        <button
                            type="submit"
                            className="flex items-center gap-2 rounded-xl bg-blue-600 px-5 py-2.5 text-sm font-semibold text-white hover:bg-blue-700"
                            disabled={isPending}
                        >
                            {isPending ? (
                                <svg
                                    className="w-5 h-5 text-white animate-spin"
                                    xmlns="http://www.w3.org/2000/svg"
                                    fill="none"
                                    viewBox="0 0 24 24"
                                >
                                    <circle
                                        className="opacity-25"
                                        cx="12"
                                        cy="12"
                                        r="10"
                                        stroke="currentColor"
                                        strokeWidth="4"
                                    ></circle>
                                    <path
                                        className="opacity-75"
                                        fill="currentColor"
                                        d="M4 12a8 8 0 018-8v8z"
                                    ></path>
                                </svg>
                            ) : (
                                <>
                                    <FiSave />
                                    Tạo khoa
                                </>
                            )}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default FacultyCreatePage;
