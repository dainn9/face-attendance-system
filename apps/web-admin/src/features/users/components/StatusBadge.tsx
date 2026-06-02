const StatusBadge = ({ isActive }: { isActive: boolean }) => {
    const styles = isActive
        ? "bg-emerald-50 text-emerald-700"
        : "bg-red-50 text-red-700";

    return (
        <span
            className={`rounded-full px-2.5 py-1 text-xs font-semibold ${styles}`}
        >
            {isActive ? "Active" : "Inactive"}
        </span>
    );
};

export default StatusBadge;
