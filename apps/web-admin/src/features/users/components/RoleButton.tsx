import type { UserRole } from "../types/user.types";

type RoleButtonProps = {
    userRole: UserRole;
    currentRole: UserRole;
    icon: React.ReactNode;
    onClick: () => void;
};

const RoleButton = ({
    userRole,
    currentRole,
    icon,
    onClick,
}: RoleButtonProps) => {
    const isSelected = userRole === currentRole;

    const selectedStyles = {
        Student: "border-green-300 bg-green-50 text-green-700",
        Lecturer: "border-blue-300 bg-blue-50 text-blue-700",
        Admin: "border-purple-300 bg-purple-50 text-purple-700",
    };

    return (
        <button
            type="button"
            onClick={onClick}
            className={`flex items-center justify-center gap-2 rounded-xl border px-4 py-3 text-sm font-semibold transition ${
                isSelected
                    ? selectedStyles[userRole]
                    : "border-gray-200 bg-white text-gray-500 hover:bg-gray-50"
            }`}
        >
            {icon}
            {userRole}
        </button>
    );
};

export default RoleButton;
