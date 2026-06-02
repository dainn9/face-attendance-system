type Props = {
    placeholder: string;
    value?: string;
    onSelect?: (value: string) => void;
    options: {
        label: string;
        value: string;
    }[];
};

const FilterSelect = ({ placeholder, options, value, onSelect }: Props) => {
    return (
        <select
            className="rounded-xl border border-gray-200 bg-white px-3 py-2 text-sm text-gray-600 outline-none focus:border-blue-500"
            value={value}
            onChange={(e) => onSelect?.(e.target.value)}
        >
            <option value="">{placeholder}</option>

            {options.map((option) => (
                <option key={option.value} value={option.value}>
                    {option.label}
                </option>
            ))}
        </select>
    );
};

export default FilterSelect;
