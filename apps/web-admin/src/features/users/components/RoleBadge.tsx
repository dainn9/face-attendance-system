import type { UserRole } from "../types/user.types";

const RoleBadge = ({ role }: { role: UserRole }) => {
    const styles = {
        Student: "bg-green-50 text-green-700",
        Lecturer: "bg-blue-50 text-blue-700",
        Admin: "bg-purple-50 text-purple-700",
    };

    return (
        <span
            className={`rounded-full px-2.5 py-1 text-xs font-semibold ${styles[role]}`}
        >
            {role}
        </span>
    );
};

export default RoleBadge;
