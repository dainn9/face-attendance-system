import { useEffect, useState } from "react";
import { useCreateMajor } from "../hooks/faculty.mutation";
import Spinner from "../../../shared/components/Spinner/Spinner";
import { FiCpu, FiX } from "react-icons/fi";
import { ValidationError } from "../../../shared/api/errors";
import Button from "../../../shared/components/Button/Button";

type Props = {
    facultyId: string;
    name: string;
    code: string;
    mode: "create" | "edit";
    initialData?: {
        name: string;
        code: string;
    };
    isPending?: boolean;
    error?: unknown;
    isOpen: boolean;
    onClose: () => void;
    onSubmit: (data: { name: string; code: string }) => void;
};

const MajorFormModal = ({
    facultyId,
    name,
    code,
    mode,
    initialData,
    isPending,
    error,
    isOpen,
    onClose,
    onSubmit,
}: Props) => {
    const title = mode === "create" ? "Thêm ngành mới" : "Cập nhật ngành";
    const submitText = mode === "create" ? "Thêm ngành" : "Lưu thay đổi";

    const initialState = {
        name: "",
        code: "",
    };
    const [formData, setFormData] = useState(initialState);

    useEffect(() => {
        setFormData(initialData ?? initialState);
    }, [initialData, isOpen]);

    // const { mutate, isPending, error, reset } = useCreateMajor(facultyId);

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
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
            <form
                onSubmit={handleSubmit}
                className="w-105 overflow-hidden rounded-2xl bg-white shadow-2xl"
            >
                <div className="flex items-center justify-between border-b border-gray-100 px-5 py-4">
                    <h2 className="text-base font-semibold text-gray-900">
                        {title}
                    </h2>

                    <button
                        type="button"
                        onClick={onClose}
                        className="flex size-8 items-center justify-center rounded-lg bg-gray-100 text-gray-500 hover:bg-gray-200"
                    >
                        <FiX size={18} />
                    </button>
                </div>

                <div className="p-5">
                    <div className="mb-5 flex items-center gap-3 rounded-xl bg-gray-50 p-3">
                        <div className="flex size-10 items-center justify-center rounded-xl bg-blue-50 text-blue-600">
                            <FiCpu size={20} />
                        </div>

                        <div>
                            <div className="text-sm font-semibold text-gray-900">
                                {name}
                            </div>
                            <div className="text-xs text-gray-500">
                                Mã khoa: {code}
                            </div>
                        </div>
                    </div>

                    {error instanceof Error &&
                        !(error instanceof ValidationError) && (
                            <div className="rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-600">
                                {error.message}
                            </div>
                        )}

                    <div className="mb-4">
                        <label className="mb-1 block text-sm font-medium text-gray-700">
                            Tên ngành <span className="text-red-500">*</span>
                        </label>
                        <input
                            value={formData.name}
                            onChange={(e) =>
                                updateField("name", e.target.value)
                            }
                            type="text"
                            placeholder="VD: Khoa học máy tính"
                            className={`w-full rounded-xl border px-3 py-2 text-sm outline-none ${
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
                    </div>

                    <div>
                        <label className="mb-1 block text-sm font-medium text-gray-700">
                            Mã ngành <span className="text-red-500">*</span>
                        </label>
                        <input
                            value={formData.code}
                            onChange={(e) =>
                                updateField("code", e.target.value)
                            }
                            type="text"
                            placeholder="VD: KHMT"
                            className={`w-full rounded-xl border px-3 py-2 text-sm outline-none ${
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
                    </div>
                </div>

                <div className="flex justify-end gap-3 border-t border-gray-100 px-5 py-4">
                    <Button variant="secondary" onClick={onClose}>
                        Hủy
                    </Button>

                    <Button
                        variant="primary"
                        type="submit"
                        disabled={isPending}
                    >
                        {isPending && <Spinner className="size-4 text-white" />}
                        {submitText}
                    </Button>
                </div>
            </form>
        </div>
    );
};

export default MajorFormModal;
