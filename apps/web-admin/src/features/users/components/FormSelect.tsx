type SelectOption<T extends string | number> = {
    label: string;
    value: T;
};

type FormSelectProps<T extends string | number> = {
    label: string;
    value: T | "";
    placeholder?: string;
    required?: boolean;
    disabled?: boolean;
    options: SelectOption<T>[];
    onChange: (value: T | "") => void;
};

const FormSelect = <T extends string | number>({
    label,
    value,
    placeholder = "Chọn",
    required = false,
    disabled = false,
    options,
    onChange,
}: FormSelectProps<T>) => {
    return (
        <div className="mb-4">
            <label className="mb-1 block text-sm font-medium text-gray-600">
                {label}
                {required && <span className="text-red-500"> *</span>}
            </label>

            <select
                disabled={disabled}
                value={value === "" ? "" : String(value)}
                onChange={(e) => {
                    const selected = options.find(
                        (x) => String(x.value) === e.target.value,
                    );

                    onChange(selected?.value ?? "");
                }}
                className="w-full rounded-xl border border-gray-200 bg-white px-3 py-2 text-sm text-gray-700 outline-none focus:border-blue-500 disabled:cursor-not-allowed disabled:bg-gray-100 disabled:text-gray-400"
            >
                <option value="">{placeholder}</option>

                {options.map((option) => (
                    <option
                        key={String(option.value)}
                        value={String(option.value)}
                    >
                        {option.label}
                    </option>
                ))}
            </select>
        </div>
    );
};

export default FormSelect;
