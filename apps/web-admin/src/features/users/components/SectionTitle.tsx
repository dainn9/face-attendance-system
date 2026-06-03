type SectionTitleProps = {
    title: string;
    className?: string;
};

const SectionTitle = ({ title, className = "" }: SectionTitleProps) => (
    <div
        className={`mb-4 border-b border-gray-200 pb-2 text-sm font-semibold text-gray-900 ${className}`}
    >
        {title}
    </div>
);

export default SectionTitle;
