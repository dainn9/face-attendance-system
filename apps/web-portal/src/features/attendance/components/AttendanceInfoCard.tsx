export type AttendanceInfoItem = readonly [label: string, value: string];

type AttendanceInfoCardProps = {
    items: AttendanceInfoItem[];
    variant?: "default" | "success" | "closed";
};

const AttendanceInfoCard = ({
    items,
    variant = "default",
}: AttendanceInfoCardProps) => {
    if (variant === "success") {
        return (
            <div className="mx-auto mt-6 grid max-w-xl gap-3 text-left sm:grid-cols-3">
                {items.map(([label, value]) => (
                    <div
                        key={label}
                        className="rounded-lg border border-emerald-100 bg-emerald-50 p-4"
                    >
                        <p className="text-xs font-medium text-emerald-700">
                            {label}
                        </p>
                        <p className="mt-1 text-sm font-semibold text-emerald-950">
                            {value}
                        </p>
                    </div>
                ))}
            </div>
        );
    }

    if (variant === "closed") {
        return (
            <div className="mt-6 grid gap-3 text-left sm:grid-cols-2">
                {items.map(([label, value]) => (
                    <div
                        key={label}
                        className="rounded-lg border border-gray-100 bg-gray-50 p-4"
                    >
                        <p className="text-xs font-medium uppercase tracking-wide text-gray-500">
                            {label}
                        </p>
                        <p className="mt-1 text-sm font-semibold text-gray-900">
                            {value}
                        </p>
                    </div>
                ))}
            </div>
        );
    }

    return (
        <section className="mx-auto grid w-full max-w-3xl gap-3 rounded-lg border border-blue-100 bg-blue-50 p-4 sm:grid-cols-2 lg:grid-cols-5">
            {items.map(([label, value]) => (
                <div key={label} className="rounded-md bg-white p-3">
                    <p className="text-xs font-medium text-blue-700">{label}</p>
                    <p className="mt-1 truncate text-sm font-semibold text-gray-900">
                        {value}
                    </p>
                </div>
            ))}
        </section>
    );
};

export default AttendanceInfoCard;
