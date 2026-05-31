type StatCardProps = {
    label: string;
    value: number;
};

const StatCard = ({ label, value }: StatCardProps) => {
    return (
        <div className="rounded-2xl border bg-white p-5 shadow-sm">
            <div className="text-sm text-gray-500">{label}</div>

            <div className="mt-2 text-2xl font-bold text-gray-900">{value}</div>
        </div>
    );
};

export default StatCard;
