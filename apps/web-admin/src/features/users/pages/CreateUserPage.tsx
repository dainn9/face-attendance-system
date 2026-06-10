import { useState } from "react";
import { FiChevronRight, FiShield, FiUser, FiUserCheck } from "react-icons/fi";
import { Link } from "react-router-dom";
import InputField from "../components/InputField";
import SectionTitle from "../components/SectionTitle";
import RoleButton from "../components/RoleButton";
import FormSelect from "../components/FormSelect";
import { UserRoleValue } from "../types/user.types";
import type { CreateUserRequest, UserRole } from "../types/user.types";
import { useFacultyLookup, useMajorLookup } from "../hooks/lookup.query";
import { useCreateUser } from "../hooks/user.mutation";
import { ValidationError } from "../../../shared/api/errors";

const genderOptions = [
    { label: "Nam", value: 1 },
    { label: "Nữ", value: 2 },
];

type CreateUserFormData = Omit<
    CreateUserRequest,
    "userCode" | "gender" | "userRole" | "facultyId" | "majorId"
> & {
    userCode: string;
    userRole: UserRole;
    gender: number | null;
    facultyId: string;
    majorId: string;
};

const initialState: CreateUserFormData = {
    userCode: "",
    email: "",
    password: "",
    userRole: "Student",
    fullName: "",
    gender: null,
    dateOfBirth: "",
    facultyId: "",
    majorId: "",
};

const generatePasswordFromDateOfBirth = (dateOfBirth: string) => {
    if (!dateOfBirth) {
        return "";
    }

    const [year, month, day] = dateOfBirth.split("-");

    if (!year || !month || !day) {
        return "";
    }

    return `${day}${month}${year}`;
};

const roleDetails = {
    Student: {
        title: "Thông tin sinh viên",
        codeLabel: "Mã sinh viên",
        codePlaceholder: "SV2024001",
    },
    Lecturer: {
        title: "Thông tin giảng viên",
        codeLabel: "Mã giảng viên",
        codePlaceholder: "GV20240001",
    },
} satisfies Record<
    Exclude<UserRole, "Admin">,
    {
        title: string;
        codeLabel: string;
        codePlaceholder: string;
    }
>;

const CreateUserPage = () => {
    const [formData, setFormData] = useState<CreateUserFormData>(initialState);
    const [useDateOfBirthPassword, setUseDateOfBirthPassword] = useState(true);
    const [dateOfBirthPasswordError, setDateOfBirthPasswordError] =
        useState("");

    const { data: faculties = [] } = useFacultyLookup();

    const selectedRoleDetails =
        formData.userRole === "Admin" ? null : roleDetails[formData.userRole];

    const { data: majors = [], isFetching: isLoadingMajors } = useMajorLookup(
        formData.userRole === "Student" ? formData.facultyId : undefined,
    );

    const { mutate: createUser, error, isPending } = useCreateUser();
    const generatedPassword = generatePasswordFromDateOfBirth(
        formData.dateOfBirth,
    );
    const displayedPassword = useDateOfBirthPassword
        ? generatedPassword
        : formData.password;

    const updateField = <K extends keyof CreateUserFormData>(
        field: K,
        value: CreateUserFormData[K],
    ) => {
        if (field === "dateOfBirth" && value) {
            setDateOfBirthPasswordError("");
        }

        setFormData((prev) => ({
            ...prev,
            [field]: value,
        }));
    };

    const changeRole = (role: UserRole) => {
        setFormData((prev) => ({
            ...prev,
            userRole: role,
            userCode: "",
            facultyId: "",
            majorId: "",
        }));
    };

    const changeFaculty = (facultyId: string) => {
        setFormData((prev) => ({
            ...prev,
            facultyId,
            majorId: "",
        }));
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        setDateOfBirthPasswordError("");

        if (useDateOfBirthPassword && !formData.dateOfBirth) {
            setDateOfBirthPasswordError(
                "Vui lòng chọn ngày sinh để tạo mật khẩu mặc định.",
            );
            return;
        }

        if (formData.gender === null) {
            return;
        }

        const finalPassword = useDateOfBirthPassword
            ? generatedPassword
            : formData.password;

        const payload: CreateUserRequest = {
            ...formData,
            userCode:
                formData.userRole === "Admin" ? null : formData.userCode,
            password: finalPassword,
            gender: formData.gender,
            userRole: UserRoleValue[formData.userRole],
            facultyId:
                formData.userRole === "Lecturer"
                    ? formData.facultyId || undefined
                    : undefined,
            majorId:
                formData.userRole === "Student"
                    ? formData.majorId || undefined
                    : undefined,
        };

        createUser(payload);
    };

    return (
        <div className="p-6">
            <div className="mb-4 flex items-center gap-2 text-sm text-gray-500">
                <Link to="/users" className="hover:text-blue-600">
                    Người dùng
                </Link>
                <FiChevronRight size={14} />
                <span className="font-medium text-gray-900">
                    Tạo người dùng
                </span>
            </div>

            <form
                onSubmit={handleSubmit}
                className="rounded-2xl border border-gray-200 bg-white p-6 shadow-sm"
            >
                <SectionTitle title="Thông tin tài khoản" />

                {error && !(error instanceof ValidationError) && (
                    <div className="rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-600">
                        {error.message}
                    </div>
                )}

                <div className="grid gap-4 md:grid-cols-2">
                    <div>
                        <InputField
                            required
                            label="Email"
                            type="email"
                            placeholder="example@school.edu"
                            value={formData.email}
                            onChange={(value) => updateField("email", value)}
                        />
                        {error instanceof ValidationError &&
                            error.get("email") && (
                                <p className="text-xs text-red-500">
                                    {error.get("email")}
                                </p>
                            )}
                    </div>
                    <div>
                        <InputField
                            required
                            label="Mật khẩu"
                            type="password"
                            placeholder="Tối thiểu 8 ký tự"
                            value={displayedPassword}
                            disabled={useDateOfBirthPassword}
                            onChange={(value) => updateField("password", value)}
                        />
                        {dateOfBirthPasswordError && (
                            <p className="text-xs text-red-500">
                                {dateOfBirthPasswordError}
                            </p>
                        )}
                        {error instanceof ValidationError &&
                            error.get("password") && (
                                <p className="text-xs text-red-500">
                                    {error.get("password")}
                                </p>
                            )}
                    </div>
                    <label className="mb-2 flex items-center gap-2 text-sm font-medium text-gray-600">
                        <input
                            type="checkbox"
                            checked={useDateOfBirthPassword}
                            onChange={(e) => {
                                setUseDateOfBirthPassword(e.target.checked);

                                if (!e.target.checked) {
                                    setDateOfBirthPasswordError("");
                                }
                            }}
                            className="h-4 w-4 rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                        />
                        <span>Sử dụng ngày sinh làm mật khẩu mặc định</span>
                    </label>
                </div>

                <SectionTitle title="Role" className="mt-2" />

                <div className="mb-5 grid gap-3 md:grid-cols-3">
                    <RoleButton
                        userRole="Student"
                        currentRole={formData.userRole}
                        icon={<FiUser size={18} />}
                        onClick={() => changeRole("Student")}
                    />

                    <RoleButton
                        userRole="Lecturer"
                        currentRole={formData.userRole}
                        icon={<FiUserCheck size={18} />}
                        onClick={() => changeRole("Lecturer")}
                    />

                    <RoleButton
                        userRole="Admin"
                        currentRole={formData.userRole}
                        icon={<FiShield size={18} />}
                        onClick={() => changeRole("Admin")}
                    />
                </div>
                {error instanceof ValidationError && error.get("userRole") && (
                    <p className="text-xs text-red-500">
                        {error.get("userRole")}
                    </p>
                )}

                <SectionTitle title="Thông tin cá nhân" />

                <div className="grid gap-4">
                    <InputField
                        required
                        label="Họ và tên"
                        placeholder="Nguyễn Văn A"
                        value={formData.fullName}
                        onChange={(value) => updateField("fullName", value)}
                    />
                    {error instanceof ValidationError &&
                        error.get("fullName") && (
                            <p className="text-xs text-red-500">
                                {error.get("fullName")}
                            </p>
                        )}
                </div>

                <div className="grid gap-4 md:grid-cols-2">
                    <div>
                        <InputField
                            label="Ngày sinh"
                            type="date"
                            value={formData.dateOfBirth}
                            onChange={(value) =>
                                updateField("dateOfBirth", value)
                            }
                        />
                        {error instanceof ValidationError &&
                            error.get("dateOfBirth") && (
                                <p className="text-xs text-red-500">
                                    {error.get("dateOfBirth")}
                                </p>
                            )}
                    </div>

                    {useDateOfBirthPassword && (
                        <div className="mb-4">
                            <label className="mb-1 block text-sm font-medium text-gray-600">
                                Mật khẩu mặc định
                            </label>
                            <div className="rounded-xl border border-gray-200 bg-gray-100 px-3 py-2 text-sm font-medium text-gray-700">
                                {generatedPassword || "Chọn ngày sinh"}
                            </div>
                        </div>
                    )}

                    <FormSelect
                        required
                        label="Giới tính"
                        placeholder="Chọn giới tính"
                        value={formData.gender ?? ""}
                        options={genderOptions}
                        onChange={(value) =>
                            updateField("gender", value === "" ? null : value)
                        }
                    />
                </div>

                {selectedRoleDetails && (
                    <div className="mb-4 rounded-2xl bg-gray-50 p-5">
                        <SectionTitle title={selectedRoleDetails.title} />

                        <div className="grid gap-4 md:grid-cols-2">
                            <div>
                                <InputField
                                    required
                                    label={selectedRoleDetails.codeLabel}
                                    placeholder={
                                        selectedRoleDetails.codePlaceholder
                                    }
                                    value={formData.userCode}
                                    onChange={(value) =>
                                        updateField("userCode", value)
                                    }
                                />
                                {error instanceof ValidationError &&
                                    error.get("userCode") && (
                                        <p className="text-xs text-red-500">
                                            {error.get("userCode")}
                                        </p>
                                    )}
                            </div>

                            <div>
                                <FormSelect
                                    required
                                    label="Khoa"
                                    value={formData.facultyId}
                                    onChange={changeFaculty}
                                    options={faculties.map((f) => ({
                                        label: f.name,
                                        value: f.id,
                                    }))}
                                    placeholder="Chọn khoa"
                                />

                                {error instanceof ValidationError &&
                                    error.get("facultyId") && (
                                        <p className="text-xs text-red-500">
                                            {error.get("facultyId")}
                                        </p>
                                    )}
                            </div>
                        </div>

                        {formData.userRole === "Student" && (
                            <div className="grid gap-4 md:grid-cols-2">
                                <div>
                                    <FormSelect
                                        required
                                        label="Ngành"
                                        value={formData.majorId}
                                        onChange={(value) =>
                                            updateField("majorId", value)
                                        }
                                        options={majors.map((m) => ({
                                            label: m.name,
                                            value: m.id,
                                        }))}
                                        placeholder={
                                            isLoadingMajors
                                                ? "Đang tải ngành"
                                                : "Chọn ngành"
                                        }
                                        disabled={
                                            !formData.facultyId ||
                                            isLoadingMajors
                                        }
                                    />
                                    {error instanceof ValidationError &&
                                        error.get("majorId") && (
                                            <p className="text-xs text-red-500">
                                                {error.get("majorId")}
                                            </p>
                                        )}
                                </div>
                            </div>
                        )}
                    </div>
                )}

                <div className="mt-6 flex justify-end gap-3 border-t border-gray-200 pt-5">
                    <Link
                        to="/users"
                        className="rounded-xl border border-gray-200 px-5 py-2.5 text-sm font-semibold text-gray-600 hover:bg-gray-50"
                    >
                        Hủy
                    </Link>

                    <button
                        type="submit"
                        disabled={isPending}
                        className="rounded-xl bg-blue-600 px-5 py-2.5 text-sm font-semibold text-white hover:bg-blue-700 disabled:cursor-not-allowed disabled:opacity-60"
                    >
                        Tạo người dùng
                    </button>
                </div>
            </form>
        </div>
    );
};

export default CreateUserPage;
