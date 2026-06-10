type InputFieldProps = {
    label: string;
    value: string;
    placeholder?: string;
    type?: string;
    required?: boolean;
    disabled?: boolean;
    onChange: (value: string) => void;
};

const InputField = ({
    label,
    value,
    placeholder,
    type = "text",
    required = false,
    disabled = false,
    onChange,
}: InputFieldProps) => (
    <div className="mb-4">
        <label className="mb-1 block text-sm font-medium text-gray-600">
            {label}
            {required && <span className="text-red-500"> *</span>}
        </label>

        <input
            type={type}
            value={value}
            placeholder={placeholder}
            disabled={disabled}
            onChange={(e) => onChange(e.target.value)}
            className="w-full rounded-xl border border-gray-200 bg-white px-3 py-2 text-sm text-gray-700 outline-none focus:border-blue-500 disabled:cursor-not-allowed disabled:bg-gray-100 disabled:text-gray-400"
        />
    </div>
);

export default InputField;
