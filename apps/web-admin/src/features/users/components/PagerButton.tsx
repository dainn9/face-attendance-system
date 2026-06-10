type PagerButtonProps = {
    children: React.ReactNode;
    active?: boolean;
    disabled?: boolean;
    onClick?: () => void;
};

const PagerButton = ({
    children,
    active = false,
    disabled = false,
    onClick,
}: PagerButtonProps) => (
    <button
        type="button"
        disabled={disabled}
        onClick={onClick}
        className={`flex size-8 items-center justify-center rounded-lg border text-sm ${
            active
                ? "border-blue-600 bg-blue-600 text-white"
                : "border-gray-200 bg-white text-gray-500 hover:bg-gray-50"
        } disabled:cursor-not-allowed disabled:opacity-40`}
    >
        {children}
    </button>
);

export default PagerButton;
