import { useEffect, useState } from "react";
import { FiX } from "react-icons/fi";
import { ValidationError } from "../../../shared/api/errors";
import Spinner from "../../../shared/components/Spinner/Spinner";
import Button from "../../../shared/components/Button/Button";

type Props = {
    isOpen: boolean;
    mode: "create" | "edit";
    initialData?: {
        name: string;
        code: string;
    };
    isPending?: boolean;
    error?: unknown;
    onClose: () => void;
    onSubmit: (data: { name: string; code: string }) => void;
};

const FacultyFormModal = ({
    isOpen,
    mode,
    initialData,
    isPending,
    error,
    onClose,
    onSubmit,
}: Props) => {
    const title = mode === "create" ? "Thêm khoa mới" : "Cập nhật khoa";
    const submitText = mode === "create" ? "Thêm khoa" : "Lưu thay đổi";

    const initialState = {
        name: "",
        code: "",
        // iconName: "cpu",
    };

    const [formData, setFormData] = useState(initialState);

    useEffect(() => {
        setFormData(initialData ?? initialState);
    }, [initialData, isOpen]);

    if (!isOpen) return null;

    const updateField = (field: keyof typeof formData, value: string) => {
        setFormData((prev) => ({
            ...prev,
            [field]: value,
        }));
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        onSubmit(formData);
    };

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 px-4">
            <div className="w-full max-w-lg overflow-hidden rounded-2xl bg-white shadow-2xl">
                <div className="flex items-center justify-between border-b border-gray-100 px-6 py-4">
                    <div>
                        <h2 className="text-lg font-bold text-gray-900">
                            {title}
                        </h2>
                        <p className="text-sm text-gray-500">
                            Tạo thông tin khoa để quản lý ngành học.
                        </p>
                    </div>

                    <button
                        type="button"
                        onClick={onClose}
                        className="rounded-lg p-2 text-gray-400 hover:bg-gray-100 hover:text-gray-700"
                    >
                        <FiX size={20} />
                    </button>
                </div>

                <form onSubmit={handleSubmit}>
                    <div className="space-y-5 px-6 py-5">
                        {error instanceof Error &&
                            !(error instanceof ValidationError) && (
                                <div className="rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-600">
                                    {error.message}
                                </div>
                            )}

                        <label className="block space-y-2">
                            <span className="text-sm font-semibold text-gray-700">
                                Tên khoa <span className="text-red-500">*</span>
                            </span>

                            <input
                                value={formData.name}
                                onChange={(e) =>
                                    updateField("name", e.target.value)
                                }
                                placeholder="VD: Công nghệ thông tin"
                                className={`h-11 w-full rounded-xl border px-4 text-sm outline-none ${
                                    error instanceof ValidationError &&
                                    error.get("name")
                                        ? "border-red-400 bg-red-50 focus:border-red-500"
                                        : "border-gray-200 bg-gray-50 focus:border-blue-400 focus:bg-white"
                                }`}
                            />

                            {error instanceof ValidationError &&
                                error.get("name") && (
                                    <p className="text-xs text-red-500">
                                        {error.get("name")}
                                    </p>
                                )}
                        </label>

                        <label className="block space-y-2">
                            <span className="text-sm font-semibold text-gray-700">
                                Mã khoa <span className="text-red-500">*</span>
                            </span>

                            <input
                                value={formData.code}
                                onChange={(e) =>
                                    updateField(
                                        "code",
                                        e.target.value.toUpperCase(),
                                    )
                                }
                                placeholder="VD: CNTT"
                                className={`h-11 w-full rounded-xl border px-4 text-sm uppercase outline-none ${
                                    error instanceof ValidationError &&
                                    error.get("code")
                                        ? "border-red-400 bg-red-50 focus:border-red-500"
                                        : "border-gray-200 bg-gray-50 focus:border-blue-400 focus:bg-white"
                                }`}
                            />

                            {error instanceof ValidationError &&
                                error.get("code") && (
                                    <p className="text-xs text-red-500">
                                        {error.get("code")}
                                    </p>
                                )}

                            <p className="text-xs text-gray-400">
                                Mã khoa phải là duy nhất trong hệ thống.
                            </p>
                        </label>
                        {/* 
                        <div>
                            <div className="mb-3 text-sm font-semibold text-gray-700">
                                Icon hiển thị
                            </div>

                            <div className="grid grid-cols-2 gap-3 sm:grid-cols-3">
                                {iconOptions.map(({ value, label, Icon }) => {
                                    const isActive =
                                        formData.iconName === value;

                                    return (
                                        <button
                                            key={value}
                                            type="button"
                                            onClick={() =>
                                                updateField("iconName", value)
                                            }
                                            className={`flex items-center gap-3 rounded-xl border p-3 text-left transition ${
                                                isActive
                                                    ? "border-blue-500 bg-blue-50 text-blue-600"
                                                    : "border-gray-200 bg-gray-50 text-gray-500 hover:border-blue-300 hover:bg-white"
                                            }`}
                                        >
                                            <Icon size={20} />
                                            <span className="text-sm font-semibold">
                                                {label}
                                            </span>
                                        </button>
                                    );
                                })}
                            </div>
                        </div> */}
                    </div>

                    <div className="flex justify-end gap-3 border-t border-gray-100 px-6 py-4">
                        <Button variant="secondary" onClick={onClose}>
                            Hủy
                        </Button>

                        <Button
                            variant="primary"
                            type="submit"
                            disabled={isPending}
                        >
                            {isPending && <Spinner />}
                            {submitText}
                        </Button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default FacultyFormModal;
