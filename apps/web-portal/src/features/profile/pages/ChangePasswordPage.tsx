import { useState } from "react";
import { FaArrowLeft, FaCheck, FaKey, FaLock, FaSpinner } from "react-icons/fa";
import { Link, useNavigate } from "react-router-dom";
import { useChangePassword } from "../../auth/hooks/auth.mutation";
import { ApiError, ValidationError } from "../../../shared/api/errors";
import { toastEmitter } from "../../../shared/utils/toastEmitter";

const initialFormData = {
    currentPassword: "",
    newPassword: "",
    confirmPassword: "",
};

const getValidationMessage = (
    error: unknown,
    field: keyof typeof initialFormData,
) => {
    if (!(error instanceof ValidationError)) return null;

    return error.get(field)?.[0] ?? null;
};

const ChangePasswordPage = () => {
    const navigate = useNavigate();
    const changePasswordMutation = useChangePassword();
    const [formData, setFormData] = useState(initialFormData);
    const [clientError, setClientError] = useState("");

    const currentPasswordError = getValidationMessage(
        changePasswordMutation.error,
        "currentPassword",
    );
    const newPasswordError = getValidationMessage(
        changePasswordMutation.error,
        "newPassword",
    );
    const confirmPasswordError = getValidationMessage(
        changePasswordMutation.error,
        "confirmPassword",
    );
    const generalError =
        changePasswordMutation.error instanceof ApiError
            ? changePasswordMutation.error.message
            : "";

    const handleChange =
        (field: keyof typeof formData) =>
        (event: React.ChangeEvent<HTMLInputElement>) => {
            setClientError("");
            changePasswordMutation.reset();
            setFormData((current) => ({
                ...current,
                [field]: event.target.value,
            }));
        };

    const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        setClientError("");

        if (formData.newPassword !== formData.confirmPassword) {
            setClientError("Mật khẩu mới nhập lại không khớp.");
            return;
        }

        changePasswordMutation.mutate(formData, {
            onSuccess: () => {
                toastEmitter.success("Đổi mật khẩu thành công.");
                setFormData(initialFormData);
                navigate("/profile");
            },
        });
    };

    return (
        <div className="mx-auto max-w-3xl space-y-5">
            <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
                <div>
                    <Link
                        to="/profile"
                        className="inline-flex items-center gap-2 text-sm font-medium text-gray-500 hover:text-gray-900"
                    >
                        <FaArrowLeft />
                        Quay lại hồ sơ
                    </Link>
                    <h1 className="mt-3 text-xl font-semibold text-gray-900">
                        Đổi mật khẩu
                    </h1>
                    <p className="mt-1 text-sm text-gray-500">
                        Cập nhật mật khẩu đăng nhập cho tài khoản của bạn.
                    </p>
                </div>
                <span className="inline-flex size-12 items-center justify-center rounded-lg bg-blue-50 text-blue-600">
                    <FaKey />
                </span>
            </div>

            <form
                onSubmit={handleSubmit}
                className="overflow-hidden rounded-lg bg-white shadow-sm"
            >
                <div className="border-b border-gray-100 px-5 py-4">
                    <h2 className="text-sm font-semibold text-gray-900">
                        Thông tin mật khẩu
                    </h2>
                </div>

                <div className="space-y-4 px-5 py-5">
                    {(clientError || generalError) && (
                        <div className="rounded-md border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
                            {clientError || generalError}
                        </div>
                    )}

                    <div>
                        <label
                            htmlFor="currentPassword"
                            className="block text-sm font-medium text-gray-700"
                        >
                            Mật khẩu hiện tại
                        </label>
                        <div className="relative mt-2">
                            <FaLock className="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
                            <input
                                id="currentPassword"
                                type="password"
                                value={formData.currentPassword}
                                onChange={handleChange("currentPassword")}
                                autoComplete="current-password"
                                className="block min-h-11 w-full rounded-md border border-gray-300 bg-white py-2 pl-10 pr-3 text-sm text-gray-900 outline-none transition focus:border-blue-500 focus:ring-2 focus:ring-blue-100"
                                required
                            />
                        </div>
                        {currentPasswordError && (
                            <p className="mt-1 text-sm text-red-600">
                                {currentPasswordError}
                            </p>
                        )}
                    </div>

                    <div>
                        <label
                            htmlFor="newPassword"
                            className="block text-sm font-medium text-gray-700"
                        >
                            Mật khẩu mới
                        </label>
                        <div className="relative mt-2">
                            <FaLock className="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
                            <input
                                id="newPassword"
                                type="password"
                                value={formData.newPassword}
                                onChange={handleChange("newPassword")}
                                autoComplete="new-password"
                                className="block min-h-11 w-full rounded-md border border-gray-300 bg-white py-2 pl-10 pr-3 text-sm text-gray-900 outline-none transition focus:border-blue-500 focus:ring-2 focus:ring-blue-100"
                                required
                            />
                        </div>
                        {newPasswordError && (
                            <p className="mt-1 text-sm text-red-600">
                                {newPasswordError}
                            </p>
                        )}
                    </div>

                    <div>
                        <label
                            htmlFor="confirmPassword"
                            className="block text-sm font-medium text-gray-700"
                        >
                            Nhập lại mật khẩu mới
                        </label>
                        <div className="relative mt-2">
                            <FaCheck className="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
                            <input
                                id="confirmPassword"
                                type="password"
                                value={formData.confirmPassword}
                                onChange={handleChange("confirmPassword")}
                                autoComplete="new-password"
                                className="block min-h-11 w-full rounded-md border border-gray-300 bg-white py-2 pl-10 pr-3 text-sm text-gray-900 outline-none transition focus:border-blue-500 focus:ring-2 focus:ring-blue-100"
                                required
                            />
                        </div>
                        {confirmPasswordError && (
                            <p className="mt-1 text-sm text-red-600">
                                {confirmPasswordError}
                            </p>
                        )}
                    </div>
                </div>

                <div className="flex flex-col-reverse gap-3 border-t border-gray-100 px-5 py-4 sm:flex-row sm:justify-end">
                    <Link
                        to="/profile"
                        className="inline-flex min-h-11 items-center justify-center rounded-md border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700 transition hover:bg-gray-50"
                    >
                        Hủy
                    </Link>
                    <button
                        type="submit"
                        disabled={changePasswordMutation.isPending}
                        className="inline-flex min-h-11 items-center justify-center gap-2 rounded-md bg-blue-600 px-5 py-2 text-sm font-semibold text-white transition hover:bg-blue-700 disabled:cursor-not-allowed disabled:bg-blue-300"
                    >
                        {changePasswordMutation.isPending && (
                            <FaSpinner className="animate-spin" />
                        )}
                        Đổi mật khẩu
                    </button>
                </div>
            </form>
        </div>
    );
};

export default ChangePasswordPage;
